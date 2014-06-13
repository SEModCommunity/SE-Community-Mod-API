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

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using System.Diagnostics;

namespace SEConfigTool
{
	public partial class SEConfigTool : Form
	{
		#region Attributes

		private string m_standardSavePath;

		private ConfigFileSerializer m_configSerializer;
		private SaveFileSerializer m_saveFileSerializer;

		private CubeBlockDefinitionsManager m_cubeBlockDefinitionsManager;
		private AmmoMagazinesDefinitionsManager m_ammoMagazinesDefinitionsManager;
		private ContainerTypesDefinitionsManager m_containerTypesDefinitionsManager;
		private GlobalEventsDefinitionsManager m_globalEventsDefinitionsManager;
		private SpawnGroupsDefinitionsManager m_spawnGroupsDefinitionsManager;
		private PhysicalItemDefinitionsManager m_physicalItemsDefinitionsManager;
		private ComponentDefinitionsManager m_componentsDefinitionsManager;
		private BlueprintDefinitionsManager m_blueprintsDefinitionsManager;
		private VoxelMaterialDefinitionsManager m_voxelMaterialsDefinitionsWrapper;
		private ScenariosDefinitionsManager m_scenariosDefinitionWrapper;

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
		}

		#endregion

		#region Form methods

		private void LoadSaveFile(FileInfo saveFileInfo)
		{
			TLS_StatusLabel.Text = "Loading sector ...";
			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			m_saveFileSerializer = new SaveFileSerializer(saveFileInfo, m_configSerializer);
			Sector sector = m_saveFileSerializer.Sector;

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

				TreeNode newNode = TRV_SavedGame_Objects.Nodes[0].Nodes.Add(cubeGrid.Name + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z + ";" + y);

				//Create the cube grid sub-item categories
				newNode.Nodes.Add("Cube Blocks");
				newNode.Nodes.Add("Conveyor Lines");
				newNode.Nodes.Add("Block Groups");

				//Add the cube blocks
				foreach (CubeBlock cubeBlock in cubeGrid.CubeBlocks)
				{
					newNode.Nodes[0].Nodes.Add(cubeBlock.Name);
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
			BTN_SavedGame_Events_Apply.Visible = false;
		}

		private void FillBlocksConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			LST_BlocksConfiguration.Items.Clear();
			m_cubeBlockDefinitionsManager = new CubeBlockDefinitionsManager(m_configSerializer.CubeBlockDefinitions);
			foreach (var definition in m_cubeBlockDefinitionsManager.Definitions)
			{
				LST_BlocksConfiguration.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillAmmoConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			LST_AmmoConfiguration.Items.Clear();
			m_ammoMagazinesDefinitionsManager = new AmmoMagazinesDefinitionsManager(m_configSerializer.AmmoMagazineDefinitions);
			foreach (var definition in m_ammoMagazinesDefinitionsManager.Definitions)
			{
				LST_AmmoConfiguration.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillContainerTypeConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			LST_ContainerTypeConfiguration.Items.Clear();
			LST_ContainerTypeConfig_Details_Items.Items.Clear();
			m_containerTypesDefinitionsManager = new ContainerTypesDefinitionsManager(m_configSerializer.ContainerTypeDefinitions);
			foreach (var definition in m_containerTypesDefinitionsManager.Definitions)
			{
				LST_ContainerTypeConfiguration.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillGlobalEventConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			LST_GlobalEventConfiguration.Items.Clear();
			m_globalEventsDefinitionsManager = new GlobalEventsDefinitionsManager(m_configSerializer.GlobalEventDefinitions);
			foreach (var definition in m_globalEventsDefinitionsManager.Definitions)
			{
				LST_GlobalEventConfiguration.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillSpawnGroupConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;
			LST_SpawnGroupConfiguration.Items.Clear();
			LST_SpawnGroupConfig_Details_Prefabs.Items.Clear();

			m_spawnGroupsDefinitionsManager = new SpawnGroupsDefinitionsManager(m_configSerializer.SpawnGroupDefinitions);

			foreach (var definition in m_spawnGroupsDefinitionsManager.Definitions)
			{
				//TODO - Find a better way to uniquely label the spawn groups
				LST_SpawnGroupConfiguration.Items.Add("Spawn Group " + LST_SpawnGroupConfiguration.Items.Count.ToString());
			}
			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillPhysicalItemConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_physicalItemsDefinitionsManager = new PhysicalItemDefinitionsManager(m_configSerializer.PhysicalItemDefinitions);
			LST_PhysicalItemConfiguration.Items.Clear();
			foreach (var definition in m_physicalItemsDefinitionsManager.Definitions)
			{
				LST_PhysicalItemConfiguration.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillComponentConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_componentsDefinitionsManager = new ComponentDefinitionsManager(m_configSerializer.ComponentDefinitions);
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

			m_blueprintsDefinitionsManager = new BlueprintDefinitionsManager(m_configSerializer.BlueprintDefinitions);
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

			m_voxelMaterialsDefinitionsWrapper = new VoxelMaterialDefinitionsManager(m_configSerializer.VoxelMaterialDefinitions);
			LST_VoxelMaterialsConfig.Items.Clear();
			foreach (var definition in m_voxelMaterialsDefinitionsWrapper.Definitions)
			{
				LST_VoxelMaterialsConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillScenariosConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_scenariosDefinitionWrapper = new ScenariosDefinitionsManager(m_configSerializer.ScenarioDefinitions);
			LST_ScenariosConfig.Items.Clear();
			foreach (var definition in m_scenariosDefinitionWrapper.Definitions)
			{
				LST_ScenariosConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		#endregion

		#region Form events

		private void SEConfigTool_Load(object sender, EventArgs e)
		{
			m_configSerializer = new ConfigFileSerializer();
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
			m_saveFileSerializer.Sector.Save();
			}

		private void BTN_SavedGame_Events_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_SavedGame_Events.SelectedIndex;

			Sector sector = m_saveFileSerializer.Sector;
			Event sectorEvent = sector.Events[index];

			sectorEvent.Enabled = CHK_SavedGame_Events_Enabled.CheckState == CheckState.Checked;
			sectorEvent.ActivationTimeMs = Convert.ToInt64(TXT_SavedGame_Events_ActivationTime.Text, m_numberFormatInfo);

			BTN_SavedGame_Events_Apply.Visible = false;
		}

		private void LST_SavedGame_Events_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_SavedGame_Events.SelectedIndex;

			Sector sector = m_saveFileSerializer.Sector;
			Event sectorEvent = sector.Events[index];

			TXT_SavedGame_Events_Type.Text = sectorEvent.DefinitionId.ToString();
			CHK_SavedGame_Events_Enabled.Checked = sectorEvent.Enabled;
			TXT_SavedGame_Events_ActivationTime.Text = sectorEvent.ActivationTimeMs.ToString();

			m_currentlySelecting = false;
			BTN_SavedGame_Events_Apply.Visible = false;
		}

		private void TRV_SavedGame_Objects_AfterSelect(object sender, TreeViewEventArgs e)
		{
			//Ignore top-level category nodes
			if (e.Node.Level == 0)
				return;

			//Sector object nodes
			if (e.Node.Level == 1)
			{
				}

			//Ignore sector object parts category nodes
			if (e.Node.Level == 2)
				return;

			//Sector object parts items nodes
			if (e.Node.Level == 3)
			{
			}
		}

		private void TXT_SavedGame_Events_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SavedGame_Events_Apply.Visible = true;
			}
		}

		private void CHK_SavedGame_Events_Enabled_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SavedGame_Events_Apply.Visible = true;
			}
		}

		#endregion

		#region CubeBlock

		private void CHK_BlocksConfig_ModelIntersection_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ConfigApplyChanges.Visible = true;
			int index = LST_BlocksConfiguration.SelectedIndex;
			m_cubeBlockDefinitionsManager.DefinitionOf(index).UseModelIntersection = CHK_BlocksConfig_ModelIntersection.Checked;
		}
        }

		private void CHK_BlocksConfig_Enabled_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ConfigApplyChanges.Visible = true;
			int index = LST_BlocksConfiguration.SelectedIndex;
			m_cubeBlockDefinitionsManager.DefinitionOf(index).Enabled = CHK_BlocksConfig_Enabled.Checked;
		}
        }

		private void LST_BlocksConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_BlocksConfiguration.SelectedIndex;

			CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsManager.DefinitionOf(index);

			TXT_ConfigBlockName.Text = cubeBlock.Name;
			TXT_ConfigBlockId.Text = cubeBlock.Id.ToString();
			TXT_ConfigBuildTime.Text = cubeBlock.BuildTime.ToString(m_numberFormatInfo);
			TXT_ConfigDisassembleRatio.Text = cubeBlock.DisassembleRatio.ToString(m_numberFormatInfo);
			CHK_BlocksConfig_Enabled.Checked = cubeBlock.Enabled;
			CHK_BlocksConfig_ModelIntersection.Checked = cubeBlock.UseModelIntersection;

			DGV_ConfigBlocks_Components.DataSource = cubeBlock.Components.ToArray().Select(x => new { x.Subtype, x.Count }).ToArray();

			m_currentlySelecting = false;

			BTN_ConfigApplyChanges.Visible = false;
		}

		private void TXT_ConfigBuildTime_KeyPress(object sender, KeyPressEventArgs e)
		{
			char ch = e.KeyChar;

			if (ch == '.' && TXT_ConfigBuildTime.Text.IndexOf('.') != -1)
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
				BTN_ConfigApplyChanges.Visible = true;
			}
		}

		private void BTN_ConfigApplyChanges_Click(object sender, EventArgs e)
		{
			int index = LST_BlocksConfiguration.SelectedIndex;

			CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsManager.DefinitionOf(index);

			cubeBlock.BuildTime = Convert.ToSingle(TXT_ConfigBuildTime.Text, m_numberFormatInfo);
			cubeBlock.DisassembleRatio = Convert.ToSingle(TXT_ConfigDisassembleRatio.Text, m_numberFormatInfo);

			BTN_ConfigApplyChanges.Visible = false;
		}

		#endregion

		#region AmmoMagazines

		private void LST_AmmoConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_AmmoConfiguration.SelectedIndex;

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(index);

			CMB_AmmoConfig_Details_Caliber.Items.Clear();
			foreach (var caliber in Enum.GetValues(typeof(MyAmmoCategoryEnum)))
			{
				CMB_AmmoConfig_Details_Caliber.Items.Add(caliber);
			}

			TXT_ConfigAmmoName.Text = ammoMagazine.Name;
			TXT_ConfigAmmoId.Text = ammoMagazine.Id.ToString();
			CMB_AmmoConfig_Details_Caliber.SelectedItem = ammoMagazine.Caliber;
			TXT_ConfigAmmoCapacity.Text = ammoMagazine.Capacity.ToString(m_numberFormatInfo);
			TXT_ConfigAmmoVolume.Text = ammoMagazine.Volume.ToString(m_numberFormatInfo);
			TXT_ConfigAmmoMass.Text = ammoMagazine.Mass.ToString(m_numberFormatInfo);

			m_currentlySelecting = false;

			BTN_AmmoConfig_Details_Apply.Visible = false;
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
			int index = LST_AmmoConfiguration.SelectedIndex;

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(index);

			ammoMagazine.Caliber = (MyAmmoCategoryEnum) CMB_AmmoConfig_Details_Caliber.SelectedItem;
			ammoMagazine.Capacity = Convert.ToInt32(TXT_ConfigAmmoCapacity.Text, m_numberFormatInfo);
			ammoMagazine.Mass = Convert.ToSingle(TXT_ConfigAmmoMass.Text, m_numberFormatInfo);
			ammoMagazine.Volume = Convert.ToSingle(TXT_ConfigAmmoVolume.Text, m_numberFormatInfo);

			BTN_AmmoConfig_Details_Apply.Visible = false;
		}

		private void TXT_ConfigAmmo_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_AmmoConfig_Details_Apply.Visible = true;
			}
		}

		private void CMB_AmmoConfig_Details_Caliber_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_AmmoConfig_Details_Apply.Visible = true;
			}
		}

		#endregion

		#region ContainerTypes

		private void LST_ContainerTypeConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ContainerTypeConfiguration.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(index);

			TXT_ConfigContainerTypeName.Text = containerType.Name;
			TXT_ConfigContainerTypeId.Text = containerType.TypeId.ToString();
			TXT_ConfigContainerTypeItemCount.Text = containerType.ItemCount.ToString();
			TXT_ConfigContainerTypeCountMax.Text = containerType.CountMax.ToString();
			TXT_ConfigContainerTypeCountMin.Text = containerType.CountMin.ToString();

			LST_ContainerTypeConfig_Details_Items.Items.Clear();
			foreach (var def in containerType.Items)
			{
				LST_ContainerTypeConfig_Details_Items.Items.Add(def.Id.ToString());
			}

			CMB_ContainerTypesConfig_Items_Type.Items.Clear();
			foreach (var itemType in m_physicalItemsDefinitionsManager.Definitions)
			{
				CMB_ContainerTypesConfig_Items_Type.Items.Add(itemType.Id);
			}

			TXT_ContainerTypeConfig_ItemAmountMin.Text = "";
			TXT_ContainerTypeConfig_ItemAmountMax.Text = "";
			TXT_ContainerTypeConfig_ItemFrequency.Text = "";

			m_currentlySelecting = false;

			BTN_ContainerTypeConfig_Details_Apply.Visible = false;
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
			int index = LST_ContainerTypeConfiguration.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(index);

			containerType.CountMax = Convert.ToInt32(TXT_ConfigContainerTypeCountMax.Text, m_numberFormatInfo);
			containerType.CountMin = Convert.ToInt32(TXT_ConfigContainerTypeCountMin.Text, m_numberFormatInfo);

			BTN_ContainerTypeConfig_Details_Apply.Visible = false;
		}

		private void TXT_ConfigContainerType_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ContainerTypeConfig_Details_Apply.Visible = true;
			}
		}

		#region Items

		private void LST_ContainerTypeConfiguration_Items_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ContainerTypeConfig_Details_Items.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LST_ContainerTypeConfiguration.SelectedIndex);
			ContainerTypeItem containerItem = containerType.Items[index];

			CMB_ContainerTypesConfig_Items_Type.SelectedItem = containerItem.Id;

			TXT_ContainerTypeConfig_ItemAmountMin.Text = containerItem.AmountMin.ToString();
			TXT_ContainerTypeConfig_ItemAmountMax.Text = containerItem.AmountMax.ToString();
			TXT_ContainerTypeConfig_ItemFrequency.Text = containerItem.Frequency.ToString();

			m_currentlySelecting = false;

			BTN_ContainerTypeConfig_Items_Apply.Visible = false;
		}

		private void BTN_ContainerTypeConfig_Items_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_ContainerTypeConfiguration.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LST_ContainerTypeConfiguration.SelectedIndex);
			ContainerTypeItem containerItem = containerType.Items[index];

			containerItem.Id = (SerializableDefinitionId) CMB_ContainerTypesConfig_Items_Type.SelectedItem;
			containerItem.AmountMin = Convert.ToInt32(TXT_ContainerTypeConfig_ItemAmountMin.Text, m_numberFormatInfo);
			containerItem.AmountMax = Convert.ToInt32(TXT_ContainerTypeConfig_ItemAmountMax.Text, m_numberFormatInfo);
			containerItem.Frequency = Convert.ToSingle(TXT_ContainerTypeConfig_ItemFrequency.Text, m_numberFormatInfo);

			BTN_ContainerTypeConfig_Items_Apply.Visible = false;
		}

		private void TXT_ContainerTypeConfig_Item_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ContainerTypeConfig_Items_Apply.Visible = true;
			}
		}

		private void CMB_ContainerTypesConfig_Items_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ContainerTypeConfig_Items_Apply.Visible = true;
			}
		}

		#endregion

		#endregion

		#region GlobalEvents

		private void LST_GlobalEventConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_GlobalEventConfiguration.SelectedIndex;

			GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsManager.DefinitionOf(index);

			CMB_GlobalEventsConfig_Details_EventType.Items.Clear();
			foreach (var eventType in Enum.GetValues(typeof(MyGlobalEventTypeEnum)))
			{
				CMB_GlobalEventsConfig_Details_EventType.Items.Add(eventType);
			}

			TXT_ConfigGlobalEventId.Text = globalEvent.Id.ToString();
			TXT_ConfigGlobalEventName.Text = globalEvent.Name;
			TXT_ConfigGlobalEventDescription.Text = globalEvent.Description;
			CMB_GlobalEventsConfig_Details_EventType.SelectedItem = globalEvent.EventType;
			TXT_ConfigGlobalEventMinActivation.Text = globalEvent.MinActivation.ToString();
			TXT_ConfigGlobalEventMaxActivation.Text = globalEvent.MaxActivation.ToString();
			TXT_ConfigGlobalEventFirstActivation.Text = globalEvent.FirstActivation.ToString();

			m_currentlySelecting = false;

			BTN_GlobalEventConfig_Apply.Visible = false;
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
			int index = LST_GlobalEventConfiguration.SelectedIndex;

			GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsManager.DefinitionOf(index);

			globalEvent.Description = TXT_ConfigGlobalEventDescription.Text;
			globalEvent.EventType = (MyGlobalEventTypeEnum) CMB_GlobalEventsConfig_Details_EventType.SelectedItem;
			globalEvent.MinActivation = Convert.ToInt32(TXT_ConfigGlobalEventMinActivation.Text, m_numberFormatInfo);
			globalEvent.MaxActivation = Convert.ToInt32(TXT_ConfigGlobalEventMaxActivation.Text, m_numberFormatInfo);
			globalEvent.FirstActivation = Convert.ToInt32(TXT_ConfigGlobalEventFirstActivation.Text, m_numberFormatInfo);

			BTN_GlobalEventConfig_Apply.Visible = false;
		}

		private void TXT_ConfigGlobalEvent_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_GlobalEventConfig_Apply.Visible = true;
			}
		}

		private void CMB_GlobalEventsConfig_Details_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_GlobalEventConfig_Apply.Visible = true;
			}
		}

		#endregion

		#region SpawnGroups

		private void LST_SpawnGroupConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_SpawnGroupConfiguration.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(index);

			TXT_ConfigSpawnGroupPrefabCount.Text = spawnGroup.PrefabCount.ToString();
			TXT_ConfigSpawnGroupFrequency.Text = spawnGroup.Frequency.ToString();

			LST_SpawnGroupConfig_Details_Prefabs.Items.Clear();
			foreach (var def in spawnGroup.Prefabs)
			{
				LST_SpawnGroupConfig_Details_Prefabs.Items.Add(def.BeaconText);
			}
			TXT_SpawnGroupConfig_Details_PrefabFile.Text = "";
			TXT_SpawnGroupConfig_Details_PrefabPosition.Text = "";
			TXT_SpawnGroupConfig_Details_PrefabBeaconText.Text = "";
			TXT_SpawnGroupConfig_Details_PrefabSpeed.Text = "";

			m_currentlySelecting = false;

			BTN_SpawnGroupConfig_Details_Apply.Visible = false;
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
			int index = LST_SpawnGroupConfiguration.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(index);

			spawnGroup.Frequency = Convert.ToSingle(TXT_ConfigSpawnGroupFrequency.Text, m_numberFormatInfo);

			BTN_SpawnGroupConfig_Details_Apply.Visible = false;
		}

		private void TXT_ConfigSpawnGroup_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SpawnGroupConfig_Details_Apply.Visible = true;
			}
		}

		#region Prefabs

		private void LST_SpawnGroupConfig_Details_Prefabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(LST_SpawnGroupConfiguration.SelectedIndex);
			SpawnGroupPrefab spawnGroupPrefab = spawnGroup.Prefabs[index];

			TXT_SpawnGroupConfig_Details_PrefabFile.Text = spawnGroupPrefab.File;
			TXT_SpawnGroupConfig_Details_PrefabPosition.Text = spawnGroupPrefab.Position.ToString();
			TXT_SpawnGroupConfig_Details_PrefabBeaconText.Text = spawnGroupPrefab.BeaconText;
			TXT_SpawnGroupConfig_Details_PrefabSpeed.Text = spawnGroupPrefab.Speed.ToString();

			m_currentlySelecting = false;

			BTN_SpawnGroupConfig_Prefabs_Apply.Visible = false;
		}

		private void BTN_SpawnGroupConfig_Prefabs_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(LST_SpawnGroupConfiguration.SelectedIndex);
			SpawnGroupPrefab spawnGroupPrefab = spawnGroup.Prefabs[index];

			spawnGroupPrefab.BeaconText = TXT_SpawnGroupConfig_Details_PrefabBeaconText.Text;
			spawnGroupPrefab.Speed = Convert.ToSingle(TXT_SpawnGroupConfig_Details_PrefabSpeed.Text, m_numberFormatInfo);

			BTN_SpawnGroupConfig_Prefabs_Apply.Visible = false;
		}

		private void TXT_SpawnGroupConfig_Details_PrefabText_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_SpawnGroupConfig_Prefabs_Apply.Visible = true;
			}
		}

		#endregion

		#endregion

		#region PhysicalItems

		private void LST_PhysicalItemConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_PhysicalItemConfiguration.SelectedIndex;

			PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.DefinitionOf(index);

			TXT_PhysicalItemConfig_Id.Text = physicalItem.Id.ToString();
			TXT_PhysicalItemConfig_Name.Text = physicalItem.Name;
			TXT_PhysicalItemConfig_Description.Text = physicalItem.Description;
			TXT_PhysicalItemConfig_Size.Text = physicalItem.Size.ToString();
			TXT_PhysicalItemConfig_Mass.Text = physicalItem.Mass.ToString();
			TXT_PhysicalItemConfig_Volume.Text = physicalItem.Volume.ToString();
			TXT_PhysicalItemConfig_Model.Text = physicalItem.Model;
			TXT_PhysicalItemConfig_Icon.Text = physicalItem.Icon;
			try
			{
			TXT_PhysicalItemConfig_IconSymbol.Text = physicalItem.IconSymbol.ToString();
			}
			catch (InvalidOperationException NREx)
			{
				Console.WriteLine(NREx.ToString());
				TXT_PhysicalItemConfig_IconSymbol.Text = "";
			}
            
            

			m_currentlySelecting = false;

			BTN_PhysicalItemConfig_Details_Apply.Visible = false;
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
			int index = LST_PhysicalItemConfiguration.SelectedIndex;

			PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.DefinitionOf(index);

			physicalItem.Mass = Convert.ToSingle(TXT_PhysicalItemConfig_Mass.Text, m_numberFormatInfo);
			physicalItem.Volume = Convert.ToSingle(TXT_PhysicalItemConfig_Volume.Text, m_numberFormatInfo);

			BTN_PhysicalItemConfig_Details_Apply.Visible = false;
		}

		private void TXT_PhysicalItemConfig_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_PhysicalItemConfig_Details_Apply.Visible = true;
			}
		}

		#endregion

		#region Components

		private void LST_ComponentsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ComponentsConfig.SelectedIndex;

			ComponentsDefinition component = m_componentsDefinitionsManager.DefinitionOf(index);

			TXT_ComponentConfig_Id.Text = component.Id.ToString();
			TXT_ComponentConfig_Name.Text = component.Name;
			TXT_ComponentConfig_Description.Text = component.Description;
			TXT_ComponentConfig_Size.Text = component.Size.ToString();
			TXT_ComponentConfig_Mass.Text = component.Mass.ToString();
			TXT_ComponentConfig_Volume.Text = component.Volume.ToString();
			TXT_ComponentConfig_Model.Text = component.Model;
			TXT_ComponentConfig_Icon.Text = component.Icon;
			TXT_ComponentConfig_MaxIntegrity.Text = component.MaxIntegrity.ToString();
			TXT_ComponentConfig_DropProbability.Text = component.DropProbability.ToString();

			m_currentlySelecting = false;

			BTN_ComponentConfig_Details_Apply.Visible = false;
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

			component.Mass = Convert.ToSingle(TXT_ComponentConfig_Mass.Text, m_numberFormatInfo);
			component.Volume = Convert.ToSingle(TXT_ComponentConfig_Volume.Text, m_numberFormatInfo);
			component.MaxIntegrity = Convert.ToInt32(TXT_ComponentConfig_MaxIntegrity.Text, m_numberFormatInfo);
			component.DropProbability = Convert.ToSingle(TXT_ComponentConfig_DropProbability.Text, m_numberFormatInfo);

			BTN_ComponentConfig_Details_Apply.Visible = false;
		}

		private void TXT_ComponentConfig_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ComponentConfig_Details_Apply.Visible = true;
			}
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
			BTN_BlueprintConfig_Details_Apply.Visible = false;
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

			BTN_BlueprintConfig_Details_Apply.Visible = false;
		}

		private void TXT_BlueprintConfig_Details_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_BlueprintConfig_Details_Apply.Visible = true;
			}
		}

		#endregion

		#region VoxelMaterials

		private void LST_VoxelMaterialsConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_VoxelMaterialsConfig.SelectedIndex;

			VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsWrapper.DefinitionOf(index);

			TXT_VoxelMaterialsConfig_Details_Name.Text = voxelMaterial.Name;
			TXT_VoxelMaterialsConfig_Details_AssetName.Text = voxelMaterial.AssetName;
			TXT_VoxelMaterialsConfig_Details_MinedOre.Text = voxelMaterial.MinedOre;

			TXT_VoxelMaterialsConfig_Details_MinedOreRatio.Text = voxelMaterial.MinedOreRatio.ToString();
			TXT_VoxelMaterialsConfig_Details_DamageRatio.Text = voxelMaterial.DamageRatio.ToString();
			TXT_VoxelMaterialsConfig_Details_SpecularPower.Text = voxelMaterial.SpecularPower.ToString();
			TXT_VoxelMaterialsConfig_Details_SpecularShininess.Text = voxelMaterial.SpecularShininess.ToString();

			CHK_VoxelMaterialsConfig_Details_CanBeHarvested.Checked = voxelMaterial.CanBeHarvested;
			CHK_VoxelMaterialsConfig_Details_IsRare.Checked = voxelMaterial.IsRare;
			CHK_VoxelMaterialsConfig_Details_UseTwoTextures.Checked = voxelMaterial.UseTwoTextures;

			m_currentlySelecting = false;
			BTN_VoxelMaterialsConfig_Details_Apply.Visible = false;
		}

		private void BTN_VoxelMaterialsConfig_Reload_Click(object sender, EventArgs e)
		{
			FillVoxelMaterialConfigurationListBox();
		}

		private void BTN_VoxelMaterialsConfig_Save_Click(object sender, EventArgs e)
		{
			m_voxelMaterialsDefinitionsWrapper.Save();
		}

		private void BTN_VoxelMaterialsConfig_Details_Apply_Click(object sender, EventArgs e)
		{
			int index = LST_VoxelMaterialsConfig.SelectedIndex;

			VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsWrapper.DefinitionOf(index);

			voxelMaterial.Name = TXT_VoxelMaterialsConfig_Details_Name.Text;
			voxelMaterial.AssetName = TXT_VoxelMaterialsConfig_Details_AssetName.Text;
			voxelMaterial.MinedOre = TXT_VoxelMaterialsConfig_Details_MinedOre.Text;

			voxelMaterial.MinedOreRatio = Convert.ToSingle(TXT_VoxelMaterialsConfig_Details_MinedOreRatio.Text, m_numberFormatInfo);
			voxelMaterial.DamageRatio = Convert.ToSingle(TXT_VoxelMaterialsConfig_Details_DamageRatio.Text, m_numberFormatInfo);
			voxelMaterial.SpecularPower = Convert.ToSingle(TXT_VoxelMaterialsConfig_Details_SpecularPower.Text, m_numberFormatInfo);
			voxelMaterial.SpecularShininess = Convert.ToSingle(TXT_VoxelMaterialsConfig_Details_SpecularShininess.Text, m_numberFormatInfo);

			voxelMaterial.CanBeHarvested = CHK_VoxelMaterialsConfig_Details_CanBeHarvested.CheckState == CheckState.Checked;
			voxelMaterial.IsRare = CHK_VoxelMaterialsConfig_Details_IsRare.CheckState == CheckState.Checked;
			voxelMaterial.UseTwoTextures = CHK_VoxelMaterialsConfig_Details_UseTwoTextures.CheckState == CheckState.Checked;

			BTN_VoxelMaterialsConfig_Details_Apply.Visible = false;
		}

		private void TXT_VoxelMaterialsConfig_Details_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_VoxelMaterialsConfig_Details_Apply.Visible = true;
			}
		}

		private void CHK_VoxelMaterialsConfig_Details_CheckedChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_VoxelMaterialsConfig_Details_Apply.Visible = true;
			}
		}

		#endregion

		#region Scenarios

		private void LST_ScenariosConfig_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LST_ScenariosConfig.SelectedIndex;

			ScenariosDefinition scenario = m_scenariosDefinitionWrapper.DefinitionOf(index);

			TXT_ScenariosConfig_Details_Info_Id.Text = scenario.Id;
			TXT_ScenariosConfig_Details_Info_Name.Text = scenario.Name;

			CHK_ScenariosConfig_Details_Asteroid_Enabled.Checked = scenario.AsteroidClusters.Enabled;
			CHK_ScenariosConfig_Details_Asteroid_CentralCluster.Checked = scenario.AsteroidClusters.CentralCluster;
			TXT_ScenariosConfig_Details_Asteroid_Offset.Text = scenario.AsteroidClusters.Offset.ToString();

			m_currentlySelecting = false;
		}

		#endregion

		#endregion
	}
}
