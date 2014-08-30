using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.API.Entity;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Common
{
	public class GameEntityManager
	{
		#region "Attributes"

		private static Dictionary<long, BaseObject> m_entityMap = new Dictionary<long,BaseObject>();

		public static string GameEntityManagerNamespace = "u0035BCAC68007431E61367F5B2CF24E2D6F";
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

		public static BaseObject GetEntity(long entityId)
		{
			if(!m_entityMap.ContainsKey(entityId))
				return null;

			return m_entityMap[entityId];
		}

		internal static void AddEntity(long entityId, BaseObject entity)
		{
			if (m_entityMap.ContainsKey(entityId))
				return;

			m_entityMap.Add(entityId, entity);
		}

		internal static void RemoveEntity(long entityId)
		{
			if (!m_entityMap.ContainsKey(entityId))
				return;

			m_entityMap.Remove(entityId);
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
