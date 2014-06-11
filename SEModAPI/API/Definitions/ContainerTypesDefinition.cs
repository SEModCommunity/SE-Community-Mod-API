using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.API.Definitions;

namespace SEModAPI.API
{
	public class ContainerTypesDefinition : OverLayerDefinition<MyObjectBuilder_ContainerTypeDefinition>
	{
		#region "Constructors and Initializers"

		public ContainerTypesDefinition(MyObjectBuilder_ContainerTypeDefinition myObjectBuilderDefinitionSubType): base(myObjectBuilderDefinitionSubType)
		{}

		#endregion

		#region "Properties"

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

		public MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem[] Items
		{
			get { return m_baseDefinition.Items; }
		}

		#endregion

        #region "Methods"

         protected override string GetNameFrom(MyObjectBuilder_ContainerTypeDefinition definition)
         {
             return definition.Name;
         }

        #endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

    public class ContainerTypesDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_ContainerTypeDefinition, ContainerTypesDefinition>
	{
		#region "Constructors and Initializers"

		public ContainerTypesDefinitionsManager(MyObjectBuilder_ContainerTypeDefinition[] baseDefinitions): base(baseDefinitions)
		{}

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

		#endregion
	}
}
