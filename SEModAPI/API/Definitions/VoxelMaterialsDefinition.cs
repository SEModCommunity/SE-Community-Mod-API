using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class VoxelMaterialsDefinition : BaseDefinition<MyObjectBuilder_VoxelMaterialDefinition>
	{
		#region "Constructors and Initializers"

		public VoxelMaterialsDefinition(MyObjectBuilder_VoxelMaterialDefinition definition)
			: base(definition)
		{
		}

		#endregion
	}
}
