using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class ShipGrinderEntity : ShipToolBaseEntity
	{
		#region "Attributes"

		public static string ShipGrinderNamespace = "";
		public static string ShipGrinderClass = "";

		#endregion

		#region "Constructors and Intializers"

		public ShipGrinderEntity(CubeGridEntity parent, MyObjectBuilder_ShipGrinder definition)
			: base(parent, definition)
		{
		}

		public ShipGrinderEntity(CubeGridEntity parent, MyObjectBuilder_ShipGrinder definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"
		#endregion

		#region "Methods"
		#endregion
	}
}
