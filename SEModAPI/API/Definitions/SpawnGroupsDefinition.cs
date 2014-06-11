using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class SpawnGroupDefinition : BaseDefinition<MyObjectBuilder_SpawnGroupDefinition>
	{
		#region "Attributes"

		private SpawnGroupPrefabsWrapper m_prefabsWrapper;

		#endregion

		#region "Constructors and Initializers"

		public SpawnGroupDefinition(MyObjectBuilder_SpawnGroupDefinition definition)
			: base(definition)
		{
			m_prefabsWrapper = new SpawnGroupPrefabsWrapper(m_definition.Prefabs);
		}

		#endregion

		#region "Properties"

		new public MyObjectBuilder_SpawnGroupDefinition Definition
		{
			get {
				m_definition.Prefabs = m_prefabsWrapper.RawDefinitions;
				return m_definition;
			}
		}

		public string Name
		{
			get { return m_definition.TypeId.ToString(); }
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

		public int PrefabCount
		{
			get { return m_definition.Prefabs.Length; }
		}

		public SpawnGroupPrefab[] Prefabs
		{
			get { return m_prefabsWrapper.Definitions; }
		}

		#endregion
	}

	public class SpawnGroupPrefab : BaseDefinition<MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab>
	{
		#region "Constructors and Initializers"

		public SpawnGroupPrefab(MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab definition)
			: base(definition)
		{
		}

		#endregion

		#region "Properties"

		public string File
		{
			get { return m_definition.File; }
			set
			{
				if (m_definition.File == value) return;
				m_definition.File = value;
				Changed = true;
			}
		}

		public VRageMath.Vector3 Position
		{
			get { return m_definition.Position; }
			set
			{
				if (m_definition.Position == value) return;
				m_definition.Position = value;
				Changed = true;
			}
		}

		public string BeaconText
		{
			get { return m_definition.BeaconText; }
			set
			{
				if (m_definition.BeaconText == value) return;
				m_definition.BeaconText = value;
				Changed = true;
			}
		}

		public float Speed
		{
			get { return m_definition.Speed; }
			set
			{
				if (m_definition.Speed == value) return;
				m_definition.Speed = value;
				Changed = true;
			}
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SpawnGroupsDefinitionsWrapper : BaseDefinitionsWrapper<MyObjectBuilder_SpawnGroupDefinition, SpawnGroupDefinition>
	{
		#region "Constructors and Initializers"

		public SpawnGroupsDefinitionsWrapper(MyObjectBuilder_SpawnGroupDefinition[] definitions)
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

		public MyObjectBuilder_SpawnGroupDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_SpawnGroupDefinition[] temp = new MyObjectBuilder_SpawnGroupDefinition[m_definitions.Count];
				SpawnGroupDefinition[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion

		#region "Methods"

		public int IndexOf(SpawnGroupDefinition item)
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

	public class SpawnGroupPrefabsWrapper : BaseDefinitionsWrapper<MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab, SpawnGroupPrefab>
	{
		#region "Constructors and Initializers"

		public SpawnGroupPrefabsWrapper(MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab[] definitions)
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

		public MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab[] temp = new MyObjectBuilder_SpawnGroupDefinition.SpawnGroupPrefab[m_definitions.Count];
				SpawnGroupPrefab[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion

		#region "Methods"

		public int IndexOf(SpawnGroupPrefab spawnGroup)
		{
			int index = 0;
			bool foundMatch = false;
			foreach (var def in m_definitions)
			{
				if (def.Value == spawnGroup)
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
