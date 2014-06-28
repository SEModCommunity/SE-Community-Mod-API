using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using VRage.Common.Utils;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Server
{
	public class ServerAssemblyWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static ServerAssemblyWrapper m_instance;

		private Assembly m_assembly;

		public static string DedicatedServerNamespace = "83BCBFA49B3A2A6EC1BC99583DA2D399";
		public static string DedicatedServerClass = "49BCFF86BA276A9C7C0D269C2924DE2D";
		public static string DedicatedServerStartupBaseMethod = "26A7ABEA729FAE1F24679E21470F8E98";

		#endregion

		#region "Constructors and Initializers"

		protected ServerAssemblyWrapper(string path)
			: base(path)
		{
			m_instance = this;

			//string assemblyPath = Path.Combine(path, "SpaceEngineersDedicated.exe");
			m_assembly = Assembly.UnsafeLoadFrom("SpaceEngineersDedicated.exe");

			Console.WriteLine("Finished loading ServerAssemblyWrapper");
		}

		new public static ServerAssemblyWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new ServerAssemblyWrapper(basePath);
			}
			return (ServerAssemblyWrapper)m_instance;
		}

		#endregion

		#region "Properties"

		new public static bool IsDebugging
		{
			get
			{
				ServerAssemblyWrapper.GetInstance();
				return m_isDebugging;
			}
			set
			{
				ServerAssemblyWrapper.GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		public bool StartServer(string worldName)
		{
			try
			{
				SandboxGameAssemblyWrapper.SetNullRender(true);

				MyFileSystem.Reset();
				object[] methodParams = new object[]
				{
					"",
					(string) null,
					false,
					true
				};
				Type dedicatedServerType = m_assembly.GetType(DedicatedServerNamespace + "." + DedicatedServerClass);
				MethodInfo serverStartupMethod = dedicatedServerType.GetMethod(DedicatedServerStartupBaseMethod, BindingFlags.Static | BindingFlags.NonPublic);
				serverStartupMethod.Invoke(null, methodParams);

				return false;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return false;
			}
		}

		#endregion
	}
}
