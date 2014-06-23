using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class GravityGeneratorDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public GravityGeneratorDefinition(MyObjectBuilder_GravityGeneratorDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Gravity generator required power input
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Gravity generator required power input.")]
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

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public virtual MyObjectBuilder_GravityGeneratorDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_GravityGeneratorDefinition)m_baseDefinition;
		}

		#endregion
	}
}
