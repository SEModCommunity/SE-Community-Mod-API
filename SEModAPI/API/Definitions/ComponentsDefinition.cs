using System;
using System.Collections.Generic;
using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class ComponentsDefinition : PhysicalItemsDefinition<MyObjectBuilder_ComponentDefinition>
	{
		#region "Constructors and Initializers"

		public ComponentsDefinition(MyObjectBuilder_ComponentDefinition definition)
			: base(definition)
		{
		}

		#endregion

        #region "Properties"

		public int MaxIntegrity
		{
			get { return m_definition.MaxIntegrity; }
			set
			{
				if (m_definition.MaxIntegrity == value) return;
				m_definition.MaxIntegrity = value;
				Changed = true;
			}
		}

		public float DropProbability
		{
			get { return m_definition.DropProbability; }
			set
			{
				if (m_definition.DropProbability == value) return;
				m_definition.DropProbability = value;
				Changed = true;
			}
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ComponentDefinitionsWrapper : NameIdIndexedWrapper<MyObjectBuilder_ComponentDefinition, ComponentsDefinition>
	{
		#region "Constructors and Initializers"

		public ComponentDefinitionsWrapper(MyObjectBuilder_ComponentDefinition[] definitions)
			: base(definitions)
		{
			int index = 0;
			foreach (var definition in definitions)
			{
				m_nameTypeIndexes.Add(new KeyValuePair<string, SerializableDefinitionId>(definition.DisplayName, definition.Id), index);

				index++;
			}
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

		public MyObjectBuilder_ComponentDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_ComponentDefinition[] temp = new MyObjectBuilder_ComponentDefinition[m_definitions.Count];
				ComponentsDefinition[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion

		#region "Methods"

		public void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.ComponentDefinitions = this.RawDefinitions;
			m_configSerializer.SaveComponentsContentFile();
		}

		#endregion
	}
}
