﻿using System;
using System.ComponentModel;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class PowerReceiver
	{
		#region "Attributes"

		private Object m_parent;
		private PowerManager m_powerManager;
		private Object m_powerReceiver;
		private float m_maxRequiredInput;
		private Func<float> m_powerRateCallback;

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

		////////////////////////////////////////////////////////////////////

		//3 - Door, 4 - Gravity Generator, 5 - Battery, 8 - BatteryBlock
		public static string PowerReceiverTypeEnumNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerReceiverTypeEnumClass = "0CAE5E7398171101A465139DC3A8A6A4";

		#endregion

		#region "Constructors and Initializers"

		public PowerReceiver(Object parent, PowerManager powerManager, Object powerReceiver)
		{
			m_parent = parent;
			m_powerManager = powerManager;
			m_powerReceiver = powerReceiver;
			m_powerRateCallback = null;

			m_maxRequiredInput = 0;
		}

		public PowerReceiver(Object parent, PowerManager powerManager, Object powerReceiver, Func<float> powerRateCallback)
		{
			m_parent = parent;
			m_powerManager = powerManager;
			m_powerReceiver = powerReceiver;
			m_powerRateCallback = powerRateCallback;

			m_maxRequiredInput = 0;
		}

		#endregion

		#region "Properties"

		[Browsable(false)]
		[ReadOnly(true)]
		public Object BackingObject
		{
			get { return m_powerReceiver; }
		}

		public float MaxRequiredInput
		{
			get { return m_maxRequiredInput; }
			set
			{
				if (BackingObject == null || m_powerRateCallback == null)
					return;
				m_maxRequiredInput = value;

				Action action = InternalUpdateMaxRequiredInput;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			}
		}

		public float CurrentInput
		{
			get
			{
				if (BackingObject == null)
					return 0;

				float currentInput = (float)BaseObject.InvokeEntityMethod(BackingObject, PowerReceiverGetCurrentInputMethod);

				return currentInput;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PowerReceiverNamespace, PowerReceiverClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PowerReceiver");
				bool result = true;
				result &= BaseObject.HasMethod(type1, PowerReceiverRunPowerRateCallbackMethod);
				result &= BaseObject.HasMethod(type1, PowerReceiverGetCurrentInputMethod);
				result &= BaseObject.HasMethod(type1, PowerReceiverGetCurrentRateMethod);
				result &= BaseObject.HasMethod(type1, PowerReceiverSetMaxRequiredInputMethod);
				result &= BaseObject.HasField(type1, PowerReceiverMaxRequiredInputField);
				result &= BaseObject.HasField(type1, PowerReceiverPowerRatioField);
				result &= BaseObject.HasField(type1, PowerReceiverInputRateCallbackField);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		protected void InternalUpdateMaxRequiredInput()
		{
			try
			{
				if (m_powerRateCallback == null)
					return;

				BaseObject.SetEntityFieldValue(BackingObject, PowerReceiverInputRateCallbackField, m_powerRateCallback);

				BaseObject.SetEntityFieldValue(BackingObject, PowerReceiverMaxRequiredInputField, m_maxRequiredInput);

				BaseObject.InvokeEntityMethod(BackingObject, PowerReceiverSetMaxRequiredInputMethod, new object[] { m_maxRequiredInput });

				m_powerManager.UnregisterPowerReceiver(m_parent);
				m_powerManager.RegisterPowerReceiver(m_parent);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
