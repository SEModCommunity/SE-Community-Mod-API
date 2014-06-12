using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class RefineryDefinition : ObjectOverLayerDefinition<MyObjectBuilder_RefineryDefinition>
	{
		#region "Constructors and Initializers"

		public RefineryDefinition(MyObjectBuilder_RefineryDefinition definition)
			: base(definition)
		{ }

		#endregion

		#region "Properties"

		/// <summary>
		/// Get or set the current Thruster build time in second.
		/// </summary>
		public float BuildTime
		{
			get { return m_baseDefinition.BuildTimeSeconds; }
			set
			{
				if (m_baseDefinition.BuildTimeSeconds == value) return;
				m_baseDefinition.BuildTimeSeconds = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Get or Set the current Thruster DisassembleRatio
		/// The value is a multiplyer of BuildTime
		/// [Disassemble time] = BuildTime * DisassembleRatio
		/// </summary>
		public float DisassembleRatio
		{
			get { return m_baseDefinition.DisassembleRatio; }
			set
			{
				if (m_baseDefinition.DisassembleRatio == value) return;
				m_baseDefinition.DisassembleRatio = value;
				Changed = true;
			}
		}
		public MyObjectBuilder_CubeBlockDefinition.CubeBlockComponent[] Components
		{
			get { return m_baseDefinition.Components; }
		}

		/// <summary>
		/// The activation state of the current Thruster
		/// </summary>
		public bool Enabled
		{
			get { return m_baseDefinition.Public; }
			set
			{
				if (m_baseDefinition.Public == value) return;
				m_baseDefinition.Public = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The Model intersection state of the current Thruster 
		/// </summary>
		public bool UseModelIntersection
		{
			get { return m_baseDefinition.UseModelIntersection; }
			set
			{
				if (m_baseDefinition.UseModelIntersection == value) return;
				m_baseDefinition.UseModelIntersection = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Refinery materiel efficiency
		/// </summary>
		public float MaterialEfficiency
		{
			get { return m_baseDefinition.MaterialEfficiency; }
			set
			{
				if (m_baseDefinition.MaterialEfficiency.Equals(value)) return;
				m_baseDefinition.MaterialEfficiency = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Refinery Refine Speed
		/// </summary>
		public float RefineSpeed
		{
			get { return m_baseDefinition.RefineSpeed; }
			set
			{
				if (m_baseDefinition.RefineSpeed.Equals(value)) return;
				m_baseDefinition.RefineSpeed = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_RefineryDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class RefineryDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_RefineryDefinition, RefineryDefinition>
	{
		#region "Constructors and Initializers"

		public RefineryDefinitionsManager(MyObjectBuilder_RefineryDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override RefineryDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_RefineryDefinition definition)
		{
			return new RefineryDefinition(definition);
		}

		protected override MyObjectBuilder_RefineryDefinition GetBaseTypeOf(RefineryDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(RefineryDefinition overLayer)
		{
			return overLayer.Changed;
		}

		public override void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.CubeBlockDefinitions = this.ExtractBaseDefinitions().ToArray();
			m_configSerializer.SaveCubeBlocksContentFile();
		}

		#endregion
	}
}