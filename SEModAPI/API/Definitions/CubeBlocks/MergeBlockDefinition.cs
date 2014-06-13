using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class MergeBlockDefinition : ObjectOverLayerDefinition<MyObjectBuilder_MergeBlockDefinition>
	{
		#region "Constructors and Initializers"

		public MergeBlockDefinition(MyObjectBuilder_MergeBlockDefinition definition)
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
		/// The current Merge Block Strength
		/// </summary>
		public float Strength
		{
			get { return m_baseDefinition.Strength; }
			set
			{
				if (m_baseDefinition.Strength.Equals(value)) return;
				m_baseDefinition.Strength = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_MergeBlockDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class MergeBlockDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_MergeBlockDefinition, MergeBlockDefinition>
	{
		#region "Constructors and Initializers"

		public MergeBlockDefinitionsManager(MyObjectBuilder_MergeBlockDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override MergeBlockDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_MergeBlockDefinition definition)
		{
			return new MergeBlockDefinition(definition);
		}

		protected override MyObjectBuilder_MergeBlockDefinition GetBaseTypeOf(MergeBlockDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(MergeBlockDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}