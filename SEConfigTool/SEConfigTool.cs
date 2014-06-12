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

namespace SEConfigTool
{
    public partial class SEConfigTool : Form
    {
        #region Attributes

        private string m_standardSavePath;

        private ConfigFileSerializer m_configSerializer;

        private CubeBlockDefinitionsManager m_cubeBlockDefinitionsManager;
        private AmmoMagazinesDefinitionsManager m_ammoMagazinesDefinitionsManager;
        private ContainerTypesDefinitionsManager m_containerTypesDefinitionsManager;
        private GlobalEventsDefinitionsManager m_globalEventsDefinitionsManager;
        private SpawnGroupsDefinitionsManager m_spawnGroupsDefinitionsManager;
        private PhysicalItemDefinitionsManager m_physicalItemsDefinitionsManager;
        private ComponentDefinitionsManager m_componentsDefinitionsManager;
        private BlueprintDefinitionsManager m_blueprintsDefinitionsManager;
        private VoxelMaterialDefinitionsManager m_voxelMaterialsDefinitionsWrapper;

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
            SaveFileSerializer save = new SaveFileSerializer(saveFileInfo.FullName, m_configSerializer);
            Sector sector = save.Sector;

            TBX_SavedGame_Properties_Position.Text = sector.Position.ToString();
            TBX_SavedGame_Properties_AppVersion.Text = sector.AppVersion.ToString();

            foreach (Event currentEvent in sector.Events)
            {
                LBX_SavedGame_Events.Items.Add(currentEvent.Name);
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
        }

        private void FillBlocksConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            LBX_BlocksConfiguration.Items.Clear();
            m_cubeBlockDefinitionsManager = new CubeBlockDefinitionsManager(m_configSerializer.CubeBlockDefinitions);
            foreach (var definition in m_cubeBlockDefinitionsManager.Definitions)
            {
                LBX_BlocksConfiguration.Items.Add(definition.Name);
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillAmmoConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            LBX_AmmoConfiguration.Items.Clear();
            m_ammoMagazinesDefinitionsManager = new AmmoMagazinesDefinitionsManager(m_configSerializer.AmmoMagazineDefinitions);
            foreach (var definition in m_ammoMagazinesDefinitionsManager.Definitions)
            {
                LBX_AmmoConfiguration.Items.Add(definition.Name);
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillContainerTypeConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            LBX_ContainerTypeConfiguration.Items.Clear();
            LBX_ContainerTypeConfig_Details_Items.Items.Clear();
            m_containerTypesDefinitionsManager = new ContainerTypesDefinitionsManager(m_configSerializer.ContainerTypeDefinitions);
            foreach (var definition in m_containerTypesDefinitionsManager.Definitions)
            {
                LBX_ContainerTypeConfiguration.Items.Add(definition.Name);
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillGlobalEventConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            LBX_GlobalEventConfiguration.Items.Clear();
            m_globalEventsDefinitionsManager = new GlobalEventsDefinitionsManager(m_configSerializer.GlobalEventDefinitions);
            foreach (var definition in m_globalEventsDefinitionsManager.Definitions)
            {
                LBX_GlobalEventConfiguration.Items.Add(definition.Name);
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillSpawnGroupConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;
            LBX_SpawnGroupConfiguration.Items.Clear();
            LBX_SpawnGroupConfig_Details_Prefabs.Items.Clear();

            m_spawnGroupsDefinitionsManager = new SpawnGroupsDefinitionsManager(m_configSerializer.SpawnGroupDefinitions);

            foreach (var definition in m_spawnGroupsDefinitionsManager.Definitions)
            {
                //TODO - Find a better way to uniquely label the spawn groups
                LBX_SpawnGroupConfiguration.Items.Add("Spawn Group " + LBX_SpawnGroupConfiguration.Items.Count.ToString());
            }
            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillPhysicalItemConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            m_physicalItemsDefinitionsManager = new PhysicalItemDefinitionsManager(m_configSerializer.PhysicalItemDefinitions);
            LBX_PhysicalItemConfiguration.Items.Clear();
            foreach (var definition in m_physicalItemsDefinitionsManager.Definitions)
            {
                LBX_PhysicalItemConfiguration.Items.Add(definition.Name);
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillComponentConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            m_componentsDefinitionsManager = new ComponentDefinitionsManager(m_configSerializer.ComponentDefinitions);
            LBX_ComponentsConfig.Items.Clear();
            foreach (var definition in m_componentsDefinitionsManager.Definitions)
            {
                LBX_ComponentsConfig.Items.Add(definition.Name);
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillBlueprintConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            m_blueprintsDefinitionsManager = new BlueprintDefinitionsManager(m_configSerializer.BlueprintDefinitions);
            LBX_BlueprintConfig.Items.Clear();
            foreach (var definition in m_blueprintsDefinitionsManager.Definitions)
            {
                //TODO - Find a better way to uniquely label the spawn groups
                LBX_BlueprintConfig.Items.Add("Blueprint " + LBX_BlueprintConfig.Items.Count.ToString());
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillVoxelMaterialConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            m_voxelMaterialsDefinitionsWrapper = new VoxelMaterialDefinitionsManager(m_configSerializer.VoxelMaterialDefinitions);
            LBX_VoxelMaterialsConfig.Items.Clear();
            foreach (var definition in m_voxelMaterialsDefinitionsWrapper.Definitions)
            {
                LBX_VoxelMaterialsConfig.Items.Add(definition.Name);
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

        private void TRV_SavedGame_Objects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Ignore top-level nodes
            if (e.Node.Level == 0)
                return;

            //Sector object nodes
            if (e.Node.Level == 1)
            {
            }

            //Sector object parts nodes
            if (e.Node.Level == 2)
            {
            }
        }

        #endregion

        #region CubeBlock

        private void CB_BlocksConfig_ModelIntersection_CheckedChanged(object sender, EventArgs e)
        {
            int index = LBX_BlocksConfiguration.SelectedIndex;
            m_cubeBlockDefinitionsManager.DefinitionOf(index).UseModelIntersection = CB_BlocksConfig_ModelIntersection.Checked;
        }

        private void CB_BlocksConfig_Enabled_CheckedChanged(object sender, EventArgs e)
        {
            int index = LBX_BlocksConfiguration.SelectedIndex;
            m_cubeBlockDefinitionsManager.DefinitionOf(index).Enabled = CB_BlocksConfig_Enabled.Checked;
        }

        private void LBX_BlocksConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_BlocksConfiguration.SelectedIndex;

            CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsManager.DefinitionOf(index);

            TBX_ConfigBlockName.Text = cubeBlock.Name;
            TBX_ConfigBlockId.Text = cubeBlock.Id.ToString();
            TBX_ConfigBuildTime.Text = cubeBlock.BuildTime.ToString(m_numberFormatInfo);
            TBX_ConfigDisassembleRatio.Text = cubeBlock.DisassembleRatio.ToString(m_numberFormatInfo);
            CB_BlocksConfig_Enabled.Checked = cubeBlock.Enabled;
            CB_BlocksConfig_ModelIntersection.Checked = cubeBlock.UseModelIntersection;

            DGV_ConfigBlocks_Components.DataSource = cubeBlock.Components.ToArray().Select(x => new { x.Subtype, x.Count }).ToArray();

            m_currentlySelecting = false;

            BTN_ConfigApplyChanges.Visible = false;
        }

        private void TBX_ConfigBuildTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            if (ch == '.' && TBX_ConfigBuildTime.Text.IndexOf('.') != -1)
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

        private void TBX_ConfigBlocks_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_ConfigApplyChanges.Visible = true;
            }
        }

        private void BTN_ConfigApplyChanges_Click(object sender, EventArgs e)
        {
            int index = LBX_BlocksConfiguration.SelectedIndex;

            CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsManager.DefinitionOf(index);

            cubeBlock.BuildTime = Convert.ToSingle(TBX_ConfigBuildTime.Text, m_numberFormatInfo);
            cubeBlock.DisassembleRatio = Convert.ToSingle(TBX_ConfigDisassembleRatio.Text, m_numberFormatInfo);

            BTN_ConfigApplyChanges.Visible = false;
        }

        #endregion

        #region AmmoMagazines

        private void LBX_AmmoConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_AmmoConfiguration.SelectedIndex;

            AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(index);

            TBX_ConfigAmmoName.Text = ammoMagazine.Name;
            TBX_ConfigAmmoId.Text = ammoMagazine.Id.ToString();
            TBX_ConfigAmmoCaliber.Text = ammoMagazine.Caliber.ToString();
            TBX_ConfigAmmoCapacity.Text = ammoMagazine.Capacity.ToString(m_numberFormatInfo);
            TBX_ConfigAmmoVolume.Text = ammoMagazine.Volume.ToString(m_numberFormatInfo);
            TBX_ConfigAmmoMass.Text = ammoMagazine.Mass.ToString(m_numberFormatInfo);

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
            int index = LBX_AmmoConfiguration.SelectedIndex;

            AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsManager.DefinitionOf(index);

            ammoMagazine.Capacity = Convert.ToInt32(TBX_ConfigAmmoCapacity.Text, m_numberFormatInfo);
            ammoMagazine.Mass = Convert.ToSingle(TBX_ConfigAmmoMass.Text, m_numberFormatInfo);
            ammoMagazine.Volume = Convert.ToSingle(TBX_ConfigAmmoVolume.Text, m_numberFormatInfo);

            BTN_AmmoConfig_Details_Apply.Visible = false;
        }

        private void TBX_ConfigAmmo_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_AmmoConfig_Details_Apply.Visible = true;
            }
        }

        #endregion

        #region ContainerTypes

        private void LBX_ContainerTypeConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_ContainerTypeConfiguration.SelectedIndex;

            ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(index);

            TBX_ConfigContainerTypeName.Text = containerType.Name;
            TBX_ConfigContainerTypeId.Text = containerType.TypeId.ToString();
            TBX_ConfigContainerTypeItemCount.Text = containerType.ItemCount.ToString();
            TBX_ConfigContainerTypeCountMax.Text = containerType.CountMax.ToString();
            TBX_ConfigContainerTypeCountMin.Text = containerType.CountMin.ToString();

            LBX_ContainerTypeConfig_Details_Items.Items.Clear();
            foreach (var def in containerType.Items)
            {
                LBX_ContainerTypeConfig_Details_Items.Items.Add(def.Id.ToString());
            }
            TBX_ContainerTypeConfig_ItemType.Text = "";
            TBX_ContainerTypeConfig_ItemSubType.Text = "";
            TBX_ContainerTypeConfig_ItemAmountMin.Text = "";
            TBX_ContainerTypeConfig_ItemAmountMax.Text = "";
            TBX_ContainerTypeConfig_ItemFrequency.Text = "";

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
            int index = LBX_ContainerTypeConfiguration.SelectedIndex;

            ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(index);

            containerType.CountMax = Convert.ToInt32(TBX_ConfigContainerTypeCountMax.Text, m_numberFormatInfo);
            containerType.CountMin = Convert.ToInt32(TBX_ConfigContainerTypeCountMin.Text, m_numberFormatInfo);

            BTN_ContainerTypeConfig_Details_Apply.Visible = false;
        }

        private void TBX_ConfigContainerType_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_ContainerTypeConfig_Details_Apply.Visible = true;
            }
        }

        #region Items

        private void LBX_ContainerTypeConfiguration_Items_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_ContainerTypeConfig_Details_Items.SelectedIndex;

            ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LBX_ContainerTypeConfiguration.SelectedIndex);
            ContainerTypeItem containerItem = containerType.Items[index];

            TBX_ContainerTypeConfig_ItemType.Text = containerItem.Id.TypeId.ToString();
            TBX_ContainerTypeConfig_ItemSubType.Text = containerItem.Id.SubtypeId.ToString();
            TBX_ContainerTypeConfig_ItemAmountMin.Text = containerItem.AmountMin.ToString();
            TBX_ContainerTypeConfig_ItemAmountMax.Text = containerItem.AmountMax.ToString();
            TBX_ContainerTypeConfig_ItemFrequency.Text = containerItem.Frequency.ToString();

            m_currentlySelecting = false;

            BTN_ContainerTypeConfig_Items_Apply.Visible = false;
        }

        private void BTN_ContainerTypeConfig_Items_Apply_Click(object sender, EventArgs e)
        {
            int index = LBX_ContainerTypeConfiguration.SelectedIndex;

            ContainerTypesDefinition containerType = m_containerTypesDefinitionsManager.DefinitionOf(LBX_ContainerTypeConfiguration.SelectedIndex);
            ContainerTypeItem containerItem = containerType.Items[index];

            containerItem.AmountMin = Convert.ToInt32(TBX_ContainerTypeConfig_ItemAmountMin.Text, m_numberFormatInfo);
            containerItem.AmountMax = Convert.ToInt32(TBX_ContainerTypeConfig_ItemAmountMax.Text, m_numberFormatInfo);
            containerItem.Frequency = Convert.ToSingle(TBX_ContainerTypeConfig_ItemFrequency.Text, m_numberFormatInfo);

            BTN_ContainerTypeConfig_Items_Apply.Visible = false;
        }

        private void TBX_ContainerTypeConfig_Item_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_ContainerTypeConfig_Items_Apply.Visible = true;
            }
        }

        #endregion

        #endregion

        #region GlobalEvents

        private void LBX_GlobalEventConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_GlobalEventConfiguration.SelectedIndex;

            GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsManager.DefinitionOf(index);

            TBX_ConfigGlobalEventId.Text = globalEvent.Id.ToString();
            TBX_ConfigGlobalEventName.Text = globalEvent.Name;
            TBX_ConfigGlobalEventDescription.Text = globalEvent.Description;
            TBX_ConfigGlobalEventType.Text = globalEvent.EventType.ToString();
            TBX_ConfigGlobalEventMinActivation.Text = globalEvent.MinActivation.ToString();
            TBX_ConfigGlobalEventMaxActivation.Text = globalEvent.MaxActivation.ToString();
            TBX_ConfigGlobalEventFirstActivation.Text = globalEvent.FirstActivation.ToString();

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
            int index = LBX_GlobalEventConfiguration.SelectedIndex;

            GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsManager.DefinitionOf(index);

            globalEvent.Description = TBX_ConfigGlobalEventDescription.Text;

            globalEvent.MinActivation = Convert.ToInt32(TBX_ConfigGlobalEventMinActivation.Text, m_numberFormatInfo);
            globalEvent.MaxActivation = Convert.ToInt32(TBX_ConfigGlobalEventMaxActivation.Text, m_numberFormatInfo);
            globalEvent.FirstActivation = Convert.ToInt32(TBX_ConfigGlobalEventFirstActivation.Text, m_numberFormatInfo);

            BTN_GlobalEventConfig_Apply.Visible = false;
        }

        private void TBX_ConfigGlobalEvent_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_GlobalEventConfig_Apply.Visible = true;
            }
        }

        #endregion

        #region SpawnGroups

        private void LBX_SpawnGroupConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_SpawnGroupConfiguration.SelectedIndex;

            SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(index);

            TBX_ConfigSpawnGroupPrefabCount.Text = spawnGroup.PrefabCount.ToString();
            TBX_ConfigSpawnGroupFrequency.Text = spawnGroup.Frequency.ToString();

            LBX_SpawnGroupConfig_Details_Prefabs.Items.Clear();
            foreach (var def in spawnGroup.Prefabs)
            {
                LBX_SpawnGroupConfig_Details_Prefabs.Items.Add(def.BeaconText);
            }
            TBX_SpawnGroupConfig_Details_PrefabFile.Text = "";
            TBX_SpawnGroupConfig_Details_PrefabPosition.Text = "";
            TBX_SpawnGroupConfig_Details_PrefabBeaconText.Text = "";
            TBX_SpawnGroupConfig_Details_PrefabSpeed.Text = "";

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
            int index = LBX_SpawnGroupConfiguration.SelectedIndex;

            SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(index);

            spawnGroup.Frequency = Convert.ToSingle(TBX_ConfigSpawnGroupFrequency.Text, m_numberFormatInfo);

            BTN_SpawnGroupConfig_Details_Apply.Visible = false;
        }

        private void TBX_ConfigSpawnGroup_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_SpawnGroupConfig_Details_Apply.Visible = true;
            }
        }

        #region Prefabs

        private void LBX_SpawnGroupConfig_Details_Prefabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

            SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(LBX_SpawnGroupConfiguration.SelectedIndex);
            SpawnGroupPrefab spawnGroupPrefab = spawnGroup.Prefabs[index];

            TBX_SpawnGroupConfig_Details_PrefabFile.Text = spawnGroupPrefab.File;
            TBX_SpawnGroupConfig_Details_PrefabPosition.Text = spawnGroupPrefab.Position.ToString();
            TBX_SpawnGroupConfig_Details_PrefabBeaconText.Text = spawnGroupPrefab.BeaconText;
            TBX_SpawnGroupConfig_Details_PrefabSpeed.Text = spawnGroupPrefab.Speed.ToString();

            m_currentlySelecting = false;

            BTN_SpawnGroupConfig_Prefabs_Apply.Visible = false;
        }

        private void BTN_SpawnGroupConfig_Prefabs_Apply_Click(object sender, EventArgs e)
        {
            int index = LBX_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

            SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsManager.DefinitionOf(LBX_SpawnGroupConfiguration.SelectedIndex);
            SpawnGroupPrefab spawnGroupPrefab = spawnGroup.Prefabs[index];

            spawnGroupPrefab.BeaconText = TBX_SpawnGroupConfig_Details_PrefabBeaconText.Text;
            spawnGroupPrefab.Speed = Convert.ToSingle(TBX_SpawnGroupConfig_Details_PrefabSpeed.Text, m_numberFormatInfo);

            BTN_SpawnGroupConfig_Prefabs_Apply.Visible = false;
        }

        private void TBX_SpawnGroupConfig_Details_PrefabText_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_SpawnGroupConfig_Prefabs_Apply.Visible = true;
            }
        }

        #endregion

        #endregion

        #region PhysicalItems

        private void LBX_PhysicalItemConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_PhysicalItemConfiguration.SelectedIndex;

            PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.DefinitionOf(index);

            TBX_PhysicalItemConfig_Id.Text = physicalItem.Id.ToString();
            TBX_PhysicalItemConfig_Name.Text = physicalItem.Name;
            TBX_PhysicalItemConfig_Description.Text = physicalItem.Description;
            TBX_PhysicalItemConfig_Size.Text = physicalItem.Size.ToString();
            TBX_PhysicalItemConfig_Mass.Text = physicalItem.Mass.ToString();
            TBX_PhysicalItemConfig_Volume.Text = physicalItem.Volume.ToString();
            TBX_PhysicalItemConfig_Model.Text = physicalItem.Model;
            TBX_PhysicalItemConfig_Icon.Text = physicalItem.Icon;
            try
            {
                TBX_PhysicalItemConfig_IconSymbol.Text = physicalItem.IconSymbol.ToString();
            }
            catch (InvalidOperationException NREx)
            {
                Console.WriteLine(NREx.ToString());
                TBX_PhysicalItemConfig_IconSymbol.Text = "";
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
            int index = LBX_PhysicalItemConfiguration.SelectedIndex;

            PhysicalItemsDefinition physicalItem = m_physicalItemsDefinitionsManager.DefinitionOf(index);

            physicalItem.Mass = Convert.ToSingle(TBX_PhysicalItemConfig_Mass.Text, m_numberFormatInfo);
            physicalItem.Volume = Convert.ToSingle(TBX_PhysicalItemConfig_Volume.Text, m_numberFormatInfo);

            BTN_PhysicalItemConfig_Details_Apply.Visible = false;
        }

        private void TBX_PhysicalItemConfig_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_PhysicalItemConfig_Details_Apply.Visible = true;
            }
        }

        #endregion

        #region Components

        private void LBX_ComponentsConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_ComponentsConfig.SelectedIndex;

            ComponentsDefinition component = m_componentsDefinitionsManager.DefinitionOf(index);

            TBX_ComponentConfig_Id.Text = component.Id.ToString();
            TBX_ComponentConfig_Name.Text = component.Name;
            TBX_ComponentConfig_Description.Text = component.Description;
            TBX_ComponentConfig_Size.Text = component.Size.ToString();
            TBX_ComponentConfig_Mass.Text = component.Mass.ToString();
            TBX_ComponentConfig_Volume.Text = component.Volume.ToString();
            TBX_ComponentConfig_Model.Text = component.Model;
            TBX_ComponentConfig_Icon.Text = component.Icon;
            TBX_ComponentConfig_MaxIntegrity.Text = component.MaxIntegrity.ToString();
            TBX_ComponentConfig_DropProbability.Text = component.DropProbability.ToString();

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
            int index = LBX_ComponentsConfig.SelectedIndex;

            ComponentsDefinition component = m_componentsDefinitionsManager.DefinitionOf(index);

            component.Mass = Convert.ToSingle(TBX_ComponentConfig_Mass.Text, m_numberFormatInfo);
            component.Volume = Convert.ToSingle(TBX_ComponentConfig_Volume.Text, m_numberFormatInfo);
            component.MaxIntegrity = Convert.ToInt32(TBX_ComponentConfig_MaxIntegrity.Text, m_numberFormatInfo);
            component.DropProbability = Convert.ToSingle(TBX_ComponentConfig_DropProbability.Text, m_numberFormatInfo);

            BTN_ComponentConfig_Details_Apply.Visible = false;
        }

        private void TBX_ComponentConfig_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_ComponentConfig_Details_Apply.Visible = true;
            }
        }

        #endregion

        #region Blueprints

        private void LBX_BlueprintConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_BlueprintConfig.SelectedIndex;

            BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(index);

            TBX_BlueprintConfig_Details_Result.Text = blueprint.Result.TypeId.ToString() + "/" + blueprint.Result.SubtypeId + " x" + blueprint.Result.Amount.ToString();
            TBX_BlueprintConfig_Details_BaseProductionTime.Text = blueprint.BaseProductionTimeInSeconds.ToString();

            LBX_BlueprintConfig_Details_Prerequisites.Items.Clear();
            foreach (var prereq in blueprint.Prerequisites)
            {
                LBX_BlueprintConfig_Details_Prerequisites.Items.Add(prereq.TypeId.ToString() + "/" + prereq.SubtypeId + " x" + prereq.Amount.ToString());
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
            int index = LBX_BlueprintConfig.SelectedIndex;

            BlueprintsDefinition blueprint = m_blueprintsDefinitionsManager.DefinitionOf(index);

            blueprint.BaseProductionTimeInSeconds = Convert.ToSingle(TBX_BlueprintConfig_Details_BaseProductionTime.Text, m_numberFormatInfo);

            BTN_BlueprintConfig_Details_Apply.Visible = false;
        }

        private void TBX_BlueprintConfig_Details_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_BlueprintConfig_Details_Apply.Visible = true;
            }
        }

        #endregion

        #region VoxelMaterials

        private void LBX_VoxelMaterialsConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_VoxelMaterialsConfig.SelectedIndex;

            VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsWrapper.DefinitionOf(index);

            TBX_VoxelMaterialsConfig_Details_Name.Text = voxelMaterial.Name;
            TBX_VoxelMaterialsConfig_Details_AssetName.Text = voxelMaterial.AssetName;
            TBX_VoxelMaterialsConfig_Details_MinedOre.Text = voxelMaterial.MinedOre;

            TBX_VoxelMaterialsConfig_Details_MinedOreRatio.Text = voxelMaterial.MinedOreRatio.ToString();
            TBX_VoxelMaterialsConfig_Details_DamageRatio.Text = voxelMaterial.DamageRatio.ToString();
            TBX_VoxelMaterialsConfig_Details_SpecularPower.Text = voxelMaterial.SpecularPower.ToString();
            TBX_VoxelMaterialsConfig_Details_SpecularShininess.Text = voxelMaterial.SpecularShininess.ToString();

            //TODO - Convert these text fields to checkboxes
            TBX_VoxelMaterialsConfig_Details_CanBeHarvested.Text = "";
            TBX_VoxelMaterialsConfig_Details_IsRare.Text = "";
            TBX_VoxelMaterialsConfig_Details_UseTwoTextures.Text = "";

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
            int index = LBX_VoxelMaterialsConfig.SelectedIndex;

            VoxelMaterialsDefinition voxelMaterial = m_voxelMaterialsDefinitionsWrapper.DefinitionOf(index);

            voxelMaterial.Name = TBX_VoxelMaterialsConfig_Details_Name.Text;
            voxelMaterial.AssetName = TBX_VoxelMaterialsConfig_Details_AssetName.Text;
            voxelMaterial.MinedOre = TBX_VoxelMaterialsConfig_Details_MinedOre.Text;

            voxelMaterial.MinedOreRatio = Convert.ToSingle(TBX_VoxelMaterialsConfig_Details_MinedOreRatio.Text, m_numberFormatInfo);
            voxelMaterial.DamageRatio = Convert.ToSingle(TBX_VoxelMaterialsConfig_Details_DamageRatio.Text, m_numberFormatInfo);
            voxelMaterial.SpecularPower = Convert.ToSingle(TBX_VoxelMaterialsConfig_Details_SpecularPower.Text, m_numberFormatInfo);
            voxelMaterial.SpecularShininess = Convert.ToSingle(TBX_VoxelMaterialsConfig_Details_SpecularShininess.Text, m_numberFormatInfo);

            //TODO - Set these values once the fields are checkboxes
            voxelMaterial.CanBeHarvested = voxelMaterial.CanBeHarvested;
            voxelMaterial.IsRare = voxelMaterial.IsRare;
            voxelMaterial.UseTwoTextures = voxelMaterial.UseTwoTextures;

            BTN_VoxelMaterialsConfig_Details_Apply.Visible = false;
        }

        private void TBX_VoxelMaterialsConfig_Details_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_VoxelMaterialsConfig_Details_Apply.Visible = true;
            }
        }

        #endregion

        #endregion
    }
}
