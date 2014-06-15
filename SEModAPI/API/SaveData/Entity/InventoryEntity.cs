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

		new public MyObjectBuilder_Inventory BaseDefinition
		{
			get
			{
				m_baseDefinition.Items = m_itemManager.ExtractBaseDefinitions();
				return m_baseDefinition;
			}
		}

		public uint NextItemId
		{
			get { return m_baseDefinition.nextItemId; }
			set
			{
				if (m_baseDefinition.nextItemId == value) return;
				m_baseDefinition.nextItemId = value;
				Changed = true;
			}
		}

		public List<InventoryItemEntity> Items
		{
			get
			{
				List<InventoryItemEntity> newList = new List<InventoryItemEntity>(m_itemManager.Definitions);
				return newList;
			}
		}

		#endregion

		#region "Methods"

		public InventoryItemEntity NewEntry()
		{
			InventoryItemEntity newItem = m_itemManager.NewEntry();
			newItem.ItemId = NextItemId;
			NextItemId = NextItemId + 1;
			return newItem;
		}

		public InventoryItemEntity NewEntry(MyObjectBuilder_InventoryItem source)
		{
			InventoryItemEntity newItem = m_itemManager.NewEntry(source);
			newItem.ItemId = NextItemId;
			NextItemId = NextItemId + 1;
			return newItem;
		}

		public InventoryItemEntity NewEntry(InventoryItemEntity source)
		{
			InventoryItemEntity newItem = m_itemManager.NewEntry(source);
			newItem.ItemId = NextItemId;
			NextItemId = NextItemId + 1;
			return newItem;
		}

		public bool DeleteEntry(InventoryItemEntity source)
		{
			return m_itemManager.DeleteEntry(source);
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

		public uint ItemId
		{
			get { return m_baseDefinition.ItemId; }
			set
			{
				if (m_baseDefinition.ItemId == value) return;
				m_baseDefinition.ItemId = value;
				Changed = true;
			}
		}

		public float Amount
		{
			get { return m_baseDefinition.Amount; }
			set
			{
				if (m_baseDefinition.Amount == value) return;
				m_baseDefinition.Amount = value;
				Changed = true;
			}
		}

		public MyObjectBuilder_PhysicalObject PhysicalContent
		{
			get { return m_baseDefinition.PhysicalContent; }
			set
			{
				if (m_baseDefinition.PhysicalContent == value) return;
				m_baseDefinition.PhysicalContent = value;
				Changed = true;
			}
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
