using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

using VRage;

namespace SEModAPIInternal.API.Common
{
	public class GameEntityManager
	{
		#region "Attributes"

		private static FastResourceLock m_resourceLock = new FastResourceLock();
		private static Dictionary<long, BaseObject> m_entityMap = new Dictionary<long,BaseObject>();

		public static string GameEntityManagerNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string GameEntityManagerClass = "CAF1EB435F77C7B77580E2E16F988BED";

		public static string GameEntityManagerGetEntityByIdTypeMethod = "EB43CD3B683033145620D0931BE5041C";

		#endregion

		#region "Properties"

		public static Type InternalType
		{
			get
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(GameEntityManagerNamespace, GameEntityManagerClass);
				return type;
			}
		}

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				bool result = true;

				Type type = InternalType;
				if (type == null)
					throw new Exception("Could not find internal type for GameEntityManager");
				//result &= BaseObject.HasMethod(type, GameEntityManagerGetEntityByIdTypeMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public static BaseObject GetEntity(long entityId)
		{
			//m_resourceLock.AcquireShared();

			if(!m_entityMap.ContainsKey(entityId))
				return null;

			BaseObject result = m_entityMap[entityId];

			//m_resourceLock.ReleaseShared();

			return result;
		}

		internal static void AddEntity(long entityId, BaseObject entity)
		{
			//m_resourceLock.AcquireExclusive();

			if (m_entityMap.ContainsKey(entityId))
				return;

			m_entityMap.Add(entityId, entity);

			//m_resourceLock.ReleaseExclusive();
		}

		internal static void RemoveEntity(long entityId)
		{
			//m_resourceLock.AcquireExclusive();

			if (!m_entityMap.ContainsKey(entityId))
				return;

			m_entityMap.Remove(entityId);

			//m_resourceLock.ReleaseExclusive();
		}

		internal static Object GetGameEntity(long entityId, Type entityType)
		{
			try
			{
				MethodInfo method = InternalType.GetMethod(GameEntityManagerGetEntityByIdTypeMethod, BindingFlags.Public | BindingFlags.Static);
				method = method.MakeGenericMethod(entityType);
				object[] parameters = new object[] { entityId, null };
				object result = method.Invoke(null, parameters);
				bool blResult = (bool)result;
				if (blResult)
				{
					return parameters[1];
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		#endregion
	}
}
