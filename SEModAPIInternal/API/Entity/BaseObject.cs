using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Utility;
using SEModAPIInternal.Support;

namespace SEModAPIInternal.API.Entity
{
	public class BaseObject : IDisposable
	{
		#region "Attributes"

		private MyObjectBuilder_Base m_objectBuilder;
		private Object m_backingObject;

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
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Represent the state of changes in the object")]
		public virtual bool Changed { get; protected set; }

		/// <summary>
		/// API formated name of the object
		/// </summary>
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The formatted name of the object")]
		public virtual string Name { get; private set; }

		/// <summary>
		/// Object builder data of the object
		/// </summary>
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
		[Category("Object")]
		[Browsable(true)]
		[TypeConverter(typeof(ObjectSerializableDefinitionIdTypeConverter))]
		public SerializableDefinitionId Id
		{
			get
			{
				SerializableDefinitionId newId = new SerializableDefinitionId(m_objectBuilder.TypeId, m_objectBuilder.SubtypeName);
				return newId;
			}
			set
			{
				if (m_objectBuilder.TypeId == value.TypeId && m_objectBuilder.SubtypeName == value.SubtypeName) return;
				m_objectBuilder = m_objectBuilder.ChangeType(value.TypeId, value.SubtypeName);

				Changed = true;
			}
		}

		/// <summary>
		/// Enum type of the object
		/// </summary>
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The value ID representing the type of the object")]
		public MyObjectBuilderType TypeId
		{
			get { return m_objectBuilder.TypeId; }
			set
			{
				if (m_objectBuilder.TypeId == value) return;
				m_objectBuilder = m_objectBuilder.ChangeType(value, m_objectBuilder.SubtypeName);

				Changed = true;
			}
		}

		/// <summary>
		/// Sub-type of the object
		/// </summary>
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("The value ID representing the sub-type of the object")]
		public string Subtype
		{
			get { return m_objectBuilder.SubtypeName; }
			set
			{
				if (m_objectBuilder.SubtypeName == value) return;
				m_objectBuilder.SubtypeName = value;
				Changed = true;
			}
		}

		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		public bool IsDisposed
		{
			get { return m_isDisposed; }
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

		#region "Internal"

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
					LogManager.GameLog.WriteLine(Environment.StackTrace);
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		internal static FieldInfo GetEntityField(Object gameEntity, string fieldName)
		{
			try
			{
				FieldInfo field = gameEntity.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (field == null)
					field = gameEntity.GetType().BaseType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				return field;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity field '" + fieldName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.GameLog.WriteLine(Environment.StackTrace);
				LogManager.GameLog.WriteLine(ex);
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
					method = objectType.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get static method '" + methodName + "'");
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.GameLog.WriteLine(Environment.StackTrace);
				LogManager.GameLog.WriteLine(ex);
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
				Type entityType = gameEntity.GetType();
				MethodInfo method = entityType.GetMethod(methodName);
				if (method == null)
					method = entityType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
					method = entityType.BaseType.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);
				if (method == null)
					throw new Exception("Method not found");
				return method;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to get entity method '" + methodName + "': " + ex.Message);
				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.GameLog.WriteLine(Environment.StackTrace);
				LogManager.GameLog.WriteLine(ex);
				return null;
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
					LogManager.GameLog.WriteLine(Environment.StackTrace);
				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		internal static Object InvokeEntityMethod(Object gameEntity, string methodName)
		{
			return InvokeEntityMethod(gameEntity, methodName, new object[] { });
		}

		internal static Object InvokeEntityMethod(Object gameEntity, string methodName, Object[] parameters)
		{
			try
			{
				MethodInfo method = GetEntityMethod(gameEntity, methodName);
				if (method == null)
					throw new Exception("Method is empty");
				Object result = method.Invoke(gameEntity, parameters);

				return result;
			}
			catch (Exception ex)
			{
				LogManager.APILog.WriteLine("Failed to invoke entity method '" + methodName + "' on type '" + gameEntity.GetType().FullName + "': " + ex.Message);
				
				//DEBUG
				if (methodName.ToLower().Equals("getobjectbuilder"))
					return null;

				if (SandboxGameAssemblyWrapper.IsDebugging)
					LogManager.GameLog.WriteLine(Environment.StackTrace);

				LogManager.GameLog.WriteLine(ex);
				return null;
			}
		}

		#endregion

		#endregion
	}

	public class BaseObjectManager
	{
		#region "Attributes"

		private FileInfo m_fileInfo;
		private readonly FieldInfo m_definitionsContainerField;
		protected Object m_backingObject;
		protected string m_backingSourceMethod;
		protected DateTime m_lastBackingHashSetLoadTime;

		//Flags
		private bool m_isMutable = true;
		private bool m_changed = false;
		private bool m_isDynamic = false;
		private bool m_isResourceLocked = false;
		protected bool m_isInternalResourceLocked = false;

		//Raw data stores
		protected HashSet<Object> m_rawDataHashSet = new HashSet<object>();
		protected List<Object> m_rawDataList = new List<object>();

		//Clean data stores
		private Dictionary<long, BaseObject> m_definitions = new Dictionary<long, BaseObject>();
		private Dictionary<Object, BaseObject> m_backingDefinitions = new Dictionary<Object, BaseObject>();

		#endregion

		#region "Constructors and Initializers"

		public BaseObjectManager()
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();
		}

		public BaseObjectManager(Object backingSource, string backingMethodName)
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;
			m_isDynamic = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();

			m_backingObject = backingSource;
			m_backingSourceMethod = backingMethodName;
		}

		public BaseObjectManager(MyObjectBuilder_Base[] baseDefinitions)
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();

			Load(baseDefinitions);
		}

		public BaseObjectManager(List<MyObjectBuilder_Base> baseDefinitions)
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();

			Load(baseDefinitions);
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
				foreach (var def in GetInternalData())
				{
					if (GetChangedState(def.Value))
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
			get { return m_isResourceLocked; }
			protected set { m_isResourceLocked = value; }
		}

		#endregion

		#region "Methods"

		#region "GetDataSource"

		protected virtual Dictionary<long, BaseObject> GetInternalData()
		{
			return m_definitions;
		}

		protected virtual Dictionary<Object, BaseObject> GetBackingInternalData()
		{
			return m_backingDefinitions;
		}

		protected HashSet<Object> GetBackingDataHashSet()
		{
			try
			{
				TimeSpan timeSinceLastLoad = DateTime.Now - m_lastBackingHashSetLoadTime;

				if (!m_isInternalResourceLocked)// && timeSinceLastLoad.TotalMilliseconds > 100)
				{
					m_lastBackingHashSetLoadTime = DateTime.Now;

					Action action = InternalGetBackingDataHashSet;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}

				return m_rawDataHashSet;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return new HashSet<object>();
			}
		}

		protected virtual void InternalGetBackingDataHashSet()
		{
			try
			{
				if (m_isInternalResourceLocked)
					return;

				m_isInternalResourceLocked = true;

				if (m_backingObject == null)
					return;
				var rawValue = BaseObject.InvokeEntityMethod(m_backingObject, m_backingSourceMethod);
				if (rawValue == null)
					return;
				m_rawDataHashSet = UtilityFunctions.ConvertHashSet(rawValue);

				m_isInternalResourceLocked = false;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
		}

		protected List<Object> GetBackingDataList()
		{
			try
			{
				if (!m_isInternalResourceLocked)
				{
					m_isInternalResourceLocked = true;

					Action action = InternalGetBackingDataList;
					SandboxGameAssemblyWrapper.Instance.EnqueueMainGameAction(action);
				}

				return m_rawDataList;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return new List<object>();
			}
		}

		protected virtual void InternalGetBackingDataList()
		{
			try
			{
				if (m_backingObject == null)
					return;
				var rawValue = BaseObject.InvokeEntityMethod(m_backingObject, m_backingSourceMethod);
				if (rawValue == null)
					return;
				m_rawDataList = UtilityFunctions.ConvertList(rawValue);

				m_isInternalResourceLocked = false;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
			}
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
				LogManager.GameLog.WriteLine(ex);
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

		protected bool GetChangedState(BaseObject overLayer)
		{
			return overLayer.Changed;
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
		
		public virtual List<T> GetTypedInternalData<T>() where T : BaseObject
		{
			try
			{
				if (IsDynamic)
				{
					LoadDynamic();
				}

				List<T> newList = new List<T>();
				if(!m_isResourceLocked)
				{
					//m_isResourceLocked = true;
					foreach (var def in GetInternalData().Values)
					{
						if (!(def is T))
							continue;

						newList.Add((T)def);
					}
					//m_isResourceLocked = false;
				}

				return newList;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return new List<T>();
			}
		}

		#endregion

		#region "NewContent"

		public T NewEntry<T>() where T : BaseObject
		{
			if (!IsMutable) return default(T);

			MyObjectBuilder_Base newBase = MyObjectBuilder_Base.CreateNewObject(typeof(MyObjectBuilder_EntityBase));
			return NewEntry<T>(newBase);
		}

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

		#endregion

		#region "DeleteContent"

		public bool DeleteEntry(long id)
		{
			if (!IsMutable) return false;

			if (GetInternalData().ContainsKey(id))
			{
				GetInternalData().Remove(id);
				m_changed = true;
				return true;
			}

			return false;
		}

		public bool DeleteEntry(MyObjectBuilder_Base entry)
		{
			if (!IsMutable) return false;

			foreach (var def in m_definitions)
			{
				if (def.Value.ObjectBuilder.Equals(entry))
				{
					GetInternalData().Remove(def.Key);
					m_changed = true;
					return true;
				}
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
					GetInternalData().Remove(def.Key);
					m_changed = true;
					return true;
				}
			}

			return false;
		}

		#endregion

		#region "LoadContent"

		public void Load(FileInfo sourceFile)
		{
			m_fileInfo = sourceFile;

			//Get the definitions content from the file
			MyObjectBuilder_Definitions definitionsContainer = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(m_fileInfo);

			if (m_definitionsContainerField == null)
				throw new SEConfigurationException(SEConfigurationExceptionState.InvalidConfigurationFile, "Failed to find matching definitions field in the specified file.");

			//Get the data from the definitions container
			MyObjectBuilder_Base[] baseDefinitions = (MyObjectBuilder_Base[])m_definitionsContainerField.GetValue(definitionsContainer);

			Load(baseDefinitions);
		}

		public void Load(MyObjectBuilder_Base[] source)
		{
			//Copy the data into the manager
			GetInternalData().Clear();
			foreach (var definition in source)
			{
				NewEntry<BaseObject>(definition);
			}
		}

		public void Load(List<MyObjectBuilder_Base> source)
		{
			Load(source.ToArray());
		}

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

		public virtual void LoadDynamic()
		{
			throw new NotImplementedException();
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
