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
	public class MergeBlockEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string MergeBlockNamespace = "AAD9061F948E6A3635200145188D64A9";
		public static string MergeBlockClass = "D6D88AF33E0073B53DC2A3445C9F12EC";

		#endregion

		#region "Constructors and Intializers"

		public MergeBlockEntity(CubeGridEntity parent, MyObjectBuilder_MergeBlock definition)
			: base(parent, definition)
		{
		}

		public MergeBlockEntity(CubeGridEntity parent, MyObjectBuilder_MergeBlock definition, Object backingObject)
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
		new internal MyObjectBuilder_MergeBlock GetSubTypeEntity()
		{
			return (MyObjectBuilder_MergeBlock)BaseEntity;
		}

		#endregion
	}
}
