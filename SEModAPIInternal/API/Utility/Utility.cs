using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Sandbox.Common.ObjectBuilders;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.Support;

using VRage.Common.Utils;
using VRageMath;

namespace SEModAPIInternal.API.Utility
{
	public class UtilityFunctions
	{
		#region "Attributes"

		public static string UtilityNamespace = "5BCAC68007431E61367F5B2CF24E2D6F";
		public static string UtilityClass = "226D9974B43A7269CDD3E322CC8110D5";

		public static string UtilityGenerateEntityIdMethod = "3B4924802BEBD1AE13B29920376CE914";

		#endregion

		#region "Methods"

		public static bool ReflectionUnitTest()
		{
			try
			{
				Type type = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(UtilityNamespace, UtilityClass);
				if (type == null)
					throw new Exception("Could not find internal type for UtilityFunctions");
				bool result = true;
				result &= BaseObject.HasMethod(type, UtilityGenerateEntityIdMethod);

				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return false;
			}
		}

		public static HashSet<Object> ConvertHashSet(Object source)
		{
			try
			{
				Type rawType = source.GetType();
				Type[] genericArgs = rawType.GetGenericArguments();
				MethodInfo conversion = typeof(UtilityFunctions).GetMethod("ConvertEntityHashSet", BindingFlags.Public | BindingFlags.Static);
				conversion = conversion.MakeGenericMethod(genericArgs[0]);
				HashSet<Object> result = (HashSet<Object>)conversion.Invoke(null, new object[] { source });

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new HashSet<object>();
			}
		}

		public static List<Object> ConvertList(Object source)
		{
			try
			{
				Type rawType = source.GetType();
				Type[] genericArgs = rawType.GetGenericArguments();
				MethodInfo conversion = typeof(UtilityFunctions).GetMethod("ConvertEntityList", BindingFlags.Public | BindingFlags.Static);
				conversion = conversion.MakeGenericMethod(genericArgs[0]);
				List<Object> result = (List<Object>)conversion.Invoke(null, new object[] { source });

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new List<object>();
			}
		}

		public static Dictionary<long, Object> ConvertDictionary(Object source)
		{
			try
			{
				Type rawType = source.GetType();
				Type[] genericArgs = rawType.GetGenericArguments();
				MethodInfo conversion = typeof(UtilityFunctions).GetMethod("ConvertEntityDictionary", BindingFlags.Public | BindingFlags.Static);
				conversion = conversion.MakeGenericMethod(genericArgs[1]);
				Dictionary<long, Object> result = (Dictionary<long, Object>)conversion.Invoke(null, new object[] { source });

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return new Dictionary<long, Object>();
			}
		}

		public static HashSet<Object> ConvertEntityHashSet<T>(IEnumerable<T> source)
		{
			HashSet<Object> dataSet = new HashSet<Object>();

			try
			{
				foreach (var rawEntity in source)
				{
					dataSet.Add(rawEntity);
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			return dataSet;
		}

		public static List<Object> ConvertEntityList<T>(IEnumerable<T> source)
		{
			List<Object> dataSet = new List<Object>();

			try
			{
				foreach (var rawEntity in source)
				{
					dataSet.Add(rawEntity);
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			return dataSet;
		}

		public static Dictionary<long, Object> ConvertEntityDictionary<T>(IEnumerable<KeyValuePair<long, T>> source)
		{
			Dictionary<long, Object> dataSet = new Dictionary<long, Object>();

			try
			{
				foreach (var rawEntity in source)
				{
					dataSet.Add(rawEntity.Key, rawEntity.Value);
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}

			return dataSet;
		}

		public static Object ChangeObjectType(Object source, Type newType)
		{
			try
			{
				MethodInfo conversion = typeof(UtilityFunctions).GetMethod("CastObject", BindingFlags.Public | BindingFlags.Static);
				conversion = conversion.MakeGenericMethod(newType);
				Object result = conversion.Invoke(null, new object[] { source });

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return source;
			}
		}

		public static T CastObject<T>(Object source)
		{
			return (T)source;
		}

		public static Object ChangeObjectGeneric(Object source, Type newGenericType)
		{
			try
			{
				Type newType = source.GetType().MakeGenericType(newGenericType);
				Object result = ChangeObjectType(source, newType);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return source;
			}
		}

		public static long GenerateEntityId()
		{
			try
			{
				Type utilityType = SandboxGameAssemblyWrapper.Instance.GetAssemblyType(UtilityNamespace, UtilityClass);
				MethodInfo generateIdMethod = utilityType.GetMethod(UtilityGenerateEntityIdMethod, BindingFlags.Public | BindingFlags.Static);
				long entityId = (long)generateIdMethod.Invoke(null, new object[] { Type.Missing });

				return entityId;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to generate entity id");
				LogManager.ErrorLog.WriteLine(ex);
				return 0;
			}
		}

		public static Vector3 GenerateRandomBorderPosition(Vector3 borderStart, Vector3 borderEnd)
		{
			BoundingBox box = new BoundingBox(borderStart, borderEnd);
			Vector3 result = MyVRageUtils.GetRandomBorderPosition(ref box);

			return result;
		}

		public static List<Type> GetObjectBuilderTypes()
		{
			List<Type> types = new List<Type>();

			Assembly assembly = Assembly.GetAssembly(typeof(MyObjectBuilder_Base));
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(MyObjectBuilder_Base).IsAssignableFrom(type))
					types.Add(type);
			}
			return types;
		}

		public static List<Type> GetCubeBlockTypes()
		{
			List<Type> types = new List<Type>();

			Assembly assembly = Assembly.GetAssembly(typeof(CubeBlockEntity));
			foreach (Type type in assembly.GetTypes())
			{
				if (typeof(CubeBlockEntity).IsAssignableFrom(type))
					types.Add(type);
			}
			return types;
		}

		#endregion
	}
}
