using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class CargoContainerDefinition : ObjectOverLayerDefinition<MyObjectBuilder_CargoContainerDefinition>
	{
		#region "Constructors and Initializers"

		public CargoContainerDefinition(MyObjectBuilder_CargoContainerDefinition definition)
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
		/// The current Cargo Container Inventory Size
		/// </summary>
		public VRageMath.Vector3 InventorySize
		{
			get { return m_baseDefinition.InventorySize; }
			set
			{
				if (m_baseDefinition.InventorySize == value) return;
				m_baseDefinition.InventorySize = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_CargoContainerDefinition definition)
		{
			return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
		}

		#endregion


	}

	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////////////

	public class CargoContainerDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_CargoContainerDefinition, CargoContainerDefinition>
	{
		#region "Constructors and Initializers"

		public CargoContainerDefinitionsManager(MyObjectBuilder_CargoContainerDefinition[] definitions)
			: base(definitions)
		{ }

		#endregion

		#region "Methods"

		protected override CargoContainerDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_CargoContainerDefinition definition)
		{
			return new CargoContainerDefinition(definition);
		}

		protected override MyObjectBuilder_CargoContainerDefinition GetBaseTypeOf(CargoContainerDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(CargoContainerDefinition overLayer)
		{
			return overLayer.Changed;
		}

		#endregion
	}
}