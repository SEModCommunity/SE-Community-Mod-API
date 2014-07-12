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
	public class ShipGrinderEntity : FunctionalBlockEntity
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

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_ShipGrinder GetSubTypeEntity()
		{
			return (MyObjectBuilder_ShipGrinder)BaseEntity;
		}

		#endregion
	}
}
