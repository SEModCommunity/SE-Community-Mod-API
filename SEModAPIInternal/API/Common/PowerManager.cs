using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	[DataContract]
	public class PowerManager
	{
		#region "Attributes"

		protected Object m_powerManager;

		public static string PowerManagerNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerManagerClass = "9EF82B537508DA0589AD02ADCEF4F09E";

		public static string PowerManagerRegisterPowerReceiverMethod = "7EC24D67D0C5C545B823312948C7C53D";
		public static string PowerManagerUnregisterPowerReceiverMethod = "EF8CE8BD7E3A4EC649E71B15336AB6E3";
		public static string PowerManagerRegisterPowerProducerMethod = "5E8FD2CD7A15CB64013EA628AA5A6F36";
		public static string PowerManagerUnregisterPowerProducerMethod = "7D2580FB3DEF36528602A4087A148755";
		public static string PowerManagerGetAvailablePowerMethod = "D807FF48CBA89A66D5325A7FE26F1CF3";
		public static string PowerManagerGetUsedPowerPercentMethod = "AD3ACAD83021E7C4396AC6DDB26F51A9";

		public static string PowerManagerPowerReceiverHashSetField = "FA93081ED4667D9B994BBD362F5BCB03";
		public static string PowerManagerPowerProducerHashSetField = "9923D3B372EC5E98B4B3E4F043C89137";
		public static string PowerManagerTotalPowerField = "2321C01912603DCD25560D74826632AC";

		#endregion

		#region "Constructors and Initializers"

		public PowerManager(Object powerManager)
		{
			m_powerManager = powerManager;
		}

		#endregion

		#region "Properties"

		public float TotalPower
		{
			get
			{
				try
				{
					FieldInfo field = BaseObject.GetEntityField(m_powerManager, PowerManagerTotalPowerField);
					float totalPower = (float)field.GetValue(m_powerManager);
					return totalPower;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return 0;
				}
			}
		}

		public float AvailablePower
		{
			get
			{
				try
				{
					float availablePower = TotalPower - (float)BaseObject.InvokeEntityMethod(m_powerManager, PowerManagerGetAvailablePowerMethod);
					return availablePower;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return 0;
				}
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PowerManagerNamespace, PowerManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PowerManager");
				bool result = true;
				result &= BaseObject.HasMethod(type1, PowerManagerRegisterPowerReceiverMethod);
				result &= BaseObject.HasMethod(type1, PowerManagerUnregisterPowerReceiverMethod);
				result &= BaseObject.HasMethod(type1, PowerManagerRegisterPowerProducerMethod);
				result &= BaseObject.HasMethod(type1, PowerManagerUnregisterPowerProducerMethod);
				result &= BaseObject.HasMethod(type1, PowerManagerGetAvailablePowerMethod);
				result &= BaseObject.HasMethod(type1, PowerManagerGetUsedPowerPercentMethod);
				result &= BaseObject.HasField(type1, PowerManagerPowerReceiverHashSetField);
				result &= BaseObject.HasField(type1, PowerManagerPowerProducerHashSetField);
				result &= BaseObject.HasField(type1, PowerManagerTotalPowerField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void RegisterPowerReceiver(Object receiver)
		{
			BaseObject.InvokeEntityMethod(m_powerManager, PowerManagerRegisterPowerReceiverMethod, new object[] { receiver });
		}

		public void UnregisterPowerReceiver(Object receiver)
		{
			BaseObject.InvokeEntityMethod(m_powerManager, PowerManagerUnregisterPowerReceiverMethod, new object[] { receiver });
		}

		public void RegisterPowerProducer(Object producer)
		{
			BaseObject.InvokeEntityMethod(m_powerManager, PowerManagerRegisterPowerProducerMethod, new object[] { producer });
		}

		public void UnregisterPowerProducer(Object producer)
		{
			BaseObject.InvokeEntityMethod(m_powerManager, PowerManagerUnregisterPowerProducerMethod, new object[] { producer });
		}

		#endregion
	}
}
