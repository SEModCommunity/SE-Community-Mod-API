using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class MotorStatorBlockDefinition : ObjectOverLayerDefinition<MyObjectBuilder_MotorStatorDefinition>
	{
		#region "Constructors and Initializers"

		public MotorStatorBlockDefinition(MyObjectBuilder_MotorStatorDefinition definition)
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
		/// The current Rotor or Stator Max force magnitude
		/// </summary>
		public float MaxForceMagnitude
		{
			get { return m_baseDefinition.MaxForceMagnitude; }
			set
			{
				if (m_baseDefinition.MaxForceMagnitude.Equals(value)) return;
				m_baseDefinition.MaxForceMagnitude = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_MotorStatorDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class RotorStatorDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_MotorStatorDefinition, MotorStatorBlockDefinition>
	{
		#region "Constructors and Initializers"

		public RotorStatorDefinitionsManager(MyObjectBuilder_MotorStatorDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override MotorStatorBlockDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_MotorStatorDefinition definition)
		{
			return new MotorStatorBlockDefinition(definition);
		}

		protected override MyObjectBuilder_MotorStatorDefinition GetBaseTypeOf(MotorStatorBlockDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(MotorStatorBlockDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}