using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class GlobalEventsDefinition : ObjectBuilderDefinition<MyObjectBuilder_GlobalEventDefinition>
	{
		#region "Constructors and Initializers"

		public GlobalEventsDefinition(MyObjectBuilder_GlobalEventDefinition definition)
			: base(definition)
		{
		}

		#endregion

		#region "Properties"

		public MyGlobalEventTypeEnum EventType
		{
			get { return m_definition.EventType; }
			set
			{
				if (m_definition.EventType == value) return;
				m_definition.EventType = value;
				Changed = true;
			}
		}

		public long MinActivation
		{
			get { return m_definition.MinActivationTimeMs.Value; }
            set
            {
				if (m_definition.MinActivationTimeMs == value) return;
				m_definition.MinActivationTimeMs = value;
                Changed = true;
            }
		}

		public long MaxActivation
		{
			get { return m_definition.MaxActivationTimeMs.Value; }
            set
            {
				if (m_definition.MaxActivationTimeMs == value) return;
				m_definition.MaxActivationTimeMs = value;
                Changed = true;
            }
		}

		public long FirstActivation
		{
			get { return m_definition.FirstActivationTimeMs.Value; }
            set
            {
				if (m_definition.FirstActivationTimeMs == value) return;
				m_definition.FirstActivationTimeMs = value;
                Changed = true;
            }
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class GlobalEventsDefinitionsWrapper : BaseDefinitionsWrapper<MyObjectBuilder_GlobalEventDefinition, GlobalEventsDefinition>
	{
		#region "Constructors and Initializers"

		public GlobalEventsDefinitionsWrapper(MyObjectBuilder_GlobalEventDefinition[] definitions)
			: base(definitions)
		{
		}

		#endregion

		#region "Properties"

		new public bool Changed
		{
			get
			{
				foreach (var def in m_definitions)
				{
					if (def.Value.Changed)
						return true;
				}

				return false;
			}
			set
			{
				base.Changed = value;
			}
		}

		public MyObjectBuilder_GlobalEventDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_GlobalEventDefinition[] temp = new MyObjectBuilder_GlobalEventDefinition[m_definitions.Count];
				GlobalEventsDefinition[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion

		#region "Methods"

		public int IndexOf(GlobalEventsDefinition item)
		{
			int index = 0;
			bool foundMatch = false;
			foreach (var def in m_definitions)
			{
				if (def.Value == item)
				{
					foundMatch = true;
					break;
				}

				index++;
			}

			if (foundMatch)
				return index;
			else
				return -1;
		}

		#endregion
	}
}
