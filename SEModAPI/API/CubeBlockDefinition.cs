using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API
{
    public class CubeBlockDefinition
    {
        private MyObjectBuilder_CubeBlockDefinition _definition;

        public bool Changed { get; private set; }

        public MyObjectBuilder_CubeBlockDefinition Definition
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
            get { return _definition.BlockPairName; }
        }

        public string Id
        {
            get { return _definition.Id.ToString(); }
        }

        public float BuildTime
        {
            get { return _definition.BuildTimeSeconds; }
        }

        public bool SetBuildTime(float time)
        {
            if (_definition.BuildTimeSeconds == time) return false;
            _definition.BuildTimeSeconds = time;
            Changed = true;
            return true;
        }

        public float DisassembleRatio
        {
            get { return _definition.DisassembleRatio; }
        }

        public bool SetDisassembleRatio(float disassembleRatio)
        {
            if (_definition.DisassembleRatio == disassembleRatio) return false;
            _definition.DisassembleRatio = disassembleRatio;
            Changed = true;
            return true;
        }

        public CubeBlockDefinition(MyObjectBuilder_CubeBlockDefinition definition)
        {
            _definition = definition;
            Changed = false;
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class CubeBlockDefinitionsWrapper
    {
        private MyObjectBuilder_CubeBlockDefinition[] _definitions;
        private Dictionary<KeyValuePair<string, string>, int> _nameIndexes = new Dictionary<KeyValuePair<string, string>, int>();

        public bool Changed { get; private set; }

        public MyObjectBuilder_CubeBlockDefinition[] Definitions
        {
            get { return _definitions; }
            set
            {
                if (_definitions == value) return;
                _definitions = value;
                Changed = true;
            }
        }

        public CubeBlockDefinition GetDefinitionOf(int index)
        {
            if (IsIndexValid(index))
            {
                return new CubeBlockDefinition(_definitions[index]);
            }
            return null;
        }

        public int IndexOf(string name, string model)
        {
            int index = -1;
            _nameIndexes.TryGetValue(new KeyValuePair<string, string>(name, model), out index);
            return index;
        }

        public string NameOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].BlockPairName : null;
        }

        public string IdOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].Id.ToString() : null;
        }

        public float BuildTimeOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].BuildTimeSeconds : -1;
        }

        public bool SetBuildTimeOf(int index, float time)
        {
            if (!IsIndexValid(index) || _definitions[index].BuildTimeSeconds == time) return false;
            _definitions[index].BuildTimeSeconds = time;
            Changed = true;
            return true;
        }

        public float DisassembleRatioOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].DisassembleRatio : -1;
        }

        public bool SetDisassembleRatioOf(int index, float disassembleRatio)
        {
            if (!IsIndexValid(index) || _definitions[index].DisassembleRatio == disassembleRatio) return false;
            _definitions[index].DisassembleRatio = disassembleRatio;
            Changed = true;
            return true;
        }

        public CubeBlockDefinitionsWrapper(MyObjectBuilder_CubeBlockDefinition[] definitions)
        {
            _definitions = definitions;
            Changed = false;
            int index = 0;
            foreach (var definition in _definitions)
            {
                _nameIndexes.Add(new KeyValuePair<string, string>(definition.BlockPairName, definition.Id.ToString()), index);
                ++index;
            }
        }

        private bool IsIndexValid(int index)
        {
            return ((index < _definitions.Length) && index >= 0);
        }
    }
}