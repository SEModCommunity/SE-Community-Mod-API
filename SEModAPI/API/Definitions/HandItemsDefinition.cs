using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class HandItemsDefinition : BaseDefinition<MyObjectBuilder_HandItemDefinition>
	{
		#region "Constructors and Initializers"

		public HandItemsDefinition(MyObjectBuilder_HandItemDefinition definition)
			: base(definition)
		{
		}

		#endregion
	}
}
