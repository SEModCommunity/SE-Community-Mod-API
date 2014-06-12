using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class ThrusterDefinition : ObjectOverLayerDefinition<MyObjectBuilder_ThrustDefinition>
	{
		#region "Constructors and Initializers"

		public ThrusterDefinition(MyObjectBuilder_ThrustDefinition definition)
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
		/// The current Thruster Damage Area Size
		/// </summary>
		public float DamageAreaSize
		{
			get { return m_baseDefinition.DamageAreaSize; }
			set
			{
				if (m_baseDefinition.DamageAreaSize == value) return;
				m_baseDefinition.DamageAreaSize = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Thruster flame scale, or "Flame volume"
		/// </summary>
		public float FlameScale
		{
			get { return m_baseDefinition.FlameScale; }
			set
			{
				if (m_baseDefinition.FlameScale == value) return;
				m_baseDefinition.FlameScale = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Thruster force magnitude, or "push capacity"
		/// </summary>
		public float ForceMagnitude
		{
			get { return m_baseDefinition.ForceMagnitude; }
			set
			{
				if (m_baseDefinition.ForceMagnitude == value) return;
				m_baseDefinition.ForceMagnitude = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Thruster maximum power consumption
		/// </summary>
		public float MaxPowerConsumption
		{
			get { return m_baseDefinition.MaxPowerConsumption; }
			set
			{
				if (m_baseDefinition.MaxPowerConsumption == value) return;
				m_baseDefinition.MaxPowerConsumption = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Thruster minimum power consumption
		/// </summary>
		public float MinPowerConsumption
		{
			get { return m_baseDefinition.MinPowerConsumption; }
			set
			{
				if (m_baseDefinition.MinPowerConsumption == value) return;
				m_baseDefinition.MinPowerConsumption = value;
				Changed = true;
			}
		}

		/// <summary>
		/// The current Thruster movement coefficient
		/// </summary>
		public float MovementCoefficient
		{
			get { return m_baseDefinition.MovementCoefficient; }
			set
			{
				if (m_baseDefinition.MovementCoefficient == value) return;
				m_baseDefinition.MovementCoefficient = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_ThrustDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class ThrusterDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_ThrustDefinition, ThrusterDefinition>
	{
		#region "Constructors and Initializers"

		public ThrusterDefinitionsManager(MyObjectBuilder_ThrustDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override ThrusterDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_ThrustDefinition definition)
		{
			return new ThrusterDefinition(definition);
		}

		protected override MyObjectBuilder_ThrustDefinition GetBaseTypeOf(ThrusterDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(ThrusterDefinition overLayer)
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