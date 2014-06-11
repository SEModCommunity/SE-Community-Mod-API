using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class BlueprintsDefinition : BaseDefinition<MyObjectBuilder_BlueprintDefinition>
	{
		#region "Constructors and Initializers"

		public BlueprintsDefinition(MyObjectBuilder_BlueprintDefinition definition)
			: base(definition)
		{
		}

		#endregion

		#region "Properties"

		public float BaseProductionTimeInSeconds
		{
			get { return m_definition.BaseProductionTimeInSeconds; }
			set
			{
				if (m_definition.BaseProductionTimeInSeconds == value) return;
				m_definition.BaseProductionTimeInSeconds = value;
				Changed = true;
			}
		}

		public MyObjectBuilder_BlueprintDefinition.Item Result
		{
			get { return m_definition.Result; }
		}

		public MyObjectBuilder_BlueprintDefinition.Item[] Prerequisites
		{
			get { return m_definition.Prerequisites; }
		}

		#endregion
	}

	public class BlueprintDefinitionsWrapper : BaseDefinitionsWrapper<MyObjectBuilder_BlueprintDefinition, BlueprintsDefinition>
	{
		#region "Constructors and Initializers"

		public BlueprintDefinitionsWrapper(MyObjectBuilder_BlueprintDefinition[] definitions)
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

		public MyObjectBuilder_BlueprintDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_BlueprintDefinition[] temp = new MyObjectBuilder_BlueprintDefinition[m_definitions.Count];
				BlueprintsDefinition[] definitionsArray = this.Definitions;

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

			m_configSerializer.BlueprintDefinitions = this.RawDefinitions;
			m_configSerializer.SaveBlueprintsContentFile();
		}

		#endregion
	}
}
