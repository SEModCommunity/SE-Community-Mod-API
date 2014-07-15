using System;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using SEModAPI.Support;
using SEModAPIInternal.Support;

namespace SEServerExtender
{
	public static class Program
	{
		public struct CommandLineArgs
		{
			public bool autoStart;
			public string worldName;
			public string instanceName;
			public bool noGUI;
			public bool noConsole;
			public bool debug;
			public string gamePath;
		}

		static SEServerExtender m_serverExtenderForm;
		static Server m_server;

		/// <summary>
		/// Main entry point of the application
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			try
			{
				//Setup error handling for unmanaged exceptions
				AppDomain.CurrentDomain.UnhandledException += AppDomain_UnhandledException;
				Application.ThreadException += Application_ThreadException;
				Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

				//Setup the default args
				CommandLineArgs extenderArgs = new CommandLineArgs();
				extenderArgs.autoStart = false;
				extenderArgs.worldName = "";
				extenderArgs.instanceName = "";
				extenderArgs.noGUI = false;
				extenderArgs.noConsole = false;
				extenderArgs.debug = false;
				extenderArgs.gamePath = "";

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
							extenderArgs.instanceName = argValue;
						}
						if (argName.ToLower().Equals("gamepath"))
						{
							extenderArgs.gamePath = argValue;
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
							extenderArgs.noGUI = true;

							//Implies nogui and autostart
							extenderArgs.noGUI = true;
							extenderArgs.autoStart = true;
						}
						if (arg.ToLower().Equals("debug"))
						{
							extenderArgs.debug = true;
						}
					}
				}

				m_server = new Server(extenderArgs);
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
				Console.WriteLine("AutoException - " + eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString());
				MessageBox.Show(eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (TargetInvocationException ex)
			{
				Console.WriteLine("TargetInvocationException - " + ex.ToString() + "\n\r" + ex.InnerException.ToString());
				MessageBox.Show(ex.ToString() + "\n\r" + ex.InnerException.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Exception - " + ex.ToString());
				MessageBox.Show(ex.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			if (LogManager.GameLog != null && LogManager.GameLog.LogEnabled)
			{
				LogManager.GameLog.WriteLine("Application.ThreadException");
				LogManager.GameLog.WriteLine(e.Exception);
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
			if (LogManager.GameLog != null && LogManager.GameLog.LogEnabled)
			{
				LogManager.GameLog.WriteLine("AppDomain.UnhandledException");
				LogManager.GameLog.WriteLine((Exception)e.ExceptionObject);
			}
		}

	}
}
