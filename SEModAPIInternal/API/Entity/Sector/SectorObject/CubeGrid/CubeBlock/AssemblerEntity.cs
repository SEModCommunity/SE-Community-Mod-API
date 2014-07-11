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
	public class AssemblerEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string AssemblerNamespace = "";
		public static string AssemblerClass = "";

		#endregion

		#region "Constructors and Intializers"

		public AssemblerEntity(CubeGridEntity parent, MyObjectBuilder_Assembler definition)
			: base(parent, definition)
		{
		}

		public AssemblerEntity(CubeGridEntity parent, MyObjectBuilder_Assembler definition, Object backingObject)
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
		new internal MyObjectBuilder_Assembler GetSubTypeEntity()
		{
			return (MyObjectBuilder_Assembler)BaseEntity;
		}

		#endregion
	}
}
