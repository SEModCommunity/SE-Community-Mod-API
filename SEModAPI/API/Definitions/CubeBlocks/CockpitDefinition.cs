using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class CockpitDefinition : ObjectOverLayerDefinition<MyObjectBuilder_CockpitDefinition>
	{
		#region "Constructors and Initializers"

		public CockpitDefinition(MyObjectBuilder_CockpitDefinition definition)
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
		/// The current character animation for this Cockpit
		/// </summary>
		public string CharacterAnimation
		{
			get { return m_baseDefinition.CharacterAnimation; }
			set
			{
				if (m_baseDefinition.CharacterAnimation == value) return;
				m_baseDefinition.CharacterAnimation = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Set or Get the possibility to enable first person
		/// </summary>
		public bool EnableFirstPerson
		{
			get { return m_baseDefinition.EnableFirstPerson; }
			set
			{
				if (m_baseDefinition.EnableFirstPerson == value) return;
				m_baseDefinition.EnableFirstPerson = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Cockpit glass model
		/// </summary>
		public string GlassModel
		{
			get { return m_baseDefinition.GlassModel; }
			set
			{
				if (m_baseDefinition.GlassModel == value) return;
				m_baseDefinition.GlassModel = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Cockpit interior model
		/// </summary>
		public string InteriorModel
		{
			get { return m_baseDefinition.InteriorModel; }
			set
			{
				if (m_baseDefinition.InteriorModel == value) return;
				m_baseDefinition.InteriorModel = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_CockpitDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion


	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class CockpitDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_CockpitDefinition, CockpitDefinition>
	{
		#region "Constructors and Initializers"

		public CockpitDefinitionsManager(MyObjectBuilder_CockpitDefinition[] definitions): base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override CockpitDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_CockpitDefinition definition)
		{
			return new CockpitDefinition(definition);
		}

		protected override MyObjectBuilder_CockpitDefinition GetBaseTypeOf(CockpitDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(CockpitDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}	
}