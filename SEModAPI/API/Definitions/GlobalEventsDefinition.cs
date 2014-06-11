using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.API.Definitions;

namespace SEModAPI.API
{
	public class GlobalEventsDefinition : OverLayerDefinition<MyObjectBuilder_GlobalEventDefinition>
	{
		#region "Constructors and Initializers"

		public GlobalEventsDefinition(MyObjectBuilder_GlobalEventDefinition definition): base(definition)
		{}

		#endregion

		#region "Properties"

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

		public long MinActivation
		{
			get { return m_baseDefinition.MinActivationTimeMs.Value; }
            set
            {
				if (m_baseDefinition.MinActivationTimeMs == value) return;
				m_baseDefinition.MinActivationTimeMs = value;
                Changed = true;
            }
		}

		public long MaxActivation
		{
			get { return m_baseDefinition.MaxActivationTimeMs.Value; }
            set
            {
				if (m_baseDefinition.MaxActivationTimeMs == value) return;
				m_baseDefinition.MaxActivationTimeMs = value;
                Changed = true;
            }
		}

		public long FirstActivation
		{
			get { return m_baseDefinition.FirstActivationTimeMs.Value; }
            set
            {
				if (m_baseDefinition.FirstActivationTimeMs == value) return;
				m_baseDefinition.FirstActivationTimeMs = value;
                Changed = true;
            }
		}

		#endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_GlobalEventDefinition definition)
	    {
	        return definition.DisplayName;
        }

        #endregion
    }

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

    public class GlobalEventsDefinitionsWrapper : OverLayerDefinitionsManager<MyObjectBuilder_GlobalEventDefinition, GlobalEventsDefinition>
	{
		#region "Constructors and Initializers"

		public GlobalEventsDefinitionsWrapper(MyObjectBuilder_GlobalEventDefinition[] definitions): base(definitions)
		{}

		#endregion

		#region "Methods"

        protected override GlobalEventsDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_GlobalEventDefinition definition)
        {
            return new GlobalEventsDefinition(definition);
        }

        protected override MyObjectBuilder_GlobalEventDefinition GetBaseTypeOf(GlobalEventsDefinition overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(GlobalEventsDefinition overLayer)
        {
            return overLayer.Changed;
        }

		#endregion
	}
}
