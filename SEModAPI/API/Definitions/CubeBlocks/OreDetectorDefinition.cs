using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class OreDetectorDefinition : ObjectOverLayerDefinition<MyObjectBuilder_OreDetectorDefinition>
	{
		#region "Constructors and Initializers"

		public OreDetectorDefinition(MyObjectBuilder_OreDetectorDefinition definition)
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
		/// The current Ore Detector maximum range
		/// </summary>
		public float RequiredPowerInput
		{
			get { return m_baseDefinition.MaximumRange; }
			set
			{
				if (m_baseDefinition.MaximumRange.Equals(value)) return;
				m_baseDefinition.MaximumRange = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_OreDetectorDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class OreDetectorDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_OreDetectorDefinition, OreDetectorDefinition>
	{
		#region "Constructors and Initializers"

		public OreDetectorDefinitionsManager(MyObjectBuilder_OreDetectorDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override OreDetectorDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_OreDetectorDefinition definition)
		{
			return new OreDetectorDefinition(definition);
		}

		protected override MyObjectBuilder_OreDetectorDefinition GetBaseTypeOf(OreDetectorDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(OreDetectorDefinition overLayer)
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