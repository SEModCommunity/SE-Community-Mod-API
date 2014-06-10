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

        #endregion

        #region Form events

        private void SEConfigTool_Load(object sender, EventArgs e)
        {
            _serializer = new ConfigFileSerializer();
            _standardSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            FillBlocksConfigurationListBox();
            FillAmmoConfigurationListBox();
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
    }
    #endregion
}
