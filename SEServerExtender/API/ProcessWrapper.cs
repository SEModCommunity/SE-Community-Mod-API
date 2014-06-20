using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Sandbox.Common;
using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using Sandbox.AppCode.App;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game.Weapons;
using Sandbox.Game.World;

using SEModAPI.API;
using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

using DedicatedConfigurator;

using VRageMath;
using VRage.Common.Utils;

using B337879D0C82A5F9C44D51D954769590;	//Source - Sandbox.Game.dll
using BD713F20F183AB7899BBC0EB8ADF61B6;	//Source - SpaceEngineersDedicated.exe

namespace SEServerExtender.API
{
	public class ServerAssemblyWrapper
	{
		#region "Attributes"

		private Assembly m_assembly;

		private Type m_dedicatedServerType;

		private MethodInfo m_dedicatedStartup;
		private MethodInfo m_dedicatedStartupBase;

		private Type m_stringLookupType1;
		private Type m_stringLookupType2;
		private Type m_stringLookupType3;
		private Type m_stringLookupType4;
		private MethodInfo m_stringLookupMethod1;
		private MethodInfo m_stringLookupMethod2;
		private MethodInfo m_stringLookupMethod3;
		private MethodInfo m_stringLookupMethod4;

		#endregion

		#region "Constructors and Initializers"

		public ServerAssemblyWrapper(string path)
		{
			//string assemblyPath = Path.Combine(path, "SpaceEngineersDedicated.exe");
			m_assembly = Assembly.UnsafeLoadFrom("SpaceEngineersDedicated.exe");

			m_dedicatedServerType = m_assembly.GetType("83BCBFA49B3A2A6EC1BC99583DA2D399.49BCFF86BA276A9C7C0D269C2924DE2D");
			m_dedicatedStartup = m_dedicatedServerType.GetMethod("0D8AAA624C2EEA412F85ABB3AEFAF743", BindingFlags.Static | BindingFlags.NonPublic);
			m_dedicatedStartupBase = m_dedicatedServerType.GetMethod("26A7ABEA729FAE1F24679E21470F8E98", BindingFlags.Static | BindingFlags.NonPublic);

			string stringLookupNamespace = "BD713F20F183AB7899BBC0EB8ADF61B6";

			m_stringLookupType1 = m_assembly.GetType(stringLookupNamespace + ".6E02B994707E60A052A320F2555CA3CE");
			m_stringLookupType2 = m_assembly.GetType(stringLookupNamespace + ".7F0DC7E5BE13D0909DEE3B9AC309419A");
			m_stringLookupType3 = m_assembly.GetType(stringLookupNamespace + ".A35C60B97C5EA5F93A3D6C693756C5AD");
			m_stringLookupType4 = m_assembly.GetType(stringLookupNamespace + ".A7D02B8FCFBFAF4F0C9A2C8C27195680");

			m_stringLookupMethod1 = m_stringLookupType1.GetMethod("8FB14F437CD839C2A1EAC7F24E98B34A", BindingFlags.Static | BindingFlags.NonPublic);
			m_stringLookupMethod2 = m_stringLookupType2.GetMethod("CE231683AC4A8C955232EB6CFB284F55", BindingFlags.Static | BindingFlags.NonPublic);
			m_stringLookupMethod3 = m_stringLookupType3.GetMethod("325DC833204460A8DB1337A0DF319F12", BindingFlags.Static | BindingFlags.NonPublic);
			m_stringLookupMethod4 = m_stringLookupType4.GetMethod("CD9E0AF5742DAE6925FF82E237147C79", BindingFlags.Static | BindingFlags.NonPublic);

			Console.WriteLine("Finished loading SpaceEngineersDedicated.exe assembly wrapper");
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

		public bool StartServer(string[] parameters)
		{
			try
			{
				object[] methodParams = new object[]
				{
					"SpaceEngineersDedicated",
					(string) null,
					true,
					true
				};
				//m_dedicatedStartupBase.Invoke(null, methodParams);
				m_dedicatedStartup.Invoke(null, new object[] { parameters });

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

	public class SandboxGameAssemblyWrapper
	{
		#region "Attributes"

		private Assembly m_assembly;

		private Type m_mainGameType;
		private Type m_checkpointManagerType;
		private Type m_cubeGridType;
		private Type m_objectManagerType;

		private MethodInfo m_saveCheckpoint;
		private MethodInfo m_getServerSector;
		private MethodInfo m_getServerCheckpoint;
		private MethodInfo m_getBaseEntities;

		private FieldInfo m_mainGameInstanceField;
		private FieldInfo m_configContainerField;
		private FieldInfo m_configContainerDedicatedDataField;

		private Type m_stringLookupType1;

		private MethodInfo m_stringLookupMethod1;

		#endregion

		#region "Constructors and Initializers"

		public SandboxGameAssemblyWrapper(string path)
		{
			//string assemblyPath = Path.Combine(path, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");

			//string cubeGridClass = "5BCAC68007431E61367F5B2CF24E2D6F.98262C3F38A1199E47F2B9338045794C";
			//string conveyorLineManagerClass = "8EAF60352312606996BD8147B0A3C880.68E5FDFBB1457F6347DEBE26175326B0";
			//string characterClass = "F79C930F3AD8FDAF31A59E2702EECE70.3B71F31E6039CAE9D8706B5F32FE468D";

			m_mainGameType = typeof(B3531963E948FB4FA1D057C4340C61B4);
			m_checkpointManagerType = m_assembly.GetType("36CC7CE820B9BBBE4B3FECFEEFE4AE86.828574590CB1B1AE5A5659D4B9659548");
			m_cubeGridType = m_assembly.GetType("5BCAC68007431E61367F5B2CF24E2D6F.98262C3F38A1199E47F2B9338045794C");
			m_objectManagerType = m_assembly.GetType("5BCAC68007431E61367F5B2CF24E2D6F.CAF1EB435F77C7B77580E2E16F988BED");
			//TODO - Update this to get the correct type for 1.035.009
			m_stringLookupType1 = m_assembly.GetType("B337879D0C82A5F9C44D51D954769590.1C03EE9704BF0B992EC4DE5FB699F226");

			Type[] argTypes = new Type[2];
			argTypes[0] = typeof(MyObjectBuilder_Checkpoint);
			argTypes[1] = typeof(string);
			m_saveCheckpoint = m_checkpointManagerType.GetMethod("03AA898C5E9A48425F2CB4FFB2A49A82", argTypes);
			m_getServerSector = m_checkpointManagerType.GetMethod("B6D13C37B0C7FDBF469AB93D18E4444A", BindingFlags.Static | BindingFlags.Public);
			m_getServerCheckpoint = m_checkpointManagerType.GetMethod("825106F82475488A49F8184C627DADEB", BindingFlags.Static | BindingFlags.Public);
			m_getBaseEntities = m_objectManagerType.GetMethod("0A1670B270D5F8417447CFCBA7BF0FA8", BindingFlags.NonPublic | BindingFlags.Static);

			m_mainGameInstanceField = m_mainGameType.GetField("392503BDB6F8C1E34A232489E2A0C6D4", BindingFlags.Static | BindingFlags.Public);
			m_configContainerField = m_mainGameType.GetField("4895ADD02F2C27ED00C63E7E506EE808", BindingFlags.Static | BindingFlags.Public);
			m_configContainerDedicatedDataField = m_configContainerField.FieldType.GetField("44A1510B70FC1BBE3664969D47820439", BindingFlags.NonPublic | BindingFlags.Instance);

			//TODO - Update this to get the correct method for 1.035.009
			if(m_stringLookupType1 != null)
				m_stringLookupMethod1 = m_stringLookupType1.GetMethod("20374FA27AA1D3C028241178455F5A70", BindingFlags.Static | BindingFlags.NonPublic);
			
			Console.WriteLine("Finished loading Sandbox.Game.dll assembly wrapper");
		}

		#endregion

		#region "Properties"

		public Type MainGameType
		{
			get { return m_mainGameType; }
		}

		public Type CheckpointManagerType
		{
			get { return m_checkpointManagerType; }
		}

		public Type CubeGridType
		{
			get { return m_cubeGridType; }
		}

		#endregion

		#region "Methods"

		private string GetLookupString(MethodInfo method, int key, int start, int length)
		{
			string result = (string)method.Invoke(null, new object[] { key, start, length });
			return result;
		}

		public string GetLookupString1(int key, int start, int length)
		{
			return GetLookupString(m_stringLookupMethod1, key, start, length);
		}

		public Object GetServerConfigContainer()
		{
			Object configObject = m_configContainerField.GetValue(null);

			return configObject;
		}

		public MyConfigDedicatedData GetServerConfig()
		{
			Object configContainer = GetServerConfigContainer();
			MyConfigDedicatedData config = (MyConfigDedicatedData)m_configContainerDedicatedDataField.GetValue(configContainer);

			return config;
		}

		public bool SaveCheckpoint(MyObjectBuilder_Checkpoint checkpoint, string worldName)
		{
			return (bool)m_saveCheckpoint.Invoke(null, new object[] { checkpoint, worldName });
		}

		public MyObjectBuilder_Sector GetServerSector(string worldName, Vector3I sectorLocation, out ulong sectorId)
		{
			object[] parameters = new object[] { worldName, sectorLocation, null };
			MyObjectBuilder_Sector result = (MyObjectBuilder_Sector)m_getServerSector.Invoke(null, parameters);
			sectorId = (ulong)parameters[1];

			return result;
		}

		public MyObjectBuilder_Checkpoint GetServerCheckpoint(string worldName, out ulong worldId)
		{
			object[] parameters = new object[] { worldName, null };
			MyObjectBuilder_Checkpoint result = (MyObjectBuilder_Checkpoint)m_getServerCheckpoint.Invoke(null, parameters);
			worldId = (ulong)parameters[1];

			return result;
		}

		public B3531963E948FB4FA1D057C4340C61B4 GetMainGameInstance()
		{
			return (B3531963E948FB4FA1D057C4340C61B4)m_mainGameInstanceField.GetValue(null);
		}

		public List<MyObjectBuilder_EntityBase> GetBaseEntities()
		{
			List<MyObjectBuilder_EntityBase> entities = (List<MyObjectBuilder_EntityBase>)m_getBaseEntities.Invoke(null, new object[] { });

			return entities;
		}

		#endregion
	}

	public class ProcessWrapper
	{
		private ServerAssemblyWrapper m_serverWrapper;
		private SandboxGameAssemblyWrapper m_sandboxGameWrapper;

		private static Thread m_runServerThread;
		private static Thread m_monitorServerThread;

		private static bool m_serverRunning;

		public ProcessWrapper()
		{
		}

		public void StartGame()
		{
			try
			{
				string basePath = Path.Combine(GameInstallationInfo.GamePath, "DedicatedServer64");
				m_serverWrapper = new ServerAssemblyWrapper(basePath);
				m_sandboxGameWrapper = new SandboxGameAssemblyWrapper(basePath);

				m_serverRunning = false;

				string worldName = "DotS - Survival World 1";
				bool result = LoadWorld(worldName);
				if (!result)
				{
					MyConfigDedicatedData config = m_sandboxGameWrapper.GetServerConfig();
					throw new Exception("Failed to load world config from checkpoint");
				}
				else
				{
					MyConfigDedicatedData config = m_sandboxGameWrapper.GetServerConfig();
					Console.WriteLine("Loaded world config from checkpoint");
				}

				m_runServerThread = new Thread(new ThreadStart(this.RunServer));
				m_runServerThread.Start();
				m_monitorServerThread = new Thread(new ThreadStart(this.MonitorServer));
				m_monitorServerThread.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void MonitorServer()
		{
			bool isLoaded = false;
			while (!isLoaded)
			{
				Console.WriteLine("MONITOR - Waiting for server to load ...");

				Thread.Sleep(1000);

				B3531963E948FB4FA1D057C4340C61B4 mainGame = m_sandboxGameWrapper.GetMainGameInstance();
				if (mainGame == null)
					continue;
				MyConfigDedicatedData config = m_sandboxGameWrapper.GetServerConfig();
				if (config == null)
					continue;
				if (config.LoadWorld == null)
					continue;

				isLoaded = true;
			}

			//TODO - Find a way to determine when the server thread is fully loaded
			Thread.Sleep(20000);

			Console.WriteLine("MONITOR - Loaded World: " + m_sandboxGameWrapper.GetServerConfig().LoadWorld);
			Console.WriteLine("MONITOR - Server Name: " + m_sandboxGameWrapper.GetServerConfig().ServerName);
			Console.WriteLine("MONITOR - Entities: " + m_sandboxGameWrapper.GetBaseEntities().Count.ToString());

			m_serverRunning = true;

			while (m_serverRunning && m_runServerThread.ThreadState != System.Threading.ThreadState.Stopped)
			{
				B3531963E948FB4FA1D057C4340C61B4 mainGame = m_sandboxGameWrapper.GetMainGameInstance();
				if (mainGame == null)
				{
					m_serverRunning = false;
					continue;
				}

				MyConfigDedicatedData config = m_sandboxGameWrapper.GetServerConfig();

				List<MyObjectBuilder_EntityBase> entities = m_sandboxGameWrapper.GetBaseEntities();

				foreach (var entity in entities)
				{
					if (entity.GetType() == typeof(MyObjectBuilder_CubeGrid))
					{
						MyObjectBuilder_CubeGrid cubeGridBase = (MyObjectBuilder_CubeGrid)entity;
						SerializableVector3 rawVelocity = cubeGridBase.LinearVelocity;
						double velocity = Math.Sqrt(rawVelocity.X * rawVelocity.X + rawVelocity.Y * rawVelocity.Y + rawVelocity.Z * rawVelocity.Z);
						if (velocity > 1)
						{
							CubeGrid cubeGrid = new CubeGrid(cubeGridBase);
							Console.WriteLine("MONITOR - CubeGrid '" + cubeGrid.Name + "' is moving!");
						}
					}
				}

				Thread.Sleep(1000);
			}

			Console.WriteLine("MONITOR - Server has shut down");
		}

		private void RunServer()
		{
			//Args as of 1.035.009
			string consoleArgs = m_serverWrapper.GetLookupString2(621743686, 82754060, 11514536);
			string[] params0 = consoleArgs.Split(' ');
			m_serverRunning = m_serverWrapper.StartServer(params0);
		}

		private bool LoadWorld(string worldName)
		{
			string contentPath = Path.Combine(new FileInfo(MyFileSystem.ExePath).Directory.FullName, "Content");
			MyFileSystem.Init(contentPath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineersDedicated"));
			MyFileSystem.InitUserSpecific((string)null);

			string contenPathTemp = MyFileSystem.ContentPath;
			string userDataPathTemp = MyFileSystem.UserDataPath;

			ulong id = 0;
			MyObjectBuilder_Checkpoint checkpoint = m_sandboxGameWrapper.GetServerCheckpoint(worldName, out id);

			if (checkpoint != null)
			{
				//TODO - Find a method to copy the checkpoint into the config

				MyFileSystem.Reset();

				return true;
			}

			return false;
		}
	}
}
