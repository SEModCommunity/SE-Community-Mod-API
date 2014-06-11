using System;
using System.Collections.Generic;
using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class ComponentsDefinition : PhysicalItemsDefinition<MyObjectBuilder_ComponentDefinition>
	{
		#region "Constructors and Initializers"

		public ComponentsDefinition(MyObjectBuilder_ComponentDefinition definition)
			: base(definition)
		{
		}

		#endregion

        #region "Properties"

		public int MaxIntegrity
		{
			get { return m_definition.MaxIntegrity; }
			set
			{
				if (m_definition.MaxIntegrity == value) return;
				m_definition.MaxIntegrity = value;
				Changed = true;
			}
		}

		public float DropProbability
		{
			get { return m_definition.DropProbability; }
			set
			{
				if (m_definition.DropProbability == value) return;
				m_definition.DropProbability = value;
				Changed = true;
			}
		}

		#endregion
	}
}
