using System;
using System.Collections.Generic;
using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
    public class ComponentsDefinition : OverLayerDefinition<MyObjectBuilder_ComponentDefinition>
	{
		#region "Constructors and Initializers"

		public ComponentsDefinition(MyObjectBuilder_ComponentDefinition definition): base(definition)
		{}

		#endregion

        #region "Properties"

		public int MaxIntegrity
		{
			get { return m_baseDefinition.MaxIntegrity; }
			set
			{
                if (m_baseDefinition.MaxIntegrity == value) return;
                m_baseDefinition.MaxIntegrity = value;
				Changed = true;
			}
		}

		public float DropProbability
		{
            get { return m_baseDefinition.DropProbability; }
			set
			{
                if (m_baseDefinition.DropProbability == value) return;
                m_baseDefinition.DropProbability = value;
				Changed = true;
			}
		}

		#endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_ComponentDefinition definition)
        {
            return definition.DisplayName;
        }

        #endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ComponentDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_ComponentDefinition, ComponentsDefinition>
	{
		#region "Constructors and Initializers"

        public ComponentDefinitionsManager(MyObjectBuilder_ComponentDefinition[] definitions): base(definitions)
		{}

		#endregion

		#region "Methods"

		public void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.ComponentDefinitions = this.RawDefinitions;
			m_configSerializer.SaveComponentsContentFile();
		}

        protected override ComponentsDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_ComponentDefinition definition)
        {
            return new ComponentsDefinition(definition);
        }

        protected override MyObjectBuilder_ComponentDefinition GetBaseTypeOf(ComponentsDefinition overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(ComponentsDefinition overLayer)
        {
            return overLayer.Changed;
        }

		#endregion    
	}
}
