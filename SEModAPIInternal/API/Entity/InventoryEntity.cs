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

		private BaseObjectManager m_itemManager;

		private bool m_isRefreshing;
		private bool m_isRefreshingInternal;

		public static string InventoryNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string InventoryClass = "DE48496EE9812E665B802D5FE9E7AD77";

		public static string InventoryCalculateMassVolumeMethod = "166CC20258091AEA72B666F9EF9503F4";
		public static string InventoryGetTotalVolumeMethod = "C8CB569A2F9A58A24BAC40AB0817AD6A";
		public static string InventoryGetTotalMassMethod = "4E701A33F8803398A50F20D8BF2E5507";
		public static string InventorySetFromObjectBuilderMethod = "D85F2B547D9197E27D0DB9D5305D624F";
		public static string InventoryAddItemMethod = "613CEE8CCA77F473C2E9F6393B7F5FC8";
		public static string InventoryGetObjectBuilder = "EFBD3CF8717682D7B59A5878FF97E0BB";
		public static string InventoryCleanUpMethod = "476A04917356C2C5FFE23B1CBFC11450";

		#endregion

		#region "Constructors and Initializers"

		public InventoryEntity(MyObjectBuilder_Inventory definition)
			: base(definition)
		{
			m_itemManager = new BaseObjectManager();
			List<InventoryItemEntity> itemList = new List<InventoryItemEntity>();
			foreach (MyObjectBuilder_InventoryItem item in definition.Items)
			{
				InventoryItemEntity newItem = new InventoryItemEntity(item);
				newItem.Container = this;
				itemList.Add(newItem);
			}
			m_itemManager.Load(itemList.ToArray());

			m_isRefreshing = false;
			m_isRefreshingInternal = false;
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

		public void RefreshInventory()
		{
			MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)BaseEntity;

			//Refresh the items content in the inventory from the items manager
			List<InventoryItemEntity> items = m_itemManager.GetTypedInternalData<InventoryItemEntity>();
			inventory.Items.Clear();
			foreach (var item in items)
			{
				inventory.Items.Add(item.GetSubTypeEntity());
			}

			if (BackingObject != null && !m_isRefreshingInternal)
			{
				m_isRefreshingInternal = true;

				Action action = InternalUpdateInventory;
				SandboxGameAssemblyWrapper.EnqueueMainGameAction(action);
			}
		}

		#region "Internal"

		protected Object GetInventoryNetworkManager()
		{
			FieldInfo inventoryNetworkManagerField = BackingObject.GetType().GetField("84CAF0B1E470C6C236DD00B4FCCBF095", BindingFlags.NonPublic | BindingFlags.Static);
			Object inventoryNetworkManager = inventoryNetworkManagerField.GetValue(null);

			return inventoryNetworkManager;
		}

		protected void InternalUpdateInventory()
		{
			try
			{
				/*
				MyObjectBuilder_Inventory internalInventory = (MyObjectBuilder_Inventory)InvokeEntityMethod(BackingObject, InventoryGetObjectBuilder);
				MyObjectBuilder_Inventory baseInventory = GetSubTypeEntity();

				List<MyObjectBuilder_InventoryItem> itemsToUpdate = new List<MyObjectBuilder_InventoryItem>();

				foreach (MyObjectBuilder_InventoryItem item in baseInventory.Items)
				{

					foreach (MyObjectBuilder_InventoryItem internalItem in internalInventory.Items)
					{
						if (internalItem.PhysicalContent.TypeId == item.PhysicalContent.TypeId && internalItem.PhysicalContent.SubtypeName == item.PhysicalContent.SubtypeName)
						{
							if (item.PhysicalContent.CanStack(internalItem.PhysicalContent))
							{
								if (item.AmountDecimal != internalItem.AmountDecimal)
								{
									itemsToUpdate.Add(item);

									break;
								}
							}
							else
							{
								//InvokeEntityMethod(BackingObject, InventoryAddItemMethod, new object[] { newItemAmount, newItem, containerPositionIndex, containerItemId });
							}
						}
					}
				}

				if (itemsToUpdate.Count > 0)
				{
					InvokeEntityMethod(BackingObject, InventoryCleanUpMethod, new object[] { true });

					foreach (MyObjectBuilder_InventoryItem item in itemsToUpdate)
					{
						Decimal newItemAmount = item.AmountDecimal;
						MyObjectBuilder_PhysicalObject newItem = item.PhysicalContent;
						int containerPositionIndex = -1;
						uint? containerItemId = item.ItemId;

						InvokeEntityMethod(BackingObject, InventoryAddItemMethod, new object[] { newItemAmount, newItem, containerPositionIndex, containerItemId });

						LogManager.GameLog.WriteLine("Updating inventory item '" + item.PhysicalContent.ToString() + "' amount to '" + item.AmountDecimal.ToString() + "'");
					}
				}*/

				InvokeEntityMethod(BackingObject, InventorySetFromObjectBuilderMethod, new object[] { (MyObjectBuilder_Inventory)BaseEntity });

				MyObjectBuilder_Inventory internalInventory = (MyObjectBuilder_Inventory)InvokeEntityMethod(BackingObject, InventoryGetObjectBuilder);
				List<InventoryItemEntity> itemList = new List<InventoryItemEntity>(); 
				foreach (MyObjectBuilder_InventoryItem item in internalInventory.Items)
				{
					InventoryItemEntity newItem = new InventoryItemEntity(item);
					newItem.Container = this;
					itemList.Add(newItem);

					Object netManager = GetInventoryNetworkManager();
					MethodInfo networkUpdateInventory = netManager.GetType().GetMethod("18F69CB3FF811B5A608841DE822B8821");
					networkUpdateInventory.Invoke(netManager, new object[] { BackingObject, item.AmountDecimal, item.ItemId, BackingObject, -1, false });
				}
				m_itemManager.Load(itemList.ToArray());
				RefreshInventory();
				//BaseEntity = internalInventory;

				m_isRefreshingInternal = false;
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
				GetSubTypeEntity().Amount = value;
				Changed = true;

				Container.RefreshInventory();
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
}
