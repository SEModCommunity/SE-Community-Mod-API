using System;
using System.Collections.Generic;
using System.ComponentModel;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class VoxelMap : BaseEntity
	{
		#region "Attributes"

		public static string VoxelMapNamespace = "";
		public static string VoxelMapClass = "";

		#endregion

		#region "Constructors and Initializers"

		public VoxelMap(MyObjectBuilder_VoxelMap definition)
			: base(definition)
		{ }

		public VoxelMap(MyObjectBuilder_VoxelMap definition, Object backingObject)
			: base(definition, backingObject)
		{ }

		#endregion

		#region "Properties"

		[Category("Voxel Map")]
		[Browsable(true)]
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

				if (BackingObject != null)
				{
					Action action = InternalUpdateVoxelMap;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
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

		protected void InternalUpdateVoxelMap()
		{
			try
			{
				InternalUpdateBaseEntity();

				//TODO - Add methods to re-load voxel map if file name changes
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
