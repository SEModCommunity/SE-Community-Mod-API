using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using VRage.Common.Utils;

namespace SEModAPI.API.Internal
{
	public class ServerAssemblyWrapper : BaseInternalWrapper
	{
		#region "Attributes"

		protected new static ServerAssemblyWrapper m_instance;

		private Assembly m_assembly;

		private Type m_dedicatedServerType;

		private MethodInfo m_dedicatedStartup;
		private MethodInfo m_dedicatedStartupBase;

		private Type m_stringLookupType1;
		private Type m_stringLookupType2;
		private Type m_stringLookupType3;
		private Type m_stringLookupType4;
		private Type m_stringLookupType5;

		private MethodInfo m_stringLookupMethod1;
		private MethodInfo m_stringLookupMethod2;
		private MethodInfo m_stringLookupMethod3;
		private MethodInfo m_stringLookupMethod4;
		private MethodInfo m_stringLookupMethod5;

		#endregion

		#region "Constructors and Initializers"

		protected ServerAssemblyWrapper(string path)
			: base(path)
		{
			m_instance = this;

			//string assemblyPath = Path.Combine(path, "SpaceEngineersDedicated.exe");
			m_assembly = Assembly.UnsafeLoadFrom("SpaceEngineersDedicated.exe");

			string mainServerNamespace = "83BCBFA49B3A2A6EC1BC99583DA2D399";
			string stringLookupNamespace = "BD713F20F183AB7899BBC0EB8ADF61B6";

			m_dedicatedServerType = m_assembly.GetType(mainServerNamespace + ".49BCFF86BA276A9C7C0D269C2924DE2D");

			m_dedicatedStartup = m_dedicatedServerType.GetMethod("0D8AAA624C2EEA412F85ABB3AEFAF743", BindingFlags.Static | BindingFlags.NonPublic);
			m_dedicatedStartupBase = m_dedicatedServerType.GetMethod("26A7ABEA729FAE1F24679E21470F8E98", BindingFlags.Static | BindingFlags.NonPublic);


			m_stringLookupType1 = m_assembly.GetType(stringLookupNamespace + ".6E02B994707E60A052A320F2555CA3CE");
			m_stringLookupType2 = m_assembly.GetType(stringLookupNamespace + ".7F0DC7E5BE13D0909DEE3B9AC309419A");
			m_stringLookupType3 = m_assembly.GetType(stringLookupNamespace + ".A35C60B97C5EA5F93A3D6C693756C5AD");
			m_stringLookupType4 = m_assembly.GetType(stringLookupNamespace + ".A7D02B8FCFBFAF4F0C9A2C8C27195680");
			m_stringLookupType5 = m_assembly.GetType(mainServerNamespace + ".AD1EEFD4E986B1869408666EB76B00CF");

			m_stringLookupMethod1 = m_stringLookupType1.GetMethod("8FB14F437CD839C2A1EAC7F24E98B34A", BindingFlags.Static | BindingFlags.NonPublic);
			m_stringLookupMethod2 = m_stringLookupType2.GetMethod("CE231683AC4A8C955232EB6CFB284F55", BindingFlags.Static | BindingFlags.NonPublic);
			m_stringLookupMethod3 = m_stringLookupType3.GetMethod("325DC833204460A8DB1337A0DF319F12", BindingFlags.Static | BindingFlags.NonPublic);
			m_stringLookupMethod4 = m_stringLookupType4.GetMethod("CD9E0AF5742DAE6925FF82E237147C79", BindingFlags.Static | BindingFlags.NonPublic);
			m_stringLookupMethod5 = m_stringLookupType5.GetMethod("3E89EC795A63B176C4AB0733443E79E0", BindingFlags.Static | BindingFlags.NonPublic);

			Console.WriteLine("Finished loading SpaceEngineersDedicated.exe assembly wrapper");
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

		#region "Methods"

		public Type DedicatedServerType
		{
			get { return m_dedicatedServerType; }
		}

		public string GetLookupString1(int key, int start, int length)
		{
			string result = (string)m_stringLookupMethod1.Invoke(null, new object[] { key, start, length });
			return result;
		}

		public string GetLookupString2(int key, int start, int length)
		{
			string result = (string)m_stringLookupMethod2.Invoke(null, new object[] { key, start, length });
			return result;
		}

		public string GetLookupString3(int key, int start, int length)
		{
			string result = (string)m_stringLookupMethod3.Invoke(null, new object[] { key, start, length });
			return result;
		}

		public string GetLookupString4(int key, int start, int length)
		{
			string result = (string)m_stringLookupMethod4.Invoke(null, new object[] { key, start, length });
			return result;
		}

		public string GetLookupString5(int key, int start, int length)
		{
			string result = (string)m_stringLookupMethod5.Invoke(null, new object[] { key, start, length });
			return result;
		}

		public bool StartServer(string worldName)
		{
			try
			{
				SandboxGameAssemblyWrapper.SetNullRender(true);
				//SandboxGameAssemblyWrapper.SetConfigWorld(worldName);

				MyFileSystem.Reset();
				object[] methodParams = new object[]
				{
					"",
					(string) null,
					false,
					true
				};
				m_dedicatedStartupBase.Invoke(null, methodParams);

				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
				return false;
			}
		}

		#endregion
	}
}
