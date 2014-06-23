using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Text;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using VRage;
using VRageMath;

namespace SEModAPI.API.Internal
{
	public class SandboxGameAssemblyWrapper
	{
		#region "Attributes"

		private Assembly m_assembly;

		private Type m_mainGameType;
		private Type m_checkpointManagerType;
		private Type m_cubeGridType;
		private Type m_characterType;
		private Type m_serverCoreType;
		private static Type m_configContainerType;
		private Type m_stringLookupType1;

		private MethodInfo m_saveCheckpoint;
		private MethodInfo m_getServerSector;
		private MethodInfo m_getServerCheckpoint;
		private static MethodInfo m_setConfigWorldName;
		private MethodInfo m_stringLookupMethod1;

		private FieldInfo m_mainGameInstanceField;
		private static FieldInfo m_configContainerField;
		private static FieldInfo m_configContainerDedicatedDataField;
		private static FieldInfo m_serverCoreNullRender;

		#endregion

		#region "Constructors and Initializers"

		public SandboxGameAssemblyWrapper(string path)
		{
			//string assemblyPath = Path.Combine(path, "Sandbox.Game.dll");
			m_assembly = Assembly.UnsafeLoadFrom("Sandbox.Game.dll");
			ResourceManager resourceManager = new ResourceManager("Resources.Strings", m_assembly);

			//string conveyorLineManagerClass = "8EAF60352312606996BD8147B0A3C880.68E5FDFBB1457F6347DEBE26175326B0";
			//string MySandboxGame_ExitMethod = "246E732EE67F7F6F88C4FF63B3901107";
			//string MySandboxGame_Initialize = "2AA66FBD3F2C5EC250558B3136F3974A";

			m_mainGameType = m_assembly.GetType("B337879D0C82A5F9C44D51D954769590.B3531963E948FB4FA1D057C4340C61B4");
			m_checkpointManagerType = m_assembly.GetType("36CC7CE820B9BBBE4B3FECFEEFE4AE86.828574590CB1B1AE5A5659D4B9659548");
			m_cubeGridType = m_assembly.GetType("5BCAC68007431E61367F5B2CF24E2D6F.98262C3F38A1199E47F2B9338045794C");
			m_characterType = m_assembly.GetType("F79C930F3AD8FDAF31A59E2702EECE70.3B71F31E6039CAE9D8706B5F32FE468D");
			m_serverCoreType = m_assembly.GetType("168638249D29224100DB50BB468E7C07.7BAD4AFD06B91BCD63EA57F7C0D4F408");
			m_stringLookupType1 = m_assembly.GetType("B337879D0C82A5F9C44D51D954769590.2F60967103E6024E563836A2572899F1");

			Type[] argTypes = new Type[2];
			argTypes[0] = typeof(MyObjectBuilder_Checkpoint);
			argTypes[1] = typeof(string);
			m_saveCheckpoint = m_checkpointManagerType.GetMethod("03AA898C5E9A48425F2CB4FFB2A49A82", argTypes);
			m_getServerSector = m_checkpointManagerType.GetMethod("B6D13C37B0C7FDBF469AB93D18E4444A", BindingFlags.Static | BindingFlags.Public);
			m_getServerCheckpoint = m_checkpointManagerType.GetMethod("825106F82475488A49F8184C627DADEB", BindingFlags.Static | BindingFlags.Public);
			m_stringLookupMethod1 = m_stringLookupType1.GetMethod("D893F76B8F00FC565CF848A64C4B6F97", BindingFlags.Static | BindingFlags.NonPublic);

			m_mainGameInstanceField = m_mainGameType.GetField("392503BDB6F8C1E34A232489E2A0C6D4", BindingFlags.Static | BindingFlags.Public);
			m_configContainerField = m_mainGameType.GetField("4895ADD02F2C27ED00C63E7E506EE808", BindingFlags.Static | BindingFlags.Public);
			m_configContainerType = m_configContainerField.FieldType;
			m_configContainerDedicatedDataField = m_configContainerType.GetField("44A1510B70FC1BBE3664969D47820439", BindingFlags.NonPublic | BindingFlags.Instance);
			m_serverCoreNullRender = m_serverCoreType.GetField("53A34747D8E8EDA65E601C194BECE141", BindingFlags.Public | BindingFlags.Static);

			m_setConfigWorldName = m_configContainerType.GetMethod("493E0E7BC7A617699C44A9A5FB8FF679", BindingFlags.Public | BindingFlags.Instance);

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

		public Type CharacterType
		{
			get { return m_characterType; }
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

		public static Object GetServerConfigContainer()
		{
			Object configObject = m_configContainerField.GetValue(null);

			return configObject;
		}

		public static MyConfigDedicatedData GetServerConfig()
		{
			Object configContainer = GetServerConfigContainer();
			MyConfigDedicatedData config = (MyConfigDedicatedData)m_configContainerDedicatedDataField.GetValue(configContainer);
			if (config == null)
			{
				MethodInfo loadConfigDataMethod = m_configContainerField.FieldType.GetMethod("4DD64FD1D45E514D01C925D07B69B3BE", BindingFlags.Public | BindingFlags.Instance);
				loadConfigDataMethod.Invoke(configContainer, new object[] { });
				config = (MyConfigDedicatedData)m_configContainerDedicatedDataField.GetValue(configContainer);
			}

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

		public B337879D0C82A5F9C44D51D954769590.B3531963E948FB4FA1D057C4340C61B4 GetMainGameInstance()
		{
			return (B337879D0C82A5F9C44D51D954769590.B3531963E948FB4FA1D057C4340C61B4)m_mainGameInstanceField.GetValue(null);
		}

		public static void SetNullRender(bool nullRender)
		{
			m_serverCoreNullRender.SetValue(null, nullRender);
		}

		public static void SetConfigWorld(string worldName)
		{
			MyConfigDedicatedData config = GetServerConfig();

			m_setConfigWorldName.Invoke(GetServerConfigContainer(), new object[] { worldName });
		}

		#endregion
	}
}
