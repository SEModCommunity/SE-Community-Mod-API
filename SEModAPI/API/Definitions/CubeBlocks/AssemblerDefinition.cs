using System.ComponentModel;
using System.Xml;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class AssemblerDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public AssemblerDefinition(MyObjectBuilder_AssemblerDefinition definition)
			: base(definition)
		{}

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Assembler operational power consumption
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Assembler operational power consumption.")]
		public float MaxPowerConsumption
		{
			get { return GetSubTypeDefinition().OperationalPowerConsumption; }
			set
			{
				if (GetSubTypeDefinition().OperationalPowerConsumption.Equals(value)) return;
				GetSubTypeDefinition().OperationalPowerConsumption = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Assembler standby power consumption
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Assembler standby power consumption.")]
		public float MinPowerConsumption
		{
			get { return GetSubTypeDefinition().StandbyPowerConsumption; }
			set
			{
				if (GetSubTypeDefinition().StandbyPowerConsumption.Equals(value)) return;
				GetSubTypeDefinition().StandbyPowerConsumption = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Assembler inventory max volume
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Assembler inventory max volume.")]
		public float MovementCoefficient
		{
			get { return GetSubTypeDefinition().InventoryMaxVolume; }
			set
			{
				if (GetSubTypeDefinition().InventoryMaxVolume.Equals(value)) return;
				GetSubTypeDefinition().InventoryMaxVolume = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new public MyObjectBuilder_AssemblerDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_AssemblerDefinition) m_baseDefinition;
		}

		#endregion
	}
}