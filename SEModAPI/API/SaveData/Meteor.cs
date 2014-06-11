using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class Meteor : SectorObject<MyObjectBuilder_Meteor>
	{
		#region "Constructors and Initializers"

		public Meteor(MyObjectBuilder_Meteor definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_Meteor definition)
		{
			return definition.Item.PhysicalContent.SubtypeName;
		}

		#endregion
	}
}
