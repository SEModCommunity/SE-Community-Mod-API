using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData.Entity
{
	public class InventoryEntity : SerializableEntity<MyObjectBuilder_Inventory>
	{
		#region "Attributes"

		private InventoryItemManager m_itemManager;

		#endregion

		#region "Constructors and Initializers"

		public InventoryEntity(MyObjectBuilder_Inventory definition)
			: base(definition)
		{
			m_itemManager = new InventoryItemManager();
			m_itemManager.Load(definition.Items.ToArray());
		}

		#endregion

		#region "Properties"

		public List<InventoryItemEntity> Items
		{
			get {
				List<InventoryItemEntity> newList = new List<InventoryItemEntity>(m_itemManager.Definitions);
				return newList;
			}
		}

		#endregion
	}

	public class InventoryItemEntity : SerializableEntity<MyObjectBuilder_InventoryItem>
	{
		#region "Attributes"

		private static PhysicalItemDefinitionsManager m_physicalItemsManager;
		private static ComponentDefinitionsManager m_componentsManager;
		private static AmmoMagazinesDefinitionsManager m_ammoManager;

		#endregion

		#region "Constructors and Initializers"

		public InventoryItemEntity(MyObjectBuilder_InventoryItem definition)
			: base(definition)
		{
			if (m_physicalItemsManager == null)
			{
				m_physicalItemsManager = new PhysicalItemDefinitionsManager();
				m_physicalItemsManager.Load(PhysicalItemDefinitionsManager.GetContentDataFile("PhysicalItems.sbc"));
			}
			if (m_componentsManager == null)
			{
				m_componentsManager = new ComponentDefinitionsManager();
				m_componentsManager.Load(ComponentDefinitionsManager.GetContentDataFile("Components.sbc"));
			}
			if (m_ammoManager == null)
			{
				m_ammoManager = new AmmoMagazinesDefinitionsManager();
				m_ammoManager.Load(AmmoMagazinesDefinitionsManager.GetContentDataFile("AmmoMagazines.sbc"));
			}
		}

		#endregion

		#region "Properties"

		public float Amount
		{
			get { return m_baseDefinition.Amount; }
		}

		public MyObjectBuilder_PhysicalObject PhysicalContent
		{
			get { return m_baseDefinition.PhysicalContent; }
		}

		public float Mass
		{
			get
			{
				foreach (var item in m_physicalItemsManager.Definitions)
				{
					if (item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						return item.Mass * m_baseDefinition.Amount;
					}
				}
				foreach (var item in m_componentsManager.Definitions)
				{
					if (item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						return item.Mass * m_baseDefinition.Amount;
					}
				}
				foreach (var item in m_ammoManager.Definitions)
				{
					if (item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						return item.Mass * m_baseDefinition.Amount;
					}
				}

				return -1;
			}
		}

		public float Volume
		{
			get
			{
				foreach (var item in m_physicalItemsManager.Definitions)
				{
					if (item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						return item.Volume * m_baseDefinition.Amount;
					}
				}
				foreach (var item in m_componentsManager.Definitions)
				{
					if (item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						return item.Volume * m_baseDefinition.Amount;
					}
				}
				foreach (var item in m_ammoManager.Definitions)
				{
					if (item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						return item.Volume * m_baseDefinition.Amount;
					}
				}

				return -1;
			}
		}

		#endregion
	}

	public class InventoryItemManager : SerializableEntityManager<MyObjectBuilder_InventoryItem, InventoryItemEntity>
	{
	}
}
