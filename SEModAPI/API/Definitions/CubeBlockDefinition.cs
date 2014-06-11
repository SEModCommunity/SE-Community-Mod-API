using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API
{
	public class CubeBlockDefinition : ObjectBuilderDefinition<MyObjectBuilder_CubeBlockDefinition>
    {
		#region "Constructors and Initializers"

		public CubeBlockDefinition(MyObjectBuilder_CubeBlockDefinition definition)
			: base(definition)
		{
		}

		#endregion

        #region "Properties"

		new public string Name
		{
			get { return m_definition.BlockPairName; }
			set
			{
				if (m_definition.BlockPairName == value) return;
				m_definition.BlockPairName = value;
				Changed = true;
			}
		}

        public float BuildTime
        {
            get { return m_definition.BuildTimeSeconds; }
            set
            {
				if (m_definition.BuildTimeSeconds == value) return;
				m_definition.BuildTimeSeconds = value;
                Changed = true;
            }
        }

        public float DisassembleRatio
        {
			get { return m_definition.DisassembleRatio; }
            set 
            {
				if (m_definition.DisassembleRatio == value) return;
				m_definition.DisassembleRatio = value;
                Changed = true;
            }
        }

		public MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[] Components
		{
			get { return m_definition.Components; }
		}

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

	public class CubeBlockDefinitionsWrapper : NameIdIndexedWrapper<MyObjectBuilder_CubeBlockDefinition, CubeBlockDefinition>
    {
		#region "Constructors and Initializers"

		public CubeBlockDefinitionsWrapper(MyObjectBuilder_CubeBlockDefinition[] definitions)
			: base(definitions)
		{
			int index = 0;
			foreach (var definition in definitions)
			{
				m_nameTypeIndexes.Add(new KeyValuePair<string, SerializableDefinitionId>(definition.DisplayName, definition.Id), index);

				index++;
			}
		}

		#endregion

		#region "Properties"

		new public bool Changed
		{
			get
			{
				foreach (var def in m_definitions)
				{
					if (def.Value.Changed)
						return true;
				}

				return false;
			}
			set
			{
				base.Changed = value;
			}
		}

		public MyObjectBuilder_CubeBlockDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_CubeBlockDefinition[] temp = new MyObjectBuilder_CubeBlockDefinition[m_definitions.Count];
				CubeBlockDefinition[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion
	}
}