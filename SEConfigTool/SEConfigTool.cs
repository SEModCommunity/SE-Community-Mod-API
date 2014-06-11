using System;
using System.Globalization;
using System.Windows.Forms;
using System.IO;

using SEModAPI;
using SEModAPI.API;
using SEModAPI.API.Definitions;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;

namespace SEConfigTool
{
    public partial class SEConfigTool : Form
    {
        #region Attributes

        private string m_standardSavePath;

        private ConfigFileSerializer m_configSerializer;

        private CubeBlockDefinitionsWrapper m_cubeBlockDefinitionsWrapper;
        private AmmoMagazinesDefinitionsWrapper m_ammoMagazinesDefinitionsWrapper;
		private ContainerTypesDefinitionsWrapper m_containerTypesDefinitionsWrapper;
		private GlobalEventsDefinitionsWrapper m_globalEventsDefinitionsWrapper;
		private SpawnGroupsDefinitionsWrapper m_spawnGroupsDefinitionsWrapper;
		private PhysicalItemDefinitionsWrapper<MyObjectBuilder_PhysicalItemDefinition> m_physicalItemsDefinitionsWrapper;
		private ComponentDefinitionsWrapper m_componentsDefinitionsWrapper;
		private BlueprintDefinitionsWrapper m_blueprintsDefinitionsWrapper;

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
            SaveFile save = new SaveFile(saveFileInfo.FullName, m_configSerializer);

            foreach (MyObjectBuilder_EntityBase currentObject in save.Objects)
            {
                float x = currentObject.PositionAndOrientation.Value.Position.x;
                float y = currentObject.PositionAndOrientation.Value.Position.y;
                float z = currentObject.PositionAndOrientation.Value.Position.z;

                float dist = (float)Math.Sqrt(x * x + y * y + z * z);

                LBX_SaveGameBlockList.Items.Add(currentObject.TypeId.ToString() + " | " + "Dist: " + dist.ToString("F2") + "m | " + x + ";" + z + ";" + y);
            }
        }

        private void FillBlocksConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            LBX_BlocksConfiguration.Items.Clear();
            m_cubeBlockDefinitionsWrapper = new CubeBlockDefinitionsWrapper(m_configSerializer.CubeBlockDefinitions);
			foreach (var definition in m_cubeBlockDefinitionsWrapper.Definitions)
            {
                LBX_BlocksConfiguration.Items.Add(definition.Name);
            }

            m_currentlyFillingConfigurationListBox = false;
        }

        private void FillAmmoConfigurationListBox()
        {
            m_currentlyFillingConfigurationListBox = true;

            LBX_AmmoConfiguration.Items.Clear();
            m_ammoMagazinesDefinitionsWrapper = new AmmoMagazinesDefinitionsWrapper(m_configSerializer.AmmoMagazineDefinitions);
			foreach (var definition in m_ammoMagazinesDefinitionsWrapper.Definitions)
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
			m_containerTypesDefinitionsWrapper = new ContainerTypesDefinitionsWrapper(m_configSerializer.ContainerTypeDefinitions);
			foreach (var definition in m_containerTypesDefinitionsWrapper.Definitions)
			{
				LBX_ContainerTypeConfiguration.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillGlobalEventConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			LBX_GlobalEventConfiguration.Items.Clear();
			m_globalEventsDefinitionsWrapper = new GlobalEventsDefinitionsWrapper(m_configSerializer.GlobalEventDefinitions);
			foreach (var definition in m_globalEventsDefinitionsWrapper.Definitions)
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

			m_spawnGroupsDefinitionsWrapper = new SpawnGroupsDefinitionsWrapper(m_configSerializer.SpawnGroupDefinitions);

			foreach (var definition in m_spawnGroupsDefinitionsWrapper.Definitions)
			{
				//TODO - Find a better way to uniquely label the spawn groups
				LBX_SpawnGroupConfiguration.Items.Add("Spawn Group " + LBX_SpawnGroupConfiguration.Items.Count.ToString());
			}
			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillPhysicalItemConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_physicalItemsDefinitionsWrapper = new PhysicalItemDefinitionsWrapper<MyObjectBuilder_PhysicalItemDefinition>(m_configSerializer.PhysicalItemDefinitions);
			LBX_PhysicalItemConfiguration.Items.Clear();
			foreach (var definition in m_physicalItemsDefinitionsWrapper.Definitions)
			{
				LBX_PhysicalItemConfiguration.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillComponentConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_componentsDefinitionsWrapper = new ComponentDefinitionsWrapper(m_configSerializer.ComponentDefinitions);
			LBX_ComponentsConfig.Items.Clear();
			foreach (var definition in m_componentsDefinitionsWrapper.Definitions)
			{
				LBX_ComponentsConfig.Items.Add(definition.Name);
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		private void FillBlueprintConfigurationListBox()
		{
			m_currentlyFillingConfigurationListBox = true;

			m_blueprintsDefinitionsWrapper = new BlueprintDefinitionsWrapper(m_configSerializer.BlueprintDefinitions);
			LBX_BlueprintConfig.Items.Clear();
			foreach (var definition in m_blueprintsDefinitionsWrapper.Definitions)
			{
				//TODO - Find a better way to uniquely label the spawn groups
				LBX_BlueprintConfig.Items.Add("Blueprint " + LBX_BlueprintConfig.Items.Count.ToString());
			}

			m_currentlyFillingConfigurationListBox = false;
		}

		#endregion

        #region Form events

        private void SEConfigTool_Load(object sender, EventArgs e)
        {
            m_configSerializer = new ConfigFileSerializer();
            m_standardSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            FillBlocksConfigurationListBox();
            FillAmmoConfigurationListBox();
			FillContainerTypeConfigurationListBox();
			FillGlobalEventConfigurationListBox();
			FillSpawnGroupConfigurationListBox();
			FillPhysicalItemConfigurationListBox();
			FillComponentConfigurationListBox();
			FillBlueprintConfigurationListBox();
        }

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

        #region CubeBlock

        private void LBX_BlocksConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_currentlySelecting = true;
            int index = LBX_BlocksConfiguration.SelectedIndex;

			CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsWrapper.GetDefinitionOf(index);

			TBX_ConfigBlockName.Text = cubeBlock.Name;
            TBX_ConfigBlockId.Text = cubeBlock.Id.ToString();
			TBX_ConfigBuildTime.Text = cubeBlock.BuildTime.ToString(m_numberFormatInfo);
			TBX_ConfigDisassembleRatio.Text = cubeBlock.DisassembleRatio.ToString(m_numberFormatInfo);

			LBX_BlocksConfig_Components.Items.Clear();
			foreach (var def in cubeBlock.Components)
			{
				LBX_BlocksConfig_Components.Items.Add(def.Subtype + " x" + def.Count.ToString());
			}

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
			m_cubeBlockDefinitionsWrapper.Save();
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

			CubeBlockDefinition cubeBlock = m_cubeBlockDefinitionsWrapper.GetDefinitionOf(index);

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

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsWrapper.GetDefinitionOf(index);

			TBX_ConfigAmmoName.Text = ammoMagazine.Name;
			TBX_ConfigAmmoId.Text = ammoMagazine.Id.ToString();
			TBX_ConfigAmmoCaliber.Text = ammoMagazine.Caliber.ToString();
			TBX_ConfigAmmoCapacity.Text = ammoMagazine.Capacity.ToString(m_numberFormatInfo);
			TBX_ConfigAmmoVolume.Text = ammoMagazine.Volume.ToString(m_numberFormatInfo);
			TBX_ConfigAmmoMass.Text = ammoMagazine.Mass.ToString(m_numberFormatInfo);

            m_currentlySelecting = false;

			BTN_ConfigAmmoApply.Visible = false;
        }

        private void BTN_ConfigAmmoReload_Click(object sender, EventArgs e)
        {
            FillAmmoConfigurationListBox();
        }

        private void BTN_SaveAmmoConfig_Click(object sender, EventArgs e)
        {
			m_ammoMagazinesDefinitionsWrapper.Save();
        }

        private void BTN_ConfigAmmoApply_Click(object sender, EventArgs e)
        {
            int index = LBX_AmmoConfiguration.SelectedIndex;

			AmmoMagazinesDefinition ammoMagazine = m_ammoMagazinesDefinitionsWrapper.GetDefinitionOf(index);

			ammoMagazine.Capacity = Convert.ToInt32(TBX_ConfigAmmoCapacity.Text, m_numberFormatInfo);
			ammoMagazine.Mass = Convert.ToSingle(TBX_ConfigAmmoMass.Text, m_numberFormatInfo);
			ammoMagazine.Volume = Convert.ToSingle(TBX_ConfigAmmoVolume.Text, m_numberFormatInfo);

            BTN_ConfigAmmoApply.Visible = false;
        }

        private void TBX_ConfigAmmo_TextChanged(object sender, EventArgs e)
        {
            if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
            {
                BTN_ConfigAmmoApply.Visible = true;
            }
        }

        #endregion

		#region ContainerTypes

		private void LBX_ContainerTypeConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LBX_ContainerTypeConfiguration.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsWrapper.GetDefinitionOf(index);

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

			BTN_ConfigContainerTypeApply.Visible = false;
		}

		private void BTN_ConfigContainerTypeReload_Click(object sender, EventArgs e)
		{
			FillContainerTypeConfigurationListBox();
		}

		private void BTN_SaveContainerTypeConfig_Click(object sender, EventArgs e)
		{
			m_containerTypesDefinitionsWrapper.Save();
		}

		private void BTN_ConfigContainerTypeApply_Click(object sender, EventArgs e)
		{
			int index = LBX_ContainerTypeConfiguration.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsWrapper.GetDefinitionOf(index);

			containerType.CountMax = Convert.ToInt32(TBX_ConfigContainerTypeCountMax.Text, m_numberFormatInfo);
			containerType.CountMin = Convert.ToInt32(TBX_ConfigContainerTypeCountMin.Text, m_numberFormatInfo);

			BTN_ConfigContainerTypeApply.Visible = false;
		}

		private void TBX_ConfigContainerType_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ConfigContainerTypeApply.Visible = true;
			}
		}

		#region Items

		private void LBX_ContainerTypeConfiguration_Items_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LBX_ContainerTypeConfig_Details_Items.SelectedIndex;

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsWrapper.GetDefinitionOf(LBX_ContainerTypeConfiguration.SelectedIndex);
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

			ContainerTypesDefinition containerType = m_containerTypesDefinitionsWrapper.GetDefinitionOf(LBX_ContainerTypeConfiguration.SelectedIndex);
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

			GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsWrapper.GetDefinitionOf(index);

			TBX_ConfigGlobalEventId.Text = globalEvent.Id.ToString();
			TBX_ConfigGlobalEventName.Text = globalEvent.Name;
			TBX_ConfigGlobalEventDescription.Text = globalEvent.Description;
			TBX_ConfigGlobalEventType.Text = globalEvent.EventType.ToString();
			TBX_ConfigGlobalEventMinActivation.Text = globalEvent.MinActivation.ToString();
			TBX_ConfigGlobalEventMaxActivation.Text = globalEvent.MaxActivation.ToString();
			TBX_ConfigGlobalEventFirstActivation.Text = globalEvent.FirstActivation.ToString();

			m_currentlySelecting = false;

			BTN_ConfigGlobalEventApply.Visible = false;
		}

		private void BTN_ConfigGlobalEventReload_Click(object sender, EventArgs e)
		{
			FillGlobalEventConfigurationListBox();
		}

		private void BTN_SaveGlobalEventConfig_Click(object sender, EventArgs e)
		{
			m_globalEventsDefinitionsWrapper.Save();
		}

		private void BTN_ConfigGlobalEventApply_Click(object sender, EventArgs e)
		{
			int index = LBX_GlobalEventConfiguration.SelectedIndex;

			GlobalEventsDefinition globalEvent = m_globalEventsDefinitionsWrapper.GetDefinitionOf(index);

			globalEvent.MinActivation = Convert.ToInt32(TBX_ConfigGlobalEventMinActivation.Text, m_numberFormatInfo);
			globalEvent.MaxActivation = Convert.ToInt32(TBX_ConfigGlobalEventMaxActivation.Text, m_numberFormatInfo);
			globalEvent.FirstActivation = Convert.ToInt32(TBX_ConfigGlobalEventFirstActivation.Text, m_numberFormatInfo);

			BTN_ConfigGlobalEventApply.Visible = false;
		}

		private void TBX_ConfigGlobalEvent_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ConfigGlobalEventApply.Visible = true;
			}
		}

		#endregion

		#region SpawnGroups

		private void LBX_SpawnGroupConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LBX_SpawnGroupConfiguration.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsWrapper.GetDefinitionOf(index);

			TBX_ConfigSpawnGroupName.Text = spawnGroup.Name;
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

			BTN_ConfigSpawnGroupApply.Visible = false;
		}

		private void BTN_ConfigSpawnGroupReload_Click(object sender, EventArgs e)
		{
			FillSpawnGroupConfigurationListBox();
		}

		private void BTN_SaveSpawnGroupConfig_Click(object sender, EventArgs e)
		{
			m_spawnGroupsDefinitionsWrapper.Save();
		}

		private void BTN_ConfigSpawnGroupApply_Click(object sender, EventArgs e)
		{
			int index = LBX_SpawnGroupConfiguration.SelectedIndex;
			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsWrapper.GetDefinitionOf(index);
			spawnGroup.Frequency = Convert.ToSingle(TBX_ConfigSpawnGroupFrequency.Text, m_numberFormatInfo);

			BTN_ConfigSpawnGroupApply.Visible = false;
		}

		private void TBX_ConfigSpawnGroup_TextChanged(object sender, EventArgs e)
		{
			if (!m_currentlyFillingConfigurationListBox && !m_currentlySelecting)
			{
				BTN_ConfigSpawnGroupApply.Visible = true;
			}
		}

		#region Prefabs

		private void LBX_SpawnGroupConfig_Details_Prefabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			m_currentlySelecting = true;
			int index = LBX_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsWrapper.GetDefinitionOf(LBX_SpawnGroupConfiguration.SelectedIndex);
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

			SpawnGroupDefinition spawnGroup = m_spawnGroupsDefinitionsWrapper.GetDefinitionOf(LBX_SpawnGroupConfiguration.SelectedIndex);
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

			PhysicalItemsDefinition<MyObjectBuilder_PhysicalItemDefinition> physicalItem = m_physicalItemsDefinitionsWrapper.GetDefinitionOf(index);

			TBX_PhysicalItemConfig_Id.Text = physicalItem.Id.ToString();
			TBX_PhysicalItemConfig_Name.Text = physicalItem.Name;
			TBX_PhysicalItemConfig_Description.Text = physicalItem.Description;
			TBX_PhysicalItemConfig_Size.Text = physicalItem.Size.ToString();
			TBX_PhysicalItemConfig_Mass.Text = physicalItem.Mass.ToString();
			TBX_PhysicalItemConfig_Volume.Text = physicalItem.Volume.ToString();
			TBX_PhysicalItemConfig_Model.Text = physicalItem.Model;
			TBX_PhysicalItemConfig_Icon.Text = physicalItem.Icon;
			TBX_PhysicalItemConfig_IconSymbol.Text = physicalItem.IconSymbol.ToString();

			m_currentlySelecting = false;

			BTN_PhysicalItemConfig_Details_Apply.Visible = false;
		}

		private void BTN_ConfigPhysicalItemReload_Click(object sender, EventArgs e)
		{
			FillPhysicalItemConfigurationListBox();
		}

		private void BTN_SavePhysicalItemConfig_Click(object sender, EventArgs e)
		{
			m_physicalItemsDefinitionsWrapper.Save();
		}

		private void BTN_ConfigPhysicalItemApply_Click(object sender, EventArgs e)
		{
			int index = LBX_PhysicalItemConfiguration.SelectedIndex;

			PhysicalItemsDefinition<MyObjectBuilder_PhysicalItemDefinition> physicalItem = m_physicalItemsDefinitionsWrapper.GetDefinitionOf(index);

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

			ComponentsDefinition component = (ComponentsDefinition) m_componentsDefinitionsWrapper.GetDefinitionOf(index);

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
			m_componentsDefinitionsWrapper.Save();
		}

		private void BTN_ComponentConfig_Details_Apply_Click(object sender, EventArgs e)
		{
			int index = LBX_ComponentsConfig.SelectedIndex;

			ComponentsDefinition component = (ComponentsDefinition) m_componentsDefinitionsWrapper.GetDefinitionOf(index);

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

			BlueprintsDefinition blueprint = m_blueprintsDefinitionsWrapper.GetDefinitionOf(index);

			TBX_BlueprintConfig_Details_Result.Text = blueprint.Result.TypeId.ToString() + "/" + blueprint.Result.SubtypeId;
			TBX_BlueprintConfig_Details_BaseProductionTime.Text = blueprint.BaseProductionTimeInSeconds.ToString();

			LBX_BlueprintConfig_Details_Prerequisites.Items.Clear();
			foreach (var prereq in blueprint.Prerequisites)
			{
				LBX_BlueprintConfig_Details_Prerequisites.Items.Add(prereq.TypeId.ToString() + "/" + prereq.SubtypeId);
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
			m_blueprintsDefinitionsWrapper.Save();
		}

		private void BTN_BlueprintConfig_Details_Apply_Click(object sender, EventArgs e)
		{
			int index = LBX_BlueprintConfig.SelectedIndex;

			BlueprintsDefinition blueprint = m_blueprintsDefinitionsWrapper.GetDefinitionOf(index);

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

		#endregion
	}
}
