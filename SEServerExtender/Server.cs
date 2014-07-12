using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;
using SEModAPI.Support;

using SEModAPIExtensions.API;

using SEModAPIInternal.API;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

using VRage.Common.Utils;

namespace SEServerExtender
{
	public class Server
	{
		#region "Attributes"

		private static Thread m_runServerThread;
		private static bool m_isServerRunning;
		private static Program.CommandLineArgs m_commandLineArgs;
		private DedicatedConfigDefinition m_dedicatedConfigDefinition;
		private GameInstallationInfo m_gameInstallationInfo;

		//Managers
		private PluginManager m_pluginManager;
		private SandboxGameAssemblyWrapper m_gameAssemblyWrapper;
		private FactionsManager m_factionsManager;
		private ServerAssemblyWrapper m_serverWrapper;
		private LogManager m_logManager;
		private EntityEventManager m_entityEventManager;
		private ChatManager m_chatManager;

		//Timers
		private System.Timers.Timer m_pluginMainLoop;

		#endregion

		#region "Constructors and Initializers"

		public Server(Program.CommandLineArgs args)
		{
			m_commandLineArgs = args;

			SetupGameInstallation();
			SetupManagers();
			ProcessCommandLineArgs();

			m_pluginMainLoop = new System.Timers.Timer();
			m_pluginMainLoop.Interval = 100;
			m_pluginMainLoop.Elapsed += PluginManagerMain;

			PluginManager.GetInstance().LoadPlugins(m_commandLineArgs.instanceName);
		}

		private bool SetupGameInstallation()
		{
			try
			{
				m_gameInstallationInfo = new GameInstallationInfo();
			}
			catch (AutoException)
			{
				string gamePath = m_commandLineArgs.gamePath;
				if (!Directory.Exists(gamePath))
				{
					return false;
				}
				m_gameInstallationInfo = new GameInstallationInfo(gamePath);
			}

			if (m_gameInstallationInfo != null)
				return true;
			else
				return false;
		}

		private bool SetupManagers()
		{
			m_serverWrapper = ServerAssemblyWrapper.GetInstance();
			m_pluginManager = PluginManager.GetInstance();
			m_gameAssemblyWrapper = SandboxGameAssemblyWrapper.Instance;
			m_factionsManager = FactionsManager.Instance;
			m_logManager = LogManager.Instance;
			m_entityEventManager = EntityEventManager.Instance;
			m_chatManager = ChatManager.Instance;

			return true;
		}

		private bool ProcessCommandLineArgs()
		{
			try
			{
				if (m_commandLineArgs.autoStart)
				{
					Console.WriteLine("Auto-Start enabled");
				}
				if (m_commandLineArgs.instanceName.Length != 0)
				{
					Console.WriteLine("Common instance pre-selected: '" + m_commandLineArgs.instanceName + "'");
				}
				if (m_commandLineArgs.noGUI)
				{
					Console.WriteLine("No GUI enabled");
				}
				if (m_commandLineArgs.debug)
				{
					Console.WriteLine("Debugging enabled");
					SandboxGameAssemblyWrapper.IsDebugging = true;
				}
			}
			catch (AutoException)
			{
				return false;
			}

			return true;
		}

		#endregion

		#region "Properties"

		public bool IsRunning
		{
			get { return m_isServerRunning; }
		}

		public Program.CommandLineArgs CommandLineArgs
		{
			get { return m_commandLineArgs; }
		}

		public DedicatedConfigDefinition Config
		{
			get { return m_dedicatedConfigDefinition; }
		}

		public string InstanceName
		{
			get { return m_commandLineArgs.instanceName; }
			set { m_commandLineArgs.instanceName = value; }
		}

		#endregion

		#region "Methods"

		private void PluginManagerMain(object sender, EventArgs e)
		{
			if (!m_pluginManager.Initialized)
			{
				if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				{
					m_pluginManager.Init();
				}
			}
			else
			{
				if (m_commandLineArgs.noGUI)
				{
					List<BaseEntity> entityList = SectorObjectManager.Instance.GetTypedInternalData<BaseEntity>();
				}
				m_pluginManager.Update();
			}
		}

		private void RunServer()
		{
			m_serverWrapper.StartServer(m_commandLineArgs.worldName, m_commandLineArgs.instanceName);

			m_isServerRunning = false;

			Console.WriteLine("Server has stopped running");
		}

		public void StartServer()
		{
			try
			{
				if (m_isServerRunning)
					return;

				PluginManager.GetInstance().LoadPlugins(m_commandLineArgs.instanceName);
				m_pluginMainLoop.Start();

				m_isServerRunning = true;

				m_runServerThread = new Thread(new ThreadStart(this.RunServer));
				m_runServerThread.Start();
			}
			catch (Exception ex)
			{
				m_isServerRunning = false;
			}
		}

		public void StopServer()
		{
			m_pluginMainLoop.Stop();

			m_runServerThread.Abort();

			m_isServerRunning = false;
		}

		public void LoadServerConfig()
		{
			string appdata = m_gameAssemblyWrapper.GetUserDataPath(m_commandLineArgs.instanceName);
			FileInfo fileInfo = new FileInfo(appdata + @"\SpaceEngineers-Dedicated.cfg");
			if (fileInfo.Exists)
			{
				MyConfigDedicatedData config = DedicatedConfigDefinition.Load(fileInfo);
				m_dedicatedConfigDefinition = new DedicatedConfigDefinition(config);
			}
		}

		public void SaveServerConfig()
		{
			string appdata = m_gameAssemblyWrapper.GetUserDataPath(m_commandLineArgs.instanceName);
			FileInfo fileInfo = new FileInfo(appdata + @"\SpaceEngineers-Dedicated.cfg");
			if (m_dedicatedConfigDefinition != null)
				m_dedicatedConfigDefinition.Save(fileInfo);
		}

		#endregion
	}
}
