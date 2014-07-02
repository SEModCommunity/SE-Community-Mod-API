using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Utility
{
	public class UtilityFunctions
	{
		#region "Attributes"

		public static string UtilityClass = "5BCAC68007431E61367F5B2CF24E2D6F.226D9974B43A7269CDD3E322CC8110D5";
		public static string UtilityGenerateEntityId = "3B4924802BEBD1AE13B29920376CE914";

		#endregion

		#region "Methods"

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
				LogManager.GameLog.WriteLine(ex);
				return new HashSet<object>();
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
				LogManager.GameLog.WriteLine(ex);
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
				LogManager.GameLog.WriteLine(ex);
				return source;
			}
		}

		public static T CastObject<T>(Object source)
		{
			return (T)source;
		}

		public static long GenerateEntityId()
		{
			try
			{
				Type utilityType = SandboxGameAssemblyWrapper.GetInstance().GameAssembly.GetType(UtilityClass);
				MethodInfo generateIdMethod = utilityType.GetMethod(UtilityGenerateEntityId, BindingFlags.Public | BindingFlags.Static);
				long entityId = (long)generateIdMethod.Invoke(null, new object[] { });

				return entityId;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to generate entity id");
				LogManager.GameLog.WriteLine(ex);
				return 0;
			}
		}

		#endregion
	}
}
