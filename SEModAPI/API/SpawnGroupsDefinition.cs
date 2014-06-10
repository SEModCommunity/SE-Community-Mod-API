using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API
{
	public class SpawnGroupsDefinition
	{
		#region "Attributes"

		private MyObjectBuilder_SpawnGroupDefinition _definition;

		#endregion

		#region "Constructors and Initializers"

		public SpawnGroupsDefinition(MyObjectBuilder_SpawnGroupDefinition definition)
		{
			_definition = definition;
			Changed = false;
		}

		#endregion

		#region "Properties"

        public bool Changed { get; private set; }

        public MyObjectBuilder_SpawnGroupDefinition Definition
        {
            get { return _definition; }
            set
            {
                if (_definition == value) return;
                _definition = value;
                Changed = true;
            }
        }

		public string Name
		{
			get { return _definition.TypeId.ToString(); }
		}

		public float Frequency
		{
			get { return _definition.Frequency; }
		}

		public int PrefabCount
		{
			get { return _definition.Prefabs.Length; }
            set
            {
                if (_definition.Frequency == value) return;
                _definition.Frequency = value;
                Changed = true;

            }
		}

		public MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab[] Prefabs
		{
			get { return _definition.Prefabs; }
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SpawnGroupsDefinitionsWrapper
	{
		#region "Attributes"

		private MyObjectBuilder_SpawnGroupDefinition[] _definitions;
		private Dictionary<KeyValuePair<string, string>, int> _nameIndexes = new Dictionary<KeyValuePair<string, string>, int>();

		#endregion

		#region "Constructors and Initializers"

		public SpawnGroupsDefinitionsWrapper(MyObjectBuilder_SpawnGroupDefinition[] definitions)
		{
			_definitions = definitions;
			Changed = false;
			int index = 0;

			foreach (var definition in _definitions)
			{
				_nameIndexes.Add(new KeyValuePair<string, string>(definition.TypeId.ToString() + "_" + _nameIndexes.Count.ToString(), definition.TypeId.ToString() + "_" + _nameIndexes.Count.ToString()), index);
				++index;
			}
		}

		#endregion

        #region "Properties"

        public bool Changed { get; private set; }

        public MyObjectBuilder_SpawnGroupDefinition[] Definitions
        {
            get { return _definitions; }
            set
            {
                if (_definitions == value) return;
                _definitions = value;
                Changed = true;
            }
        }


        #endregion

        #region "Getters"

        public string NameOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].TypeId.ToString() : null;
		}

		public float FrequencyOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].Frequency : -1;
		}

		public int PrefabCountOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].Prefabs.Length : -1;
		}

		public MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab[] PrefabsOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].Prefabs : null;
		}

		#endregion

		#region "Setters"

		public bool SetFrequencyOf(int index, float frequency)
		{
			if (!IsIndexValid(index) || _definitions[index].Frequency == frequency) return false;
			_definitions[index].Frequency = frequency;
			Changed = true;
			return true;
		}

		#endregion

		#region "Methods"

		public SpawnGroupsDefinition GetDefinitionOf(int index)
		{
			if (IsIndexValid(index))
			{
				return new SpawnGroupsDefinition(_definitions[index]);
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
