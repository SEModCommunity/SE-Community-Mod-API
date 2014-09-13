using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Serializer;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

using VRage;
using Sandbox.Definitions;

namespace SEModAPIInternal.API.Entity
{
	public struct InventoryDelta
	{
		public InventoryItemEntity item;
		public float oldAmount;
		public float newAmount;
	}

	[DataContract(Name = "InventoryEntityProxy")]
	[KnownType(typeof(InventoryItemEntity))]
	public class InventoryEntity : BaseObject
	{
		#region "Attributes"

		private InventoryItemManager m_itemManager;
		private Queue<InventoryDelta> m_itemDeltaQueue;

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
			m_itemDeltaQueue = new Queue<InventoryDelta>();
		}

		public InventoryEntity(MyObjectBuilder_Inventory definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_itemManager = new InventoryItemManager(this, backingObject, InventoryGetItemListMethod);
			m_itemManager.Refresh();
			m_itemDeltaQueue = new Queue<InventoryDelta>();
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
				argTypes[0] = typeof(MyFixedPoint);
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
			defaults.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(itemTypeId);
			defaults.Amount = 1;

			InventoryItemEntity newItem = new InventoryItemEntity(defaults);
			newItem.ItemId = NextItemId;
			m_itemManager.NewEntry<InventoryItemEntity>(newItem);

			NextItemId = NextItemId + 1;

			RefreshInventory();

			return newItem;
		}

		public bool NewEntry(InventoryItemEntity source)
		{
			m_itemManager.AddEntry<InventoryItemEntity>(NextItemId, source);

			//TODO - Figure out the right way to add new items
			//Just updating an item amount doesn't seem to work right
			UpdateItemAmount(source, source.Amount * 2);

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
					MyObjectBuilder_Inventory inventory = (MyObjectBuilder_Inventory)InvokeEntityMethod(BackingObject, InventoryGetObjectBuilderMethod);
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

		[Obsolete]
		public void UpdateItemAmount(InventoryItemEntity item, Decimal newAmount)
		{
			UpdateItemAmount(item, (float)newAmount);
		}

		public void UpdateItemAmount(InventoryItemEntity item, float newAmount)
		{
			InventoryDelta delta = new InventoryDelta();
			delta.item = item;
			delta.oldAmount = item.Amount;
			delta.newAmount = newAmount;

			m_itemDeltaQueue.Enqueue(delta);

			Action action = InternalUpdateItemAmount;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#region "Internal"

		protected void InternalUpdateItemAmount()
		{
			try
			{
				if (m_itemDeltaQueue.Count == 0)
					return;

				InventoryDelta itemDelta = m_itemDeltaQueue.Dequeue();

				float delta = itemDelta.newAmount - itemDelta.oldAmount;

				MyObjectBuilder_PhysicalObject physicalContent = itemDelta.item.ObjectBuilder.PhysicalContent;

				if (delta > 0)
				{
					Object[] parameters = new object[] {
						(MyFixedPoint)delta,
						physicalContent,
						-1
					};

					InvokeEntityMethod(BackingObject, InventoryAddItemAmountMethod, parameters);
				}
				else
				{
					Type[] argTypes = new Type[3];
					argTypes[0] = typeof(MyFixedPoint);
					argTypes[1] = typeof(MyObjectBuilder_PhysicalObject);
					argTypes[2] = typeof(bool);

					Object[] parameters = new object[] {
						(MyFixedPoint)(-delta),
						physicalContent,
						Type.Missing
					};

					InvokeEntityMethod(BackingObject, InventoryRemoveItemAmountMethod, parameters, argTypes);
				}
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

		public static string InventoryItemNamespace = "33FB6E717989660631E6772B99F502AD";
		public static string InventoryItemClass = "555069178719BB1B546FB026B906CE00";

		public static string InventoryItemGetObjectBuilderMethod = "B45B0C201826847F0E087D82F9AD3DF1";

		public static string InventoryItemItemIdField = "33FDC4CADA8125F411D1F07103A65358";

		#endregion

		#region "Constructors and Initializers"

		public InventoryItemEntity(MyObjectBuilder_InventoryItem definition)
			: base(definition)
		{
			m_definition = MyDefinitionManager.Static.GetPhysicalItemDefinition(PhysicalContent);
			m_definitionId = m_definition.Id;
		}

		public InventoryItemEntity(MyObjectBuilder_InventoryItem definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_definition = MyDefinitionManager.Static.GetPhysicalItemDefinition(PhysicalContent);
			m_definitionId = m_definition.Id;
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
			get
			{
				MyPhysicalItemDefinition def = Definition;
				if (def == null)
					return base.Name;

				return def.Id.SubtypeName;
			}
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
		[ReadOnly(true)]
		new public MyPhysicalItemDefinition Definition
		{
			get
			{
				return (MyPhysicalItemDefinition)base.Definition;
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
			get { return (float)ObjectBuilder.Amount; }
			set
			{
				var baseEntity = ObjectBuilder;
				if ((float)baseEntity.Amount == value) return;

				if(Container != null)
					Container.UpdateItemAmount(this, value);

				baseEntity.Amount = (MyFixedPoint)value;
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
			get
			{
				return (float)ObjectBuilder.Amount * Mass;
			}
		}

		[IgnoreDataMember]
		[Category("Container Item")]
		[ReadOnly(true)]
		public float TotalVolume
		{
			get { return (float)ObjectBuilder.Amount * Volume; }
		}

		[DataMember]
		[Category("Container Item")]
		[ReadOnly(true)]
		public float Mass
		{
			get
			{
				if (Definition == null)
					return 0;
				return Definition.Mass;
			}
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Container Item")]
		[ReadOnly(true)]
		public float Volume
		{
			get
			{
				if (Definition == null)
					return 0;
				return Definition.Volume;
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
					throw new Exception("Could not find internal type for InventoryItemEntity");
				bool result = true;
				result &= BaseObject.HasMethod(type, InventoryItemGetObjectBuilderMethod);
				result &= BaseObject.HasField(type, InventoryItemItemIdField);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		public static uint GetInventoryItemId(object item)
		{
			try
			{
				uint result = (uint)GetEntityFieldValue(item, InventoryItemItemIdField);
				return result;
			} catch(Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return 0;
			}
		}

		public override void Dispose()
		{
			Amount = 0;

			base.Dispose();
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

		protected override void LoadDynamic()
		{
			try
			{
				List<Object> rawEntities = GetBackingDataList();
				Dictionary<long, BaseObject> internalDataCopy = new Dictionary<long, BaseObject>(GetInternalData());

				//Update the main data mapping
				foreach (Object entity in rawEntities)
				{
					try
					{
						if (!IsValidEntity(entity))
							continue;

						MyObjectBuilder_InventoryItem baseEntity = (MyObjectBuilder_InventoryItem)InventoryItemEntity.InvokeEntityMethod(entity, InventoryItemEntity.InventoryItemGetObjectBuilderMethod);
						if (baseEntity == null)
							continue;

						uint entityItemId = InventoryItemEntity.GetInventoryItemId(entity);
						long itemId = baseEntity.ItemId;

						//If the original data already contains an entry for this, skip creation
						if (internalDataCopy.ContainsKey(itemId))
						{
							InventoryItemEntity matchingItem = (InventoryItemEntity)GetEntry(itemId);
							if (matchingItem == null || matchingItem.IsDisposed)
								continue;

							matchingItem.BackingObject = entity;
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
