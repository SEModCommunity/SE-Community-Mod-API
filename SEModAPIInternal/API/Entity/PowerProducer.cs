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
	public class PowerProducer
	{
		#region "Attributes"

		private PowerManager m_parent;
		private Object m_powerProducer;

		protected float m_maxPowerOutput;
		protected float m_powerOutput;

		public static string PowerProducerNamespace = "FB8C11741B7126BD9C97FE76747E087F";
		public static string PowerProducerClass = "7E69388ED0DB47818FB7AFF9F16C6EDA";

		public static string PowerProducerGetMaxPowerOutputMethod = "76FD8E0B2EFDFCA3480DCB00C0AA2818";
		public static string PowerProducerGetCurrentOutputMethod = "225CE0D4BF4EE7A1ECCCA6574B34B214";
		public static string PowerProducerSetCurrentOutputMethod = "B7380636860CA1CC479BAD06EE8033E0";

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
