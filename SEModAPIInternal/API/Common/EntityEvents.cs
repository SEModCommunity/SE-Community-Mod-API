using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class EntityEventManager
	{
		public enum EntityEventType
		{
			OnPlayerJoined,
			OnPlayerLeft,
			OnBaseEntityMoved,
			OnBaseEntityCreated,
			OnBaseEntityDeleted,
			OnCubeGridMoved,
			OnCubeGridCreated,
			OnCubeGridDeleted,
            OnCubeGridAdded,
			OnCubeBlockCreated,
			OnCubeBlockDeleted,
			OnCharacterCreated,
			OnCharacterDeleted,
		}

		public struct EntityEvent
		{
			public EntityEventType type;
			public DateTime timestamp;
			public Object entity;
			public ushort priority;
		}

		#region "Attributes"

		private static EntityEventManager m_instance;
		private List<EntityEvent> m_entityEvents;
		private List<EntityEvent> m_entityEventsBuffer;
		private bool m_isResourceLocked;

		#endregion

		#region "Constructors and Initializers"

		protected EntityEventManager()
		{
			m_entityEvents = new List<EntityEvent>();
			m_entityEventsBuffer = new List<EntityEvent>();
			m_isResourceLocked = false;

			m_instance = this;

			Console.WriteLine("Finished loading EntityEventManager");
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
			get
			{
				return m_entityEvents;
			}
		}

		public bool ResourceLocked
		{
			get { return m_isResourceLocked; }
			set
			{
				if (value == false)
				{
					m_entityEvents.AddList(m_entityEventsBuffer);
				}

				m_isResourceLocked = value;
			}
		}

		#endregion

		#region "Methods"

		public void AddEvent(EntityEvent newEvent)
		{
			try
			{
				if (ResourceLocked)
				{
					//Only add priority 0 and 1 events to the buffer while the list is locked
					if (newEvent.priority < 2)
						m_entityEventsBuffer.Add(newEvent);
				}
				else
				{
					m_entityEvents.Add(newEvent);
				}
			}
			catch (Exception ex)
			{
				if(SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(ex);
			}
		}

		public void ClearEvents()
		{
			m_entityEvents.Clear();
		}

		#endregion
	}
}
