using Sandbox.Common.ObjectBuilders.Definitions;
using System.IO;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class CubeBlockDefinition : ObjectOverLayerDefinition<MyObjectBuilder_CubeBlockDefinition>
    {
		#region "Constructors and Initializers"

		public CubeBlockDefinition(MyObjectBuilder_CubeBlockDefinition definition): base(definition)
		{}

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

        protected override string GetNameFrom(MyObjectBuilder_CubeBlockDefinition definition)
        {
            return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
        }

        #endregion
    }

    public class CubeBlockDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_CubeBlockDefinition, CubeBlockDefinition>
    {
	}
}