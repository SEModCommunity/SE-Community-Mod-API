using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	public class CharacterEntity : BaseEntity
	{
		#region "Attributes"

		private InventoryEntity m_inventory;
		private static Type m_internalType;

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

		public CharacterEntity(FileInfo characterFile)
			: base(null)
		{
			MyObjectBuilder_Character character = BaseObjectManager.LoadContentFile<MyObjectBuilder_Character, MyObjectBuilder_CharacterSerializer>(characterFile);
			ObjectBuilder = character;

			m_inventory = new InventoryEntity(character.Inventory);
		}

		public CharacterEntity(MyObjectBuilder_Character definition)
			: base(definition)
		{
			m_inventory = new InventoryEntity(definition.Inventory);
		}

		public CharacterEntity(MyObjectBuilder_Character definition, Object backingObject)
			: base(definition, backingObject)
		{
			m_inventory = new InventoryEntity(definition.Inventory, InternalGetCharacterInventory());

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnPlayerJoined;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		[ReadOnly(true)]
		internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CharacterNamespace, CharacterClass);
				return m_internalType;
			}
		}

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
		[Browsable(false)]
		[ReadOnly(true)]
		internal new MyObjectBuilder_Character ObjectBuilder
		{
			get
			{
				MyObjectBuilder_Character character = (MyObjectBuilder_Character)base.ObjectBuilder;

				//Make sure the inventory is up-to-date
				Inventory.RefreshInventory();
				character.Inventory = Inventory.ObjectBuilder;

				return character;
			}
			set
			{
				base.ObjectBuilder = value;
			}
		}

		[Category("Character")]
		[Browsable(false)]
		[ReadOnly(true)]
		public MyObjectBuilder_Battery Battery
		{
			get { return ObjectBuilder.Battery; }
			set
			{
				if (ObjectBuilder.Battery == value) return;
				ObjectBuilder.Battery = value;
				Changed = true;
			}
		}

		[Category("Character")]
		public float BatteryLevel
		{
			get
			{
				float originalValue = Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				return percentageValue;
			}
			set
			{
				float originalValue = Battery.CurrentCapacity;
				float percentageValue = (float)Math.Round(originalValue * 10000000, 2);
				if (percentageValue == value) return;
				Battery.CurrentCapacity = value / 10000000;
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
				float health = ObjectBuilder.Health.GetValueOrDefault(-1);
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

				ObjectBuilder.Health = value;
				Changed = true;
			}
		}

		[Category("Character")]
		[Browsable(false)]
		[ReadOnly(true)]
		public InventoryEntity Inventory
		{
			get
			{
				return m_inventory;
			}
		}

		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public bool DampenersEnabled
		{
			get { return ObjectBuilder.DampenersEnabled; }
		}

		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public bool JetpackEnabled
		{
			get { return ObjectBuilder.JetpackEnabled; }
		}

		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public bool LightEnabled
		{
			get { return ObjectBuilder.LightEnabled; }
		}

		#endregion

		#region "Methods"

		public override void Dispose()
		{
			m_isDisposed = true;

			LogManager.APILog.WriteLine("Disposing CharacterEntity '" + Name + "'");

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnPlayerLeft;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);

			base.Dispose();
		}

		public override void Export(FileInfo fileInfo)
		{
			BaseObjectManager.SaveContentFile<MyObjectBuilder_Character, MyObjectBuilder_CharacterSerializer>(ObjectBuilder, fileInfo);
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

		#endregion

		#endregion
	}
}
