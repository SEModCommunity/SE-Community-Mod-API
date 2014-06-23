using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Xml;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEModAPI.API.Definitions.CubeBlocks
{
	/// <summary>
	/// This class is intended to manage the modification and persistente of CubeBlocks.sbc
	/// </summary>
	public class BlocksManager
	{
		#region "Attributes"

		private static string m_DefaultFileName = "CubeBlocks.sbc";
		private bool m_pendingChanges = false;
		private ConfigFileSerializer m_configFileSerializer;
		private MyObjectBuilder_Definitions m_definitions;


		private List<CubeBlockDef> m_cubeBlocks = new List<CubeBlockDef>();
		private List<AssemblerDefinition> m_assemblers = new List<AssemblerDefinition>();
		private List<CargoContainerDefinition> m_cargoContainers = new List<CargoContainerDefinition>();
		private List<CockpitDefinition> m_cockpits = new List<CockpitDefinition>();
		private List<GravityGeneratorDefinition> m_gravityGenerators = new List<GravityGeneratorDefinition>();
		private List<GyroscopeDefinition> m_gyroscopes = new List<GyroscopeDefinition>();
		private List<MergeBlockDefinition> m_mergeBlocks = new List<MergeBlockDefinition>();
		private List<MotorStatorDefinition> m_motorStators = new List<MotorStatorDefinition>();
		private List<OreDetectorDefinition> m_oreDetectors = new List<OreDetectorDefinition>();
		private List<ReactorDefinition> m_reactors = new List<ReactorDefinition>();
		private List<RefineryDefinition> m_refineries = new List<RefineryDefinition>();
		private List<LightingBlockDefinition> m_lightingBlocks = new List<LightingBlockDefinition>();
		private List<ShipDrillDefinition> m_shipDrills = new List<ShipDrillDefinition>();
		private List<SolarPanelDefinition> m_solarPanels = new List<SolarPanelDefinition>();
		private List<ThrusterDefinition> m_thrusters = new List<ThrusterDefinition>();
		private List<VirtualMassDefinition> m_virtualMasses = new List<VirtualMassDefinition>();

		#endregion
		

		#region "Constructor & Initializers"

		/// <summary>
		/// Default RAII constructor of Manager
		/// </summary>
		/// <param name="cubeBlocksFileInfo">The valid FileInfo that points to a valid CubeBlocks.sbc file</param>
		/// <param name="defaultName">Defines if the file has the defaultName: CubeBlocks.sbc</param>
		public BlocksManager(FileInfo cubeBlocksFileInfo, bool defaultName = true)
		{
			if (defaultName)
			{
				if (cubeBlocksFileInfo.Name != m_DefaultFileName)
				{
					throw new SEConfigurationException(SEConfigurationExceptionState.InvalidDefaultConfigFileName, "The given file name is not matching the default configuration name pattern: CubeBlocks.sbc");
				}
			}
			m_configFileSerializer = new ConfigFileSerializer(cubeBlocksFileInfo, defaultName);
			if (cubeBlocksFileInfo.Exists)
			{
				Deserialize();
			}
		}

		#endregion

		#region "Properties"

		/// <summary>
		/// Get the container for Assemblers
		/// </summary>
		public List<AssemblerDefinition> Assemblers
		{
			get { return m_assemblers; }
		}

		/// <summary>
		/// Get the container for CargoContainers
		/// </summary>
		public List<CargoContainerDefinition> CargoContainers
		{
			get { return m_cargoContainers; }
		}

		/// <summary>
		/// Get the container for CubeBlocks
		/// </summary>
		public List<CubeBlockDef> CubeBlocks
		{
			get { return m_cubeBlocks; }
		}

		/// <summary>
		/// Get the container for Cockpits
		/// </summary>
		public List<CockpitDefinition> Cockpits
		{
			get { return m_cockpits; }
		}

		/// <summary>
		/// Get the container for GravityGenerators
		/// </summary>
		public List<GravityGeneratorDefinition> GravityGenerators
		{
			get { return m_gravityGenerators; }
		}

		/// <summary>
		/// Get the container for Gyroscopes
		/// </summary>
		public List<GyroscopeDefinition> Gyroscopes
		{
			get { return m_gyroscopes; }
		}

		/// <summary>
		/// Get the container for LightingBlocks
		/// </summary>
		public List<LightingBlockDefinition> LightingBlocks
		{
			get { return m_lightingBlocks; }
		}

		/// <summary>
		/// Get the container for MergeBlocks
		/// </summary>
		public List<MergeBlockDefinition> MergeBlocks
		{
			get { return m_mergeBlocks; }
		}

		/// <summary>
		/// Get the container for LightingBlocks
		/// </summary>
		public List<MotorStatorDefinition> MotorStators
		{
			get { return m_motorStators; }
		}

		/// <summary>
		/// Get the container for OreDetectors
		/// </summary>
		public List<OreDetectorDefinition> OreDetectors
		{
			get { return m_oreDetectors; }
		}

		/// <summary>
		/// Get the container for Reactors
		/// </summary>
		public List<ReactorDefinition> Reactors
		{
			get { return m_reactors; }
		}

		/// <summary>
		/// Get the container for Refineries
		/// </summary>
		public List<RefineryDefinition> Refineries
		{
			get { return m_refineries; }
		}

		/// <summary>
		/// Get the container for ShipDrills
		/// </summary>
		public List<ShipDrillDefinition> ShipDrills
		{
			get { return m_shipDrills; }
		}

		/// <summary>
		/// Get the container for ShipDrills
		/// </summary>
		public List<SolarPanelDefinition> SolarPanels
		{
			get { return m_solarPanels; }
		}

		/// <summary>
		/// Get the container for ShipDrills
		/// </summary>
		public List<ThrusterDefinition> Thrusters
		{
			get { return m_thrusters; }
		}

		/// <summary>
		/// Get the container for ShipDrills
		/// </summary>
		public List<VirtualMassDefinition> VirtualMasses
		{
			get { return m_virtualMasses; }
		}

		#endregion

		#region "Methods"

		/// <summary>
		/// Method that scan definitions for changes
		/// </summary>
		/// <returns></returns>
		public bool FindChangesInDefinitions()
		{
			foreach (var block in ExtractDefinitionsFromContainers())
			{
				if (block.Changed)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Method to Serialize the current inner Configuration File
		/// </summary>
		public void Serialize()
		{
			m_definitions.CubeBlocks = ExtractBaseDefinitionsFromContainers().ToArray();
			m_configFileSerializer.Serialize(m_definitions);
			m_pendingChanges = false;
		}

		/// <summary>
		/// Method to Deserialize the current inner Configuration File
		/// </summary>
		public void Deserialize()
		{
			m_definitions = m_configFileSerializer.Deserialize();
			FillOverLayerContainers(m_definitions.CubeBlocks);
		}

		/// <summary>
		/// Method that fill the containers the underlayed definitions
		/// </summary>
		/// <param name="blocks">If an array is given, the containers will be filled with this array instead of the default underlayed one</param>
		public void FillOverLayerContainers(MyObjectBuilder_CubeBlockDefinition[] blocks)
		{
			m_cubeBlocks.Clear();
			m_assemblers.Clear();
			m_cargoContainers.Clear();
			m_cockpits.Clear();
			m_gravityGenerators.Clear();
			m_gyroscopes.Clear();
			m_mergeBlocks.Clear();
			m_motorStators.Clear();
			m_oreDetectors.Clear();
			m_reactors.Clear();
			m_refineries.Clear();
			m_lightingBlocks.Clear();
			m_shipDrills.Clear();
			m_solarPanels.Clear();
			m_thrusters.Clear();
			m_virtualMasses.Clear();
			foreach (var cubeBlock in blocks)
			{
				switch (cubeBlock.Id.TypeId)
				{
					case (MyObjectBuilderTypeEnum.CubeBlock):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Assembler):
					{
						m_assemblers.Add(new AssemblerDefinition((MyObjectBuilder_AssemblerDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Beacon):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.CargoContainer):
					{
						m_cargoContainers.Add(new CargoContainerDefinition((MyObjectBuilder_CargoContainerDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Cockpit):
					{
						m_cockpits.Add(new CockpitDefinition((MyObjectBuilder_CockpitDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Collector):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Conveyor):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ConveyorConnector):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Decoy):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Door):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Drill):
					{
						m_shipDrills.Add(new ShipDrillDefinition((MyObjectBuilder_ShipDrillDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.GravityGenerator):
					{
						m_gravityGenerators.Add(new GravityGeneratorDefinition((MyObjectBuilder_GravityGeneratorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Gyro):
					{
						m_gyroscopes.Add(new GyroscopeDefinition((MyObjectBuilder_GyroDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.LandingGear):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.LargeGatlingTurret):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.LightingBlock):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MedicalRoom):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MergeBlock):
					{
						m_mergeBlocks.Add(new MergeBlockDefinition((MyObjectBuilder_MergeBlockDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MotorRotor):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.MotorStator):
					{
						m_motorStators.Add(new MotorStatorDefinition((MyObjectBuilder_MotorStatorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.OreDetector):
					{
						m_oreDetectors.Add(new OreDetectorDefinition((MyObjectBuilder_OreDetectorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Passage):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.RadioAntenna):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Reactor):
					{
						m_reactors.Add(new ReactorDefinition((MyObjectBuilder_ReactorDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.RealWheel):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Refinery):
					{
						m_refineries.Add(new RefineryDefinition((MyObjectBuilder_RefineryDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ReflectorLight):
					{
						m_lightingBlocks.Add(new LightingBlockDefinition((MyObjectBuilder_LightingBlockDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ShipConnector):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ShipGrinder):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.ShipWelder):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.SmallGatlingGun):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.SmallMissileLauncher):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.SolarPanel):
					{
						m_solarPanels.Add(new SolarPanelDefinition((MyObjectBuilder_SolarPanelDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Thrust):
					{
						m_thrusters.Add(new ThrusterDefinition((MyObjectBuilder_ThrustDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.VirtualMass):
					{
						m_virtualMasses.Add(new VirtualMassDefinition((MyObjectBuilder_VirtualMassDefinition)cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Warhead):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;

					case (MyObjectBuilderTypeEnum.Wheel):
					{
						m_cubeBlocks.Add(new CubeBlockDef(cubeBlock));
					}
					break;
				}	
			}
			m_pendingChanges = false;
		}

		/// <summary>
		/// Method that Extract the base cubeblocks definitions from every container.
		/// </summary>
		public List<MyObjectBuilder_CubeBlockDefinition> ExtractBaseDefinitionsFromContainers()
		{
			List<MyObjectBuilder_CubeBlockDefinition> blocks = new List<MyObjectBuilder_CubeBlockDefinition>();

			foreach (var item in m_cubeBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_assemblers)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_cargoContainers)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_cockpits)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_gravityGenerators)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_gyroscopes)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_lightingBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_mergeBlocks)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_motorStators)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_oreDetectors)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_reactors)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_refineries)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_shipDrills)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_solarPanels)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_thrusters)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}
			foreach (var item in m_virtualMasses)
			{
				blocks.Add(item.GetSubTypeDefinition());
			}

			return blocks;
		}

		/// <summary>
		/// Method that Extract base blocks definitions from container.
		/// </summary>
		public List<BlockDefinition> ExtractDefinitionsFromContainers()
		{
			List<BlockDefinition> blocks = new List<BlockDefinition>();

			foreach (var item in m_cubeBlocks)
			{
				blocks.Add(item);
			}
			foreach (var item in m_cargoContainers)
			{
				blocks.Add(item);
			}
			foreach (var item in m_cockpits)
			{
				blocks.Add(item);
			}
			foreach (var item in m_gravityGenerators)
			{
				blocks.Add(item);
			}
			foreach (var item in m_gyroscopes)
			{
				blocks.Add(item);
			}
			foreach (var item in m_lightingBlocks)
			{
				blocks.Add(item);
			}
			foreach (var item in m_mergeBlocks)
			{
				blocks.Add(item);
			}
			foreach (var item in m_motorStators)
			{
				blocks.Add(item);
			}
			foreach (var item in m_oreDetectors)
			{
				blocks.Add(item);
			}
			foreach (var item in m_reactors)
			{
				blocks.Add(item);
			}
			foreach (var item in m_refineries)
			{
				blocks.Add(item);
			}
			foreach (var item in m_shipDrills)
			{
				blocks.Add(item);
			}
			foreach (var item in m_solarPanels)
			{
				blocks.Add(item);
			}
			foreach (var item in m_thrusters)
			{
				blocks.Add(item);
			}
			foreach (var item in m_virtualMasses)
			{
				blocks.Add(item);
			}

			return blocks;
		}

		#endregion
	}
}
