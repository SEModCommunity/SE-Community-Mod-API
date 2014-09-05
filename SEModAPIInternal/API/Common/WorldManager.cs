using System;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

using VRage;

namespace SEModAPIInternal.API.Common
{
	public class WorldManager
	{
		#region "Attributes"

		private static WorldManager m_instance;
		private bool m_isSaving = false;

		public static string WorldManagerNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string WorldManagerClass = "D580AE7552E79DAB03A3D64B1F7B67F9";

		public static string WorldManagerGetPlayerManagerMethod = "4C1B66FF099503DCB589BBFFC4976633";
		public static string WorldManagerSaveWorldMethod = "50092B623574C842837BD09CE21A96D6";
		public static string WorldManagerGetCheckpointMethod = "6CA03E6E730B7881842157B90C864031";
		public static string WorldManagerGetSectorMethod = "B2DFAD1262F75849DA03F64C5E3535B7";
		public static string WorldManagerGetSessionNameMethod = "193678BC97A6081A8AA344BF44620BC5";

		public static string WorldManagerInstanceField = "AE8262481750DAB9C8D416E4DBB9BA04";
		public static string WorldManagerFactionManagerField = "0A481A0F72FB8D956A8E00BB2563E605";
		public static string WorldManagerSessionSettingsField = "3D4D3F0E4E3582FF30FD014D9BB1E504";

		////////////////////////////////////////////////////////////////////

		public static string WorldResourceManagerNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string WorldResourceManagerClass = "15B6B94DB5BE105E7B58A34D4DC11412";

		public static string WorldResourceManagerResourceLockField = "5378A366A1927C9686ABCFD6316F5AE6";

		#endregion

		#region "Constructors and Initializers"

		protected WorldManager()
		{
			m_instance = this;

			Console.WriteLine("Finished loading WorldManager");
		}

		#endregion

		#region "Properties"

		public static WorldManager Instance
		{
			get
			{
				if (m_instance == null)
					m_instance = new WorldManager();

				return m_instance;
			}
		}

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(WorldManagerNamespace, WorldManagerClass);
				return type;
			}
		}

		public Object BackingObject
		{
			get
			{
				try
				{
					Object worldManager = BaseObject.GetStaticFieldValue(InternalType, WorldManagerInstanceField);

					return worldManager;
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
					return null;
				}
			}
		}

		public string Name
		{
			get
			{
				string name = (string)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerGetSessionNameMethod);

				return name;
			}
		}

		public bool IsWorldSaving
		{
			get
			{
				return m_isSaving;
			}
		}

		public MySessionSettings SessionSettings
		{
			get
			{
				try
				{
					MySessionSettings sessionSettings = (MySessionSettings)BaseObject.GetEntityFieldValue(BackingObject, WorldManagerSessionSettingsField);

					return sessionSettings;
				}
				catch (Exception ex)
				{
					LogManager.ErrorLog.WriteLine(ex);
					return new MySessionSettings();
				}
			}
		}

		public MyObjectBuilder_Checkpoint Checkpoint
		{
			get
			{
				MyObjectBuilder_Checkpoint checkpoint = (MyObjectBuilder_Checkpoint)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerGetCheckpointMethod, new object[] { Name });

				return checkpoint;
			}
		}

		public MyObjectBuilder_Sector Sector
		{
			get
			{
				MyObjectBuilder_Sector sector = (MyObjectBuilder_Sector)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerGetSectorMethod);

				return sector;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type1 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(WorldManagerNamespace, WorldManagerClass);
				if (type1 == null)
					throw new Exception("Could not find internal type for WorldManager");
				bool result = true;
				result &= BaseObject.HasMethod(type1, WorldManagerGetPlayerManagerMethod);
				Type[] argTypes = new Type[1];
				argTypes[0] = typeof(string);
				result &= BaseObject.HasMethod(type1, WorldManagerSaveWorldMethod, argTypes);
				result &= BaseObject.HasMethod(type1, WorldManagerGetCheckpointMethod);
				result &= BaseObject.HasMethod(type1, WorldManagerGetSectorMethod);
				result &= BaseObject.HasMethod(type1, WorldManagerGetSessionNameMethod);
				result &= BaseObject.HasField(type1, WorldManagerInstanceField);
				result &= BaseObject.HasField(type1, WorldManagerFactionManagerField);
				result &= BaseObject.HasField(type1, WorldManagerSessionSettingsField);

				Type type2 = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(WorldResourceManagerNamespace, WorldResourceManagerClass);
				if (type2 == null)
					throw new Exception("Could not find world resource manager type for WorldManager");
				result &= BaseObject.HasField(type2, WorldResourceManagerResourceLockField);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public void SaveWorld()
		{
			Action action = InternalSaveWorld;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		internal Object InternalGetFactionManager()
		{
			try
			{
				Object worldManager = BaseObject.GetEntityFieldValue(BackingObject, WorldManagerFactionManagerField);

				return worldManager;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal Object InternalGetPlayerManager()
		{
			Object playerManager = BaseObject.InvokeEntityMethod(BackingObject, WorldManagerGetPlayerManagerMethod);

			return playerManager;
		}

		internal FastResourceLock InternalGetResourceLock()
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(WorldResourceManagerNamespace, WorldResourceManagerClass);
				FastResourceLock result = (FastResourceLock)BaseObject.GetStaticFieldValue(type, WorldResourceManagerResourceLockField);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal void InternalSaveWorld()
		{
			try
			{
				DateTime saveStartTime = DateTime.Now;

				Type type = BackingObject.GetType();
				Type[] argTypes = new Type[1];
				argTypes[0] = typeof(string);
				bool result = (bool)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerSaveWorldMethod, new object[] { null }, argTypes);

				if (result)
				{
					TimeSpan timeToSave = DateTime.Now - saveStartTime;
					LogManager.APILog.WriteLineAndConsole("Save complete and took " + timeToSave.TotalSeconds + " seconds");
					m_isSaving = false;

					EntityEventManager.EntityEvent newEvent = new EntityEventManager.EntityEvent();
					newEvent.type = EntityEventManager.EntityEventType.OnSectorSaved;
					newEvent.timestamp = DateTime.Now;
					newEvent.entity = null;
					newEvent.priority = 0;
					EntityEventManager.Instance.AddEvent(newEvent);
				}
				else
				{
					LogManager.APILog.WriteLineAndConsole("Save failed!");
					m_isSaving = false;
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
