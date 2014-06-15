using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPI.API.SaveData.Entity
{
	public class MedicalRoomEntity : CubeBlock<MyObjectBuilder_MedicalRoom>
	{
		#region "Constructors and Initializers"

		public MedicalRoomEntity(MyObjectBuilder_MedicalRoom definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		public ulong SteamUserId
		{
			get { return m_baseDefinition.SteamUserId; }
		}

		#endregion
	}
}
