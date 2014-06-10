using System;
using System.Globalization;
using System.Windows.Forms;
using SEModAPI;
using SEModAPI.API;
using Sandbox.Common.ObjectBuilders;
using System.IO;

namespace SEConfigTool
{
    public partial class SEConfigTool : Form
    {
        #region Attributes

        private string _standardSavePath;
        private ConfigFileSerializer _serializer;
        private CubeBlockDefinitionsWrapper _cubeBlockDefinitionsWrapper;
        private AmmoMagazinesDefinitionsWrapper _ammoMagazinesDefinitionsWrapper;
		private ContainerTypesDefinitionsWrapper _containerTypesDefinitionsWrapper;
		private GlobalEventsDefinitionsWrapper _globalEventsDefinitionsWrapper;
		private SpawnGroupsDefinitionsWrapper _spawnGroupsDefinitionsWrapper;

        private bool _currentlyFillingConfigurationListBox;
        private bool _currentlySelecting;

        private NumberFormatInfo _numberFormatInfo;
        private string _decimalSeparator;
        private string _groupSeparator;
        private string _negativeSign;

        #endregion

        #region Properties

        #endregion

        public SEConfigTool()
        {
            InitializeComponent();

            _numberFormatInfo = CultureInfo.GetCultureInfo("EN-US").NumberFormat;
            _decimalSeparator = _numberFormatInfo.CurrencyDecimalSeparator;
            _groupSeparator = _numberFormatInfo.NumberGroupSeparator;
            _negativeSign = _numberFormatInfo.NegativeSign;
        }

        #region Form methods

        private void LoadSaveFile(FileInfo saveFileInfo)
        {
            SaveFile save = new SaveFile(saveFileInfo.FullName, _serializer);

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
            _currentlyFillingConfigurationListBox = true;
            LBX_BlocksConfiguration.Items.Clear();

            TBX_ConfigBlockId.Text = "";
            TBX_ConfigBlockName.Text = "";
            TBX_ConfigBuildTime.Text = "";
            TBX_ConfigDisassembleRatio.Text = "";

            _cubeBlockDefinitionsWrapper = new CubeBlockDefinitionsWrapper(_serializer.CubeBlockDefinitions);

            foreach (var definition in _serializer.CubeBlockDefinitions)
            {
                LBX_BlocksConfiguration.Items.Add(definition.BlockPairName);
            }
            _currentlyFillingConfigurationListBox = false;
        }

        private void FillAmmoConfigurationListBox()
        {
            _currentlyFillingConfigurationListBox = true;
            LBX_AmmoConfiguration.Items.Clear();

            //TBX_ConfigBlockId.Text = "";
            //TBX_ConfigBlockName.Text = "";
            //TBX_ConfigBuildTime.Text = "";
            //TBX_ConfigDisassembleRatio.Text = "";

            _ammoMagazinesDefinitionsWrapper = new AmmoMagazinesDefinitionsWrapper(_serializer.AmmoMagazineDefinitions);

            foreach (var definition in _serializer.AmmoMagazineDefinitions)
            {
                LBX_AmmoConfiguration.Items.Add(definition.DisplayName);
            }
            _currentlyFillingConfigurationListBox = false;
        }

		private void FillContainerTypeConfigurationListBox()
		{
			_currentlyFillingConfigurationListBox = true;
			LBX_ContainerTypeConfiguration.Items.Clear();
			LBX_ContainerTypeConfig_Details_Items.Items.Clear();

			_containerTypesDefinitionsWrapper = new ContainerTypesDefinitionsWrapper(_serializer.ContainerTypeDefinitions);

			foreach (var definition in _serializer.ContainerTypeDefinitions)
			{
				LBX_ContainerTypeConfiguration.Items.Add(definition.Name);
			}
			_currentlyFillingConfigurationListBox = false;
		}

		private void FillGlobalEventConfigurationListBox()
		{
			_currentlyFillingConfigurationListBox = true;
			LBX_GlobalEventConfiguration.Items.Clear();

			_globalEventsDefinitionsWrapper = new GlobalEventsDefinitionsWrapper(_serializer.GlobalEventDefinitions);

			foreach (var definition in _serializer.GlobalEventDefinitions)
			{
				LBX_GlobalEventConfiguration.Items.Add(definition.DisplayName);
			}
			_currentlyFillingConfigurationListBox = false;
		}

		private void FillSpawnGroupConfigurationListBox()
		{
			_currentlyFillingConfigurationListBox = true;
			LBX_SpawnGroupConfiguration.Items.Clear();
			LBX_SpawnGroupConfig_Details_Prefabs.Items.Clear();

			_spawnGroupsDefinitionsWrapper = new SpawnGroupsDefinitionsWrapper(_serializer.SpawnGroupDefinitions);

			foreach (var definition in _serializer.SpawnGroupDefinitions)
			{
				//TODO - Find a better way to uniquely label the spawn groups
				LBX_SpawnGroupConfiguration.Items.Add("Spawn Group " + LBX_SpawnGroupConfiguration.Items.Count.ToString());
			}
			_currentlyFillingConfigurationListBox = false;
		}

		#endregion

        #region Form events

        private void SEConfigTool_Load(object sender, EventArgs e)
        {
            _serializer = new ConfigFileSerializer();
            _standardSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            FillBlocksConfigurationListBox();
            FillAmmoConfigurationListBox();
			FillContainerTypeConfigurationListBox();
			FillGlobalEventConfigurationListBox();
			FillSpawnGroupConfigurationListBox();
        }

        private void BTN_LoadSaveGame_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _standardSavePath,
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
            _currentlySelecting = true;
            int index = LBX_BlocksConfiguration.SelectedIndex;

			CubeBlockDefinition cubeBlock = _cubeBlockDefinitionsWrapper.GetDefinitionOf(index);

			TBX_ConfigBlockName.Text = cubeBlock.Name;
            TBX_ConfigBlockId.Text = cubeBlock.Id.ToString();
			TBX_ConfigBuildTime.Text = cubeBlock.BuildTime.ToString(_numberFormatInfo);
			TBX_ConfigDisassembleRatio.Text = cubeBlock.DisassembleRatio.ToString(_numberFormatInfo);

			LBX_BlocksConfig_Components.Items.Clear();
			foreach (var def in cubeBlock.Components)
			{
				LBX_BlocksConfig_Components.Items.Add(def.Subtype + " x" + def.Count.ToString());
			}

            _currentlySelecting = false;
        }

        private void TBX_ConfigBuildTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            char ch = e.KeyChar;

            if (ch == '.' && TBX_ConfigBuildTime.Text.IndexOf('.') != -1)
            {
                e.Handled = true;
            }

            if (!Char.IsDigit(ch) && ch != _decimalSeparator[0] && ch != 8) //8 for ASCII (backspace)
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
            if (!_cubeBlockDefinitionsWrapper.Changed) return;
            _serializer.CubeBlockDefinitions = _cubeBlockDefinitionsWrapper.RawDefinitions;
            _serializer.SaveCubeBlocksContentFile();
        }

        private void TBX_ConfigBuildTime_TextChanged(object sender, EventArgs e)
        {
            if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
            {
                BTN_ConfigApplyChanges.Visible = true;
            }
        }

        private void BTN_ConfigApplyChanges_Click(object sender, EventArgs e)
        {
            int index = LBX_BlocksConfiguration.SelectedIndex;

			CubeBlockDefinition cubeBlock = _cubeBlockDefinitionsWrapper.GetDefinitionOf(index);

			cubeBlock.BuildTime = Convert.ToSingle(TBX_ConfigBuildTime.Text, _numberFormatInfo);
            cubeBlock.DisassembleRatio = Convert.ToSingle(TBX_ConfigDisassembleRatio.Text, _numberFormatInfo);

            BTN_ConfigApplyChanges.Visible = false;
        }

        #endregion

        #region AmmoMagazines

        private void LBX_AmmoConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentlySelecting = true;
            int index = LBX_AmmoConfiguration.SelectedIndex;

			AmmoMagazinesDefinition ammoMagazine = _ammoMagazinesDefinitionsWrapper.GetDefinitionOf(index);

			TBX_ConfigAmmoName.Text = ammoMagazine.Name;
			TBX_ConfigAmmoId.Text = ammoMagazine.Id.ToString();
			TBX_ConfigAmmoCaliber.Text = ammoMagazine.Caliber.ToString();
			TBX_ConfigAmmoCapacity.Text = ammoMagazine.Capacity.ToString(_numberFormatInfo);
			TBX_ConfigAmmoVolume.Text = ammoMagazine.Volume.ToString(_numberFormatInfo);
			TBX_ConfigAmmoMass.Text = ammoMagazine.Mass.ToString(_numberFormatInfo);

            _currentlySelecting = false;
        }

        private void BTN_ConfigAmmoReload_Click(object sender, EventArgs e)
        {
            FillAmmoConfigurationListBox();
        }

        private void BTN_SaveAmmoConfig_Click(object sender, EventArgs e)
        {
            if (!_ammoMagazinesDefinitionsWrapper.Changed) return;
            _serializer.AmmoMagazineDefinitions = _ammoMagazinesDefinitionsWrapper.RawDefinitions;
            _serializer.SaveAmmoMagazinesContentFile();
        }

        private void BTN_ConfigAmmoApply_Click(object sender, EventArgs e)
        {
            int index = LBX_AmmoConfiguration.SelectedIndex;

			AmmoMagazinesDefinition ammoMagazine = _ammoMagazinesDefinitionsWrapper.GetDefinitionOf(index);

			ammoMagazine.Capacity = Convert.ToInt32(TBX_ConfigAmmoCapacity.Text, _numberFormatInfo);
			ammoMagazine.Mass = Convert.ToSingle(TBX_ConfigAmmoMass.Text, _numberFormatInfo);
			ammoMagazine.Volume = Convert.ToSingle(TBX_ConfigAmmoVolume.Text, _numberFormatInfo);

            BTN_ConfigAmmoApply.Visible = false;
        }

        private void TBX_ConfigAmmo_TextChanged(object sender, EventArgs e)
        {
            if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
            {
                BTN_ConfigAmmoApply.Visible = true;
            }
        }

        #endregion

		#region ContainerTypes

		private void LBX_ContainerTypeConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentlySelecting = true;
			int index = LBX_ContainerTypeConfiguration.SelectedIndex;

			ContainerTypesDefinition containerType = _containerTypesDefinitionsWrapper.GetDefinitionOf(index);

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

			_currentlySelecting = false;
		}

		private void BTN_ConfigContainerTypeReload_Click(object sender, EventArgs e)
		{
			FillContainerTypeConfigurationListBox();
		}

		private void BTN_SaveContainerTypeConfig_Click(object sender, EventArgs e)
		{
			if (!_containerTypesDefinitionsWrapper.Changed) return;
			_serializer.ContainerTypeDefinitions = _containerTypesDefinitionsWrapper.RawDefinitions;
			_serializer.SaveContainerTypesContentFile();
		}

		private void BTN_ConfigContainerTypeApply_Click(object sender, EventArgs e)
		{
			int index = LBX_ContainerTypeConfiguration.SelectedIndex;

			ContainerTypesDefinition containerType = _containerTypesDefinitionsWrapper.GetDefinitionOf(index);

			containerType.CountMax = Convert.ToInt32(TBX_ConfigContainerTypeCountMax.Text, _numberFormatInfo);
			containerType.CountMin = Convert.ToInt32(TBX_ConfigContainerTypeCountMin.Text, _numberFormatInfo);

			BTN_ConfigContainerTypeApply.Visible = false;
		}

		private void TBX_ConfigContainerTypeCountMax_TextChanged(object sender, EventArgs e)
		{
			if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
			{
				BTN_ConfigContainerTypeApply.Visible = true;
			}
		}

		private void TBX_ConfigContainerTypeCountMin_TextChanged(object sender, EventArgs e)
		{
			if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
			{
				BTN_ConfigContainerTypeApply.Visible = true;
			}
		}

		#region Items

		private void LBX_ContainerTypeConfiguration_Items_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentlySelecting = true;
			int index = LBX_ContainerTypeConfig_Details_Items.SelectedIndex;

			ContainerTypesDefinition containerType = _containerTypesDefinitionsWrapper.GetDefinitionOf(LBX_ContainerTypeConfiguration.SelectedIndex);

			var items = containerType.Items;

			TBX_ContainerTypeConfig_ItemType.Text = items[index].Id.TypeId.ToString();
			TBX_ContainerTypeConfig_ItemSubType.Text = items[index].Id.SubtypeId.ToString();
			TBX_ContainerTypeConfig_ItemAmountMin.Text = items[index].AmountMin.ToString();
			TBX_ContainerTypeConfig_ItemAmountMax.Text = items[index].AmountMax.ToString();
			TBX_ContainerTypeConfig_ItemFrequency.Text = items[index].Frequency.ToString();

			_currentlySelecting = false;
		}

		#endregion

		#endregion

		#region GlobalEvents

		private void LBX_GlobalEventConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentlySelecting = true;
			int index = LBX_GlobalEventConfiguration.SelectedIndex;

			GlobalEventsDefinition globalEvent = _globalEventsDefinitionsWrapper.GetDefinitionOf(index);

			TBX_ConfigGlobalEventId.Text = globalEvent.SubtypeId;
			TBX_ConfigGlobalEventName.Text = globalEvent.Name;
			TBX_ConfigGlobalEventDescription.Text = globalEvent.Description;
			TBX_ConfigGlobalEventType.Text = globalEvent.TypeId.ToString();
			TBX_ConfigGlobalEventMinActivation.Text = globalEvent.MinActivation.ToString();
			TBX_ConfigGlobalEventMaxActivation.Text = globalEvent.MaxActivation.ToString();
			TBX_ConfigGlobalEventFirstActivation.Text = globalEvent.FirstActivation.ToString();

			_currentlySelecting = false;
		}

		private void BTN_ConfigGlobalEventReload_Click(object sender, EventArgs e)
		{
			FillGlobalEventConfigurationListBox();
		}

		private void BTN_SaveGlobalEventConfig_Click(object sender, EventArgs e)
		{
			if (!_globalEventsDefinitionsWrapper.Changed) return;
			_serializer.SetGlobalEventsDefinitions(_globalEventsDefinitionsWrapper.RawDefinitions);
			_serializer.SaveGlobalEventsContentFile();
		}

		private void BTN_ConfigGlobalEventApply_Click(object sender, EventArgs e)
		{
			int index = LBX_GlobalEventConfiguration.SelectedIndex;

			GlobalEventsDefinition globalEvent = _globalEventsDefinitionsWrapper.GetDefinitionOf(index);

			globalEvent.MinActivation = Convert.ToInt32(TBX_ConfigGlobalEventMinActivation.Text, _numberFormatInfo);
			globalEvent.MaxActivation = Convert.ToInt32(TBX_ConfigGlobalEventMaxActivation.Text, _numberFormatInfo);
			globalEvent.FirstActivation = Convert.ToInt32(TBX_ConfigGlobalEventFirstActivation.Text, _numberFormatInfo);

			BTN_ConfigGlobalEventApply.Visible = false;
		}

		private void TBX_ConfigGlobalEventMinActivation_TextChanged(object sender, EventArgs e)
		{
			if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
			{
				BTN_ConfigGlobalEventApply.Visible = true;
			}
		}

		private void TBX_ConfigGlobalEventMaxActivation_TextChanged(object sender, EventArgs e)
		{
			if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
			{
				BTN_ConfigGlobalEventApply.Visible = true;
			}
		}

		private void TBX_ConfigGlobalEventFirstActivation_TextChanged(object sender, EventArgs e)
		{
			if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
			{
				BTN_ConfigGlobalEventApply.Visible = true;
			}
		}

		#endregion

		#region SpawnGroups

		private void LBX_SpawnGroupConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentlySelecting = true;
			int index = LBX_SpawnGroupConfiguration.SelectedIndex;

			SpawnGroupDefinition spawnGroup = _spawnGroupsDefinitionsWrapper.GetDefinitionOf(index);

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

			_currentlySelecting = false;
		}

		private void BTN_ConfigSpawnGroupReload_Click(object sender, EventArgs e)
		{
			FillSpawnGroupConfigurationListBox();
		}

		private void BTN_SaveSpawnGroupConfig_Click(object sender, EventArgs e)
		{
			if (!_spawnGroupsDefinitionsWrapper.Changed) return;
			_serializer.SpawnGroupDefinitions = _spawnGroupsDefinitionsWrapper.RawDefinitions;
			_serializer.SaveSpawnGroupsContentFile();
		}

		private void BTN_ConfigSpawnGroupApply_Click(object sender, EventArgs e)
		{
			int index = LBX_SpawnGroupConfiguration.SelectedIndex;
			SpawnGroupDefinition spawnGroup = _spawnGroupsDefinitionsWrapper.GetDefinitionOf(index);
			spawnGroup.Frequency = Convert.ToSingle(TBX_ConfigSpawnGroupFrequency.Text, _numberFormatInfo);

			BTN_ConfigSpawnGroupApply.Visible = false;
		}

		private void TBX_ConfigSpawnGroupFrequency_TextChanged(object sender, EventArgs e)
		{
			if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
			{
				BTN_ConfigSpawnGroupApply.Visible = true;
			}
		}

		#region Prefabs

		private void LBX_SpawnGroupConfig_Details_Prefabs_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentlySelecting = true;
			int index = LBX_SpawnGroupConfig_Details_Prefabs.SelectedIndex;

			SpawnGroupDefinition spawnGroup = _spawnGroupsDefinitionsWrapper.GetDefinitionOf(LBX_SpawnGroupConfiguration.SelectedIndex);
			SpawnGroupPrefab spawnGroupPrefab = spawnGroup.Prefabs[index];

			TBX_SpawnGroupConfig_Details_PrefabFile.Text = spawnGroupPrefab.File;
			TBX_SpawnGroupConfig_Details_PrefabPosition.Text = spawnGroupPrefab.Position.ToString();
			TBX_SpawnGroupConfig_Details_PrefabBeaconText.Text = spawnGroupPrefab.BeaconText;
			TBX_SpawnGroupConfig_Details_PrefabSpeed.Text = spawnGroupPrefab.Speed.ToString();

			_currentlySelecting = false;
		}

		#endregion

		#endregion

		#endregion
	}
}
