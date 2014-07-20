using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class SaveManager
	{
		#region "Attributes"

		private static bool m_isSaving = false;

		public static string SaveManagerNamespace = "6D7C9F7F9CFF9877B430DBAFB54F1802";
		public static string SaveManagerClass = "8DEBD6C63930F8C065956AC979F27488";
		public static string SaveManagerSaveWorldMethod = "0E05B81B936D03329E9F49031001FE33";

		#endregion

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(SaveManagerNamespace, SaveManagerClass);
				return type;
			}
		}

		public static bool IsSaving
		{
			get { return m_isSaving; }
		}

		#endregion

		#region "Methods"

		public static void SaveWorld()
		{
			Action action = InternalSaveWorld;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		protected static void InternalSaveWorld()
		{
			try
			{
				m_isSaving = true;
				DateTime saveStartTime = DateTime.Now;
				BaseObject.InvokeStaticMethod(InternalType, SaveManagerSaveWorldMethod, new object[] { Type.Missing });
				TimeSpan timeToSave = DateTime.Now - saveStartTime;
				LogManager.GameLog.WriteLineAndConsole("Save complete and took " + timeToSave.TotalSeconds + " seconds");
				m_isSaving = false;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
