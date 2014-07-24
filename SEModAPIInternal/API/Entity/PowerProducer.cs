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

		#endregion

		#region "Constructors and Initializers"

		public PowerProducer(PowerManager parent, Object powerProducer)
		{
			m_parent = parent;
			m_powerProducer = powerProducer;

			m_maxPowerOutput = 0;
			m_powerOutput = 0;
		}

		#endregion

		#region "Properties"

		public float MaxPowerOutput
		{
			get
			{
				//TODO - Get this value directly from the entity
				return m_maxPowerOutput;
			}
			set
			{
				m_maxPowerOutput = value;

				Action action = InternalUpdateMaxPowerOutput;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
			}
		}

		public float PowerOutput
		{
			get
			{
				//TODO - Get this value directly from the entity
				return m_powerOutput;
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

		protected void InternalUpdateMaxPowerOutput()
		{
			//TODO - Do stuff
		}

		protected void InternalUpdatePowerOutput()
		{
			//TODO - Do stuff
		}

		#endregion
	}
}
