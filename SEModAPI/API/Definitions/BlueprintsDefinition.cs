using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders;
using System;

namespace SEModAPI.API.Definitions
{
	public class BlueprintsDefinition : OverLayerDefinition<MyObjectBuilder_BlueprintDefinition>
	{
		#region "Attributes"

		BlueprintItemDefinition[] m_prerequisites;
		BlueprintItemDefinition m_result;

		#endregion

		#region "Constructors and Initializers"

		public BlueprintsDefinition(MyObjectBuilder_BlueprintDefinition definition)
			: base(definition)
		{
			m_result = new BlueprintItemDefinition(definition.Result);

			m_prerequisites = new BlueprintItemDefinition[definition.Prerequisites.Length];
			for (int i = 0; i < definition.Prerequisites.Length; i++)
			{
				m_prerequisites[i] = new BlueprintItemDefinition(definition.Prerequisites[i]);
			}
		}

		#endregion

		#region "Properties"

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
		}

		public BlueprintItemDefinition[] Prerequisites
		{
			get { return m_prerequisites; }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_BlueprintDefinition definition)
		{
			return m_baseDefinition.Result.SubtypeId;
		}

		#endregion
	}

	public class BlueprintItemDefinition : OverLayerDefinition<MyObjectBuilder_BlueprintDefinition.Item>
	{
		#region "Constructors and Initializers"

		public BlueprintItemDefinition(MyObjectBuilder_BlueprintDefinition.Item definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		public decimal Amount
		{
			get { return m_baseDefinition.Amount; }
			set
			{
				if (value < 0) throw new ArgumentOutOfRangeException();
				if (m_baseDefinition.Amount == value) return;
				m_baseDefinition.Amount = value;
				Changed = true;
			}
		}

		public MyObjectBuilderTypeEnum TypeId
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

		new public string Name
		{
			get { return base.Name; }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_BlueprintDefinition.Item definition)
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
	}
}
