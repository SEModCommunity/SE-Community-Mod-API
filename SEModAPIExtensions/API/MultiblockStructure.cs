using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SEModAPIInternal.API.Entity.Sector.SectorObject;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid;
using SEModAPIInternal.API.Entity.Sector.SectorObject.CubeGrid.CubeBlock;
using SEModAPIInternal.Support;

using VRageMath;

namespace SEModAPIExtensions.API
{
	public abstract class MultiblockStructure
	{
		#region "Attributes"

		protected Dictionary<Vector3I, Type> m_definition;
		protected List<CubeBlockEntity> m_blocks;
		protected CubeBlockEntity m_anchor;
		protected CubeGridEntity m_parent;

		#endregion

		#region "Constructors and Initializers"

		public MultiblockStructure(CubeGridEntity parent)
		{
			m_parent = parent;
			m_definition = new Dictionary<Vector3I, Type>();
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
				bool isFunctional = true;

				foreach (CubeBlockEntity cubeBlock in m_blocks)
				{
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

				var def = GetMultiblockDefinition();
				foreach (Vector3I key in def.Keys)
				{
					Type type = def[key];

					CubeBlockEntity matchingBlock = null;
					foreach (CubeBlockEntity cubeBlock in m_blocks)
					{
						Vector3I relativePosition = (Vector3I)cubeBlock.Min - (Vector3I)m_anchor.Min;
						if (cubeBlock.GetType() == type && relativePosition == key)
						{
							matchingBlock = cubeBlock;
							break;
						}
					}
					if (matchingBlock == null)
					{
						isFunctional = false;
						break;
					}
				}

				return isFunctional;
			}
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

		public void LoadBlocksFromAnchor(Vector3I anchor)
		{
			m_anchor = Parent.GetCubeBlock(anchor);

			var def = GetMultiblockDefinition();
			foreach (Vector3I key in def.Keys)
			{
				m_blocks.Add(Parent.GetCubeBlock(anchor + key));
			}
		}

		public List<Vector3I> GetDefinitionMatches(CubeGridEntity cubeGrid)
		{
			try
			{
				List<Vector3I> anchorList = new List<Vector3I>();

				Dictionary<Vector3I, Type> def = GetMultiblockDefinition();
				Type anchorType = def[Vector3I.Zero];
				foreach (CubeBlockEntity cubeBlock in cubeGrid.CubeBlocks)
				{
					if (cubeBlock.GetType() == anchorType)
					{
						bool isMatch = true;
						foreach (Vector3I key in def.Keys)
						{
							Type entry = def[key];
							CubeBlockEntity posCubeBlock = cubeGrid.GetCubeBlock(cubeBlock.Min + key);
							if (posCubeBlock == null)
							{
								isMatch = false;
								break;
							}
							if (!entry.IsAssignableFrom(posCubeBlock.GetType()))
							{
								isMatch = false;
								break;
							}
						}

						if (isMatch)
						{
							anchorList.Add(cubeBlock.Min);
						}
					}
				}

				return anchorList;
			}
			catch (Exception ex)
			{
				LogManager.GameLog.WriteLine(ex);
				return new List<Vector3I>();
			}
		}

		public abstract Dictionary<Vector3I, Type> GetMultiblockDefinition();

		#endregion
	}
}
