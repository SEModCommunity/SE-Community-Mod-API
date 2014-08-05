using System;
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
	[DataContract(Name = "LandingGearEntityProxy")]
	public class LandingGearEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string LandingGearNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string LandingGearClass = "5C73AAF1736F3AA9956574C6D9A2EEBE";

		#endregion

		#region "Constructors and Intializers"

		public LandingGearEntity(CubeGridEntity parent, MyObjectBuilder_LandingGear definition)
			: base(parent, definition)
		{
		}

		public LandingGearEntity(CubeGridEntity parent, MyObjectBuilder_LandingGear definition, Object backingObject)
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
		new internal MyObjectBuilder_LandingGear GetSubTypeEntity()
		{
			return (MyObjectBuilder_LandingGear)ObjectBuilder;
		}

		#endregion
	}
}
