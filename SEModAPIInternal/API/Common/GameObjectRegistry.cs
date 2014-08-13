using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Entity;

namespace SEModAPIInternal.API.Common
{
	public class GameObjectRegistry
	{
		#region "Attributes"

		private static GameObjectRegistry m_instance;
		private Dictionary<Type, Type> m_typeMap;

		#endregion

		#region "Constructors and Initializers"

		protected GameObjectRegistry()
		{
			m_typeMap = new Dictionary<Type, Type>();
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

		#endregion

		#region "Methods"

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
		}

		#endregion
	}
}
