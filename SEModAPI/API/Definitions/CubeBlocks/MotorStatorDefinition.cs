using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class MotorStatorDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public MotorStatorDefinition(MyObjectBuilder_MotorStatorDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Motor stator required power input
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Motor stator required power input.")]
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
		/// The current Rotor or Stator Max force magnitude
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Gravity generator required power input.")]
		public float MaxForceMagnitude
		{
			get { return GetSubTypeDefinition().MaxForceMagnitude; }
			set
			{
				if (GetSubTypeDefinition().MaxForceMagnitude.Equals(value)) return;
				GetSubTypeDefinition().MaxForceMagnitude = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_MotorStatorDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_MotorStatorDefinition)m_baseDefinition;
		}

		#endregion
	}
}