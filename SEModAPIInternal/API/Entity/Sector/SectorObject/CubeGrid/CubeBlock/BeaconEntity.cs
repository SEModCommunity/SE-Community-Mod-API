using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class BeaconEntity : FunctionalBlockEntity
	{
		#region "Constructors and Initializers"

		public BeaconEntity(MyObjectBuilder_Beacon definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion
	}
}
