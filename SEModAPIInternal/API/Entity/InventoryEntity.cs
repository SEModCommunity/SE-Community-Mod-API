using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
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
	[DataContract(Name = "InventoryEntityProxy")]
	[KnownType(typeof(InventoryItemEntity))]
	public class InventoryEntity : BaseObject
	{
		#region "Attributes"

		private InventoryItemManager m_itemManager;

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
			m_itemManager.Refresh();
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Category("Container Inventory")]
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

		[IgnoreDataMember]
		[Category("Container Inventory")]
		[ReadOnly(true)]
		public override string Name
		{
			get { return "Inventory"; }
		}

		[IgnoreDataMember]
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Object builder data of the object")]
		internal new MyObjectBuilder_Inventory ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)base.ObjectBuilder;

				return inventory;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Container Inventory")]
		[ReadOnly(true)]
		public uint NextItemId
		{
			get { return ObjectBuilder.nextItemId; }
			set
			{
				if (ObjectBuilder.nextItemId == value) return;
				ObjectBuilder.nextItemId = value;
				Changed = true;
			}
		}

		[IgnoreDataMember]
		[Category("Container Inventory")]
		[Browsable(false)]
		[ReadOnly(true)]
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
					LogManager.ErrorLog.WriteLine(ex);
					return new List<InventoryItemEntity>();
				}
			}
			private set
			{
				//Do nothing!
			}
		}

		#endregion

		#region "Methods"

		new public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for InventoryEntity");
				bool result = true;
				result &= BaseObject.HasMethod(type, InventoryCalculateMassVolumeMethod);
				result &= BaseObject.HasMethod(type, InventoryGetTotalVolumeMethod);
				result &= BaseObject.HasMethod(type, InventoryGetTotalMassMethod);
				result &= BaseObject.HasMethod(type, InventorySetFromObjectBuilderMethod);
				result &= BaseObject.HasMethod(type, InventoryGetObjectBuilderMethod);
				result &= BaseObject.HasMethod(type, InventoryCleanUpMethod);
				result &= BaseObject.HasMethod(type, InventoryGetItemListMethod);
				result &= BaseObject.HasMethod(type, InventoryAddItemAmountMethod);

				Type[] argTypes = new Type[3];
				argTypes[0] = typeof(Decimal);
				argTypes[1] = typeof(MyObjectBuilder_PhysicalObject);
				argTypes[2] = typeof(bool);
				result &= BaseObject.HasMethod(type, InventoryRemoveItemAmountMethod, argTypes);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public InventoryItemEntity NewEntry()
		{
			MyObjectBuilder_InventoryItem defaults = new MyObjectBuilder_InventoryItem();
			SerializableDefinitionId itemTypeId = new SerializableDefinitionId(typeof(MyObjectBuilder_Ore), "Stone");
			defaults.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(itemTypeId);
			defaults.Amount = 1;

			InventoryItemEntity newItem = m_itemManager.NewEntry<InventoryItemEntity>(defaults);
			newItem.ItemId = NextItemId;
			NextItemId = NextItemId + 1;

			RefreshInventory();

			return newItem;
		}

		public bool NewEntry(InventoryItemEntity source)
		{
			m_itemManager.AddEntry<InventoryItemEntity>(NextItemId, source);

			//TODO - Figure out the right way to add new items
			//Just updating an item amount doesn't seem to work right
			UpdateItemAmount(source, (Decimal)(source.Amount * 2));

			RefreshInventory();

			return true;
		}

		public bool DeleteEntry(InventoryItemEntity source)
		{
			bool result = m_itemManager.DeleteEntry(source);

			RefreshInventory();

			return result;
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
					ObjectBuilder = inventory;
				}
				else
				{
					//Update the item manager
					MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)ObjectBuilder;
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
				LogManager.ErrorLog.WriteLine(ex);
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
						m_itemToUpdate.ObjectBuilder.PhysicalContent,
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
						m_itemToUpdate.ObjectBuilder.PhysicalContent,
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
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}

	[DataContract(Name = "InventoryItemEntityProxy")]
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

		[IgnoreDataMember]
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

		[DataMember]
		[Category("Container Item")]
		[ReadOnly(true)]
		public override string Name
		{
			get { return Id.SubtypeName; }
		}

		[IgnoreDataMember]
		[Category("Container Item")]
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_InventoryItem ObjectBuilder
		{
			get
			{
				return (MyObjectBuilder_InventoryItem)base.ObjectBuilder;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[IgnoreDataMember]
		[Category("Container Item")]
		[Browsable(false)]
		public InventoryEntity Container
		{
			get { return m_parentContainer; }
			set { m_parentContainer = value; }
		}

		[DataMember]
		[Category("Container Item")]
		[ReadOnly(false)]
		[TypeConverter(typeof(ItemSerializableDefinitionIdTypeConverter))]
		new public SerializableDefinitionId Id
		{
			get { return m_itemId; }
			set
			{
				if (m_itemId.Equals(value)) return;

				ObjectBuilder.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(value);
				bool result = FindMatchingItem();
				if (!result)
				{
					ObjectBuilder.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject(m_itemId);
					return;
				}

				Changed = true;
			}
		}

		[DataMember]
		[Category("Container Item")]
		[ReadOnly(true)]
		public uint ItemId
		{
			get { return ObjectBuilder.ItemId; }
			set
			{
				if (ObjectBuilder.ItemId == value) return;
				ObjectBuilder.ItemId = value;
				Changed = true;
			}
		}

		[DataMember]
		[Category("Container Item")]
		public float Amount
		{
			get { return ObjectBuilder.Amount; }
			set
			{
				var baseEntity = ObjectBuilder;
				if (baseEntity.Amount == value) return;

				if(Container != null)
					Container.UpdateItemAmount(this, (Decimal)value);

				baseEntity.Amount = value;
				Changed = true;
			}
		}

		[IgnoreDataMember]
		[Category("Container Item")]
		[Browsable(false)]
		[ReadOnly(true)]
		public MyObjectBuilder_PhysicalObject PhysicalContent
		{
			get { return ObjectBuilder.PhysicalContent; }
			set
			{
				if (ObjectBuilder.PhysicalContent == value) return;
				ObjectBuilder.PhysicalContent = value;
				Changed = true;
			}
		}

		[IgnoreDataMember]
		[Category("Container Item")]
		[ReadOnly(true)]
		public float TotalMass
		{
			get { return ObjectBuilder.Amount * m_itemMass; }
		}

		[IgnoreDataMember]
		[Category("Container Item")]
		[ReadOnly(true)]
		public float TotalVolume
		{
			get { return ObjectBuilder.Amount * m_itemVolume; }
		}

		[DataMember]
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

		[DataMember]
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

		new public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for InventoryItemEntity");
				bool result = true;
				result &= BaseObject.HasMethod(type, InventoryItemGetObjectBuilderMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public override void Dispose()
		{
			Amount = 0;
		}

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

	public class InventoryItemManager : BaseObjectManager
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
			: base(backingSource, backingSourceMethodName, InternalBackingType.List)
		{
			m_parent = parent;
		}

		#endregion

		#region "Methods"

		protected override bool IsValidEntity(Object entity)
		{
			try
			{
				if (entity == null)
					return false;

				return true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		protected override void InternalRefreshObjectBuilderMap()
		{
			try
			{
				if (m_rawDataObjectBuilderListResourceLock.Owned)
					return;
				if (WorldManager.Instance.IsWorldSaving)
					return;
				if (WorldManager.Instance.InternalGetResourceLock() == null)
					return;
				if (WorldManager.Instance.InternalGetResourceLock().Owned)
					return;

				m_rawDataObjectBuilderListResourceLock.AcquireExclusive();

				m_rawDataObjectBuilderList.Clear();
				foreach (Object entity in GetBackingDataList())
				{
					if (!IsValidEntity(entity))
						continue;

					MyObjectBuilder_InventoryItem baseEntity = (MyObjectBuilder_InventoryItem)InventoryItemEntity.InvokeEntityMethod(entity, InventoryItemEntity.InventoryItemGetObjectBuilderMethod);
					if (baseEntity == null)
						continue;

					m_rawDataObjectBuilderList.Add(entity, baseEntity);
				}

				m_rawDataObjectBuilderListResourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				m_rawDataObjectBuilderListResourceLock.ReleaseExclusive();
			}
		}

		protected override void LoadDynamic()
		{
			try
			{
				Dictionary<Object, MyObjectBuilder_Base> objectBuilderList = GetObjectBuilderMap();
				List<Object> rawEntities = GetBackingDataList();
				Dictionary<long, BaseObject> internalDataCopy = new Dictionary<long, BaseObject>(GetInternalData());

				if (objectBuilderList.Count != rawEntities.Count)
				{
					if (SandboxGameAssemblyWrapper.IsDebugging)
						LogManager.APILog.WriteLine("InventoryItemManager - Mismatch between raw entities and object builders");
					m_resourceLock.ReleaseExclusive();
					return;
				}

				//Update the main data mapping
				foreach (Object entity in rawEntities)
				{
					try
					{
						if (!IsValidEntity(entity))
							continue;

						if (!objectBuilderList.ContainsKey(entity))
							continue;

						MyObjectBuilder_InventoryItem baseEntity = (MyObjectBuilder_InventoryItem)objectBuilderList[entity];
						if (baseEntity == null)
							continue;

						long itemId = baseEntity.ItemId;

						//If the original data already contains an entry for this, skip creation
						if (GetInternalData().ContainsKey(itemId))
						{
							InventoryItemEntity matchingItem = (InventoryItemEntity)GetEntry(itemId);

							//Update the base entity (not the same as BackingObject which is the internal object)
							matchingItem.ObjectBuilder = baseEntity;
						}
						else
						{
							InventoryItemEntity newItemEntity = new InventoryItemEntity(baseEntity, entity);
							newItemEntity.Container = m_parent;

							AddEntry(newItemEntity.ItemId, newItemEntity);
						}
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}

				//Cleanup old entities
				foreach (var entry in internalDataCopy)
				{
					try
					{
						if (!rawEntities.Contains(entry.Value.BackingObject))
							DeleteEntry(entry.Value);
					}
					catch (Exception ex)
					{
						LogManager.ErrorLog.WriteLine(ex);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
