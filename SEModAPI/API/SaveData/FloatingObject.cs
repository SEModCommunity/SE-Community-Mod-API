using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class FloatingObject : SectorObject<MyObjectBuilder_FloatingObject>
	{
		#region "Constructors and Initializers"

		public FloatingObject(MyObjectBuilder_FloatingObject definition)
			: base(definition)
		{}

		#endregion

		#region "Properties"

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_FloatingObject definition)
		{
			return definition.Item.PhysicalContent.SubtypeName;
		}

		#endregion
	}
}
