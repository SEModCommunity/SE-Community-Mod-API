using SteamSDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

using SysUtils.Utils;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using VRage;
using VRageMath;

namespace SEModAPI.API.Internal
{
	public class SandboxGameAssemblyWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static SandboxGameAssemblyWrapper m_instance;

		private static Assembly m_assembly;

		private static Type m_mainGameType;
		private Type m_checkpointManagerType;
		private Type m_serverCoreType;
		private static Type m_configContainerType;

		private MethodInfo m_saveCheckpoint;
		private MethodInfo m_getServerSector;
		private MethodInfo m_getServerCheckpoint;
		private static MethodInfo m_setConfigWorldName;

		private FieldInfo m_mainGameInstanceField;
		private static FieldInfo m_configContainerField;
		private static FieldInfo m_configContainerDedicatedDataField;
		private static FieldInfo m_serverCoreNullRender;

		public static string GameConstantsClass = "00DD5482C0A3DF0D94B151167E77A6D9.5FBC15A83966C3D53201318E6F912741";
		public static string GameConstantsFactionsEnabledField = "AE3FD6A65A631D2BF9835EE8E86F8110";

		public static string CheckpointManagerClass = "36CC7CE820B9BBBE4B3FECFEEFE4AE86.828574590CB1B1AE5A5659D4B9659548";
		public static string CheckpointManagerSaveCheckpointMethod = "03AA898C5E9A48425F2CB4FFB2A49A82";
		public static string CheckpointManagerGetServerSectorMethod = "B6D13C37B0C7FDBF469AB93D18E4444A";
		public static string CheckpointManagerGetServerCheckpointMethod = "825106F82475488A49F8184C627DADEB";

		public static string ServerCoreClass = "168638249D29224100DB50BB468E7C07.7BAD4AFD06B91BCD63EA57F7C0D4F408";
		public static string ServerCoreNullRenderField = "53A34747D8E8EDA65E601C194BECE141";

		public static string MainGameClass = "B337879D0C82A5F9C44D51D954769590.B3531963E948FB4FA1D057C4340C61B4";
		public static string MainGameInstanceField = "392503BDB6F8C1E34A232489E2A0C6D4";
		public static string MainGameEnqueueActionMethod = "0172226C0BA7DAE0B1FCE0AF8BC7F735";
		public static string MainGameMyLogField = "1976E5D4FE6E8C1BD369273DEE0025AC";
		public static string MainGameConfigContainerField = "4895ADD02F2C27ED00C63E7E506EE808";
		public static string MainGameAction1 = "0CAB22C866086930782A91BA5F21A936";	//() Entity loading complete
		public static string MainGameAction2 = "736ABFDB88EC08BFEA24D3A2AB06BE80";	//(Bool) ??
		public static string MainGameAction3 = "F7E4614DB0033215C446B502BA17BDDB";	//() Triggers Action1
		public static string MainGameAction4 = "B43682C38AD089E0EE792C74E4503633";	//() Triggered by 'Ctrl+C'

		public static string ConfigContainerGetConfigData = "4DD64FD1D45E514D01C925D07B69B3BE";
		public static string ConfigContainerSetWorldName = "493E0E7BC7A617699C44A9A5FB8FF679";
		public static string ConfigContainerDedicatedDataField = "44A1510B70FC1BBE3664969D47820439";

		#endregion

		#region "Constructors and Initializers"

		protected SandboxGameAssemblyWrapper(string path)
			: base(path)
		{
			m_instance = this;

			//string assemblyPath = Path.Combine(path, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			//string MySandboxGame_ExitMethod = "246E732EE67F7F6F88C4FF63B3901107";
			//string MySandboxGame_Initialize = "2AA66FBD3F2C5EC250558B3136F3974A";

			m_mainGameType = m_assembly.GetType(MainGameClass);
			m_checkpointManagerType = m_assembly.GetType(CheckpointManagerClass);
			m_serverCoreType = m_assembly.GetType(ServerCoreClass);

			Type[] argTypes = new Type[2];
			argTypes[0] = typeof(MyObjectBuilder_Checkpoint);
			argTypes[1] = typeof(string);
			m_saveCheckpoint = m_checkpointManagerType.GetMethod(CheckpointManagerSaveCheckpointMethod, argTypes);
			m_getServerSector = m_checkpointManagerType.GetMethod(CheckpointManagerGetServerSectorMethod, BindingFlags.Static | BindingFlags.Public);
			m_getServerCheckpoint = m_checkpointManagerType.GetMethod(CheckpointManagerGetServerCheckpointMethod, BindingFlags.Static | BindingFlags.Public);

			m_mainGameInstanceField = m_mainGameType.GetField(MainGameInstanceField, BindingFlags.Static | BindingFlags.Public);
			m_configContainerField = m_mainGameType.GetField(MainGameConfigContainerField, BindingFlags.Static | BindingFlags.Public);
			m_configContainerType = m_configContainerField.FieldType;
			m_serverCoreNullRender = m_serverCoreType.GetField(ServerCoreNullRenderField, BindingFlags.Public | BindingFlags.Static);

			m_configContainerDedicatedDataField = m_configContainerType.GetField(ConfigContainerDedicatedDataField, BindingFlags.NonPublic | BindingFlags.Instance);
			m_setConfigWorldName = m_configContainerType.GetMethod(ConfigContainerSetWorldName, BindingFlags.Public | BindingFlags.Instance);

			Console.WriteLine("Finished loading ServerAssemblyWrapper");
		}

		new public static SandboxGameAssemblyWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new SandboxGameAssemblyWrapper(basePath);
			}
			return (SandboxGameAssemblyWrapper)m_instance;
		}

		#endregion

		#region "Properties"

		public Type MainGameType
		{
			get { return m_mainGameType; }
		}

		public Type CheckpointManagerType
		{
			get { return m_checkpointManagerType; }
		}

		new public static bool IsDebugging
		{
			get
			{
				SandboxGameAssemblyWrapper.GetInstance();
				return m_isDebugging;
			}
			set
			{
				SandboxGameAssemblyWrapper.GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		public static bool SetupMainGameEventHandlers(Object mainGame)
		{
			try
			{
				FieldInfo actionField = m_mainGameType.GetField(MainGameAction1, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction1 = MainGameEvent1;
				actionField.SetValue(null, newAction1);
				
				actionField = m_mainGameType.GetField(MainGameAction2, BindingFlags.NonPublic | BindingFlags.Instance);
				Action<bool> newAction2 = MainGameEvent2;
				actionField.SetValue(mainGame, newAction2);
				
				actionField = m_mainGameType.GetField(MainGameAction3, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction3 = MainGameEvent3;
				actionField.SetValue(null, newAction3);
				
				actionField = m_mainGameType.GetField(MainGameAction4, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction4 = MainGameEvent4;
				actionField.SetValue(null, newAction4);
				
				return true;
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static bool EnqueueMainGameAction(Action action)
		{
			try
			{
				MethodInfo enqueue = m_mainGameType.GetMethod(MainGameEnqueueActionMethod);
				enqueue.Invoke(GetMainGameInstance(), new object[] { action });

				return true;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return false;
			}
		}

		#region "Actions"

		public static void MainGameEvent1()
		{
			try
			{
				Console.WriteLine("MainGameEvent - Entity loading complete");

				TestSteamAPI();

				FieldInfo actionField = m_mainGameType.GetField(MainGameAction1, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction = MainGameEvent1;
				actionField.SetValue(null, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		public static void MainGameEvent2(bool param0)
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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		public static void MainGameEvent3()
		{
			try
			{
				Console.WriteLine("MainGameEvent - '3'");

				FieldInfo actionField = m_mainGameType.GetField(MainGameAction3, BindingFlags.NonPublic | BindingFlags.Static);
				Action newAction = MainGameEvent3;
				actionField.SetValue(null, newAction);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		public static void MainGameEvent4()
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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		#endregion

		private string GetLookupString(MethodInfo method, int key, int start, int length)
		{
			try
			{
				string result = (string)method.Invoke(null, new object[] { key, start, length });
				return result;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		public static Object GetServerConfigContainer()
		{
			try
			{
				Object configObject = m_configContainerField.GetValue(null);

				return configObject;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		public static MyConfigDedicatedData GetServerConfig()
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
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		public bool SaveCheckpoint(MyObjectBuilder_Checkpoint checkpoint, string worldName)
		{
			try
			{
				return (bool)m_saveCheckpoint.Invoke(null, new object[] { checkpoint, worldName });
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return false;
			}
		}

		public MyObjectBuilder_Sector GetServerSector(string worldName, Vector3I sectorLocation, out ulong sectorId)
		{
			try
			{
				object[] parameters = new object[] { worldName, sectorLocation, null };
				MyObjectBuilder_Sector result = (MyObjectBuilder_Sector)m_getServerSector.Invoke(null, parameters);
				sectorId = (ulong)parameters[1];

				return result;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				sectorId = 0;
				return null;
			}
		}

		public MyObjectBuilder_Checkpoint GetServerCheckpoint(string worldName, out ulong worldId)
		{
			try
			{
				object[] parameters = new object[] { worldName, null };
				MyObjectBuilder_Checkpoint result = (MyObjectBuilder_Checkpoint)m_getServerCheckpoint.Invoke(null, parameters);
				worldId = (ulong)parameters[1];

				return result;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				worldId = 0;
				return null;
			}
		}

		public static Object GetMainGameInstance()
		{
			try
			{
				FieldInfo mainGameInstanceField = m_mainGameType.GetField(MainGameInstanceField, BindingFlags.Static | BindingFlags.Public);
				Object mainGame = mainGameInstanceField.GetValue(null);

				//SetupMainGameEventHandlers(mainGame);

				return mainGame;
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return null;
			}
		}

		public static void SetNullRender(bool nullRender)
		{
			try
			{
				m_serverCoreNullRender.SetValue(null, nullRender);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		public static void SetConfigWorld(string worldName)
		{
			try
			{
				MyConfigDedicatedData config = GetServerConfig();

				m_setConfigWorldName.Invoke(GetServerConfigContainer(), new object[] { worldName });
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		public static MyLog GetMyLog()
		{
			try
			{
				FieldInfo myLogField = m_mainGameType.GetField(MainGameMyLogField, BindingFlags.Public | BindingFlags.Static);
				MyLog log = (MyLog)myLogField.GetValue(null);

				return log;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public static void TestSteamAPI()
		{
			try
			{
				SteamServerAPI serverAPI = SteamServerAPI.Instance;
				GameServer gameServer = serverAPI.GameServer;
				ulong serverSteamId = gameServer.GetSteamID();

				Console.WriteLine("DEBUG - Steam ID: " + serverSteamId.ToString());
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		public static void KickPlayer(ulong steamId)
		{
			try
			{
				SteamServerAPI serverAPI = SteamServerAPI.Instance;
				serverAPI.GameServer.SendUserDisconnect(steamId);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		public static void EnableFactions(bool enabled = true)
		{
			try
			{
				//Force initialization just in case because this method can be called from the UI
				GetInstance();

				Type gameConstantsType = m_assembly.GetType(GameConstantsClass);
				FieldInfo factionsEnabledField = gameConstantsType.GetField(GameConstantsFactionsEnabledField, BindingFlags.Public | BindingFlags.Static);
				bool currentValue = (bool)factionsEnabledField.GetValue(null);

				Console.WriteLine("Changing 'Factions' enabled from '" + currentValue.ToString() + "' to '" + enabled.ToString() + "'");

				factionsEnabledField.SetValue(null, enabled);
			}
			catch (Exception ex)
			{
				SandboxGameAssemblyWrapper.GetMyLog().WriteLine(ex.ToString());
				return;
			}
		}

		#endregion
	}
}
