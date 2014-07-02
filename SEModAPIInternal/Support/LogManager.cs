using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SysUtils.Utils;

using SEModAPIInternal;
using SEModAPIInternal.API.Common;

namespace SEModAPIInternal.Support
{
	public class LogManager
	{
		#region "Attributes"

		private static LogManager m_instance;

		private static MyLog m_gameLog;
		private static MyLog m_apiLog;

		public static string MainGameMyLogField = "1976E5D4FE6E8C1BD369273DEE0025AC";

		#endregion

		#region "Constructors and Initializers"

		protected LogManager()
		{
			m_instance = this;

			m_apiLog = new MyLog();
			StringBuilder internalAPIAppVersion = new StringBuilder(typeof(LogManager).Assembly.GetName().Version.ToString());
			m_apiLog.Init("SEModAPIInternal.log", internalAPIAppVersion);

			Console.WriteLine("Finished loading LogManager");
		}

		public static LogManager GetInstance()
		{
			if (m_instance == null)
			{
				m_instance = new LogManager();
			}
			return (LogManager)m_instance;
		}

		#endregion

		#region "Properties"

		public static MyLog GameLog
		{
			get
			{
				GetInstance();

				if (m_gameLog == null)
				{
					try
					{
						Type mainGameType = SandboxGameAssemblyWrapper.GetInstance().MainGameType;
						FieldInfo myLogField = mainGameType.GetField(MainGameMyLogField, BindingFlags.Public | BindingFlags.Static);
						m_gameLog = (MyLog)myLogField.GetValue(null);
					}
					catch (Exception ex)
					{
						LogManager.APILog.WriteLine(ex);
					}
				}

				return m_gameLog;
			}
		}

		public static MyLog APILog
		{
			get
			{
				GetInstance();

				return m_apiLog;
			}
		}

		#endregion
	}
}
