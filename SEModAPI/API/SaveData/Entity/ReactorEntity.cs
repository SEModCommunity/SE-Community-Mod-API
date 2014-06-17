using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPI.API.SaveData.Entity
{
	public class ReactorEntity : CubeBlock<MyObjectBuilder_Reactor>
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;

		#endregion

		#region "Constructors and Initializers"

		public ReactorEntity(MyObjectBuilder_Reactor definition)
			: base(definition)
		{
			EntityId = definition.EntityId;
			m_Inventory = new InventoryEntity(definition.Inventory);
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		new public MyObjectBuilder_Reactor BaseDefinition
		{
			get
			{
				m_baseDefinition.Inventory = m_Inventory.BaseDefinition;
				return m_baseDefinition;
			}
		}

		[Browsable(false)]
		public InventoryEntity Inventory
		{
			get { return m_Inventory; }
		}

		public float Fuel
		{
			get
			{
				float fuelMass = 0;
				foreach (var item in m_Inventory.Items)
				{
					fuelMass += item.Mass;
				}
				return fuelMass;
			}
		}

		#endregion
	}
}
