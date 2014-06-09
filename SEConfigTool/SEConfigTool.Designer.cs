using SEModAPI.Support;

namespace SEConfigTool
{
    partial class SEConfigTool
    {
        /// <summary>
        /// Needed member to designer.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Ressources cleanup
        /// </summary>
        /// <param name="disposing">true if every managed ressources need to be deleeted ; else, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Needed method for designer support - do not modofy manually.
        /// </summary>
        private void InitializeComponent()
        {
            this.LayoutPages = new System.Windows.Forms.TabControl();
            this.SaveGamePage = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.BTN_LoadSaveGame = new System.Windows.Forms.Button();
            this.GBX_Blocks = new System.Windows.Forms.GroupBox();
            this.LBX_SaveGameBlockList = new System.Windows.Forms.ListBox();
            this.BlocksConfigurationPage = new System.Windows.Forms.TabPage();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.BTN_ConfigReload = new System.Windows.Forms.Button();
            this.BTN_SaveBlocksConfiguration = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.LBX_BlocksConfiguration = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.splitContainer4 = new System.Windows.Forms.SplitContainer();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.BTN_ConfigApplyChanges = new System.Windows.Forms.Button();
            this.TBX_ConfigBlockId = new System.Windows.Forms.TextBox();
            this.TBX_ConfigDisassembleRatio = new System.Windows.Forms.TextBox();
            this.TBX_ConfigBlockName = new System.Windows.Forms.TextBox();
            this.TBX_ConfigBuildTime = new System.Windows.Forms.TextBox();
            this.LayoutPages.SuspendLayout();
            this.SaveGamePage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.GBX_Blocks.SuspendLayout();
            this.BlocksConfigurationPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
            this.splitContainer4.Panel1.SuspendLayout();
            this.splitContainer4.Panel2.SuspendLayout();
            this.splitContainer4.SuspendLayout();
            this.SuspendLayout();
            // 
            // LayoutPages
            // 
            this.LayoutPages.Controls.Add(this.SaveGamePage);
            this.LayoutPages.Controls.Add(this.BlocksConfigurationPage);
            this.LayoutPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayoutPages.Location = new System.Drawing.Point(0, 0);
            this.LayoutPages.Name = "LayoutPages";
            this.LayoutPages.SelectedIndex = 0;
            this.LayoutPages.Size = new System.Drawing.Size(560, 435);
            this.LayoutPages.TabIndex = 0;
            // 
            // SaveGamePage
            // 
            this.SaveGamePage.Controls.Add(this.splitContainer1);
            this.SaveGamePage.Location = new System.Drawing.Point(4, 22);
            this.SaveGamePage.Name = "SaveGamePage";
            this.SaveGamePage.Padding = new System.Windows.Forms.Padding(3);
            this.SaveGamePage.Size = new System.Drawing.Size(552, 409);
            this.SaveGamePage.TabIndex = 0;
            this.SaveGamePage.Text = "SaveGame";
            this.SaveGamePage.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.BTN_LoadSaveGame);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.GBX_Blocks);
            this.splitContainer1.Size = new System.Drawing.Size(546, 403);
            this.splitContainer1.SplitterDistance = 26;
            this.splitContainer1.TabIndex = 1;
            // 
            // BTN_LoadSaveGame
            // 
            this.BTN_LoadSaveGame.Location = new System.Drawing.Point(4, 1);
            this.BTN_LoadSaveGame.Name = "BTN_LoadSaveGame";
            this.BTN_LoadSaveGame.Size = new System.Drawing.Size(100, 23);
            this.BTN_LoadSaveGame.TabIndex = 0;
            this.BTN_LoadSaveGame.Text = "Load SaveGame";
            this.BTN_LoadSaveGame.UseVisualStyleBackColor = true;
            this.BTN_LoadSaveGame.Click += new System.EventHandler(this.BTN_LoadSaveGame_Click);
            // 
            // GBX_Blocks
            // 
            this.GBX_Blocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GBX_Blocks.Controls.Add(this.LBX_SaveGameBlockList);
            this.GBX_Blocks.Location = new System.Drawing.Point(0, 0);
            this.GBX_Blocks.Name = "GBX_Blocks";
            this.GBX_Blocks.Size = new System.Drawing.Size(538, 395);
            this.GBX_Blocks.TabIndex = 1;
            this.GBX_Blocks.TabStop = false;
            this.GBX_Blocks.Text = "Blocks";
            // 
            // LBX_SaveGameBlockList
            // 
            this.LBX_SaveGameBlockList.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.LBX_SaveGameBlockList.FormattingEnabled = true;
            this.LBX_SaveGameBlockList.Location = new System.Drawing.Point(3, 24);
            this.LBX_SaveGameBlockList.Name = "LBX_SaveGameBlockList";
            this.LBX_SaveGameBlockList.Size = new System.Drawing.Size(532, 368);
            this.LBX_SaveGameBlockList.TabIndex = 0;
            // 
            // BlocksConfigurationPage
            // 
            this.BlocksConfigurationPage.Controls.Add(this.splitContainer3);
            this.BlocksConfigurationPage.Location = new System.Drawing.Point(4, 22);
            this.BlocksConfigurationPage.Name = "BlocksConfigurationPage";
            this.BlocksConfigurationPage.Padding = new System.Windows.Forms.Padding(3);
            this.BlocksConfigurationPage.Size = new System.Drawing.Size(552, 409);
            this.BlocksConfigurationPage.TabIndex = 1;
            this.BlocksConfigurationPage.Text = "Blocks Configuration";
            this.BlocksConfigurationPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer3.Location = new System.Drawing.Point(3, 3);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.BTN_ConfigReload);
            this.splitContainer3.Panel1.Controls.Add(this.BTN_SaveBlocksConfiguration);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer3.Size = new System.Drawing.Size(546, 403);
            this.splitContainer3.SplitterDistance = 25;
            this.splitContainer3.TabIndex = 1;
            // 
            // BTN_ConfigReload
            // 
            this.BTN_ConfigReload.Location = new System.Drawing.Point(3, 0);
            this.BTN_ConfigReload.Name = "BTN_ConfigReload";
            this.BTN_ConfigReload.Size = new System.Drawing.Size(116, 23);
            this.BTN_ConfigReload.TabIndex = 2;
            this.BTN_ConfigReload.Text = "Reload Configuration";
            this.BTN_ConfigReload.UseVisualStyleBackColor = true;
            this.BTN_ConfigReload.Click += new System.EventHandler(this.BTN_ConfigReload_Click);
            // 
            // BTN_SaveBlocksConfiguration
            // 
            this.BTN_SaveBlocksConfiguration.Location = new System.Drawing.Point(122, 0);
            this.BTN_SaveBlocksConfiguration.Name = "BTN_SaveBlocksConfiguration";
            this.BTN_SaveBlocksConfiguration.Size = new System.Drawing.Size(105, 23);
            this.BTN_SaveBlocksConfiguration.TabIndex = 0;
            this.BTN_SaveBlocksConfiguration.Text = "Save Configuration";
            this.BTN_SaveBlocksConfiguration.UseVisualStyleBackColor = true;
            this.BTN_SaveBlocksConfiguration.Click += new System.EventHandler(this.BTN_SaveBlocksConfiguration_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.splitContainer2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(546, 374);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Blocks";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 16);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.LBX_BlocksConfiguration);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(540, 355);
            this.splitContainer2.SplitterDistance = 239;
            this.splitContainer2.TabIndex = 1;
            // 
            // LBX_BlocksConfiguration
            // 
            this.LBX_BlocksConfiguration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LBX_BlocksConfiguration.FormattingEnabled = true;
            this.LBX_BlocksConfiguration.Location = new System.Drawing.Point(0, 0);
            this.LBX_BlocksConfiguration.Name = "LBX_BlocksConfiguration";
            this.LBX_BlocksConfiguration.Size = new System.Drawing.Size(239, 355);
            this.LBX_BlocksConfiguration.TabIndex = 0;
            this.LBX_BlocksConfiguration.SelectedIndexChanged += new System.EventHandler(this.LBX_BlocksConfiguration_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.splitContainer4);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(297, 355);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selected Block Informations";
            // 
            // splitContainer4
            // 
            this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer4.Location = new System.Drawing.Point(3, 16);
            this.splitContainer4.Name = "splitContainer4";
            // 
            // splitContainer4.Panel1
            // 
            this.splitContainer4.Panel1.Controls.Add(this.label4);
            this.splitContainer4.Panel1.Controls.Add(this.label3);
            this.splitContainer4.Panel1.Controls.Add(this.label1);
            this.splitContainer4.Panel1.Controls.Add(this.label2);
            // 
            // splitContainer4.Panel2
            // 
            this.splitContainer4.Panel2.Controls.Add(this.BTN_ConfigApplyChanges);
            this.splitContainer4.Panel2.Controls.Add(this.TBX_ConfigBlockId);
            this.splitContainer4.Panel2.Controls.Add(this.TBX_ConfigDisassembleRatio);
            this.splitContainer4.Panel2.Controls.Add(this.TBX_ConfigBlockName);
            this.splitContainer4.Panel2.Controls.Add(this.TBX_ConfigBuildTime);
            this.splitContainer4.Size = new System.Drawing.Size(291, 336);
            this.splitContainer4.SplitterDistance = 136;
            this.splitContainer4.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 41);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Block Id Number:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 93);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Block Disassemble Ratio:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Block Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Block Build Time:";
            // 
            // BTN_ConfigApplyChanges
            // 
            this.BTN_ConfigApplyChanges.Location = new System.Drawing.Point(3, 310);
            this.BTN_ConfigApplyChanges.Name = "BTN_ConfigApplyChanges";
            this.BTN_ConfigApplyChanges.Size = new System.Drawing.Size(145, 23);
            this.BTN_ConfigApplyChanges.TabIndex = 8;
            this.BTN_ConfigApplyChanges.Text = "Apply changes";
            this.BTN_ConfigApplyChanges.UseVisualStyleBackColor = true;
            this.BTN_ConfigApplyChanges.Visible = false;
            this.BTN_ConfigApplyChanges.Click += new System.EventHandler(this.BTN_ConfigApplyChanges_Click);
            // 
            // TBX_ConfigBlockId
            // 
            this.TBX_ConfigBlockId.Location = new System.Drawing.Point(3, 38);
            this.TBX_ConfigBlockId.Name = "TBX_ConfigBlockId";
            this.TBX_ConfigBlockId.ReadOnly = true;
            this.TBX_ConfigBlockId.Size = new System.Drawing.Size(145, 20);
            this.TBX_ConfigBlockId.TabIndex = 7;
            // 
            // TBX_ConfigDisassembleRatio
            // 
            this.TBX_ConfigDisassembleRatio.Location = new System.Drawing.Point(3, 90);
            this.TBX_ConfigDisassembleRatio.Name = "TBX_ConfigDisassembleRatio";
            this.TBX_ConfigDisassembleRatio.Size = new System.Drawing.Size(145, 20);
            this.TBX_ConfigDisassembleRatio.TabIndex = 6;
            this.TBX_ConfigDisassembleRatio.TextChanged += new System.EventHandler(this.TBX_ConfigBuildTime_TextChanged);
            this.TBX_ConfigDisassembleRatio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBX_ConfigBuildTime_KeyPress);
            // 
            // TBX_ConfigBlockName
            // 
            this.TBX_ConfigBlockName.Location = new System.Drawing.Point(3, 13);
            this.TBX_ConfigBlockName.Name = "TBX_ConfigBlockName";
            this.TBX_ConfigBlockName.ReadOnly = true;
            this.TBX_ConfigBlockName.Size = new System.Drawing.Size(145, 20);
            this.TBX_ConfigBlockName.TabIndex = 5;
            // 
            // TBX_ConfigBuildTime
            // 
            this.TBX_ConfigBuildTime.Location = new System.Drawing.Point(3, 64);
            this.TBX_ConfigBuildTime.Name = "TBX_ConfigBuildTime";
            this.TBX_ConfigBuildTime.Size = new System.Drawing.Size(145, 20);
            this.TBX_ConfigBuildTime.TabIndex = 4;
            this.TBX_ConfigBuildTime.TextChanged += new System.EventHandler(this.TBX_ConfigBuildTime_TextChanged);
            this.TBX_ConfigBuildTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TBX_ConfigBuildTime_KeyPress);
            // 
            // SEConfigTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 435);
            this.Controls.Add(this.LayoutPages);
            this.Name = "SEConfigTool";
            this.Text = "SEConfigTool";
            this.Load += new System.EventHandler(this.SEConfigTool_Load);
            this.LayoutPages.ResumeLayout(false);
            this.SaveGamePage.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.GBX_Blocks.ResumeLayout(false);
            this.BlocksConfigurationPage.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.splitContainer4.Panel1.ResumeLayout(false);
            this.splitContainer4.Panel1.PerformLayout();
            this.splitContainer4.Panel2.ResumeLayout(false);
            this.splitContainer4.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
            this.splitContainer4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl LayoutPages;
        private System.Windows.Forms.TabPage SaveGamePage;
        private System.Windows.Forms.TabPage BlocksConfigurationPage;
        private System.Windows.Forms.GroupBox GBX_Blocks;
        private System.Windows.Forms.ListBox LBX_SaveGameBlockList;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button BTN_LoadSaveGame;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.Button BTN_SaveBlocksConfiguration;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.ListBox LBX_BlocksConfiguration;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitContainer4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TBX_ConfigDisassembleRatio;
        private System.Windows.Forms.TextBox TBX_ConfigBlockName;
        private System.Windows.Forms.TextBox TBX_ConfigBuildTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TBX_ConfigBlockId;
        private System.Windows.Forms.Button BTN_ConfigReload;
        private System.Windows.Forms.Button BTN_ConfigApplyChanges;
    }
}

