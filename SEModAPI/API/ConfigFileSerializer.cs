using Microsoft.Xml.Serialization.GeneratedAssembly;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.VRageData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SEModAPI.Support;
using VRageMath;


namespace SEModAPI.API
{
    public class ConfigFileSerializer
    {


        #region "Attributes"

        private MyObjectBuilder_Definitions _ammoMagazineDefinitions;
        private MyObjectBuilder_Definitions _cubeBlockDefinitions;
        private MyObjectBuilder_Definitions _componentDefinitions;
        private MyObjectBuilder_Definitions _blueprintDefinitions;
        private MyObjectBuilder_Definitions _physicalItemDefinitions;
        private MyObjectBuilder_Definitions _voxelMaterialDefinitions;
        private Dictionary<string, byte> _materialIndex;

        private readonly GameInstallationInfo _gameInstallation;

        #endregion

        #region properties

        public MyObjectBuilder_CubeBlockDefinition[] CubeBlockDefinitions
        {
            get
            {
                return _cubeBlockDefinitions.CubeBlocks;
            }
        }

        public void SetCubeBlockDefinitions(MyObjectBuilder_CubeBlockDefinition[] cubeBlockDefinitions)
        {
            _cubeBlockDefinitions.CubeBlocks = cubeBlockDefinitions;
        }

        public void SetCubeBlockDefinitionsIndex(int index, MyObjectBuilder_CubeBlockDefinition cubeBlockDefinition)
        {
            if (index < _cubeBlockDefinitions.CubeBlocks.Length)
            {
                _cubeBlockDefinitions.CubeBlocks[index] = cubeBlockDefinition;
            } 
        }



        public MyBlockPosition[] CubeBlockPositions
        {
            get { return _cubeBlockDefinitions.BlockPositions; }
        }

        public MyObjectBuilder_ComponentDefinition[] ComponentDefinitions
        {
            get { return _componentDefinitions.Components; }
        }

        public MyObjectBuilder_PhysicalItemDefinition[] PhysicalItemDefinitions
        {
            get { return _physicalItemDefinitions.PhysicalItems; }
        }

        public MyObjectBuilder_AmmoMagazineDefinition[] AmmoMagazineDefinitions
        {
            get { return _ammoMagazineDefinitions.AmmoMagazines; }
        }

        public MyObjectBuilder_VoxelMaterialDefinition[] VoxelMaterialDefinitions
        {
            get { return _voxelMaterialDefinitions.VoxelMaterials; }
        }

        public MyObjectBuilder_BlueprintDefinition[] BlueprintDefinitions
        {
            get { return _blueprintDefinitions.Blueprints; }
        }

        #endregion

        #region "Constructors & Initializers"

        public ConfigFileSerializer()
        {
            //Prepare game installation configuration
            _gameInstallation = new GameInstallationInfo();
            // Dynamically read all definitions as soon as the SpaceEngineersAPI class is first invoked.
            ReadCubeBlockDefinitions();
        }

        #endregion

        #region Serializers

        public T ReadSpaceEngineersFile<T, TS>(Stream stream)
        where TS : XmlSerializer1
        {
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            object obj;

            using (var xmlReader = XmlReader.Create(stream, settings))
            {

                var serializer = (TS)Activator.CreateInstance(typeof(TS));
                //serializer.UnknownAttribute += serializer_UnknownAttribute;
                //serializer.UnknownElement += serializer_UnknownElement;
                //serializer.UnknownNode += serializer_UnknownNode;
                obj = serializer.Deserialize(xmlReader);
            }

            return (T)obj;
        }

        public bool TryReadSpaceEngineersFile<T, TS>(string filename, out T entity)
             where TS : XmlSerializer1
        {
            try
            {
                entity = ReadSpaceEngineersFile<T, TS>(filename);
                return true;
            }
            catch
            {
                entity = default(T);
                return false;
            }
        }

        public T ReadSpaceEngineersFile<T, TS>(string filename)
        where TS : XmlSerializer1
        {
            var settings = new XmlReaderSettings
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            object obj = null;

            if (File.Exists(filename))
            {
                using (var xmlReader = XmlReader.Create(filename, settings))
                {
                    var serializer = (TS)Activator.CreateInstance(typeof(TS));
                    obj = serializer.Deserialize(xmlReader);
                }
            }

            return (T)obj;
        }

        public T Deserialize<T>(string xml)
        {
            using (var textReader = new StringReader(xml))
            {
                return (T)(new XmlSerializerContract().GetSerializer(typeof(T)).Deserialize(textReader));
            }
        }

        public string Serialize<T>(object item)
        {
            using (var textWriter = new StringWriter())
            {
                new XmlSerializerContract().GetSerializer(typeof(T)).Serialize(textWriter, item);
                return textWriter.ToString();
            }
        }

        public bool WriteSpaceEngineersFile<T, TS>(T sector, string filename)
            where TS : XmlSerializer1
        {
            // How they appear to be writing the files currently.
            try
            {
                using (var xmlTextWriter = new XmlTextWriter(filename, null))
                {
                    xmlTextWriter.Formatting = Formatting.Indented;
                    xmlTextWriter.Indentation = 2;
                    var serializer = (TS)Activator.CreateInstance(typeof(TS));
                    serializer.Serialize(xmlTextWriter, sector);
                }
            }
            catch
            {
                return false;
            }

            //// How they should be doing it to support Unicode.
            //var settingsDestination = new XmlWriterSettings()
            //{
            //    Indent = true, // Set indent to false to compress.
            //    Encoding = new UTF8Encoding(false)   // codepage 65001 without signature. Removes the Byte Order Mark from the start of the file.
            //};

            //try
            //{
            //    using (var xmlWriter = XmlWriter.Create(filename, settingsDestination))
            //    {
            //        S serializer = (S)Activator.CreateInstance(typeof(S));
            //        serializer.Serialize(xmlWriter, sector);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    return false;
            //}

            return true;
        }

        #endregion

        #region GenerateEntityId

        public long GenerateEntityId()
        {
            // Not the offical SE way of generating IDs, but its fast and we don't have to worry about a random seed.
            var buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        #endregion

        #region SetCubeOrientation

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

        public SerializableBlockOrientation GetCubeOrientation(CubeType type)
        {
            if (CubeOrientations.ContainsKey(type))
                return CubeOrientations[type];

            throw new NotImplementedException(string.Format("SetCubeOrientation of type [{0}] not yet implemented.", type));
        }

        #endregion

        #region ReadCubeBlockDefinitions

        public void ReadCubeBlockDefinitions()
        {
            _ammoMagazineDefinitions = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>("AmmoMagazines.sbc");
            _voxelMaterialDefinitions = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>("VoxelMaterials.sbc");
            _physicalItemDefinitions = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>("PhysicalItems.sbc");
            _componentDefinitions = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>("Components.sbc");
            _cubeBlockDefinitions = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>("CubeBlocks.sbc");
            _blueprintDefinitions = LoadContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>("Blueprints.sbc");
            _materialIndex = new Dictionary<string, byte>();
        }

        private T LoadContentFile<T, TS>(string filename) where TS : XmlSerializer1
        {
            object fileContent = null;

            string filePath = Path.Combine(Path.Combine(_gameInstallation.GamePath, @"Content\Data"), filename);

            if (!File.Exists(filePath))
            {
                throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing), filePath);
            }

            try
            {
                fileContent = ReadSpaceEngineersFile<T, TS>(filePath);
            }
            catch
            {
                throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted), filePath);
            }

            if (fileContent == null)
            {
                throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty), filePath);
            }

            // TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
            //     Lock the load during this time, in case it happens multiple times.
            // Report a friendly error if this load fails.

            return (T)fileContent;
        }

        #endregion

        #region WriteCubeBlocksDefinitions

        public void WriteCubeBlockDefinitions()
        {
            SaveAmmoMagazinesContentFile();
            SaveVoxelMaterialsContentFile();
            SavePhysicalItemsContentFile();
            SaveComponentsContentFile();
            SaveCubeBlocksContentFile();
            SaveBlueprintsContentFile();
        }

        public void SaveAmmoMagazinesContentFile()
        {
            SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(_ammoMagazineDefinitions, "AmmoMagazines.sbc");
        }

        public void SaveVoxelMaterialsContentFile()
        {
            SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(_voxelMaterialDefinitions, "VoxelMaterials.sbc");
        }
        public void SavePhysicalItemsContentFile()
        {
            SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(_physicalItemDefinitions, "PhysicalItems.sbc");
        }
        public void SaveComponentsContentFile()
        {
            SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(_componentDefinitions, "Components.sbc");
        }
        public void SaveCubeBlocksContentFile()
        {
            SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(_cubeBlockDefinitions, "CubeBlocks.sbc");
        }
        public void SaveBlueprintsContentFile()
        {
            SaveContentFile<MyObjectBuilder_Definitions, MyObjectBuilder_DefinitionsSerializer>(_blueprintDefinitions, "Blueprints.sbc");
        }

        private void SaveContentFile<T, TS>(T fileContent,string filename) where TS : XmlSerializer1
        {

            string filePath = Path.Combine(Path.Combine(_gameInstallation.GamePath, @"Content\Data"), filename);

            if (!File.Exists(filePath))
            {
                throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileMissing), filePath);
            }

            try
            {
                WriteSpaceEngineersFile<T, TS>(fileContent, filePath);
            }
            catch
            {
                throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileCorrupted), filePath);
            }

            if (fileContent == null)
            {
                throw new AutoException(new GameInstallationInfoException(GameInstallationInfoExceptionState.ConfigFileEmpty), filePath);
            }

            // TODO: set a file watch to reload the files, incase modding is occuring at the same time this is open.
            //     Lock the load during this time, in case it happens multiple times.
            // Report a friendly error if this load fails.
        }

        #endregion

        #region FetchCubeBlockMass

        public float FetchCubeBlockMass(MyObjectBuilderTypeEnum typeId, MyCubeSize cubeSize, string subTypeid)
        {
            float mass = 0;

            var cubeBlockDefinition = GetCubeDefinition(typeId, cubeSize, subTypeid);

            if (cubeBlockDefinition != null)
            {
                foreach (var component in cubeBlockDefinition.Components)
                {
                    mass += _componentDefinitions.Components.Where(c => c.Id.SubtypeId == component.Subtype).Sum(c => c.Mass) * component.Count;
                }
            }

            return mass;
        }

        public void AccumulateCubeBlueprintRequirements(string subType, MyObjectBuilderTypeEnum typeId, decimal amount, Dictionary<string, MyObjectBuilder_BlueprintDefinition.Item> requirements, out TimeSpan timeTaken)
        {
            TimeSpan time = new TimeSpan();
            var bp = _blueprintDefinitions.Blueprints.FirstOrDefault(b => b.Result.SubtypeId == subType && b.Result.TypeId == typeId);
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
            var cube = _cubeBlockDefinitions.CubeBlocks.FirstOrDefault(d => d.Id.TypeId == typeId && d.Id.SubtypeId == subTypeId);
            if (cube != null)
            {
                return cube;
            }

            var item = _physicalItemDefinitions.PhysicalItems.FirstOrDefault(d => d.Id.TypeId == typeId && d.Id.SubtypeId == subTypeId);
            if (item != null)
            {
                return item;
            }

            var component = _componentDefinitions.Components.FirstOrDefault(c => c.Id.TypeId == typeId && c.Id.SubtypeId == subTypeId);
            if (component != null)
            {
                return component;
            }

            var magazine = _ammoMagazineDefinitions.AmmoMagazines.FirstOrDefault(c => c.Id.TypeId == typeId && c.Id.SubtypeId == subTypeId);
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
            return _voxelMaterialDefinitions.VoxelMaterials;
        }

        public byte GetMaterialIndex(string materialName)
        {
            if (_materialIndex.ContainsKey(materialName))
                return _materialIndex[materialName];
            else
            {
                var material = _voxelMaterialDefinitions.VoxelMaterials.FirstOrDefault(m => m.Name == materialName);
                var index = (byte)_voxelMaterialDefinitions.VoxelMaterials.ToList().IndexOf(material);
                _materialIndex.Add(materialName, index);
                return index;
            }
        }

        public string GetMaterialName(byte materialIndex, byte defaultMaterialIndex)
        {
            if (materialIndex <= _voxelMaterialDefinitions.VoxelMaterials.Length)
                return _voxelMaterialDefinitions.VoxelMaterials[materialIndex].Name;
            else
                return _voxelMaterialDefinitions.VoxelMaterials[defaultMaterialIndex].Name;
        }

        public string GetMaterialName(byte materialIndex)
        {
            return _voxelMaterialDefinitions.VoxelMaterials[materialIndex].Name;
        }

        #endregion

        #region GetCubeDefinition

        public MyObjectBuilder_CubeBlockDefinition GetCubeDefinition(MyObjectBuilderTypeEnum typeId, MyCubeSize cubeSize, string subtypeId)
        {
            if (string.IsNullOrEmpty(subtypeId))
            {
                return _cubeBlockDefinitions.CubeBlocks.FirstOrDefault(d => d.CubeSize == cubeSize && d.Id.TypeId == typeId);
            }

            return _cubeBlockDefinitions.CubeBlocks.FirstOrDefault(d => d.Id.SubtypeId == subtypeId || (d.Variants != null && d.Variants.Any(v => subtypeId == d.Id.SubtypeId + v.Color)));
            // Returns null if it doesn't find the required SubtypeId.
        }

        #endregion

        #region GetBoundingBox

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

        #endregion

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
    }
}
