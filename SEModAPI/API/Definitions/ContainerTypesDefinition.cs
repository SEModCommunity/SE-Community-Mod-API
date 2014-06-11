using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.API.Definitions;

namespace SEModAPI.API.Definitions
{
    public class ContainerTypesDefinition : OverLayerDefinition<MyObjectBuilder_ContainerTypeDefinition>
    {
        #region "Attributes"

        private ContainerTypeItemsWrapper m_itemsWrapper;

        #endregion

        #region "Constructors and Initializers"

        public ContainerTypesDefinition(MyObjectBuilder_ContainerTypeDefinition myObjectBuilderDefinitionSubType)
            : base(myObjectBuilderDefinitionSubType)
        { }

        #endregion

        #region "Properties"

        new public bool Changed
        {
            get
            {
                foreach (var def in m_itemsWrapper.Definitions)
                {
                    if (def.Changed)
                        return true;
                }

                return false;
            }
            set
            {
                base.Changed = value;
            }
        }

        new public MyObjectBuilder_ContainerTypeDefinition Definition
        {
            get
            {
                m_baseDefinition.Items = m_itemsWrapper.RawDefinitions;
                return m_definition;
            }
        }

        public MyObjectBuilderTypeEnum TypeId
        {
            get { return m_baseDefinition.TypeId; }
        }

        public int SubtypeId
        {
            get { return m_baseDefinition.SubtypeId; }
        }

        public int ItemCount
        {
            get { return m_baseDefinition.Items.Length; }
        }

        public int CountMin
        {
            get { return m_baseDefinition.CountMin; }
            set
            {
                if (m_baseDefinition.CountMin == value) return;
                m_baseDefinition.CountMin = value;
                Changed = true;
            }
        }

        public int CountMax
        {
            get { return m_baseDefinition.CountMax; }
            set
            {
                if (m_baseDefinition.CountMax == value) return;
                m_baseDefinition.CountMax = value;
                Changed = true;
            }
        }

        public ContainerTypeItem[] Items
        {
            get { return m_itemsWrapper.Definitions; }
        }

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_ContainerTypeDefinition definition)
        {
            return definition.Name;
        }

        #endregion
    }

    public class ContainerTypeItem : OverLayerDefinition<MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem>
    {
        #region "Constructors and Initializers"

        public ContainerTypeItem(MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem definition)
            : base(definition)
        {
        }

        #endregion

        #region "Properties"

        public SerializableDefinitionId Id
        {
            get { return m_baseDefinition.Id; }
            set
            {
                if (m_baseDefinition.Id.ToString() == value.ToString()) return;
                m_baseDefinition.Id = value;
                Changed = true;
            }
        }

        public decimal AmountMin
        {
            get { return m_baseDefinition.AmountMin; }
            set
            {
                if (m_baseDefinition.AmountMin == value) return;
                m_baseDefinition.AmountMin = value;
                Changed = true;
            }
        }

        public decimal AmountMax
        {
            get { return m_baseDefinition.AmountMax; }
            set
            {
                if (m_baseDefinition.AmountMax == value) return;
                m_baseDefinition.AmountMax = value;
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

        #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem definition)
        {
            return definition.Id.ToString();
        }

        #endregion
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ContainerTypesDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_ContainerTypeDefinition, ContainerTypesDefinition>
    {
        #region "Constructors and Initializers"

        public ContainerTypesDefinitionsManager(MyObjectBuilder_ContainerTypeDefinition[] baseDefinitions)
            : base(baseDefinitions)
        { }

        #endregion

        #region "Methods"

        protected override ContainerTypesDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_ContainerTypeDefinition definition)
        {
            return new ContainerTypesDefinition(definition);
        }

        protected override MyObjectBuilder_ContainerTypeDefinition GetBaseTypeOf(ContainerTypesDefinition overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(ContainerTypesDefinition overLayer)
        {
            return overLayer.Changed;
        }

        public override void Save()
        {
            if (!this.Changed) return;

			m_configSerializer.ContainerTypeDefinitions = this.ExtractBaseDefinitions().ToArray();
            m_configSerializer.SaveContainerTypesContentFile();
        }

        #endregion
    }

    public class ContainerTypeItemsWrapper : OverLayerDefinitionsManager<MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem, ContainerTypeItem>
    {
        #region "Constructors and Initializers"

        public ContainerTypeItemsWrapper(MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem[] definitions)
            : base(definitions)
        {
        }

        #endregion

        #region "Methods"

        protected override ContainerTypeItem CreateOverLayerSubTypeInstance(MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem definition)
        {
            return new ContainerTypeItem(definition);
        }

        protected override MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem GetBaseTypeOf(ContainerTypeItem overLayer)
        {
            return overLayer.BaseDefinition;
        }

        protected override bool GetChangedState(ContainerTypeItem overLayer)
        {
            return overLayer.Changed;
        }

        #endregion
    }
}
