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
	public class ReflectorLightEntity : LightEntity
	{
		#region "Attributes"

		public static string ReflectorLightNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ReflectorLightClass = "21308EE2FF83128D511F35390DB03784";

		#endregion

		#region "Constructors and Initializers"

		public ReflectorLightEntity(CubeGridEntity parent, MyObjectBuilder_ReflectorLight definition)
			: base(parent, definition)
		{
		}

		public ReflectorLightEntity(CubeGridEntity parent, MyObjectBuilder_ReflectorLight definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion
	}
}
