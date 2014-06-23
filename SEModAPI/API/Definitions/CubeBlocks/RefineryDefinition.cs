using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class RefineryDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public RefineryDefinition(MyObjectBuilder_RefineryDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Refinery materiel efficiency
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Refinery materiel efficiency.")]
		public float MaterialEfficiency
		{
			get { return GetSubTypeDefinition().MaterialEfficiency; }
			set
			{
				if (GetSubTypeDefinition().MaterialEfficiency.Equals(value)) return;
				GetSubTypeDefinition().MaterialEfficiency = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Refinery Refine Speed
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Refinery Refine Speed.")]
		public float RefineSpeed
		{
			get { return GetSubTypeDefinition().RefineSpeed; }
			set
			{
				if (GetSubTypeDefinition().RefineSpeed.Equals(value)) return;
				GetSubTypeDefinition().RefineSpeed = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_RefineryDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_RefineryDefinition)m_baseDefinition;
		}

		#endregion
	}
}