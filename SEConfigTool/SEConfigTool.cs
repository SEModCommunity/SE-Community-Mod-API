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

            TBX_ConfigBlockName.Text = _cubeBlockDefinitionsWrapper.NameOf(index);
            TBX_ConfigBlockId.Text = _cubeBlockDefinitionsWrapper.IdOf(index);
            TBX_ConfigBuildTime.Text = _cubeBlockDefinitionsWrapper.BuildTimeOf(index).ToString(_numberFormatInfo);
            TBX_ConfigDisassembleRatio.Text = _cubeBlockDefinitionsWrapper.DisassembleRatioOf(index).ToString(_numberFormatInfo);
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
            _serializer.SetCubeBlockDefinitions(_cubeBlockDefinitionsWrapper.Definitions);
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
            _cubeBlockDefinitionsWrapper.SetBuildTimeOf(index, Convert.ToSingle(TBX_ConfigBuildTime.Text, _numberFormatInfo));
            _cubeBlockDefinitionsWrapper.SetDisassembleRatioOf(index, Convert.ToSingle(TBX_ConfigDisassembleRatio.Text, _numberFormatInfo));
            BTN_ConfigApplyChanges.Visible = false;
        }

        #endregion

        #region AmmoMagazines

        private void LBX_AmmoConfiguration_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentlySelecting = true;
            int index = LBX_AmmoConfiguration.SelectedIndex;

            TBX_ConfigAmmoName.Text = _ammoMagazinesDefinitionsWrapper.NameOf(index);
            TBX_ConfigAmmoId.Text = _ammoMagazinesDefinitionsWrapper.IdOf(index);
            TBX_ConfigAmmoCaliber.Text = _ammoMagazinesDefinitionsWrapper.CaliberOf(index);
            TBX_ConfigAmmoCapacity.Text = _ammoMagazinesDefinitionsWrapper.CapacityOf(index).ToString(_numberFormatInfo);
            TBX_ConfigAmmoVolume.Text = _ammoMagazinesDefinitionsWrapper.VolumeOf(index).ToString(_numberFormatInfo);
            TBX_ConfigAmmoMass.Text = _ammoMagazinesDefinitionsWrapper.MassOf(index).ToString(_numberFormatInfo);
            _currentlySelecting = false;
        }

        private void BTN_ConfigAmmoReload_Click(object sender, EventArgs e)
        {
            FillAmmoConfigurationListBox();
        }

        private void BTN_SaveAmmoConfig_Click(object sender, EventArgs e)
        {
            if (!_ammoMagazinesDefinitionsWrapper.Changed) return;
            _serializer.SetAmmoMagazinesDefinitions(_ammoMagazinesDefinitionsWrapper.Definitions);
            _serializer.SaveAmmoMagazinesContentFile();
        }

        private void BTN_ConfigAmmoApply_Click(object sender, EventArgs e)
        {
            int index = LBX_AmmoConfiguration.SelectedIndex;
            _ammoMagazinesDefinitionsWrapper.SetCapacityOf(index, Convert.ToInt32(TBX_ConfigAmmoCapacity.Text, _numberFormatInfo));
            _ammoMagazinesDefinitionsWrapper.SetMassOf(index, Convert.ToSingle(TBX_ConfigAmmoMass.Text, _numberFormatInfo));
            _ammoMagazinesDefinitionsWrapper.SetMassOf(index, Convert.ToSingle(TBX_ConfigAmmoVolume.Text, _numberFormatInfo));
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

			TBX_ConfigContainerTypeName.Text = _containerTypesDefinitionsWrapper.NameOf(index);
			TBX_ConfigContainerTypeId.Text = _containerTypesDefinitionsWrapper.IdOf(index);
			TBX_ConfigContainerTypeItemCount.Text = _containerTypesDefinitionsWrapper.ItemCountOf(index).ToString();
			TBX_ConfigContainerTypeCountMax.Text = _containerTypesDefinitionsWrapper.CountMaxOf(index).ToString();
			TBX_ConfigContainerTypeCountMin.Text = _containerTypesDefinitionsWrapper.CountMinOf(index).ToString();

			_currentlySelecting = false;
		}

		private void BTN_ConfigContainerTypeReload_Click(object sender, EventArgs e)
		{
			FillContainerTypeConfigurationListBox();
		}

		private void BTN_SaveContainerTypeConfig_Click(object sender, EventArgs e)
		{
			if (!_containerTypesDefinitionsWrapper.Changed) return;
			_serializer.SetContainerTypesDefinitions(_containerTypesDefinitionsWrapper.Definitions);
			_serializer.SaveContainerTypesContentFile();
		}

		private void BTN_ConfigContainerTypeApply_Click(object sender, EventArgs e)
		{
			int index = LBX_ContainerTypeConfiguration.SelectedIndex;
			_containerTypesDefinitionsWrapper.SetCountMaxOf(index, Convert.ToInt32(TBX_ConfigContainerTypeCountMax.Text, _numberFormatInfo));
			_containerTypesDefinitionsWrapper.SetCountMinOf(index, Convert.ToInt32(TBX_ConfigContainerTypeCountMin.Text, _numberFormatInfo));
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

		#endregion

		#region GlobalEvents

		private void LBX_GlobalEventConfiguration_SelectedIndexChanged(object sender, EventArgs e)
		{
			_currentlySelecting = true;
			int index = LBX_GlobalEventConfiguration.SelectedIndex;

			TBX_ConfigGlobalEventId.Text = _globalEventsDefinitionsWrapper.IdOf(index);
			TBX_ConfigGlobalEventName.Text = _globalEventsDefinitionsWrapper.NameOf(index);
			TBX_ConfigGlobalEventDescription.Text = _globalEventsDefinitionsWrapper.DescriptionOf(index);
			TBX_ConfigGlobalEventType.Text = _globalEventsDefinitionsWrapper.TypeOf(index);
			TBX_ConfigGlobalEventMinActivation.Text = _globalEventsDefinitionsWrapper.MinActivationOf(index).ToString();
			TBX_ConfigGlobalEventMaxActivation.Text = _globalEventsDefinitionsWrapper.MaxActivationOf(index).ToString();
			TBX_ConfigGlobalEventFirstActivation.Text = _globalEventsDefinitionsWrapper.FirstActivationOf(index).ToString();

			_currentlySelecting = false;
		}

		private void BTN_ConfigGlobalEventReload_Click(object sender, EventArgs e)
		{
			FillGlobalEventConfigurationListBox();
		}

		private void BTN_SaveGlobalEventConfig_Click(object sender, EventArgs e)
		{
			if (!_globalEventsDefinitionsWrapper.Changed) return;
			_serializer.SetGlobalEventsDefinitions(_globalEventsDefinitionsWrapper.Definitions);
			_serializer.SaveGlobalEventsContentFile();
		}

		private void BTN_ConfigGlobalEventApply_Click(object sender, EventArgs e)
		{
			int index = LBX_GlobalEventConfiguration.SelectedIndex;
			_globalEventsDefinitionsWrapper.SetMinActivationOf(index, Convert.ToInt32(TBX_ConfigGlobalEventMinActivation.Text, _numberFormatInfo));
			_globalEventsDefinitionsWrapper.SetMaxActivationOf(index, Convert.ToInt32(TBX_ConfigGlobalEventMaxActivation.Text, _numberFormatInfo));
			_globalEventsDefinitionsWrapper.SetFirstActivationOf(index, Convert.ToInt32(TBX_ConfigGlobalEventFirstActivation.Text, _numberFormatInfo));
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

			TBX_ConfigSpawnGroupName.Text = _spawnGroupsDefinitionsWrapper.NameOf(index);
			TBX_ConfigSpawnGroupPrefabCount.Text = _spawnGroupsDefinitionsWrapper.PrefabCountOf(index).ToString();
			TBX_ConfigSpawnGroupFrequency.Text = _spawnGroupsDefinitionsWrapper.FrequencyOf(index).ToString();

			_currentlySelecting = false;
		}

		private void BTN_ConfigSpawnGroupReload_Click(object sender, EventArgs e)
		{
			FillSpawnGroupConfigurationListBox();
		}

		private void BTN_SaveSpawnGroupConfig_Click(object sender, EventArgs e)
		{
			if (!_spawnGroupsDefinitionsWrapper.Changed) return;
			_serializer.SetSpawnGroupsDefinitions(_spawnGroupsDefinitionsWrapper.Definitions);
			_serializer.SaveSpawnGroupsContentFile();
		}

		private void BTN_ConfigSpawnGroupApply_Click(object sender, EventArgs e)
		{
			int index = LBX_SpawnGroupConfiguration.SelectedIndex;
			_spawnGroupsDefinitionsWrapper.SetFrequencyOf(index, Convert.ToSingle(TBX_ConfigSpawnGroupFrequency.Text, _numberFormatInfo));
			BTN_ConfigSpawnGroupApply.Visible = false;
		}

		private void TBX_ConfigSpawnGroupFrequency_TextChanged(object sender, EventArgs e)
		{
			if (!_currentlyFillingConfigurationListBox && !_currentlySelecting)
			{
				BTN_ConfigSpawnGroupApply.Visible = true;
			}
		}

		#endregion

		#endregion
	}
}
