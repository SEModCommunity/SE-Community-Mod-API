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

    public class BlueprintDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_BlueprintDefinition, BlueprintsDefinition>
    {
    }
}
