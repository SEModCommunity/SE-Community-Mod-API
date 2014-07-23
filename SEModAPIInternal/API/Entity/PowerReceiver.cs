using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Definitions;
using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class PowerReceiver
	{
		private Object m_parent;
		private Object m_powerReceiver;
		private float m_maxCapacity;
		private float m_maxRequiredInput;

		//Power Receiver Type
		public static string PowerReceiverNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerReceiverClass = "BD46495A378D4E0EEAB7EADCF05C74F1";

		//Power Receiver Methods
		public static string PowerReceiverRunPowerRateCallbackMethod = "29C1EE7C1DFD2BFF25004AFFEFA00723";
		public static string PowerReceiverGetCurrentInputMethod = "D1114EB448825412789ECD52D28D40A1";
		public static string PowerReceiverGetCurrentRateMethod = "C1054E3D5EB57AE417205025EF53EFCD";
		public static string PowerReceiverSetMaxRequiredInputMethod = "F45F239EB3FDF7512379B9147E014547";

		//Power Receiver Fields
		public static string PowerReceiverMaxRequiredInputField = "59318896499727A72FF42D624ECE3084";
		public static string PowerReceiverPowerRatioField = "0E8DD5E4ED55981F95E90E0524F6FA1A";
		public static string PowerReceiverInputRateCallbackField = "053C195FD883E42B0FB64E893A4000D6";

		//3 - Door, 4 - Gravity Generator, 5 - Battery, 8 - BatteryBlock
		public static string PowerReceiverTypeEnumNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerReceiverTypeEnumClass = "0CAE5E7398171101A465139DC3A8A6A4";

		public PowerReceiver(Object parent, Object powerReceiver)
		{
			m_parent = parent;
			m_powerReceiver = powerReceiver;

			m_maxRequiredInput = 0;
			m_maxCapacity = 0;
		}

		public void SetMaxRequiredInput(float input)
		{
			m_maxRequiredInput = input;

			Action action = InternalUpdateMaxRequiredInput;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		public void SetMaxCapacity(float capacity)
		{
			m_maxCapacity = capacity;
		}

		protected float PowerReceiverCallback()
		{
			return m_maxRequiredInput;
		}

		protected void InternalUpdateMaxRequiredInput()
		{
			try
			{
				if (m_maxRequiredInput == 0)
					return;

				FieldInfo field = BaseObject.GetEntityField(m_powerReceiver, PowerReceiverInputRateCallbackField);
				field.SetValue(m_powerReceiver, new Func<float>(PowerReceiverCallback));

				FieldInfo field2 = BaseObject.GetEntityField(m_powerReceiver, PowerReceiverMaxRequiredInputField);
				field2.SetValue(m_powerReceiver, m_maxRequiredInput);

				BaseObject.InvokeEntityMethod(m_powerReceiver, PowerReceiverSetMaxRequiredInputMethod, new object[] { m_maxRequiredInput });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}
	}
}
