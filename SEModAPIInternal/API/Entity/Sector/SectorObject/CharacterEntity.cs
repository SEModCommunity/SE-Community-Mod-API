using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.Game.Weapons;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class CharacterEntity : BaseEntity
	{
		#region "Attributes"

		private InventoryEntity m_Inventory;

		public static string CharacterNamespace = "F79C930F3AD8FDAF31A59E2702EECE70";
		public static string CharacterClass = "3B71F31E6039CAE9D8706B5F32FE468D";

		public static string CharacterGetHealthMethod = "7047AFF5D44FC8A44572E92DBAD13011";
		public static string CharacterDamageCharacterMethod = "CF6EEF37B5AE4047E65CA4A0BB43F774";
		public static string CharacterSetHealthMethod = "92A0500FD8772AB1AC3A6F79FD2A1C72";
		public static string CharacterGetBatteryCapacityMethod = "CF72A89940254CB8F535F177150FC743";
		public static string CharacterSetBatteryCapacityMethod = "C3BF60F3540A8A48CB8FEE0CDD3A95C6";
		public static string CharacterGetInventoryMethod = "GetInventory";
		public static string CharacterGetDisplayNameMethod = "DB913685BC5152DC19A4796E9E8CF659";

		public static string CharacterItemListField = "02F6468D864F3203482135334BEB58AD";

		#endregion

		#region "Constructors and Initializers"

		public CharacterEntity(MyObjectBuilder_Character definition)
			: base(definition)
		{
			m_Inventory = new InventoryEntity(definition.Inventory);
		}

		public CharacterEntity(MyObjectBuilder_Character definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_Inventory = new InventoryEntity(definition.Inventory, InternalGetCharacterInventory());

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnPlayerJoined;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);

			Action action = InternalRegisterCharacterClosedEvent;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				string name = base.Name;
				if (BackingObject != null)
				{
					string internalName = InternalGetDisplayName();
					if (internalName != "")
						name = internalName;
				}

				return base.Name;
			}
		}

		[Category("Character")]
		[Browsable(true)]
		[TypeConverter(typeof(Vector3TypeConverter))]
		public override SerializableVector3 LinearVelocity
		{
			get { return GetSubTypeEntity().LinearVelocity; }
			set
			{
				if (GetSubTypeEntity().LinearVelocity.Equals(value)) return;
				GetSubTypeEntity().LinearVelocity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateEntityLinearVelocity;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Character")]
		[Browsable(false)]
		public MyObjectBuilder_Battery Battery
		{
			get { return GetSubTypeEntity().Battery; }
			set
			{
				if (GetSubTypeEntity().Battery == value) return;
				GetSubTypeEntity().Battery = value;
				Changed = true;
			}
		}

		[Category("Character")]
		public float BatteryLevel
		{
			get
			{
				float originalValue = GetSubTypeEntity().Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				return percentageValue;
			}
			set
			{
				float originalValue = GetSubTypeEntity().Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				if (percentageValue == value) return;
				GetSubTypeEntity().Battery.CurrentCapacity = value / 10000000;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBatteryLevel;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Character")]
		public float Health
		{
			get
			{
				float health = GetSubTypeEntity().Health.GetValueOrDefault(-1);
				if (BackingObject != null)
					if (health <= 0)
						health = InternalGetCharacterHealth();
				return health;
			}
			set
			{
				if (Health == value) return;

				if (BackingObject != null)
				{
					Action action = InternalDamageCharacter;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}

				GetSubTypeEntity().Health = value;
				Changed = true;
			}
		}

		[Category("Character")]
		[Browsable(false)]
		public InventoryEntity Inventory
		{
			get
			{
				return m_Inventory;
			}
		}

		[Category("Character")]
		[Browsable(true)]
		public bool DampenersEnabled
		{
			get { return GetSubTypeEntity().DampenersEnabled; }
		}

		[Category("Character")]
		[Browsable(true)]
		public bool JetpackEnabled
		{
			get { return GetSubTypeEntity().JetpackEnabled; }
		}

		[Category("Character")]
		[Browsable(true)]
		public bool LightEnabled
		{
			get { return GetSubTypeEntity().LightEnabled; }
		}

		#endregion

		#region "Methods"

		new public void Dispose()
		{
			LogManager.APILog.WriteLine("Disposing CharacterEntity '" + Name + "'");

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnPlayerLeft;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);

			m_isDisposed = true;

			base.Dispose();
		}

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal MyObjectBuilder_Character GetSubTypeEntity()
		{
			MyObjectBuilder_Character character = (MyObjectBuilder_Character)BaseEntity;

			//Make sure the inventory is up-to-date
			Inventory.RefreshInventory();
			character.Inventory = Inventory.GetSubTypeEntity();

			return character;
		}

		#region "Internal"

		protected string InternalGetDisplayName()
		{
			try
			{
				string name = (string)InvokeEntityMethod(BackingObject, CharacterGetDisplayNameMethod, new object[] { });

				return name;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return "";
			}
		}

		protected float InternalGetCharacterHealth()
		{
			try
			{
				float health = (float)InvokeEntityMethod(BackingObject, CharacterGetHealthMethod, new object[] { });

				return health;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return -1;
			}
		}

		protected void InternalDamageCharacter()
		{
			try
			{
				float damage = InternalGetCharacterHealth() - Health;
				InvokeEntityMethod(BackingObject, CharacterDamageCharacterMethod, new object[] { damage, MyDamageType.Unknown, true });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected Object InternalGetCharacterBattery()
		{
			try
			{
				Object battery = InvokeEntityMethod(BackingObject, CharacterGetBatteryCapacityMethod, new object[] { });

				return battery;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateBatteryLevel()
		{
			try
			{
				float capacity = Battery.CurrentCapacity;
				Object battery = InternalGetCharacterBattery();
				InvokeEntityMethod(battery, CharacterSetBatteryCapacityMethod, new object[] { capacity });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected Object InternalGetCharacterInventory()
		{
			try
			{
				Object inventory = InvokeEntityMethod(BackingObject, CharacterGetInventoryMethod, new object[] { 0 });

				return inventory;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		public void InternalRegisterCharacterClosedEvent()
		{
			try
			{
				Action<Object> action = InternalCharacterClosedEvent;

				MethodInfo method = BackingObject.GetType().GetMethod(BaseEntityCombineOnClosedEventMethod);
				method.Invoke(BackingObject, new object[] { action });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		public void InternalCharacterClosedEvent(Object entity)
		{
			try
			{
				if (IsDisposed)
					return;

				Dispose();
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#endregion
	}
}
