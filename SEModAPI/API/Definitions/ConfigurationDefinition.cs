using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
    public class ConfigurationDefinition : OverLayerDefinition<MyObjectBuilder_Configuration>
	{
		#region "Constructors and Initializers"

		public ConfigurationDefinition(MyObjectBuilder_Configuration definition): base(definition)
		{}

		#endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_Configuration definition)
        {
            return null;
        }

        #endregion
    }
}
