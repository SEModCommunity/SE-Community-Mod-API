using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class ScenariosDefinition : OverLayerDefinition<MyObjectBuilder_ScenarioDefinition>
	{
		#region "Constructors and Initializers"

		public ScenariosDefinition(MyObjectBuilder_ScenarioDefinition definition): base(definition)
		{}

		#endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_ScenarioDefinition definition)
        {
            return definition.DisplayName;
        }

        #endregion
	}
}
