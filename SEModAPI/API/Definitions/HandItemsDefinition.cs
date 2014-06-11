using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
    public class HandItemsDefinition : OverLayerDefinition<MyObjectBuilder_HandItemDefinition>
	{
		#region "Constructors and Initializers"

		public HandItemsDefinition(MyObjectBuilder_HandItemDefinition definition): base(definition)
		{}

		#endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_HandItemDefinition definition)
        {
            return definition.DisplayName;
        }

        #endregion
	}
}
