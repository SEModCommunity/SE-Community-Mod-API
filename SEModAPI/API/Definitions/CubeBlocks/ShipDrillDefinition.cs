using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class ShipDrillDefinition: BlockDefinition
	{
		#region "Constructors and Initializers"

		public ShipDrillDefinition(MyObjectBuilder_ShipDrillDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Ship drill sensor radius
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Ship drill sensor radius.")]
		public float RequiredPowerInput
		{
			get { return GetSubTypeDefinition().SensorRadius; }
			set
			{
				if (GetSubTypeDefinition().SensorRadius.Equals(value)) return;
				GetSubTypeDefinition().SensorRadius = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Ship drill sensor offset
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Ship drill sensor offset.")]
		public float MaxForceMagnitude
		{
			get { return GetSubTypeDefinition().SensorOffset; }
			set
			{
				if (GetSubTypeDefinition().SensorOffset.Equals(value)) return;
				GetSubTypeDefinition().SensorOffset = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_ShipDrillDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_ShipDrillDefinition)m_baseDefinition;
		}

		#endregion
	}
}
