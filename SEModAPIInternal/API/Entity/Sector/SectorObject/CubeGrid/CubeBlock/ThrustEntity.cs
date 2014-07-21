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
	public class ThrustEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string ThrustNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string ThrustClass = "A52459FBA230B557AC325120832EB494";

		#endregion

		#region "Constructors and Intializers"

		public ThrustEntity(CubeGridEntity parent, MyObjectBuilder_Thrust definition)
			: base(parent, definition)
		{
		}

		public ThrustEntity(CubeGridEntity parent, MyObjectBuilder_Thrust definition, Object backingObject)
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
		new internal MyObjectBuilder_Thrust GetSubTypeEntity()
		{
			return (MyObjectBuilder_Thrust)ObjectBuilder;
		}

		#endregion
	}
}
