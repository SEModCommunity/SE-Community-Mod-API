using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class GravityGeneratorDefinition : ObjectOverLayerDefinition<MyObjectBuilder_GravityGeneratorDefinition>
	{
		#region "Constructors and Initializers"

		public GravityGeneratorDefinition(MyObjectBuilder_GravityGeneratorDefinition definition)
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

		protected override string GetNameFrom(MyObjectBuilder_GravityGeneratorDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion


	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class GravityGeneratorDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_GravityGeneratorDefinition, GravityGeneratorDefinition>
	{
		#region "Constructors and Initializers"

		public GravityGeneratorDefinitionsManager(MyObjectBuilder_GravityGeneratorDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override GravityGeneratorDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_GravityGeneratorDefinition definition)
		{
			return new GravityGeneratorDefinition(definition);
		}

		protected override MyObjectBuilder_GravityGeneratorDefinition GetBaseTypeOf(GravityGeneratorDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(GravityGeneratorDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}
