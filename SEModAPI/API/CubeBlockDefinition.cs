using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API
{
    public class CubeBlockDefinition
    {
        #region "Attributes"

        private MyObjectBuilder_CubeBlockDefinition _definition;

        #endregion

        #region "Constructors and Initializers"

        public CubeBlockDefinition(MyObjectBuilder_CubeBlockDefinition definition)
        {
            _definition = definition;
            Changed = false;
        }

        #endregion

        #region "Properties"

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
            set
            {
                if (_definition.BuildTimeSeconds == value) return;
                _definition.BuildTimeSeconds = value;
                Changed = true;
            }
        }

        public float DisassembleRatio
        {
            get { return _definition.DisassembleRatio; }
            set 
            {
                if (_definition.DisassembleRatio == value) return;
                _definition.DisassembleRatio = value;
                Changed = true;
            }
        }

		public MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[] Components
		{
			get { return _definition.Components; }
		}

        #endregion

    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class CubeBlockDefinitionsWrapper
    {
        #region "Attributes"

        private MyObjectBuilder_CubeBlockDefinition[] _definitions;
        private Dictionary<KeyValuePair<string, string>, int> _nameIndexes = new Dictionary<KeyValuePair<string, string>, int>();

        #endregion

        #region "Contructors and Initializers"

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

        #endregion

        #region "Properties"

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

        #endregion

        #region "Getters"

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

        public float DisassembleRatioOf(int index)
        {
            return IsIndexValid(index) ? _definitions[index].DisassembleRatio : -1;
        }

		public MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[] ComponentsOf(int index)
		{
			return IsIndexValid(index) ? _definitions[index].Components : null;
		}

        #endregion

        #region "Setters"

        public bool SetBuildTimeOf(int index, float time)
        {
            if (!IsIndexValid(index) || _definitions[index].BuildTimeSeconds == time) return false;
            _definitions[index].BuildTimeSeconds = time;
            Changed = true;
            return true;
        }

        public bool SetDisassembleRatioOf(int index, float disassembleRatio)
        {
            if (!IsIndexValid(index) || _definitions[index].DisassembleRatio == disassembleRatio) return false;
            _definitions[index].DisassembleRatio = disassembleRatio;
            Changed = true;
            return true;
        }

        #endregion

        #region "Methods"

        public CubeBlockDefinition GetDefinitionOf(int index)
        {
            if (IsIndexValid(index))
            {
                return new CubeBlockDefinition(_definitions[index]);
            }
            return null;
        }

        private bool IsIndexValid(int index)
        {
            return ((index < _definitions.Length) && index >= 0);
        }

        public int IndexOf(string name, string model)
        {
            int index = -1;
            _nameIndexes.TryGetValue(new KeyValuePair<string, string>(name, model), out index);
            return index;
        }


        #endregion

    }
}