using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class VirtualMassDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public VirtualMassDefinition(MyObjectBuilder_VirtualMassDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Virtual Mass required power input
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Virtual Mass required power input.")]
		public float RequiredPowerInput
		{
			get { return GetSubTypeDefinition().RequiredPowerInput; }
			set
			{
				if (GetSubTypeDefinition().RequiredPowerInput.Equals(value)) return;
				GetSubTypeDefinition().RequiredPowerInput = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Virtual Mass "mass"
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set The current Virtual Mass 'mass'.")]
		public float MaxLightFalloff
		{
			get { return GetSubTypeDefinition().VirtualMass; }
			set
			{
				if (GetSubTypeDefinition().VirtualMass.Equals(value)) return;
				GetSubTypeDefinition().VirtualMass = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_VirtualMassDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_VirtualMassDefinition)m_baseDefinition;
		}

		#endregion
	}
}
