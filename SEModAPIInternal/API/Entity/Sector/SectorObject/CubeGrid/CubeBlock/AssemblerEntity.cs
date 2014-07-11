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

		public static string AssemblerNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string AssemblerClass = "B5257F4B8254F1D432BFCE1B5DCC1A5E";

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
