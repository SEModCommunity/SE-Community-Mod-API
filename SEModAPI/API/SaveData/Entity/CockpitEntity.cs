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
	public class CockpitEntity : TerminalBlockEntity<MyObjectBuilder_Cockpit>
	{
		#region "Constructors and Initializers"

		public CockpitEntity(MyObjectBuilder_Cockpit definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
		}

		#endregion

		#region "Properties"

		[Category("Cockpit")]
		public MyObjectBuilder_AutopilotBase Autopilot
		{
			get { return m_baseDefinition.Autopilot; }
		}

		[Category("Cockpit")]
		public MyObjectBuilder_Character Pilot
		{
			get { return m_baseDefinition.Pilot; }
		}

		#endregion
	}
}
