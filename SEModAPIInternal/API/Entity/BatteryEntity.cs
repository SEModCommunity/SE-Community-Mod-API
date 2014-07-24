using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIInternal.API.Entity.Sector
{
	[Obsolete]
	public class BatteryEntity : BaseObject
	{
		#region "Attributes"

		protected Object m_powerReceiver;
		protected PowerManager m_parentPowerManager;
		protected float m_powerInputRate;	//in MW (megawatts)
		protected float m_powerOutputRate;	//in MW (megawatts)
		protected float m_powerMaxCapacity;	//in MJ (megajoules)
		protected float m_currentCapacity;	//in MJ (megajoules)
		protected bool m_producerEnabled;

		//Battery Type
		public static string BatteryNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string BatteryClass = "328929D5EC05DF770D51383F6FC0B025";

		//Battery Methods
		public static string BatteryGetCurrentCapacityMethod =	"C039735A461FE5B2A1B15E3E64C0B182";
		public static string BatterySetCurrentCapacityMethod =	"C3BF60F3540A8A48CB8FEE0CDD3A95C6";
		public static string BatteryGetProducerEnabledMethod =	"CAE965B9044E66DDE3D37732C7016829";
		public static string BatterySetProducerEnabledMethod =	"2CA1CC44FD889DE688E240B449C91327";
		public static string BatteryGetPowerOutputMethod =		"94A581644BA2FE208B8F28BB42698AC4";
		public static string BatterySetPowerOutputMethod =		"66DD52AFBD694F8CA8584587C60C8A42";
		public static string BatteryGetPowerReceiverMethod =	"get_PowerReceiver";
		public static string BatterySetPowerReceiverMethod =	"976FC090F0BE88CB4342C1084A962B3D";
		public static string BatteryUpdateHasPowerMethod =		"A91C16B3A7E55EA805F02F7530C6BC8A";
		public static string BatteryHasPowerMethod =			"241617E8B1B0C927D0852E128336E816";

		//Battery Fields
		public static string BatteryCurrentCapacityField = "0BAEC0F968A4BEAE30E7C46D9406765C";
		public static string BatteryProducerEnabledField = "F5E407B2BB6524F07CCC1C043CC38BCF";
		public static string BatteryPowerOutputField = "84560E0035B8892361F434CE17311D11";
		public static string BatteryHasPowerField = "1FE8C047D525A7A32AE9E02979FA46C9";
		public static string BatteryPowerReceiverField = "6059CCFDAE1F9677267A15EB7F5B520D";
		public static string BatteryNetworkManagerField = "2F5B63E5862CA37D1924667D787601A3";
		public static string BatteryLastUpdateMillisecondsField = "71122D8FFE2A1E5B86AAE33427F12A5E";

		//Battery Network Manager Type
		public static string BatteryNetworkManagerNamespace = "5F381EA9388E0A32A8C817841E192BE8";
		public static string BatteryNetworkManagerClass = "49C494F48F1DDFFBF3C07546ACC34140";

		//Power Receiver Type
		public static string PowerReceiverNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerReceiverClass = "BD46495A378D4E0EEAB7EADCF05C74F1";

		//Power Receiver Methods
		public static string PowerReceiverRunPowerRateCallbackMethod = "29C1EE7C1DFD2BFF25004AFFEFA00723";
		public static string PowerReceiverGetCurrentInputMethod = "D1114EB448825412789ECD52D28D40A1";
		public static string PowerReceiverGetCurrentRateMethod = "C1054E3D5EB57AE417205025EF53EFCD";

		//Power Receiver Fields
		public static string PowerReceiverMaxRequiredInputField = "59318896499727A72FF42D624ECE3084";

		//3 - Door, 4 - Gravity Generator, 5 - Battery, 8 - BatteryBlock
		public static string PowerReceiverTypeEnumNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerReceiverTypeEnumClass = "0CAE5E7398171101A465139DC3A8A6A4";
		
		//0 - Solar Panel, 1 - Reactor, 2 - Battery, 3 - BatteryBlock
		public static string PowerProducerTypeEnumNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerProducerTypeEnumClass = "4A6D842A6ACB931992C3C36C49CD7544";

		#endregion

		#region "Constructors and Initializers"

		public BatteryEntity(MyObjectBuilder_Battery battery, PowerManager powerManager, float inputRate, float outputRate, float maxCapacity)
			: base(battery)
		{
			m_parentPowerManager = powerManager;

			m_powerInputRate = inputRate;
			m_powerOutputRate = outputRate;
			m_powerMaxCapacity = maxCapacity;
			m_currentCapacity = battery.CurrentCapacity;
			m_producerEnabled = battery.ProducerEnabled;

			Action action = InternalSetupBattery;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		#endregion

		#region "Properties"

		[Category("Battery")]
		[Description("Maximum capacity of the battery in megajoules")]
		public float MaxCapacity
		{
			get { return m_powerMaxCapacity; }
			set
			{
				if (m_powerMaxCapacity == value) return;
				m_powerMaxCapacity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBattery;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery")]
		[Description("Current capacity of the battery in megajoules")]
		public float CurrentCapacity
		{
			get { return m_currentCapacity; }
			set
			{
				//if (m_currentCapacity == value) return;
				m_currentCapacity = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBattery;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery")]
		[Description("Whether or not the output (aka producer) of the battery is enabled")]
		public bool ProducerEnabled
		{
			get { return m_producerEnabled; }
			set
			{
				//if (GetSubTypeEntity().ProducerEnabled == value) return;
				m_producerEnabled = value;
				Changed = true;

				if (BackingObject != null)
				{
					Action action = InternalUpdateBattery;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}
			}
		}

		[Category("Battery")]
		[Description("Input rate of the battery in megawatts per hour")]
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

		[Category("Battery")]
		[Description("Output rate of the battery in megawatts per hour")]
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

		[Category("Battery")]
		[Description("Internal power receiver object")]
		[Browsable(false)]
		protected Object PowerReceiver
		{
			get
			{
				try
				{
					Object currentReceiver = InvokeEntityMethod(BackingObject, BatteryGetPowerReceiverMethod);
					if (currentReceiver != null)
						m_powerReceiver = currentReceiver;

					return m_powerReceiver;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return m_powerReceiver;
				}
			}
			set
			{
				m_powerReceiver = value;
			}
		}

		#endregion

		#region "Methods"

		public void Update()
		{
			if (BackingObject == null) return;
			if (PowerReceiver == null) return;

			try
			{
				Type someType1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType("5F381EA9388E0A32A8C817841E192BE8", "0735AF1E7659DFDA65E92992C7ECBE13");
				Type someType2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType("AAC05F537A6F0F6775339593FBDFC564", "D580AE7552E79DAB03A3D64B1F7B67F9");

				bool someStaticValue1 = (bool)InvokeStaticMethod(someType1, "75DC57D5A6FF72BADC5278AC0A138D97");
				Object someType2Instance = InvokeStaticMethod(someType2, "93A33C4F0F6620ACAFCD191E8A26832E");
				bool isGlobalPowerDisabled = (bool)InvokeEntityMethod(someType2Instance, "AA5487FAD57571EA60A3EB79E8B16322");

				int gameTimeMillis = SandboxGameAssemblyWrapper.Instance.GetMainGameMilliseconds();
				float powerReceiverRate = (float)InvokeEntityMethod(PowerReceiver, PowerReceiverGetCurrentRateMethod);

				//TODO - Need to figure out what this boolean value indicates
				if (!someStaticValue1)
					return;

				InvokeEntityMethod(BackingObject, BatteryUpdateHasPowerMethod);

				if (InternalHasPower() || GetPowerReceiverCurrentInput() > 0.0f)
				{
					//Update time scale
					int timeSinceLastUpdate = gameTimeMillis - InternalGetLastUpdatedMilliseconds();
					InternalSetLastUpdatedMilliseconds(gameTimeMillis);

					//Calculate time-scaled power outputs as megawatts per millisecond
					float producerOutput = (float)timeSinceLastUpdate * (InternalGetPowerOutput() / 3600000f);	//This is typically a positive value
					float receiverOutput = (float)timeSinceLastUpdate * (-powerReceiverRate / 3600000f);			//This is typically a negative value

					//Update current capacity
					CurrentCapacity = MathHelper.Clamp(CurrentCapacity - (isGlobalPowerDisabled ? 0.0f : (producerOutput + receiverOutput)), 0.0f, MaxCapacity);
				}

				InvokeEntityMethod(BackingObject, BatteryUpdateHasPowerMethod);

				//TODO - Find out what we need to broadcast here to push the power updates out
			}
			catch(Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#region "Internal"

		protected void InternalSetupBattery()
		{
			try
			{
				Type batteryType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(BatteryNamespace, BatteryClass);

				BackingObject = Activator.CreateInstance(batteryType, new object[] { null });

				m_powerReceiver = CreatePowerReceiver();
				InvokeEntityMethod(BackingObject, BatterySetPowerReceiverMethod, new object[] { m_powerReceiver });
				InvokeEntityMethod(PowerReceiver, PowerReceiverRunPowerRateCallbackMethod);

				InternalUpdateBattery();

				InvokeEntityMethod(BackingObject, BatteryUpdateHasPowerMethod);

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

		protected float GetPowerReceiverCurrentInput()
		{
			float result = (float)InvokeEntityMethod(PowerReceiver, PowerReceiverGetCurrentInputMethod);

			return result;
		}

		protected float PowerInputRate()
		{
			if (CurrentCapacity >= m_powerMaxCapacity)
				return 0.0f;
			else
				return m_powerInputRate;
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

		protected float InternalGetCurrentCapacity()
		{
			try
			{
				float result = (float)InvokeEntityMethod(BackingObject, BatteryGetCurrentCapacityMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return 0.0f;
			}
		}

		protected bool InternalGetProducerEnabled()
		{
			try
			{
				bool result = (bool)InvokeEntityMethod(BackingObject, BatteryGetProducerEnabledMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return false;
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

		protected bool InternalHasPower()
		{
			try
			{
				bool result = (bool)InvokeEntityMethod(BackingObject, BatteryHasPowerMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return false;
			}
		}

		protected void InternalSetLastUpdatedMilliseconds(int value)
		{
			try
			{
				FieldInfo batteryLastUpdatedField = GetEntityField(BackingObject, BatteryLastUpdateMillisecondsField);
				batteryLastUpdatedField.SetValue(BackingObject, value);
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected int InternalGetLastUpdatedMilliseconds()
		{
			try
			{
				FieldInfo batteryLastUpdatedField = GetEntityField(BackingObject, BatteryLastUpdateMillisecondsField);
				int result = (int)batteryLastUpdatedField.GetValue(BackingObject);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return 0;
			}
		}

		#endregion

		#endregion
	}
}
