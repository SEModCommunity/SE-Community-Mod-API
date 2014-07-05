using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class InventoryEntity : BaseObject
	{
		#region "Attributes"

		private BaseEntityManager m_itemManager;
		private InventoryNetworkManager m_networkManager;

		public static string InventoryNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string InventoryClass = "DE48496EE9812E665B802D5FE9E7AD77";

		public static string InventoryCalculateMassVolumeMethod = "166CC20258091AEA72B666F9EF9503F4";
		public static string InventoryGetTotalVolumeMethod = "C8CB569A2F9A58A24BAC40AB0817AD6A";
		public static string InventoryGetTotalMassMethod = "4E701A33F8803398A50F20D8BF2E5507";
		public static string InventorySetFromObjectBuilderMethod = "D85F2B547D9197E27D0DB9D5305D624F";
		public static string InventoryAddItemMethod = "613CEE8CCA77F473C2E9F6393B7F5FC8";
		public static string InventoryGetObjectBuilderMethod = "EFBD3CF8717682D7B59A5878FF97E0BB";
		public static string InventoryCleanUpMethod = "476A04917356C2C5FFE23B1CBFC11450";

		#endregion

		#region "Constructors and Initializers"

		public InventoryEntity(MyObjectBuilder_Inventory definition)
			: base(definition)
		{
			m_itemManager = new BaseEntityManager();
			List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
			foreach (MyObjectBuilder_InventoryItem item in definition.Items)
			{
				InventoryItemEntity newItem = new InventoryItemEntity(item);
				newItem.Container = this;
				itemList.Add(newItem);
			}
			m_itemManager.Load(itemList.ToArray());
		}

		public InventoryEntity(MyObjectBuilder_Inventory definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_itemManager = new BaseEntityManager();
			List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
			foreach (MyObjectBuilder_InventoryItem item in definition.Items)
			{
				InventoryItemEntity newItem = new InventoryItemEntity(item);
				newItem.Container = this;
				itemList.Add(newItem);
			}
			m_itemManager.Load(itemList.ToArray());

			m_networkManager = new InventoryNetworkManager(this);
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
					RefreshInventory();

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
			return (MyObjectBuilder_Inventory)BaseEntity;
		}

		public void RefreshInventory()
		{
			try
			{
				if (BackingObject != null)
					InternalRefreshInventory();

				//Update the item manager
				List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
				foreach (MyObjectBuilder_InventoryItem item in GetSubTypeEntity().Items)
				{
					InventoryItemEntity newItem = new InventoryItemEntity(item);
					newItem.Container = this;
					itemList.Add(newItem);
				}
				m_itemManager.Load(itemList);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#region "Internal"

		public void InternalRefreshInventory()
		{
			try
			{
				//Update the base entity
				MethodInfo getObjectBuilder = BackingObject.GetType().GetMethod(InventoryGetObjectBuilderMethod);
				MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)getObjectBuilder.Invoke(BackingObject, new object[] { });
				BaseEntity = inventory;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void InternalUpdateItemAmount(Decimal oldAmount, Decimal newAmount, uint itemId)
		{
			if (m_networkManager != null)
			{
				Decimal delta = oldAmount - newAmount;
				m_networkManager.UpdateItemAmount(delta, itemId);
			}
		}

		#endregion

		#endregion
	}

	public class InventoryItemEntity : BaseObject
	{
		#region "Attributes"

		private InventoryEntity m_parentContainer;

		private static PhysicalItemDefinitionsManager m_physicalItemsManager;
		private static ComponentDefinitionsManager m_componentsManager;
		private static AmmoMagazinesDefinitionsManager m_ammoManager;

		private SerializableDefinitionId m_itemId;
		private float m_itemMass = -1;
		private float m_itemVolume = -1;

		public static string InventoryItemNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string InventoryItemClass = "555069178719BB1B546FB026B906CE00";

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
		[ReadOnly(true)]
		public override string Name
		{
			get { return Id.SubtypeName; }
		}

		[Category("Container Item")]
		[Browsable(false)]
		public InventoryEntity Container
		{
			get { return m_parentContainer; }
			set { m_parentContainer = value; }
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
		[ReadOnly(true)]
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

				Container.InternalUpdateItemAmount((Decimal)GetSubTypeEntity().Amount, (Decimal)value, ItemId);

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
		[ReadOnly(true)]
		public float TotalMass
		{
			get { return GetSubTypeEntity().Amount * m_itemMass; }
		}

		[Category("Container Item")]
		[ReadOnly(true)]
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

	public class InventoryNetworkManager
	{
		#region "Attributes"

		private InventoryEntity m_container;
		private Object m_networkManager;

		//Field in parent container that contains the network manager
		public static string InventoryNetworkManagerField = "84CAF0B1E470C6C236DD00B4FCCBF095";

		public static string InventoryNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string InventoryNetworkManagerClass = "98C1408628C42B9F7FDB1DE7B8FAE776";

		//Source, Amount, ItemId, Destination, DestinationSlot = -1, SomeBool = false
		public static string InventoryNetworkManagerTransferItemMethod = "18F69CB3FF811B5A608841DE822B8821";
		//Source, Amount, ItemId, SomeBool = false
		public static string InventoryNetworkManagerUpdateItemMethod = "DB1C87495038A9B539EE33337B3A4694";

		#endregion

		#region "Constructors and Initializers"

		public InventoryNetworkManager(InventoryEntity container)
		{
			m_container = container;
			m_networkManager = NetworkManager;
		}

		#endregion

		#region "Properties"

		internal Object NetworkManager
		{
			get
			{
				if(m_networkManager == null)
				{
					try
					{
						FieldInfo inventoryNetworkManagerField = m_container.BackingObject.GetType().GetField(InventoryNetworkManagerField, BindingFlags.NonPublic | BindingFlags.Static);
						Object inventoryNetworkManager = inventoryNetworkManagerField.GetValue(null);
						m_networkManager = inventoryNetworkManager;
					}
					catch (Exception ex)
					{
						LogManager.GameLog.WriteLine(ex);
					}
				}

				return m_networkManager;
			}
		}

		#endregion

		#region "Methods"

		internal void TransferItems(Object destination, Decimal amount, uint itemId)
		{
			try
			{
				Object source = m_container.BackingObject;
				Object networkManager = NetworkManager;
				MethodInfo method = networkManager.GetType().GetMethod(InventoryNetworkManagerTransferItemMethod);
				method.Invoke(networkManager, new object[] { source, amount, itemId, destination, -1, false });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		internal void UpdateItemAmount(Decimal amount, uint itemId)
		{
			try
			{
				Object source = m_container.BackingObject;
				Object networkManager = NetworkManager;
				MethodInfo method = networkManager.GetType().GetMethod(InventoryNetworkManagerUpdateItemMethod);
				method.Invoke(networkManager, new object[] { source, amount, itemId, false});
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
