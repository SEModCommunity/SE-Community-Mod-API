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
		private static MyLog m_chatLog;

		public static string MainGameMyLogField = "1976E5D4FE6E8C1BD369273DEE0025AC";

		#endregion

		#region "Constructors and Initializers"

		protected LogManager()
		{
			m_instance = this;

			Console.WriteLine("Finished loading LogManager");
		}

		#endregion

		#region "Properties"

		public static LogManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new LogManager();

				return m_instance;
			}
		}

		public static MyLog GameLog
		{
			get
			{
				if (m_gameLog == null)
				{
					var temp = Instance;

					try
					{
						Type mainGameType = SandboxGameAssemblyWrapper.Instance.MainGameType;
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
				if (m_apiLog == null)
				{
					var temp = Instance;

					try
					{
						m_apiLog = new MyLog();
						StringBuilder internalAPIAppVersion = new StringBuilder(typeof(LogManager).Assembly.GetName().Version.ToString());
						m_apiLog.Init("SEModAPIInternal.log", internalAPIAppVersion);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}

				return m_apiLog;
			}
		}

		public static MyLog ChatLog
		{
			get
			{
				if (m_chatLog == null)
				{
					var temp = Instance;

					try
					{
						m_chatLog = new MyLog();
						StringBuilder internalAPIAppVersion = new StringBuilder(typeof(LogManager).Assembly.GetName().Version.ToString());
						m_chatLog.Init("SEModAPIInternal_Chat.log", internalAPIAppVersion);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.ToString());
					}
				}

				return m_chatLog;
			}
		}

		#endregion
	}
}
