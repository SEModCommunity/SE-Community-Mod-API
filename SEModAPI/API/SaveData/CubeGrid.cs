using System;
using System.Collections.Generic;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

using SEModAPI.API.Definitions;

namespace SEModAPI.API.SaveData
{
	public class CubeGrid : SectorObject<MyObjectBuilder_CubeGrid>
	{
		#region "Attributes"

		private CubeBlockManager m_cubeBlockManager;

		#endregion

		#region "Constructors and Initializers"

		public CubeGrid(MyObjectBuilder_CubeGrid definition)
			: base(definition)
		{
			m_cubeBlockManager = new CubeBlockManager(definition.CubeBlocks);
		}

		#endregion

		#region "Properties"

		public MyCubeSize GridSizeEnum
		{
			get { return m_baseDefinition.GridSizeEnum; }
			set
			{
				if (m_baseDefinition.GridSizeEnum == value) return;
				m_baseDefinition.GridSizeEnum = value;
				Changed = true;
			}
		}

		public bool IsStatic
		{
			get { return m_baseDefinition.IsStatic; }
			set
			{
				if (m_baseDefinition.IsStatic == value) return;
				m_baseDefinition.IsStatic = value;
				Changed = true;
			}
		}

		public VRageMath.Vector3 LinearVelocity
		{
			get { return m_baseDefinition.LinearVelocity; }
			set
			{
				if (m_baseDefinition.LinearVelocity == value) return;
				m_baseDefinition.LinearVelocity = value;
				Changed = true;
			}
		}

		public VRageMath.Vector3 AngularVelocity
		{
			get { return m_baseDefinition.AngularVelocity; }
			set
			{
				if (m_baseDefinition.AngularVelocity == value) return;
				m_baseDefinition.AngularVelocity = value;
				Changed = true;
			}
		}

		public List<CubeBlock> CubeBlocks
		{
			get
			{
				//TODO - Look into changing manager base class to return a List so we don't have to do the array conversion
				List<CubeBlock> newList = new List<CubeBlock>(m_cubeBlockManager.Definitions);
				return newList;
			}
		}

		public List<BoneInfo> Skeleton
		{
			get { return m_baseDefinition.Skeleton; }
		}

		public List<MyObjectBuilder_ConveyorLine> ConveyorLines
		{
			get { return m_baseDefinition.ConveyorLines; }
		}

		public List<MyObjectBuilder_BlockGroup> BlockGroups
		{
			get { return m_baseDefinition.BlockGroups; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Calculate the name of the cube grid by concatenating beacon text
		/// </summary>
		/// <param name="definition"></param>
		/// <returns></returns>
		protected override string GetNameFrom(MyObjectBuilder_CubeGrid definition)
		{
			string name = "";
			foreach (var cubeBlock in definition.CubeBlocks)
			{
				if (cubeBlock.TypeId == MyObjectBuilderTypeEnum.Beacon)
				{
					if (name.Length > 0)
						name += "|";
					name += ((MyObjectBuilder_Beacon)cubeBlock).CustomName;
				}
			}
			if (name.Length == 0)
				return definition.EntityId.ToString();
			else
				return name;
		}

		#endregion
	}

	public class CubeGridManager : OverLayerDefinitionsManager<MyObjectBuilder_CubeGrid, CubeGrid>
	{
		#region "Constructors and Initializers"

		public CubeGridManager(List<MyObjectBuilder_CubeGrid> definitions)
			: base(definitions.ToArray())
		{}

		public CubeGridManager(MyObjectBuilder_CubeGrid[] definitions)
			: base(definitions)
		{}

		#endregion

		#region "Methods"

		protected override CubeGrid CreateOverLayerSubTypeInstance(MyObjectBuilder_CubeGrid definition)
		{
			return new CubeGrid(definition);
		}

		protected override MyObjectBuilder_CubeGrid GetBaseTypeOf(CubeGrid overLayer)
		{
			return overLayer.BaseDefinition;
		}

		protected override bool GetChangedState(CubeGrid overLayer)
		{
			foreach (var def in overLayer.CubeBlocks)
			{
				if (def.Changed)
					return true;
			}

			return overLayer.Changed;
		}

		public override void Save()
		{
			//TODO - Implement save mechanism for cube grids
		}

		#endregion
	}
}
