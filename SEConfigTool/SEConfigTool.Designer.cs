namespace SEConfigTool
{
    partial class SEConfigTool
	{
		#region "Attributes"

		private System.Windows.Forms.TabControl LayoutPages;
		private System.Windows.Forms.TabPage ConfigurationPage;
		private System.Windows.Forms.TabPage Temp;
		private System.Windows.Forms.GroupBox GBX_Blocks;
		private System.Windows.Forms.ListBox BlockList;
		private System.Windows.Forms.GroupBox GBX_SpawnGroups;
		private System.Windows.Forms.ListBox SpawnGroupList;
		private System.Windows.Forms.GroupBox GBX_GlobalEvents;
		private System.Windows.Forms.ListBox GlobalEventList;

		private System.ComponentModel.IContainer components = null;

		#endregion

		#region "Methods"

		protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
			this.LayoutPages = new System.Windows.Forms.TabControl();
			this.ConfigurationPage = new System.Windows.Forms.TabPage();
			this.GBX_GlobalEvents = new System.Windows.Forms.GroupBox();
			this.GlobalEventList = new System.Windows.Forms.ListBox();
			this.GBX_SpawnGroups = new System.Windows.Forms.GroupBox();
			this.SpawnGroupList = new System.Windows.Forms.ListBox();
			this.GBX_Blocks = new System.Windows.Forms.GroupBox();
			this.BlockList = new System.Windows.Forms.ListBox();
			this.Temp = new System.Windows.Forms.TabPage();
			this.LayoutPages.SuspendLayout();
			this.ConfigurationPage.SuspendLayout();
			this.GBX_GlobalEvents.SuspendLayout();
			this.GBX_SpawnGroups.SuspendLayout();
			this.GBX_Blocks.SuspendLayout();
			this.SuspendLayout();
			// 
			// LayoutPages
			// 
			this.LayoutPages.Controls.Add(this.ConfigurationPage);
			this.LayoutPages.Controls.Add(this.Temp);
			this.LayoutPages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LayoutPages.Location = new System.Drawing.Point(0, 0);
			this.LayoutPages.Name = "LayoutPages";
			this.LayoutPages.SelectedIndex = 0;
			this.LayoutPages.Size = new System.Drawing.Size(594, 601);
			this.LayoutPages.TabIndex = 0;
			// 
			// ConfigurationPage
			// 
			this.ConfigurationPage.Controls.Add(this.GBX_GlobalEvents);
			this.ConfigurationPage.Controls.Add(this.GBX_SpawnGroups);
			this.ConfigurationPage.Controls.Add(this.GBX_Blocks);
			this.ConfigurationPage.Location = new System.Drawing.Point(4, 22);
			this.ConfigurationPage.Name = "ConfigurationPage";
			this.ConfigurationPage.Padding = new System.Windows.Forms.Padding(3);
			this.ConfigurationPage.Size = new System.Drawing.Size(586, 575);
			this.ConfigurationPage.TabIndex = 0;
			this.ConfigurationPage.Text = "Configuration";
			this.ConfigurationPage.UseVisualStyleBackColor = true;
			// 
			// GBX_GlobalEvents
			// 
			this.GBX_GlobalEvents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GBX_GlobalEvents.Controls.Add(this.GlobalEventList);
			this.GBX_GlobalEvents.Location = new System.Drawing.Point(6, 406);
			this.GBX_GlobalEvents.Name = "GBX_GlobalEvents";
			this.GBX_GlobalEvents.Size = new System.Drawing.Size(572, 200);
			this.GBX_GlobalEvents.TabIndex = 3;
			this.GBX_GlobalEvents.TabStop = false;
			this.GBX_GlobalEvents.Text = "Global Events";
			// 
			// GlobalEventList
			// 
			this.GlobalEventList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GlobalEventList.FormattingEnabled = true;
			this.GlobalEventList.Location = new System.Drawing.Point(6, 25);
			this.GlobalEventList.Name = "GlobalEventList";
			this.GlobalEventList.Size = new System.Drawing.Size(560, 160);
			this.GlobalEventList.TabIndex = 0;
			// 
			// GBX_SpawnGroups
			// 
			this.GBX_SpawnGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GBX_SpawnGroups.Controls.Add(this.SpawnGroupList);
			this.GBX_SpawnGroups.Location = new System.Drawing.Point(6, 206);
			this.GBX_SpawnGroups.Name = "GBX_SpawnGroups";
			this.GBX_SpawnGroups.Size = new System.Drawing.Size(572, 200);
			this.GBX_SpawnGroups.TabIndex = 2;
			this.GBX_SpawnGroups.TabStop = false;
			this.GBX_SpawnGroups.Text = "Spawn Groups";
			// 
			// SpawnGroupList
			// 
			this.SpawnGroupList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.SpawnGroupList.FormattingEnabled = true;
			this.SpawnGroupList.Location = new System.Drawing.Point(6, 25);
			this.SpawnGroupList.Name = "SpawnGroupList";
			this.SpawnGroupList.Size = new System.Drawing.Size(560, 160);
			this.SpawnGroupList.TabIndex = 0;
			// 
			// GBX_Blocks
			// 
			this.GBX_Blocks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.GBX_Blocks.Controls.Add(this.BlockList);
			this.GBX_Blocks.Location = new System.Drawing.Point(6, 6);
			this.GBX_Blocks.Name = "GBX_Blocks";
			this.GBX_Blocks.Size = new System.Drawing.Size(572, 200);
			this.GBX_Blocks.TabIndex = 1;
			this.GBX_Blocks.TabStop = false;
			this.GBX_Blocks.Text = "Blocks";
			// 
			// BlockList
			// 
			this.BlockList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.BlockList.FormattingEnabled = true;
			this.BlockList.Location = new System.Drawing.Point(6, 25);
			this.BlockList.Name = "BlockList";
			this.BlockList.Size = new System.Drawing.Size(560, 160);
			this.BlockList.TabIndex = 0;
			// 
			// Temp
			// 
			this.Temp.Location = new System.Drawing.Point(4, 22);
			this.Temp.Name = "Temp";
			this.Temp.Padding = new System.Windows.Forms.Padding(3);
			this.Temp.Size = new System.Drawing.Size(552, 409);
			this.Temp.TabIndex = 1;
			this.Temp.Text = "Test page";
			this.Temp.UseVisualStyleBackColor = true;
			// 
			// SEConfigTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(594, 601);
			this.Controls.Add(this.LayoutPages);
			this.Name = "SEConfigTool";
			this.Text = "SEConfigTool";
			this.Load += new System.EventHandler(this.SEConfigTool_Load);
			this.LayoutPages.ResumeLayout(false);
			this.ConfigurationPage.ResumeLayout(false);
			this.GBX_GlobalEvents.ResumeLayout(false);
			this.GBX_SpawnGroups.ResumeLayout(false);
			this.GBX_Blocks.ResumeLayout(false);
			this.ResumeLayout(false);

		}

        #endregion
    }
}

