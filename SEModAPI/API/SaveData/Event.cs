using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class Event : OverLayerDefinition<MyObjectBuilder_GlobalEventBase>
	{
		#region "Constructors and Initializers"

		public Event(MyObjectBuilder_GlobalEventBase definition)
			: base(definition)
		{}

		#endregion

		#region "Properties"

		new public string Name
		{
			get { return this.GetNameFrom(m_baseDefinition); }
		}

		public long ActivationTimeMs
		{
			get { return m_baseDefinition.ActivationTimeMs; }
			set
			{
				if (m_baseDefinition.ActivationTimeMs == value) return;
				m_baseDefinition.ActivationTimeMs = value;
				Changed = true;
			}
		}

		public SerializableDefinitionId DefinitionId
		{
			get { return m_baseDefinition.DefinitionId; }
			set
			{
				if (m_baseDefinition.DefinitionId.Equals(value)) return;
				m_baseDefinition.DefinitionId = value;
				Changed = true;
			}
		}

		public bool Enabled
		{
			get { return m_baseDefinition.Enabled; }
			set
			{
				if (m_baseDefinition.Enabled == value) return;
				m_baseDefinition.Enabled = value;
				Changed = true;
			}
		}
		public MyGlobalEventTypeEnum EventType
		{
			get { return m_baseDefinition.EventType; }
			set
			{
				if (m_baseDefinition.EventType == value) return;
				m_baseDefinition.EventType = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_GlobalEventBase definition)
		{
			return m_baseDefinition.DefinitionId.SubtypeId;
		}

		#endregion
	}

	public class EventManager : OverLayerDefinitionsManager<MyObjectBuilder_GlobalEventBase, Event>
	{
		#region "Constructors and Initializers"

		public EventManager(List<MyObjectBuilder_GlobalEventBase> definitions)
			: base(definitions.ToArray())
		{}

		public EventManager(MyObjectBuilder_GlobalEventBase[] definitions)
			: base(definitions)
		{}

		#endregion

		#region "Methods"

		protected override Event CreateOverLayerSubTypeInstance(MyObjectBuilder_GlobalEventBase definition)
		{
			return new Event(definition);
		}

		protected override MyObjectBuilder_GlobalEventBase GetBaseTypeOf(Event overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(Event overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}
