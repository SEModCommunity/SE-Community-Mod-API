using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API
{
	public class ContainerTypesDefinition
	{
		#region "Attributes"

		private MyObjectBuilder_ContainerTypeDefinition _definition;

		#endregion

		#region "Constructors and Initializers"

		public ContainerTypesDefinition(MyObjectBuilder_ContainerTypeDefinition definition)
		{
			_definition = definition;
			Changed = false;
		}

		#endregion

		#region "Getters"

		public string Name
		{
			get { return _definition.Name; }
		}

		public string Id
		{
			get { return _definition.Name; }
		}

		public int ItemCount
		{
			get { return _definition.Items.Length; }
		}

		public int CountMin
		{
			get { return _definition.CountMin; }
		}

		public int CountMax
		{
			get { return _definition.CountMax; }
		}

		#endregion

		#region "Setters"

		public bool SetCountMin(int countMin)
		{
			if (_definition.CountMin == countMin) return false;
			_definition.CountMin = countMin;
			Changed = true;
			return true;
		}

		public bool SetCountMax(int countMax)
		{
			if (_definition.CountMax == countMax) return false;
			_definition.CountMax = countMax;
			Changed = true;
			return true;
		}

		#endregion

		#region "Methods"

		public bool Changed { get; private set; }

		public MyObjectBuilder_ContainerTypeDefinition Definition
		{
			get { return _definition; }
			set
			{
				if (_definition == value) return;
				_definition = value;
				Changed = true;
			}
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContainerTypesDefinitionsWrapper
	{
		#region "Attributes"

		private MyObjectBuilder_ContainerTypeDefinition[] _definitions;
		private Dictionary<KeyValuePair<string, string>, int> _nameIndexes = new Dictionary<KeyValuePair<string, string>, int>();

		#endregion

		#region "Constructors and Initializers"

		public ContainerTypesDefinitionsWrapper(MyObjectBuilder_ContainerTypeDefinition[] definitions)
		{
			_definitions = definitions;
			Changed = false;
			int index = 0;

			foreach (var definition in _definitions)
			{
				_nameIndexes.Add(new KeyValuePair<string, string>(definition.Name, definition.Name), index);
				++index;
			}
		}

		#endregion

		#region "Getters"

		public string NameOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].Name : null;
		}

		public string IdOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].Name : null;
		}

		public int ItemCountOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].Items.Length : -1;
		}

		public int CountMinOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].CountMin : -1;
		}

		public int CountMaxOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].CountMax : -1;
		}

		#endregion

		#region "Setters"

		public bool SetCountMinOf(int index, int countMin)
		{
			if (!IsIndexValid(index) || _definitions[index].CountMin == countMin) return false;
			_definitions[index].CountMin = countMin;
			Changed = true;
			return true;
		}

		public bool SetCountMaxOf(int index, int countMax)
		{
			if (!IsIndexValid(index) || _definitions[index].CountMax == countMax) return false;
			_definitions[index].CountMax = countMax;
			Changed = true;
			return true;
		}

		#endregion

		#region "Methods"

		public bool Changed { get; private set; }

		public MyObjectBuilder_ContainerTypeDefinition[] Definitions
		{
			get { return _definitions; }
			set
			{
				if (_definitions == value) return;
				_definitions = value;
				Changed = true;
			}
		}
		public ContainerTypesDefinition GetDefinitionOf(int index)
		{
			if (IsIndexValid(index))
			{
				return new ContainerTypesDefinition(_definitions[index]);
			}
			return null;
		}

		public int IndexOf(string name, string model)
		{
			int index = -1;
			_nameIndexes.TryGetValue(new KeyValuePair<string, string>(name, model), out index);
			return index;
		}

		private bool IsIndexValid(int index)
		{
			return (index < _definitions.Length && index >= 0);
		}

		#endregion
	}
}
