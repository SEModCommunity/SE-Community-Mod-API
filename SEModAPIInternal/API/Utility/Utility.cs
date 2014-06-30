using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Utility
{
	public class UtilityFunctions
	{
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
	}
}
