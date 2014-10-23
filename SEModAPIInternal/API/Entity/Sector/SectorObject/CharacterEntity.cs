using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;

using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Game.Weapons;


using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;
using System.Reflection;

namespace SEModAPIInternal.API.Entity.Sector.SectorObject
{
	[DataContract(Name = "CharacterEntityProxy")]
	public class CharacterEntity : BaseEntity
	{
		#region "Attributes"

		private CharacterEntityNetworkManager m_networkManager;
		private InventoryEntity m_inventory;
		private static Type m_internalType;

		public static string CharacterNamespace = "F79C930F3AD8FDAF31A59E2702EECE70";
		public static string CharacterClass = "3B71F31E6039CAE9D8706B5F32FE468D";

		public static string CharacterGetHealthMethod = "7047AFF5D44FC8A44572E92DBAD13011";
		public static string CharacterDamageCharacterMethod = "DoDamage";
		public static string CharacterSetHealthMethod = "92A0500FD8772AB1AC3A6F79FD2A1C72";
		public static string CharacterGetBatteryMethod = "CF72A89940254CB8F535F177150FC743";
		public static string CharacterGetInventoryMethod = "GetInventory";
		public static string CharacterGetDisplayNameMethod = "DB913685BC5152DC19A4796E9E8CF659";
		public static string CharacterGetNetworkManagerMethod = "7605238EEE4275E4AD5608E7BD34D9C2";

		public static string CharacterItemListField = "02F6468D864F3203482135334BEB58AD";

		///////////////////////////////////////////////////////////

		public static string CharacterBatteryNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string CharacterBatteryClass = "328929D5EC05DF770D51383F6FC0B025";

		public static string CharacterBatterySetBatteryCapacityMethod = "C3BF60F3540A8A48CB8FEE0CDD3A95C6";

		public static string CharacterBatteryCapacityField = "0BAEC0F968A4BEAE30E7C46D9406765C";

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
			m_networkManager = new CharacterEntityNetworkManager(this, GetCharacterNetworkManager());

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnCharacterCreated;
			newEvent.timestamp = DateTime.Now;
			newEvent.entity = this;
			newEvent.priority = 1;
			EntityEventManager.Instance.AddEvent(newEvent);
		}

		#endregion

		#region "Properties"

		[IgnoreDataMember]
		[Browsable(false)]
		[ReadOnly(true)]
		new internal static Type InternalType
		{
			get
			{
				if (m_internalType == null)
					m_internalType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CharacterNamespace, CharacterClass);
				return m_internalType;
			}
		}

		[DataMember]
		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public override string Name
		{
			get
			{
				if(BackingObject == null || SteamId == 0)
					return DisplayName;

				//string name = PlayerMap.Instance.GetPlayerNameFromSteamId(SteamId);
				string name = ""+SteamId;
				return name;
			}
		}

		[DataMember]
		[Category("Character")]
		[ReadOnly(true)]
		public override string DisplayName
		{
			get { return ObjectBuilder.DisplayName; }
			set
			{
				if (ObjectBuilder.DisplayName == value) return;
				ObjectBuilder.DisplayName = value;
				Changed = true;

				base.DisplayName = value;
			}
		}

		[DataMember]
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

		[IgnoreDataMember]
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

		[DataMember]
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

		[DataMember]
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

		[IgnoreDataMember]
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

		[DataMember]
		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public bool DampenersEnabled
		{
			get { return ObjectBuilder.DampenersEnabled; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public bool JetpackEnabled
		{
			get { return ObjectBuilder.JetpackEnabled; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public bool LightEnabled
		{
			get { return ObjectBuilder.LightEnabled; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Character")]
		[Browsable(true)]
		[ReadOnly(true)]
		public ulong SteamId
		{
			get { return PlayerMap.Instance.GetSteamId(EntityId); }
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
					throw new Exception("Could not find internal type for CharacterEntity");
				bool result = true;
				result &= BaseObject.HasMethod(type, CharacterGetHealthMethod);
				result &= BaseObject.HasMethod(type, CharacterDamageCharacterMethod);
				result &= BaseObject.HasMethod(type, CharacterSetHealthMethod);
				result &= BaseObject.HasMethod(type, CharacterGetBatteryMethod);
				result &= BaseObject.HasMethod(type, CharacterGetInventoryMethod);
				result &= BaseObject.HasMethod(type, CharacterGetDisplayNameMethod);
				result &= BaseObject.HasMethod(type, CharacterGetNetworkManagerMethod);
				result &= BaseObject.HasField(type, CharacterItemListField);

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CharacterBatteryNamespace, CharacterBatteryClass);
				if (type2 == null)
					throw new Exception("Could not find battery type for CharacterEntity");
				result &= BaseObject.HasMethod(type2, CharacterBatterySetBatteryCapacityMethod);
				result &= BaseObject.HasField(type2, CharacterBatteryCapacityField);

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
			m_isDisposed = true;

			LogManager.APILog.WriteLine("Disposing CharacterEntity '" + Name + "'");

			EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
			newEvent.type = EntityEventManager.EntityEventType.OnCharacterDeleted;
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

		protected Object GetCharacterNetworkManager()
		{
			Object result = InvokeEntityMethod(BackingObject, CharacterGetNetworkManagerMethod);
			return result;
		}

		protected string InternalGetDisplayName()
		{
			try
			{
				string name = (string)InvokeEntityMethod(BackingObject, CharacterGetDisplayNameMethod, new object[] { });

				return name;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
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
				LogManager.ErrorLog.WriteLine(ex);
				return -1;
			}
		}

		protected void InternalDamageCharacter()
		{
			try
			{
				float damage = InternalGetCharacterHealth() - Health;
				MyDamageType damageType = MyDamageType.Unknown;
				if (Health <= 0)
					damageType = MyDamageType.Suicide;
				InvokeEntityMethod(BackingObject, CharacterDamageCharacterMethod, new object[] { damage, damageType, true });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected Object InternalGetCharacterBattery()
		{
			try
			{
				Object battery = InvokeEntityMethod(BackingObject, CharacterGetBatteryMethod, new object[] { });

				return battery;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		protected void InternalUpdateBatteryLevel()
		{
			try
			{
				float capacity = Battery.CurrentCapacity;
				Object battery = InternalGetCharacterBattery();
				InvokeEntityMethod(battery, CharacterBatterySetBatteryCapacityMethod, new object[] { capacity });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
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
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		#endregion

		#endregion
	}

	public class CharacterEntityNetworkManager
	{
		#region "Attributes"

		private static bool m_isRegistered;

		private CharacterEntity m_parent;
		private Object m_backingObject;

		public static string CharacterNetManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string CharacterNetManagerClass = "FA70B722FFD1F55F5A5019DA2499E60B";

		//Packets
		//2
		//3
		//4 - Character orientation
		//22
		//4758 - Character model name and color HSV
		//7414 - Character main data
		//7415
		//7416

		#endregion

		#region "Constructors and Initializers"

		public CharacterEntityNetworkManager(CharacterEntity parent, Object backingObject)
		{
			m_parent = parent;
			m_backingObject = backingObject;

			Action action = RegisterPacketHandlers;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(CharacterNetManagerNamespace, CharacterNetManagerClass);
				return type;
			}
		}

		public Object BackingObject
		{
			get { return m_backingObject; }
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for CharacterEntityNetworkManager");
				bool result = true;
				//result &= BaseObject.HasMethod(type, CharacterGetHealthMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine(ex);
				return false;
			}
		}

		protected static void RegisterPacketHandlers()
		{
			try
			{
				if (m_isRegistered)
					return;

				bool result = true;
				/*
				Type packetType = InternalType.GetNestedType("3BEB0A4A04463445218D632E2CD94536", BindingFlags.Public | BindingFlags.NonPublic);
				MethodInfo method = typeof(CharacterEntityNetworkManager).GetMethod("ReceiveMainDataPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				result &= NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType, method, InternalType);
				packetType = InternalType.GetNestedType("06F1DF314B7D765E189DFBBF84C09B00", BindingFlags.Public | BindingFlags.NonPublic);
				method = typeof(CharacterEntityNetworkManager).GetMethod("ReceiveOrientationPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				result &= NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Instance, packetType, method, InternalType);

				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType("AAC05F537A6F0F6775339593FBDFC564", "7B40EEB62BF9EBADF967050BFA3976CA");
				packetType = type.GetNestedType("4850B8A3B1027F683755D493244815AA", BindingFlags.Public | BindingFlags.NonPublic);
				method = typeof(CharacterEntityNetworkManager).GetMethod("ReceiveSpawnPacket", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
				result &= NetworkManager.RegisterCustomPacketHandler(PacketRegistrationType.Static, packetType, method, InternalType);
				*/
				if (!result)
					return;

				m_isRegistered = true;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void ReceiveMainDataPacket<T>(Object instanceNetManager, ref T packet, Object masterNetManager) where T : struct
		{
			try
			{
				MethodInfo basePacketHandlerMethod = BaseObject.GetStaticMethod(InternalType, "4055A1176BF0FA0C554491A3206CD656");
				basePacketHandlerMethod.Invoke(null, new object[] { instanceNetManager, packet, masterNetManager });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void ReceiveOrientationPacket<T>(Object instanceNetManager, ref T packet, Object masterNetManager) where T : struct
		{
			try
			{
				MethodInfo basePacketHandlerMethod = BaseObject.GetStaticMethod(InternalType, "F990CA0A818DDC8A56001B3D630EE54C");
				basePacketHandlerMethod.Invoke(null, new object[] { instanceNetManager, packet, masterNetManager });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		protected static void ReceiveSpawnPacket<T>(ref T packet, Object masterNetManager) where T : struct
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType("AAC05F537A6F0F6775339593FBDFC564", "7B40EEB62BF9EBADF967050BFA3976CA");
				MethodInfo basePacketHandlerMethod = BaseObject.GetStaticMethod(type, "364216D779218E8D22F3991B8FBA170A");
				basePacketHandlerMethod.Invoke(null, new object[] { packet, masterNetManager });
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
