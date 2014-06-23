using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class OreDetectorDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public OreDetectorDefinition(MyObjectBuilder_OreDetectorDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Ore Detector maximum range
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Ore Detector maximum range.")]
		public float RequiredPowerInput
		{
			get { return GetSubTypeDefinition().MaximumRange; }
			set
			{
				if (GetSubTypeDefinition().MaximumRange.Equals(value)) return;
				GetSubTypeDefinition().MaximumRange = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_OreDetectorDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_OreDetectorDefinition)m_baseDefinition;
		}

		#endregion
	}
}