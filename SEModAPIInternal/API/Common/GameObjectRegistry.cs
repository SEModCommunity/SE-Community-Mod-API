using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Entity;
using System.Runtime.InteropServices;

namespace SEModAPIInternal.API.Common
{
	public struct GameObjectTypeEntry
	{
		public Guid source;
		public Type gameType;
		public Type apiType;
		public bool enabled;
		public GameObjectCategory category;

		public override int GetHashCode()
		{
			return gameType.GetHashCode();
		}
	}

	public struct GameObjectCategory
	{
		public ushort id;
		public string name;
	}

	public class GameObjectRegistry
	{
		#region "Attributes"

		private static GameObjectRegistry m_instance;

		private Dictionary<Guid, HashSet<GameObjectTypeEntry>> m_registry;
		private Dictionary<Type, Type> m_typeMap;
		private GameObjectCategory m_generalCategory;

		#endregion

		#region "Constructors and Initializers"

		protected GameObjectRegistry()
		{
			m_registry = new Dictionary<Guid, HashSet<GameObjectTypeEntry>>();
			m_typeMap = new Dictionary<Type, Type>();
			m_generalCategory = new GameObjectCategory();
			m_generalCategory.id = 0;
			m_generalCategory.name = "General";
		}

		#endregion

		#region "Properties"

		public static GameObjectRegistry Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new GameObjectRegistry();

				return m_instance;
			}
		}

		public Dictionary<Type, Type> TypeMap
		{
			get
			{
				return new Dictionary<Type, Type>(m_typeMap);
			}
		}

		#endregion

		#region "Methods"

		public bool ContainsGameType(Type gameType)
		{
			if (gameType == null)
				return false;

			return m_typeMap.ContainsKey(gameType);
		}

		public bool ContainsAPIType(Type apiType)
		{
			if (apiType == null)
				return false;

			return m_typeMap.ContainsValue(apiType);
		}

		public Type GetAPIType(Type gameType)
		{
			if (!ContainsGameType(gameType))
				return null;

			return m_typeMap[gameType];
		}

		public List<Type> GetGameTypes()
		{
			List<Type> types = new List<Type>(m_typeMap.Keys.ToList());
			return types;
		}

		public List<Type> GetAPITypes()
		{
			List<Type> types = new List<Type>(m_typeMap.Values.ToList());
			return types;
		}

		public void Register(Type gameType, Type apiType)
		{
			Register(gameType, apiType, m_generalCategory);
		}

		public void Register(Type gameType, Type apiType, GameObjectCategory category)
		{
			if (apiType == null || gameType == null)
				return;
			if (!typeof(MyObjectBuilder_Base).IsAssignableFrom(gameType))
				return;
			if (!typeof(BaseObject).IsAssignableFrom(apiType))
				return;
			if (m_typeMap.ContainsKey(gameType))
				return;
			if (m_typeMap.ContainsValue(apiType))
				return;
			if (!ValidateRegistration(gameType, apiType))
				return;

			GuidAttribute guid = (GuidAttribute)Assembly.GetCallingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
			Guid guidValue = new Guid(guid.Value);

			GameObjectTypeEntry entry = new GameObjectTypeEntry();
			entry.source = guidValue;
			entry.gameType = gameType;
			entry.apiType = apiType;
			entry.enabled = true;
			entry.category = category;

			m_typeMap.Add(gameType, apiType);

			if (m_registry.ContainsKey(guidValue))
			{
				HashSet<GameObjectTypeEntry> hashSet = m_registry[guidValue];
				hashSet.Add(entry);
			}
			else
			{
				HashSet<GameObjectTypeEntry> hashSet = new HashSet<GameObjectTypeEntry>();
				hashSet.Add(entry);
				m_registry.Add(guidValue, hashSet);
			}
		}

		public void Unregister()
		{
			GuidAttribute guid = (GuidAttribute)Assembly.GetCallingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
			Guid guidValue = new Guid(guid.Value);

			if (!m_registry.ContainsKey(guidValue))
				return;

			HashSet<GameObjectTypeEntry> hashSet = m_registry[guidValue];
			foreach (var entry in hashSet)
			{
				if (m_typeMap.ContainsKey(entry.gameType) || m_typeMap.ContainsValue(entry.apiType))
					m_typeMap.Remove(entry.gameType);
			}

			m_registry.Remove(guidValue);
		}

		public void Unregister(Type gameType)
		{
			if (gameType == null)
				return;
			if (!m_typeMap.ContainsKey(gameType))
				return;

			GuidAttribute guid = (GuidAttribute)Assembly.GetCallingAssembly().GetCustomAttributes(typeof(GuidAttribute), true)[0];
			Guid guidValue = new Guid(guid.Value);

			HashSet<GameObjectTypeEntry> hashSet = m_registry[guidValue];
			foreach (var entry in hashSet)
			{
				if (entry.gameType == gameType)
				{
					hashSet.Remove(entry);
					break;
				}
			}

			m_typeMap.Remove(gameType);
		}

		protected virtual bool ValidateRegistration(Type gameType, Type apiType)
		{
			return true;
		}

		#endregion
	}
}
