using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity;

namespace SEModAPIInternal.API.Common
{
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

		public static string PowerManagerPowerReceiverHashSetField = "FA93081ED4667D9B994BBD362F5BCB03";
		public static string PowerManagerPowerProducerHashSetField = "9923D3B372EC5E98B4B3E4F043C89137";

		#endregion

		#region "Constructors and Initializers"

		public PowerManager(Object powerManager)
		{
			m_powerManager = powerManager;
		}

		#endregion

		#region "Properties"

		#endregion

		#region "Methods"

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
