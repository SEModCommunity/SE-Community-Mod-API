using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class SolarPanelDefinition : ObjectOverLayerDefinition<MyObjectBuilder_SolarPanelDefinition>
	{
		#region "Constructors and Initializers"

		public SolarPanelDefinition(MyObjectBuilder_SolarPanelDefinition definition)
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
		/// The current Assembler panel offset
		/// </summary>
		public float PanelOffset
		{
			get { return m_baseDefinition.PanelOffset; }
			set
			{
				if (m_baseDefinition.PanelOffset.Equals(value)) return;
				m_baseDefinition.PanelOffset = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Solar Panel Orientation
		/// </summary>
		public VRageMath.Vector3 PanelOrientation
		{
			get { return m_baseDefinition.PanelOrientation; }
			set
			{
				if (m_baseDefinition.PanelOrientation.Equals(value)) return;
				m_baseDefinition.PanelOrientation = value;
				Changed = true;
			}
		}

		/// <summary>
		/// Get the current Solar panel "two sided" property
		/// </summary>
		public bool TwoSidedPanel
		{
			get { return m_baseDefinition.TwoSidedPanel; }
			set
			{
				if (m_baseDefinition.TwoSidedPanel == value) return;
				m_baseDefinition.TwoSidedPanel = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_SolarPanelDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class SolarPanelDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_SolarPanelDefinition, SolarPanelDefinition>
	{
		#region "Constructors and Initializers"

		public SolarPanelDefinitionsManager(MyObjectBuilder_SolarPanelDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override SolarPanelDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_SolarPanelDefinition definition)
		{
			return new SolarPanelDefinition(definition);
		}

		protected override MyObjectBuilder_SolarPanelDefinition GetBaseTypeOf(SolarPanelDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(SolarPanelDefinition overLayer)
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