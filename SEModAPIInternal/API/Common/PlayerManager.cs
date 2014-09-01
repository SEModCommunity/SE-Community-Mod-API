using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

using VRage.Serialization;

namespace SEModAPIInternal.API.Common
{
	public class PlayerMap
	{
		public struct InternalPlayerItem
		{
			public string name;
			public bool isDead;
			public ulong steamId;
			public string model;

			public InternalPlayerItem(Object source)
			{
				name = (string)BaseObject.GetEntityFieldValue(source, "E520D0BC4EC9B47A81D9F52D4B70345F");
				isDead = (bool)BaseObject.GetEntityFieldValue(source, "FE57DC9F46A21EA612B9769D5A7A9606");
				steamId = (ulong)BaseObject.GetEntityFieldValue(source, "F18302983BC40AE893A0E0E0F2263A93");
				model = (string)BaseObject.GetEntityFieldValue(source, "2E57D5D124FF88C06AD1DFF6FE070B73");
			}
		}

		#region "Attributes"

		private static PlayerMap m_instance;

		public static string PlayerMapNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string PlayerMapClass = "E4C2964159826A46D102C2D7FDDC0733";

		public static string PlayerMapEntityToPlayerMappingMethod = "79B43F2C2366136E88B5B7064CA93A74";
		public static string PlayerMapSteamIdToPlayerMappingMethod = "AC5FA6C0D87D43E4B5550A1BB7812DEB";
		public static string PlayerMapGetSerializableDictionaryMethod = "460B7921B2E774D61F63929C4032F1AC";
		public static string PlayerMapGetPlayerItemMappingMethod = "0EB2BF49DCB5C20A059E3D6CCA3665AA";
		public static string PlayerMapAddPlayerItemMappingMethod = "FC99AC4D95CE082574C5C7F4F48C63DE";

		//////////////////////////////////////////////////////

		public static string PlayerMapEntryNamespace = "";
		public static string PlayerMapEntryClass = "";

		public static string PlayerMapEntrySteamIdField = "208AE30D2628BD946A59F72F1A373ED4";

		#endregion

		#region "Constructors and Initializers"

		protected PlayerMap()
		{
			m_instance = this;

			Console.WriteLine("Finished loading PlayerMap");
		}

		#endregion

		#region "Properties"

		public static PlayerMap Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new PlayerMap();

				return m_instance;
			}
		}

		public Object BackingObject
		{
			get
			{
				Object backingObject = PlayerManager.Instance.InternalGetPlayerMap();

				return backingObject;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerMapNamespace, PlayerMapClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PlayerMap");
				bool result = true;
				result &= BaseObject.HasMethod(type1, PlayerMapEntityToPlayerMappingMethod);
				result &= BaseObject.HasMethod(type1, PlayerMapSteamIdToPlayerMappingMethod);
				result &= BaseObject.HasMethod(type1, PlayerMapGetSerializableDictionaryMethod);
				result &= BaseObject.HasMethod(type1, PlayerMapGetPlayerItemMappingMethod);
				result &= BaseObject.HasMethod(type1, PlayerMapAddPlayerItemMappingMethod);
				/*
				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerMapEntryNamespace, PlayerMapEntryClass);
				if (type2 == null)
					throw new Exception("Could not find player map entry type for PlayerMap");
				result &= BaseObject.HasField(type2, PlayerMapEntrySteamIdField);
				*/
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public string GetPlayerNameFromSteamId(ulong steamId)
		{
			string playerName = steamId.ToString();

			List<long> playerIds = PlayerMap.Instance.GetPlayerIdsFromSteamId(steamId);
			foreach (var entry in playerIds)
			{
				MyObjectBuilder_Checkpoint.PlayerItem playerItem = PlayerMap.Instance.GetPlayerItemFromPlayerId(entry);
				playerName = playerItem.Name;
			}

			return playerName;
		}

		public ulong GetSteamIdFromPlayerName(string playerName)
		{
			ulong steamId = 0;

			foreach (var entry in InternalGetPlayerItemMappping().Keys)
			{
				MyObjectBuilder_Checkpoint.PlayerItem playerItem = PlayerMap.Instance.GetPlayerItemFromPlayerId(entry);
				if (!playerItem.Name.Equals(playerName))
					continue;

				steamId = playerItem.SteamId;
			}

			if (steamId == 0)
			{
				try
				{
					steamId = ulong.Parse(playerName);
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
				}
			}

			return steamId;
		}

		public long GetPlayerEntityId(ulong steamId)
		{
			SerializableDictionary<long, ulong> map = InternalGetSerializableDictionary();
			if (!map.Dictionary.ContainsValue(steamId))
				return 0;

			long result = 0;
			foreach (var entry in map.Dictionary)
			{
				if (entry.Value == steamId)
				{
					result = entry.Key;
					break;
				}
			}

			return result;
		}

		public ulong GetSteamId(long playerEntityId)
		{
			SerializableDictionary<long, ulong> map = InternalGetSerializableDictionary();
			if (!map.Dictionary.ContainsKey(playerEntityId))
				return 0;

			ulong result = map.Dictionary[playerEntityId];

			return result;
		}

		public MyObjectBuilder_Checkpoint.PlayerItem GetPlayerItemFromPlayerId(long playerId)
		{
			MyObjectBuilder_Checkpoint.PlayerItem playerItem = new MyObjectBuilder_Checkpoint.PlayerItem();

			try
			{
				Dictionary<long, Object> allPlayers = InternalGetPlayerItemMappping();
				if (!allPlayers.ContainsKey(playerId))
					return playerItem;
				Object item = allPlayers[playerId];
				InternalPlayerItem internalPlayerItem = new InternalPlayerItem(item);
				playerItem.PlayerId = playerId;
				playerItem.SteamId = internalPlayerItem.steamId;
				playerItem.Name = internalPlayerItem.name;
				playerItem.Model = internalPlayerItem.model;
				playerItem.IsDead = internalPlayerItem.isDead;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			return playerItem;
		}

		public List<long> GetPlayerIdsFromSteamId(ulong steamId, bool ignoreDead = true)
		{
			List<long> matchingPlayerIds = new List<long>();

			try
			{
				Dictionary<long, Object> allPlayers = InternalGetPlayerItemMappping();
				foreach (var entry in allPlayers)
				{
					InternalPlayerItem internalPlayerItem = new InternalPlayerItem(entry.Value);
					if (ignoreDead && internalPlayerItem.isDead)
						continue;

					if (internalPlayerItem.steamId == steamId)
						matchingPlayerIds.Add(entry.Key);
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			return matchingPlayerIds;
		}

		public List<long> GetPlayerIds()
		{
			return new List<long>(InternalGetPlayerItemMappping().Keys);
		}

		protected Object GetPlayerFromSteamId(ulong steamId)
		{
			try
			{
				Object result = BaseObject.InvokeEntityMethod(BackingObject, PlayerMapSteamIdToPlayerMappingMethod, new object[] { steamId });

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		protected ulong GetSteamIdFromPlayer(Object player)
		{
			try
			{
				ulong steamId = (ulong)BaseObject.GetEntityFieldValue(player, PlayerMapEntrySteamIdField);

				return steamId;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return 0;
			}
		}

		protected ulong GetCharacterInternalEntityToSteamId(Object gameEntity)
		{
			try
			{
				Object mapEntry = BaseObject.InvokeEntityMethod(BackingObject, PlayerMapEntityToPlayerMappingMethod, new object[] { gameEntity });
				ulong steamId = GetSteamIdFromPlayer(mapEntry);

				return steamId;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return 0;
			}
		}

		protected SerializableDictionary<long, ulong> InternalGetSerializableDictionary()
		{
			try
			{
				SerializableDictionary<long, ulong> result = (SerializableDictionary<long, ulong>)BaseObject.InvokeEntityMethod(BackingObject, PlayerMapGetSerializableDictionaryMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new SerializableDictionary<long, ulong>();
			}
		}

		protected Dictionary<long, Object> InternalGetPlayerItemMappping()
		{
			try
			{
				Object rawPlayerItemMapping = BaseObject.InvokeEntityMethod(BackingObject, PlayerMapGetPlayerItemMappingMethod);
				Dictionary<long, Object> allPlayersMapping = UtilityFunctions.ConvertDictionary<long>(rawPlayerItemMapping);

				return allPlayersMapping;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new Dictionary<long, Object>();
			}
		}

		public long GetServerVirtualPlayerId()
		{
			ulong serverSteamId = 0;
			foreach (var entry in WorldManager.Instance.Checkpoint.Players.Dictionary)
			{
				if (entry.Value.PlayerId == 0L)
				{
					serverSteamId = entry.Value.SteamID;
				}
			}

			long serverVirtualPlayerId = 0;
			List<long> playerIds = GetPlayerIdsFromSteamId(serverSteamId);
			if (playerIds.Count == 0)
			{
				serverVirtualPlayerId = BaseEntity.GenerateEntityId();
				if (serverVirtualPlayerId < 0)
					serverVirtualPlayerId = -serverVirtualPlayerId;

				MyObjectBuilder_Checkpoint.PlayerItem playerItem = new MyObjectBuilder_Checkpoint.PlayerItem();
				playerItem.Name = "Server";
				playerItem.IsDead = false;
				playerItem.Model = "";
				playerItem.PlayerId = serverVirtualPlayerId;
				playerItem.SteamId = serverSteamId;

				List<MyObjectBuilder_Checkpoint.PlayerItem> dummyList = new List<MyObjectBuilder_Checkpoint.PlayerItem>();
				dummyList.Add(playerItem);
				BaseObject.InvokeEntityMethod(BackingObject, PlayerMapAddPlayerItemMappingMethod, new object[] { dummyList });
			}
			else
			{
				serverVirtualPlayerId = playerIds[0];
			}

			return serverVirtualPlayerId;
		}

		#endregion
	}

	public class PlayerManager
	{
		#region "Attributes"

		private static PlayerManager m_instance;

		public static string PlayerManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string PlayerManagerClass = "08FBF1782D25BEBDA2070CAF8CE47D72";

		public static string PlayerManagerPlayerMapField = "3F86E23829227B55C95CEB9F813578B2";

		#endregion

		#region "Constructors and Initializers"

		protected PlayerManager()
		{
			m_instance = this;

			Console.WriteLine("Finished loading PlayerManager");
		}

		#endregion

		#region "Properties"

		public static PlayerManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new PlayerManager();

				return m_instance;
			}
		}

		public Object BackingObject
		{
			get
			{
				Object backingObject = WorldManager.Instance.InternalGetPlayerManager();

				return backingObject;
			}
		}

		public PlayerMap PlayerMap
		{
			get { return PlayerMap.Instance; }
		}

		public List<ulong> ConnectedPlayers
		{
			get
			{
				List<ulong> connectedPlayers = new List<ulong>(ServerNetworkManager.Instance.GetConnectedPlayers());

				return connectedPlayers;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PlayerManagerNamespace, PlayerManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PlayerManager");
				bool result = true;
				result &= BaseObject.HasField(type1, PlayerManagerPlayerMapField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public Object InternalGetPlayerMap()
		{
			Object playerMap = BaseObject.GetEntityFieldValue(BackingObject, PlayerManagerPlayerMapField);

			return playerMap;
		}

		public void KickPlayer(ulong steamId)
		{
			ServerNetworkManager.Instance.KickPlayer(steamId);
		}

		public void BanPlayer(ulong steamId)
		{
			ServerNetworkManager.Instance.SetPlayerBan(steamId, true);
		}

		public void UnBanPlayer(ulong steamId)
		{
			ServerNetworkManager.Instance.SetPlayerBan(steamId, false);
		}

		public bool IsUserAdmin(ulong remoteUserId)
		{
			bool result = false;

			List<string> adminUsers = SandboxGameAssemblyWrapper.Instance.GetServerConfig().Administrators;
			foreach (string userId in adminUsers)
			{
				if (remoteUserId.ToString().Equals(userId))
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
