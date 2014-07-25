using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

using VRage.Serialization;

namespace SEModAPIInternal.API.Common
{
	public class PlayerMap
	{
		#region "Attributes"

		private static PlayerMap m_instance;

		public static string PlayerMapNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string PlayerMapClass = "E4C2964159826A46D102C2D7FDDC0733";
		public static string PlayerMapEntityToPlayerMappingMethod = "79B43F2C2366136E88B5B7064CA93A74";
		public static string PlayerMapSteamIdToPlayerMappingMethod = "AC5FA6C0D87D43E4B5550A1BB7812DEB";
		public static string PlayerMapGetSerializableDictionaryMethod = "460B7921B2E774D61F63929C4032F1AC";

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

		protected Object GetPlayerFromSteamId(ulong steamId)
		{
			try
			{
				Object result = BaseObject.InvokeEntityMethod(BackingObject, PlayerMapSteamIdToPlayerMappingMethod, new object[] { steamId });

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected ulong GetSteamIdFromPlayer(Object player)
		{
			try
			{
				FieldInfo steamIdField = BaseObject.GetEntityField(player, PlayerMapEntrySteamIdField);
				ulong steamId = (ulong)steamIdField.GetValue(player);

				return steamId;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
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
				LogManager.GameLog.WriteLine(ex);
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
				LogManager.GameLog.WriteLine(ex);
				return new SerializableDictionary<long, ulong>();
			}
		}

		#endregion
	}

	public class PlayerManager
	{
		#region "Attributes"

		private static PlayerManager m_instance;
		private PlayerMap m_playerMap;

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

		#endregion

		#region "Methods"

		public Object InternalGetPlayerMap()
		{
			FieldInfo playerMapField = BaseObject.GetEntityField(BackingObject, PlayerManagerPlayerMapField);
			Object playerMap = playerMapField.GetValue(BackingObject);

			return playerMap;
		}

		#endregion
	}
}
