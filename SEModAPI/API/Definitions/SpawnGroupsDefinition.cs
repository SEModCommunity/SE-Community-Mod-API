using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions
{
    public class SpawnGroupDefinition : OverLayerDefinition<MyObjectBuilder_SpawnGroupDefinition>
	{
		#region "Attributes"

		private SpawnGroupPrefabsManager m_prefabsManager;

		#endregion

		#region "Constructors and Initializers"

		public SpawnGroupDefinition(MyObjectBuilder_SpawnGroupDefinition definition)
            : base(definition)
        {
			m_prefabsManager = new SpawnGroupPrefabsManager();
			if (definition.Prefabs != null)
				m_prefabsManager.Load(definition.Prefabs);
		}

        #endregion

        #region "Properties"

		new public bool Changed
		{
			get
			{
				if (base.Changed) return true;
				foreach (var def in m_prefabsManager.Definitions)
				{
					if (def.Changed)
						return true;
				}
				return false;
			}
			private set { base.Changed = value; }
		}

		new public MyObjectBuilder_SpawnGroupDefinition BaseDefinition
		{
			get
			{
				m_baseDefinition.Prefabs = m_prefabsManager.ExtractBaseDefinitions().ToArray();
				return m_baseDefinition;
			}
		}

		public string DisplayName
		{
			get { return m_baseDefinition.DisplayName; }
			set
			{
				if (m_baseDefinition.DisplayName == value) return;
				m_baseDefinition.DisplayName = value;
				Changed = true;
			}
		}

		public string Description
		{
			get { return m_baseDefinition.Description; }
			set
			{
				if (m_baseDefinition.Description == value) return;
				m_baseDefinition.Description = value;
				Changed = true;
			}
		}

		public float Frequency
        {
            get { return m_baseDefinition.Frequency; }
            set
            {
                if (m_baseDefinition.Frequency == value) return;
                m_baseDefinition.Frequency = value;
                Changed = true;
            }
        }

		public SpawnGroupPrefab[] Prefabs
		{
			get { return m_prefabsManager.Definitions; }
		}

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_SpawnGroupDefinition definition)
        {
			return definition.DisplayName;
        }

		public SpawnGroupPrefab NewEntry()
		{
			return m_prefabsManager.NewEntry();
		}

		public bool DeleteEntry(SpawnGroupPrefab source)
		{
			return m_prefabsManager.DeleteEntry(source);
		}

        #endregion
    }

    public class SpawnGroupPrefab : OverLayerDefinition<MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab>
    {
        #region "Constructors and Initializers"

        public SpawnGroupPrefab(MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab myObjectBuilderDefinitionSubType)
            : base(myObjectBuilderDefinitionSubType)
        { }

        #endregion

        #region "Properties"

        public string File
        {
            get { return m_baseDefinition.File; }
            set
            {
                if (m_baseDefinition.File == value) return;
                m_baseDefinition.File = value;
                Changed = true;
            }
        }

        public VRageMath.Vector3 Position
        {
            get { return m_baseDefinition.Position; }
            set
            {
                if (m_baseDefinition.Position == value) return;
                m_baseDefinition.Position = value;
                Changed = true;
            }
        }

        public string BeaconText
        {
            get { return m_baseDefinition.BeaconText; }
            set
            {
                if (m_baseDefinition.BeaconText == value) return;
                m_baseDefinition.BeaconText = value;
                Changed = true;
            }
        }

        public float Speed
        {
            get { return m_baseDefinition.Speed; }
            set
            {
                if (m_baseDefinition.Speed == value) return;
                m_baseDefinition.Speed = value;
                Changed = true;
            }
        }

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab definition)
        {
            return definition.BeaconText;
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SpawnGroupsDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_SpawnGroupDefinition, SpawnGroupDefinition>
    {
        #region "Methods"

		protected override MyObjectBuilder_SpawnGroupDefinition GetBaseTypeOf(SpawnGroupDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		#endregion
    }

	public class SpawnGroupPrefabsManager : SerializableDefinitionsManager<MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab, SpawnGroupPrefab>
    {
    }
}
