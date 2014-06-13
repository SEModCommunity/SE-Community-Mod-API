using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class GyroscopeDefinition : ObjectOverLayerDefinition<MyObjectBuilder_GyroDefinition>
	{
		#region "Constructors and Initializers"

		public GyroscopeDefinition(MyObjectBuilder_GyroDefinition definition)
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
		/// The current Gravity generator required power input
		/// </summary>
		public float RequiredPowerInput
		{
			get { return m_baseDefinition.RequiredPowerInput; }
			set
			{
				if (m_baseDefinition.RequiredPowerInput == value) return;
				m_baseDefinition.RequiredPowerInput = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_GyroDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion


	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class GyroscopeDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_GyroDefinition, GyroscopeDefinition>
	{
		#region "Constructors and Initializers"

		public GyroscopeDefinitionsManager(MyObjectBuilder_GyroDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override GyroscopeDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_GyroDefinition definition)
		{
			return new GyroscopeDefinition(definition);
		}

		protected override MyObjectBuilder_GyroDefinition GetBaseTypeOf(GyroscopeDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(GyroscopeDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}