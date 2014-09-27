using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders;
using System;

namespace SEModAPI.API.Definitions
{
	public class BlueprintsDefinition : OverLayerDefinition<MyObjectBuilder_BlueprintDefinition>
	{
		#region "Attributes"

		BlueprintItemsManager m_prerequisitesManager;
		BlueprintItemDefinition m_result;

		#endregion

		#region "Constructors and Initializers"

		public BlueprintsDefinition(MyObjectBuilder_BlueprintDefinition definition)
			: base(definition)
		{
			m_prerequisitesManager = new BlueprintItemsManager();

			if(definition.Result != null)
				m_result = new BlueprintItemDefinition(definition.Result);

			if (definition.Prerequisites != null)
				m_prerequisitesManager.Load(definition.Prerequisites);
		}

		#endregion

		#region "Properties"

		new public bool Changed
		{
			get
			{
				if (base.Changed) return true;
				foreach (var def in m_prerequisitesManager.Definitions)
				{
					if (def.Changed)
						return true;
				}
				return false;
			}
			private set { base.Changed = value; }
		}

		new public MyObjectBuilder_BlueprintDefinition BaseDefinition
		{
			get
			{
				m_baseDefinition.Prerequisites = m_prerequisitesManager.ExtractBaseDefinitions().ToArray();
				return m_baseDefinition;
			}
		}

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

		public BlueprintItemDefinition Result
		{
			get { return m_result; }
			set
			{
				if (m_result == value) return;
				m_result = value;
				Changed = true;
			}
		}

		public BlueprintItemDefinition[] Prerequisites
		{
			get { return m_prerequisitesManager.Definitions; }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_BlueprintDefinition definition)
		{
			return m_baseDefinition.Result.SubtypeId;
		}

		public BlueprintItemDefinition NewEntry()
		{
			return m_prerequisitesManager.NewEntry();
		}

		public bool DeleteEntry(BlueprintItemDefinition source)
		{
			return m_prerequisitesManager.DeleteEntry(source);
		}

		#endregion
	}

	public class BlueprintItemDefinition : OverLayerDefinition<BlueprintItem>
	{
		#region "Constructors and Initializers"

		public BlueprintItemDefinition(BlueprintItem definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		public string Amount
		{
			get { return m_baseDefinition.Amount; }
			set
			{
				if (m_baseDefinition.Amount == value) return;
				m_baseDefinition.Amount = value;
				Changed = true;
			}
		}

		public string TypeId
		{
			get { return m_baseDefinition.TypeId; }
			set
			{
				if (m_baseDefinition.TypeId == value) return;
				m_baseDefinition.TypeId = value;
				Changed = true;
			}
		}

		public string SubTypeId
		{
			get { return m_baseDefinition.SubtypeId; }
			set
			{
				if (m_baseDefinition.SubtypeId == value) return;
				m_baseDefinition.SubtypeId = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(BlueprintItem definition)
		{
			return definition.SubtypeId + " " + definition.TypeId;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class BlueprintDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_BlueprintDefinition, BlueprintsDefinition>
	{
		#region "Methods"

		protected override MyObjectBuilder_BlueprintDefinition GetBaseTypeOf(BlueprintsDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		#endregion
	}

	public class BlueprintItemsManager : SerializableDefinitionsManager<BlueprintItem, BlueprintItemDefinition>
	{
	}
}
