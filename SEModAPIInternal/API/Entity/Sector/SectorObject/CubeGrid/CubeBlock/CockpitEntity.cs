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

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition)
			: base(parent, definition)
		{
		}

		public CockpitEntity(CubeGridEntity parent, MyObjectBuilder_Cockpit definition, Object backingObject)
			: base(parent, definition, backingObject)
		{
		}

		#endregion

		#region "Properties"

		[Category("Cockpit")]
		public bool ControlThrusters
		{
			get { return GetSubTypeEntity().ControlThrusters; }
		}

		[Category("Cockpit")]
		public bool ControlWheels
		{
			get { return GetSubTypeEntity().ControlWheels; }
		}

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

		[Category("Cockpit")]
		public bool IsPassengerSeat
		{
			get
			{
				if (GetSubTypeEntity().SubtypeName == "PassengerSeatLarge")
					return true;

				if (GetSubTypeEntity().SubtypeName == "PassengerSeatSmall")
					return true;

				return false;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Cockpit GetSubTypeEntity()
		{
			return (MyObjectBuilder_Cockpit)ObjectBuilder;
		}

		#endregion
	}
}
