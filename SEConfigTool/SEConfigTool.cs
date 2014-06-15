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

			//Add the cube grids
			foreach (CubeGrid cubeGrid in sector.CubeGrids)
			{
				float x = cubeGrid.PositionAndOrientation.Position.x;
				float y = cubeGrid.PositionAndOrientation.Position.y;
				float z = cubeGrid.PositionAndOrientation.Position.z;

				float dist = (float)Math.Sqrt(x * x + y * y + z * z);

				TreeNode newNode = TRV_SavedGame_Objects.Nodes[0].Nodes.Add(cubeGrid.EntityId.ToString(), cubeGrid.Name + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z + ";" + y);
				newNode.Tag = cubeGrid;

				//Create the cube grid sub-item categories
				TreeNode blocksNode = newNode.Nodes.Add("Cube Blocks (" + cubeGrid.CubeBlocks.Count.ToString() + ")");
				newNode.Nodes.Add("Conveyor Lines (" + cubeGrid.ConveyorLines.Count.ToString() + ")");
				newNode.Nodes.Add("Block Groups (" + cubeGrid.BlockGroups.Count.ToString() + ")");

				TreeNode structuralBlocksNode = blocksNode.Nodes.Add("Structural");
				TreeNode containerBlocksNode = blocksNode.Nodes.Add("Containers");
				TreeNode productionBlocksNode = blocksNode.Nodes.Add("Refinement and Production");
				TreeNode energyBlocksNode = blocksNode.Nodes.Add("Energy");
				TreeNode conveyorBlocksNode = blocksNode.Nodes.Add("Conveyor");
				TreeNode utilityBlocksNode = blocksNode.Nodes.Add("Utility");
				TreeNode miscBlocksNode = blocksNode.Nodes.Add("Misc");

				//Add the cube blocks
				foreach (CubeBlock cubeBlock in cubeGrid.CubeBlocks)
				{
					TreeNode blockNode = null;
					MyObjectBuilder_Inventory blockInventory = null;
					switch (cubeBlock.BaseDefinition.TypeId)
					{
						case MyObjectBuilderTypeEnum.CubeBlock:
							blockNode = structuralBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
							break;
						case MyObjectBuilderTypeEnum.CargoContainer:
							blockNode = containerBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
							MyObjectBuilder_CargoContainer containerBlock = (MyObjectBuilder_CargoContainer) cubeBlock.BaseDefinition;
							blockInventory = containerBlock.Inventory;
							break;
						case MyObjectBuilderTypeEnum.Refinery:
							blockNode = productionBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
							break;
						case MyObjectBuilderTypeEnum.Assembler:
							blockNode = productionBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
							break;
						case MyObjectBuilderTypeEnum.Reactor:
							MyObjectBuilder_Reactor reactorBlock = (MyObjectBuilder_Reactor)cubeBlock.BaseDefinition;
							blockInventory = reactorBlock.Inventory;
							blockNode = energyBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
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
						case MyObjectBuilderTypeEnum.MedicalRoom:
							blockNode = utilityBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
							break;
						default:
							blockNode = miscBlocksNode.Nodes.Add(cubeBlock.EntityId.ToString(), cubeBlock.Name);
							break;
					}
					if (blockNode == null)
						continue;

					blockNode.Tag = cubeBlock;

					if (blockInventory != null)
					{
						foreach (var item in blockInventory.Items)
						{
							blockNode.Nodes.Add(item.PhysicalContent.SubtypeName + " x" + item.AmountDecimal.ToString());
						}
					}
				}
			}

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
				LST_BlueprintConfig.Items.Add("Blueprint " + LST_BlueprintConfig.Items.Count.ToString());
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillVoxelMaterialConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

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

		private void FillTransparentMaterialsConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_transparentMaterialsDefinitionManager.Load(GetContentDataFile("TransparentMaterials.sbc"));

			LST_TransparentMaterialsConfig.Items.Clear();
			foreach (var definition in m_transparentMaterialsDefinitionManager.Definitions)
			{
				LST_TransparentMaterialsConfig.Items.Add(definition.Name);
			}

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
		}

		#region SavedGame

		private void BTN_LoadSaveGame_Click(object sender, EventArgs e)
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
			m_sectorManager.Save();
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

			var linkedObject = e.Node.Tag;
			if (linkedObject == null)
				return;

			if (linkedObject.GetType() == typeof(CubeBlock))
			{
				CubeBlock cubeBlock = (CubeBlock)linkedObject;

				LBL_Sector_Objects_Field1.Visible = true;
				LBL_Sector_Objects_Field2.Visible = true;
				TXT_Sector_Objects_Field1.Visible = true;
				TXT_Sector_Objects_Field2.Visible = true;

				LBL_Sector_Objects_Field1.Text = "Type:";
				LBL_Sector_Objects_Field2.Text = "Entity Id:";
				TXT_Sector_Objects_Field1.Text = cubeBlock.SubtypeName;
				TXT_Sector_Objects_Field2.Text = cubeBlock.EntityId.ToString();

				if (cubeBlock.BaseDefinition.GetType() == typeof(MyObjectBuilder_CubeBlock))
				{
					//TODO - Maybe display something like integrity percent or build percent for structural blocks?
				}
				else if (cubeBlock.BaseDefinition.GetType() == typeof(MyObjectBuilder_CargoContainer))
				{
					MyObjectBuilder_CargoContainer containerBlock = (MyObjectBuilder_CargoContainer)cubeBlock.BaseDefinition;

					LBL_Sector_Objects_Field3.Visible = true;
					LBL_Sector_Objects_Field4.Visible = true;
					LBL_Sector_Objects_Field5.Visible = true;
					TXT_Sector_Objects_Field3.Visible = true;
					TXT_Sector_Objects_Field4.Visible = true;
					TXT_Sector_Objects_Field5.Visible = true;

					LBL_Sector_Objects_Field3.Text = "Item Count:";
					LBL_Sector_Objects_Field4.Text = "Item Volume (L):";
					LBL_Sector_Objects_Field5.Text = "Item Mass (kg):";

					float itemCount = 0;
					float itemVolume = 0;
					float itemMass = 0;
					foreach (var item in containerBlock.Inventory.Items)
					{
						itemCount += item.Amount;

						if (item.PhysicalContent.TypeId == MyObjectBuilderTypeEnum.PhysicalGunObject)
						{
							foreach (var physicalItem in m_physicalItemsDefinitionsManager.Definitions)
							{
								if (physicalItem.Id.SubtypeId == item.PhysicalContent.SubtypeName)
								{
									itemVolume += physicalItem.Volume * item.Amount;
									itemMass += physicalItem.Mass * item.Amount;
									break;
								}
							}
						}
						if (item.PhysicalContent.TypeId == MyObjectBuilderTypeEnum.Component)
						{
							foreach (var component in m_componentsDefinitionsManager.Definitions)
							{
								if (component.Id.SubtypeId == item.PhysicalContent.SubtypeName)
								{
									itemVolume += component.Volume * item.Amount;
									itemMass += component.Mass * item.Amount;
									break;
								}
							}
						}
						if (item.PhysicalContent.TypeId == MyObjectBuilderTypeEnum.AmmoMagazine)
						{
							foreach (var ammo in m_ammoMagazinesDefinitionsManager.Definitions)
							{
								if (ammo.Id.SubtypeId == item.PhysicalContent.SubtypeName)
								{
									itemVolume += ammo.Volume * item.Amount;
									itemMass += ammo.Mass * item.Amount;
									break;
								}
							}
						}
					}
					TXT_Sector_Objects_Field3.Text = itemCount.ToString();
					TXT_Sector_Objects_Field4.Text = itemVolume.ToString();
					TXT_Sector_Objects_Field5.Text = itemMass.ToString();
				}
				else
				{
					MyObjectBuilder_Reactor containerBlock = (MyObjectBuilder_Reactor)cubeBlock.BaseDefinition;

					LBL_Sector_Objects_Field3.Visible = true;
					TXT_Sector_Objects_Field3.Visible = true;

					LBL_Sector_Objects_Field3.Text = "Fuel (kg):";

					float fuelMass = 0;
					foreach (var item in containerBlock.Inventory.Items)
					{
						fuelMass += item.Amount;
					}
					TXT_Sector_Objects_Field3.Text = fuelMass.ToString();
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

		#endregion

		#region AmmoMagazines

		private void LST_AmmoConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_AmmoConfig.SelectedIndex;

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(index);

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
		}

		private void BTN_ConfigAmmoReload_Click(object sender, EventArgs e)
		{
			FillAmmoConfigurationListBox();
		}

		private void BTN_SaveAmmoConfig_Click(object sender, EventArgs e)
		{
			m_ammoMagazinesDefinitionsManager.Save();
		}

		private void BTN_ConfigAmmoApply_Click(object sender, EventArgs e)
		{
			int index = LST_AmmoConfig.SelectedIndex;

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(index);

			ammoMagazine.Id = new SerializableDefinitionId(MyObjectBuilderTypeEnum.AmmoMagazine, TXT_AmmoConfig_Details_Id.Text);
			ammoMagazine.Name = TXT_AmmoConfig_Details_Name.Text;
			ammoMagazine.DisplayName = TXT_AmmoConfig_Details_Name.Text;
			ammoMagazine.Description = TXT_AmmoConfig_Details_Description.Text;
			ammoMagazine.Icon = TXT_AmmoConfig_Details_Icon.Text;
			ammoMagazine.Model = TXT_AmmoConfig_Details_Model.Text;
			ammoMagazine.Caliber = (MyAmmoCategoryEnum) CMB_AmmoConfig_Details_Caliber.SelectedItem;
			ammoMagazine.Capacity = Convert.ToInt32(TXT_AmmoConfig_Details_Capacity.Text, m_numberFormatInfo);
			ammoMagazine.Mass = Convert.ToSingle(TXT_AmmoConfig_Details_Mass.Text, m_numberFormatInfo);
			ammoMagazine.Volume = Convert.ToSingle(TXT_AmmoConfig_Details_Volume.Text, m_numberFormatInfo);

			BTN_AmmoConfig_Details_Apply.Enabled = false;
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

		#region Items

		private void LST_ContainerTypeConfiguration_Items_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ContainerTypeConfig_Details_Items.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LST_ContainerTypesConfig.SelectedIndex);
			ContainerTypeItem containerItem = containerType.Items[index];

			CMB_ContainerTypeConfig_Items_Type.SelectedItem = containerItem.Id;

			TXT_ContainerTypeConfig_Item_AmountMin.Text = containerItem.AmountMin.ToString();
			TXT_ContainerTypeConfig_Item_AmountMax.Text = containerItem.AmountMax.ToString();
			TXT_ContainerTypeConfig_Item_Frequency.Text = containerItem.Frequency.ToString();

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

		#endregion

		#endregion

		#region GlobalEvents

		private void LST_GlobalEventConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_GlobalEventConfig.SelectedIndex;

			GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsManager.DefinitionOf(index);

			TXT_ConfigGlobalEvent_Details_Name.Text = globalEvent.Name;
			TXT_ConfigGlobalEvent_Details_Description.Text = globalEvent.Description;
			CMB_GlobalEventsConfig_Details_EventType.SelectedItem = globalEvent.EventType;
			TXT_ConfigGlobalEvent_Details_MinActivation.Text = globalEvent.MinActivation.ToString();
			TXT_ConfigGlobalEvent_Details_MaxActivation.Text = globalEvent.MaxActivation.ToString();
			TXT_ConfigGlobalEvent_Details_FirstActivation.Text = globalEvent.FirstActivation.ToString();

			m_currentlySelecting = false;

			BTN_GlobalEventConfig_Details_Apply.Enabled = false;
		}

		private void BTN_ConfigGlobalEventReload_Click(object sender, EventArgs e)
		{
			FillGlobalEventConfigurationListBox();
		}

		private void BTN_SaveGlobalEventConfig_Click(object sender, EventArgs e)
		{
			m_globalEventsDefinitionsManager.Save();
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

		#endregion

		#region SpawnGroups

		private void LST_SpawnGroupConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_SpawnGroupConfig.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(index);

			TXT_SpawnGroupConfig_Details_Info_Count.Text = spawnGroup.PrefabCount.ToString();
			TXT_SpawnGroupConfig_Details_Info_Frequency.Text = spawnGroup.Frequency.ToString();

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
			TXT_SpawnGroupConfig_Details_Prefabs_Speed.Text = spawnGroupPrefab.Speed.ToString();

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

		#endregion

		#endregion

		#region PhysicalItems

		private void LST_PhysicalItemConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_PhysicalItemConfig.SelectedIndex;

			PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.DefinitionOf(index);

			CMB_PhysicalItemConfig_Details_Type.SelectedItem = physicalItem.Id.TypeId;
			TXT_PhysicalItemConfig_Details_Id.Text = physicalItem.Id.SubtypeId;
			TXT_PhysicalItemConfig_Details_Name.Text = physicalItem.Name;
			TXT_PhysicalItemConfig_Details_Description.Text = physicalItem.Description;
			TXT_PhysicalItemConfig_Details_Icon.Text = physicalItem.Icon;
			TXT_PhysicalItemConfig_Details_Model.Text = physicalItem.Model;
			TXT_PhysicalItemConfig_Details_IconSymbol.Text = physicalItem.IconSymbol.ToString();
			TXT_PhysicalItemConfig_Details_Size_X.Text = physicalItem.Size.X.ToString();
			TXT_PhysicalItemConfig_Details_Size_Y.Text = physicalItem.Size.Y.ToString();
			TXT_PhysicalItemConfig_Details_Size_Z.Text = physicalItem.Size.Z.ToString();
			TXT_PhysicalItemConfig_Details_Mass.Text = physicalItem.Mass.ToString();
			TXT_PhysicalItemConfig_Details_Volume.Text = physicalItem.Volume.ToString();

			m_currentlySelecting = false;

			BTN_PhysicalItemConfig_Details_Apply.Enabled = false;
		}

		private void BTN_ConfigPhysicalItemReload_Click(object sender, EventArgs e)
		{
			FillPhysicalItemConfigurationListBox();
		}

		private void BTN_SavePhysicalItemConfig_Click(object sender, EventArgs e)
		{
			m_physicalItemsDefinitionsManager.Save();
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

		#endregion

		#region Components

		private void LST_ComponentsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ComponentsConfig.SelectedIndex;

			ComponentsDefinition component = m_componentsDefinitionsManager.DefinitionOf(index);

			TXT_ComponentConfig_Details_Id.Text = component.Id.SubtypeId;
			TXT_ComponentConfig_Details_Name.Text = component.Name;
			TXT_ComponentConfig_Details_Description.Text = component.Description;
			TXT_ComponentConfig_Details_Icon.Text = component.Icon;
			TXT_ComponentConfig_Details_Model.Text = component.Model;
			TXT_ComponentConfig_Details_Size_X.Text = component.Size.X.ToString();
			TXT_ComponentConfig_Details_Size_Y.Text = component.Size.Y.ToString();
			TXT_ComponentConfig_Details_Size_Z.Text = component.Size.Z.ToString();
			TXT_ComponentConfig_Details_Mass.Text = component.Mass.ToString();
			TXT_ComponentConfig_Details_Volume.Text = component.Volume.ToString();
			TXT_ComponentConfig_Details_MaxIntegrity.Text = component.MaxIntegrity.ToString();
			TXT_ComponentConfig_Details_DropProbability.Text = component.DropProbability.ToString();

			m_currentlySelecting = false;

			BTN_ComponentConfig_Details_Apply.Enabled = false;
		}

		private void BTN_ComponentConfig_Reload_Click(object sender, EventArgs e)
		{
			FillComponentConfigurationListBox();
		}

		private void BTN_ComponentConfig_Save_Click(object sender, EventArgs e)
		{
			m_componentsDefinitionsManager.Save();
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

		#endregion

		#region Blueprints

		private void LST_BlueprintConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_BlueprintConfig.SelectedIndex;

			BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(index);

			TXT_BlueprintConfig_Details_Result.Text = blueprint.Result.TypeId.ToString() + "/" + blueprint.Result.SubtypeId + " x" + blueprint.Result.Amount.ToString();
			TXT_BlueprintConfig_Details_BaseProductionTime.Text = blueprint.BaseProductionTimeInSeconds.ToString();

			LST_BlueprintConfig_Details_Prerequisites.Items.Clear();
			foreach (var prereq in blueprint.Prerequisites)
			{
				LST_BlueprintConfig_Details_Prerequisites.Items.Add(prereq.TypeId.ToString() + "/" + prereq.SubtypeId + " x" + prereq.Amount.ToString());
			}

			m_currentlySelecting = false;
			BTN_BlueprintConfig_Details_Apply.Enabled = false;
		}

		private void BTN_BlueprintConfig_Reload_Click(object sender, EventArgs e)
		{
			FillBlueprintConfigurationListBox();
		}

		private void BTN_BlueprintConfig_Save_Click(object sender, EventArgs e)
		{
			m_blueprintsDefinitionsManager.Save();
		}

		private void BTN_BlueprintConfig_Details_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_BlueprintConfig.SelectedIndex;

			BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(index);

			blueprint.BaseProductionTimeInSeconds = Convert.ToSingle(TXT_BlueprintConfig_Details_BaseProductionTime.Text, m_numberFormatInfo);

			BTN_BlueprintConfig_Details_Apply.Enabled = false;
		}

		private void TXT_BlueprintConfig_Details_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_BlueprintConfig_Details_Apply.Enabled = true;
			}
		}

		#endregion

		#region VoxelMaterials

		private void LST_VoxelMaterialsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_VoxelMaterialsConfig.SelectedIndex;

			VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsManager.DefinitionOf(index);

			TXT_VoxelMaterialConfig_Details_Name.Text = voxelMaterial.Name;
			TXT_VoxelMaterialConfig_Details_AssetName.Text = voxelMaterial.AssetName;
			TXT_VoxelMaterialConfig_Details_MinedOre.Text = voxelMaterial.MinedOre;

			TXT_VoxelMaterialConfig_Details_MinedOreRatio.Text = voxelMaterial.MinedOreRatio.ToString();
			TXT_VoxelMaterialConfig_Details_DamageRatio.Text = voxelMaterial.DamageRatio.ToString();
			TXT_VoxelMaterialConfig_Details_SpecularPower.Text = voxelMaterial.SpecularPower.ToString();
			TXT_VoxelMaterialConfig_Details_SpecularShininess.Text = voxelMaterial.SpecularShininess.ToString();

			CHK_VoxelMaterialConfig_Details_CanBeHarvested.Checked = voxelMaterial.CanBeHarvested;
			CHK_VoxelMaterialConfig_Details_IsRare.Checked = voxelMaterial.IsRare;
			CHK_VoxelMaterialConfig_Details_UseTwoTextures.Checked = voxelMaterial.UseTwoTextures;

			m_currentlySelecting = false;
			BTN_VoxelMaterialsConfig_Details_Apply.Enabled = false;
		}

		private void BTN_VoxelMaterialsConfig_Reload_Click(object sender, EventArgs e)
		{
			FillVoxelMaterialConfigurationListBox();
		}

		private void BTN_VoxelMaterialsConfig_Save_Click(object sender, EventArgs e)
		{
			m_voxelMaterialsDefinitionsManager.Save();
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
			TXT_ScenariosConfig_Details_Asteroid_Offset.Text = scenario.AsteroidClusters.Offset.ToString();

			m_currentlySelecting = false;
		}

		#endregion

		#region TransparentMaterials

		private void LST_TransparentMaterialsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_TransparentMaterialsConfig.SelectedIndex;

			TransparentMaterialsDefinition transparentMaterials = m_transparentMaterialsDefinitionManager.DefinitionOf(index);

			TXT_TransparentMaterialConfig_Details_Name.Text = transparentMaterials.Name;
			TXT_TransparentMaterialConfig_Details_Emissivity.Text = transparentMaterials.Emissivity.ToString();
			TXT_TransparentMaterialConfig_Details_SoftParticleDistanceScale.Text = transparentMaterials.SoftParticleDistanceScale.ToString();
			TXT_TransparentMaterialConfig_Details_Texture.Text = transparentMaterials.Texture;

			TXT_TransparentMaterialConfig_Details_UVOffset_X.Text = transparentMaterials.UVOffset.X.ToString();
			TXT_TransparentMaterialConfig_Details_UVOffset_Y.Text = transparentMaterials.UVOffset.Y.ToString();
			TXT_TransparentMaterialConfig_Details_UVSize_X.Text = transparentMaterials.UVSize.X.ToString();
			TXT_TransparentMaterialConfig_Details_UVSize_Y.Text = transparentMaterials.UVSize.Y.ToString();

			CHK_TransparentMaterialConfig_Details_AlphaMistingEnable.Checked = transparentMaterials.AlphaMistingEnable;
			CHK_TransparentMaterialConfig_Details_CanBeAffectedByLight.Checked = transparentMaterials.CanBeAffectedByOtherLights;
			CHK_TransparentMaterialConfig_Details_IgnoreDepth.Checked = transparentMaterials.IgnoreDepth;
			CHK_TransparentMaterialConfig_Details_UseAtlas.Checked = transparentMaterials.UseAtlas;

			BTN_TransparentMaterialConfig_Details_Apply.Enabled = false;
			m_currentlySelecting = false;
		}

		private void BTN_TransparentMaterialsConfig_Reload_Click(object sender, EventArgs e)
		{
			FillTransparentMaterialsConfigurationListBox();
		}

		private void BTN_TransparentMaterialsConfig_Save_Click(object sender, EventArgs e)
		{
			m_transparentMaterialsDefinitionManager.Save();
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

		#endregion

		#endregion
	}
}
