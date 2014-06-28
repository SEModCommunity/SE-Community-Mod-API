using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;

using SEModAPI.API.Definitions;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class VoxelMap : BaseEntity
	{
		#region "Constructors and Initializers"

		public VoxelMap(MyObjectBuilder_VoxelMap definition)
			: base(definition)
		{}

		#endregion

		#region "Properties"

		public override string Name
		{
			get { return Filename; }
		}

		[Category("Voxel Map")]
		public string Filename
		{
			get { return GetSubTypeEntity().Filename; }
			set
			{
				if (GetSubTypeEntity().Filename == value) return;
				GetSubTypeEntity().Filename = value;
				Changed = true;

				//TODO - Adding backing object functionality for this property
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_VoxelMap GetSubTypeEntity()
		{
			return (MyObjectBuilder_VoxelMap)BaseEntity;
		}

		#endregion
	}
}
