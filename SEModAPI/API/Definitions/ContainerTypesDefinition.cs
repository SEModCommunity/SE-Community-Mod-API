using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.API.Definitions;

namespace SEModAPI.API.Definitions
{
    public class ContainerTypesDefinition : OverLayerDefinition<MyObjectBuilder_ContainerTypeDefinition>
    {
        #region "Attributes"

        private ContainerTypeItemsManager m_itemsManager;

        #endregion

        #region "Constructors and Initializers"

        public ContainerTypesDefinition(MyObjectBuilder_ContainerTypeDefinition myObjectBuilderDefinitionSubType)
            : base(myObjectBuilderDefinitionSubType)
        {
			m_itemsManager = new ContainerTypeItemsManager();
			m_itemsManager.Load(myObjectBuilderDefinitionSubType.Items);
			m_itemsManager.IsMutable = true;
		}

        #endregion

        #region "Properties"

        new public bool Changed
        {
            get
            {
				foreach (var def in m_itemsManager.Definitions)
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

		new public string Name
		{
			get { return m_baseDefinition.Name; }
			set
			{
				if (m_baseDefinition.Name == value) return;
				m_baseDefinition.Name = value;
				Changed = true;
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

		public MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem[] BaseItems
		{
			get { return m_itemsManager.ExtractBaseDefinitions().ToArray(); }
		}

        public ContainerTypeItem[] Items
        {
            get { return m_itemsManager.Definitions; }
        }

		public ContainerTypeItemsManager ItemsManager
		{
			get { return m_itemsManager; }
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
                if (m_baseDefinition.Id.Equals(value)) return;
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

	public class ContainerTypesDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_ContainerTypeDefinition, ContainerTypesDefinition>
    {
		#region "Methods"

		protected override bool GetChangedState(ContainerTypesDefinition overLayer)
		{
			foreach (var def in overLayer.Items)
			{
				if (def.Changed)
					return true;
			}

			return overLayer.Changed;
		}

		protected override MyObjectBuilder_ContainerTypeDefinition GetBaseTypeOf(ContainerTypesDefinition overLayer)
		{
			MyObjectBuilder_ContainerTypeDefinition baseDef = overLayer.BaseDefinition;
			baseDef.Items = overLayer.BaseItems;

			return baseDef;
		}

		#endregion
	}

	public class ContainerTypeItemsManager : SerializableDefinitionsManager<MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem, ContainerTypeItem>
    {
    }
}
