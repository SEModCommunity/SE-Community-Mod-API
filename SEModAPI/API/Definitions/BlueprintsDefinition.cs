using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions
{
	public class BlueprintsDefinition : OverLayerDefinition<MyObjectBuilder_BlueprintDefinition>
	{
		#region "Constructors and Initializers"

		public BlueprintsDefinition(MyObjectBuilder_BlueprintDefinition definition): base(definition)
		{}

		#endregion

		#region "Properties"

		public float BaseProductionTimeInSeconds
		{
			get { return m_baseDefinition.BaseProductionTimeInSeconds; }
			set
			{
				if (m_baseDefinition.BaseProductionTimeInSeconds == value) return;
				m_baseDefinition.BaseProductionTimeInSeconds = value;
				Changed = true;
			}
		}

		public MyObjectBuilder_BlueprintDefinition.Item Result
		{
			get { return m_baseDefinition.Result; }
		}

		public MyObjectBuilder_BlueprintDefinition.Item[] Prerequisites
		{
			get { return m_baseDefinition.Prerequisites; }
		}

		#endregion

        #region "Methods"

	    protected override string GetNameFrom(MyObjectBuilder_BlueprintDefinition definition)
	    {
	        return definition.TypeId.ToString() + "_" + definition.SubtypeId.ToString();
	    }

        #endregion
	}

    public class BlueprintDefinitionsManager :OverLayerDefinitionsManager<MyObjectBuilder_BlueprintDefinition, BlueprintsDefinition>
    {
        #region "Constructors and Initializers"

        public BlueprintDefinitionsManager(MyObjectBuilder_BlueprintDefinition[] definitions): base(definitions)
        {}

        #endregion

        #region "Methods"

        protected override BlueprintsDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_BlueprintDefinition definition)
        {
            return new BlueprintsDefinition(definition);
        }

        protected override MyObjectBuilder_BlueprintDefinition GetBaseTypeOf(BlueprintsDefinition overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(BlueprintsDefinition overLayer)
        {
            return overLayer.Changed;
        }

		public override void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.BlueprintDefinitions = this.ExtractBaseDefinitions().ToArray();
			m_configSerializer.SaveBlueprintsContentFile();
		}
		
		#endregion  
    }
}
