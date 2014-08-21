using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	[DataContract(Name = "MedicalRoomEntityProxy")]
	public class MedicalRoomEntity : CubeBlockEntity
	{
		#region "Attributes"

		public static string MedicalRoomNamespace = "6DDCED906C852CFDABA0B56B84D0BD74";
		public static string MedicalRoomClass = "1497FAB5CDC67F0A1CD4BC2BA9AFF5D7";

		#endregion

		#region "Constructors and Initializers"

		public MedicalRoomEntity(CubeGridEntity parent, MyObjectBuilder_MedicalRoom definition)
			: base(parent, definition)
		{
		}

		public MedicalRoomEntity(CubeGridEntity parent, MyObjectBuilder_MedicalRoom definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Medical Room")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_MedicalRoom ObjectBuilder
		{
			get
			{
				MyObjectBuilder_MedicalRoom block = (MyObjectBuilder_MedicalRoom)base.ObjectBuilder;


				return block;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Obsolete]
		[Category("Medical Room")]
		public ulong SteamUserId
		{
			get { return ObjectBuilder.SteamUserId; }
		}

		#endregion

		#region "Methods"

		#endregion
	}
}
