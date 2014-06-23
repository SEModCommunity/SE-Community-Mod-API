using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class ReactorDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public ReactorDefinition(MyObjectBuilder_ReactorDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Reactor Inventory Size
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Reactor Inventory Size.")]
		public VRageMath.Vector3 InventorySize
		{
			get { return GetSubTypeDefinition().InventorySize; }
			set
			{
				if (GetSubTypeDefinition().InventorySize.Equals(value)) return;
				GetSubTypeDefinition().InventorySize = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		public new virtual MyObjectBuilder_ReactorDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_ReactorDefinition)m_baseDefinition;
		}

		#endregion
	}
}