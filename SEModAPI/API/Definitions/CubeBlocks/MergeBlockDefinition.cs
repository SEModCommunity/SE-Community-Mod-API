using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class MergeBlockDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public MergeBlockDefinition(MyObjectBuilder_MergeBlockDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Merge Block Strength
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Merge Block Strength.")]
		public float Strength
		{
			get { return GetSubTypeDefinition().Strength; }
			set
			{
				if (GetSubTypeDefinition().Strength.Equals(value)) return;
				GetSubTypeDefinition().Strength = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_MergeBlockDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_MergeBlockDefinition)m_baseDefinition;
		}

		#endregion
	}
}