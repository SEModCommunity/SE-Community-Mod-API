using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions
{
    public class SpawnGroupDefinition : OverLayerDefinition<MyObjectBuilder_SpawnGroupDefinition>
	{
		#region "Attributes"

		private SpawnGroupPrefabsManager m_prefabsManager;

		#endregion

		#region "Constructors and Initializers"

		public SpawnGroupDefinition(MyObjectBuilder_SpawnGroupDefinition myObjectBuilderDefinitionSubType)
            : base(myObjectBuilderDefinitionSubType)
        {
			m_prefabsManager = new SpawnGroupPrefabsManager(myObjectBuilderDefinitionSubType.Prefabs);
		}

        #endregion

        #region "Properties"

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

        public int PrefabCount
        {
            get { return m_baseDefinition.Prefabs.Length; }
        }

		public SpawnGroupPrefab[] Prefabs
		{
			get { return m_prefabsManager.Definitions; }
		}

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_SpawnGroupDefinition definition)
        {
            return definition.TypeId.ToString();
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

    public class SpawnGroupsDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_SpawnGroupDefinition, SpawnGroupDefinition>
    {
        #region "Constructors and Initializers"

        public SpawnGroupsDefinitionsManager(MyObjectBuilder_SpawnGroupDefinition[] definitions): base(definitions)
        {}

        #endregion

        #region "Methods"

        protected override SpawnGroupDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_SpawnGroupDefinition definition)
        {
            return new SpawnGroupDefinition(definition);
        }

        protected override MyObjectBuilder_SpawnGroupDefinition GetBaseTypeOf(SpawnGroupDefinition overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(SpawnGroupDefinition overLayer)
        {
			foreach (var def in overLayer.Prefabs)
			{
				if (def.Changed)
					return true;
			}

			return overLayer.Changed;
        }

		public override void Save()
        {
            if (!this.Changed) return;

			m_configSerializer.SpawnGroupDefinitions = this.ExtractBaseDefinitions().ToArray();
            m_configSerializer.SaveSpawnGroupsContentFile();
        }

        #endregion
    }

    public class SpawnGroupPrefabsManager : OverLayerDefinitionsManager<MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab, SpawnGroupPrefab>
    {
        #region "Constructors and Initializers"

        public SpawnGroupPrefabsManager(MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab[] definitions)
            : base(definitions)
        { }

        #endregion

        #region "Methods"

        protected override SpawnGroupPrefab CreateOverLayerSubTypeInstance(MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab definition)
        {
            return new SpawnGroupPrefab(definition);
        }

        protected override MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab GetBaseTypeOf(SpawnGroupPrefab overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(SpawnGroupPrefab overLayer)
        {
			return overLayer.Changed;
        }

		public override void Save()
		{
			throw new System.NotImplementedException();
		}

        #endregion
    }
}
