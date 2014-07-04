using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class MedicalRoomEntity : CubeBlockEntity
	{
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

		[Category("Medical Room")]
		public ulong SteamUserId
		{
			get { return GetSubTypeEntity().SteamUserId; }
			set
			{
				if (GetSubTypeEntity().SteamUserId == value) return;
				GetSubTypeEntity().SteamUserId = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_MedicalRoom GetSubTypeEntity()
		{
			return (MyObjectBuilder_MedicalRoom)BaseEntity;
		}

		#endregion
	}
}
