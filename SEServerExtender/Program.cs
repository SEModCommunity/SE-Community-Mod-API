using System;
using System.Reflection;
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
				//Setup the default args
				CommandLineArgs extenderArgs = new CommandLineArgs();
				extenderArgs.autoStart = false;
				extenderArgs.worldName = "";
				extenderArgs.instanceName = "TestService";
				extenderArgs.noGUI = false;
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
				MessageBox.Show(eEx.AdditionnalInfo + "\n\r" + eEx.GetDebugString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (TargetInvocationException ex)
			{
				MessageBox.Show(ex.ToString() + "\n\r" + ex.InnerException.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString(), @"SEServerExtender", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}
	}
}
