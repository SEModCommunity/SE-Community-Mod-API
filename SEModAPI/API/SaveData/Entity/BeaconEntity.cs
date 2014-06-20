using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPI.API.SaveData.Entity
{
	public class BeaconEntity : FunctionalBlockEntity<MyObjectBuilder_Beacon>
	{
		#region "Constructors and Initializers"

		public BeaconEntity(MyObjectBuilder_Beacon definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion
	}
}
