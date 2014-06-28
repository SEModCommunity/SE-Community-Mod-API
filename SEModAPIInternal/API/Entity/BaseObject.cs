using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API;
using SEModAPI.API.Definitions;

using SEModAPIInternal.API.Entity.Sector;
using System.IO;
using System.Reflection;

namespace SEModAPIInternal.API.Entity
{
	/// <summary>
	/// This class is only intended for easy data access and management. It is a wrapper around all MyObjectBuilder_Base children sub type
	/// </summary>
	public class BaseObject
	{
		#region "Attributes"

		private MyObjectBuilder_Base m_baseEntity;

		#endregion

		#region "Constructors and Initializers"

		public BaseObject(MyObjectBuilder_Base baseEntity)
		{
			m_baseEntity = baseEntity;
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
		/// Internal data of the object
		/// </summary>
		[Category("Object")]
		[Browsable(false)]
		[ReadOnly(true)]
		[Description("Internal data of the object")]
		protected MyObjectBuilder_Base BaseEntity
		{
			get { return m_baseEntity; }
			set
			{
				if (m_baseEntity == value) return;
				m_baseEntity = value;

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
				SerializableDefinitionId newId = new SerializableDefinitionId(m_baseEntity.TypeId, m_baseEntity.SubtypeName);
				return newId;
			}
			set
			{
				if (m_baseEntity.TypeId == value.TypeId && m_baseEntity.SubtypeName == value.SubtypeName) return;
				m_baseEntity = m_baseEntity.ChangeType(value.TypeId, value.SubtypeName);

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
		public MyObjectBuilderTypeEnum TypeId
		{
			get { return m_baseEntity.TypeId; }
			set
			{
				if (m_baseEntity.TypeId == value) return;
				m_baseEntity = m_baseEntity.ChangeType(value, m_baseEntity.SubtypeName);

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
			get { return m_baseEntity.SubtypeName; }
			set
			{
				if (m_baseEntity.SubtypeName == value) return;
				m_baseEntity.SubtypeName = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method to get the casted instance from parent signature
		/// </summary>
		/// <returns>The casted instance into the class type</returns>
		internal virtual MyObjectBuilder_Base GetSubTypeEntity()
		{
			return (MyObjectBuilder_Base)m_baseEntity;
		}

		#endregion
	}

	public class BaseObjectManager
	{
		#region "Attributes"

		private bool m_isMutable = true;
		private bool m_changed = false;

		//Use Long as key and BaseObject as value
		private Dictionary<long, BaseObject> m_definitions = new Dictionary<long, BaseObject>();

		private FileInfo m_fileInfo;
		private readonly FieldInfo m_definitionsContainerField;

		#endregion

		#region "Constructors and Initializers"

		public BaseObjectManager()
		{
			m_fileInfo = null;
			m_changed = false;
			m_isMutable = true;

			m_definitionsContainerField = GetMatchingDefinitionsContainerField();
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

		public BaseObject[] Definitions
		{
			get
			{
				return GetInternalData().Values.ToArray();
			}
		}

		public FileInfo FileInfo
		{
			get { return m_fileInfo; }
			set { m_fileInfo = value; }
		}

		#endregion

		#region "Methods"

		protected virtual Dictionary<long, BaseObject> GetInternalData()
		{
			return m_definitions;
		}

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
				using (var xmlReader = XmlReader.Create(filename, settings))
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
		
		public List<T> GetTypedInternalData<T>() where T : BaseObject
		{
			List<T> newList = new List<T>();
			foreach(var def in GetInternalData().Values)
			{
				newList.Add((T)def);
			}
			return newList;
		}

		public T NewEntry<T>() where T : BaseObject
		{
			if (!IsMutable) return default(T);

			MyObjectBuilder_Base newBase = MyObjectBuilder_Base.CreateNewObject(MyObjectBuilderTypeEnum.EntityBase);
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

			var newEntry = (T)Activator.CreateInstance(typeof(T), new object[] { source.GetSubTypeEntity() });
			GetInternalData().Add(m_definitions.Count, newEntry);
			m_changed = true;

			return newEntry;
		}

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
				if (def.Value.GetSubTypeEntity().Equals(entry))
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

		public void Load(BaseObject[] source)
		{
			//Copy the data into the manager
			GetInternalData().Clear();
			foreach (var definition in source)
			{
				GetInternalData().Add(GetInternalData().Count, definition);
			}
		}

		public void Load(List<BaseObject> source)
		{
			Load(source.ToArray());
		}

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
				baseDefs.Add(baseObject.GetSubTypeEntity());
			}

			//Save the source data into the definitions container
			m_definitionsContainerField.SetValue(definitionsContainer, baseDefs.ToArray());

			//Save the definitions container out to the file
			SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(definitionsContainer, m_fileInfo);

			return true;
		}

		#endregion
	}
}