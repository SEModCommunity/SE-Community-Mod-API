using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class InventoryEntity : BaseObject
	{
		#region "Attributes"

		private BaseObjectManager m_itemManager;

		#endregion

		#region "Constructors and Initializers"

		public InventoryEntity(MyObjectBuilder_Inventory definition)
			: base(definition)
		{
			m_itemManager = new BaseObjectManager();
			List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
			foreach (MyObjectBuilder_InventoryItem item in definition.Items)
			{
				itemList.Add(new InventoryItemEntity(item));
			}
			m_itemManager.Load(itemList.ToArray());
		}

		#endregion

		#region "Properties"

		public override string Name
		{
			get { return "Inventory"; }
		}

		[Category("Container Inventory")]
		public uint NextItemId
		{
			get { return GetSubTypeEntity().nextItemId; }
			set
			{
				if (GetSubTypeEntity().nextItemId == value) return;
				GetSubTypeEntity().nextItemId = value;
				Changed = true;
			}
		}

		[Category("Container Inventory")]
		[Browsable(false)]
		public List<InventoryItemEntity> Items
		{
			get
			{
				try
				{
					List<InventoryItemEntity> newList = m_itemManager.GetTypedInternalData<InventoryItemEntity>();
					return newList;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return new List<InventoryItemEntity>();
				}
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

			InventoryItemEntity newItem = m_itemManager.NewEntry<InventoryItemEntity>(defaults);
			newItem.ItemId = NextItemId;
			NextItemId = NextItemId + 1;

			RefreshInventory();

			return newItem;
		}

		public InventoryItemEntity NewEntry(MyObjectBuilder_InventoryItem source)
		{
			InventoryItemEntity newItem = m_itemManager.NewEntry<InventoryItemEntity>(source);
			newItem.ItemId = NextItemId;
			NextItemId = NextItemId + 1;

			RefreshInventory();

			return newItem;
		}

		public InventoryItemEntity NewEntry(InventoryItemEntity source)
		{
			InventoryItemEntity newItem = (InventoryItemEntity)m_itemManager.NewEntry(source);
			newItem.ItemId = NextItemId;
			NextItemId = NextItemId + 1;

			RefreshInventory();

			return newItem;
		}

		public bool DeleteEntry(InventoryItemEntity source)
		{
			bool result = m_itemManager.DeleteEntry(source);

			RefreshInventory();

			return result;
		}

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Inventory GetSubTypeEntity()
		{
			RefreshInventory();

			return (MyObjectBuilder_Inventory)BaseEntity;
		}

		private void RefreshInventory()
		{
			MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)BaseEntity;

			//Refresh the items content in the inventory from the items manager
			List<InventoryItemEntity> items = m_itemManager.GetTypedInternalData<InventoryItemEntity>();
			inventory.Items.Clear();
			foreach (var item in items)
			{
				inventory.Items.Add(item.GetSubTypeEntity());
			}
		}

		#endregion
	}

	public class InventoryItemEntity : BaseObject
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

		public override string Name
		{
			get { return Id.SubtypeName; }
		}

		[Category("Container Item")]
		[ReadOnly(false)]
		[TypeConverter(typeof(ItemSerializableDefinitionIdTypeConverter))]
		new public SerializableDefinitionId Id
		{
			get { return m_itemId; }
			set
			{
				if (m_itemId.Equals(value)) return;

				GetSubTypeEntity().PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(value);
				bool result = FindMatchingItem();
				if (!result)
				{
					GetSubTypeEntity().PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(m_itemId);
					return;
				}

				Changed = true;
			}
		}

		[Category("Container Item")]
		public uint ItemId
		{
			get { return GetSubTypeEntity().ItemId; }
			set
			{
				if (GetSubTypeEntity().ItemId == value) return;
				GetSubTypeEntity().ItemId = value;
				Changed = true;
			}
		}

		[Category("Container Item")]
		public float Amount
		{
			get { return GetSubTypeEntity().Amount; }
			set
			{
				if (GetSubTypeEntity().Amount == value) return;
				GetSubTypeEntity().Amount = value;
				Changed = true;
			}
		}

		[Category("Container Item")]
		[Browsable(false)]
		public MyObjectBuilder_PhysicalObject PhysicalContent
		{
			get { return GetSubTypeEntity().PhysicalContent; }
			set
			{
				if (GetSubTypeEntity().PhysicalContent == value) return;
				GetSubTypeEntity().PhysicalContent = value;
				Changed = true;
			}
		}

		[Category("Container Item")]
		public float TotalMass
		{
			get { return GetSubTypeEntity().Amount * m_itemMass; }
		}

		[Category("Container Item")]
		public float TotalVolume
		{
			get { return GetSubTypeEntity().Amount * m_itemVolume; }
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

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		internal MyObjectBuilder_InventoryItem GetSubTypeEntity()
		{
			return (MyObjectBuilder_InventoryItem)BaseEntity;
		}

		#endregion
	}
}
