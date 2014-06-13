using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class VirtualMassDefinition : ObjectOverLayerDefinition<MyObjectBuilder_VirtualMassDefinition>
	{
		#region "Constructors and Initializers"

		public VirtualMassDefinition(MyObjectBuilder_VirtualMassDefinition definition)
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
		/// The current Virtual Mass "mass"
		/// </summary>
		public float MaxLightFalloff
		{
			get { return m_baseDefinition.VirtualMass; }
			set
			{
				if (m_baseDefinition.VirtualMass.Equals(value)) return;
				m_baseDefinition.VirtualMass = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_VirtualMassDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class VirtualMassDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_VirtualMassDefinition, VirtualMassDefinition>
	{
		#region "Constructors and Initializers"

		public VirtualMassDefinitionsManager(MyObjectBuilder_VirtualMassDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override VirtualMassDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_VirtualMassDefinition definition)
		{
			return new VirtualMassDefinition(definition);
		}

		protected override MyObjectBuilder_VirtualMassDefinition GetBaseTypeOf(VirtualMassDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(VirtualMassDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}
