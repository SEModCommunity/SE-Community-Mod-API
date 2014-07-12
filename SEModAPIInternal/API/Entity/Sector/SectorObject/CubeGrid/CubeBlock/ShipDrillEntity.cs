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
	public class ShipDrillEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string AssemblerNamespace = "";
		public static string AssemblerClass = "";

		#endregion

		#region "Constructors and Intializers"

		public ShipDrillEntity(CubeGridEntity parent, MyObjectBuilder_Drill definition)
			: base(parent, definition)
		{
		}

		public ShipDrillEntity(CubeGridEntity parent, MyObjectBuilder_Drill definition, Object backingObject)
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
		new internal MyObjectBuilder_Drill GetSubTypeEntity()
		{
			return (MyObjectBuilder_Drill)BaseEntity;
		}

		#endregion
	}
}
