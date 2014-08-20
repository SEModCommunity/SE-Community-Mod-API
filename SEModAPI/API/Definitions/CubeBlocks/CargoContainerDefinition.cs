using System.ComponentModel;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class CargoContainerDefinition : BlockDefinition
	{
		#region "Constructors and Initializers"

		public CargoContainerDefinition(MyObjectBuilder_CargoContainerDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// The current Cargo Container Inventory Size
		/// </summary>
		[Browsable(true)]
		[ReadOnly(false)]
		[Description("Get or set the current Cargo Container Inventory Size.")]
		public VRageMath.Vector3 InventorySize
		{
			get { return GetSubTypeDefinition().InventorySize; }
			set
			{
				if (GetSubTypeDefinition().InventorySize == value) return;
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
		new public MyObjectBuilder_CargoContainerDefinition GetSubTypeDefinition()
		{
			return (MyObjectBuilder_CargoContainerDefinition)m_baseDefinition;
		}

		#endregion

	}
}