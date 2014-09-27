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
using System.ServiceModel.Web;
using System.IdentityModel.Selectors;
using System.ServiceModel.Security;

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
		public bool noWCF;
		public bool noSLWCF;
		public ushort wcfPort;
		public ushort slWcfPort;
		public int autosave;
		public string path;
	}

	[ServiceContract]
	public interface IServerServiceContract
	{
		#region "Methods"

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

		#endregion
	}

	[ServiceBehavior(
		ConcurrencyMode = ConcurrencyMode.Single,
		IncludeExceptionDetailInFaults = true,
		IgnoreExtensionDataObject = true
	)]
	public class ServerService : IServerServiceContract
	{
		#region "Methods"
		
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

		#endregion
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
		private static bool m_isWCFEnabled;
		private static bool m_isSLWCFEnabled;

		private CommandLineArgs m_commandLineArgs;
		private DedicatedConfigDefinition m_dedicatedConfigDefinition;
		private GameInstallationInfo m_gameInstallationInfo;
		private ServiceHost m_serverHost;
		private ServiceHost m_baseHost;
		private WebServiceHost m_webHost;
		private WebServiceHost m_webHostSilverlight;

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
			m_autosaveTimer.Interval = 300000;
			m_autosaveTimer.Elapsed += AutoSaveMain;

			m_isWCFEnabled = true;

			Console.WriteLine("Finished creating server!");
		}

		private bool SetupServerService()
		{
			try
			{
				m_serverHost = CreateServiceHost(typeof(ServerService), typeof(IServerServiceContract), "Server/", "ServerService");
				m_serverHost.Open();
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				m_serverHost.Abort();
				return false;
			}

			return true;
		}

		private bool SetupMainService()
		{
			try
			{
				m_baseHost = CreateServiceHost(typeof(InternalService), typeof(IInternalServiceContract), "", "InternalService");
				m_baseHost.Open();
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				m_baseHost.Abort();
				return false;
			}

			return true;
		}

		private bool SetupSLServerService()
		{
			try
			{
				m_serverHost = CreateSLServiceHost(typeof(ServerService), typeof(IServerServiceContract), "Server/", "ServerService");
				m_serverHost.Open();
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				m_serverHost.Abort();
				return false;
			}

			return true;
		}

		private bool SetupSLMainService()
		{
			try
			{
				m_baseHost = CreateSLServiceHost(typeof(InternalService), typeof(IInternalServiceContract), "", "InternalService");
				m_baseHost.Open();
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				m_baseHost.Abort();
				return false;
			}

			return true;
		}

		private bool SetupWebService()
		{
			Uri webServiceAddress = new Uri("http://localhost:" + WCFPort.ToString() + "/SEServerExtender/Web/");
			m_webHost = new WebServiceHost(typeof(WebService), webServiceAddress);
			try
			{
				//WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
				//binding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
				//binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
				//ServiceEndpoint endpoint = m_webHost.AddServiceEndpoint(typeof(IWebServiceContract), binding, "WebService");
				//m_webHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
				//m_webHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserNameValidator();
				EnableCorsBehavior ecb = new EnableCorsBehavior();
				m_webHost.Description.Behaviors.Add(ecb);
				m_webHost.Open();
			}
			catch (CommunicationException ex)
			{
				Console.WriteLine("An exception occurred: {0}", ex.Message);
				m_webHost.Abort();
				return false;
			}

			return true;
		}

		private bool SetupWebServiceSilverlight()
		{
			Uri webServiceAddress = new Uri("http://localhost:" + SLWCFPort.ToString() + "/");
			m_webHostSilverlight = new WebServiceHost(typeof(WebServiceSilverlight), webServiceAddress);
			try
			{
				WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
				ServiceEndpoint endpoint = m_webHostSilverlight.AddServiceEndpoint(typeof(IPolicyRetriever), binding, "");
				m_webHostSilverlight.SetEndpointAddress(endpoint, "");
				m_webHostSilverlight.Open();
			}
			catch (CommunicationException ex)
			{
				Console.WriteLine("An exception occurred: {0}", ex.Message);
				m_webHostSilverlight.Abort();
				return false;
			}

			return true;
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
					Console.WriteLine("GUI disabled");
				}
				if (m_commandLineArgs.debug)
				{
					Console.WriteLine("Debugging enabled");
					SandboxGameAssemblyWrapper.IsDebugging = true;
				}
				if (m_commandLineArgs.noWCF)
				{
					Console.WriteLine("WCF disabled");
				}
				if (m_commandLineArgs.noSLWCF)
				{
					Console.WriteLine("SLWCF disabled");
				}
				if (m_commandLineArgs.wcfPort > 0)
				{
					Console.WriteLine("WCF port: " + m_commandLineArgs.wcfPort.ToString());
				}
				if (m_commandLineArgs.slWcfPort > 0)
				{
					Console.WriteLine("SLWCF port: " + m_commandLineArgs.slWcfPort.ToString());
				}
				if (m_commandLineArgs.autosave > 0)
				{
					Console.WriteLine("Autosave interval: " + m_commandLineArgs.autosave.ToString());
				}
				if (m_commandLineArgs.path.Length != 0)
				{
					Console.WriteLine("Full path pre-selected: '" + m_commandLineArgs.path + "'");
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
			get
			{
				m_commandLineArgs.autosave = (int)Math.Round(m_autosaveTimer.Interval / 60000.0);
				return m_autosaveTimer.Interval;
			}
			set
			{
				m_autosaveTimer.Interval = value;

				if (m_autosaveTimer.Interval <= 0)
					m_autosaveTimer.Interval = 300000;

				m_commandLineArgs.autosave = (int)Math.Round(m_autosaveTimer.Interval / 60000.0);
			}
		}

		[IgnoreDataMember]
		public bool IsWCFEnabled
		{
			get { return m_isWCFEnabled; }
			set
			{
				if (m_isWCFEnabled == value) return;
				m_isWCFEnabled = value;

				if (m_isWCFEnabled)
				{
					if (m_serverHost != null)
						m_serverHost.Open();
					if (m_webHost != null)
						m_webHost.Open();
					if (m_baseHost != null)
						m_baseHost.Open();
				}
				else
				{
					if (m_serverHost != null)
						m_serverHost.Close();
					if (m_webHost != null)
						m_webHost.Close();
					if (m_baseHost != null)
						m_baseHost.Close();
				}
			}
		}

		[IgnoreDataMember]
		public bool IsSLWCFEnabled
		{
			get { return m_isSLWCFEnabled; }
			set
			{
				if (m_isSLWCFEnabled == value) return;
				m_isSLWCFEnabled = value;

				if (m_isSLWCFEnabled)
				{
					if (m_webHostSilverlight != null)
						m_webHostSilverlight.Open();
				}
				else
				{
					if (m_webHostSilverlight != null)
						m_webHostSilverlight.Close();
				}
			}
		}

		[IgnoreDataMember]
		public ushort WCFPort
		{
			get
			{
				ushort port = m_commandLineArgs.wcfPort;
				if (port == 0)
					port = 8000;

				return port;
			}
			set { m_commandLineArgs.wcfPort = value; }
		}

		[IgnoreDataMember]
		public ushort SLWCFPort
		{
			get
			{
				ushort port = m_commandLineArgs.slWcfPort;
				if (port == 0)
					port = 8001;

				return port;
			}
			set { m_commandLineArgs.slWcfPort = value; }
		}

		[IgnoreDataMember]
		public string Path
		{
			get
			{
				string path = m_commandLineArgs.path;
				if (path == null || string.IsNullOrEmpty(path))
				{
					if (InstanceName.Length != 0)
					{
						SandboxGameAssemblyWrapper.UseCommonProgramData = true;
						SandboxGameAssemblyWrapper.Instance.InitMyFileSystem(InstanceName, false);
					}
					path = m_gameAssemblyWrapper.GetUserDataPath(InstanceName);
				}

				return path;
			}
			set { m_commandLineArgs.path = value; }
		}

		#endregion

		#region "Methods"

		public static ServiceHost CreateServiceHost(Type serviceType, Type contractType, string urlExtension, string name)
		{
			try
			{
				Uri baseAddress = new Uri("http://localhost:" + Server.Instance.WCFPort.ToString() + "/SEServerExtender/" + urlExtension);
				ServiceHost selfHost = new ServiceHost(serviceType, baseAddress);

				WSHttpBinding binding = new WSHttpBinding();
				binding.Security.Mode = SecurityMode.Message;
				binding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
				selfHost.AddServiceEndpoint(contractType, binding, name);

				ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				smb.HttpGetEnabled = true;
				selfHost.Description.Behaviors.Add(smb);

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					Console.WriteLine("Created WCF service at '" + baseAddress.ToString() + "'");
				}

				return selfHost;
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				return null;
			}
		}

		public static ServiceHost CreateSLServiceHost(Type serviceType, Type contractType, string urlExtension, string name)
		{
			try
			{
				Uri baseAddress = new Uri("http://localhost:" + (Server.Instance.SLWCFPort).ToString() + "/SEServerExtender/" + urlExtension);
				ServiceHost selfHost = new ServiceHost(serviceType, baseAddress);

				BasicHttpBinding binding = new BasicHttpBinding();
				selfHost.AddServiceEndpoint(contractType, binding, name);

				ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				smb.HttpGetEnabled = true;
				selfHost.Description.Behaviors.Add(smb);

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					Console.WriteLine("Created WCF service at '" + baseAddress.ToString() + "'");
				}

				return selfHost;
			}
			catch (CommunicationException ex)
			{
				LogManager.ErrorLog.WriteLineAndConsole("An exception occurred: " + ex.Message);
				return null;
			}
		}

		public void Init()
		{
			if (m_isInitialized)
				return;

			bool setupResult = true;
			setupResult &= SetupGameInstallation();
			setupResult &= SetupManagers();
			setupResult &= ProcessCommandLineArgs();

			if (IsWCFEnabled)
			{
				SetupServerService();
				SetupMainService();
				SetupWebService();
			}
			if (IsSLWCFEnabled)
			{
				SetupSLServerService();
				SetupSLMainService();
				SetupWebServiceSilverlight();
			}

			if (m_commandLineArgs.autosave > 0)
				m_autosaveTimer.Interval = m_commandLineArgs.autosave * 60000;

			if (!setupResult)
			{
				LogManager.ErrorLog.WriteLineAndConsole("Failed to initialize server");
				return;
			}

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

			if (!m_pluginManager.Initialized && !m_pluginManager.Loaded)
			{
				if (SandboxGameAssemblyWrapper.Instance.IsGameStarted)
				{
					m_pluginManager.LoadPlugins();
					m_pluginManager.Init();
				}
			}
			else
			{
				//Force a refresh of the chat messages before running the plugin update
				List<string> messages = ChatManager.Instance.ChatMessages;

				//Run the plugin update
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
				bool result = m_serverWrapper.StartServer(m_commandLineArgs.instanceName, m_commandLineArgs.path, !m_commandLineArgs.noConsole);

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
			catch (Exception ex)
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

				if (m_dedicatedConfigDefinition == null)
					LoadServerConfig();

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
			FileInfo fileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg");
			if (fileInfo.Exists)
			{
				MyConfigDedicatedData config = DedicatedConfigDefinition.Load(fileInfo);
				m_dedicatedConfigDefinition = new DedicatedConfigDefinition(config);
			}
		}

		public void SaveServerConfig()
		{
			FileInfo fileInfo = new FileInfo(Path + @"\SpaceEngineers-Dedicated.cfg");
			if (m_dedicatedConfigDefinition != null)
				m_dedicatedConfigDefinition.Save(fileInfo);
		}

		#endregion
	}
}
