using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid
{
	class ConveyorLine : BaseObject
	{
		public ConveyorLine(CubeGridEntity parent, MyObjectBuilder_ConveyorLine definition)
			: base(definition)
		{
		}
	}

	class ConveyorLineManager
	{
	}
}
