using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
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

using SEModAPIInternal.API;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

using VRage.Common.Utils;
using SEModAPIExtensions.API.IPC;

namespace SEModAPIExtensions.API
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

	[ServiceContract]
	public interface IServerServiceContract
	{
		[OperationContract]
		Server GetServer();

		[OperationContract]
		void StartServer();

		[OperationContract]
		void StopServer();

		[OperationContract]
		void LoadServerConfig();

		[OperationContract]
		void SaveServerConfig();

		[OperationContract]
		void SetAutosaveInterval(double interval);
	}

	[ServiceBehavior(
		ConcurrencyMode = ConcurrencyMode.Single,
		IncludeExceptionDetailInFaults = true,
		IgnoreExtensionDataObject = true
	)]
	public class ServerService : IServerServiceContract
	{
		public Server GetServer()
		{
			return Server.Instance;
		}

		public void StartServer()
		{
			Server.Instance.StartServer();
		}

		public void StopServer()
		{
			Server.Instance.StopServer();
		}

		public void LoadServerConfig()
		{
			Server.Instance.LoadServerConfig();
		}

		public void SaveServerConfig()
		{
			Server.Instance.SaveServerConfig();
		}

		public void SetAutosaveInterval(double interval)
		{
			Server.Instance.AutosaveInterval = interval;
		}
	}

	[DataContract(
		Name = "ServerProxy",
		IsReference = true
	)]
	public class Server
	{
		#region "Attributes"

		private static Server m_instance;
		private static bool m_isInitialized;
		private static Thread m_runServerThread;
		private static bool m_isServerRunning;
		private static DateTime m_lastRestart;
		private static int m_restartLimit;

		private CommandLineArgs m_commandLineArgs;
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
		private System.Timers.Timer m_autosaveTimer;

		#endregion

		#region "Constructors and Initializers"

		protected Server()
		{
			if (m_isInitialized)
				return;

			m_lastRestart = DateTime.Now;
			m_restartLimit = 3;

			m_pluginMainLoop = new System.Timers.Timer();
			m_pluginMainLoop.Interval = 200;
			m_pluginMainLoop.Elapsed += PluginManagerMain;

			m_autosaveTimer = new System.Timers.Timer();
			m_autosaveTimer.Interval = 120000;
			m_autosaveTimer.Elapsed += AutoSaveMain;

			Uri baseAddress = new Uri(InternalService.BaseURI + "Server/");
			ServiceHost selfHost = new ServiceHost(typeof(ServerService), baseAddress);

			try
			{
				selfHost.AddServiceEndpoint(typeof(IServerServiceContract), new WSHttpBinding(), "ServerService");
				ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				smb.HttpGetEnabled = true;
				selfHost.Description.Behaviors.Add(smb);
				selfHost.Open();
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				selfHost.Abort();
			}

			Console.WriteLine("Finished creating server!");
		}

		private bool SetupGameInstallation()
		{
			try
			{
				string gamePath = m_commandLineArgs.gamePath;
				if (gamePath.Length > 0)
				{
					if (!GameInstallationInfo.IsValidGamePath(gamePath))
						return false;
					m_gameInstallationInfo = new GameInstallationInfo(gamePath);
				}
				else
				{
					m_gameInstallationInfo = new GameInstallationInfo();
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			if (m_gameInstallationInfo != null)
				return true;
			else
				return false;
		}

		private bool SetupManagers()
		{
			m_serverWrapper = ServerAssemblyWrapper.Instance;
			m_pluginManager = PluginManager.Instance;
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
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}

			return true;
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		public static Server Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new Server();

				return m_instance;
			}
		}

		[DataMember]
		public bool IsRunning
		{
			get { return m_isServerRunning; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		public CommandLineArgs CommandLineArgs
		{
			get { return m_commandLineArgs; }
			set { m_commandLineArgs = value; }
		}

		[IgnoreDataMember]
		public DedicatedConfigDefinition Config
		{
			get { return m_dedicatedConfigDefinition; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		public string InstanceName
		{
			get { return m_commandLineArgs.instanceName; }
			set { m_commandLineArgs.instanceName = value; }
		}

		[DataMember]
		public double AutosaveInterval
		{
			get { return m_autosaveTimer.Interval; }
			set { m_autosaveTimer.Interval = value; }
		}

		#endregion

		#region "Methods"

		public void Init()
		{
			if (m_isInitialized)
				return;

			bool setupResult = true;
			setupResult &= SetupGameInstallation();
			setupResult &= SetupManagers();
			setupResult &= ProcessCommandLineArgs();

			if (!setupResult)
				return;

			m_isInitialized = true;
		}

		private void PluginManagerMain(object sender, EventArgs e)
		{
			if (!Server.Instance.IsRunning)
			{
				m_pluginMainLoop.Stop();
				return;
			}

			if (m_pluginManager == null)
			{
				m_pluginMainLoop.Stop();
				return;
			}

			if (!m_pluginManager.Initialized)
			{
				if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				{
					m_pluginManager.Init();
				}
			}
			else
			{
				m_pluginManager.Update();
			}
		}
		
		private void AutoSaveMain(object sender, EventArgs e)
		{
			if (!Server.Instance.IsRunning)
			{
				m_autosaveTimer.Stop();
				return;
			}

			WorldManager.Instance.SaveWorld();
		}

		private void RunServer()
		{
			if (m_restartLimit < 0)
				return;

			m_lastRestart = DateTime.Now;

			try
			{
				bool result = m_serverWrapper.StartServer(m_commandLineArgs.worldName, m_commandLineArgs.instanceName, !m_commandLineArgs.noConsole);

				m_isServerRunning = false;

				m_pluginMainLoop.Stop();
				m_autosaveTimer.Stop();

				m_pluginManager.Shutdown();

				Console.WriteLine("Server has stopped running");
				/*
				if (!result)
				{
					LogManager.APILog.WriteLineAndConsole("Server crashed, attempting auto-restart ...");

					TimeSpan timeSinceLastRestart = DateTime.Now - m_lastRestart;

					//Reset the restart limit if the server has been running for more than 5 minutes before the crash
					if (timeSinceLastRestart.TotalMinutes > 5)
						m_restartLimit = 3;

					m_restartLimit--;

					m_isServerRunning = true;
					SectorObjectManager.Instance.IsShutDown = false;

					m_runServerThread = new Thread(new ThreadStart(this.RunServer));
					m_runServerThread.Start();
				}*/
			}
			catch(Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void StartServer()
		{
			try
			{
				if (!m_isInitialized)
					return;
				if (m_isServerRunning)
					return;

				PluginManager.Instance.LoadPlugins(m_commandLineArgs.instanceName);
				m_pluginMainLoop.Start();
				m_autosaveTimer.Start();

				m_isServerRunning = true;

				m_runServerThread = new Thread(new ThreadStart(this.RunServer));
				m_runServerThread.Start();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				m_isServerRunning = false;
			}
		}

		public void StopServer()
		{
			m_pluginMainLoop.Stop();
			m_autosaveTimer.Stop();

			m_pluginManager.Shutdown();

			//m_serverWrapper.StopServer();
			m_runServerThread.Interrupt();

			m_isServerRunning = false;

			Console.WriteLine("Server has been stopped");
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
