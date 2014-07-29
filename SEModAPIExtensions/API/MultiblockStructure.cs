using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIExtensions.API
{
	public class StructureEntry
	{
		public Type type;
		public Color color;
		public bool useOrientation;
		public MyBlockOrientation orientation;

		public StructureEntry()
		{
			type = typeof(CubeBlockEntity);
			color = new Color(0, 0, 0);
			useOrientation = false;
			orientation = MyBlockOrientation.Identity;
		}
	}

	public abstract class MultiblockStructure
	{
		#region "Attributes"

		protected Dictionary<Vector3I, StructureEntry> m_definition;
		protected List<CubeBlockEntity> m_blocks;
		protected CubeBlockEntity m_anchor;
		protected CubeGridEntity m_parent;

		#endregion

		#region "Constructors and Initializers"

		public MultiblockStructure(CubeGridEntity parent)
		{
			m_parent = parent;
			m_definition = new Dictionary<Vector3I, StructureEntry>();
			m_blocks = new List<CubeBlockEntity>();
		}

		#endregion

		#region "Properties"

		public CubeGridEntity Parent
		{
			get { return m_parent; }
		}

		public bool IsFunctional
		{
			get
			{
				try
				{
					bool isFunctional = true;

					//Check if the structure is structurally sound and functional blocks are enabled
					foreach (CubeBlockEntity cubeBlock in m_blocks)
					{
						if (cubeBlock == null)
							return false;
						if (cubeBlock.IsDisposed)
							return false;

						if (cubeBlock.IntegrityPercent < 0.75)
						{
							isFunctional = false;
							break;
						}

						if (cubeBlock is FunctionalBlockEntity)
						{
							FunctionalBlockEntity functionalBlock = (FunctionalBlockEntity)cubeBlock;
							if (!functionalBlock.Enabled)
							{
								isFunctional = false;
								break;
							}
						}
					}

					//Check if the structure still matches the definition
					if (!IsDefinitionMatch(AnchorBlock))
						isFunctional = false;

					return isFunctional;
				}
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return false;
				}
			}
		}

		public CubeBlockEntity AnchorBlock
		{
			get { return m_anchor; }
		}

		public List<CubeBlockEntity> Blocks
		{
			get { return m_blocks; }
		}

		public BoundingBox Bounds
		{
			get
			{
				Vector3I min = Vector3I.Zero;
				Vector3I max = Vector3I.Zero;

				foreach (FunctionalBlockEntity cubeBlock in m_blocks)
				{
					if (((Vector3I)cubeBlock.Min).CompareTo(min) < 0)
						min = cubeBlock.Min;
					if (((Vector3I)cubeBlock.Min).CompareTo(max) > 0)
						max = cubeBlock.Min;
				}

				BoundingBox bounds = new BoundingBox(min, max);

				return bounds;
			}
		}

		#endregion

		#region "Methods"

		public void LoadBlocks()
		{
			var def = GetMultiblockDefinition();
			m_blocks.Clear();
			foreach (Vector3I key in def.Keys)
			{
				Vector3I blockPos = m_anchor.Min + key;
				CubeBlockEntity cubeBlock = Parent.GetCubeBlock(blockPos);
				if (cubeBlock == null)
				{
					m_blocks.Clear();
					return;
				}
				m_blocks.Add(cubeBlock);
			}
		}

		public void LoadBlocksFromAnchor(Vector3I anchor)
		{
			var def = GetMultiblockDefinition();
			m_blocks.Clear();
			foreach (Vector3I key in def.Keys)
			{
				Vector3I blockPos = anchor + key;
				CubeBlockEntity cubeBlock = Parent.GetCubeBlock(blockPos);
				if (cubeBlock == null)
				{
					m_blocks.Clear();
					return;
				}
				m_blocks.Add(cubeBlock);
			}
		}

		public bool IsDefinitionMatch(CubeBlockEntity cubeToCheck, int recurseDepth = 0)
		{
			if (cubeToCheck == null)
				return false;
			if (cubeToCheck.IsDisposed)
				return false;
			if (cubeToCheck.Parent == null)
				return false;
			if (cubeToCheck.Parent.IsDisposed)
				return false;
			if (cubeToCheck.Parent.IsLoading)
				return false;
			if (recurseDepth < 0 || recurseDepth > 2)
				return false;

			bool isMatch = false;
			Vector3I cubePos = cubeToCheck.Min;
			Type cubeType = cubeToCheck.GetType();

			Dictionary<Vector3I, StructureEntry> structureDef = GetMultiblockDefinition();
			StructureEntry anchorDef = structureDef[Vector3I.Zero];

			//Check if this block isn't the anchor type
			if (cubeType != anchorDef.type)
			{
				//Check if this block is anywhere in the definition
				bool foundMatch = false;
				foreach (StructureEntry entry in structureDef.Values)
				{
					if (entry.type == cubeType)
					{
						foundMatch = true;
						break;
					}
				}
				if (!foundMatch)
					return false;

				//Recursively search through possible anchor blocks
				foreach (Vector3I key in structureDef.Keys)
				{
					StructureEntry entry = structureDef[key];

					if (cubeType == entry.type)
					{
						Vector3I possibleAnchorPos = cubePos - key;
						CubeBlockEntity posCubeBlock = cubeToCheck.Parent.GetCubeBlock(possibleAnchorPos);
						isMatch = IsDefinitionMatch(posCubeBlock, recurseDepth + 1);
						if (isMatch)
							break;
					}
				}
			}
			else
			{
				isMatch = true;
				foreach (Vector3I key in structureDef.Keys)
				{
					StructureEntry entry = structureDef[key];
					Vector3I defPos = cubePos + key;
					CubeBlockEntity posCubeBlock = cubeToCheck.Parent.GetCubeBlock(defPos);
					if (posCubeBlock == null)
					{
						isMatch = false;
						break;
					}

					//Compare the block type
					if (!entry.type.IsAssignableFrom(posCubeBlock.GetType()))
					{
						isMatch = false;
						break;
					}

					//Compare the block color, if set
					if (entry.color != null && entry.color.ColorToHSV() != Vector3.Zero)
					{
						if (entry.color.ColorToHSV() != (Vector3)posCubeBlock.ColorMaskHSV)
						{
							isMatch = false;
							break;
						}
					}

					//Compare the block orientation, if set
					if (entry.useOrientation)
					{
						if (entry.orientation.Forward != posCubeBlock.BlockOrientation.Forward || entry.orientation.Up != posCubeBlock.BlockOrientation.Up)
						{
							isMatch = false;
							break;
						}
					}
				}
				if(isMatch)
					m_anchor = cubeToCheck;
			}

			if (isMatch && SandboxGameAssemblyWrapper.IsDebugging && recurseDepth == 0)
			{
				//LogManager.APILog.WriteLine("Found multiblock match in cube grid '" + cubeToCheck.Parent.Name + "' anchored at " + ((Vector3I)m_anchor.Min).ToString());
			}

			return isMatch;
		}

		public List<Vector3I> GetDefinitionMatches(CubeGridEntity cubeGrid)
		{
			try
			{
				List<Vector3I> anchorList = new List<Vector3I>();
				foreach (CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks)
				{
					if(IsDefinitionMatch(cubeBlock))
						anchorList.Add(m_anchor.Min);
				}

				return anchorList;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return new List<Vector3I>();
			}
		}

		public abstract Dictionary<Vector3I, StructureEntry> GetMultiblockDefinition();

		#endregion
	}
}
