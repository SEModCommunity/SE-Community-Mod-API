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
	public class SolarPanelEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string SolarPanelNamespace = "";
		public static string SolarPanelClass = "";

		#endregion

		#region "Constructors and Intializers"

		public SolarPanelEntity(CubeGridEntity parent, MyObjectBuilder_SolarPanel definition)
			: base(parent, definition)
		{
		}

		public SolarPanelEntity(CubeGridEntity parent, MyObjectBuilder_SolarPanel definition, Object backingObject)
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
		new internal MyObjectBuilder_SolarPanel GetSubTypeEntity()
		{
			return (MyObjectBuilder_SolarPanel)BaseEntity;
		}

		#endregion
	}
}
