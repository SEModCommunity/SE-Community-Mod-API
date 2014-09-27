using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.Support;

using VRage.Common.Utils;
using SEModAPIInternal.API.Entity;

namespace SEModAPIInternal.API.Common
{
	public class SandboxGameAssemblyWrapper
	{
		#region "Attributes"

		private Assembly m_assembly;

		protected static SandboxGameAssemblyWrapper m_instance;
		protected static bool m_isDebugging;
		protected static bool m_isUsingCommonProgramData;
		protected static bool m_isInSafeMode;

		protected bool m_isGameLoaded;

		protected DateTime m_lastProfilingOutput;
		protected int m_countQueuedActions;
		protected double m_averageQueuedActions;

		/////////////////////////////////////////////////////////////////////////////

		public static string MainGameNamespace = "B337879D0C82A5F9C44D51D954769590";
		public static string MainGameClass = "B3531963E948FB4FA1D057C4340C61B4";

		public static string MainGameEnqueueActionMethod = "0172226C0BA7DAE0B1FCE0AF8BC7F735";
		public static string MainGameGetTimeMillisMethod = "676C50EDDF93A0D8452B6BAFE7A33F32";

		public static string MainGameInstanceField = "392503BDB6F8C1E34A232489E2A0C6D4";
		public static string MainGameConfigContainerField = "4895ADD02F2C27ED00C63E7E506EE808";
		public static string MainGameIsLoadedField = "76E577DA6C1683D13B1C0BE5D704C241";
		public static string MainGameLoadingCompleteActionField = "0CAB22C866086930782A91BA5F21A936";
		public static string MainGameMyLogField = "1976E5D4FE6E8C1BD369273DEE0025AC";

		/////////////////////////////////////////////////////////////////////////////

		public static string ServerCoreNamespace = "168638249D29224100DB50BB468E7C07";
		public static string ServerCoreClass = "7BAD4AFD06B91BCD63EA57F7C0D4F408";

		public static string ServerCoreNullRenderField = "53A34747D8E8EDA65E601C194BECE141";

		/////////////////////////////////////////////////////////////////////////////

		public static string GameConstantsNamespace = "00DD5482C0A3DF0D94B151167E77A6D9";
		public static string GameConstantsClass = "5FBC15A83966C3D53201318E6F912741";

		/////////////////////////////////////////////////////////////////////////////

		public static string ConfigContainerNamespace = "00DD5482C0A3DF0D94B151167E77A6D9";
		public static string ConfigContainerClass = "EB0B0448CDB2C619C686429C597589BC";

		public static string ConfigContainerGetConfigDataMethod = "Load";

		public static string ConfigContainerDedicatedDataField = "44A1510B70FC1BBE3664969D47820439";

		/////////////////////////////////////////////////////////////////////////////

		public static string CubeBlockObjectFactoryNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string CubeBlockObjectFactoryClass = "8E009F375CE3CE0A06E67CA053084252";

		public static string CubeBlockObjectFactoryGetBuilderFromEntityMethod = "967C934A80A75642EADF86455E924134";

		/////////////////////////////////////////////////////////////////////////////

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

			string assemblyPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom(assemblyPath);

			m_lastProfilingOutput = DateTime.Now;
			m_countQueuedActions = 0;
			m_averageQueuedActions = 0;

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

		public static Type MainGameType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(MainGameNamespace, MainGameClass);
				return type;
			}
		}

		public static Type ServerCoreType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ServerCoreNamespace, ServerCoreClass);
				return type;
			}
		}

		public static Type GameConstantsType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(GameConstantsNamespace, GameConstantsClass);
				return type;
			}
		}

		public static Type ConfigContainerType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(ConfigContainerNamespace, ConfigContainerClass);
				return type;
			}
		}

		public static Type CubeBlockObjectFactoryType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CubeBlockObjectFactoryNamespace, CubeBlockObjectFactoryClass);
				return type;
			}
		}

		public static Type EntityBaseObjectFactoryType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(EntityBaseObjectFactoryNamespace, EntityBaseObjectFactoryClass);
				return type;
			}
		}

		public static Object MainGame
		{
			get
			{
				try
				{
					Object mainGame = BaseObject.GetStaticFieldValue(MainGameType, MainGameInstanceField);

					return mainGame;
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
					return null;
				}
			}
		}

		public bool IsGameStarted
		{
			get
			{
				try
				{
					if (MainGame == null)
						return false;

					if (!m_isGameLoaded)
					{
						bool someValue = (bool)BaseObject.GetEntityFieldValue(MainGame, MainGameIsLoadedField);
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
					LogManager.ErrorLog.WriteLine(ex);
					return false;
				}
			}
		}

		#endregion

		#region "Methods"

		private void EntitiesLoadedEvent()
		{
			try
			{
				LogManager.APILog.WriteLineAndConsole("MainGameEvent - Entity loading complete");

				//TODO - Do stuff
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		private Object GetServerConfigContainer()
		{
			try
			{
				Object configObject = BaseObject.GetEntityFieldValue(MainGame, MainGameConfigContainerField);

				return configObject;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = MainGameType;
				if (type == null)
					throw new Exception("Could not find internal type for MainGame");
				bool result = true;
				result &= BaseObject.HasMethod(type, MainGameEnqueueActionMethod);
				result &= BaseObject.HasMethod(type, MainGameGetTimeMillisMethod);
				result &= BaseObject.HasField(type, MainGameInstanceField);
				result &= BaseObject.HasField(type, MainGameConfigContainerField);
				result &= BaseObject.HasField(type, MainGameIsLoadedField);
				result &= BaseObject.HasField(type, MainGameLoadingCompleteActionField);
				result &= BaseObject.HasField(type, MainGameMyLogField);

				Type type2 = ServerCoreType;
				if (type2 == null)
					throw new Exception("Could not find physics manager type for ServerCore");
				result &= BaseObject.HasField(type2, ServerCoreNullRenderField);

				Type type3 = GameConstantsType;
				if (type3 == null)
					throw new Exception("Could not find physics manager type for GameConstants");

				Type type4 = ConfigContainerType;
				if (type4 == null)
					throw new Exception("Could not find physics manager type for ConfigContainer");
				result &= BaseObject.HasMethod(type4, ConfigContainerGetConfigDataMethod);
				result &= BaseObject.HasField(type4, ConfigContainerDedicatedDataField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public MyObjectBuilder_CubeBlock GetCubeBlockObjectBuilderFromEntity(Object entity)
		{
			try
			{
				MyObjectBuilder_CubeBlock newObjectBuilder = (MyObjectBuilder_CubeBlock)BaseObject.InvokeStaticMethod(CubeBlockObjectFactoryType, CubeBlockObjectFactoryGetBuilderFromEntityMethod, new object[] { entity });

				return newObjectBuilder;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		public MyObjectBuilder_EntityBase GetEntityBaseObjectBuilderFromEntity(Object entity)
		{
			try
			{
				MyObjectBuilder_EntityBase newObjectBuilder = (MyObjectBuilder_EntityBase)BaseObject.InvokeStaticMethod(EntityBaseObjectFactoryType, EntityBaseObjectFactoryGetBuilderFromEntityMethod, new object[] { entity });

				return newObjectBuilder;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal bool EnqueueMainGameAction(Action action)
		{
			try
			{
				BaseObject.InvokeEntityMethod(MainGame, MainGameEnqueueActionMethod, new object[] { action });

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					m_countQueuedActions++;

					TimeSpan timeSinceLastProfilingOutput = DateTime.Now - m_lastProfilingOutput;
					if (timeSinceLastProfilingOutput.TotalSeconds > 60)
					{
						m_averageQueuedActions = m_countQueuedActions / timeSinceLastProfilingOutput.TotalSeconds;

						LogManager.APILog.WriteLine("Average actions queued per second: " + Math.Round(m_averageQueuedActions, 2).ToString());

						m_countQueuedActions = 0;
						m_lastProfilingOutput = DateTime.Now;
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		public MyConfigDedicatedData GetServerConfig()
		{
			try
			{
				Object configContainer = GetServerConfigContainer();
				MyConfigDedicatedData config = (MyConfigDedicatedData)BaseObject.GetEntityFieldValue(configContainer, ConfigContainerDedicatedDataField);
				if (config == null)
				{
					BaseObject.InvokeEntityMethod(configContainer, ConfigContainerGetConfigDataMethod);
					config = (MyConfigDedicatedData)BaseObject.GetEntityFieldValue(configContainer, ConfigContainerDedicatedDataField);
				}

				return config;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		public void SetNullRender(bool nullRender)
		{
			try
			{
				BaseObject.SetStaticFieldValue(ServerCoreType, ServerCoreNullRenderField, nullRender);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return;
			}
		}

		public Type GetAssemblyType(string namespaceName, string className)
		{
			try
			{
				Type type = m_assembly.GetType(namespaceName + "." + className);

				return type;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		public int GetMainGameMilliseconds()
		{
			try
			{
				int gameTimeMillis = (int)BaseObject.InvokeStaticMethod(MainGameType, MainGameGetTimeMillisMethod);

				return gameTimeMillis;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
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

		public void InitMyFileSystem(string instanceName = "", bool reset = true)
		{
			string contentPath = Path.Combine(new FileInfo(MyFileSystem.ExePath).Directory.FullName, "Content");
			string userDataPath = SandboxGameAssemblyWrapper.Instance.GetUserDataPath(instanceName);

			if (reset)
			{
				MyFileSystem.Reset();
			}
			else
			{
				try
				{
					if (!string.IsNullOrWhiteSpace(MyFileSystem.ContentPath))
						return;
					if (!string.IsNullOrWhiteSpace(MyFileSystem.UserDataPath))
						return;
				}
				catch (Exception)
				{
					//Do nothing
				}
			}

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

		#endregion
	}
}
