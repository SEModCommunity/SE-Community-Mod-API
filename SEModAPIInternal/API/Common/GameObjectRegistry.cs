using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Entity;

namespace SEModAPIInternal.API.Common
{
	public struct GameObjectCategory
	{
		public ushort id;
		public string name;
	}

	public class GameObjectRegistry
	{
		#region "Attributes"

		private static GameObjectRegistry m_instance;
		private Dictionary<Type, Type> m_typeMap;
		private Dictionary<GameObjectCategory, List<Type>> m_categoryMap;

		#endregion

		#region "Constructors and Initializers"

		protected GameObjectRegistry()
		{
			m_typeMap = new Dictionary<Type, Type>();
			m_categoryMap = new Dictionary<GameObjectCategory, List<Type>>();
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

		public Dictionary<Type, Type> Registry
		{
			get { return m_typeMap; }
		}

		public Dictionary<GameObjectCategory, List<Type>> CategoryMap
		{
			get { return m_categoryMap; }
		}

		#endregion

		#region "Methods"

		public void Register(Type gameType, Type apiType, GameObjectCategory category)
		{
			if (CategoryMap.ContainsKey(category))
			{
				//Update the existing category list with the new game type
				List<Type> types = CategoryMap[category];
				if (!types.Contains(gameType))
				{
					types.Add(gameType);
				}
			}
			else
			{
				//Create a new entry in the category map with the new game type
				List<Type> types = new List<Type>();
				types.Add(gameType);
				m_categoryMap.Add(category, types);
			}

			//Register the game type
			Register(gameType, apiType);
		}

		public virtual void Register(Type gameType, Type apiType)
		{
			if (apiType == null || gameType == null)
				return;
			if (!typeof(MyObjectBuilder_Base).IsAssignableFrom(gameType))
				return;
			if (!typeof(BaseObject).IsAssignableFrom(apiType))
				return;
			if (Registry.ContainsKey(apiType))
				return;
			if (Registry.ContainsValue(gameType))
				return;

			Registry.Add(gameType, apiType);
		}

		public virtual void Unregister(Type gameType)
		{
			if (gameType == null)
				return;
			if (!Registry.ContainsKey(gameType))
				return;

			Registry.Remove(gameType);

			foreach (var entry in m_categoryMap)
			{
				List<Type> types = entry.Value;
				if (types.Contains(gameType))
				{
					types.Remove(gameType);
				}
			}
		}

		#endregion
	}
}
