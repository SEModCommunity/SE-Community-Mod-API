using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class ThrusterDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public ThrusterDefinition(MyObjectBuilder_ThrustDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Thruster force magnitude, or "push capacity"
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Thruster force magnitude, or 'push capacity'.")]
		public float ForceMagnitude
		{
			get { return GetSubTypeDefinition().ForceMagnitude; }
			set
			{
				if (GetSubTypeDefinition().ForceMagnitude.Equals(value)) return;
				GetSubTypeDefinition().ForceMagnitude = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Thruster maximum power consumption
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Thruster maximum power consumption.")]
		public float MaxPowerConsumption
		{
			get { return GetSubTypeDefinition().MaxPowerConsumption; }
			set
			{
				if (GetSubTypeDefinition().MaxPowerConsumption.Equals(value)) return;
				GetSubTypeDefinition().MaxPowerConsumption = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Thruster minimum power consumption
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Thruster minimum power consumption.")]
		public float MinPowerConsumption
		{
			get { return GetSubTypeDefinition().MinPowerConsumption; }
			set
			{
				if (GetSubTypeDefinition().MinPowerConsumption.Equals(value)) return;
				GetSubTypeDefinition().MinPowerConsumption = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_ThrustDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_ThrustDefinition)m_baseDefinition;
		}

		#endregion
	}
}