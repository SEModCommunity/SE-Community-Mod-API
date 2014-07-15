using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

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

		private InventoryItemManager m_itemManager;
		private InventoryNetworkManager m_networkManager;

		protected InventoryItemEntity m_itemToUpdate;
		protected Decimal m_oldItemAmount;
		protected Decimal m_newItemAmount;

		public static string InventoryNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string InventoryClass = "DE48496EE9812E665B802D5FE9E7AD77";

		public static string InventoryCalculateMassVolumeMethod = "166CC20258091AEA72B666F9EF9503F4";
		public static string InventoryGetTotalVolumeMethod = "C8CB569A2F9A58A24BAC40AB0817AD6A";
		public static string InventoryGetTotalMassMethod = "4E701A33F8803398A50F20D8BF2E5507";
		public static string InventorySetFromObjectBuilderMethod = "D85F2B547D9197E27D0DB9D5305D624F";
		public static string InventoryGetObjectBuilderMethod = "EFBD3CF8717682D7B59A5878FF97E0BB";
		public static string InventoryCleanUpMethod = "476A04917356C2C5FFE23B1CBFC11450";
		public static string InventoryGetItemListMethod = "C43E297C0F568726D4BDD5D71B901911";

		public static string InventoryAddItemAmountMethod = "FB009222ACFCEACDC546801B06DDACB6";
		public static string InventoryRemoveItemAmountMethod = "623B0AC0E7D9C30410680C76A55F0C6B";

		#endregion

		#region "Constructors and Initializers"

		public InventoryEntity(MyObjectBuilder_Inventory definition)
			: base(definition)
		{
			m_itemManager = new InventoryItemManager(this);

			List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
			foreach (MyObjectBuilder_InventoryItem item in definition.Items)
			{
				InventoryItemEntity newItem = new InventoryItemEntity(item);
				newItem.Container = this;
				itemList.Add(newItem);
			}
			m_itemManager.Load(itemList);
		}

		public InventoryEntity(MyObjectBuilder_Inventory definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_itemManager = new InventoryItemManager(this, backingObject, InventoryGetItemListMethod);
			m_itemManager.LoadDynamic();

			m_networkManager = new InventoryNetworkManager(this);
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(InventoryNamespace, InventoryClass);
				return type;
			}
		}

		[Category("Container Inventory")]
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

		public void RefreshInventory()
		{
			try
			{
				if (BackingObject != null)
				{
					//Update the base entity
					MethodInfo getObjectBuilder = BackingObject.GetType().GetMethod(InventoryGetObjectBuilderMethod);
					MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)getObjectBuilder.Invoke(BackingObject, new object[] { });
					BaseEntity = inventory;
				}
				else
				{
					//Update the item manager
					MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)BaseEntity;
					List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
					foreach (MyObjectBuilder_InventoryItem item in inventory.Items)
					{
						InventoryItemEntity newItem = new InventoryItemEntity(item);
						newItem.Container = this;
						itemList.Add(newItem);
					}
					m_itemManager.Load(itemList);
				}
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void UpdateItemAmount(InventoryItemEntity item, Decimal newAmount)
		{
			m_itemToUpdate = item;
			m_oldItemAmount = (Decimal)item.Amount;
			m_newItemAmount = newAmount;

			Action action = InternalUpdateItemAmount;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#region "Internal"

		protected void InternalUpdateItemAmount()
		{
			try
			{
				if (m_itemToUpdate == null)
					return;

				Decimal delta = m_newItemAmount - m_oldItemAmount;

				MethodInfo method;
				Object[] parameters;
				if (delta > 0)
				{
					method = BackingObject.GetType().GetMethod(InventoryAddItemAmountMethod);
					parameters = new object[] {
						delta,
						m_itemToUpdate.GetSubTypeEntity().PhysicalContent,
						Type.Missing
					};
				}
				else
				{
					Type[] argTypes = new Type[3];
					argTypes[0] = typeof(Decimal);
					argTypes[1] = typeof(MyObjectBuilder_PhysicalObject);
					argTypes[2] = typeof(bool);

					method = BackingObject.GetType().GetMethod(InventoryRemoveItemAmountMethod, argTypes);
					parameters = new object[] {
						-delta,
						m_itemToUpdate.GetSubTypeEntity().PhysicalContent,
						Type.Missing
					};
				}

				method.Invoke(BackingObject, parameters);

				m_itemToUpdate = null;
				m_oldItemAmount = 0;
				m_newItemAmount = 0;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
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

		public static string InventoryItemGetObjectBuilderMethod = "B45B0C201826847F0E087D82F9AD3DF1";

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

		public InventoryItemEntity(MyObjectBuilder_InventoryItem definition, Object backingObject)
			: base(definition, backingObject)
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

		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(InventoryItemNamespace, InventoryItemClass);
				return type;
			}
		}

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
				var baseEntity = GetSubTypeEntity();
				if (baseEntity.Amount == value) return;

				if(Container != null)
					Container.UpdateItemAmount(this, (Decimal)value);

				baseEntity.Amount = value;
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

	public class InventoryItemManager : BaseEntityManager
	{
		#region "Attributes"

		private InventoryEntity m_parent;

		#endregion

		#region "Constructors and Initializers"

		public InventoryItemManager(InventoryEntity parent)
		{
			m_parent = parent;
		}

		public InventoryItemManager(InventoryEntity parent, Object backingSource, string backingSourceMethodName)
			: base(backingSource, backingSourceMethodName)
		{
			m_parent = parent;
		}

		#endregion

		#region "Methods"

		public override void LoadDynamic()
		{
			if (IsResourceLocked)
				return;

			IsResourceLocked = true;

			List<Object> rawEntities = GetBackingDataList();
			Dictionary<long, BaseObject> data = GetInternalData();
			Dictionary<Object, BaseObject> backingData = GetBackingInternalData();

			//Update the main data mapping
			data.Clear();
			int entityCount = 0;
			foreach (Object entity in rawEntities)
			{
				entityCount++;

				try
				{
					MyObjectBuilder_InventoryItem baseEntity = (MyObjectBuilder_InventoryItem)InventoryItemEntity.InvokeEntityMethod(entity, InventoryItemEntity.InventoryItemGetObjectBuilderMethod);

					if (baseEntity == null)
						continue;
					if (data.ContainsKey(baseEntity.ItemId))
						continue;

					InventoryItemEntity matchingItem = null;

					//If the original data already contains an entry for this, skip creation
					if (backingData.ContainsKey(entity))
					{
						matchingItem = (InventoryItemEntity)backingData[entity];

						//Update the base entity (not the same as BackingObject which is the internal object)
						matchingItem.BaseEntity = baseEntity;
					}
					else
					{
						matchingItem = new InventoryItemEntity(baseEntity, entity);
						matchingItem.Container = m_parent;
					}

					if (matchingItem == null)
						throw new Exception("Failed to match/create inventory item entity");

					data.Add(matchingItem.ItemId, matchingItem);
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
				}
			}

			//Update the backing data mapping
			foreach (var key in backingData.Keys)
			{
				var entry = backingData[key];
				if (!data.ContainsValue(entry))
				{
					entry.Dispose();
				}
			}
			backingData.Clear();
			foreach (var key in data.Keys)
			{
				var entry = data[key];
				backingData.Add(entry.BackingObject, entry);
			}

			IsResourceLocked = false;
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

		#endregion
	}
}
