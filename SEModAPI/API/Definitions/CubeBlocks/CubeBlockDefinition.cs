using Sandbox.Common.ObjectBuilders.Definitions;
using System.IO;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	public class CubeBlockDefinition : ObjectOverLayerDefinition<MyObjectBuilder_CubeBlockDefinition>
    {
		#region "Constructors and Initializers"

		public CubeBlockDefinition(MyObjectBuilder_CubeBlockDefinition definition): base(definition)
		{}

		#endregion

        #region "Properties"

        /// <summary>
        /// Get or set the current CubeBlock build time in second.
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
        /// Get or Set the current CubeBlock DisassembleRatio
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
        /// The activation state of the current CubeBlock
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
        /// The Model intersection state of the current CubeBlock 
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

	    #endregion

        #region "Methods"

        protected override string GetNameFrom(MyObjectBuilder_CubeBlockDefinition definition)
        {
            return definition.Id.SubtypeName == "" ? definition.Id.TypeId.ToString() : definition.Id.SubtypeName;
        }

        #endregion

		#region "Utility"
		/*
		#region "CubeOrientations"

		public readonly Dictionary<CubeType, SerializableBlockOrientation> CubeOrientations = new Dictionary<CubeType, SerializableBlockOrientation>()
		{
			// TODO: Remove the Cube Armor orientation, as these appear to work fine with the Generic.
			{CubeType.Cube, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Up)},

			// TODO: Remove the Slope Armor orientations, as these appear to work fine with the Generic.
			{CubeType.SlopeCenterBackTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Forward)}, // -90 around X
			{CubeType.SlopeRightBackCenter, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Left)},
			{CubeType.SlopeLeftBackCenter, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.SlopeCenterBackBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Up)}, // no rotation
			{CubeType.SlopeRightCenterTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Left)},
			{CubeType.SlopeLeftCenterTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.SlopeRightCenterBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Left)}, // +90 around Z
			{CubeType.SlopeLeftCenterBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Right)}, // -90 around Z
			{CubeType.SlopeCenterFrontTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Down)}, // 180 around X
			{CubeType.SlopeRightFrontCenter, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Left)},
			{CubeType.SlopeLeftFrontCenter, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.SlopeCenterFrontBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Backward)},// +90 around X

				// Probably got the names of these all messed up in relation to their actual orientation.
			{CubeType.NormalCornerLeftFrontTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.NormalCornerRightFrontTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Down)}, // 180 around X
			{CubeType.NormalCornerLeftBackTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.NormalCornerRightBackTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Forward)}, // -90 around X
			{CubeType.NormalCornerLeftFrontBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.NormalCornerRightFrontBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Backward)},// +90 around X 
			{CubeType.NormalCornerLeftBackBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Right)},// -90 around Z
			{CubeType.NormalCornerRightBackBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Up)},  // no rotation

			{CubeType.InverseCornerLeftFrontTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.InverseCornerRightFrontTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Down)}, // 180 around X
			{CubeType.InverseCornerLeftBackTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.InverseCornerRightBackTop, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Forward)},  // -90 around X
			{CubeType.InverseCornerLeftFrontBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.InverseCornerRightFrontBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Backward)}, // +90 around X
			{CubeType.InverseCornerLeftBackBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Right)}, // -90 around Z
			{CubeType.InverseCornerRightBackBottom, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Up)},  // no rotation

			// Generic, which seems to work for everything but Corner armor blocks.
			{CubeType.Axis24_Backward_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Down)},
			{CubeType.Axis24_Backward_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Left)},
			{CubeType.Axis24_Backward_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.Axis24_Backward_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Backward, VRageMath.Base6Directions.Direction.Up)},
			{CubeType.Axis24_Down_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Backward)},
			{CubeType.Axis24_Down_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Forward)},
			{CubeType.Axis24_Down_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Left)},
			{CubeType.Axis24_Down_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Down, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.Axis24_Forward_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Down)},
			{CubeType.Axis24_Forward_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Left)},
			{CubeType.Axis24_Forward_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Right)},
			{CubeType.Axis24_Forward_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Forward, VRageMath.Base6Directions.Direction.Up)},
			{CubeType.Axis24_Left_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Backward)},
			{CubeType.Axis24_Left_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Down)},
			{CubeType.Axis24_Left_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Forward)},
			{CubeType.Axis24_Left_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Left, VRageMath.Base6Directions.Direction.Up)},
			{CubeType.Axis24_Right_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Backward)},
			{CubeType.Axis24_Right_Down, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Down)},
			{CubeType.Axis24_Right_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Forward)},
			{CubeType.Axis24_Right_Up, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Right, VRageMath.Base6Directions.Direction.Up)},
			{CubeType.Axis24_Up_Backward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Backward)},
			{CubeType.Axis24_Up_Forward, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Forward)},
			{CubeType.Axis24_Up_Left, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Left)},
			{CubeType.Axis24_Up_Right, new SerializableBlockOrientation(VRageMath.Base6Directions.Direction.Up, VRageMath.Base6Directions.Direction.Right)},
		};

		#endregion

		public long GenerateEntityId()
		{
			// Not the offical SE way of generating IDs, but its fast and we don't have to worry about a random seed.
			var buffer = Guid.NewGuid().ToByteArray();
			return BitConverter.ToInt64(buffer, 0);
		}

		public SerializableBlockOrientation GetCubeOrientation(CubeType type)
		{
			if (CubeOrientations.ContainsKey(type))
				return CubeOrientations[type];

			throw new NotImplementedException(string.Format("SetCubeOrientation of type [{0}] not yet implemented.", type));
		}

		#region FetchCubeBlockMass

		public float FetchCubeBlockMass(MyObjectBuilderTypeEnum typeId, MyCubeSize cubeSize, string subTypeid)
		{
			float mass = 0;

			var cubeBlockDefinition = GetCubeDefinition(typeId, cubeSize, subTypeid);

			if (cubeBlockDefinition != null)
			{
				foreach (var component in cubeBlockDefinition.Components)
				{
					mass += m_componentDefinitions.Components.Where(c => c.Id.SubtypeId == component.Subtype).Sum(c => c.Mass) * component.Count;
				}
			}

			return mass;
		}

		public void AccumulateCubeBlueprintRequirements(string subType, MyObjectBuilderTypeEnum typeId, decimal amount, Dictionary<string, MyObjectBuilder_BlueprintDefinition.Item> requirements, out TimeSpan timeTaken)
		{
			TimeSpan time = new TimeSpan();
			var bp = m_blueprintDefinitions.Blueprints.FirstOrDefault(b => b.Result.SubtypeId == subType && b.Result.TypeId == typeId);
			if (bp != null)
			{
				foreach (var item in bp.Prerequisites)
				{
					if (requirements.ContainsKey(item.SubtypeId))
					{
						// append existing
						requirements[item.SubtypeId].Amount += (amount / bp.Result.Amount) * item.Amount;
					}
					else
					{
						// add new
						requirements.Add(item.SubtypeId, new MyObjectBuilder_BlueprintDefinition.Item()
						{
							Amount = (amount / bp.Result.Amount) * item.Amount,
							TypeId = item.TypeId,
							SubtypeId = item.SubtypeId,
							Id = item.Id
						});
					}

					var ticks = TimeSpan.TicksPerSecond * (decimal)bp.BaseProductionTimeInSeconds * amount;
					var ts = new TimeSpan((long)ticks);
					time += ts;
				}
			}

			timeTaken = time;
		}

		public MyObjectBuilder_DefinitionBase GetDefinition(MyObjectBuilderTypeEnum typeId, string subTypeId)
		{
			var cube = m_cubeBlockDefinitions.CubeBlocks.FirstOrDefault(d => d.Id.TypeId == typeId && d.Id.SubtypeId == subTypeId);
			if (cube != null)
			{
				return cube;
			}

			var item = m_physicalItemDefinitions.PhysicalItems.FirstOrDefault(d => d.Id.TypeId == typeId && d.Id.SubtypeId == subTypeId);
			if (item != null)
			{
				return item;
			}

			var component = m_componentDefinitions.Components.FirstOrDefault(c => c.Id.TypeId == typeId && c.Id.SubtypeId == subTypeId);
			if (component != null)
			{
				return component;
			}

			var magazine = m_ammoMagazineDefinitions.AmmoMagazines.FirstOrDefault(c => c.Id.TypeId == typeId && c.Id.SubtypeId == subTypeId);
			if (magazine != null)
			{
				return magazine;
			}

			return null;
		}

		public float GetItemMass(MyObjectBuilderTypeEnum typeId, string subTypeId)
		{
			var def = GetDefinition(typeId, subTypeId);
			if (def is MyObjectBuilder_PhysicalItemDefinition)
			{
				var item2 = def as MyObjectBuilder_PhysicalItemDefinition;
				return item2.Mass;
			}

			return 0;
		}

		public float GetItemVolume(MyObjectBuilderTypeEnum typeId, string subTypeId)
		{
			var def = GetDefinition(typeId, subTypeId);
			if (def is MyObjectBuilder_PhysicalItemDefinition)
			{
				var item2 = def as MyObjectBuilder_PhysicalItemDefinition;
				if (item2.Volume.HasValue)
					return item2.Volume.Value;
			}

			return 0;
		}

		public IList<MyObjectBuilder_VoxelMaterialDefinition> GetMaterialList()
		{
			return m_voxelMaterialDefinitions.VoxelMaterials;
		}

		public byte GetMaterialIndex(string materialName)
		{
			if (m_materialIndex.ContainsKey(materialName))
				return m_materialIndex[materialName];
			else
			{
				var material = m_voxelMaterialDefinitions.VoxelMaterials.FirstOrDefault(m => m.Name == materialName);
				var index = (byte)m_voxelMaterialDefinitions.VoxelMaterials.ToList().IndexOf(material);
				m_materialIndex.Add(materialName, index);
				return index;
			}
		}

		public string GetMaterialName(byte materialIndex, byte defaultMaterialIndex)
		{
			if (materialIndex <= m_voxelMaterialDefinitions.VoxelMaterials.Length)
				return m_voxelMaterialDefinitions.VoxelMaterials[materialIndex].Name;
			else
				return m_voxelMaterialDefinitions.VoxelMaterials[defaultMaterialIndex].Name;
		}

		public string GetMaterialName(byte materialIndex)
		{
			return m_voxelMaterialDefinitions.VoxelMaterials[materialIndex].Name;
		}

		public MyObjectBuilder_CubeBlockDefinition GetCubeDefinition(MyObjectBuilderTypeEnum typeId, MyCubeSize cubeSize, string subtypeId)
		{
			if (string.IsNullOrEmpty(subtypeId))
			{
				return m_cubeBlockDefinitions.CubeBlocks.FirstOrDefault(d => d.CubeSize == cubeSize && d.Id.TypeId == typeId);
			}

			return m_cubeBlockDefinitions.CubeBlocks.FirstOrDefault(d => d.Id.SubtypeId == subtypeId || (d.Variants != null && d.Variants.Any(v => subtypeId == d.Id.SubtypeId + v.Color)));
			// Returns null if it doesn't find the required SubtypeId.
		}

		public BoundingBox GetBoundingBox(MyObjectBuilder_CubeGrid entity)
		{
			var min = new Vector3(int.MaxValue, int.MaxValue, int.MaxValue);
			var max = new Vector3(int.MinValue, int.MinValue, int.MinValue);

			foreach (var block in entity.CubeBlocks)
			{
				min.X = Math.Min(min.X, block.Min.X);
				min.Y = Math.Min(min.Y, block.Min.Y);
				min.Z = Math.Min(min.Z, block.Min.Z);
				max.X = Math.Max(max.X, block.Min.X);       // TODO: resolve cubetype size.
				max.Y = Math.Max(max.Y, block.Min.Y);
				max.Z = Math.Max(max.Z, block.Min.Z);
			}

			// scale box to GridSize
			var size = max - min;
			if (entity.GridSizeEnum == MyCubeSize.Large)
			{
				size = new Vector3(size.X * 2.5f, size.Y * 2.5f, size.Z * 2.5f);
			}
			else if (entity.GridSizeEnum == MyCubeSize.Small)
			{
				size = new Vector3(size.X * 0.5f, size.Y * 0.5f, size.Z * 0.5f);
			}

			// translate box according to min/max, but reset origin.
			var bb = new BoundingBox(new Vector3(0, 0, 0), size);

			// TODO: translate for rotation.
			//bb. ????

			// translate position.
			bb.Translate(entity.PositionAndOrientation.Value.Position);


			return bb;
		}

		public string GetResourceName(string value)
		{
			if (value == null)
				return null;

			Sandbox.Common.Localization.MyTextsWrapperEnum myText;

			if (Enum.TryParse<Sandbox.Common.Localization.MyTextsWrapperEnum>(value, out myText))
			{
				try
				{
					return Sandbox.Common.Localization.MyTextsWrapper.GetFormatString(myText);
				}
				catch
				{
					return value;
				}
			}

			return value;
		}
*/
		#endregion
    }

    public class CubeBlockDefinitionsManager : SerializableDefinitionsManager<MyObjectBuilder_CubeBlockDefinition, CubeBlockDefinition>
    {
	}
}