using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity;

namespace SEModAPIInternal.API.Common
{
	public class EntityEventManager
	{
		public enum EntityEventType
		{
			OnPlayerJoined,
			OnPlayerLeft,
		}

		public struct EntityEvent
		{
			public EntityEventType type;
			public DateTime timestamp;
			public BaseObject entity;
		}

		#region "Attributes"

		private static EntityEventManager m_instance;
		private List<EntityEvent> m_entityEvents;

		#endregion

		#region "Constructors and Initializers"

		protected EntityEventManager()
		{
			m_entityEvents = new List<EntityEvent>();
		}

		#endregion

		#region "Properties"

		public static EntityEventManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new EntityEventManager();

				return m_instance;
			}
		}

		public List<EntityEvent> EntityEvents
		{
			get { return m_entityEvents; }
		}

		#endregion

		#region "Methods"

		#endregion
	}
}
