using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock
{
	public class CargoContainerEntity : TerminalBlockEntity
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

		[Category("Cargo Container")]
		[Browsable(false)]
		public InventoryEntity Inventory
		{
			get { return m_Inventory; }
		}

		[Category("Cargo Container")]
		public float ItemCount
		{
			get
			{
				float count = 0;
				foreach (var item in m_Inventory.Items)
				{
					count += item.Amount;
				}
				return count;
			}
		}

		[Category("Cargo Container")]
		public float ItemMass
		{
			get
			{
				float mass = 0;
				foreach (var item in m_Inventory.Items)
				{
					mass += item.Mass;
				}
				return mass;
			}
		}

		[Category("Cargo Container")]
		public float ItemVolume
		{
			get
			{
				float volume = 0;
				foreach (var item in m_Inventory.Items)
				{
					volume += item.Volume;
				}
				return volume;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_CargoContainer GetSubTypeEntity()
		{
			MyObjectBuilder_CargoContainer container = (MyObjectBuilder_CargoContainer)BaseEntity;

			//Make sure the inventory is up-to-date
			container.Inventory = Inventory.GetSubTypeEntity();

			return container;
		}

		#endregion
	}
}
