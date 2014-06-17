using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		[Browsable(false)]
		new public MyObjectBuilder_MedicalRoom BaseDefinition
		{
			get { return m_baseDefinition; }
		}

		public ulong SteamUserId
		{
			get { return m_baseDefinition.SteamUserId; }
			set
			{
				if (m_baseDefinition.SteamUserId == value) return;
				m_baseDefinition.SteamUserId = value;
				Changed = true;
			}
		}

		#endregion
	}
}
