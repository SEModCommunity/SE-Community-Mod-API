using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class ScenariosDefinition : BaseDefinition<MyObjectBuilder_ScenarioDefinition>
	{
		#region "Constructors and Initializers"

		public ScenariosDefinition(MyObjectBuilder_ScenarioDefinition definition)
			: base(definition)
		{
		}

		#endregion
	}
}
