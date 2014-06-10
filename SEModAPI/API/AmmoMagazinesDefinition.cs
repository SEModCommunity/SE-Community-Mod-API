using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API
{
    public class AmmoMagazinesDefinition
    {
        #region "Attributes"

        private MyObjectBuilder_AmmoMagazineDefinition _definition;

        #endregion

        #region "Constructors and Initializers"

        public AmmoMagazinesDefinition(MyObjectBuilder_AmmoMagazineDefinition definition)
        {
            _definition = definition;
            Changed = false;
        }

        #endregion

        #region "Properties"

        public bool Changed { get; private set; }

        public MyObjectBuilder_AmmoMagazineDefinition Definition
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
            get { return _definition.DisplayName; }
        }

        public string Id
        {
            get { return _definition.Id.ToString(); }
        }

        public string Caliber
        {
            get { return _definition.Category.ToString(); }
        }

        public int Capacity
        {
            get { return _definition.Capacity; }
            set
            {
                if (_definition.Capacity == value) return;
                _definition.Capacity = value;
                Changed = true;
            }
        }

        public float Mass
        {
            get { return _definition.Mass; }
            set
            {
                if (_definition.Mass == value) return;
                _definition.Mass = value;
                Changed = true;
            }
        }

        public float Volume
        {
            get { return _definition.Volume.GetValueOrDefault(-1); }
            set
            {
                if (_definition.Volume == value) return;
                _definition.Volume = value;
                Changed = true;

            }
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class AmmoMagazinesDefinitionsWrapper
    {
        #region "Attributes"

        private MyObjectBuilder_AmmoMagazineDefinition[] _definitions;
        private Dictionary<KeyValuePair<string, string>, int> _nameIndexes = new Dictionary<KeyValuePair<string, string>, int>();

        #endregion

        #region "Constructors and Initializers"

        public AmmoMagazinesDefinitionsWrapper(MyObjectBuilder_AmmoMagazineDefinition[] definitions)
        {
            _definitions = definitions;
            Changed = false;
            int index = 0;

            foreach (var definition in _definitions)
            {
                _nameIndexes.Add(new KeyValuePair<string, string>(definition.DisplayName, definition.Id.ToString()), index);
                ++index;
            }
        }

        #endregion

        #region "Attributes"

        public bool Changed { get; private set; }

        public MyObjectBuilder_AmmoMagazineDefinition[] Definitions
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

        #region "Methods"

        private bool IsIndexValid(int index)
        {
            return (index < _definitions.Length && index >= 0);
        }

        public int IndexOf(string name, string model)
        {
            int index = -1;
            _nameIndexes.TryGetValue(new KeyValuePair<string, string>(name, model), out index);
            return index;
        }

        #region "Getters"

        public AmmoMagazinesDefinition DefinitionOf(int index)
        {
            if (IsIndexValid(index))
            {
                return new AmmoMagazinesDefinition(_definitions[index]);
            }
            return null;
        }

        public string NameOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].DisplayName : null;
        }

        public string IdOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].Id.ToString() : null;
        }

        public string CaliberOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].Category.ToString() : null;
        }

        public int CapacityOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].Capacity : -1;
        }

        public float MassOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].Mass : -1;
        }

        public float VolumeOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].Volume.GetValueOrDefault(-1) : -1;
        }

        #endregion

        #region "Setters"

        public bool SetCapacityOf(int index, int capacity)
        {
            if (!IsIndexValid(index) || _definitions[index].Capacity == capacity) return false;
            _definitions[index].Capacity = capacity;
            Changed = true;
            return true;
        }

        public bool SetMassOf(int index, float mass)
        {
            if (!IsIndexValid(index) || _definitions[index].Mass == mass) return false;
            _definitions[index].Mass = mass;
            Changed = true;
            return true;
        }

        public bool SetVolumeOf(int index, float volume)
        {
            if (!IsIndexValid(index) || _definitions[index].Volume == volume) return false;
            _definitions[index].Volume = volume;
            Changed = true;
            return true;
        }

        #endregion

        #endregion
    }
}
