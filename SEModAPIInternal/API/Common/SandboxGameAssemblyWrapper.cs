using SteamSDK;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

using SysUtils.Utils;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using VRage;
using VRage.Common.Utils;
using VRageMath;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class SandboxGameAssemblyWrapper
	{
		#region "Attributes"

		protected static SandboxGameAssemblyWrapper m_instance;
		protected static bool m_isDebugging;
		protected static bool m_isGameLoaded;

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

		private static Object m_nextEntityToUpdate;

		public static string GameConstantsClass = "00DD5482C0A3DF0D94B151167E77A6D9.5FBC15A83966C3D53201318E6F912741";
		public static string GameConstantsFactionsEnabledField = "AE3FD6A65A631D2BF9835EE8E86F8110";

		public static string CheckpointManagerClass = "36CC7CE820B9BBBE4B3FECFEEFE4AE86.828574590CB1B1AE5A5659D4B9659548";
		public static string CheckpointManagerSaveCheckpointMethod = "03AA898C5E9A48425F2CB4FFB2A49A82";
		public static string CheckpointManagerGetServerSectorMethod = "B6D13C37B0C7FDBF469AB93D18E4444A";
		public static string CheckpointManagerGetServerCheckpointMethod = "825106F82475488A49F8184C627DADEB";

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

		public static string ConfigContainerGetConfigData = "4DD64FD1D45E514D01C925D07B69B3BE";
		public static string ConfigContainerSetWorldName = "493E0E7BC7A617699C44A9A5FB8FF679";
		public static string ConfigContainerDedicatedDataField = "44A1510B70FC1BBE3664969D47820439";

		public static string ObjectManagerClass = "5BCAC68007431E61367F5B2CF24E2D6F.CAF1EB435F77C7B77580E2E16F988BED";
		public static string ObjectManagerGetEntityHashSet = "84C54760C0F0DDDA50B0BE27B7116ED8";
		public static string ObjectManagerAddEntity = "E5E18F5CAD1F62BB276DF991F20AE6AF";

		public static string NetworkSerializerClass = "5F381EA9388E0A32A8C817841E192BE8.8EFE49A46AB934472427B7D117FD3C64";
		public static string NetworkSerializerSendEntity = "A6B585C993B43E72219511726BBB0649";
		#endregion

		#region "Constructors and Initializers"

		protected SandboxGameAssemblyWrapper(string path)
		{
			m_instance = this;
			m_isDebugging = false;

			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			m_mainGameType = m_assembly.GetType(MainGameNamespace + "." + MainGameClass);
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

			Console.WriteLine("Finished loading SandboxGameAssemblyWrapper");
		}

		public static SandboxGameAssemblyWrapper GetInstance(string basePath = "")
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

		public static bool IsDebugging
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

		public Assembly GameAssembly
		{
			get { return m_assembly; }
		}

		#endregion

		#region "Methods"

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
				LogManager.GameLog.WriteLine(ex.ToString());
				return false;
			}
		}

		#region "Actions"

		public static void MainGameEvent1()
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
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public static void MainGameEvent3()
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
				LogManager.GameLog.WriteLine(ex);
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
				LogManager.GameLog.WriteLine(ex.ToString());
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
				LogManager.GameLog.WriteLine(ex.ToString());
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
				LogManager.GameLog.WriteLine(ex.ToString());
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
				LogManager.GameLog.WriteLine(ex.ToString());
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
				LogManager.GameLog.WriteLine(ex.ToString());
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
				LogManager.GameLog.WriteLine(ex.ToString());
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

				return mainGame;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
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
				LogManager.GameLog.WriteLine(ex);
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
				LogManager.GameLog.WriteLine(ex.ToString());
				return;
			}
		}

		public Object GetSteamInterface()
		{
			MethodInfo getSteamInterface = MainGameType.GetMethod("B2BA465BE723C583AB344246B0272095", BindingFlags.Public | BindingFlags.Static);
			Object steamInterface = getSteamInterface.Invoke(null, new object[] { });

			return steamInterface;
		}

		public Object GetServerSteamManager()
		{
			Type networkManagerWrapper = GameAssembly.GetType("C42525D7DE28CE4CFB44651F3D03A50D.8920513CC2D9F0BEBCDC74DBD637049F");
			FieldInfo networkManagerField = networkManagerWrapper.GetField("8E8199A1194065205F01051DC8B72DE7", BindingFlags.NonPublic | BindingFlags.Static);
			Object serverSteamManager = networkManagerField.GetValue(null);

			return serverSteamManager;
		}

		public bool IsGameStarted()
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

		public List<ulong> GetConnectedPlayers()
		{
			Object steamServerManager = GetServerSteamManager();
			if (steamServerManager == null)
				return new List<ulong>();

			FieldInfo connectedPlayersField = steamServerManager.GetType().GetField("89E92B070228A8BC746EFB57A3F6D2E5", BindingFlags.NonPublic | BindingFlags.Instance);
			List<ulong> connectedPlayers = (List<ulong>)connectedPlayersField.GetValue(steamServerManager);

			return connectedPlayers;
		}

		public void PeerSessionRequest(ulong remoteUserId)
		{
			ChatManager.GetInstance().ChatMessages.Add(remoteUserId.ToString() + " is connecting ...");
			LogManager.APILog.WriteLine("Steam user '" + remoteUserId.ToString() + "' submitted a p2p session request");
		}

		public void SetupSessionRequestHandlers()
		{
			try
			{
				if (GetSteamInterface() == null)
					return;

				if (GetServerSteamManager() == null)
					return;

				Peer2Peer.SessionRequest += new SessionRequest(PeerSessionRequest);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void SetPlayerBan(ulong remoteUserId, bool isBanned)
		{
			Object steamServerManager = GetServerSteamManager();
			MethodInfo banPlayerMethod = steamServerManager.GetType().GetMethod("E65F4CDD9FACA817DB685540E98F318C");
			banPlayerMethod.Invoke(steamServerManager, new object[] { remoteUserId, isBanned });
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
				LogManager.GameLog.WriteLine(ex);
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
				LogManager.GameLog.WriteLine(ex);
				return;
			}
		}

		public Vector3? GenerateRandomBorderPosition(Vector3 borderStart, Vector3 borderEnd)
		{
			BoundingBox box = new BoundingBox(borderStart, borderEnd);
			Vector3? nullableResult = new Vector3?(MyVRageUtils.GetRandomBorderPosition(ref box));

			return nullableResult;
		}

		public static HashSet<Object> GetObjectManagerHashSetData()
		{
			try
			{
				Type objectManagerType = SandboxGameAssemblyWrapper.GetInstance().GameAssembly.GetType(ObjectManagerClass);
				MethodInfo getEntityHashSet = objectManagerType.GetMethod(ObjectManagerGetEntityHashSet, BindingFlags.Public | BindingFlags.Static);
				var rawValue = getEntityHashSet.Invoke(null, new object[] { });
				HashSet<Object> convertedSet = UtilityFunctions.ConvertHashSet(rawValue);

				return convertedSet;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex.ToString());
				return null;
			}
		}

		private static List<T> GetAPIEntityList<T, TO>(MyObjectBuilderTypeEnum type)
			where T : BaseEntity
			where TO : MyObjectBuilder_EntityBase
		{
			HashSet<Object> rawEntities = GetObjectManagerHashSetData();
			List<T> list = new List<T>();

			foreach (Object entity in rawEntities)
			{
				try
				{
					MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(entity, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { false });

					if (baseEntity.TypeId == type)
					{
						TO objectBuilder = (TO)baseEntity;
						T apiEntity = (T)Activator.CreateInstance(typeof(T), new object[] { objectBuilder, entity });

						list.Add(apiEntity);
					}
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex.ToString());
				}
			}

			return list;
		}

		public static List<CubeGridEntity> GetCubeGrids()
		{
			return GetAPIEntityList<CubeGridEntity, MyObjectBuilder_CubeGrid>(MyObjectBuilderTypeEnum.CubeGrid);
		}

		public static List<CharacterEntity> GetCharacters()
		{
			return GetAPIEntityList<CharacterEntity, MyObjectBuilder_Character>(MyObjectBuilderTypeEnum.Character);
		}

		public static List<VoxelMap> GetVoxelMaps()
		{
			return GetAPIEntityList<VoxelMap, MyObjectBuilder_VoxelMap>(MyObjectBuilderTypeEnum.VoxelMap);
		}

		public static List<FloatingObject> GetFloatingObjects()
		{
			return GetAPIEntityList<FloatingObject, MyObjectBuilder_FloatingObject>(MyObjectBuilderTypeEnum.FloatingObject);
		}

		public static List<Meteor> GetMeteors()
		{
			return GetAPIEntityList<Meteor, MyObjectBuilder_Meteor>(MyObjectBuilderTypeEnum.Meteor);
		}

		public static void AddEntity(Object gameEntity)
		{
			try
			{
				m_nextEntityToUpdate = gameEntity;

				Action action = InternalAddEntity;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public static void InternalAddEntity()
		{
			try
			{
				if (m_nextEntityToUpdate == null)
					return;

				if (SandboxGameAssemblyWrapper.IsDebugging)
					Console.WriteLine("Entity '': Adding to scene ...");

				Assembly assembly = SandboxGameAssemblyWrapper.GetInstance().GameAssembly;
				Type objectManagerType = assembly.GetType(ObjectManagerClass);

				MethodInfo addEntityMethod = objectManagerType.GetMethod(ObjectManagerAddEntity, BindingFlags.Public | BindingFlags.Static);
				addEntityMethod.Invoke(null, new object[] { m_nextEntityToUpdate, true });

				MyObjectBuilder_EntityBase baseEntity = (MyObjectBuilder_EntityBase)BaseEntity.InvokeEntityMethod(m_nextEntityToUpdate, BaseEntity.BaseEntityGetObjectBuilderMethod, new object[] { false });
				Type someManager = assembly.GetType(NetworkSerializerClass);
				MethodInfo sendEntityMethod = someManager.GetMethod(NetworkSerializerSendEntity, BindingFlags.Public | BindingFlags.Static);
				sendEntityMethod.Invoke(null, new object[] { baseEntity });

				m_nextEntityToUpdate = null;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole("Failed to add new entity");
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
