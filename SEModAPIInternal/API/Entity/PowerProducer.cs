using System;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class PowerProducer
	{
		#region "Attributes"

		private PowerManager m_parent;
		private Object m_powerProducer;

		protected float m_maxPowerOutput;
		protected float m_powerOutput;

		public static string PowerProducerNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerProducerClass = "7E69388ED0DB47818FB7AFF9F16C6EDA";

		public static string PowerProducerGetMaxPowerOutputMethod = "387129D7EFDBCE7A4ED1B9178FD8D732";
		public static string PowerProducerGetCurrentOutputMethod = "9B0E679A5F8A3F9EE1F9A2FEE446F13F";
		public static string PowerProducerSetCurrentOutputMethod = "2BA99BA0FEA6613A8408A4EC072B0484";

		#endregion

		#region "Constructors and Initializers"

		public PowerProducer(PowerManager parent, Object powerProducer)
		{
			m_parent = parent;
			m_powerProducer = powerProducer;

			m_maxPowerOutput = 0;
			m_powerOutput = 0;

			m_maxPowerOutput = MaxPowerOutput;
			m_powerOutput = PowerOutput;
		}

		#endregion

		#region "Properties"

		public float MaxPowerOutput
		{
			get
			{
				if (m_powerProducer == null)
					return m_maxPowerOutput;

				try
				{
					float result = (float)BaseObject.InvokeEntityMethod(m_powerProducer, PowerProducerGetMaxPowerOutputMethod);
					return result;
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
					return m_maxPowerOutput;
				}
			}
		}

		public float PowerOutput
		{
			get
			{
				if (m_powerProducer == null)
					return m_powerOutput;

				try
				{
					float result = (float)BaseObject.InvokeEntityMethod(m_powerProducer, PowerProducerGetCurrentOutputMethod);
					return result;
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
					return m_powerOutput;
				}
			}
			set
			{
				m_powerOutput = value;

				Action action = InternalUpdatePowerOutput;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(PowerProducerNamespace, PowerProducerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for PowerProducer");
				bool result = true;
				result &= BaseObject.HasMethod(type1, PowerProducerGetMaxPowerOutputMethod);
				result &= BaseObject.HasMethod(type1, PowerProducerGetCurrentOutputMethod);
				result &= BaseObject.HasMethod(type1, PowerProducerSetCurrentOutputMethod);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		protected void InternalUpdatePowerOutput()
		{
			BaseObject.InvokeEntityMethod(m_powerProducer, PowerProducerSetCurrentOutputMethod, new object[] { m_powerOutput });
		}

		#endregion
	}
}
