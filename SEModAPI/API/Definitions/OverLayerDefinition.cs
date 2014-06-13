using Microsoft.Xml.Serialization.GeneratedAssembly;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Reflection;

using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	/// <summary>
	/// This class is only intended for easy data access and management. It is a wrapper around all MyObjectBuilder_Definitions children sub type
	/// </summary>
	public abstract class OverLayerDefinition<TMyObjectBuilder_Definitions_SubType>
	{
		#region "Attributes"

		protected TMyObjectBuilder_Definitions_SubType m_baseDefinition;
		protected string m_name;

		#endregion

		#region "Constructors and Initializers"

		protected OverLayerDefinition(TMyObjectBuilder_Definitions_SubType baseDefinition)
		{
			m_baseDefinition = baseDefinition;
			m_name = GetNameFrom(m_baseDefinition);
			Changed = false;
		}

		#endregion

		#region "Properties"

		public bool Changed { get; protected set; }

		public string Name
		{
			get { return m_name; }
		}

		public TMyObjectBuilder_Definitions_SubType BaseDefinition
		{
			get { return m_baseDefinition; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// This template method should return the representative name of the sub type underlayed by a children
		/// </summary>
		/// <param name="definition">The definition from which to get the information</param>
		/// <returns></returns>
		protected abstract string GetNameFrom(TMyObjectBuilder_Definitions_SubType definition);

		#endregion
	}

	public abstract class ObjectOverLayerDefinition<TMyObjectBuilder_Definitions_SubType> : OverLayerDefinition<TMyObjectBuilder_Definitions_SubType> where TMyObjectBuilder_Definitions_SubType : MyObjectBuilder_DefinitionBase
	{
		#region "Constructors and Initializers"

		protected ObjectOverLayerDefinition(TMyObjectBuilder_Definitions_SubType baseDefinition)
			: base(baseDefinition)
		{ }

		#endregion

		#region "Properties"

		public SerializableDefinitionId Id
		{
			get { return m_baseDefinition.Id; }
			set
			{
				if (m_baseDefinition.Id.ToString() == value.ToString()) return;
				m_baseDefinition.Id = value;
				Changed = true;
			}
		}

		public string Description
		{
			get { return m_baseDefinition.Description; }
			set
			{
				if (m_baseDefinition.Description == value) return;
				m_baseDefinition.Description = value;
				Changed = true;
			}
		}

		#endregion
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public abstract class OverLayerDefinitionsManager<TMyObjectBuilder_Definitions_SubType, TOverLayerDefinition_SubType>
	{
		#region "Attributes"

		protected bool m_changed = false;
		protected long m_index = 0;

		//Use Long (key) as Id and OverLayerDefinition sub type (value) as Name
		protected Dictionary<long, TOverLayerDefinition_SubType> m_definitions = new Dictionary<long, TOverLayerDefinition_SubType>();

		#endregion

		#region "Constructors and Initializers"

		protected OverLayerDefinitionsManager()
		{
			m_changed = false;
		}

		protected OverLayerDefinitionsManager(TMyObjectBuilder_Definitions_SubType[] baseDefinitions)
		{
			m_changed = false;

			foreach (var definition in baseDefinitions)
			{
				AddChildrenFrom(definition);
			}
		}

		public void AddChildrenFrom(TMyObjectBuilder_Definitions_SubType definition)
		{
			m_definitions.Add(m_index, CreateOverLayerSubTypeInstance(definition));
			++m_index;
		}

		#endregion

		#region "Properties"

		protected bool Changed
		{
			get
			{
				foreach (var def in m_definitions)
				{
					if (GetChangedState(def.Value))
						return true;
				}
				return false;
			}
		}

		public TOverLayerDefinition_SubType[] Definitions
		{
			get
			{
				return m_definitions.Values.ToArray();
			}
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// This method is used to extract all instances of MyObjectBuilder_Definitions sub type encapsulated in the manager
		/// This method is slow and should only be used to extract the underlying type
		/// </summary>
		/// <returns>All instances of the MyObjectBuilder_Definitions sub type sub type in the manager</returns>
		public List<TMyObjectBuilder_Definitions_SubType> ExtractBaseDefinitions()
		{
			List<TMyObjectBuilder_Definitions_SubType> list = new List<TMyObjectBuilder_Definitions_SubType>();
			foreach (var def in m_definitions.Values)
			{
				list.Add(GetBaseTypeOf(def));
			}
			return list;
		}

		/// <summary>
		/// This method is to find the index of a specific Id
		/// </summary>
		/// <param name="id">The Id to search for</param>
		/// <returns>The index where to find the definition</returns>
		public long IndexOf(long id)
		{
			return m_definitions.ContainsKey(id) ? id : -1;
		}

		private bool IsIndexValid(int index)
		{
			return (index < m_definitions.Keys.Count && index >= 0);
		}

		public TOverLayerDefinition_SubType DefinitionOf(int index)
		{
			return IsIndexValid(index) ? m_definitions.Values.ToArray()[index] : default(TOverLayerDefinition_SubType);
		}

		#region "Abstract Methods"

		/// <summary>
		/// Template method that create the instance of the children of OverLayerDefinition sub type
		/// </summary>
		/// <param name="definition">MyObjectBuilder_Definitions object</param>
		/// <returns>An instance representing OverLayerDefinition sub type</returns>
		protected abstract TOverLayerDefinition_SubType CreateOverLayerSubTypeInstance(TMyObjectBuilder_Definitions_SubType definition);

		/// <summary>
		/// This template method is intended to extact the BaseObject inside the overlayer
		/// </summary>
		/// <param name="overLayer">the over layer from which to extract the base object</param>
		/// <returns>MyObjectBuilder_Definitions Sub Type</returns>
		protected abstract TMyObjectBuilder_Definitions_SubType GetBaseTypeOf(TOverLayerDefinition_SubType overLayer);

		/// <summary>
		/// This template method is intended to know if the state of the object insate the overlayer has changed
		/// </summary>
		/// <param name="overLayer">the overlayer from which to know if the base type has changed</param>
		/// <returns>if the underlying object has changed</returns>
		protected abstract bool GetChangedState(TOverLayerDefinition_SubType overLayer);

		#endregion

		#endregion
	}

	public class SerializableDefinitionsManager<T, U> : OverLayerDefinitionsManager<T, U> where U : OverLayerDefinition<T>
	{
		#region "Attributes"

		private FileInfo m_fileInfo;
		private readonly GameInstallationInfo m_gameInstallation;
		private MyObjectBuilder_Definitions m_definitionsContainer;

		#endregion

		#region "Constructors and Initializers"

		protected SerializableDefinitionsManager()
		{
			m_changed = false;
			m_gameInstallation = new GameInstallationInfo();
		}

		#endregion

		#region "Properties"

		public FileInfo FileInfo
		{
			get { return m_fileInfo; }
		}

		public MyObjectBuilder_Definitions DefinitionsContainer
		{
			get { return m_definitionsContainer; }
		}

		#endregion

		#region "Methods"

		#region "Static"

		public static FileInfo GetContentDataFile(string configName)
		{
			string filePath = Path.Combine(Path.Combine(GameInstallationInfo.GetGameRegistryPath(), @"Content\Data"), configName);
			FileInfo saveFileInfo = new FileInfo(filePath);

			return saveFileInfo;
		}

		#endregion

		#region "Serializers"

		protected T LoadContentFile<T, TS>(FileInfo fileInfo) where TS : XmlSerializer1
		{
			object fileContent = null;

			string filePath = fileInfo.FullName;

			if (!File.Exists(filePath))
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing), filePath);
			}

			try
			{
				fileContent = ReadSpaceEngineersFile<T, TS>(filePath);
			}
			catch
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted), filePath);
			}

			if (fileContent == null)
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty), filePath);
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.

			return (T)fileContent;
		}

		protected void SaveContentFile<T, TS>(T fileContent, FileInfo fileInfo) where TS : XmlSerializer1
		{

			string filePath = fileInfo.FullName;

			if (!File.Exists(filePath))
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing), filePath);
			}

			try
			{
				WriteSpaceEngineersFile<T, TS>(fileContent, filePath);
			}
			catch
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted), filePath);
			}

			if (fileContent == null)
			{
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty), filePath);
			}

			// TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
			//     Lock the load during this time, in case it happens multiple times.
			// Report a friendly error if this load fails.
		}

		protected T ReadSpaceEngineersFile<T, TS>(Stream stream)
			where TS : XmlSerializer1
		{
			var settings = new XmlReaderSettings
			{
				IgnoreComments = true,
				IgnoreWhitespace = true,
			};

			object obj;

			using (var xmlReader = XmlReader.Create(stream, settings))
			{

				var serializer = (TS)Activator.CreateInstance(typeof(TS));
				//serializer.UnknownAttribute += serializer_UnknownAttribute;
				//serializer.UnknownElement += serializer_UnknownElement;
				//serializer.UnknownNode += serializer_UnknownNode;
				obj = serializer.Deserialize(xmlReader);
			}

			return (T)obj;
		}

		protected bool TryReadSpaceEngineersFile<T, TS>(string filename, out T entity)
			 where TS : XmlSerializer1
		{
			try
			{
				entity = ReadSpaceEngineersFile<T, TS>(filename);
				return true;
			}
			catch
			{
				entity = default(T);
				return false;
			}
		}

		protected T ReadSpaceEngineersFile<T, TS>(string filename)
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

		protected bool WriteSpaceEngineersFile<T, TS>(T sector, string filename)
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

		protected override U CreateOverLayerSubTypeInstance(T definition)
		{
			return (U)Activator.CreateInstance(typeof(U), new object[] { definition });
		}

		protected override T GetBaseTypeOf(U overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(U overLayer)
		{
			return overLayer.Changed;
		}

		public void Load(FileInfo sourceFile)
		{
			m_fileInfo = sourceFile;

			//Get the definitions content from the file
			m_definitionsContainer = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(m_fileInfo);

			//Find the the matching field in the container
			Type thisType = typeof(T[]);
			Type defType = m_definitionsContainer.GetType();
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
			if (matchingField == null)
				throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.Invalid), "Failed to find matching definitions field");

			//Get the data from the definitions container
			T[] baseDefinitions = (T[])matchingField.GetValue(m_definitionsContainer);

			//Copy the data into the manager
			m_definitions.Clear();
			foreach (var definition in baseDefinitions)
			{
				AddChildrenFrom(definition);
			}
		}

		public void Save()
		{
			if (!this.Changed) return;

			SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(m_definitionsContainer, m_fileInfo);
		}

		#endregion
	}
}
