using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity.Sector
{
	public class BatteryEntity : BaseObject
	{
		#region "Attributes"

		protected PowerManager m_parentPowerManager;
		protected float m_powerInputRate;	//in MW (megawatts)
		protected float m_powerOutputRate;	//in MW (megawatts)
		protected float m_powerMaxCapacity;	//in MJ (megajoules)

		public static string BatteryNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string BatteryClass = "328929D5EC05DF770D51383F6FC0B025";
		public static string BatteryInitFromObjectBuilderMethod = "ACB2C225540B096AAF057FD35CF2E87D";
		public static string BatteryGetCurrentCapacityMethod = "E80F74B7BDC457CEBD0E61DDCAB57398";
		public static string BatterySetCurrentCapacityMethod = "DF68FCA120D54CC1EC18AF7F06B426CF";
		public static string BatteryGetProducerEnabledMethod = "4A1669EF365B3728E3D8D8651474C170";
		public static string BatterySetProducerEnabledMethod = "18C96C726F64BAF2B00697AD4436C42C";
		public static string BatteryGetPowerOutputMethod = "083A16BEA16C103A875FE1F35AF145A5";
		public static string BatterySetPowerOutputMethod = "E68168DF3F1F7FB98C70A800772301D6";
		public static string BatteryGetPowerReceiverMethod = "get_PowerReceiver";
		public static string BatterySetPowerReceiverMethod = "976FC090F0BE88CB4342C1084A962B3D";
		public static string BatteryUpdateMethod = "27608CF8A780F418515861FE13FC7CAC";
		public static string BatteryPowerReceiverField = "6059CCFDAE1F9677267A15EB7F5B520D";
		public static string BatteryNetworkManagerField = "2F5B63E5862CA37D1924667D787601A3";

		public static string BatteryNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string BatteryNetworkManagerClass = "49C494F48F1DDFFBF3C07546ACC34140";

		public static string PowerReceiverNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerReceiverClass = "BD46495A378D4E0EEAB7EADCF05C74F1";
		public static string PowerReceiverRunPowerRateCallbackMethod = "29C1EE7C1DFD2BFF25004AFFEFA00723";
		public static string PowerReceiverGetCurrentInputMethod = "D1114EB448825412789ECD52D28D40A1";
		public static string PowerReceiverMaxRequiredInputField = "59318896499727A72FF42D624ECE3084";

		//3 - Door, 5 - Battery
		public static string PowerReceiverTypeEnumNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerReceiverTypeEnumClass = "0CAE5E7398171101A465139DC3A8A6A4";

		#endregion

		#region "Constructors and Initializers"

		public BatteryEntity(MyObjectBuilder_Battery battery, PowerManager powerManager, float inputRate, float outputRate, float maxCapacity)
			: base(battery)
		{
			m_parentPowerManager = powerManager;

			m_powerInputRate = inputRate;
			m_powerOutputRate = outputRate;
			m_powerMaxCapacity = maxCapacity;

			Action action = InternalSetupBattery;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		public float MaxCapacity
		{
			get { return m_powerMaxCapacity; }
			set
			{
				if (m_powerMaxCapacity == value) return;
				m_powerMaxCapacity = value;
				Changed = true;
			}
		}

		public float CurrentCapacity
		{
			get { return GetSubTypeEntity().CurrentCapacity; }
			set
			{
				//if (GetSubTypeEntity().CurrentCapacity == value) return;
				GetSubTypeEntity().CurrentCapacity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBattery;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		public bool ProducerEnabled
		{
			get { return GetSubTypeEntity().ProducerEnabled; }
			set
			{
				//if (GetSubTypeEntity().ProducerEnabled == value) return;
				GetSubTypeEntity().ProducerEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBattery;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		public float PowerInput
		{
			get { return m_powerInputRate; }
			set
			{
				if (m_powerInputRate == value) return;
				m_powerInputRate = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBattery;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		public float PowerOutput
		{
			get { return m_powerOutputRate; }
			set
			{
				if (m_powerOutputRate == value) return;
				m_powerOutputRate = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBattery;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		new internal virtual MyObjectBuilder_Battery GetSubTypeEntity()
		{
			return (MyObjectBuilder_Battery)BaseEntity;
		}

		public void Update()
		{
			if (BackingObject == null) return;
			InvokeEntityMethod(BackingObject, BatteryUpdateMethod);
		}

		#region "Internal"

		protected void InternalSetupBattery()
		{
			try
			{
				Assembly assembly = SandboxGameAssemblyWrapper.Instance.GameAssembly;
				Type batteryType = assembly.GetType(BatteryNamespace + "." + BatteryClass);
				Type characterType = assembly.GetType(CharacterEntity.CharacterNamespace + "." + CharacterEntity.CharacterClass);

				Object tempCharacterObject = Activator.CreateInstance(characterType);
				BackingObject = Activator.CreateInstance(batteryType, new object[] { tempCharacterObject });

				//InvokeEntityMethod(BackingObject, BatteryInitFromObjectBuilderMethod, new object[] { GetSubTypeEntity() });
				InternalInit();

				m_powerOutputRate = InternalGetPowerOutput();

				m_parentPowerManager.RegisterPowerProducer(BackingObject);
				m_parentPowerManager.RegisterPowerReceiver(BackingObject);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected Object CreatePowerReceiver()
		{
			try
			{
				Assembly assembly = SandboxGameAssemblyWrapper.Instance.GameAssembly;
				Type someEnum = assembly.GetType(PowerReceiverTypeEnumNamespace + "." + PowerReceiverTypeEnumClass);
				Array someEnumValues = someEnum.GetEnumValues();
				Object enumValue = someEnumValues.GetValue(5);

				Type powerReceiverType = assembly.GetType(PowerReceiverNamespace + "." + PowerReceiverClass);

				Func<float> func = new Func<float>(PowerInputRate);
				Object powerReceiver = Activator.CreateInstance(powerReceiverType, new object[] { enumValue, true, m_powerInputRate, func });

				return powerReceiver;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected Object GetPowerReceiver()
		{
			try
			{
				Object result = InvokeEntityMethod(BackingObject, BatteryGetPowerReceiverMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected float PowerInputRate()
		{
			if (CurrentCapacity >= m_powerMaxCapacity)
				return 0.0f;
			else
				return m_powerInputRate;
		}

		protected void InternalInit()
		{
			try
			{
				Object powerReceiver = CreatePowerReceiver();
				InvokeEntityMethod(BackingObject, BatterySetPowerReceiverMethod, new object[] { powerReceiver });
				InvokeEntityMethod(powerReceiver, PowerReceiverRunPowerRateCallbackMethod);

				ProducerEnabled = true;
				CurrentCapacity = 0;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected void InternalUpdateBattery()
		{
			try
			{
				InvokeEntityMethod(BackingObject, BatterySetCurrentCapacityMethod, new object[] { CurrentCapacity });
				InvokeEntityMethod(BackingObject, BatterySetPowerOutputMethod, new object[] { PowerOutput });
				InvokeEntityMethod(BackingObject, BatterySetProducerEnabledMethod, new object[] { ProducerEnabled });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected float InternalGetPowerOutput()
		{
			try
			{
				float result = (float)InvokeEntityMethod(BackingObject, BatteryGetPowerOutputMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return 0.0f;
			}
		}

		#endregion

		#endregion
	}
}
