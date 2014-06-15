using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPI.API.SaveData.Entity
{
	public class CargoContainerEntity : CubeBlock<MyObjectBuilder_CargoContainer>
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;

		#endregion

		#region "Constructors and Initializers"

		public CargoContainerEntity(MyObjectBuilder_CargoContainer definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
			m_Inventory = new InventoryEntity(definition.Inventory);
		}

		#endregion

		#region "Properties"

		new public MyObjectBuilder_CargoContainer BaseDefinition
		{
			get
			{
				m_baseDefinition.Inventory = m_Inventory.BaseDefinition;
				return m_baseDefinition;
			}
		}

		public InventoryEntity Inventory
		{
			get { return m_Inventory; }
		}

		#endregion
	}
}
