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
	public class RefineryEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string RefineryNamespace = "";
		public static string RefineryClass = "";

		#endregion

		#region "Constructors and Intializers"

		public RefineryEntity(CubeGridEntity parent, MyObjectBuilder_Refinery definition)
			: base(parent, definition)
		{
		}

		public RefineryEntity(CubeGridEntity parent, MyObjectBuilder_Refinery definition, Object backingObject)
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
		new internal MyObjectBuilder_Refinery GetSubTypeEntity()
		{
			return (MyObjectBuilder_Refinery)BaseEntity;
		}

		#endregion
	}
}
