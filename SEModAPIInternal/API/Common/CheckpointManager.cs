using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.Support;

using VRage.Common.Utils;
using VRageMath;

namespace SEModAPIInternal.API.Common
{
	public class CheckpointManager
	{
		#region "Attributes"

		private static CheckpointManager m_instance;

		private Type m_checkpointManagerType;
		private MethodInfo m_saveCheckpoint;
		private MethodInfo m_getServerSector;
		private MethodInfo m_getServerCheckpoint;

		public static string CheckpointManagerClass = "36CC7CE820B9BBBE4B3FECFEEFE4AE86.828574590CB1B1AE5A5659D4B9659548";
		public static string CheckpointManagerSaveCheckpointMethod = "03AA898C5E9A48425F2CB4FFB2A49A82";
		public static string CheckpointManagerGetServerSectorMethod = "B6D13C37B0C7FDBF469AB93D18E4444A";
		public static string CheckpointManagerGetServerCheckpointMethod = "825106F82475488A49F8184C627DADEB";

		#endregion

		#region "Constructors and Initializers"

		protected CheckpointManager()
		{
			try
			{
				Assembly assembly = SandboxGameAssemblyWrapper.Instance.GameAssembly;

				//Do our sanity checks to make sure all of the types, methods, and fields exist
				m_checkpointManagerType = assembly.GetType(CheckpointManagerClass);
				Type[] argTypes = new Type[2];
				argTypes[0] = typeof(MyObjectBuilder_Checkpoint);
				argTypes[1] = typeof(string);
				m_saveCheckpoint = m_checkpointManagerType.GetMethod(CheckpointManagerSaveCheckpointMethod, argTypes);
				m_getServerSector = m_checkpointManagerType.GetMethod(CheckpointManagerGetServerSectorMethod, BindingFlags.Static | BindingFlags.Public);
				m_getServerCheckpoint = m_checkpointManagerType.GetMethod(CheckpointManagerGetServerCheckpointMethod, BindingFlags.Static | BindingFlags.Public);

				m_instance = this;

				Console.WriteLine("Finished loading CheckpointManager");
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion

		#region "Properties"

		public static CheckpointManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new CheckpointManager();

				return m_instance;
			}
		}

		public Type CheckpointManagerType
		{
			get { return m_checkpointManagerType; }
		}

		#endregion

		#region "Methods

		public bool SaveCheckpoint(MyObjectBuilder_Checkpoint checkpoint, string worldName)
		{
			try
			{
				return (bool)m_saveCheckpoint.Invoke(null, new object[] { checkpoint, worldName });
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return false;
			}
		}

		public MyObjectBuilder_Sector GetServerSector(string worldName, Vector3I sectorLocation, out ulong sectorId)
		{
			try
			{
				object[] parameters = new object[] { worldName, sectorLocation, null };
				MyObjectBuilder_Sector result = (MyObjectBuilder_Sector)m_getServerSector.Invoke(null, parameters);
				sectorId = (ulong)parameters[1];

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				sectorId = 0;
				return null;
			}
		}

		public MyObjectBuilder_Checkpoint GetServerCheckpoint(string worldName, out ulong worldId)
		{
			try
			{
				object[] parameters = new object[] { worldName, null };
				MyObjectBuilder_Checkpoint result = (MyObjectBuilder_Checkpoint)m_getServerCheckpoint.Invoke(null, parameters);
				worldId = (ulong)parameters[1];

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				worldId = 0;
				return null;
			}
		}

		#endregion
	}
}
