using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using System.ServiceModel;
using System.ServiceModel.Description;

using SEModAPI.Support;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using SEModAPIExtensions.API;
using SEModAPIExtensions.API.IPC;
using System.ServiceModel.Web;

namespace SEServerExtender
{
	public static class Program
	{
		static SEServerExtender m_serverExtenderForm;
		static Server m_server;

		/// <summary>
		/// Main entry point of the application
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			//Setup error handling for unmanaged exceptions
			AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
			Application.ThreadException += Application_ThreadException;
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

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

					Console.WriteLine("Name-Value Arg: name='" + argName + "' value='" + argValue + "'");

					if (argName.ToLower().Equals("instance"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.instanceName = argValue;
					}
					if (argName.ToLower().Equals("gamepath"))
					{
						if (argValue[argValue.Length - 1] == '"')
							argValue = argValue.Substring(0, argValue.Length - 1);
						extenderArgs.gamePath = argValue;
					}
					if (argName.ToLower().Equals("autosave"))
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
					if (argName.ToLower().Equals("wcfport"))
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
					if (argName.ToLower().Equals("path"))
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
				m_server.Init();
				if (extenderArgs.autoStart)
				{
					m_server.StartServer();
				}

				if (!extenderArgs.noGUI)
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					m_serverExtenderForm = new SEServerExtender(m_server);
					Application.Run(m_serverExtenderForm);
				}
			}
			catch (AutoException eEx)
			{
				if(!extenderArgs.noConsole)
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
	}
}
