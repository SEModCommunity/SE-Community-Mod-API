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
	public class InteriorLightEntity : LightEntity
	{
		#region "Attributes"

		public static string InteriorLightNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string InteriorLightClass = "05750D6A5B237D5ABEB54E060707026B";

		#endregion

		#region "Constructors and Initializers"

		public InteriorLightEntity(CubeGridEntity parent, MyObjectBuilder_InteriorLight definition)
			: base(parent, definition)
		{
		}

		public InteriorLightEntity(CubeGridEntity parent, MyObjectBuilder_InteriorLight definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion
	}
}
