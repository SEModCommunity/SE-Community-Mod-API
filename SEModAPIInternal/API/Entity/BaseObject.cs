﻿using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Xml;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

using VRage;

namespace SEModAPIInternal.API.Entity
{
	[DataContract(Name="BaseObjectProxy")]
	[KnownType(typeof(BaseEntity))]
	[KnownType(typeof(CharacterEntity))]
	[KnownType(typeof(CubeGridEntity))]
	[KnownType(typeof(FloatingObject))]
	[KnownType(typeof(Meteor))]
	[KnownType(typeof(VoxelMap))]
	public class BaseObject : IDisposable
	{
		#region "Attributes"

		protected MyObjectBuilder_Base m_objectBuilder;
		protected Object m_backingObject;

		protected bool m_isDisposed = false;

		#endregion

		#region "Constructors and Initializers"

		public BaseObject(MyObjectBuilder_Base baseEntity)
		{
			m_objectBuilder = baseEntity;
		}

		public BaseObject(MyObjectBuilder_Base baseEntity, Object backingObject)
		{
			m_objectBuilder = baseEntity;
			m_backingObject = backingObject;
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Changed status of the object
		/// </summary>
		[IgnoreDataMember]
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Represent the state of changes in the object")]
		public virtual bool Changed { get; protected set; }

		/// <summary>
		/// API formated name of the object
		/// </summary>
		[DataMember]
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The formatted name of the object")]
		public virtual string Name { get; private set; }

		/// <summary>
		/// Object builder data of the object
		/// </summary>
		[IgnoreDataMember]
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Object builder data of the object")]
		internal MyObjectBuilder_Base ObjectBuilder
		{
			get { return m_objectBuilder; }
			set
			{
				if (m_objectBuilder == value) return;
				m_objectBuilder = value;

				Changed = true;
			}
		}

		/// <summary>
		/// Internal, in-game object that matches to this object
		/// </summary>
		[IgnoreDataMember]
		[Category("Object")]
		[Browsable(false)]
		[Description("Internal, in-game object that matches to this object")]
		public Object BackingObject
		{
			get { return m_backingObject; }
			set
			{
				m_backingObject = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Full type of the object
		/// </summary>
		[IgnoreDataMember]
		[Category("Object")]
		[Browsable(true)]
		[ReadOnly(true)]
		[TypeConverter(typeof(ObjectSerializableDefinitionIdTypeConverter))]
		public SerializableDefinitionId Id
		{
			get
			{
				SerializableDefinitionId newId = new SerializableDefinitionId(m_objectBuilder.TypeId, m_objectBuilder.SubtypeName);
				return newId;
			}
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The value ID representing the type of the object")]
		public MyObjectBuilderType TypeId
		{
			get { return m_objectBuilder.TypeId; }
			private set
			{
				//Do nothing!
			}
		}

		[IgnoreDataMember]
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The value ID representing the sub-type of the object")]
		public string Subtype
		{
			get { return m_objectBuilder.SubtypeName; }
			private set
			{
				//Do nothing!
			}
		}

		[DataMember]
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsDisposed
		{
			get { return m_isDisposed; }
			private set
			{
				//Do nothing!
			}
		}

		#endregion

		#region "Methods"

		public virtual void Dispose()
		{
			if (IsDisposed)
				return;

			if (BackingObject != null)
			{
				//Do stuff
			}

			m_isDisposed = true;
		}

		public virtual void Export(FileInfo fileInfo)
		{
			BaseObjectManager.SaveContentFile<MyObjectBuilder_Base, MyObjectBuilder_BaseSerializer>(ObjectBuilder, fileInfo);
		}

		public static bool ReflectionUnitTest()
		{
			return true;
		}

		#region "Internal"

		public static bool HasField(Type objectType, string fieldName)
		{
			try
			{
				if (fieldName == null || fieldName.Length == 0)
					return false;
				FieldInfo field = objectType.GetField(fieldName);
				if (field == null)
					field = objectType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (field == null)
					field = objectType.BaseType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (field == null)
				{
					if(SandboxGameAssemblyWrapper.IsDebugging)
						LogManager.ErrorLog.WriteLineAndConsole("Failed to find field '" + fieldName + "' in type '" + objectType.FullName + "'");
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLineAndConsole("Failed to find field '" + fieldName + "' in type '" + objectType.FullName + "': " + ex.Message);
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		public static bool HasMethod(Type objectType, string methodName)
		{
			return HasMethod(objectType, methodName, null);
		}

		public static bool HasMethod(Type objectType, string methodName, Type[] argTypes)
		{
			try
			{
				if (methodName == null || methodName.Length == 0)
					return false;

				if (argTypes == null)
				{
					MethodInfo method = objectType.GetMethod(methodName);
					if (method == null)
						method = objectType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
					if (method == null && objectType.BaseType != null)
						method = objectType.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
					if (method == null)
					{
						if (SandboxGameAssemblyWrapper.IsDebugging)
							LogManager.ErrorLog.WriteLineAndConsole("Failed to find method '" + methodName + "' in type '" + objectType.FullName + "'");
						return false;
					}
				}
				else
				{
					MethodInfo method = objectType.GetMethod(methodName, argTypes);
					if (method == null)
						method = objectType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null);
					if (method == null && objectType.BaseType != null)
						method = objectType.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null);
					if (method == null)
					{
						if (SandboxGameAssemblyWrapper.IsDebugging)
							LogManager.ErrorLog.WriteLineAndConsole("Failed to find method '" + methodName + "' in type '" + objectType.FullName + "'");
						return false;
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLineAndConsole("Failed to find method '" + methodName + "' in type '" + objectType.FullName + "': " + ex.Message);
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		public static bool HasProperty(Type objectType, string propertyName)
		{
			try
			{
				if (propertyName == null || propertyName.Length == 0)
					return false;
				PropertyInfo property = objectType.GetProperty(propertyName);
				if (property == null)
					property = objectType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (property == null)
					property = objectType.BaseType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (property == null)
				{
					if (SandboxGameAssemblyWrapper.IsDebugging)
						LogManager.ErrorLog.WriteLineAndConsole("Failed to find property '" + propertyName + "' in type '" + objectType.FullName + "'");
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLineAndConsole("Failed to find property '" + propertyName + "' in type '" + objectType.FullName + "': " + ex.Message);
				LogManager.ErrorLog.WriteLine(ex);
				return false;
			}
		}

		internal static FieldInfo GetStaticField(Type objectType, string fieldName)
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
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static FieldInfo GetEntityField(Object gameEntity, string fieldName)
		{
			try
			{
				FieldInfo field = gameEntity.GetType().GetField(fieldName);
				if (field == null)
				{
					//Recurse up through the class heirarchy to try to find the field
					Type type = gameEntity.GetType();
					while (type != typeof(Object))
					{
						field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
						if (field != null)
							break;

						type = type.BaseType;
					}
				}
				return field;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity field '" + fieldName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static MethodInfo GetStaticMethod(Type objectType, string methodName)
		{
			try
			{
				if (methodName == null || methodName.Length == 0)
					throw new Exception("Method name was empty");
				MethodInfo method = objectType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
				{
					//Recurse up through the class heirarchy to try to find the method
					Type type = objectType;
					while (type != typeof(Object))
					{
						method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
						if (method != null)
							break;

						type = type.BaseType;
					}
				}
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get static method '" + methodName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static MethodInfo GetStaticMethod(Type objectType, string methodName, Type[] argTypes)
		{
			try
			{
				if (argTypes == null || argTypes.Length == 0)
					return GetStaticMethod(objectType, methodName);

				if (methodName == null || methodName.Length == 0)
					throw new Exception("Method name was empty");
				MethodInfo method = objectType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null);
				if (method == null)
				{
					//Recurse up through the class heirarchy to try to find the method
					Type type = objectType;
					while (type != typeof(Object))
					{
						method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null);
						if (method != null)
							break;

						type = type.BaseType;
					}
				}
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get static method '" + methodName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static MethodInfo GetEntityMethod(Object gameEntity, string methodName)
		{
			try
			{
				if (gameEntity == null)
					throw new Exception("Game entity was null");
				if (methodName == null || methodName.Length == 0)
					throw new Exception("Method name was empty");
				MethodInfo method = gameEntity.GetType().GetMethod(methodName);
				if (method == null)
				{
					//Recurse up through the class heirarchy to try to find the method
					Type type = gameEntity.GetType();
					while (type != typeof(Object))
					{
						method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
						if (method != null)
							break;

						type = type.BaseType;
					}
				}
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity method '" + methodName + "': " + ex.Message);
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static MethodInfo GetEntityMethod(Object gameEntity, string methodName, Type[] argTypes)
		{
			try
			{
				if (argTypes == null || argTypes.Length == 0)
					return GetEntityMethod(gameEntity, methodName);

				if (gameEntity == null)
					throw new Exception("Game entity was null");
				if (methodName == null || methodName.Length == 0)
					throw new Exception("Method name was empty");
				MethodInfo method = gameEntity.GetType().GetMethod(methodName, argTypes);
				if (method == null)
				{
					//Recurse up through the class heirarchy to try to find the method
					Type type = gameEntity.GetType();
					while (type != typeof(Object))
					{
						method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, Type.DefaultBinder, argTypes, null);
						if (method != null)
							break;

						type = type.BaseType;
					}
				}
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity method '" + methodName + "': " + ex.Message);
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static Object GetStaticFieldValue(Type objectType, string fieldName)
		{
			try
			{
				FieldInfo field = GetStaticField(objectType, fieldName);
				if (field == null)
					return null;
				Object value = field.GetValue(null);
				return value;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static void SetStaticFieldValue(Type objectType, string fieldName, Object value)
		{
			try
			{
				FieldInfo field = GetStaticField(objectType, fieldName);
				if (field == null)
					return;
				field.SetValue(null, value);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		internal static Object GetEntityFieldValue(Object gameEntity, string fieldName)
		{
			try
			{
				FieldInfo field = GetEntityField(gameEntity, fieldName);
				if (field == null)
					return null;
				Object value = field.GetValue(gameEntity);
				return value;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static void SetEntityFieldValue(Object gameEntity, string fieldName, Object value)
		{
			try
			{
				FieldInfo field = GetEntityField(gameEntity, fieldName);
				if (field == null)
					return;
				field.SetValue(gameEntity, value);
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		internal static Object InvokeStaticMethod(Type objectType, string methodName)
		{
			return InvokeStaticMethod(objectType, methodName, new object[] { });
		}

		internal static Object InvokeStaticMethod(Type objectType, string methodName, Object[] parameters)
		{
			try
			{
				MethodInfo method = GetStaticMethod(objectType, methodName);
				if (method == null)
					throw new Exception("Method is empty");
				Object result = method.Invoke(null, parameters);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to invoke static method '" + methodName + "': " + ex.Message);
				if(SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static Object InvokeEntityMethod(Object gameEntity, string methodName)
		{
			return InvokeEntityMethod(gameEntity, methodName, new object[] { });
		}

		internal static Object InvokeEntityMethod(Object gameEntity, string methodName, Object[] parameters)
		{
			return InvokeEntityMethod(gameEntity, methodName, parameters, null);
		}

		internal static Object InvokeEntityMethod(Object gameEntity, string methodName, Object[] parameters, Type[] argTypes)
		{
			try
			{
				MethodInfo method = GetEntityMethod(gameEntity, methodName, argTypes);
				if (method == null)
					throw new Exception("Method is empty");
				Object result = method.Invoke(gameEntity, parameters);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to invoke entity method '" + methodName + "' on type '" + gameEntity.GetType().FullName + "': " + ex.Message);

				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);

				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static PropertyInfo GetEntityProperty(Object gameEntity, string propertyName)
		{
			try
			{
				PropertyInfo property = gameEntity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (property == null)
					property = gameEntity.GetType().BaseType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

				return property;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity property '" + propertyName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static Object GetEntityPropertyValue(Object gameEntity, string propertyName)
		{
			try
			{
				PropertyInfo property = GetEntityProperty(gameEntity, propertyName);
				if (property == null)
					return null;

				Object result = property.GetValue(gameEntity, null);
				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity property value '" + propertyName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return null;
			}
		}

		internal static void SetEntityPropertyValue(Object gameEntity, string propertyName, Object value)
		{
			try
			{
				PropertyInfo property = GetEntityProperty(gameEntity, propertyName);
				if (property == null)
					return;

				property.SetValue(gameEntity, value, null);
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to set entity property value '" + propertyName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.ErrorLog.WriteLine(Environment.StackTrace);
				LogManager.ErrorLog.WriteLine(ex);
				return;
			}
		}

		#endregion

		#endregion
	}

	[DataContract]
	[KnownType(typeof(SectorObjectManager))]
	[KnownType(typeof(InventoryItemManager))]
	[KnownType(typeof(CubeBlockManager))]
	public class BaseObjectManager
	{
		public enum InternalBackingType
		{
			Hashset,
			List,
			Dictionary,
		}

		#region "Attributes"

		private FileInfo m_fileInfo;
		private readonly FieldInfo m_definitionsContainerField;
		private Object m_backingObject;
		private string m_backingSourceMethod;
		private InternalBackingType m_backingSourceType;
		private DateTime m_lastLoadTime;
		private double m_refreshInterval;

		private static double m_averageRefreshDataTime;
		private static double m_averageRefreshInternalDataTime;
		private static double m_averageRefreshInternalObjectBuilderDataTime;
		private static DateTime m_lastProfilingOutput;
		private static DateTime m_lastInternalProfilingOutput;

		private static int m_staticRefreshCount;
		private static Dictionary<Type, int> m_staticRefreshCountMap;

		protected FastResourceLock m_resourceLock = new FastResourceLock();
		protected FastResourceLock m_rawDataHashSetResourceLock = new FastResourceLock();
		protected FastResourceLock m_rawDataListResourceLock = new FastResourceLock();
		protected FastResourceLock m_rawDataObjectBuilderListResourceLock = new FastResourceLock();

		//Flags
		private bool m_isMutable = true;
		private bool m_changed = false;
		private bool m_isDynamic = false;

		//Raw data stores
		protected HashSet<Object> m_rawDataHashSet = new HashSet<object>();
		protected List<Object> m_rawDataList = new List<object>();
		protected Dictionary<Object, MyObjectBuilder_Base> m_rawDataObjectBuilderList = new Dictionary<object, MyObjectBuilder_Base>();

		//Clean data stores
		private Dictionary<long, BaseObject> m_definitions = new Dictionary<long, BaseObject>();

		#endregion

		#region "Constructors and Initializers"

		public BaseObjectManager()
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();

			m_backingSourceType = InternalBackingType.Hashset;

			m_lastLoadTime = DateTime.Now;

			if (m_lastProfilingOutput == null)
				m_lastProfilingOutput = DateTime.Now;
			if (m_lastInternalProfilingOutput == null)
				m_lastInternalProfilingOutput = DateTime.Now;

			if (m_staticRefreshCountMap == null)
				m_staticRefreshCountMap = new Dictionary<Type, int>();

			m_refreshInterval = 250;
		}

		public BaseObjectManager(Object backingSource, string backingMethodName, InternalBackingType backingSourceType)
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;
			m_isDynamic = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();

			m_backingObject = backingSource;
			m_backingSourceMethod = backingMethodName;
			m_backingSourceType = backingSourceType;

			m_lastLoadTime = DateTime.Now;

			if (m_lastProfilingOutput == null)
				m_lastProfilingOutput = DateTime.Now;
			if (m_lastInternalProfilingOutput == null)
				m_lastInternalProfilingOutput = DateTime.Now;

			if (m_staticRefreshCountMap == null)
				m_staticRefreshCountMap = new Dictionary<Type, int>();

			m_refreshInterval = 250;
		}

		public BaseObjectManager(BaseObject[] baseDefinitions)
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();

			Load(baseDefinitions);
		}

		public BaseObjectManager(List<BaseObject> baseDefinitions)
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();

			Load(baseDefinitions);
		}

		#endregion

		#region "Properties"

		public bool IsMutable
		{
			get { return m_isMutable; }
			set { m_isMutable = value; }
		}

		protected bool Changed
		{
			get
			{
				if (m_changed) return true;
				foreach (BaseObject def in GetInternalData().Values)
				{
					if (def.Changed)
						return true;
				}
				return false;
			}
		}

		public FileInfo FileInfo
		{
			get { return m_fileInfo; }
			set { m_fileInfo = value; }
		}

		public bool IsDynamic
		{
			get { return m_isDynamic; }
			set { m_isDynamic = value; }
		}

		public bool IsResourceLocked
		{
			get { return m_resourceLock.Owned; }
		}

		public bool IsInternalResourceLocked
		{
			get { return (m_rawDataHashSetResourceLock.Owned || m_rawDataListResourceLock.Owned || m_rawDataObjectBuilderListResourceLock.Owned); }
		}

		public bool CanRefresh
		{
			get
			{
				if (!IsDynamic)
					return false;
				if (!IsMutable)
					return false;
				if (IsResourceLocked)
					return false;
				if (IsInternalResourceLocked)
					return false;
				if (!SandboxGameAssemblyWrapper.Instance.IsGameStarted)
					return false;
				if (WorldManager.Instance.IsWorldSaving)
					return false;
				if (WorldManager.Instance.InternalGetResourceLock() == null)
					return false;
				if (WorldManager.Instance.InternalGetResourceLock().Owned)
					return false;

				return true;
			}
		}

		public int Count
		{
			get { return m_definitions.Count; }
		}

		#endregion

		#region "Methods"

		public void SetBackingProperties(Object backingObject, string backingMethod, InternalBackingType backingType)
		{
			m_isDynamic = true;

			m_backingObject = backingObject;
			m_backingSourceMethod = backingMethod;
			m_backingSourceType = backingType;
		}

		private FieldInfo GetMatchingDefinitionsContainerField()
		{
			//Find the the matching field in the container
			Type thisType = typeof(MyObjectBuilder_Base[]);
			Type defType = typeof(MyObjectBuilder_Definitions);
			FieldInfo matchingField = null;
			foreach (FieldInfo field in defType.GetFields())
			{
				Type fieldType = field.FieldType;
				if (thisType.FullName == fieldType.FullName)
				{
					matchingField = field;
					break;
				}
			}

			return matchingField;
		}

		protected virtual bool IsValidEntity(Object entity)
		{
			return true;
		}

		#region "GetDataSource"

		protected Dictionary<long, BaseObject> GetInternalData()
		{
			return m_definitions;
		}

		protected HashSet<Object> GetBackingDataHashSet()
		{
			return m_rawDataHashSet;
		}

		protected List<Object> GetBackingDataList()
		{
			return m_rawDataList;
		}

		protected Dictionary<object, MyObjectBuilder_Base> GetObjectBuilderMap()
		{
			return m_rawDataObjectBuilderList;
		}

		#endregion

		#region "RefreshDataSource"

		public void Refresh()
		{
			if (!CanRefresh)
				return;

			TimeSpan timeSinceLastLoad = DateTime.Now - m_lastLoadTime;
			if (timeSinceLastLoad.TotalMilliseconds < m_refreshInterval)
				return;
			m_lastLoadTime = DateTime.Now;

			//Run the refresh
			RefreshData();

			//Update the refresh counts
			if (!m_staticRefreshCountMap.ContainsKey(this.GetType()))
				m_staticRefreshCountMap.Add(this.GetType(), 1);
			else
				m_staticRefreshCountMap[this.GetType()]++;
			int typeRefreshCount = m_staticRefreshCountMap[this.GetType()];
			m_staticRefreshCount++;

			//Adjust the refresh interval based on percentage of total refreshes for this type
			m_refreshInterval = (typeRefreshCount / m_staticRefreshCount) * 850 + 150;
		}

		private void RefreshData()
		{
			if (!CanRefresh)
				return;

			try
			{
				DateTime startRefreshTime = DateTime.Now;

				Action action = RefreshInternalData;
				SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);

				//Lock the main data
				m_resourceLock.AcquireExclusive();

				//Lock all of the raw data
				if (m_backingSourceType == InternalBackingType.Hashset)
					m_rawDataHashSetResourceLock.AcquireShared();
				if (m_backingSourceType == InternalBackingType.List)
					m_rawDataListResourceLock.AcquireShared();
				m_rawDataObjectBuilderListResourceLock.AcquireShared();

				//Refresh the main data
				LoadDynamic();

				//Unlock the main data
				m_resourceLock.ReleaseExclusive();

				//Unlock all of the raw data
				if (m_backingSourceType == InternalBackingType.Hashset)
					m_rawDataHashSetResourceLock.ReleaseShared();
				if (m_backingSourceType == InternalBackingType.List)
					m_rawDataListResourceLock.ReleaseShared();
				m_rawDataObjectBuilderListResourceLock.ReleaseShared();

				if (SandboxGameAssemblyWrapper.IsDebugging)
				{
					TimeSpan timeToRefresh = DateTime.Now - startRefreshTime;
					m_averageRefreshDataTime = (m_averageRefreshDataTime + timeToRefresh.TotalMilliseconds) / 2;
					TimeSpan timeSinceLastProfilingOutput = DateTime.Now - m_lastProfilingOutput;
					if (timeSinceLastProfilingOutput.TotalSeconds > 30)
					{
						m_lastProfilingOutput = DateTime.Now;
						LogManager.APILog.WriteLine("ObjectManager - Average of " + Math.Round(m_averageRefreshDataTime, 2).ToString() + "ms to refresh API data");
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
			}
		}

		private void RefreshInternalData()
		{
			DateTime startRefreshTime = DateTime.Now;

			//Request refreshes of all internal raw data
			if (m_backingSourceType == InternalBackingType.Hashset)
				InternalRefreshBackingDataHashSet();
			if (m_backingSourceType == InternalBackingType.List)
				InternalRefreshBackingDataList();

			if (SandboxGameAssemblyWrapper.IsDebugging)
			{
				TimeSpan timeToRefresh = DateTime.Now - startRefreshTime;
				m_averageRefreshInternalDataTime = (m_averageRefreshInternalDataTime + timeToRefresh.TotalMilliseconds) / 2;
			}

			startRefreshTime = DateTime.Now;

			InternalRefreshObjectBuilderMap();

			if (SandboxGameAssemblyWrapper.IsDebugging)
			{
				TimeSpan timeToRefresh = DateTime.Now - startRefreshTime;
				m_averageRefreshInternalObjectBuilderDataTime = (m_averageRefreshInternalObjectBuilderDataTime + timeToRefresh.TotalMilliseconds) / 2;
			}

			if (SandboxGameAssemblyWrapper.IsDebugging)
			{
				TimeSpan timeSinceLastProfilingOutput = DateTime.Now - m_lastInternalProfilingOutput;
				if (timeSinceLastProfilingOutput.TotalSeconds > 30)
				{
					m_lastInternalProfilingOutput = DateTime.Now;
					LogManager.APILog.WriteLine("ObjectManager - Average of " + Math.Round(m_averageRefreshInternalDataTime, 2).ToString() + "ms to refresh internal entity data");
					LogManager.APILog.WriteLine("ObjectManager - Average of " + Math.Round(m_averageRefreshInternalObjectBuilderDataTime, 2).ToString() + "ms to refresh internal object builder data");
				}
			}
		}

		protected virtual void LoadDynamic()
		{
			return;
		}

		protected virtual void InternalRefreshBackingDataHashSet()
		{
			try
			{
				if (!CanRefresh)
					return;

				m_rawDataHashSetResourceLock.AcquireExclusive();

				if (m_backingObject == null)
					return;
				var rawValue = BaseObject.InvokeEntityMethod(m_backingObject, m_backingSourceMethod);
				if (rawValue == null)
					return;

				//Create/Clear the hash set
				if (m_rawDataHashSet == null)
					m_rawDataHashSet = new HashSet<object>();
				else
					m_rawDataHashSet.Clear();

				//Only allow valid entities in the hash set
				foreach (var entry in UtilityFunctions.ConvertHashSet(rawValue))
				{
					if (!IsValidEntity(entry))
						continue;

					m_rawDataHashSet.Add(entry);
				}

				m_rawDataHashSetResourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				if(m_rawDataHashSetResourceLock.Owned)
					m_rawDataHashSetResourceLock.ReleaseExclusive();
			}
		}

		protected virtual void InternalRefreshBackingDataList()
		{
			try
			{
				if (!CanRefresh)
					return;

				m_rawDataListResourceLock.AcquireExclusive();

				if (m_backingObject == null)
					return;
				var rawValue = BaseObject.InvokeEntityMethod(m_backingObject, m_backingSourceMethod);
				if (rawValue == null)
					return;

				//Create/Clear the list
				if (m_rawDataList == null)
					m_rawDataList = new List<object>();
				else
					m_rawDataList.Clear();

				//Only allow valid entities in the list
				foreach (var entry in UtilityFunctions.ConvertList(rawValue))
				{
					if (!IsValidEntity(entry))
						continue;

					m_rawDataList.Add(entry);
				}

				m_rawDataListResourceLock.ReleaseExclusive();
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				if(m_rawDataListResourceLock.Owned)
					m_rawDataListResourceLock.ReleaseExclusive();
			}
		}

		protected virtual void InternalRefreshObjectBuilderMap()
		{
			
		}

		#endregion

		#region "Static"

		public static FileInfo GetContentDataFile(string configName)
		{
			string filePath = Path.Combine(Path.Combine(GameInstallationInfo.GamePath, @"Content\Data"), configName);
			FileInfo saveFileInfo = new FileInfo(filePath);

			return saveFileInfo;
		}

		#endregion

		#region "Serializers"

		public static T LoadContentFile<T, TS>(FileInfo fileInfo) where TS : XmlSerializer1
		{
			object fileContent = null;

			string filePath = fileInfo.FullName;

			if (!File.Exists(filePath))
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing, filePath);
			}

			try
			{
				fileContent = ReadSpaceEngineersFile<T, TS>(filePath);
			}
			catch(Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted, filePath);
			}

			if (fileContent == null)
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty, filePath);
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.

			return (T)fileContent;
		}

		public static void SaveContentFile<T, TS>(T fileContent, FileInfo fileInfo) where TS : XmlSerializer1
		{

			string filePath = fileInfo.FullName;

			//if (!File.Exists(filePath))
			//{
			//	throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing, filePath);
			//}

			try
			{
				WriteSpaceEngineersFile<T, TS>(fileContent, filePath);
			}
			catch
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted, filePath);
			}

			if (fileContent == null)
			{
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty, filePath);
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.
		}

		public static T ReadSpaceEngineersFile<T, TS>(string filename)
			where TS : XmlSerializer1
		{
			var settings = new XmlReaderSettings
			{
				IgnoreComments = true,
				IgnoreWhitespace = true,
			};

			object obj = null;

			if (File.Exists(filename))
			{
				using(var xmlReader = XmlReader.Create(filename, settings))
				{
					var serializer = (TS)Activator.CreateInstance(typeof(TS));
					obj = serializer.Deserialize(xmlReader);
				}
			}

			return (T)obj;
		}

		protected T Deserialize<T>(string xml)
		{
			using (var textReader = new StringReader(xml))
			{
				return (T)(new XmlSerializerContract().GetSerializer(typeof(T)).Deserialize(textReader));
			}
		}

		protected string Serialize<T>(object item)
		{
			using (var textWriter = new StringWriter())
			{
				new XmlSerializerContract().GetSerializer(typeof(T)).Serialize(textWriter, item);
				return textWriter.ToString();
			}
		}

		public static bool WriteSpaceEngineersFile<T, TS>(T sector, string filename)
			where TS : XmlSerializer1
		{
			// How they appear to be writing the files currently.
			try
			{
				using (var xmlTextWriter = new XmlTextWriter(filename, null))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.Indentation = 2;
					xmlTextWriter.IndentChar = ' ';
					TS serializer = (TS)Activator.CreateInstance(typeof(TS));
					serializer.Serialize(xmlTextWriter, sector);
				}
			}
			catch
			{
				return false;
			}

			//// How they should be doing it to support Unicode.
			//var settingsDestination = new XmlWriterSettings()
			//{
			//    Indent = true, // Set indent to false to compress.
			//    Encoding = new UTF8Encoding(false)   // codepage 65001 without signature. Removes the Byte Order Mark from the start of the file.
			//};

			//try
			//{
			//    using (var xmlWriter = XmlWriter.Create(filename, settingsDestination))
			//    {
			//        S serializer = (S)Activator.CreateInstance(typeof(S));
			//        serializer.Serialize(xmlWriter, sector);
			//    }
			//}
			//catch (Exception ex)
			//{
			//    return false;
			//}

			return true;
		}

		#endregion

		#region "GetContent"

		public BaseObject GetEntry(long key)
		{
			if (!GetInternalData().ContainsKey(key))
				return null;

			return GetInternalData()[key];
		}

		public List<T> GetTypedInternalData<T>() where T : BaseObject
		{
			try
			{
				m_resourceLock.AcquireShared();

				List<T> newList = new List<T>();
				foreach (var def in GetInternalData().Values)
				{
					if (!(def is T))
						continue;

					newList.Add((T)def);
				}

				m_resourceLock.ReleaseShared();

				Refresh();

				return newList;
			}
			catch (Exception ex)
			{
				LogManager.ErrorLog.WriteLine(ex);
				if(m_resourceLock.Owned)
					m_resourceLock.ReleaseShared();
				return new List<T>();
			}
		}

		#endregion

		#region "NewContent"

		public T NewEntry<T>() where T : BaseObject
		{
			if (!IsMutable) return default(T);

			MyObjectBuilder_Base newBase = MyObjectBuilder_Base.CreateNewObject(typeof(MyObjectBuilder_EntityBase));
			var newEntry = (T)Activator.CreateInstance(typeof(T), new object[] { newBase });
			GetInternalData().Add(m_definitions.Count, newEntry);
			m_changed = true;

			return newEntry;
		}

		[Obsolete]
		public T NewEntry<T>(MyObjectBuilder_Base source) where T : BaseObject
		{
			if (!IsMutable) return default(T);

			var newEntry = (T)Activator.CreateInstance(typeof(T), new object[] { source });
			GetInternalData().Add(m_definitions.Count, newEntry);
			m_changed = true;

			return newEntry;
		}

		public T NewEntry<T>(T source) where T : BaseObject
		{
			if (!IsMutable) return default(T);

			var newEntry = (T)Activator.CreateInstance(typeof(T), new object[] { source.ObjectBuilder });
			GetInternalData().Add(m_definitions.Count, newEntry);
			m_changed = true;

			return newEntry;
		}

		public void AddEntry<T>(long key, T entry) where T : BaseObject
		{
			if (!IsMutable) return;

			GetInternalData().Add(key, entry);
			m_changed = true;
		}

		#endregion

		#region "DeleteContent"

		public bool DeleteEntry(long id)
		{
			if (!IsMutable) return false;

			if (GetInternalData().ContainsKey(id))
			{
				var entry = GetInternalData()[id];
				GetInternalData().Remove(id);
				entry.Dispose();
				m_changed = true;
				return true;
			}

			return false;
		}

		public bool DeleteEntry(BaseObject entry)
		{
			if (!IsMutable) return false;

			foreach (var def in m_definitions)
			{
				if (def.Value.Equals(entry))
				{
					DeleteEntry(def.Key);
					break;
				}
			}

			return false;
		}

		public bool DeleteEntries<T>(List<T> entries) where T : BaseObject
		{
			if (!IsMutable) return false;

			foreach (var entry in entries)
			{
				DeleteEntry(entry);
			}

			return true;
		}

		public bool DeleteEntries<T>(Dictionary<long, T> entries) where T : BaseObject
		{
			if (!IsMutable) return false;

			foreach (var entry in entries.Keys)
			{
				DeleteEntry(entry);
			}

			return true;
		}

		#endregion

		#region "LoadContent"

		public void Load<T>(T[] source) where T : BaseObject
		{
			//Copy the data into the manager
			GetInternalData().Clear();
			foreach (var definition in source)
			{
				GetInternalData().Add(GetInternalData().Count, definition);
			}
		}

		public void Load<T>(List<T> source) where T : BaseObject
		{
			Load(source.ToArray());
		}

		#endregion

		#region "SaveContent"

		public bool Save()
		{
			if (!this.Changed) return false;
			if (!this.IsMutable) return false;
			if (this.FileInfo == null) return false;

			MyObjectBuilder_Definitions definitionsContainer = new MyObjectBuilder_Definitions();

			if (m_definitionsContainerField == null)
				throw new GameInstallationInfoException(GameInstallationInfoExceptionState.Invalid, "Failed to find matching definitions field in the given file.");

			List<MyObjectBuilder_Base> baseDefs = new List<MyObjectBuilder_Base>();
			foreach(var baseObject in GetInternalData().Values)
			{
				baseDefs.Add(baseObject.ObjectBuilder);
			}

			//Save the source data into the definitions container
			m_definitionsContainerField.SetValue(definitionsContainer, baseDefs.ToArray());

			//Save the definitions container out to the file
			SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(definitionsContainer, m_fileInfo);

			return true;
		}

		#endregion

		#endregion
	}
}
