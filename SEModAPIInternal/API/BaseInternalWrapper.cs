using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using VRage;

using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API
{
	public class BaseInternalWrapper
	{
		#region "Attributes"

		protected static BaseInternalWrapper m_instance;

		protected static bool m_isDebugging = false;

		#endregion

		#region "Constructors and Initializers"

		protected BaseInternalWrapper(string basePath)
		{
		}

		public static BaseInternalWrapper GetInstance(string basePath = "")
		{
			if (m_instance == null)
			{
				m_instance = new BaseInternalWrapper(basePath);
			}
			return m_instance;
		}

		#endregion

		#region "Properties"

		public static bool IsDebugging
		{
			get
			{
				GetInstance();
				return m_isDebugging;
			}
			set
			{
				GetInstance();
				m_isDebugging = value;
			}
		}

		#endregion

		#region "Methods"

		protected HashSet<Object> ConvertHashSet(Object source)
		{
			Type rawType = source.GetType();
			Type[] genericArgs = rawType.GetGenericArguments();
			MethodInfo conversion = this.GetType().GetMethod("ConvertEntityHashSet", BindingFlags.NonPublic | BindingFlags.Instance);
			conversion = conversion.MakeGenericMethod(genericArgs[0]);
			HashSet<Object> result = (HashSet<Object>)conversion.Invoke(this, new object[] { source });

			return result;
		}

		protected HashSet<Object> ConvertEntityHashSet<T>(IEnumerable<T> source)
		{
			HashSet<Object> dataSet = new HashSet<Object>();
			foreach (var rawEntity in source)
			{
				dataSet.Add(rawEntity);
			}

			return dataSet;
		}

		protected static FieldInfo GetStaticField(Type objectType, string fieldName)
		{
			try
			{
				FieldInfo field = objectType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (field == null)
					field = objectType.BaseType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				return field;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get static field '" + fieldName + "'");
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected static FieldInfo GetEntityField(Object gameEntity, string fieldName)
		{
			try
			{
				FieldInfo field = gameEntity.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if(field == null)
					field = gameEntity.GetType().BaseType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				return field;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity field '" + fieldName + "'");
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected static MethodInfo GetStaticMethod(Type objectType, string methodName)
		{
			try
			{
				if (methodName == null || methodName.Length == 0)
					throw new Exception("Method name was empty");
				MethodInfo method = objectType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
					method = objectType.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get static method '" + methodName + "'");
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected static MethodInfo GetEntityMethod(Object gameEntity, string methodName)
		{
			try
			{
				if (gameEntity == null)
					throw new Exception("Game entity was null");
				if (methodName == null || methodName.Length == 0)
					throw new Exception("Method name was empty");
				MethodInfo method = gameEntity.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
					method = gameEntity.GetType().BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity method '" + methodName + "'");
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected static Object InvokeStaticMethod(Type objectType, string methodName)
		{
			return InvokeStaticMethod(objectType, methodName, new object[] { });
		}

		protected static Object InvokeStaticMethod(Type objectType, string methodName, Object[] parameters)
		{
			try
			{
				MethodInfo method = GetStaticMethod(objectType, methodName);
				Object result = method.Invoke(null, parameters);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to invoke static method '" + methodName + "'");
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		protected static Object InvokeEntityMethod(Object gameEntity, string methodName)
		{
			return InvokeEntityMethod(gameEntity, methodName, new object[] { });
		}

		protected static Object InvokeEntityMethod(Object gameEntity, string methodName, Object[] parameters)
		{
			try
			{
				MethodInfo method = GetEntityMethod(gameEntity, methodName);
				Object result = method.Invoke(gameEntity, parameters);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to invoke entity method '" + methodName + "'");
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		#endregion
	}
}
