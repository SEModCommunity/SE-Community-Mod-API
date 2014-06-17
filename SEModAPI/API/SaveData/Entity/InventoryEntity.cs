using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		[Category("Container Inventory")]
		[Browsable(false)]
		new public MyObjectBuilder_Inventory BaseDefinition
		{
			get
			{
				m_baseDefinition.Items = m_itemManager.ExtractBaseDefinitions();
				return m_baseDefinition;
			}
		}

		[Category("Container Inventory")]
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

		[Category("Container Inventory")]
		[Browsable(false)]
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
			MyObjectBuilder_InventoryItem defaults = new MyObjectBuilder_InventoryItem();
			SerializableDefinitionId itemTypeId = new SerializableDefinitionId(MyObjectBuilderTypeEnum.Ore, "Stone");
			defaults.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(itemTypeId);
			defaults.Amount = 1;

			InventoryItemEntity newItem = m_itemManager.NewEntry(defaults);
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

		private SerializableDefinitionId m_itemId;
		private float m_itemMass = -1;
		private float m_itemVolume = -1;

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

			FindMatchingItem();
		}

		#endregion

		#region "Properties"

		[Category("Container Item")]
		[ReadOnly(false)]
		[TypeConverter(typeof(ItemSerializableDefinitionIdTypeConverter))]
		new public SerializableDefinitionId Id
		{
			get { return m_itemId; }
			set
			{
				if (m_itemId.Equals(value)) return;

				m_baseDefinition.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(value);
				bool result = FindMatchingItem();
				if (!result)
				{
					m_baseDefinition.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(m_itemId);
					return;
				}

				Changed = true;
			}
		}

		[Category("Container Item")]
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

		[Category("Container Item")]
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

		[Category("Container Item")]
		[Browsable(false)]
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

		[Category("Container Item")]
		public float TotalMass
		{
			get { return m_baseDefinition.Amount * m_itemMass; }
		}

		[Category("Container Item")]
		public float TotalVolume
		{
			get { return m_baseDefinition.Amount * m_itemVolume; }
		}

		[Category("Container Item")]
		[ReadOnly(true)]
		public float Mass
		{
			get { return m_itemMass; }
			set
			{
				if (m_itemMass == value) return;
				m_itemMass = value;
				Changed = true;
			}
		}

		[Category("Container Item")]
		[ReadOnly(true)]
		public float Volume
		{
			get { return m_itemVolume; }
			set
			{
				if (m_itemVolume == value) return;
				m_itemVolume = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		private bool FindMatchingItem()
		{
			bool foundMatchingItem = false;
			if (!foundMatchingItem)
			{
				foreach (var item in m_physicalItemsManager.Definitions)
				{
					if (item.Id.TypeId == PhysicalContent.TypeId && item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						m_itemId = item.Id;
						m_itemMass = item.Mass;
						m_itemVolume = item.Volume;

						foundMatchingItem = true;
						break;
					}
				}
			}
			if (!foundMatchingItem)
			{
				foreach (var item in m_componentsManager.Definitions)
				{
					if (item.Id.TypeId == PhysicalContent.TypeId && item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						m_itemId = item.Id;
						m_itemMass = item.Mass;
						m_itemVolume = item.Volume;

						foundMatchingItem = true;
						break;
					}
				}
			}
			if (!foundMatchingItem)
			{
				foreach (var item in m_ammoManager.Definitions)
				{
					if (item.Id.TypeId == PhysicalContent.TypeId && item.Id.SubtypeId == PhysicalContent.SubtypeName)
					{
						m_itemId = item.Id;
						m_itemMass = item.Mass;
						m_itemVolume = item.Volume;

						foundMatchingItem = true;
						break;
					}
				}
			}

			return foundMatchingItem;
		}

		#endregion
	}

	public class InventoryItemManager : SerializableEntityManager<MyObjectBuilder_InventoryItem, InventoryItemEntity>
	{
	}
}
