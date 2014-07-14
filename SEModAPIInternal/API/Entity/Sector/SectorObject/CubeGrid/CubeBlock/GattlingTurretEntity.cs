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
	public class GatlingTurretEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string GatlingTurretNamespace = "";
		public static string GatlingTurretClass = "";

		#endregion

		#region "Constructors and Intializers"

		public GatlingTurretEntity(CubeGridEntity parent, MyObjectBuilder_LargeGatlingTurret definition)
			: base(parent, definition)
		{
		}

		public GatlingTurretEntity(CubeGridEntity parent, MyObjectBuilder_LargeGatlingTurret definition, Object backingObject)
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
		new internal MyObjectBuilder_LargeGatlingTurret GetSubTypeEntity()
		{
			return (MyObjectBuilder_LargeGatlingTurret)BaseEntity;
		}

		#endregion
	}
}
