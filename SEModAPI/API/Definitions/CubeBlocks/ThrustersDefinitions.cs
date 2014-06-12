using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
    public class ThrusterDefinition : ObjectOverLayerDefinition<MyObjectBuilder_ThrustDefinition>
    {
        #region "Constructors and Initializers"

        public ThrusterDefinition(MyObjectBuilder_ThrustDefinition definition)
            : base(definition)
        { }

        #endregion

        #region "Properties"

        /// <summary>
        /// Get or set the current CubeBlock build time in second.
        /// </summary>
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

        /// <summary>
        /// Get or Set the current CubeBlock DisassembleRatio
        /// The value is a multiplyer of BuildTime
        /// [Disassemble time] = BuildTime * DisassembleRatio
        /// </summary>
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

        /// <summary>
        /// The activation state of the current CubeBlock
        /// </summary>
        public bool Enabled
        {
            get { return m_baseDefinition.Public; }
            set
            {
                if (m_baseDefinition.Public == value) return;
                m_baseDefinition.Public = value;
                Changed = true;
            }
        }

        /// <summary>
        /// The Model intersection state of the current CubeBlock 
        /// </summary>
        public bool UseModelIntersection
        {
            get { return m_baseDefinition.UseModelIntersection; }
            set
            {
                if (m_baseDefinition.UseModelIntersection == value) return;
                m_baseDefinition.UseModelIntersection = value;
                Changed = true;
            }
        }

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_ThrustDefinition definition)
        {
            return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ThrusterDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_ThrustDefinition, ThrusterDefinition>
    {
        #region "Constructors and Initializers"

        public ThrusterDefinitionsManager(MyObjectBuilder_ThrustDefinition[] definitions)
            : base(definitions)
        { }

        #endregion

        #region "Methods"

        protected override ThrusterDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_ThrustDefinition definition)
        {
            return new ThrusterDefinition(definition);
        }

        protected override MyObjectBuilder_ThrustDefinition GetBaseTypeOf(ThrusterDefinition overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(ThrusterDefinition overLayer)
        {
            return overLayer.Changed;
        }

        public override void Save()
        {
            if (!this.Changed) return;

            m_configSerializer.CubeBlockDefinitions = this.ExtractBaseDefinitions().ToArray();
            m_configSerializer.SaveCubeBlocksContentFile();
        }

        #endregion
    }
}