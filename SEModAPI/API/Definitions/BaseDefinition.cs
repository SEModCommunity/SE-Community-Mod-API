using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions
{
	/// <summary>
	/// Base definition that wraps around an object
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class BaseDefinition<T>
	{
		#region "Attributes"

		protected T m_definition;
		protected long m_id;

		#endregion

		#region "Constructors and Initializers"

		public BaseDefinition(T definition)
		{
			m_definition = definition;
			Changed = false;
		}

		#endregion

		#region "Properties"

		public bool Changed { get; protected set; }

		public T Definition
		{
			get { return m_definition; }
		}

		public long InternalId
		{
			get { return m_id; }
			set
			{
				if (m_id == value) return;
				m_id = value;
				Changed = true;
			}
		}

		#endregion
	}

	public class ObjectBuilderDefinition<T> : BaseDefinition<T> where T : MyObjectBuilder_DefinitionBase
	{
		#region "Constructors and Initializers"

		public ObjectBuilderDefinition(T definition)
			: base(definition)
		{
		}

		#endregion

		#region "Properties"

		public SerializableDefinitionId Id
		{
			get { return m_definition.Id; }
			set
			{
				if (m_definition.Id.ToString() == value.ToString()) return;
				m_definition.Id = value;
				Changed = true;
			}
		}

		public string Name
		{
			get { return m_definition.DisplayName; }
			set
			{
				if (m_definition.DisplayName == value) return;
				m_definition.DisplayName = value;
				Changed = true;
			}
		}

		public string Description
		{
			get { return m_definition.Description; }
			set
			{
				if (m_definition.Description == value) return;
				m_definition.Description = value;
				Changed = true;
			}
		}

		#endregion
	}

	//////////////
	// Wrappers //
	//////////////

	/// <summary>
	/// Base definition collection wrapper
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="U"></typeparam>
	public class BaseDefinitionsWrapper<T, U>
	{
		#region "Attributes"

		protected bool m_changed = false;
		protected Dictionary<long, U> m_definitions = new Dictionary<long, U>();
		protected ConfigFileSerializer m_configSerializer = new ConfigFileSerializer();

		#endregion

		#region "Constructors and Initializers"

		public BaseDefinitionsWrapper(T[] definitions)
		{
			m_changed = false;
			long index = 0;

			foreach (var definition in definitions)
			{
				//Create a new instance of the templated type and pass 'definition' as the parameter for the constructor
				U entry = (U)Activator.CreateInstance(typeof(U), new object[] { definition });

				m_definitions.Add(index, entry);

				index++;
			}
		}

		#endregion

		#region "Properties"

		public bool Changed
		{
			get
			{
				return m_changed;
			}
			set
			{
				m_changed = value;
			}
		}

		public U[] Definitions
		{
			get
			{
				//Only way to get dictionary as array until C# 3.0
				U[] temp = new U[m_definitions.Count];
				m_definitions.Values.CopyTo(temp, 0);

				return temp;
			}
		}

		#endregion

		#region "Methods"

		public U GetDefinitionOf(int index)
		{
			if (IsIndexValid(index))
			{
				return m_definitions[index];
			}
			return default(U);
		}

		public long IndexOf(long id)
		{
			if (m_definitions.ContainsKey(id))
				return id;	//TODO - Change this to calculate the index in the dictionary
			else
				return -1;
		}

		private bool IsIndexValid(int index)
		{
			return (index < m_definitions.Values.Count && index >= 0);
		}

		#endregion
	}

	public class NameIdIndexedWrapper<T, U> : BaseDefinitionsWrapper<T, U> where T : MyObjectBuilder_DefinitionBase
	{
		#region "Attributes"

		protected Dictionary<KeyValuePair<string, SerializableDefinitionId>, int> m_nameTypeIndexes = new Dictionary<KeyValuePair<string, SerializableDefinitionId>, int>();

		#endregion

		#region "Constructors and Initializers"

		public NameIdIndexedWrapper(T[] definitions)
			: base(definitions)
		{
		}

		#endregion

		#region "Methods"

		public int IndexOf(string name, SerializableDefinitionId type)
		{
			int index = -1;
			m_nameTypeIndexes.TryGetValue(new KeyValuePair<string, SerializableDefinitionId>(name, type), out index);
			return index;
		}

		#endregion
	}
}
