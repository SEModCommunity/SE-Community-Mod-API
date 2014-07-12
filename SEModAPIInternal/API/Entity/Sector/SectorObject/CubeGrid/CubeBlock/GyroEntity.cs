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
	public class GyroEntity : FunctionalBlockEntity
	{
		#region "Attributes"

		public static string GyroNamespace = "";
		public static string GyroClass = "";

		#endregion

		#region "Constructors and Intializers"

		public GyroEntity(CubeGridEntity parent, MyObjectBuilder_Gyro definition)
			: base(parent, definition)
		{
		}

		public GyroEntity(CubeGridEntity parent, MyObjectBuilder_Gyro definition, Object backingObject)
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
		new internal MyObjectBuilder_Gyro GetSubTypeEntity()
		{
			return (MyObjectBuilder_Gyro)BaseEntity;
		}

		#endregion
	}
}
