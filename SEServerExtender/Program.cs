using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.ServiceProcess;

using SEModAPI.Support;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using SEModAPIExtensions.API;
using SEModAPIExtensions.API.IPC;

namespace SEServerExtender
{
	public static class Program
	{
		public class WindowsService : ServiceBase
		{
			public WindowsService()
			{
				this.ServiceName = "SEServerExtender";
				this.CanPauseAndContinue = false;
				this.CanStop = true;
				this.AutoLog = true;
			}

			protected override void OnStart(string[] args)
			{
				LogManager.APILog.WriteLine("Starting SEServerExtender Service with " + args.Length.ToString() + " arguments ...");

				Program.Start(args);
			}

			protected override void OnStop()
			{
				LogManager.APILog.WriteLine("Stopping SEServerExtender Service ...");

				Program.Stop();
			}
		}

		static SEServerExtender m_serverExtenderForm;
		static Server m_server;

		/// <summary>
		/// Main entry point of the application
		/// </summary>
		static void Main(string[] args)
		{
			if (!Environment.UserInteractive)
			{
				using (var service = new WindowsService())
				{
					ServiceBase.Run(service);
				}
			}
			else
			{
				Start(args);
			}
		}

		private static void Start(string[] args)
		{
			//Setup error handling for unmanaged exceptions
			AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

			LogManager.APILog.WriteLine("Starting SEServerExtender with " + args.Length.ToString() + " arguments ...");

			CommandLineArgs extenderArgs = new CommandLineArgs();

			//Setup the default args
			extenderArgs.autoStart = false;
			extenderArgs.worldName = "";
			extenderArgs.instanceName = "";
			extenderArgs.noGUI = false;
			extenderArgs.noConsole = false;
			extenderArgs.debug = false;
			extenderArgs.gamePath = "";
			extenderArgs.noWCF = false;
			extenderArgs.autosave = 0;
			extenderArgs.wcfPort = 0;
			extenderArgs.path = "";

			//Process the args
			foreach (string arg in args)
			{
				if (arg.Split('=').Length > 1)
				{
					string argName = arg.Split('=')[0];
					string argValue = arg.Split('=')[1];

					if (argName.ToLower().Equals("instance"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.instanceName = argValue;
					}
					else if (argName.ToLower().Equals("gamepath"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.gamePath = argValue;
					}
					else if (argName.ToLower().Equals("autosave"))
					{
						try
						{
							extenderArgs.autosave = int.Parse(argValue);
						}
						catch (Exception)
						{
							//Do nothing
						}
					}
					else if (argName.ToLower().Equals("wcfport"))
					{
						try
						{
							extenderArgs.wcfPort = ushort.Parse(argValue);
						}
						catch (Exception)
						{
							//Do nothing
						}
					}
					else if (argName.ToLower().Equals("path"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.path = argValue;
					}
				}
				else
				{
					if (arg.ToLower().Equals("autostart"))
					{
						extenderArgs.autoStart = true;
					}
					if (arg.ToLower().Equals("nogui"))
					{
						extenderArgs.noGUI = true;

						//Implies autostart
						extenderArgs.autoStart = true;
					}
					if (arg.ToLower().Equals("noconsole"))
					{
						extenderArgs.noConsole = true;

						//Implies nogui and autostart
						extenderArgs.noGUI = true;
						extenderArgs.autoStart = true;
					}
					if (arg.ToLower().Equals("debug"))
					{
						extenderArgs.debug = true;
					}
					if (arg.ToLower().Equals("nowcf"))
					{
						extenderArgs.noWCF = true;
					}
				}
			}

			if (extenderArgs.noWCF)
				extenderArgs.wcfPort = 0;

			if (!string.IsNullOrEmpty(extenderArgs.path))
			{
				extenderArgs.instanceName = "";
			}

			if (!Environment.UserInteractive)
			{
				extenderArgs.noConsole = true;
				extenderArgs.noGUI = true;
				extenderArgs.autoStart = true;
			}

			if (extenderArgs.debug)
				SandboxGameAssemblyWrapper.IsDebugging = true;

			try
			{
				bool unitTestResult = BasicUnitTestManager.Instance.Run();
				if (!unitTestResult)
					SandboxGameAssemblyWrapper.IsInSafeMode = true;

				m_server = Server.Instance;
				m_server.CommandLineArgs = extenderArgs;
				m_server.IsWCFEnabled = !extenderArgs.noWCF;
				m_server.WCFPort = extenderArgs.wcfPort;
				m_server.Init();

				ChatManager.ChatCommand guiCommand = new ChatManager.ChatCommand();
				guiCommand.command = "gui";
				guiCommand.callback = ChatCommand_GUI;
				ChatManager.Instance.RegisterChatCommand(guiCommand);

				if (extenderArgs.autoStart)
				{
					m_server.StartServer();
				}

				if (!extenderArgs.noGUI)
				{
					Thread uiThread = new Thread(new ThreadStart(StartGUI));
					uiThread.SetApartmentState(ApartmentState.STA);
					uiThread.Start();
				}
			}
			catch (AutoException eEx)
			{
				if (!extenderArgs.noConsole)
					Console.WriteLine("AutoException - " + eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString());
				if (!extenderArgs.noGUI)
					MessageBox.Show(eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (extenderArgs.noConsole && extenderArgs.noGUI)
					throw eEx.GetBaseException();
			}
			catch (TargetInvocationException ex)
			{
				if (!extenderArgs.noConsole)
					Console.WriteLine("TargetInvocationException - " + ex.ToString() + "\n\r" + ex.InnerException.ToString());
				if (!extenderArgs.noGUI)
					MessageBox.Show(ex.ToString() + "\n\r" + ex.InnerException.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (extenderArgs.noConsole && extenderArgs.noGUI)
					throw ex;
			}
			catch (Exception ex)
			{
				if (!extenderArgs.noConsole)
					Console.WriteLine("Exception - " + ex.ToString());
				if (!extenderArgs.noGUI)
					MessageBox.Show(ex.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);

				if (extenderArgs.noConsole && extenderArgs.noGUI)
					throw ex;
			}
		}

		private static void Stop()
		{
			if(m_server != null && m_server.IsRunning)
				m_server.StopServer();
			if (m_serverExtenderForm != null && m_serverExtenderForm.Visible == true)
				m_serverExtenderForm.Close();
		}

		static void Application_ThreadException(Object sender, ThreadExceptionEventArgs e)
		{
			Console.WriteLine("Application.ThreadException - " + e.Exception.ToString());

			if (LogManager.APILog != null && LogManager.APILog.LogEnabled)
			{
				LogManager.APILog.WriteLine("Application.ThreadException");
				LogManager.APILog.WriteLine(e.Exception);
			}
			if (LogManager.ErrorLog != null && LogManager.ErrorLog.LogEnabled)
			{
				LogManager.ErrorLog.WriteLine("Application.ThreadException");
				LogManager.ErrorLog.WriteLine(e.Exception);
			}
		}

		static void AppDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine("AppDomain.UnhandledException - " + ((Exception)e.ExceptionObject).ToString());

			if (LogManager.APILog != null && LogManager.APILog.LogEnabled)
			{
				LogManager.APILog.WriteLine("AppDomain.UnhandledException");
				LogManager.APILog.WriteLine((Exception)e.ExceptionObject);
			}
			if (LogManager.ErrorLog != null && LogManager.ErrorLog.LogEnabled)
			{
				LogManager.ErrorLog.WriteLine("AppDomain.UnhandledException");
				LogManager.ErrorLog.WriteLine((Exception)e.ExceptionObject);
			}
		}

		static void ChatCommand_GUI(ChatManager.ChatEvent chatEvent)
		{
			Thread uiThread = new Thread(new ThreadStart(StartGUI));
			uiThread.SetApartmentState(ApartmentState.STA);
			uiThread.Start();
		}

		[STAThread]
		static void StartGUI()
		{
			if (!Environment.UserInteractive)
				return;

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (m_serverExtenderForm == null || m_serverExtenderForm.IsDisposed)
				m_serverExtenderForm = new SEServerExtender(m_server);
			else if (m_serverExtenderForm.Visible)
				return;

			Application.Run(m_serverExtenderForm);
		}
	}
}
