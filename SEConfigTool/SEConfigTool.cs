using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using System.IO;

using SEModAPI;
using SEModAPI.API;
using SEModAPI.API.Definitions;
using SEModAPI.API.Definitions.CubeBlocks;
using SEModAPI.API.SaveData;
using SEModAPI.API.SaveData.Entity;

using Sandbox.Common.Localization;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using VRageMath;
using System.Diagnostics;

namespace SEConfigTool
{
	public partial class SEConfigTool : Form
	{
		#region Attributes

		private string m_standardSavePath;

		private SectorManager m_sectorManager;

		private CubeBlockDefinitionsManager m_cubeBlockDefinitionsManager;
		private AmmoMagazinesDefinitionsManager m_ammoMagazinesDefinitionsManager;
		private ContainerTypesDefinitionsManager m_containerTypesDefinitionsManager;
		private GlobalEventsDefinitionsManager m_globalEventsDefinitionsManager;
		private SpawnGroupsDefinitionsManager m_spawnGroupsDefinitionsManager;
		private PhysicalItemDefinitionsManager m_physicalItemsDefinitionsManager;
		private ComponentDefinitionsManager m_componentsDefinitionsManager;
		private BlueprintDefinitionsManager m_blueprintsDefinitionsManager;
		private VoxelMaterialDefinitionsManager m_voxelMaterialsDefinitionsManager;
		private ScenariosDefinitionsManager m_scenariosDefinitionManager;
		private TransparentMaterialsDefinitionManager m_transparentMaterialsDefinitionManager;
		private ConfigurationDefinition m_configurationDefinitionManager;
		private EnvironmentDefinition m_environmentDefinitionManager;

		private bool m_currentlyFillingConfigurationListBox;
		private bool m_currentlySelecting;

		private NumberFormatInfo m_numberFormatInfo;
		private string m_decimalSeparator;
		private string m_groupSeparator;
		private string m_negativeSign;

		#endregion

		#region Constructors and Initializers

		public SEConfigTool()
		{
			InitializeComponent();

			m_numberFormatInfo = CultureInfo.GetCultureInfo("EN-US").NumberFormat;
			m_decimalSeparator = m_numberFormatInfo.CurrencyDecimalSeparator;
			m_groupSeparator = m_numberFormatInfo.NumberGroupSeparator;
			m_negativeSign = m_numberFormatInfo.NegativeSign;

			m_sectorManager = new SectorManager();
			m_cubeBlockDefinitionsManager = new CubeBlockDefinitionsManager();
			m_ammoMagazinesDefinitionsManager = new AmmoMagazinesDefinitionsManager();
			m_containerTypesDefinitionsManager = new ContainerTypesDefinitionsManager();
			m_globalEventsDefinitionsManager = new GlobalEventsDefinitionsManager();
			m_spawnGroupsDefinitionsManager = new SpawnGroupsDefinitionsManager();
			m_physicalItemsDefinitionsManager = new PhysicalItemDefinitionsManager();
			m_componentsDefinitionsManager = new ComponentDefinitionsManager();
			m_blueprintsDefinitionsManager = new BlueprintDefinitionsManager();
			m_voxelMaterialsDefinitionsManager = new VoxelMaterialDefinitionsManager();
			m_scenariosDefinitionManager = new ScenariosDefinitionsManager();
			m_transparentMaterialsDefinitionManager = new TransparentMaterialsDefinitionManager();

			m_configurationDefinitionManager = new ConfigurationDefinition();
			m_environmentDefinitionManager = new EnvironmentDefinition();

			m_globalEventsDefinitionsManager.IsMutable = true;
			m_ammoMagazinesDefinitionsManager.IsMutable = true;
			m_componentsDefinitionsManager.IsMutable = true;
			m_physicalItemsDefinitionsManager.IsMutable = true;
			m_containerTypesDefinitionsManager.IsMutable = true;
		}

		#endregion

		#region Form methods

		private FileInfo GetContentDataFile(string configFileName)
		{
			return SerializableDefinitionsManager<MyObjectBuilder_Base, OverLayerDefinition<MyObjectBuilder_Base>>.GetContentDataFile(configFileName);
		}

		private void LoadSaveFile(FileInfo saveFileInfo)
		{
			TLS_StatusLabel.Text = "Loading sector ...";
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			m_sectorManager.Load(saveFileInfo);
			Sector sector = m_sectorManager.Sector;

			TXT_SavedGame_Properties_Position.Text = sector.Position.ToString();
			TXT_SavedGame_Properties_AppVersion.Text = sector.AppVersion.ToString();

			LST_SavedGame_Events.Items.Clear();
			foreach (Event currentEvent in sector.Events)
			{
				LST_SavedGame_Events.Items.Add(currentEvent.Name);
			}

			TRV_SavedGame_Objects.BeginUpdate();
			TRV_SavedGame_Objects.Nodes.Clear();

			//Add the sector object categories
			TRV_SavedGame_Objects.Nodes.Add("Cube Grids");
			TRV_SavedGame_Objects.Nodes.Add("Voxel Maps");
			TRV_SavedGame_Objects.Nodes.Add("Floating Objects");
			TRV_SavedGame_Objects.Nodes.Add("Meteors");
			TRV_SavedGame_Objects.Nodes.Add("Unknown");

			#region CubeGrids

			//Add the cube grids
			foreach (CubeGrid cubeGrid in sector.CubeGrids)
			{
				float x = cubeGrid.PositionAndOrientation.Position.x;
				float y = cubeGrid.PositionAndOrientation.Position.y;
				float z = cubeGrid.PositionAndOrientation.Position.z;

				float dist = (float)Math.Sqrt(x * x + y * y + z * z);

				TreeNode newNode = TRV_SavedGame_Objects.Nodes[0].Nodes.Add(cubeGrid.EntityId.ToString(), cubeGrid.Name + " | " + "Dist: " + dist.ToString("F2") + "m");
				newNode.Tag = cubeGrid;

				//Create the cube grid sub-item categories
				TreeNode blocksNode = newNode.Nodes.Add("Cube Blocks (" + cubeGrid.CubeBlocks.Count.ToString() + ")");
				TreeNode conveyorLinesNode = newNode.Nodes.Add("Conveyor Lines (" + cubeGrid.ConveyorLines.Count.ToString() + ")");
				TreeNode blockGroupsNode = newNode.Nodes.Add("Block Groups (" + cubeGrid.BlockGroups.Count.ToString() + ")");

				#region CubeBlocks

				//Create the cube block categories
				TreeNode structuralBlocksNode = blocksNode.Nodes.Add("Structural");
				TreeNode containerBlocksNode = blocksNode.Nodes.Add("Containers");
				TreeNode productionBlocksNode = blocksNode.Nodes.Add("Refinement and Production");
				TreeNode energyBlocksNode = blocksNode.Nodes.Add("Energy");
				TreeNode conveyorBlocksNode = blocksNode.Nodes.Add("Conveyor");
				TreeNode utilityBlocksNode = blocksNode.Nodes.Add("Utility");
				TreeNode miscBlocksNode = blocksNode.Nodes.Add("Misc");

				//Add the cube blocks
				foreach (var cubeBlockObject in cubeGrid.CubeBlocks)
				{
					TreeNode blockNode = null;

					Type cubeType = cubeBlockObject.GetType();

					if (cubeType.IsAssignableFrom(typeof(CubeBlock<MyObjectBuilder_CubeBlock>)))
					{
						CubeBlock<MyObjectBuilder_CubeBlock> cubeBlock = (CubeBlock<MyObjectBuilder_CubeBlock>)cubeBlockObject;
						switch (cubeBlock.TypeId)
						{
							case MyObjectBuilderTypeEnum.CubeBlock:
								blockNode = structuralBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.Refinery:
								blockNode = productionBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.Assembler:
								blockNode = productionBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.SolarPanel:
								blockNode = energyBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.ShipConnector:
								blockNode = conveyorBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.Collector:
								blockNode = conveyorBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.Conveyor:
								blockNode = conveyorBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.ConveyorConnector:
								blockNode = conveyorBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							case MyObjectBuilderTypeEnum.Cockpit:
								blockNode = utilityBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
							default:
								blockNode = miscBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
								break;
						}
					}
					
					if (cubeType.IsAssignableFrom(typeof(CargoContainerEntity)))
					{
						CargoContainerEntity cargoContainer = (CargoContainerEntity)cubeBlockObject;

						blockNode = containerBlocksNode.Nodes.Add(cargoContainer.EntityId.ToString(), cargoContainer.Name);

						foreach (var item in cargoContainer.Inventory.Items)
						{
							TreeNode itemNode = blockNode.Nodes.Add(item.PhysicalContent.SubtypeName + " x" + item.Amount.ToString());
							itemNode.Tag = item;
						}
					}
					
					if (cubeType.IsAssignableFrom(typeof(ReactorEntity)))
					{
						ReactorEntity reactorBlock = (ReactorEntity)cubeBlockObject;

						blockNode = energyBlocksNode.Nodes.Add(reactorBlock.EntityId.ToString(), reactorBlock.Name);

						foreach (var item in reactorBlock.Inventory.Items)
						{
							TreeNode itemNode = blockNode.Nodes.Add(item.PhysicalContent.SubtypeName + " x" + item.Amount.ToString());
							itemNode.Tag = item;
						}
					}

					if (cubeType.IsAssignableFrom(typeof(MedicalRoomEntity)))
					{
						MedicalRoomEntity medicalBlock = (MedicalRoomEntity)cubeBlockObject;

						blockNode = utilityBlocksNode.Nodes.Add(medicalBlock.EntityId.ToString(), medicalBlock.Name);
					}

					if (blockNode == null)
						continue;

					blockNode.Tag = cubeBlockObject;
				}

				#endregion

				#region ConveyorLines
				#endregion

				#region BlockGroups
				#endregion
			}

			#endregion

			//Add the voxel maps
			foreach (VoxelMap voxelMap in sector.VoxelMaps)
			{
				float x = voxelMap.PositionAndOrientation.Position.x;
				float y = voxelMap.PositionAndOrientation.Position.y;
				float z = voxelMap.PositionAndOrientation.Position.z;

				float dist = (float)Math.Sqrt(x * x + y * y + z * z);

				TRV_SavedGame_Objects.Nodes[1].Nodes.Add(voxelMap.Name + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z + ";" + y);
			}

			//Add the floating objects
			foreach (FloatingObject floatingObject in sector.FloatingObjects)
			{
				float x = floatingObject.PositionAndOrientation.Position.x;
				float y = floatingObject.PositionAndOrientation.Position.y;
				float z = floatingObject.PositionAndOrientation.Position.z;

				float dist = (float)Math.Sqrt(x * x + y * y + z * z);

				TRV_SavedGame_Objects.Nodes[2].Nodes.Add(floatingObject.Name + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z + ";" + y);
			}

			//Add the meteors
			foreach (Meteor meteor in sector.Meteors)
			{
				float x = meteor.PositionAndOrientation.Position.x;
				float y = meteor.PositionAndOrientation.Position.y;
				float z = meteor.PositionAndOrientation.Position.z;

				float dist = (float)Math.Sqrt(x * x + y * y + z * z);

				TRV_SavedGame_Objects.Nodes[3].Nodes.Add(meteor.Name + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z + ";" + y);
			}

			//Add any unknown objects
			foreach (SectorObject<MyObjectBuilder_EntityBase> unknown in sector.UnknownObjects)
			{
				float x = unknown.PositionAndOrientation.Position.x;
				float y = unknown.PositionAndOrientation.Position.y;
				float z = unknown.PositionAndOrientation.Position.z;

				float dist = (float)Math.Sqrt(x * x + y * y + z * z);

				TRV_SavedGame_Objects.Nodes[4].Nodes.Add(unknown.Name + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z + ";" + y);
			}

			TRV_SavedGame_Objects.EndUpdate();

			stopWatch.Stop();
			TLS_StatusLabel.Text = "Done in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";
			BTN_SavedGame_Events_Apply.Enabled = false;

			MessageBox.Show(this, "Sector loaded successfully in " + stopWatch.ElapsedMilliseconds.ToString() + "ms!");
		}

		private void FillBlocksConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_cubeBlockDefinitionsManager.Load(GetContentDataFile("CubeBlocks.sbc"));

			LST_BlocksConfig.Items.Clear();
			foreach (var definition in m_cubeBlockDefinitionsManager.Definitions)
			{
				LST_BlocksConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillAmmoConfigurationListBox(bool loadFromFile = true)
		{
			m_currentlyFillingConfigurationListBox = true;

			if (loadFromFile)
				m_ammoMagazinesDefinitionsManager.Load(GetContentDataFile("AmmoMagazines.sbc"));

			LST_AmmoConfig.Items.Clear();
			foreach (var definition in m_ammoMagazinesDefinitionsManager.Definitions)
			{
				LST_AmmoConfig.Items.Add(definition.Id.SubtypeId);
			}

			TXT_AmmoConfig_Details_Id.Text = "";
			TXT_AmmoConfig_Details_Name.Text = "";
			TXT_AmmoConfig_Details_Description.Text = "";
			TXT_AmmoConfig_Details_Icon.Text = "";
			TXT_AmmoConfig_Details_Model.Text = "";
			CMB_AmmoConfig_Details_Caliber.SelectedItem = MyAmmoCategoryEnum.SmallCaliber;
			TXT_AmmoConfig_Details_Capacity.Text = "";
			TXT_AmmoConfig_Details_Volume.Text = "";
			TXT_AmmoConfig_Details_Mass.Text = "";

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillContainerTypeConfigurationListBox(bool loadFromFile = true)
		{
			m_currentlyFillingConfigurationListBox = true;

			if(loadFromFile)
				m_containerTypesDefinitionsManager.Load(GetContentDataFile("ContainerTypes.sbc"));

			LST_ContainerTypesConfig.Items.Clear();
			LST_ContainerTypeConfig_Details_Items.Items.Clear();
			foreach (var definition in m_containerTypesDefinitionsManager.Definitions)
			{
				LST_ContainerTypesConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillGlobalEventConfigurationListBox(bool loadFromFile = true)
		{
			m_currentlyFillingConfigurationListBox = true;

			if(loadFromFile)
				m_globalEventsDefinitionsManager.Load(GetContentDataFile("GlobalEvents.sbc"));

			LST_GlobalEventConfig.Items.Clear();
			foreach (var definition in m_globalEventsDefinitionsManager.Definitions)
			{
				LST_GlobalEventConfig.Items.Add(definition.Name);
			}

			CMB_GlobalEventsConfig_Details_EventType.Items.Clear();
			foreach (var eventType in Enum.GetValues(typeof(MyGlobalEventTypeEnum)))
			{
				CMB_GlobalEventsConfig_Details_EventType.Items.Add(eventType);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillSpawnGroupConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_spawnGroupsDefinitionsManager.Load(GetContentDataFile("SpawnGroups.sbc"));

			LST_SpawnGroupConfig.Items.Clear();
			LST_SpawnGroupConfig_Details_Prefabs.Items.Clear();
			foreach (var definition in m_spawnGroupsDefinitionsManager.Definitions)
			{
				//TODO - Find a better way to uniquely label the spawn groups
				LST_SpawnGroupConfig.Items.Add("Spawn Group " + LST_SpawnGroupConfig.Items.Count.ToString());
			}
			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillPhysicalItemConfigurationListBox(bool loadFromFile = true)
		{
			m_currentlyFillingConfigurationListBox = true;

			if(loadFromFile)
				m_physicalItemsDefinitionsManager.Load(GetContentDataFile("PhysicalItems.sbc"));

			LST_PhysicalItemConfig.Items.Clear();
			foreach (var definition in m_physicalItemsDefinitionsManager.Definitions)
			{
				LST_PhysicalItemConfig.Items.Add(definition.Name);
			}

			CMB_PhysicalItemConfig_Details_Type.Items.Clear();
			foreach (var type in Enum.GetValues(typeof(MyObjectBuilderTypeEnum)))
			{
				CMB_PhysicalItemConfig_Details_Type.Items.Add(type);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillComponentConfigurationListBox(bool loadFromFile = true)
		{
			m_currentlyFillingConfigurationListBox = true;

			if(loadFromFile)
				m_componentsDefinitionsManager.Load(GetContentDataFile("Components.sbc"));

			LST_ComponentsConfig.Items.Clear();
			foreach (var definition in m_componentsDefinitionsManager.Definitions)
			{
				LST_ComponentsConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillBlueprintConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_blueprintsDefinitionsManager.Load(GetContentDataFile("Blueprints.sbc"));

			LST_BlueprintConfig.Items.Clear();
			foreach (var definition in m_blueprintsDefinitionsManager.Definitions)
			{
				//TODO - Find a better way to uniquely label the spawn groups
				LST_BlueprintConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillVoxelMaterialConfigurationListBox(bool loadFromFile = true)
		{
			m_currentlyFillingConfigurationListBox = true;

			if(loadFromFile)
				m_voxelMaterialsDefinitionsManager.Load(GetContentDataFile("VoxelMaterials.sbc"));

			LST_VoxelMaterialsConfig.Items.Clear();
			foreach (var definition in m_voxelMaterialsDefinitionsManager.Definitions)
			{
				LST_VoxelMaterialsConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillScenariosConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_scenariosDefinitionManager.Load(GetContentDataFile("Scenarios.sbc"));

			LST_ScenariosConfig.Items.Clear();
			foreach (var definition in m_scenariosDefinitionManager.Definitions)
			{
				LST_ScenariosConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillTransparentMaterialsConfigurationListBox(bool loadFromFile = true)
		{
			m_currentlyFillingConfigurationListBox = true;

			if(loadFromFile)
				m_transparentMaterialsDefinitionManager.Load(GetContentDataFile("TransparentMaterials.sbc"));

			LST_TransparentMaterialsConfig.Items.Clear();
			foreach (var definition in m_transparentMaterialsDefinitionManager.Definitions)
			{
				LST_TransparentMaterialsConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillEnvironmentConfigurationInfo()
		{
			m_currentlyFillingConfigurationListBox = true;

			//m_environmentDefinitionManager.Load(GetContentDataFile("Environment.sbc"));

			//EnvironmentDefinition environment = m_environmentDefinitionManager.DefinitionOf(0);

			//TXT_EnvironmentConfig_SunDirection_X.Text = environment.SunDirection.X.ToString(m_numberFormatInfo);
			//TXT_EnvironmentConfig_SunDirection_Y.Text = environment.SunDirection.Y.ToString(m_numberFormatInfo);
			//TXT_EnvironmentConfig_SunDirection_Z.Text = environment.SunDirection.Z.ToString(m_numberFormatInfo);
			//TXT_EnvironmentConfig_EnvironmentTexture.Text = environment.EnvironmentTexture;
			//TXT_EnvironmentConfig_EnvironmentOrientation_Pitch.Text = environment.EnvironmentOrientation.Pitch.ToString(m_numberFormatInfo);
			//TXT_EnvironmentConfig_EnvironmentOrientation_Roll.Text = environment.EnvironmentOrientation.Roll.ToString(m_numberFormatInfo);
			//TXT_EnvironmentConfig_EnvironmentOrientation_Yaw.Text = environment.EnvironmentOrientation.Yaw.ToString(m_numberFormatInfo);

			m_currentlyFillingConfigurationListBox = false;
		}

		#endregion

		#region Form events

		private void SEConfigTool_Load(object sender, EventArgs e)
		{
			m_standardSavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SpaceEngineers", "Saves");

			FillBlocksConfigurationListBox();
			FillAmmoConfigurationListBox();
			FillContainerTypeConfigurationListBox();
			FillGlobalEventConfigurationListBox();
			FillSpawnGroupConfigurationListBox();
			FillPhysicalItemConfigurationListBox();
			FillComponentConfigurationListBox();
			FillBlueprintConfigurationListBox();
			FillVoxelMaterialConfigurationListBox();
			FillScenariosConfigurationListBox();
			FillTransparentMaterialsConfigurationListBox();
			FillEnvironmentConfigurationInfo();
		}

		#region SavedGame

		private void BTN_SavedGame_Load_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				InitialDirectory = m_standardSavePath,
				DefaultExt = "sbs file (*.sbs)"
			};

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				FileInfo saveFileInfo = new FileInfo(openFileDialog.FileName);
				if (saveFileInfo.Exists)
				{
					try
					{
						LoadSaveFile(saveFileInfo);
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, ex.Message);
					}  
				}
			}
		}

		private void BTN_SavedGame_Save_Click(object sender, EventArgs e)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			m_sectorManager.Save();

			stopWatch.Stop();
			TLS_StatusLabel.Text = "Done saving Sector in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";

			MessageBox.Show(this, "Sector saved successfully in " + stopWatch.ElapsedMilliseconds.ToString() + "ms!");
		}

		private void BTN_SavedGame_Events_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_SavedGame_Events.SelectedIndex;

			Sector sector = m_sectorManager.Sector;
			Event sectorEvent = sector.Events[index];

			sectorEvent.Enabled = CHK_SavedGame_Events_Enabled.CheckState == CheckState.Checked;
			sectorEvent.ActivationTimeMs = Convert.ToInt64(TXT_SavedGame_Events_ActivationTime.Text, m_numberFormatInfo);

			BTN_SavedGame_Events_Apply.Enabled = false;
		}

		private void LST_SavedGame_Events_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_SavedGame_Events.SelectedIndex;

			Sector sector = m_sectorManager.Sector;
			Event sectorEvent = sector.Events[index];

			TXT_SavedGame_Events_Type.Text = sectorEvent.DefinitionId.ToString();
			CHK_SavedGame_Events_Enabled.Checked = sectorEvent.Enabled;
			TXT_SavedGame_Events_ActivationTime.Text = sectorEvent.ActivationTimeMs.ToString();

			m_currentlySelecting = false;
			BTN_SavedGame_Events_Apply.Enabled = false;
		}

		private void TRV_SavedGame_Objects_AfterSelect(object sender, TreeViewEventArgs e)
		{
			m_currentlySelecting = true;

			LBL_Sector_Objects_Field1.Visible = false;
			LBL_Sector_Objects_Field2.Visible = false;
			LBL_Sector_Objects_Field3.Visible = false;
			LBL_Sector_Objects_Field4.Visible = false;
			LBL_Sector_Objects_Field5.Visible = false;

			TXT_Sector_Objects_Field1.Visible = false;
			TXT_Sector_Objects_Field2.Visible = false;
			TXT_Sector_Objects_Field3.Visible = false;
			TXT_Sector_Objects_Field4.Visible = false;
			TXT_Sector_Objects_Field5.Visible = false;

			CMB_Sector_Objects_Field1.Visible = false;

			TXT_Sector_Objects_Field1.Enabled = false;
			TXT_Sector_Objects_Field2.Enabled = false;
			TXT_Sector_Objects_Field3.Enabled = false;
			TXT_Sector_Objects_Field4.Enabled = false;
			TXT_Sector_Objects_Field5.Enabled = false;

			BTN_Sector_Objects_New.Enabled = false;
			BTN_Sector_Objects_Apply.Enabled = false;
			BTN_Sector_Objects_Delete.Enabled = false;

			TXT_Sector_Objects_Field1.ReadOnly = true;
			TXT_Sector_Objects_Field2.ReadOnly = true;
			TXT_Sector_Objects_Field3.ReadOnly = true;
			TXT_Sector_Objects_Field4.ReadOnly = true;
			TXT_Sector_Objects_Field5.ReadOnly = true;

			CMB_Sector_Objects_Field1.Items.Clear();

			var linkedObject = e.Node.Tag;
			if (linkedObject == null)
				return;

			Type linkedType = linkedObject.GetType();

			if (linkedType.IsAssignableFrom(typeof(CubeBlock<MyObjectBuilder_CubeBlock>)))
			{
				CubeBlock<MyObjectBuilder_CubeBlock> cubeBlock = (CubeBlock<MyObjectBuilder_CubeBlock>)linkedObject;

				LBL_Sector_Objects_Field1.Visible = true;
				LBL_Sector_Objects_Field2.Visible = true;
				TXT_Sector_Objects_Field1.Visible = true;
				TXT_Sector_Objects_Field2.Visible = true;

				LBL_Sector_Objects_Field1.Text = "Type:";
				LBL_Sector_Objects_Field2.Text = "Entity Id:";
				TXT_Sector_Objects_Field1.Text = cubeBlock.SubtypeName;
				TXT_Sector_Objects_Field2.Text = cubeBlock.EntityId.ToString();
			}
			
			if (linkedType.IsAssignableFrom(typeof(CargoContainerEntity)))
			{
				CargoContainerEntity containerBlock = (CargoContainerEntity)linkedObject;

				LBL_Sector_Objects_Field1.Visible = true;
				LBL_Sector_Objects_Field2.Visible = true;
				LBL_Sector_Objects_Field3.Visible = true;
				LBL_Sector_Objects_Field4.Visible = true;
				LBL_Sector_Objects_Field5.Visible = true;

				TXT_Sector_Objects_Field1.Visible = true;
				TXT_Sector_Objects_Field2.Visible = true;
				TXT_Sector_Objects_Field3.Visible = true;
				TXT_Sector_Objects_Field4.Visible = true;
				TXT_Sector_Objects_Field5.Visible = true;

				LBL_Sector_Objects_Field1.Text = "Type:";
				LBL_Sector_Objects_Field2.Text = "Entity Id:";
				LBL_Sector_Objects_Field3.Text = "Item Count:";
				LBL_Sector_Objects_Field4.Text = "Item Volume (L):";
				LBL_Sector_Objects_Field5.Text = "Item Mass (kg):";

				float itemCount = 0;
				float itemVolume = 0;
				float itemMass = 0;
				foreach (var item in containerBlock.Inventory.Items)
				{
					itemCount += item.Amount;
					itemVolume += item.Volume;
					itemMass += item.Mass;
				}

				TXT_Sector_Objects_Field1.Text = containerBlock.SubtypeName;
				TXT_Sector_Objects_Field2.Text = containerBlock.EntityId.ToString();
				TXT_Sector_Objects_Field3.Text = itemCount.ToString();
				TXT_Sector_Objects_Field4.Text = itemVolume.ToString();
				TXT_Sector_Objects_Field5.Text = itemMass.ToString();
			}

			if (linkedType.IsAssignableFrom(typeof(ReactorEntity)))
			{
				ReactorEntity reactorBlock = (ReactorEntity)linkedObject;

				LBL_Sector_Objects_Field1.Visible = true;
				LBL_Sector_Objects_Field2.Visible = true;
				LBL_Sector_Objects_Field3.Visible = true;

				TXT_Sector_Objects_Field1.Visible = true;
				TXT_Sector_Objects_Field2.Visible = true;
				TXT_Sector_Objects_Field3.Visible = true;

				LBL_Sector_Objects_Field1.Text = "Type:";
				LBL_Sector_Objects_Field2.Text = "Entity Id:";
				LBL_Sector_Objects_Field3.Text = "Fuel (kg):";

				float fuelMass = 0;
				foreach (var item in reactorBlock.Inventory.Items)
				{
					fuelMass += item.Amount;
				}

				TXT_Sector_Objects_Field1.Text = reactorBlock.SubtypeName;
				TXT_Sector_Objects_Field2.Text = reactorBlock.EntityId.ToString();
				TXT_Sector_Objects_Field3.Text = fuelMass.ToString();
			}

			if (linkedType.IsAssignableFrom(typeof(MedicalRoomEntity)))
			{
				MedicalRoomEntity medicalBlock = (MedicalRoomEntity)linkedObject;

				LBL_Sector_Objects_Field1.Visible = true;
				LBL_Sector_Objects_Field2.Visible = true;
				LBL_Sector_Objects_Field3.Visible = true;

				TXT_Sector_Objects_Field1.Visible = true;
				TXT_Sector_Objects_Field2.Visible = true;
				TXT_Sector_Objects_Field3.Visible = true;

				TXT_Sector_Objects_Field3.Enabled = true;
				TXT_Sector_Objects_Field3.ReadOnly = false;

				LBL_Sector_Objects_Field1.Text = "Type:";
				LBL_Sector_Objects_Field2.Text = "Entity Id:";
				LBL_Sector_Objects_Field3.Text = "Steam User Id:";

				TXT_Sector_Objects_Field1.Text = medicalBlock.SubtypeName;
				TXT_Sector_Objects_Field2.Text = medicalBlock.EntityId.ToString();
				TXT_Sector_Objects_Field3.Text = medicalBlock.SteamUserId.ToString();
			}

			if (linkedType.IsAssignableFrom(typeof(CubeGrid)))
			{
				CubeGrid cubeGrid = (CubeGrid)linkedObject;

				LBL_Sector_Objects_Field1.Visible = true;
				LBL_Sector_Objects_Field2.Visible = true;

				TXT_Sector_Objects_Field1.Visible = true;
				TXT_Sector_Objects_Field2.Visible = true;

				LBL_Sector_Objects_Field1.Text = "Position:";
				LBL_Sector_Objects_Field2.Text = "Entity Id:";

				TXT_Sector_Objects_Field1.Text = cubeGrid.PositionAndOrientation.Position.x.ToString(m_numberFormatInfo) + ", " + cubeGrid.PositionAndOrientation.Position.y.ToString(m_numberFormatInfo) + ", " + cubeGrid.PositionAndOrientation.Position.z.ToString(m_numberFormatInfo);
				TXT_Sector_Objects_Field2.Text = cubeGrid.EntityId.ToString();
			}

			if (linkedType.IsAssignableFrom(typeof(InventoryItemEntity)))
			{
				InventoryItemEntity item = (InventoryItemEntity)linkedObject;

				LBL_Sector_Objects_Field1.Visible = true;
				LBL_Sector_Objects_Field2.Visible = true;

				TXT_Sector_Objects_Field2.Visible = true;

				CMB_Sector_Objects_Field1.Visible = true;

				BTN_Sector_Objects_New.Enabled = true;
				BTN_Sector_Objects_Delete.Enabled = true;

				TXT_Sector_Objects_Field2.Enabled = true;
				TXT_Sector_Objects_Field2.ReadOnly = false;

				LBL_Sector_Objects_Field1.Text = "Type:";
				LBL_Sector_Objects_Field2.Text = "Amount:";

				//Add all physical items, components, and ammo to the combo box
				CMB_Sector_Objects_Field1.Items.Clear();
				foreach (var itemType in m_physicalItemsDefinitionsManager.Definitions)
				{
					CMB_Sector_Objects_Field1.Items.Add(itemType.Id);
				}
				foreach (var itemType in m_componentsDefinitionsManager.Definitions)
				{
					CMB_Sector_Objects_Field1.Items.Add(itemType.Id);
				}
				foreach (var itemType in m_ammoMagazinesDefinitionsManager.Definitions)
				{
					CMB_Sector_Objects_Field1.Items.Add(itemType.Id);
				}

				//Select the matching type/subtype item in the list
				SerializableDefinitionId itemTypeId = new SerializableDefinitionId(item.PhysicalContent.TypeId, item.PhysicalContent.SubtypeName);
				CMB_Sector_Objects_Field1.SelectedItem = itemTypeId;

				TXT_Sector_Objects_Field2.Text = item.Amount.ToString();
			}

			m_currentlySelecting = false;
		}

		private void TXT_Sector_Objects_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_Sector_Objects_Apply.Enabled = true;
			}
		}

		private void CMB_Sector_Objects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_Sector_Objects_Apply.Enabled = true;
			}
		}

		private void BTN_Sector_Objects_Apply_Click(object sender, EventArgs e)
		{
			TreeNode selectedNode = TRV_SavedGame_Objects.SelectedNode;

			var linkedObject = selectedNode.Tag;
			if (linkedObject == null)
				return;

			Type linkedType = linkedObject.GetType();

			if (linkedType.IsAssignableFrom(typeof(MedicalRoomEntity)))
			{
				MedicalRoomEntity medicalBlock = (MedicalRoomEntity)linkedObject;

				medicalBlock.SteamUserId = Convert.ToUInt64(TXT_Sector_Objects_Field3.Text, m_numberFormatInfo);
			}

			if (linkedType.IsAssignableFrom(typeof(InventoryItemEntity)))
			{
				InventoryItemEntity itemEntity = (InventoryItemEntity)linkedObject;

				//Update the item
				itemEntity.PhysicalContent = (MyObjectBuilder_PhysicalObject)MyObjectBuilder_PhysicalObject.CreateNewObject((SerializableDefinitionId)CMB_Sector_Objects_Field1.SelectedItem);
				itemEntity.Amount = Convert.ToSingle(TXT_Sector_Objects_Field2.Text, m_numberFormatInfo);

				TreeNode parentNode = selectedNode.Parent;

				var linkedContainer = parentNode.Tag;
				if (linkedContainer == null)
					return;

				Type linkedContainerType = linkedContainer.GetType();

				if (linkedContainerType.IsAssignableFrom(typeof(CargoContainerEntity)))
				{
					CargoContainerEntity containerBlock = (CargoContainerEntity)linkedContainer;

					//Refresh the sub-item list on the container
					parentNode.Nodes.Clear();
					foreach (var item in containerBlock.Inventory.Items)
					{
						TreeNode itemNode = parentNode.Nodes.Add(item.PhysicalContent.SubtypeName + " x" + item.Amount.ToString());
						itemNode.Tag = item;
					}
				}
			}

			BTN_Sector_Objects_Apply.Enabled = false;
		}

		private void BTN_Sector_Objects_New_Click(object sender, EventArgs e)
		{
			TreeNode selectedNode = TRV_SavedGame_Objects.SelectedNode;

			var linkedObject = selectedNode.Tag;
			if (linkedObject == null)
				return;

			Type linkedType = linkedObject.GetType();

			if (linkedType.IsAssignableFrom(typeof(InventoryItemEntity)))
			{
				TreeNode parentNode = selectedNode.Parent;

				var linkedContainer = parentNode.Tag;
				if (linkedContainer == null)
					return;

				Type linkedContainerType = linkedContainer.GetType();

				if (linkedContainerType.IsAssignableFrom(typeof(CargoContainerEntity)))
				{
					CargoContainerEntity containerBlock = (CargoContainerEntity)linkedContainer;

					InventoryItemEntity newItem = containerBlock.Inventory.NewEntry();
					SerializableDefinitionId itemTypeId = new SerializableDefinitionId(MyObjectBuilderTypeEnum.Component, "SteelPlate");
					newItem.PhysicalContent = (MyObjectBuilder_PhysicalObject) MyObjectBuilder_PhysicalObject.CreateNewObject(itemTypeId);
					newItem.Amount = 1;

					parentNode.Nodes.Clear();
					foreach (var item in containerBlock.Inventory.Items)
					{
						TreeNode itemNode = parentNode.Nodes.Add(item.PhysicalContent.SubtypeName + " x" + item.Amount.ToString());
						itemNode.Tag = item;
					}
				}
			}
		}

		private void BTN_Sector_Objects_Delete_Click(object sender, EventArgs e)
		{
			TreeNode selectedNode = TRV_SavedGame_Objects.SelectedNode;

			var linkedObject = selectedNode.Tag;
			if (linkedObject == null)
				return;

			Type linkedType = linkedObject.GetType();

			if (linkedType.IsAssignableFrom(typeof(InventoryItemEntity)))
			{
				InventoryItemEntity itemEntity = (InventoryItemEntity)linkedObject;

				TreeNode parentNode = selectedNode.Parent;

				var linkedContainer = parentNode.Tag;
				if (linkedContainer == null)
					return;

				Type linkedContainerType = linkedContainer.GetType();

				if (linkedContainerType.IsAssignableFrom(typeof(CargoContainerEntity)))
				{
					CargoContainerEntity containerBlock = (CargoContainerEntity)linkedContainer;

					//Delete the item from the container
					containerBlock.Inventory.DeleteEntry(itemEntity);

					//Refresh the sub-item list on the container
					parentNode.Nodes.Clear();
					foreach (var item in containerBlock.Inventory.Items)
					{
						TreeNode itemNode = parentNode.Nodes.Add(item.PhysicalContent.SubtypeName + " x" + item.Amount.ToString());
						itemNode.Tag = item;
					}
				}
			}
		}

		private void TXT_SavedGame_Events_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SavedGame_Events_Apply.Enabled = true;
			}
		}

		private void CHK_SavedGame_Events_Enabled_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SavedGame_Events_Apply.Enabled = true;
			}
		}

		#endregion

		#region CubeBlock

		private void CHK_BlocksConfig_ModelIntersection_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_BlocksConfig_Details_Apply.Enabled = true;
			int index = LST_BlocksConfig.SelectedIndex;
			m_cubeBlockDefinitionsManager.DefinitionOf(index).UseModelIntersection = CHK_BlocksConfig_ModelIntersection.Checked;
		}
        }

		private void CHK_BlocksConfig_Enabled_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_BlocksConfig_Details_Apply.Enabled = true;
			int index = LST_BlocksConfig.SelectedIndex;
			m_cubeBlockDefinitionsManager.DefinitionOf(index).Enabled = CHK_BlocksConfig_Enabled.Checked;
		}
        }

		private void LST_BlocksConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_BlocksConfig.SelectedIndex;

			CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsManager.DefinitionOf(index);

			TXT_BlocksConfig_Details_Name.Text = cubeBlock.Name;
			TXT_BlocksConfig_Details_Id.Text = cubeBlock.Id.ToString();
			TXT_BlocksConfig_Details_BuildTime.Text = cubeBlock.BuildTime.ToString(m_numberFormatInfo);
			TXT_BlocksConfig_Details_DisassembleRatio.Text = cubeBlock.DisassembleRatio.ToString(m_numberFormatInfo);
			CHK_BlocksConfig_Enabled.Checked = cubeBlock.Enabled;
			CHK_BlocksConfig_ModelIntersection.Checked = cubeBlock.UseModelIntersection;

			DGV_BlocksConfig_Details_Components.DataSource = cubeBlock.Components.ToArray().Select(x => new { x.Subtype, x.Count }).ToArray();

			m_currentlySelecting = false;

			BTN_BlocksConfig_Details_Apply.Enabled = false;
		}

		private void TXT_ConfigBuildTime_KeyPress(object sender, KeyPressEventArgs e)
		{
			char ch = e.KeyChar;

			if (ch == '.' && TXT_BlocksConfig_Details_BuildTime.Text.IndexOf('.') != -1)
			{
				e.Handled = true;
			}

			if (!Char.IsDigit(ch) && ch != m_decimalSeparator[0] && ch != 8) //8 for ASCII (backspace)
			{
				e.Handled = true;
			}
		}

		private void BTN_ConfigReload_Click(object sender, EventArgs e)
		{
			FillBlocksConfigurationListBox();
		}

		private void BTN_SaveBlocksConfiguration_Click(object sender, EventArgs e)
		{
			m_cubeBlockDefinitionsManager.Save();
		}

		private void TXT_ConfigBlocks_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_BlocksConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_ConfigApplyChanges_Click(object sender, EventArgs e)
		{
			int index = LST_BlocksConfig.SelectedIndex;

			CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsManager.DefinitionOf(index);

			cubeBlock.BuildTime = Convert.ToSingle(TXT_BlocksConfig_Details_BuildTime.Text, m_numberFormatInfo);
			cubeBlock.DisassembleRatio = Convert.ToSingle(TXT_BlocksConfig_Details_DisassembleRatio.Text, m_numberFormatInfo);

			BTN_BlocksConfig_Details_Apply.Enabled = false;
		}

		private void BTN_BlocksConfig_Details_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_BlocksConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#endregion

		#region AmmoMagazines

		private void LST_AmmoConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;

			if (LST_AmmoConfig.SelectedIndex < 0)
			{
				BTN_AmmoConfig_Details_Delete.Enabled = false;
				return;
			}

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(LST_AmmoConfig.SelectedIndex);

			CMB_AmmoConfig_Details_Caliber.Items.Clear();
			foreach (var caliber in Enum.GetValues(typeof(MyAmmoCategoryEnum)))
			{
				CMB_AmmoConfig_Details_Caliber.Items.Add(caliber);
			}

			TXT_AmmoConfig_Details_Id.Text = ammoMagazine.Id.SubtypeId;
			TXT_AmmoConfig_Details_Name.Text = ammoMagazine.Name;
			TXT_AmmoConfig_Details_Description.Text = ammoMagazine.Description;
			TXT_AmmoConfig_Details_Icon.Text = ammoMagazine.Icon;
			TXT_AmmoConfig_Details_Model.Text = ammoMagazine.Model;
			CMB_AmmoConfig_Details_Caliber.SelectedItem = ammoMagazine.Caliber;
			TXT_AmmoConfig_Details_Capacity.Text = ammoMagazine.Capacity.ToString(m_numberFormatInfo);
			TXT_AmmoConfig_Details_Volume.Text = ammoMagazine.Volume.ToString(m_numberFormatInfo);
			TXT_AmmoConfig_Details_Mass.Text = ammoMagazine.Mass.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;

			BTN_AmmoConfig_Details_Apply.Enabled = false;
			BTN_AmmoConfig_Details_Delete.Enabled = true;
		}

		private void BTN_ConfigAmmoReload_Click(object sender, EventArgs e)
		{
			FillAmmoConfigurationListBox();
		}

		private void BTN_SaveAmmoConfig_Click(object sender, EventArgs e)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			bool saveResult = m_ammoMagazinesDefinitionsManager.Save();

			stopWatch.Stop();

			if(!saveResult)
			{
				MessageBox.Show(this, "Failed to save AmmoMagazines config!");
				return;
			}

			TLS_StatusLabel.Text = "Done saving AmmoMagazines in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";
		}

		private void TXT_ConfigAmmo_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_AmmoConfig_Details_Apply.Enabled = true;
			}
		}

		private void CMB_AmmoConfig_Details_Caliber_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_AmmoConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_ConfigAmmoApply_Click(object sender, EventArgs e)
		{
			if (LST_AmmoConfig.SelectedIndex < 0) return;

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(LST_AmmoConfig.SelectedIndex);

			ammoMagazine.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.AmmoMagazine, TXT_AmmoConfig_Details_Id.Text);
			ammoMagazine.Name = TXT_AmmoConfig_Details_Name.Text;
			ammoMagazine.DisplayName = TXT_AmmoConfig_Details_Name.Text;
			ammoMagazine.Description = TXT_AmmoConfig_Details_Description.Text;
			ammoMagazine.Icon = TXT_AmmoConfig_Details_Icon.Text;
			ammoMagazine.Model = TXT_AmmoConfig_Details_Model.Text;
			ammoMagazine.Caliber = (MyAmmoCategoryEnum)CMB_AmmoConfig_Details_Caliber.SelectedItem;
			ammoMagazine.Capacity = Convert.ToInt32(TXT_AmmoConfig_Details_Capacity.Text, m_numberFormatInfo);
			ammoMagazine.Mass = Convert.ToSingle(TXT_AmmoConfig_Details_Mass.Text, m_numberFormatInfo);
			ammoMagazine.Volume = Convert.ToSingle(TXT_AmmoConfig_Details_Volume.Text, m_numberFormatInfo);

			BTN_AmmoConfig_Details_Apply.Enabled = false;
		}

		private void BTN_AmmoConfig_Details_New_Click(object sender, EventArgs e)
		{
			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.NewEntry();
			if (ammoMagazine == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			ammoMagazine.Name = "(New)";
			ammoMagazine.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.AmmoMagazine, "NewSubtype");

			FillAmmoConfigurationListBox(false);

			LST_AmmoConfig.SelectedIndex = LST_AmmoConfig.Items.Count - 1;
		}

		private void BTN_AmmoConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			if (LST_AmmoConfig.SelectedIndex < 0) return;

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(LST_AmmoConfig.SelectedIndex);

			bool deleteResult = m_ammoMagazinesDefinitionsManager.DeleteEntry(ammoMagazine);
			if (!deleteResult)
			{
				MessageBox.Show(this, "Failed to delete AmmoMagazines entry!");
				return;
			}

			FillAmmoConfigurationListBox(false);
		}

		#endregion

		#region ContainerTypes

		private void LST_ContainerTypeConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ContainerTypesConfig.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(index);

			TXT_ContainerTypeConfig_Details_Information_Name.Text = containerType.Name;
			TXT_ContainerTypeConfig_Details_Information_CountMin.Text = containerType.CountMin.ToString();
			TXT_ContainerTypeConfig_Details_Information_CountMax.Text = containerType.CountMax.ToString();

			LST_ContainerTypeConfig_Details_Items.Items.Clear();
			foreach (var def in containerType.Items)
			{
				LST_ContainerTypeConfig_Details_Items.Items.Add(def.Id.ToString());
			}

			//Add all physical items, components, and ammo to the combo box
			CMB_ContainerTypeConfig_Items_Type.Items.Clear();
			foreach (var itemType in m_physicalItemsDefinitionsManager.Definitions)
			{
				CMB_ContainerTypeConfig_Items_Type.Items.Add(itemType.Id);
			}
			foreach (var itemType in m_componentsDefinitionsManager.Definitions)
			{
				CMB_ContainerTypeConfig_Items_Type.Items.Add(itemType.Id);
			}
			foreach (var itemType in m_ammoMagazinesDefinitionsManager.Definitions)
			{
				CMB_ContainerTypeConfig_Items_Type.Items.Add(itemType.Id);
			}

			TXT_ContainerTypeConfig_Item_AmountMin.Text = "";
			TXT_ContainerTypeConfig_Item_AmountMax.Text = "";
			TXT_ContainerTypeConfig_Item_Frequency.Text = "";

			m_currentlySelecting = false;

			BTN_ContainerTypesConfig_Details_Apply.Enabled = false;
		}

		private void BTN_ConfigContainerTypeReload_Click(object sender, EventArgs e)
		{
			FillContainerTypeConfigurationListBox();
		}

		private void BTN_SaveContainerTypeConfig_Click(object sender, EventArgs e)
		{
			m_containerTypesDefinitionsManager.Save();
		}

		private void BTN_ConfigContainerTypeApply_Click(object sender, EventArgs e)
		{
			int index = LST_ContainerTypesConfig.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(index);

			containerType.Name = TXT_ContainerTypeConfig_Details_Information_Name.Text;
			containerType.CountMin = Convert.ToInt32(TXT_ContainerTypeConfig_Details_Information_CountMin.Text, m_numberFormatInfo);
			containerType.CountMax = Convert.ToInt32(TXT_ContainerTypeConfig_Details_Information_CountMax.Text, m_numberFormatInfo);

			BTN_ContainerTypesConfig_Details_Apply.Enabled = false;
		}

		private void TXT_ConfigContainerType_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ContainerTypesConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_ContainerTypesConfig_Details_New_Click(object sender, EventArgs e)
		{
			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.NewEntry();
			if (containerType == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			containerType.Name = "(New)";
			containerType.CountMin = 1;
			containerType.CountMax = 1;

			FillContainerTypeConfigurationListBox(false);

			LST_ContainerTypesConfig.SelectedIndex = LST_ContainerTypesConfig.Items.Count - 1;
		}

		private void BTN_ContainerTypesConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#region Items

		private void LST_ContainerTypeConfiguration_Items_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ContainerTypeConfig_Details_Items.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LST_ContainerTypesConfig.SelectedIndex);
			ContainerTypeItem containerItem = containerType.Items[index];

			CMB_ContainerTypeConfig_Items_Type.SelectedItem = containerItem.Id;

			TXT_ContainerTypeConfig_Item_AmountMin.Text = containerItem.AmountMin.ToString(m_numberFormatInfo);
			TXT_ContainerTypeConfig_Item_AmountMax.Text = containerItem.AmountMax.ToString(m_numberFormatInfo);
			TXT_ContainerTypeConfig_Item_Frequency.Text = containerItem.Frequency.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;

			BTN_ContainerTypeConfig_Items_Apply.Enabled = false;
		}

		private void BTN_ContainerTypeConfig_Items_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_ContainerTypeConfig_Details_Items.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LST_ContainerTypesConfig.SelectedIndex);
			ContainerTypeItem containerItem = containerType.Items[index];

			containerItem.Id = (SerializableDefinitionId) CMB_ContainerTypeConfig_Items_Type.SelectedItem;
			containerItem.AmountMin = Convert.ToInt32(TXT_ContainerTypeConfig_Item_AmountMin.Text, m_numberFormatInfo);
			containerItem.AmountMax = Convert.ToInt32(TXT_ContainerTypeConfig_Item_AmountMax.Text, m_numberFormatInfo);
			containerItem.Frequency = Convert.ToSingle(TXT_ContainerTypeConfig_Item_Frequency.Text, m_numberFormatInfo);

			BTN_ContainerTypeConfig_Items_Apply.Enabled = false;

			int currentIndex = LST_ContainerTypeConfig_Details_Items.SelectedIndex;
			LST_ContainerTypeConfig_Details_Items.Items.Clear();
			foreach (var def in containerType.Items)
			{
				LST_ContainerTypeConfig_Details_Items.Items.Add(def.Id.ToString());
			}
			LST_ContainerTypeConfig_Details_Items.SelectedIndex = currentIndex;
		}

		private void TXT_ContainerTypeConfig_Item_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ContainerTypeConfig_Items_Apply.Enabled = true;
			}
		}

		private void CMB_ContainerTypesConfig_Items_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ContainerTypeConfig_Items_Apply.Enabled = true;
			}
		}

		private void BTN_ContainerTypeConfig_Items_New_Click(object sender, EventArgs e)
		{
			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LST_ContainerTypesConfig.SelectedIndex);
			ContainerTypeItem containerItem = containerType.ItemsManager.NewEntry();
			if (containerItem == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			//Set some default values for the new entry
			containerItem.Name = "(New)";
			containerItem.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.Ore, "Stone");
			containerItem.AmountMin = 1;
			containerItem.AmountMax = 1;

			LST_ContainerTypeConfig_Details_Items.Items.Clear();
			foreach (var def in containerType.Items)
			{
				LST_ContainerTypeConfig_Details_Items.Items.Add(def.Id.ToString());
			}

			LST_ContainerTypeConfig_Details_Items.SelectedIndex = LST_ContainerTypeConfig_Details_Items.Items.Count - 1;
		}

		private void BTN_ContainerTypeConfig_Items_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#endregion

		#endregion

		#region GlobalEvents

		private void LST_GlobalEventConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;

			if (LST_GlobalEventConfig.SelectedIndex < 0)
			{
				BTN_GlobalEventConfig_Details_Delete.Enabled = false;
				return;
			}

			GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsManager.DefinitionOf(LST_GlobalEventConfig.SelectedIndex);

			TXT_ConfigGlobalEvent_Details_Name.Text = globalEvent.Name;
			TXT_ConfigGlobalEvent_Details_Description.Text = globalEvent.Description;
			CMB_GlobalEventsConfig_Details_EventType.SelectedItem = globalEvent.EventType;
			TXT_ConfigGlobalEvent_Details_MinActivation.Text = globalEvent.MinActivation.ToString(m_numberFormatInfo);
			TXT_ConfigGlobalEvent_Details_MaxActivation.Text = globalEvent.MaxActivation.ToString(m_numberFormatInfo);
			TXT_ConfigGlobalEvent_Details_FirstActivation.Text = globalEvent.FirstActivation.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;

			BTN_GlobalEventConfig_Details_Apply.Enabled = false;
			BTN_GlobalEventConfig_Details_Delete.Enabled = true;
		}

		private void BTN_ConfigGlobalEventReload_Click(object sender, EventArgs e)
		{
			FillGlobalEventConfigurationListBox();
		}

		private void BTN_SaveGlobalEventConfig_Click(object sender, EventArgs e)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			bool saveResult = m_globalEventsDefinitionsManager.Save();

			stopWatch.Stop();

			if (!saveResult)
			{
				MessageBox.Show(this, "Failed to save GlobalEvents config!");
				return;
			}

			TLS_StatusLabel.Text = "Done saving GlobalEvents in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";
		}

		private void BTN_ConfigGlobalEventApply_Click(object sender, EventArgs e)
		{
			int index = LST_GlobalEventConfig.SelectedIndex;

			GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsManager.DefinitionOf(index);

			globalEvent.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.EventDefinition, ((MyGlobalEventTypeEnum)CMB_GlobalEventsConfig_Details_EventType.SelectedItem).ToString());
			globalEvent.Name = TXT_ConfigGlobalEvent_Details_Name.Text;
			globalEvent.DisplayName = globalEvent.Name;
			globalEvent.Description = TXT_ConfigGlobalEvent_Details_Description.Text;
			globalEvent.EventType = (MyGlobalEventTypeEnum) CMB_GlobalEventsConfig_Details_EventType.SelectedItem;
			globalEvent.MinActivation = Convert.ToInt32(TXT_ConfigGlobalEvent_Details_MinActivation.Text, m_numberFormatInfo);
			globalEvent.MaxActivation = Convert.ToInt32(TXT_ConfigGlobalEvent_Details_MaxActivation.Text, m_numberFormatInfo);
			globalEvent.FirstActivation = Convert.ToInt32(TXT_ConfigGlobalEvent_Details_FirstActivation.Text, m_numberFormatInfo);

			BTN_GlobalEventConfig_Details_Apply.Enabled = false;
		}

		private void TXT_ConfigGlobalEvent_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_GlobalEventConfig_Details_Apply.Enabled = true;
			}
		}

		private void CMB_GlobalEventsConfig_Details_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_GlobalEventConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_GlobalEventsConfig_Details_New_Click(object sender, EventArgs e)
		{
			GlobalEventsDefinition newGlobalEventDef = m_globalEventsDefinitionsManager.NewEntry();
			if (newGlobalEventDef == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			newGlobalEventDef.Name = "(New)";
			newGlobalEventDef.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.EventDefinition, MyGlobalEventTypeEnum.InvalidEventType.ToString());

			FillGlobalEventConfigurationListBox(false);

			LST_GlobalEventConfig.SelectedIndex = LST_GlobalEventConfig.Items.Count - 1;
		}

		private void BTN_GlobalEventConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			if (LST_GlobalEventConfig.SelectedIndex < 0) return;

			GlobalEventsDefinition itemToDelete = m_globalEventsDefinitionsManager.DefinitionOf(LST_GlobalEventConfig.SelectedIndex);

			bool deleteResult = m_globalEventsDefinitionsManager.DeleteEntry(itemToDelete);
			if (!deleteResult)
			{
				MessageBox.Show(this, "Failed to delete GlobalEvents entry!");
				return;
			}

			FillGlobalEventConfigurationListBox(false);
		}

		#endregion

		#region SpawnGroups

		private void LST_SpawnGroupConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_SpawnGroupConfig.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(index);

			TXT_SpawnGroupConfig_Details_Info_Count.Text = spawnGroup.PrefabCount.ToString();
			TXT_SpawnGroupConfig_Details_Info_Frequency.Text = spawnGroup.Frequency.ToString(m_numberFormatInfo);

			LST_SpawnGroupConfig_Details_Prefabs.Items.Clear();
			foreach (var def in spawnGroup.Prefabs)
			{
				LST_SpawnGroupConfig_Details_Prefabs.Items.Add(def.BeaconText);
			}
			TXT_SpawnGroupConfig_Details_Prefabs_File.Text = "";
			TXT_SpawnGroupConfig_Details_Prefabs_Position.Text = "";
			TXT_SpawnGroupConfig_Details_Prefabs_BeaconText.Text = "";
			TXT_SpawnGroupConfig_Details_Prefabs_Speed.Text = "";

			m_currentlySelecting = false;

			BTN_SpawnGroupConfig_Details_Info_Apply.Enabled = false;
		}

		private void BTN_ConfigSpawnGroupReload_Click(object sender, EventArgs e)
		{
			FillSpawnGroupConfigurationListBox();
		}

		private void BTN_SaveSpawnGroupConfig_Click(object sender, EventArgs e)
		{
			m_spawnGroupsDefinitionsManager.Save();
		}

		private void BTN_ConfigSpawnGroupApply_Click(object sender, EventArgs e)
		{
			int index = LST_SpawnGroupConfig.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(index);

			spawnGroup.Frequency = Convert.ToSingle(TXT_SpawnGroupConfig_Details_Info_Frequency.Text, m_numberFormatInfo);

			BTN_SpawnGroupConfig_Details_Info_Apply.Enabled = false;
		}

		private void TXT_ConfigSpawnGroup_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SpawnGroupConfig_Details_Info_Apply.Enabled = true;
			}
		}

		private void BTN_SpawnGroupConfig_Details_Info_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_SpawnGroupConfig_Details_Info_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#region Prefabs

		private void LST_SpawnGroupConfig_Details_Prefabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(LST_SpawnGroupConfig.SelectedIndex);
			SpawnGroupPrefab spawnGroupPrefab = spawnGroup.Prefabs[index];

			TXT_SpawnGroupConfig_Details_Prefabs_File.Text = spawnGroupPrefab.File;
			TXT_SpawnGroupConfig_Details_Prefabs_Position.Text = spawnGroupPrefab.Position.ToString();
			TXT_SpawnGroupConfig_Details_Prefabs_BeaconText.Text = spawnGroupPrefab.BeaconText;
			TXT_SpawnGroupConfig_Details_Prefabs_Speed.Text = spawnGroupPrefab.Speed.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;

			BTN_SpawnGroupConfig_Details_Prefabs_Apply.Enabled = false;
		}

		private void BTN_SpawnGroupConfig_Prefabs_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(LST_SpawnGroupConfig.SelectedIndex);
			SpawnGroupPrefab spawnGroupPrefab = spawnGroup.Prefabs[index];

			spawnGroupPrefab.BeaconText = TXT_SpawnGroupConfig_Details_Prefabs_BeaconText.Text;
			spawnGroupPrefab.Speed = Convert.ToSingle(TXT_SpawnGroupConfig_Details_Prefabs_Speed.Text, m_numberFormatInfo);

			BTN_SpawnGroupConfig_Details_Prefabs_Apply.Enabled = false;
		}

		private void TXT_SpawnGroupConfig_Details_PrefabText_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SpawnGroupConfig_Details_Prefabs_Apply.Enabled = true;
			}
		}

		private void BTN_SpawnGroupConfig_Details_Prefabs_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_SpawnGroupConfig_Details_Prefabs_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#endregion

		#endregion

		#region PhysicalItems

		private void LST_PhysicalItemConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;

			if (LST_PhysicalItemConfig.SelectedIndex < 0)
			{
				BTN_PhysicalItemConfig_Details_Delete.Enabled = false;
				return;
			}

			PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.DefinitionOf(LST_PhysicalItemConfig.SelectedIndex);

			CMB_PhysicalItemConfig_Details_Type.SelectedItem = physicalItem.Id.TypeId;
			TXT_PhysicalItemConfig_Details_Id.Text = physicalItem.Id.SubtypeId;
			TXT_PhysicalItemConfig_Details_Name.Text = physicalItem.Name;
			TXT_PhysicalItemConfig_Details_Description.Text = physicalItem.Description;
			TXT_PhysicalItemConfig_Details_Icon.Text = physicalItem.Icon;
			TXT_PhysicalItemConfig_Details_Model.Text = physicalItem.Model;
			TXT_PhysicalItemConfig_Details_IconSymbol.Text = physicalItem.IconSymbol.ToString();
			TXT_PhysicalItemConfig_Details_Size_X.Text = physicalItem.Size.X.ToString(m_numberFormatInfo);
			TXT_PhysicalItemConfig_Details_Size_Y.Text = physicalItem.Size.Y.ToString(m_numberFormatInfo);
			TXT_PhysicalItemConfig_Details_Size_Z.Text = physicalItem.Size.Z.ToString(m_numberFormatInfo);
			TXT_PhysicalItemConfig_Details_Mass.Text = physicalItem.Mass.ToString(m_numberFormatInfo);
			TXT_PhysicalItemConfig_Details_Volume.Text = physicalItem.Volume.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;

			BTN_PhysicalItemConfig_Details_Apply.Enabled = false;
			BTN_PhysicalItemConfig_Details_Delete.Enabled = true;
		}

		private void BTN_ConfigPhysicalItemReload_Click(object sender, EventArgs e)
		{
			FillPhysicalItemConfigurationListBox();
		}

		private void BTN_SavePhysicalItemConfig_Click(object sender, EventArgs e)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			bool saveResult = m_physicalItemsDefinitionsManager.Save();

			stopWatch.Stop();

			if (!saveResult)
			{
				MessageBox.Show(this, "Failed to save PhysicalItems config!");
				return;
			}

			TLS_StatusLabel.Text = "Done saving PhysicalItems in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";
		}

		private void BTN_ConfigPhysicalItemApply_Click(object sender, EventArgs e)
		{
			int index = LST_PhysicalItemConfig.SelectedIndex;

			PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.DefinitionOf(index);

			physicalItem.Id = new SerializableDefinitionId((MyObjectBuilderTypeEnum) CMB_PhysicalItemConfig_Details_Type.SelectedItem, TXT_PhysicalItemConfig_Details_Id.Text);
			physicalItem.Name = TXT_PhysicalItemConfig_Details_Name.Text;
			physicalItem.DisplayName = TXT_PhysicalItemConfig_Details_Name.Text;
			physicalItem.Description = TXT_PhysicalItemConfig_Details_Description.Text;
			physicalItem.Icon = TXT_PhysicalItemConfig_Details_Icon.Text;
			physicalItem.Model = TXT_PhysicalItemConfig_Details_Model.Text;
			MyTextsWrapperEnum iconSymbol;
			Enum.TryParse<MyTextsWrapperEnum>(TXT_PhysicalItemConfig_Details_IconSymbol.Text, true, out iconSymbol);
			physicalItem.IconSymbol = iconSymbol;
			physicalItem.Size = new Vector3(Convert.ToSingle(TXT_PhysicalItemConfig_Details_Size_X.Text, m_numberFormatInfo), Convert.ToSingle(TXT_PhysicalItemConfig_Details_Size_Y.Text, m_numberFormatInfo), Convert.ToSingle(TXT_PhysicalItemConfig_Details_Size_Z.Text, m_numberFormatInfo));
			physicalItem.Mass = Convert.ToSingle(TXT_PhysicalItemConfig_Details_Mass.Text, m_numberFormatInfo);
			physicalItem.Volume = Convert.ToSingle(TXT_PhysicalItemConfig_Details_Volume.Text, m_numberFormatInfo);

			BTN_PhysicalItemConfig_Details_Apply.Enabled = false;
		}

		private void TXT_PhysicalItemConfig_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_PhysicalItemConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_PhysicalItemConfig_Details_New_Click(object sender, EventArgs e)
		{
			PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.NewEntry();
			if (physicalItem == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			physicalItem.Name = "(New)";
			physicalItem.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.Ore, "NewSubtype");

			FillPhysicalItemConfigurationListBox(false);

			LST_PhysicalItemConfig.SelectedIndex = LST_PhysicalItemConfig.Items.Count - 1;
		}

		private void BTN_PhysicalItemConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			if (LST_PhysicalItemConfig.SelectedIndex < 0) return;

			PhysicalItemsDefinition itemToDelete = m_physicalItemsDefinitionsManager.DefinitionOf(LST_PhysicalItemConfig.SelectedIndex);

			bool deleteResult = m_physicalItemsDefinitionsManager.DeleteEntry(itemToDelete);
			if (!deleteResult)
			{
				MessageBox.Show(this, "Failed to delete PhysicalItems entry!");
				return;
			}

			FillPhysicalItemConfigurationListBox(false);
		}

		#endregion

		#region Components

		private void LST_ComponentsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;

			if (LST_ComponentsConfig.SelectedIndex < 0)
			{
				BTN_ComponentConfig_Details_Delete.Enabled = false;
				return;
			}

			ComponentsDefinition component = m_componentsDefinitionsManager.DefinitionOf(LST_ComponentsConfig.SelectedIndex);

			TXT_ComponentConfig_Details_Id.Text = component.Id.SubtypeId;
			TXT_ComponentConfig_Details_Name.Text = component.Name;
			TXT_ComponentConfig_Details_Description.Text = component.Description;
			TXT_ComponentConfig_Details_Icon.Text = component.Icon;
			TXT_ComponentConfig_Details_Model.Text = component.Model;
			TXT_ComponentConfig_Details_Size_X.Text = component.Size.X.ToString(m_numberFormatInfo);
			TXT_ComponentConfig_Details_Size_Y.Text = component.Size.Y.ToString(m_numberFormatInfo);
			TXT_ComponentConfig_Details_Size_Z.Text = component.Size.Z.ToString(m_numberFormatInfo);
			TXT_ComponentConfig_Details_Mass.Text = component.Mass.ToString(m_numberFormatInfo);
			TXT_ComponentConfig_Details_Volume.Text = component.Volume.ToString(m_numberFormatInfo);
			TXT_ComponentConfig_Details_MaxIntegrity.Text = component.MaxIntegrity.ToString();
			TXT_ComponentConfig_Details_DropProbability.Text = component.DropProbability.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;

			BTN_ComponentConfig_Details_Apply.Enabled = false;
			BTN_ComponentConfig_Details_Delete.Enabled = true;
		}

		private void BTN_ComponentConfig_Reload_Click(object sender, EventArgs e)
		{
			FillComponentConfigurationListBox();
		}

		private void BTN_ComponentConfig_Save_Click(object sender, EventArgs e)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			bool saveResult = m_componentsDefinitionsManager.Save();

			stopWatch.Stop();

			if (!saveResult)
			{
				MessageBox.Show(this, "Failed to save Components config!");
				return;
			}

			TLS_StatusLabel.Text = "Done saving Components in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";
		}

		private void BTN_ComponentConfig_Details_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_ComponentsConfig.SelectedIndex;

			ComponentsDefinition component = m_componentsDefinitionsManager.DefinitionOf(index);

			component.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.Component, TXT_ComponentConfig_Details_Id.Text);
			component.Name = TXT_ComponentConfig_Details_Name.Text;
			component.DisplayName = TXT_ComponentConfig_Details_Name.Text;
			component.Description = TXT_ComponentConfig_Details_Description.Text;
			component.Icon = TXT_ComponentConfig_Details_Icon.Text;
			component.Model = TXT_ComponentConfig_Details_Model.Text;
			component.Size = new Vector3(Convert.ToSingle(TXT_ComponentConfig_Details_Size_X.Text, m_numberFormatInfo), Convert.ToSingle(TXT_ComponentConfig_Details_Size_Y.Text, m_numberFormatInfo), Convert.ToSingle(TXT_ComponentConfig_Details_Size_Z.Text, m_numberFormatInfo));
			component.Mass = Convert.ToSingle(TXT_ComponentConfig_Details_Mass.Text, m_numberFormatInfo);
			component.Volume = Convert.ToSingle(TXT_ComponentConfig_Details_Volume.Text, m_numberFormatInfo);
			component.MaxIntegrity = Convert.ToInt32(TXT_ComponentConfig_Details_MaxIntegrity.Text, m_numberFormatInfo);
			component.DropProbability = Convert.ToSingle(TXT_ComponentConfig_Details_DropProbability.Text, m_numberFormatInfo);

			BTN_ComponentConfig_Details_Apply.Enabled = false;
		}

		private void TXT_ComponentConfig_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ComponentConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_ComponentConfig_Details_New_Click(object sender, EventArgs e)
		{
			ComponentsDefinition component = m_componentsDefinitionsManager.NewEntry();
			if (component == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			component.Name = "(New)";
			component.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.Component, "NewSubtype");

			FillComponentConfigurationListBox(false);

			LST_ComponentsConfig.SelectedIndex = LST_ComponentsConfig.Items.Count - 1;
		}

		private void BTN_ComponentConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			if (LST_ComponentsConfig.SelectedIndex < 0) return;

			ComponentsDefinition itemToDelete = m_componentsDefinitionsManager.DefinitionOf(LST_ComponentsConfig.SelectedIndex);

			bool deleteResult = m_componentsDefinitionsManager.DeleteEntry(itemToDelete);
			if (!deleteResult)
			{
				MessageBox.Show(this, "Failed to delete Components entry!");
				return;
			}

			FillComponentConfigurationListBox(false);
		}

		#endregion

		#region Blueprints

		private void LST_BlueprintConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_BlueprintConfig.SelectedIndex;

			BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(index);

			TXT_BlueprintConfig_Details_Result_SubtypeId.Text = blueprint.Result.SubTypeId;
			TXT_BlueprintConfig_Details_Result_Amount.Text = blueprint.Result.Amount.ToString(m_numberFormatInfo);
			TXT_BlueprintConfig_Details_Result_BaseProductionTime.Text = blueprint.BaseProductionTimeInSeconds.ToString(m_numberFormatInfo);

			LST_BlueprintConfig_Details_Prerequisites.Items.Clear();
			foreach (var prereq in blueprint.Prerequisites)
			{
				LST_BlueprintConfig_Details_Prerequisites.Items.Add(prereq.Name);
			}

			CMB_BlueprintConfig_Details_Result_TypeId.DataSource = Enum.GetValues(typeof(MyObjectBuilderTypeEnum));
			CMB_BlueprintConfig_Details_Prerequisites_TypeId.DataSource = Enum.GetValues(typeof(MyObjectBuilderTypeEnum));

			CMB_BlueprintConfig_Details_Result_TypeId.SelectedItem = blueprint.Result.TypeId;
			CMB_BlueprintConfig_Details_Prerequisites_TypeId.SelectedIndex = -1;

			m_currentlySelecting = false;

			BTN_BlueprintConfig_Details_Result_Apply.Enabled = false;
			BTN_BlueprintConfig_Details_Prerequisites_Apply.Enabled = false;
		}

		private void BTN_BlueprintConfig_Reload_Click(object sender, EventArgs e)
		{
			FillBlueprintConfigurationListBox();
		}

		private void BTN_BlueprintConfig_Save_Click(object sender, EventArgs e)
		{
			m_blueprintsDefinitionsManager.Save();
		}

		private void BTN_BlueprintConfig_Details_Result_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_BlueprintConfig.SelectedIndex;
			BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(index);

			blueprint.Result.TypeId = (MyObjectBuilderTypeEnum)CMB_BlueprintConfig_Details_Result_TypeId.SelectedValue;
			blueprint.Result.SubTypeId = TXT_BlueprintConfig_Details_Prerequisites_SubtypeId.Text;
			blueprint.Result.Amount = Convert.ToDecimal(TXT_BlueprintConfig_Details_Result_Amount.Text, m_numberFormatInfo);
			blueprint.BaseProductionTimeInSeconds = Convert.ToSingle(TXT_BlueprintConfig_Details_Result_BaseProductionTime.Text, m_numberFormatInfo);

			BTN_BlueprintConfig_Details_Result_Apply.Enabled = false;
		}

		private void TXT_BlueprintConfig_Details_Result_TextChanged(object sender, EventArgs e)
		{
			BTN_BlueprintConfig_Details_Result_Apply.Enabled = true;
		}

		private void CMB_BlueprintConfig_Details_Result_TypeId_SelectedIndexChanged(object sender, EventArgs e)
		{
			BTN_BlueprintConfig_Details_Result_Apply.Enabled = true;
		}

		private void BTN_BlueprintConfig_Details_Result_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_BlueprintConfig_Details_Result_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#region Prerequisites

		private void LST_BlueprintConfig_Details_Prerequisites_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int blueprintIndex = LST_BlueprintConfig.SelectedIndex;
			int prereqIndex = LST_BlueprintConfig_Details_Prerequisites.SelectedIndex;

			BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(blueprintIndex);
			BlueprintItemDefinition prereq = blueprint.Prerequisites[prereqIndex];

			CMB_BlueprintConfig_Details_Prerequisites_TypeId.SelectedItem = prereq.TypeId;
			TXT_BlueprintConfig_Details_Prerequisites_SubtypeId.Text = prereq.SubTypeId;
			TXT_BlueprintConfig_Details_Prerequisites_Amount.Text = prereq.Amount.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;
			BTN_BlueprintConfig_Details_Prerequisites_Apply.Enabled = false;
		}

		private void BTN_BlueprintConfig_Details_Prerequisites_Apply_Click(object sender, EventArgs e)
		{
			int blueprintIndex = LST_BlueprintConfig.SelectedIndex;
			int prereqIndex = LST_BlueprintConfig_Details_Prerequisites.SelectedIndex;

			BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(blueprintIndex);
			BlueprintItemDefinition prereq = blueprint.Prerequisites[prereqIndex];

			prereq.TypeId = (MyObjectBuilderTypeEnum)CMB_BlueprintConfig_Details_Prerequisites_TypeId.SelectedValue;
			prereq.SubTypeId = TXT_BlueprintConfig_Details_Prerequisites_SubtypeId.Text;
			prereq.Amount = Convert.ToDecimal(TXT_BlueprintConfig_Details_Prerequisites_Amount.Text, m_numberFormatInfo);

			BTN_BlueprintConfig_Details_Prerequisites_Apply.Enabled = false;
		}


		private void TXT_BlueprintConfig_Details_Prerequisites_TextChanged(object sender, EventArgs e)
		{
			BTN_BlueprintConfig_Details_Prerequisites_Apply.Enabled = true;
		}

		private void CMB_BlueprintConfig_Details_Prerequisites_TypeId_SelectedIndexChanged(object sender, EventArgs e)
		{
			BTN_BlueprintConfig_Details_Prerequisites_Apply.Enabled = true;
		}

		private void BTN_BlueprintConfig_Details_Prerequisites_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_BlueprintConfig_Details_Prerequisites_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#endregion

		#endregion

		#region VoxelMaterials

		private void LST_VoxelMaterialsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;

			if (LST_VoxelMaterialsConfig.SelectedIndex < 0)
			{
				BTN_VoxelMaterialsConfig_Details_Delete.Enabled = false;
				return;
			}

			VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsManager.DefinitionOf(LST_VoxelMaterialsConfig.SelectedIndex);

			TXT_VoxelMaterialConfig_Details_Name.Text = voxelMaterial.Name;
			TXT_VoxelMaterialConfig_Details_AssetName.Text = voxelMaterial.AssetName;
			TXT_VoxelMaterialConfig_Details_MinedOre.Text = voxelMaterial.MinedOre;

			TXT_VoxelMaterialConfig_Details_MinedOreRatio.Text = voxelMaterial.MinedOreRatio.ToString(m_numberFormatInfo);
			TXT_VoxelMaterialConfig_Details_DamageRatio.Text = voxelMaterial.DamageRatio.ToString(m_numberFormatInfo);
			TXT_VoxelMaterialConfig_Details_SpecularPower.Text = voxelMaterial.SpecularPower.ToString(m_numberFormatInfo);
			TXT_VoxelMaterialConfig_Details_SpecularShininess.Text = voxelMaterial.SpecularShininess.ToString(m_numberFormatInfo);

			CHK_VoxelMaterialConfig_Details_CanBeHarvested.Checked = voxelMaterial.CanBeHarvested;
			CHK_VoxelMaterialConfig_Details_IsRare.Checked = voxelMaterial.IsRare;
			CHK_VoxelMaterialConfig_Details_UseTwoTextures.Checked = voxelMaterial.UseTwoTextures;

			m_currentlySelecting = false;
			BTN_VoxelMaterialsConfig_Details_Apply.Enabled = false;
			BTN_VoxelMaterialsConfig_Details_Delete.Enabled = true;
		}

		private void BTN_VoxelMaterialsConfig_Reload_Click(object sender, EventArgs e)
		{
			FillVoxelMaterialConfigurationListBox();
		}

		private void BTN_VoxelMaterialsConfig_Save_Click(object sender, EventArgs e)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			bool saveResult = m_voxelMaterialsDefinitionsManager.Save();

			stopWatch.Stop();

			if (!saveResult)
			{
				MessageBox.Show(this, "Failed to save VoxelMaterials config!");
				return;
			}

			TLS_StatusLabel.Text = "Done saving VoxelMaterials in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";
		}

		private void BTN_VoxelMaterialsConfig_Details_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_VoxelMaterialsConfig.SelectedIndex;

			VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsManager.DefinitionOf(index);

			voxelMaterial.Name = TXT_VoxelMaterialConfig_Details_Name.Text;
			voxelMaterial.AssetName = TXT_VoxelMaterialConfig_Details_AssetName.Text;
			voxelMaterial.MinedOre = TXT_VoxelMaterialConfig_Details_MinedOre.Text;

			voxelMaterial.MinedOreRatio = Convert.ToSingle(TXT_VoxelMaterialConfig_Details_MinedOreRatio.Text, m_numberFormatInfo);
			voxelMaterial.DamageRatio = Convert.ToSingle(TXT_VoxelMaterialConfig_Details_DamageRatio.Text, m_numberFormatInfo);
			voxelMaterial.SpecularPower = Convert.ToSingle(TXT_VoxelMaterialConfig_Details_SpecularPower.Text, m_numberFormatInfo);
			voxelMaterial.SpecularShininess = Convert.ToSingle(TXT_VoxelMaterialConfig_Details_SpecularShininess.Text, m_numberFormatInfo);

			voxelMaterial.CanBeHarvested = CHK_VoxelMaterialConfig_Details_CanBeHarvested.CheckState == CheckState.Checked;
			voxelMaterial.IsRare = CHK_VoxelMaterialConfig_Details_IsRare.CheckState == CheckState.Checked;
			voxelMaterial.UseTwoTextures = CHK_VoxelMaterialConfig_Details_UseTwoTextures.CheckState == CheckState.Checked;

			BTN_VoxelMaterialsConfig_Details_Apply.Enabled = false;
		}

		private void TXT_VoxelMaterialsConfig_Details_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_VoxelMaterialsConfig_Details_Apply.Enabled = true;
			}
		}

		private void CHK_VoxelMaterialsConfig_Details_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_VoxelMaterialsConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_VoxelMaterialsConfig_Details_New_Click(object sender, EventArgs e)
		{
			VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsManager.NewEntry();
			if (voxelMaterial == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			voxelMaterial.Name = "(New)";

			FillVoxelMaterialConfigurationListBox(false);

			LST_VoxelMaterialsConfig.SelectedIndex = LST_VoxelMaterialsConfig.Items.Count - 1;
		}

		private void BTN_VoxelMaterialsConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			if (LST_VoxelMaterialsConfig.SelectedIndex < 0) return;

			VoxelMaterialsDefinition itemToDelete = m_voxelMaterialsDefinitionsManager.DefinitionOf(LST_VoxelMaterialsConfig.SelectedIndex);

			bool deleteResult = m_voxelMaterialsDefinitionsManager.DeleteEntry(itemToDelete);
			if (!deleteResult)
			{
				MessageBox.Show(this, "Failed to delete VoxelMaterials entry!");
				return;
			}

			FillVoxelMaterialConfigurationListBox(false);
		}

		#endregion

		#region Scenarios

		private void LST_ScenariosConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ScenariosConfig.SelectedIndex;

			ScenariosDefinition scenario = m_scenariosDefinitionManager.DefinitionOf(index);

			TXT_ScenarioConfig_Details_Information_Id.Text = scenario.Id;
			TXT_ScenarioConfig_Details_Information_Name.Text = scenario.Name;

			CHK_ScenarioConfig_Details_Asteroid_Enabled.Checked = scenario.AsteroidClusters.Enabled;
			CHK_ScenarioConfig_Details_Asteroid_CentralCluster.Checked = scenario.AsteroidClusters.CentralCluster;
			TXT_ScenariosConfig_Details_Asteroid_Offset.Text = scenario.AsteroidClusters.Offset.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;
		}

		private void BTN_ScenarioConfig_Details_Info_Apply_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_ScenarioConfig_Details_Info_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_ScenarioConfig_Details_Info_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#region Asteroid

		private void BTN_ScenarioConfig_Details_Asteroid_Apply_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#endregion

		#region GeneratorOperations

		private void BTN_ScenariosConfig_Details_GeneratorOperations_Apply_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_ScenariosConfig_Details_GeneratorOperations_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_ScenariosConfig_Details_GeneratorOperations_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#endregion

		#region StartingStates

		private void BTN_ScenarioConfig_Details_StartingStates_Apply_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_ScenarioConfig_Details_StartingStates_New_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		private void BTN_ScenarioConfig_Details_StartingStates_Delete_Click(object sender, EventArgs e)
		{
			MessageBox.Show(this, "This feature is not yet implemented");
		}

		#endregion

		#endregion

		#region TransparentMaterials

		private void LST_TransparentMaterialsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;

			if (LST_TransparentMaterialsConfig.SelectedIndex < 0)
			{
				BTN_TransparentMaterialConfig_Details_Delete.Enabled = false;
				return;
			}

			TransparentMaterialsDefinition transparentMaterials = m_transparentMaterialsDefinitionManager.DefinitionOf(LST_TransparentMaterialsConfig.SelectedIndex);

			TXT_TransparentMaterialConfig_Details_Name.Text = transparentMaterials.Name;
			TXT_TransparentMaterialConfig_Details_Emissivity.Text = transparentMaterials.Emissivity.ToString(m_numberFormatInfo);
			TXT_TransparentMaterialConfig_Details_SoftParticleDistanceScale.Text = transparentMaterials.SoftParticleDistanceScale.ToString(m_numberFormatInfo);
			TXT_TransparentMaterialConfig_Details_Texture.Text = transparentMaterials.Texture;

			TXT_TransparentMaterialConfig_Details_UVOffset_X.Text = transparentMaterials.UVOffset.X.ToString(m_numberFormatInfo);
			TXT_TransparentMaterialConfig_Details_UVOffset_Y.Text = transparentMaterials.UVOffset.Y.ToString(m_numberFormatInfo);
			TXT_TransparentMaterialConfig_Details_UVSize_X.Text = transparentMaterials.UVSize.X.ToString(m_numberFormatInfo);
			TXT_TransparentMaterialConfig_Details_UVSize_Y.Text = transparentMaterials.UVSize.Y.ToString(m_numberFormatInfo);

			CHK_TransparentMaterialConfig_Details_AlphaMistingEnable.Checked = transparentMaterials.AlphaMistingEnable;
			CHK_TransparentMaterialConfig_Details_CanBeAffectedByLight.Checked = transparentMaterials.CanBeAffectedByOtherLights;
			CHK_TransparentMaterialConfig_Details_IgnoreDepth.Checked = transparentMaterials.IgnoreDepth;
			CHK_TransparentMaterialConfig_Details_UseAtlas.Checked = transparentMaterials.UseAtlas;

			m_currentlySelecting = false;
			BTN_TransparentMaterialConfig_Details_Apply.Enabled = false;
			BTN_TransparentMaterialConfig_Details_Delete.Enabled = true;
		}

		private void BTN_TransparentMaterialsConfig_Reload_Click(object sender, EventArgs e)
		{
			FillTransparentMaterialsConfigurationListBox();
		}

		private void BTN_TransparentMaterialsConfig_Save_Click(object sender, EventArgs e)
		{
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			bool saveResult = m_transparentMaterialsDefinitionManager.Save();

			stopWatch.Stop();

			if (!saveResult)
			{
				MessageBox.Show(this, "Failed to save TransparentMaterials config!");
				return;
			}

			TLS_StatusLabel.Text = "Done saving TransparentMaterials in " + stopWatch.ElapsedMilliseconds.ToString() + "ms";
		}

		private void BTN_TransparentMaterialsConfig_Details_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_TransparentMaterialsConfig.SelectedIndex;

			TransparentMaterialsDefinition transparentMaterial = m_transparentMaterialsDefinitionManager.DefinitionOf(index);

			transparentMaterial.Name = TXT_TransparentMaterialConfig_Details_Name.Text;
			transparentMaterial.Emissivity = Convert.ToSingle(TXT_TransparentMaterialConfig_Details_Emissivity.Text, m_numberFormatInfo);
			transparentMaterial.SoftParticleDistanceScale = Convert.ToSingle(TXT_TransparentMaterialConfig_Details_SoftParticleDistanceScale.Text, m_numberFormatInfo);
			transparentMaterial.Texture = TXT_TransparentMaterialConfig_Details_Texture.Text;

			transparentMaterial.AlphaMistingEnable = CHK_TransparentMaterialConfig_Details_AlphaMistingEnable.Checked;
			transparentMaterial.CanBeAffectedByOtherLights = CHK_TransparentMaterialConfig_Details_CanBeAffectedByLight.Checked;
			transparentMaterial.IgnoreDepth = CHK_TransparentMaterialConfig_Details_IgnoreDepth.Checked;
			transparentMaterial.UseAtlas = CHK_TransparentMaterialConfig_Details_UseAtlas.Checked;

			float uvOffsetX = Convert.ToSingle(TXT_TransparentMaterialConfig_Details_UVOffset_X.Text, m_numberFormatInfo);
			float uvOffsetY = Convert.ToSingle(TXT_TransparentMaterialConfig_Details_UVOffset_Y.Text, m_numberFormatInfo);

			transparentMaterial.UVOffset = new Vector2(uvOffsetX, uvOffsetY);

			float uvSizeX = Convert.ToSingle(TXT_TransparentMaterialConfig_Details_UVSize_X.Text, m_numberFormatInfo);
			float uvSizeY = Convert.ToSingle(TXT_TransparentMaterialConfig_Details_UVSize_Y.Text, m_numberFormatInfo);

			transparentMaterial.UVSize = new Vector2(uvSizeX, uvSizeY);

			BTN_TransparentMaterialConfig_Details_Apply.Enabled = false;
		}

		private void TXT_TransparentMaterialsConfig_Details_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_TransparentMaterialConfig_Details_Apply.Enabled = true;
			}
		}

		private void CHK_TransparentMaterialsConfig_Details_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_TransparentMaterialConfig_Details_Apply.Enabled = true;
			}
		}

		private void BTN_TransparentMaterialConfig_Details_New_Click(object sender, EventArgs e)
		{
			TransparentMaterialsDefinition transparentMaterials = m_transparentMaterialsDefinitionManager.NewEntry();
			if (transparentMaterials == null)
			{
				MessageBox.Show(this, "Failed to create new entry");
				return;
			}

			transparentMaterials.Name = "(New)";

			FillTransparentMaterialsConfigurationListBox(false);

			LST_TransparentMaterialsConfig.SelectedIndex = LST_TransparentMaterialsConfig.Items.Count - 1;
		}

		private void BTN_TransparentMaterialConfig_Details_Delete_Click(object sender, EventArgs e)
		{
			if (LST_TransparentMaterialsConfig.SelectedIndex < 0) return;

			TransparentMaterialsDefinition itemToDelete = m_transparentMaterialsDefinitionManager.DefinitionOf(LST_TransparentMaterialsConfig.SelectedIndex);

			bool deleteResult = m_transparentMaterialsDefinitionManager.DeleteEntry(itemToDelete);
			if (!deleteResult)
			{
				MessageBox.Show(this, "Failed to delete TransparentMaterials entry!");
				return;
			}

			FillTransparentMaterialsConfigurationListBox(false);
		}

		#endregion

		#region Environment

		private void BTN_EnvironmentConfig_Reload_Click(object sender, EventArgs e)
		{
			FillEnvironmentConfigurationInfo();
		}

		private void BTN_EnvironmentConfig_Save_Click(object sender, EventArgs e)
		{
			//m_environmentDefinitionManager.Save();
		}

		#endregion 

		#endregion
	}
}
