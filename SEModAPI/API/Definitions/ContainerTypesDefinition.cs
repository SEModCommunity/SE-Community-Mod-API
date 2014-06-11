using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class ContainerTypesDefinition : BaseDefinition<MyObjectBuilder_ContainerTypeDefinition>
	{
		#region "Attributes"

		private ContainerTypeItemsWrapper m_itemsWrapper;

		#endregion

		#region "Constructors and Initializers"

		public ContainerTypesDefinition(MyObjectBuilder_ContainerTypeDefinition definition)
			: base(definition)
		{
			m_itemsWrapper = new ContainerTypeItemsWrapper(m_definition.Items);
		}

		#endregion

		#region "Properties"

		new public MyObjectBuilder_ContainerTypeDefinition Definition
		{
			get
			{
				m_definition.Items = m_itemsWrapper.RawDefinitions;
				return m_definition;
			}
		}

		public MyObjectBuilderTypeEnum TypeId
		{
			get { return m_definition.TypeId; }
		}

		public int SubtypeId
		{
			get { return m_definition.SubtypeId; }
		}

		public string Name
		{
			get { return m_definition.Name; }
			set
			{
				if (m_definition.Name == value) return;
				m_definition.Name = value;
				Changed = true;
			}
		}

		public int ItemCount
		{
			get { return m_definition.Items.Length; }
		}

		public int CountMin
		{
			get { return m_definition.CountMin; }
            set
            {
				if (m_definition.CountMin == value) return;
				m_definition.CountMin = value;
                Changed = true;
            }
		}

		public int CountMax
		{
			get { return m_definition.CountMax; }
            set
            {
				if (m_definition.CountMax == value) return;
				m_definition.CountMax = value;
                Changed = true;
            }
		}

		public ContainerTypeItem[] Items
		{
			get { return m_itemsWrapper.Definitions; }
		}

		#endregion
	}

	public class ContainerTypeItem : BaseDefinition<MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem>
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
			get { return m_definition.Id; }
			set
			{
				if (m_definition.Id.ToString() == value.ToString()) return;
				m_definition.Id = value;
				Changed = true;
			}
		}

		public decimal AmountMin
		{
			get { return m_definition.AmountMin; }
			set
			{
				if (m_definition.AmountMin == value) return;
				m_definition.AmountMin = value;
				Changed = true;
			}
		}

		public decimal AmountMax
		{
			get { return m_definition.AmountMax; }
			set
			{
				if (m_definition.AmountMax == value) return;
				m_definition.AmountMax = value;
				Changed = true;
			}
		}

		public float Frequency
		{
			get { return m_definition.Frequency; }
			set
			{
				if (m_definition.Frequency == value) return;
				m_definition.Frequency = value;
				Changed = true;
			}
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ContainerTypesDefinitionsWrapper : BaseDefinitionsWrapper<MyObjectBuilder_ContainerTypeDefinition, ContainerTypesDefinition>
	{
		#region "Constructors and Initializers"

		public ContainerTypesDefinitionsWrapper(MyObjectBuilder_ContainerTypeDefinition[] definitions)
			: base(definitions)
		{
		}

		#endregion

		#region "Properties"

		new public bool Changed
		{
			get
			{
				foreach (var def in m_definitions)
				{
					if (def.Value.Changed)
						return true;
				}

				return false;
			}
			set
			{
				base.Changed = value;
			}
		}

		public MyObjectBuilder_ContainerTypeDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_ContainerTypeDefinition[] temp = new MyObjectBuilder_ContainerTypeDefinition[m_definitions.Count];
				ContainerTypesDefinition[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion

		#region "Methods"

		public int IndexOf(ContainerTypesDefinition item)
		{
			int index = 0;
			bool foundMatch = false;
			foreach (var def in m_definitions)
			{
				if (def.Value == item)
				{
					foundMatch = true;
					break;
				}

				index++;
			}

			if (foundMatch)
				return index;
			else
				return -1;
		}

		#endregion
	}

	public class ContainerTypeItemsWrapper : BaseDefinitionsWrapper<MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem, ContainerTypeItem>
	{
		#region "Constructors and Initializers"

		public ContainerTypeItemsWrapper(MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem[] definitions)
			: base(definitions)
		{
		}

		#endregion

		#region "Properties"

		new public bool Changed
		{
			get
			{
				foreach (var def in m_definitions)
				{
					if (def.Value.Changed)
						return true;
				}

				return false;
			}
			set
			{
				base.Changed = value;
			}
		}

		public MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem[] temp = new MyObjectBuilder_ContainerTypeDefinition.ContainerTypeItem[m_definitions.Count];
				ContainerTypeItem[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion
		
		#region "Methods"

		public int IndexOf(ContainerTypeItem item)
		{
			int index = 0;
			bool foundMatch = false;
			foreach (var def in m_definitions)
			{
				if (def.Value == item)
				{
					foundMatch = true;
					break;
				}

				index++;
			}

			if (foundMatch)
				return index;
			else
				return -1;
		}

		#endregion
	}
}
