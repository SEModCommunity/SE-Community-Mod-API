using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SysUtils.Utils;

using VRage.Common.Utils;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Server
{
	public class ServerAssemblyWrapper
	{
		#region "Attributes"

		private static ServerAssemblyWrapper m_instance;
		private static Assembly m_assembly;
		private DateTime m_lastRestart;

		public static string DedicatedServerNamespace = "83BCBFA49B3A2A6EC1BC99583DA2D399";
		public static string DedicatedServerClass = "49BCFF86BA276A9C7C0D269C2924DE2D";
		public static string DedicatedServerStartupBaseMethod = "26A7ABEA729FAE1F24679E21470F8E98";

		#endregion

		#region "Constructors and Initializers"

		protected ServerAssemblyWrapper()
		{
			m_instance = this;

			m_lastRestart = DateTime.Now;

			m_assembly = Assembly.UnsafeLoadFrom("SpaceEngineersDedicated.exe");

			Console.WriteLine("Finished loading ServerAssemblyWrapper");
		}

		#endregion

		#region "Properties"

		public static ServerAssemblyWrapper Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new ServerAssemblyWrapper();

				return m_instance;
			}
		}

		public static Type InternalType
		{
			get
			{
				Type dedicatedServerType = m_assembly.GetType(DedicatedServerNamespace + "." + DedicatedServerClass);
				return dedicatedServerType;
			}
		}

		#endregion

		#region "Methods"

		public bool StartServer(string worldName = "", string instanceName = "", bool useConsole = true, int restartLimit = 3)
		{
			if (restartLimit < 0)
				return false;

			try
			{
				//Make sure the log, if running, is closed out before we begin
				if (MyLog.Default != null)
					MyLog.Default.Close();

				SandboxGameAssemblyWrapper.Instance.SetNullRender(true);
				MyFileSystem.Reset();

				//Prepare the parameters
				bool isUsingInstance = false;
				if (instanceName != "")
					isUsingInstance = true;
				object[] methodParams = new object[]
				{
					instanceName,
					(string) null,
					isUsingInstance,
					useConsole
				};

				//Start the server
				MethodInfo serverStartupMethod = InternalType.GetMethod(DedicatedServerStartupBaseMethod, BindingFlags.Static | BindingFlags.NonPublic);
				serverStartupMethod.Invoke(null, methodParams);

				//Close out the log since the server has stopped running
				if (MyLog.Default != null)
					MyLog.Default.Close();

				return false;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLineAndConsole("Server crashed, attempting auto-restart ...");
				LogManager.GameLog.WriteLine(ex);

				TimeSpan timeSinceLastRestart = DateTime.Now - m_lastRestart;
				m_lastRestart = DateTime.Now;

				//Reset the restart limit if the server has been running for more than 5 minutes before the crash
				if (timeSinceLastRestart.TotalMinutes > 5)
					restartLimit = 3;

				//Close out the log since the server has stopped running
				if(MyLog.Default != null)
					MyLog.Default.Close();

				//Recursively start the server again
				return StartServer(worldName, instanceName, useConsole, restartLimit - 1);
			}
		}

		public void StopServer()
		{
			try
			{
				//TODO - Find the right method to use for shut down
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
