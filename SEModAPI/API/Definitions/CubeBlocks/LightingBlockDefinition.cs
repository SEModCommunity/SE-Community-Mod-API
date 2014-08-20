using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class LightingBlockDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public LightingBlockDefinition(MyObjectBuilder_LightingBlockDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Lithing block required power input
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Lithing block required power input.")]
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
		/// The current Lithing block Max light fall off
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Lithing block Max light fall off.")]
		public float MaxLightFalloff
		{
			get { return GetSubTypeDefinition().LightFalloff.Max; }
			set
			{
				if (GetSubTypeDefinition().LightFalloff.Max.Equals(value)) return;
				GetSubTypeDefinition().LightFalloff.Max = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Lithing block Min light fall off
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Lithing block Min light fall off.")]
		public float MinLightFalloff
		{
			get { return GetSubTypeDefinition().LightFalloff.Min; }
			set
			{
				if (GetSubTypeDefinition().LightFalloff.Min.Equals(value)) return;
				GetSubTypeDefinition().LightFalloff.Min = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Lithing block Default light fall off
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Lithing block Default light fall off.")]
		public float DefaultLightFalloff
		{
			get { return GetSubTypeDefinition().LightFalloff.Default; }
			set
			{
				if (GetSubTypeDefinition().LightFalloff.Default.Equals(value)) return;
				GetSubTypeDefinition().LightFalloff.Default = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new public MyObjectBuilder_LightingBlockDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_LightingBlockDefinition)m_baseDefinition;
		}

		#endregion
	}
}
