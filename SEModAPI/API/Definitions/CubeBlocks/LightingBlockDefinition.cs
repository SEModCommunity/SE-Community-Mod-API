using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class LightingBlockDefinition : ObjectOverLayerDefinition<MyObjectBuilder_LightingBlockDefinition>
	{
		#region "Constructors and Initializers"

		public LightingBlockDefinition(MyObjectBuilder_LightingBlockDefinition definition)
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
		/// The current Assembler required power input
		/// </summary>
		public float RequiredPowerInput
		{
			get { return m_baseDefinition.RequiredPowerInput; }
			set
			{
				if (m_baseDefinition.RequiredPowerInput.Equals(value)) return;
				m_baseDefinition.RequiredPowerInput = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Lithing block Max light fall off
		/// </summary>
		public float MaxLightFalloff
		{
			get { return m_baseDefinition.LightFalloff.Max; }
			set
			{
				if (m_baseDefinition.LightFalloff.Max.Equals(value)) return;
				m_baseDefinition.LightFalloff.Max = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Lithing block Min light fall off
		/// </summary>
		public float MinLightFalloff
		{
			get { return m_baseDefinition.LightFalloff.Min; }
			set
			{
				if (m_baseDefinition.LightFalloff.Min.Equals(value)) return;
				m_baseDefinition.LightFalloff.Min = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Lithing block Default light fall off
		/// </summary>
		public float DefaultLightFalloff
		{
			get { return m_baseDefinition.LightFalloff.Default; }
			set
			{
				if (m_baseDefinition.LightFalloff.Default.Equals(value)) return;
				m_baseDefinition.LightFalloff.Default = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_LightingBlockDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class LightingBlockDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_LightingBlockDefinition, LightingBlockDefinition>
	{
		#region "Constructors and Initializers"

		public LightingBlockDefinitionsManager(MyObjectBuilder_LightingBlockDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override LightingBlockDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_LightingBlockDefinition definition)
		{
			return new LightingBlockDefinition(definition);
		}

		protected override MyObjectBuilder_LightingBlockDefinition GetBaseTypeOf(LightingBlockDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(LightingBlockDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}
