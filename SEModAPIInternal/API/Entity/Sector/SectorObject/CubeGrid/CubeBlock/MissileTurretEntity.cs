﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "MissileTurretEntityProxy")]
	public class MissileTurretEntity : TurretBaseEntity
	{
		#region "Attributes"

		public static string MissileTurretNamespace = "";
		public static string MissileTurretClass = "";

		#endregion

		#region "Constructors and Intializers"

		public MissileTurretEntity(CubeGridEntity parent, MyObjectBuilder_LargeMissileTurret definition)
			: base(parent, definition)
		{
		}

		public MissileTurretEntity(CubeGridEntity parent, MyObjectBuilder_LargeMissileTurret definition, Object backingObject)
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
