using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	class TransparentMaterialsDefinition : OverLayerDefinition<MyObjectBuilder_TransparentMaterial>
	{
		#region "Constructors and Initializers"

		public TransparentMaterialsDefinition(MyObjectBuilder_TransparentMaterial definition)
			: base(definition)
		{}

		#endregion
		
		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_TransparentMaterial definition)
		{
			return definition.Name;
		}

		#endregion
	}
}
