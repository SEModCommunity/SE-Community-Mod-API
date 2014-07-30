using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

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
		private FactionsManager m_factionManager;

		public static string WorldManagerNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string WorldManagerClass = "D580AE7552E79DAB03A3D64B1F7B67F9";

		public static string WorldManagerGetPlayerManagerMethod = "4C1B66FF099503DCB589BBFFC4976633";
		public static string WorldManagerSaveWorldMethod = "50092B623574C842837BD09CE21A96D6";
		public static string WorldManagerGetCheckpoint = "6CA03E6E730B7881842157B90C864031";
		public static string WorldManagerGetSector = "B2DFAD1262F75849DA03F64C5E3535B7";
		public static string WorldManagerGetSessionName = "193678BC97A6081A8AA344BF44620BC5";

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
					FieldInfo field = BaseObject.GetStaticField(InternalType, WorldManagerInstanceField);
					Object worldManager = field.GetValue(null);

					return worldManager;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return null;
				}
			}
		}

		public string Name
		{
			get
			{
				string name = (string)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerGetSessionName);

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
					FieldInfo field = BaseObject.GetEntityField(BackingObject, WorldManagerSessionSettingsField);
					MySessionSettings sessionSettings = (MySessionSettings)field.GetValue(BackingObject);

					return sessionSettings;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return new MySessionSettings();
				}
			}
		}

		public MyObjectBuilder_Checkpoint Checkpoint
		{
			get
			{
				MyObjectBuilder_Checkpoint checkpoint = (MyObjectBuilder_Checkpoint)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerGetCheckpoint, new object[] { Name });

				return checkpoint;
			}
		}

		public MyObjectBuilder_Sector Sector
		{
			get
			{
				MyObjectBuilder_Sector sector = (MyObjectBuilder_Sector)BaseObject.InvokeEntityMethod(BackingObject, WorldManagerGetSector);

				return sector;
			}
		}

		#endregion

		#region "Methods"

		public void SaveWorld()
		{
			Action action = InternalSaveWorld;
			SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
		}

		internal Object InternalGetFactionManager()
		{
			try
			{
				FieldInfo field = BaseObject.GetEntityField(BackingObject, WorldManagerFactionManagerField);
				Object worldManager = field.GetValue(BackingObject);

				return worldManager;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
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
				FieldInfo field = BaseObject.GetStaticField(type, WorldResourceManagerResourceLockField);
				FastResourceLock result = (FastResourceLock)field.GetValue(null);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
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
				MethodInfo method = type.GetMethod(WorldManagerSaveWorldMethod, argTypes);
				bool result = (bool)method.Invoke(BackingObject, new object[] { null });

				if (result)
				{
					TimeSpan timeToSave = DateTime.Now - saveStartTime;
					LogManager.GameLog.WriteLineAndConsole("Save complete and took " + timeToSave.TotalSeconds + " seconds");
					m_isSaving = false;
				}
				else
				{
					LogManager.GameLog.WriteLineAndConsole("Save failed!");
					m_isSaving = false;
				}
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		#endregion
	}
}
