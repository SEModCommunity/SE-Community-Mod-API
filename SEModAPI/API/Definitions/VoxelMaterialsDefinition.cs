using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class VoxelMaterialsDefinition : OverLayerDefinition<MyObjectBuilder_VoxelMaterialDefinition>
	{
		#region "Constructors and Initializers"

		public VoxelMaterialsDefinition(MyObjectBuilder_VoxelMaterialDefinition definition)
			: base(definition)
		{
		}

		#endregion

		#region "Properties"

		new public string Name
		{
			get { return m_baseDefinition.Name; }
		}

		public string MinedOre
		{
			get { return m_baseDefinition.MinedOre; }
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_VoxelMaterialDefinition definition)
		{
			return definition.Name;
		}

		#endregion
	}

	public class VoxelMaterialDefinitionsManager : OverLayerDefinitionsManager<MyObjectBuilder_VoxelMaterialDefinition, VoxelMaterialsDefinition>
	{
		#region "Constructors and Initializers"

		public VoxelMaterialDefinitionsManager(MyObjectBuilder_VoxelMaterialDefinition[] definitions)
			: base(definitions)
		{
		}

		#endregion

		#region "Methods"

		protected override VoxelMaterialsDefinition CreateOverLayerSubTypeInstance(MyObjectBuilder_VoxelMaterialDefinition definition)
		{
			return new VoxelMaterialsDefinition(definition);
		}

		protected override MyObjectBuilder_VoxelMaterialDefinition GetBaseTypeOf(VoxelMaterialsDefinition overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(VoxelMaterialsDefinition overLayer)
		{
			return overLayer.Changed;
		}

		public override void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.VoxelMaterialDefinitions = this.ExtractBaseDefinitions().ToArray();
			m_configSerializer.SaveVoxelMaterialsContentFile();
		}

		#endregion
	}
}
