using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;

using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract(Name = "VoxelMapProxy")]
	public class VoxelMap : BaseEntity
	{
		#region "Attributes"

		private static Type m_internalType;

		public static string VoxelMapNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string VoxelMapClass = "6EC806B54BA319767DA878841A56ECD8";

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

		[IgnoreDataMember]
		[Category("Voxel Map")]
		[Browsable(false)]
		[ReadOnly(true)]
		new internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(VoxelMapNamespace, VoxelMapClass);
				return m_internalType;
			}
		}

		[DataMember]
		[Category("Voxel Map")]
		[Browsable(true)]
		[ReadOnly(true)]
		public override string Name
		{
			get { return Filename; }
		}

		[IgnoreDataMember]
		[Category("Voxel Map")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_VoxelMap ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_VoxelMap)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[DataMember]
		[Category("Voxel Map")]
		[ReadOnly(true)]
		public string Filename
		{
			get { return ObjectBuilder.Filename; }
			private set
			{
				//Do nothing!
			}
		}

		#endregion

		#region "Methods"

		protected void InternalUpdateVoxelMap()
		{
			try
			{
				//TODO - Add methods to re-load voxel map if file name changes
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
