using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.API.Definitions;

namespace SEModAPI.API
{
    public class CubeBlockDefinition : OverLayerDefinition<MyObjectBuilder_CubeBlockDefinition>
    {
		#region "Constructors and Initializers"

		public CubeBlockDefinition(MyObjectBuilder_CubeBlockDefinition definition): base(definition)
		{}

		#endregion

        #region "Properties"

		new public string Name
		{
			get { return m_baseDefinition.BlockPairName; }
			set
			{
                if (m_baseDefinition.BlockPairName == value) return;
                m_baseDefinition.BlockPairName = value;
				Changed = true;
			}
		}

        public float BuildTime
        {
            get { return m_baseDefinition.BuildTimeSeconds; }
            set
            {
                if (m_baseDefinition.BuildTimeSeconds == value) return;
                m_baseDefinition.BuildTimeSeconds = value;
                Changed = true;
            }
        }

        public float DisassembleRatio
        {
            get { return m_baseDefinition.DisassembleRatio; }
            set 
            {
                if (m_baseDefinition.DisassembleRatio == value) return;
                m_baseDefinition.DisassembleRatio = value;
                Changed = true;
            }
        }
        public MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[] Components
        {
            get { return m_baseDefinition.Components; }
        }

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_CubeBlockDefinition definition)
        {
            return definition.BlockPairName;
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class CubeBlockDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_CubeBlockDefinition, CubeBlockDefinition>
    {
		#region "Constructors and Initializers"

		public CubeBlockDefinitionsManager(MyObjectBuilder_CubeBlockDefinition[] definitions): base(definitions)
		{}

		#endregion

        #region "Methods"

        protected override CubeBlockDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_CubeBlockDefinition definition)
        {
            return new CubeBlockDefinition(definition);
        }

        protected override MyObjectBuilder_CubeBlockDefinition GetBaseTypeOf(CubeBlockDefinition overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(CubeBlockDefinition overLayer)
        {
            return overLayer.Changed;
        }

        #endregion
    }
}