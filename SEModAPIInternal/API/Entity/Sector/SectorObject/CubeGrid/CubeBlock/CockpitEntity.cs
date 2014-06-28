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
	public class CockpitEntity : TerminalBlockEntity
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
			get { return GetSubTypeEntity().Autopilot; }
		}

		[Category("Cockpit")]
		public MyObjectBuilder_Character Pilot
		{
			get { return GetSubTypeEntity().Pilot; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Cockpit GetSubTypeEntity()
		{
			return (MyObjectBuilder_Cockpit)BaseEntity;
		}

		#endregion
	}
}
