using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class SolarPanelDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public SolarPanelDefinition(MyObjectBuilder_SolarPanelDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Assembler panel offset
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Assembler panel offset.")]
		public float PanelOffset
		{
			get { return GetSubTypeDefinition().PanelOffset; }
			set
			{
				if (GetSubTypeDefinition().PanelOffset.Equals(value)) return;
				GetSubTypeDefinition().PanelOffset = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Solar Panel Orientation
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Solar Panel Orientation.")]
		public VRageMath.Vector3 PanelOrientation
		{
			get { return GetSubTypeDefinition().PanelOrientation; }
			set
			{
				if (GetSubTypeDefinition().PanelOrientation.Equals(value)) return;
				GetSubTypeDefinition().PanelOrientation = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Solar panel "two sided" property
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Solar panel 'two sided' property.")]
		public bool TwoSidedPanel
		{
			get { return GetSubTypeDefinition().TwoSidedPanel; }
			set
			{
				if (GetSubTypeDefinition().TwoSidedPanel == value) return;
				GetSubTypeDefinition().TwoSidedPanel = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_SolarPanelDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_SolarPanelDefinition)m_baseDefinition;
		}

		#endregion
	}
}