using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class CubeBlock : OverLayerDefinition<MyObjectBuilder_CubeBlock>
	{
		#region "Constructors and Initializers"

		public CubeBlock(MyObjectBuilder_CubeBlock definition)
			: base(definition)
		{}

		#endregion

		#region "Properties"

		new public string Name
		{
			get { return this.GetNameFrom(m_baseDefinition); }
		}

		public SerializableVector3I Min
		{
			get { return m_baseDefinition.Min; }
			set
			{
				if (m_baseDefinition.Min.Equals(value)) return;
				m_baseDefinition.Min = value;
				Changed = true;
			}
		}

		public SerializableBlockOrientation BlockOrientation
		{
			get { return m_baseDefinition.BlockOrientation; }
			set
			{
				if (m_baseDefinition.BlockOrientation.Equals(value)) return;
				m_baseDefinition.BlockOrientation = value;
				Changed = true;
			}
		}

		public SerializableVector3 ColorMaskHSV
		{
			get { return m_baseDefinition.ColorMaskHSV; }
			set
			{
				if (m_baseDefinition.ColorMaskHSV.Equals(value)) return;
				m_baseDefinition.ColorMaskHSV = value;
				Changed = true;
			}
		}

		#endregion

		#region "Methods"

		protected override string GetNameFrom(MyObjectBuilder_CubeBlock definition)
		{
			return m_baseDefinition.SubtypeName;
		}

		#endregion
	}

	public class CubeBlockManager : OverLayerDefinitionsManager<MyObjectBuilder_CubeBlock, CubeBlock>
	{
		#region "Constructors and Initializers"

		public CubeBlockManager(List<MyObjectBuilder_CubeBlock> definitions)
			: base(definitions.ToArray())
		{}

		public CubeBlockManager(MyObjectBuilder_CubeBlock[] definitions)
			: base(definitions)
		{}

		#endregion

		#region "Methods"

		protected override CubeBlock CreateOverLayerSubTypeInstance(MyObjectBuilder_CubeBlock definition)
		{
			return new CubeBlock(definition);
		}

		protected override MyObjectBuilder_CubeBlock GetBaseTypeOf(CubeBlock overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(CubeBlock overLayer)
		{
			return overLayer.Changed;
		}

		public override void Save()
		{
			//TODO - Implement save mechanism for cube blocks
		}

		#endregion
	}
}
