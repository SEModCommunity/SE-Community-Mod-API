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
				try
				{
					bool isFunctional = true;

					foreach (CubeBlockEntity cubeBlock in m_blocks)
					{
						if (cubeBlock == null || cubeBlock.IsDisposed)
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
				catch (Exception ex)
				{
					LogManager.GameLog.WriteLine(ex);
					return false;
				}
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
			if (recurseDepth < 0 || recurseDepth > 2)
				return false;

			bool isMatch = true;
			Vector3I cubePos = cubeToCheck.Min;
			Type cubeType = cubeToCheck.GetType();

			while (cubeToCheck.Parent.IsLoading)
			{
				Thread.Sleep(15);
			}

			Dictionary<Vector3I, Type> def = GetMultiblockDefinition();
			Type anchorType = def[Vector3I.Zero];

			//Check if this block isn't the anchor type
			if (cubeType != anchorType)
			{
				//Check if this block is anywhere in the definition
				if (!def.ContainsValue(cubeType))
					return false;

				//Recursively search through possible anchor blocks
				isMatch = false;
				foreach (Vector3I key in def.Keys)
				{
					Type entry = def[key];
					if (key == Vector3I.Zero)
						continue;

					if (cubeType == entry)
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
				foreach (Vector3I key in def.Keys)
				{
					Type entry = def[key];
					Vector3I defPos = cubePos + key;
					CubeBlockEntity posCubeBlock = cubeToCheck.Parent.GetCubeBlock(defPos);
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
			}

			if (isMatch && SandboxGameAssemblyWrapper.IsDebugging)
			{
				LogManager.APILog.WriteLineAndConsole("Found multiblock match in cube grid '" + cubeToCheck.Parent.Name + "' at " + cubePos.ToString());
			}

			return isMatch;
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
					if(IsDefinitionMatch(cubeBlock))
						anchorList.Add(cubeBlock.Min);
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
