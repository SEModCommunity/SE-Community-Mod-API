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
		private FactionsManager m_factionManager;

		public static string WorldManagerNamespace = "AAC05F537A6F0F6775339593FBDFC564";
		public static string WorldManagerClass = "D580AE7552E79DAB03A3D64B1F7B67F9";
		public static string WorldManagerInstanceField = "AE8262481750DAB9C8D416E4DBB9BA04";
		public static string WorldManagerFactionManagerField = "0A481A0F72FB8D956A8E00BB2563E605";
		public static string WorldManagerGetPlayerManagerMethod = "4C1B66FF099503DCB589BBFFC4976633";

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

		public bool IsAutosaving
		{
			get
			{
				return SaveManager.IsSaving;
			}
		}

		#endregion

		#region "Methods"

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

		#endregion
	}
}
