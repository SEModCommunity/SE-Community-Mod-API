using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.Support;

using VRage.Common.Utils;

namespace SEModAPIInternal.API.Common
{
	public class SandboxGameAssemblyWrapper
	{
		#region "Attributes"

		private Assembly m_assembly;

		private Type m_mainGameType;
		private Type m_serverCoreType;
		private Type m_configContainerType;

		private MethodInfo m_setConfigWorldName;

		private FieldInfo m_mainGameInstanceField;
		private FieldInfo m_configContainerField;
		private FieldInfo m_configContainerDedicatedDataField;
		private FieldInfo m_serverCoreNullRender;

		protected static SandboxGameAssemblyWrapper m_instance;
		protected static bool m_isDebugging;
		protected static bool m_isUsingCommonProgramData;
		protected static bool m_isInSafeMode;

		protected bool m_isGameLoaded;

		public static string GameConstantsNamespace = "00DD5482C0A3DF0D94B151167E77A6D9";
		public static string GameConstantsClass = "5FBC15A83966C3D53201318E6F912741";
		public static string GameConstantsGamePausedField = "6F202682A23A6CD845931A94886C4872";

		public static string ServerCoreClass = "168638249D29224100DB50BB468E7C07.7BAD4AFD06B91BCD63EA57F7C0D4F408";
		public static string ServerCoreNullRenderField = "53A34747D8E8EDA65E601C194BECE141";

		public static string MainGameNamespace = "B337879D0C82A5F9C44D51D954769590";
		public static string MainGameClass = "B3531963E948FB4FA1D057C4340C61B4";
		public static string MainGameClientClass = "C47C2A2584662292007B04D8AD796E3D";
		public static string MainGameInstanceField = "392503BDB6F8C1E34A232489E2A0C6D4";
		public static string MainGameEnqueueActionMethod = "0172226C0BA7DAE0B1FCE0AF8BC7F735";
		public static string MainGameConfigContainerField = "4895ADD02F2C27ED00C63E7E506EE808";
		public static string MainGameAction1 = "0CAB22C866086930782A91BA5F21A936";	//() Entity loading complete
		public static string MainGameAction2 = "736ABFDB88EC08BFEA24D3A2AB06BE80";	//(Bool) ??
		public static string MainGameAction3 = "F7E4614DB0033215C446B502BA17BDDB";	//() Triggers Action1
		public static string MainGameAction4 = "B43682C38AD089E0EE792C74E4503633";	//() Triggered by 'Ctrl+C'
		public static string MainGameInitializeMethod = "2AA66FBD3F2C5EC250558B3136F3974A";
		public static string MainGameExitMethod = "246E732EE67F7F6F88C4FF63B3901107";
		public static string MainGameIsLoadedField = "76E577DA6C1683D13B1C0BE5D704C241";
		public static string MainGameGetTimeMillisMethod = "676C50EDDF93A0D8452B6BAFE7A33F32";

		public static string ConfigContainerGetConfigData = "4DD64FD1D45E514D01C925D07B69B3BE";
		public static string ConfigContainerSetWorldName = "493E0E7BC7A617699C44A9A5FB8FF679";
		public static string ConfigContainerDedicatedDataField = "44A1510B70FC1BBE3664969D47820439";

		public static string CubeBlockObjectFactoryNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string CubeBlockObjectFactoryClass = "8E009F375CE3CE0A06E67CA053084252";
		public static string CubeBlockObjectFactoryGetBuilderFromEntityMethod = "967C934A80A75642EADF86455E924134";

		public static string EntityBaseObjectFactoryNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string EntityBaseObjectFactoryClass = "E825333D6467D99DD83FB850C600395C";
		public static string EntityBaseObjectFactoryGetBuilderFromEntityMethod = "85DD00A89AFE64DF0A1B3FD4A5139A04";

		#endregion

		#region "Constructors and Initializers"

		protected SandboxGameAssemblyWrapper()
		{
			m_instance = this;
			m_isDebugging = false;
			m_isUsingCommonProgramData = false;
			m_isInSafeMode = false;

			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			m_mainGameType = m_assembly.GetType(MainGameNamespace + "." + MainGameClass);
			m_serverCoreType = m_assembly.GetType(ServerCoreClass);

			m_mainGameInstanceField = m_mainGameType.GetField(MainGameInstanceField, BindingFlags.Static | BindingFlags.Public);
			m_configContainerField = m_mainGameType.GetField(MainGameConfigContainerField, BindingFlags.Static | BindingFlags.Public);
			m_configContainerType = m_configContainerField.FieldType;
			m_serverCoreNullRender = m_serverCoreType.GetField(ServerCoreNullRenderField, BindingFlags.Public | BindingFlags.Static);

			m_configContainerDedicatedDataField = m_configContainerType.GetField(ConfigContainerDedicatedDataField, BindingFlags.NonPublic | BindingFlags.Instance);
			m_setConfigWorldName = m_configContainerType.GetMethod(ConfigContainerSetWorldName, BindingFlags.Public | BindingFlags.Instance);

			Console.WriteLine("Finished loading SandboxGameAssemblyWrapper");
		}

		#endregion

		#region "Properties"

		public static SandboxGameAssemblyWrapper Instance
		{
			get
			{
				if (m_instance == null)
				{
					m_instance = new SandboxGameAssemblyWrapper();
				}

				return m_instance;
			}
		}

		public static bool IsDebugging
		{
			get
			{
				var inst = SandboxGameAssemblyWrapper.Instance;
				return m_isDebugging;
			}
			set
			{
				var inst = SandboxGameAssemblyWrapper.Instance;
				m_isDebugging = value;
			}
		}

		public static bool UseCommonProgramData
		{
			get
			{
				var inst = SandboxGameAssemblyWrapper.Instance;
				return m_isUsingCommonProgramData;
			}
			set
			{
				var inst = SandboxGameAssemblyWrapper.Instance;
				m_isUsingCommonProgramData = value;
			}
		}

		public static bool IsInSafeMode
		{
			get
			{
				var inst = SandboxGameAssemblyWrapper.Instance;
				return m_isInSafeMode;
			}
			set
			{
				var inst = SandboxGameAssemblyWrapper.Instance;
				m_isInSafeMode = value;
			}
		}

		public Type MainGameType
		{
			get { return m_mainGameType; }
		}

		public Type CubeBlockObjectFactoryType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeBlockObjectFactoryNamespace, CubeBlockObjectFactoryClass);
				return type;
			}
		}

		public Type EntityBaseObjectFactoryType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(EntityBaseObjectFactoryNamespace, EntityBaseObjectFactoryClass);
				return type;
			}
		}

		public Assembly GameAssembly
		{
			get { return m_assembly; }
		}

		public bool IsGameStarted
		{
			get
			{
				try
				{
					var mainGame = GetMainGameInstance();
					if (mainGame == null)
						return false;

					if (!m_isGameLoaded)
					{
						FieldInfo gameLoadedField = MainGameType.BaseType.GetField(MainGameIsLoadedField, BindingFlags.NonPublic | BindingFlags.Instance);
						bool someValue = (bool)gameLoadedField.GetValue(mainGame);
						if (someValue)
						{
							m_isGameLoaded = true;

							return true;
						}

						return false;
					}

					return true;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return false;
				}
			}
		}

		#endregion

		#region "Methods"

		#region "Actions"

		public void MainGameEvent1()
		{
			try
			{
				Console.WriteLine("MainGameEvent - Entity loading complete");

				FieldInfo actionField = m_mainGameType.GetField(MainGameAction1, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction = MainGameEvent1;
				actionField.SetValue(null, newAction);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void MainGameEvent2(bool param0)
		{
			try
			{
				Console.WriteLine("MainGameEvent - '2' - " + param0.ToString());

				FieldInfo actionField = m_mainGameType.GetField(MainGameAction2, BindingFlags.NonPublic | BindingFlags.Instance);
				Action<bool> newAction = MainGameEvent2;
				actionField.SetValue(GetMainGameInstance(), newAction);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void MainGameEvent3()
		{
			try
			{
				Console.WriteLine("MainGameEvent - Game loaded!");

				m_isGameLoaded = true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void MainGameEvent4()
		{
			try
			{
				Console.WriteLine("MainGameEvent - 'Ctrl+C' pressed");

				FieldInfo actionField = m_mainGameType.GetField(MainGameAction4, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction = MainGameEvent4;
				actionField.SetValue(null, newAction);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		public MyObjectBuilder_CubeBlock GetCubeBlockObjectBuilderFromEntity(Object entity)
		{
			try
			{
				MethodInfo method = CubeBlockObjectFactoryType.GetMethod(CubeBlockObjectFactoryGetBuilderFromEntityMethod);
				MyObjectBuilder_CubeBlock newObjectBuilder = (MyObjectBuilder_CubeBlock)method.Invoke(null, new object[] { entity });

				return newObjectBuilder;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public MyObjectBuilder_EntityBase GetEntityBaseObjectBuilderFromEntity(Object entity)
		{
			try
			{
				MethodInfo method = EntityBaseObjectFactoryType.GetMethod(EntityBaseObjectFactoryGetBuilderFromEntityMethod);
				MyObjectBuilder_EntityBase newObjectBuilder = (MyObjectBuilder_EntityBase)method.Invoke(null, new object[] { entity });

				return newObjectBuilder;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public bool EnqueueMainGameAction(Action action)
		{
			try
			{
				MethodInfo enqueue = m_mainGameType.GetMethod(MainGameEnqueueActionMethod);
				enqueue.Invoke(GetMainGameInstance(), new object[] { action });

				return true;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return false;
			}
		}

		public Object GetServerConfigContainer()
		{
			try
			{
				Object configObject = m_configContainerField.GetValue(null);

				return configObject;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return null;
			}
		}

		public MyConfigDedicatedData GetServerConfig()
		{
			try
			{
				Object configContainer = GetServerConfigContainer();
				MyConfigDedicatedData config = (MyConfigDedicatedData)m_configContainerDedicatedDataField.GetValue(configContainer);
				if (config == null)
				{
					MethodInfo loadConfigDataMethod = m_configContainerField.FieldType.GetMethod(ConfigContainerGetConfigData, BindingFlags.Public | BindingFlags.Instance);
					loadConfigDataMethod.Invoke(configContainer, new object[] { });
					config = (MyConfigDedicatedData)m_configContainerDedicatedDataField.GetValue(configContainer);
				}

				return config;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return null;
			}
		}

		public Object GetMainGameInstance()
		{
			try
			{
				FieldInfo mainGameInstanceField = m_mainGameType.GetField(MainGameInstanceField, BindingFlags.Static | BindingFlags.Public);
				Object mainGame = mainGameInstanceField.GetValue(null);

				return mainGame;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public void SetNullRender(bool nullRender)
		{
			try
			{
				m_serverCoreNullRender.SetValue(null, nullRender);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return;
			}
		}

		public void SetConfigWorld(string worldName)
		{
			try
			{
				MyConfigDedicatedData config = GetServerConfig();

				m_setConfigWorldName.Invoke(GetServerConfigContainer(), new object[] { worldName });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return;
			}
		}

		public Type GetAssemblyType(string namespaceName, string className)
		{
			try
			{
				Type type = GameAssembly.GetType(namespaceName + "." + className);

				return type;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public int GetMainGameMilliseconds()
		{
			try
			{
				MethodInfo getTimeMillisMethod = MainGameType.GetMethod(MainGameGetTimeMillisMethod);
				int gameTimeMillis = (int)getTimeMillisMethod.Invoke(null, new object[] { });

				return gameTimeMillis;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return 0;
			}
		}

		public String GetUserDataPath(string instanceName = "")
		{
			string userDataPath = "";
			if (SandboxGameAssemblyWrapper.UseCommonProgramData && instanceName != "")
			{
				userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SpaceEngineersDedicated", instanceName);
			}
			else
			{
				userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineersDedicated");
			}

			return userDataPath;
		}

		public void InitMyFileSystem(string instanceName = "")
		{
			string contentPath = Path.Combine(new FileInfo(MyFileSystem.ExePath).Directory.FullName, "Content");
			string userDataPath = SandboxGameAssemblyWrapper.Instance.GetUserDataPath(instanceName);

			MyFileSystem.Reset();
			MyFileSystem.Init(contentPath, userDataPath);
			MyFileSystem.InitUserSpecific((string)null);

			string debugContentPath = MyFileSystem.ContentPath;
			string debugUserDataPath = MyFileSystem.UserDataPath;
		}

		public List<string> GetCommonInstanceList()
		{
			List<string> result = new List<string>();

			try
			{
				string commonPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "SpaceEngineersDedicated");
				if (Directory.Exists(commonPath))
				{
					string[] subDirectories = Directory.GetDirectories(commonPath);
					foreach (string fullInstancePath in subDirectories)
					{
						string[] directories = fullInstancePath.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
						string instanceName = directories[directories.Length - 1];

						result.Add(instanceName);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			return result;
		}

		public bool IsUserAdmin(ulong remoteUserId)
		{
			bool result = false;

			List<ulong> adminUsers = GetServerConfig().Administrators;
			foreach (ulong userId in adminUsers)
			{
				if (remoteUserId == userId)
				{
					result = true;
					break;
				}
			}

			return result;
		}

		#endregion
	}
}
