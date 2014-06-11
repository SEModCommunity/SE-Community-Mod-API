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

		private List<CubeBlock> m_cubeBlocks;

		#endregion

		#region "Constructors and Initializers"

		public CubeGrid(MyObjectBuilder_CubeGrid definition)
			: base(definition)
		{
			//TODO - Change this to use a manager rather than a flat list
			m_cubeBlocks = new List<CubeBlock>();
			foreach (MyObjectBuilder_CubeBlock cubeBlock in definition.CubeBlocks)
			{
				m_cubeBlocks.Add(new CubeBlock(cubeBlock));
			}
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
			get { return m_cubeBlocks; }
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
}
