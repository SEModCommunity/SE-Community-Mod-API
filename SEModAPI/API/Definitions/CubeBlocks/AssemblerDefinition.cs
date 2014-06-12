using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class AssemblerDefinition : ObjectOverLayerDefinition<MyObjectBuilder_AssemblerDefinition>
	{
		#region "Constructors and Initializers"

		public AssemblerDefinition(MyObjectBuilder_AssemblerDefinition definition)
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
		/// The current Assembler operational power consumption
		/// </summary>
		public float MaxPowerConsumption
		{
			get { return m_baseDefinition.OperationalPowerConsumption; }
			set
			{
				if (m_baseDefinition.OperationalPowerConsumption == value) return;
				m_baseDefinition.OperationalPowerConsumption = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Assembler standby power consumption
		/// </summary>
		public float MinPowerConsumption
		{
			get { return m_baseDefinition.StandbyPowerConsumption; }
			set
			{
				if (m_baseDefinition.StandbyPowerConsumption == value) return;
				m_baseDefinition.StandbyPowerConsumption = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Assembler inventory max volume
		/// </summary>
		public float MovementCoefficient
		{
			get { return m_baseDefinition.InventoryMaxVolume; }
			set
			{
				if (m_baseDefinition.InventoryMaxVolume == value) return;
				m_baseDefinition.InventoryMaxVolume = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_AssemblerDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion

		
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class AssemblerDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_AssemblerDefinition, AssemblerDefinition>
	{
		#region "Constructors and Initializers"

		public AssemblerDefinitionsManager(MyObjectBuilder_AssemblerDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override AssemblerDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_AssemblerDefinition definition)
		{
			return new AssemblerDefinition(definition);
		}

		protected override MyObjectBuilder_AssemblerDefinition GetBaseTypeOf(AssemblerDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(AssemblerDefinition overLayer)
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