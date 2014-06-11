using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class BlueprintsDefinition : BaseDefinition<MyObjectBuilder_BlueprintDefinition>
	{
		#region "Constructors and Initializers"

		public BlueprintsDefinition(MyObjectBuilder_BlueprintDefinition definition)
			: base(definition)
		{
		}

		#endregion
	}
}
