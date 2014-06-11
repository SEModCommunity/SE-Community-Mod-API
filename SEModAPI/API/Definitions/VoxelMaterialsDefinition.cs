using System;
using System.Collections.Generic;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using SEModAPI.Support;

namespace SEModAPI.API.Definitions
{
	public class VoxelMaterialsDefinition : BaseDefinition<MyObjectBuilder_VoxelMaterialDefinition>
	{
		#region "Constructors and Initializers"

		public VoxelMaterialsDefinition(MyObjectBuilder_VoxelMaterialDefinition definition)
			: base(definition)
		{
		}

		#endregion

		#region "Properties"

		public string Name
		{
			get { return m_definition.Name; }
		}

		public string MinedOre
		{
			get { return m_definition.MinedOre; }
		}

		#endregion
	}

	public class VoxelMaterialDefinitionsWrapper : BaseDefinitionsWrapper<MyObjectBuilder_VoxelMaterialDefinition, VoxelMaterialsDefinition>
	{
		#region "Constructors and Initializers"

		public VoxelMaterialDefinitionsWrapper(MyObjectBuilder_VoxelMaterialDefinition[] definitions)
			: base(definitions)
		{
		}

		#endregion

		#region "Properties"

		new public bool Changed
		{
			get
			{
				foreach (var def in m_definitions)
				{
					if (def.Value.Changed)
						return true;
				}

				return false;
			}
			set
			{
				base.Changed = value;
			}
		}

		public MyObjectBuilder_VoxelMaterialDefinition[] RawDefinitions
		{
			get
			{
				MyObjectBuilder_VoxelMaterialDefinition[] temp = new MyObjectBuilder_VoxelMaterialDefinition[m_definitions.Count];
				VoxelMaterialsDefinition[] definitionsArray = this.Definitions;

				for (int i = 0; i < definitionsArray.Length; i++)
				{
					temp[i] = definitionsArray[i].Definition;
				}

				return temp;
			}
		}

		#endregion

		#region "Methods"

		public void Save()
		{
			if (!this.Changed) return;

			m_configSerializer.VoxelMaterialDefinitions = this.RawDefinitions;
			m_configSerializer.SaveVoxelMaterialsContentFile();
		}

		#endregion
	}
}
