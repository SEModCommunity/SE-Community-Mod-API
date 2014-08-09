using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using SysUtils.Utils;

using SEModAPIInternal;
using SEModAPIInternal.API.Common;
using System.Threading;
using VRage.Common.Utils;

namespace SEModAPIInternal.Support
{
	public class ApplicationLog
	{
		#region "Attributes"

		private DirectoryInfo m_libraryPath;
		private FileInfo m_filePath;
		private StringBuilder m_stringBuilder;

		#endregion

		#region "Constructors and Initializers"

		public ApplicationLog(bool useGamePath = false)
		{
			if (useGamePath && SandboxGameAssemblyWrapper.Instance.IsGameStarted && MyFileSystem.UserDataPath != null)
			{
				m_libraryPath = new DirectoryInfo(MyFileSystem.UserDataPath);
			}
			else
			{
				string codeBase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				m_libraryPath = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(path), "Logs"));
				if (!m_libraryPath.Exists)
					Directory.CreateDirectory(m_libraryPath.ToString());
			}

			m_stringBuilder = new StringBuilder();
		}

		#endregion

		#region "Properties"

		public bool LogEnabled
		{
			get { return m_filePath != null; }
		}

		#endregion

		#region "Methods"

		public string GetFilePath()
		{
			if (m_filePath == null)
				return "";

			return m_filePath.ToString();
		}

		public void Init(string logFileName, StringBuilder appVersionString)
		{
			m_filePath = new FileInfo(Path.Combine(m_libraryPath.ToString(), logFileName));

			//If the log file already exists then archive it
			if (m_filePath.Exists)
			{
				DateTime lastWriteTime = m_filePath.LastWriteTime;
				string modifiedTimestamp = lastWriteTime.Year.ToString() + "_" + lastWriteTime.Month.ToString() + "_" + lastWriteTime.Day.ToString() + "_" + lastWriteTime.Hour.ToString() + "_" + lastWriteTime.Minute.ToString() + "_" + lastWriteTime.Second.ToString();
				string fileNameWithoutExtension = m_filePath.Name.Remove(m_filePath.Name.Length - m_filePath.Extension.Length);
				string newFileName = fileNameWithoutExtension + "_" + modifiedTimestamp + m_filePath.Extension;

				File.Move(m_filePath.ToString(), Path.Combine(m_libraryPath.ToString(), newFileName).ToString());
			}

			int num = (int)Math.Round((DateTime.Now - DateTime.UtcNow).TotalHours);

			WriteLine("Log Started");
			WriteLine("Timezone (local - UTC): " + num.ToString() + "h");
			WriteLine("App Version: " + appVersionString);
		}

		public void WriteLine(string msg)
		{
			if (m_filePath == null)
				return;
			try
			{
				m_stringBuilder.Clear();
				AppendDateAndTime(m_stringBuilder);
				m_stringBuilder.Append(" - ");
				AppendThreadInfo(m_stringBuilder);
				m_stringBuilder.Append(" -> ");
				m_stringBuilder.Append(msg);
				TextWriter m_Writer = new StreamWriter(m_filePath.ToString(), true);
				TextWriter.Synchronized(m_Writer).WriteLine(m_stringBuilder.ToString());
				m_Writer.Close();
				m_stringBuilder.Clear();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to write to log: " + ex.ToString());
			}

		}

		public void WriteLine(string message, LoggingOptions option)
		{
			WriteLine(message);
		}

		public void WriteLine(Exception ex)
		{
			if (m_filePath == null)
				return;

			if (ex == null)
				return;

			WriteLine(ex.ToString());

			if (ex.InnerException == null)
				return;

			WriteLine(ex.InnerException);
		}

		public void WriteLineAndConsole(string msg)
		{
			if (m_filePath == null)
				return;

			WriteLine(msg);

			m_stringBuilder.Clear();
			AppendDateAndTime(m_stringBuilder);
			m_stringBuilder.Append(" - ");
			m_stringBuilder.Append(msg);
			Console.WriteLine(m_stringBuilder.ToString());
			m_stringBuilder.Clear();
		}

		private int GetThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

		private void AppendDateAndTime(StringBuilder sb)
		{
			DateTimeOffset now = DateTimeOffset.Now;
			StringBuilderExtensions.Concat(sb, now.Year, 4U, '0', 10U, false).Append('-');
			StringBuilderExtensions.Concat(sb, now.Month, 2U).Append('-');
			StringBuilderExtensions.Concat(sb, now.Day, 2U).Append(' ');
			StringBuilderExtensions.Concat(sb, now.Hour, 2U).Append(':');
			StringBuilderExtensions.Concat(sb, now.Minute, 2U).Append(':');
			StringBuilderExtensions.Concat(sb, now.Second, 2U).Append('.');
			StringBuilderExtensions.Concat(sb, now.Millisecond, 3U);
		}

		private void AppendThreadInfo(StringBuilder sb)
		{
			sb.Append("Thread: " + GetThreadId().ToString());
		}

		#endregion
	}

	public class LogManager
	{
		#region "Attributes"

		private static LogManager m_instance;

		private static MyLog m_gameLog;
		private static ApplicationLog m_apiLog;
		private static ApplicationLog m_chatLog;
		private static ApplicationLog m_errorLog;

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
						Console.WriteLine(ex);
					}
				}

				if (m_gameLog == null)
					throw new Exception("Failed to load game log!");

				return m_gameLog;
			}
		}

		public static ApplicationLog APILog
		{
			get
			{
				if (m_apiLog == null)
				{
					var temp = Instance;

					try
					{
						m_apiLog = new ApplicationLog(true);
						StringBuilder internalAPIAppVersion = new StringBuilder(typeof(LogManager).Assembly.GetName().Version.ToString());
						m_apiLog.Init("SEModAPIInternal.log", internalAPIAppVersion);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}

				if (m_apiLog == null)
					throw new Exception("Failed to create API log!");

				return m_apiLog;
			}
		}

		public static ApplicationLog ChatLog
		{
			get
			{
				if (m_chatLog == null)
				{
					var temp = Instance;

					try
					{
						m_chatLog = new ApplicationLog(true);
						StringBuilder internalAPIAppVersion = new StringBuilder(typeof(LogManager).Assembly.GetName().Version.ToString());
						m_chatLog.Init("SEModAPIInternal_Chat.log", internalAPIAppVersion);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}

				if (m_chatLog == null)
					throw new Exception("Failed to create chat log!");

				return m_chatLog;
			}
		}

		public static ApplicationLog ErrorLog
		{
			get
			{
				if (m_errorLog == null)
				{
					var temp = Instance;

					try
					{
						m_errorLog = new ApplicationLog();
						StringBuilder internalAPIAppVersion = new StringBuilder(typeof(LogManager).Assembly.GetName().Version.ToString());
						m_errorLog.Init("SEModAPIInternal_Error.log", internalAPIAppVersion);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex);
					}
				}

				if (m_errorLog == null)
					throw new Exception("Failed to create error log!");

				return m_errorLog;
			}
		}

		#endregion
	}
}
