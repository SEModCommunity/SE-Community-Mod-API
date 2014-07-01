namespace SEServerExtender
{
	partial class SEServerExtender
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.TAB_MainTabs = new System.Windows.Forms.TabControl();
			this.TAB_Control_Page = new System.Windows.Forms.TabPage();
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.BTN_ServerControl_Start = new System.Windows.Forms.Button();
			this.BTN_ServerControl_Stop = new System.Windows.Forms.Button();
			this.splitContainer4 = new System.Windows.Forms.SplitContainer();
			this.LBL_Control_EnableFactions = new System.Windows.Forms.Label();
			this.LBL_Control_Debugging = new System.Windows.Forms.Label();
			this.CHK_Control_EnableFactions = new System.Windows.Forms.CheckBox();
			this.CHK_Control_Debugging = new System.Windows.Forms.CheckBox();
			this.TAB_Entities_Page = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.TRV_Entities = new System.Windows.Forms.TreeView();
			this.BTN_Entities_Export = new System.Windows.Forms.Button();
			this.BTN_Entities_New = new System.Windows.Forms.Button();
			this.BTN_Entities_Delete = new System.Windows.Forms.Button();
			this.PG_Entities_Details = new System.Windows.Forms.PropertyGrid();
			this.TAB_Chat_Page = new System.Windows.Forms.TabPage();
			this.splitContainer6 = new System.Windows.Forms.SplitContainer();
			this.splitContainer8 = new System.Windows.Forms.SplitContainer();
			this.LST_Chat_Messages = new System.Windows.Forms.ListBox();
			this.LST_Chat_ConnectedPlayers = new System.Windows.Forms.ListBox();
			this.splitContainer7 = new System.Windows.Forms.SplitContainer();
			this.TXT_Chat_Message = new System.Windows.Forms.TextBox();
			this.BTN_Chat_Send = new System.Windows.Forms.Button();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.TAB_MainTabs.SuspendLayout();
			this.TAB_Control_Page.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.Panel2.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).BeginInit();
			this.splitContainer4.Panel1.SuspendLayout();
			this.splitContainer4.Panel2.SuspendLayout();
			this.splitContainer4.SuspendLayout();
			this.TAB_Entities_Page.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
			this.TAB_Chat_Page.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).BeginInit();
			this.splitContainer6.Panel1.SuspendLayout();
			this.splitContainer6.Panel2.SuspendLayout();
			this.splitContainer6.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).BeginInit();
			this.splitContainer8.Panel1.SuspendLayout();
			this.splitContainer8.Panel2.SuspendLayout();
			this.splitContainer8.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).BeginInit();
			this.splitContainer7.Panel1.SuspendLayout();
			this.splitContainer7.Panel2.SuspendLayout();
			this.splitContainer7.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.TAB_MainTabs);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.statusStrip1);
			this.splitContainer1.Size = new System.Drawing.Size(951, 699);
			this.splitContainer1.SplitterDistance = 670;
			this.splitContainer1.TabIndex = 0;
			// 
			// TAB_MainTabs
			// 
			this.TAB_MainTabs.Controls.Add(this.TAB_Control_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Entities_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Chat_Page);
			this.TAB_MainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TAB_MainTabs.Location = new System.Drawing.Point(0, 0);
			this.TAB_MainTabs.Name = "TAB_MainTabs";
			this.TAB_MainTabs.SelectedIndex = 0;
			this.TAB_MainTabs.Size = new System.Drawing.Size(951, 670);
			this.TAB_MainTabs.TabIndex = 0;
			// 
			// TAB_Control_Page
			// 
			this.TAB_Control_Page.Controls.Add(this.splitContainer3);
			this.TAB_Control_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Control_Page.Name = "TAB_Control_Page";
			this.TAB_Control_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Control_Page.Size = new System.Drawing.Size(943, 644);
			this.TAB_Control_Page.TabIndex = 0;
			this.TAB_Control_Page.Text = "Control";
			this.TAB_Control_Page.UseVisualStyleBackColor = true;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.Location = new System.Drawing.Point(3, 3);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.BTN_ServerControl_Start);
			this.splitContainer3.Panel1.Controls.Add(this.BTN_ServerControl_Stop);
			// 
			// splitContainer3.Panel2
			// 
			this.splitContainer3.Panel2.Controls.Add(this.splitContainer4);
			this.splitContainer3.Size = new System.Drawing.Size(937, 638);
			this.splitContainer3.SplitterDistance = 455;
			this.splitContainer3.TabIndex = 4;
			// 
			// BTN_ServerControl_Start
			// 
			this.BTN_ServerControl_Start.Location = new System.Drawing.Point(5, 6);
			this.BTN_ServerControl_Start.Name = "BTN_ServerControl_Start";
			this.BTN_ServerControl_Start.Size = new System.Drawing.Size(75, 23);
			this.BTN_ServerControl_Start.TabIndex = 0;
			this.BTN_ServerControl_Start.Text = "Start Server";
			this.BTN_ServerControl_Start.UseVisualStyleBackColor = true;
			this.BTN_ServerControl_Start.Click += new System.EventHandler(this.BTN_ServerControl_Start_Click);
			// 
			// BTN_ServerControl_Stop
			// 
			this.BTN_ServerControl_Stop.Location = new System.Drawing.Point(5, 35);
			this.BTN_ServerControl_Stop.Name = "BTN_ServerControl_Stop";
			this.BTN_ServerControl_Stop.Size = new System.Drawing.Size(75, 23);
			this.BTN_ServerControl_Stop.TabIndex = 1;
			this.BTN_ServerControl_Stop.Text = "Stop Server";
			this.BTN_ServerControl_Stop.UseVisualStyleBackColor = true;
			this.BTN_ServerControl_Stop.Click += new System.EventHandler(this.BTN_ServerControl_Stop_Click);
			// 
			// splitContainer4
			// 
			this.splitContainer4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer4.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer4.Location = new System.Drawing.Point(0, 0);
			this.splitContainer4.Name = "splitContainer4";
			// 
			// splitContainer4.Panel1
			// 
			this.splitContainer4.Panel1.Controls.Add(this.LBL_Control_EnableFactions);
			this.splitContainer4.Panel1.Controls.Add(this.LBL_Control_Debugging);
			this.splitContainer4.Panel1MinSize = 120;
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.CHK_Control_EnableFactions);
			this.splitContainer4.Panel2.Controls.Add(this.CHK_Control_Debugging);
			this.splitContainer4.Size = new System.Drawing.Size(478, 638);
			this.splitContainer4.SplitterDistance = 120;
			this.splitContainer4.TabIndex = 0;
			// 
			// LBL_Control_EnableFactions
			// 
			this.LBL_Control_EnableFactions.AutoSize = true;
			this.LBL_Control_EnableFactions.Location = new System.Drawing.Point(3, 26);
			this.LBL_Control_EnableFactions.Name = "LBL_Control_EnableFactions";
			this.LBL_Control_EnableFactions.Size = new System.Drawing.Size(86, 13);
			this.LBL_Control_EnableFactions.TabIndex = 1;
			this.LBL_Control_EnableFactions.Text = "Enable Factions:";
			// 
			// LBL_Control_Debugging
			// 
			this.LBL_Control_Debugging.AutoSize = true;
			this.LBL_Control_Debugging.Location = new System.Drawing.Point(3, 4);
			this.LBL_Control_Debugging.Name = "LBL_Control_Debugging";
			this.LBL_Control_Debugging.Size = new System.Drawing.Size(98, 13);
			this.LBL_Control_Debugging.TabIndex = 0;
			this.LBL_Control_Debugging.Text = "Enable Debugging:";
			// 
			// CHK_Control_EnableFactions
			// 
			this.CHK_Control_EnableFactions.AutoSize = true;
			this.CHK_Control_EnableFactions.Location = new System.Drawing.Point(3, 26);
			this.CHK_Control_EnableFactions.Name = "CHK_Control_EnableFactions";
			this.CHK_Control_EnableFactions.Size = new System.Drawing.Size(15, 14);
			this.CHK_Control_EnableFactions.TabIndex = 1;
			this.CHK_Control_EnableFactions.UseVisualStyleBackColor = true;
			this.CHK_Control_EnableFactions.CheckedChanged += new System.EventHandler(this.CHK_Control_EnableFactions_CheckedChanged);
			// 
			// CHK_Control_Debugging
			// 
			this.CHK_Control_Debugging.AutoSize = true;
			this.CHK_Control_Debugging.Location = new System.Drawing.Point(3, 3);
			this.CHK_Control_Debugging.Name = "CHK_Control_Debugging";
			this.CHK_Control_Debugging.Size = new System.Drawing.Size(15, 14);
			this.CHK_Control_Debugging.TabIndex = 0;
			this.CHK_Control_Debugging.UseVisualStyleBackColor = true;
			this.CHK_Control_Debugging.CheckedChanged += new System.EventHandler(this.CHK_Control_Debugging_CheckedChanged);
			// 
			// TAB_Entities_Page
			// 
			this.TAB_Entities_Page.Controls.Add(this.splitContainer2);
			this.TAB_Entities_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Entities_Page.Name = "TAB_Entities_Page";
			this.TAB_Entities_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Entities_Page.Size = new System.Drawing.Size(943, 644);
			this.TAB_Entities_Page.TabIndex = 1;
			this.TAB_Entities_Page.Text = "Entities";
			this.TAB_Entities_Page.UseVisualStyleBackColor = true;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.Location = new System.Drawing.Point(3, 3);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer5);
			this.splitContainer2.Panel1MinSize = 300;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.PG_Entities_Details);
			this.splitContainer2.Size = new System.Drawing.Size(937, 638);
			this.splitContainer2.SplitterDistance = 300;
			this.splitContainer2.TabIndex = 0;
			// 
			// splitContainer5
			// 
			this.splitContainer5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer5.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer5.Location = new System.Drawing.Point(0, 0);
			this.splitContainer5.Name = "splitContainer5";
			this.splitContainer5.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer5.Panel1
			// 
			this.splitContainer5.Panel1.Controls.Add(this.TRV_Entities);
			// 
			// splitContainer5.Panel2
			// 
			this.splitContainer5.Panel2.Controls.Add(this.BTN_Entities_Export);
			this.splitContainer5.Panel2.Controls.Add(this.BTN_Entities_New);
			this.splitContainer5.Panel2.Controls.Add(this.BTN_Entities_Delete);
			this.splitContainer5.Size = new System.Drawing.Size(300, 638);
			this.splitContainer5.SplitterDistance = 603;
			this.splitContainer5.TabIndex = 0;
			// 
			// TRV_Entities
			// 
			this.TRV_Entities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TRV_Entities.Location = new System.Drawing.Point(0, 0);
			this.TRV_Entities.Name = "TRV_Entities";
			this.TRV_Entities.Size = new System.Drawing.Size(300, 603);
			this.TRV_Entities.TabIndex = 0;
			this.TRV_Entities.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TRV_Entities_NodeRefresh);
			this.TRV_Entities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TRV_Entities_AfterSelect);
			// 
			// BTN_Entities_Export
			// 
			this.BTN_Entities_Export.Enabled = false;
			this.BTN_Entities_Export.Location = new System.Drawing.Point(10, 5);
			this.BTN_Entities_Export.Name = "BTN_Entities_Export";
			this.BTN_Entities_Export.Size = new System.Drawing.Size(75, 23);
			this.BTN_Entities_Export.TabIndex = 2;
			this.BTN_Entities_Export.Text = "Export";
			this.BTN_Entities_Export.UseVisualStyleBackColor = true;
			this.BTN_Entities_Export.Click += new System.EventHandler(this.BTN_Entities_Export_Click);
			// 
			// BTN_Entities_New
			// 
			this.BTN_Entities_New.Enabled = false;
			this.BTN_Entities_New.Location = new System.Drawing.Point(91, 5);
			this.BTN_Entities_New.Name = "BTN_Entities_New";
			this.BTN_Entities_New.Size = new System.Drawing.Size(75, 23);
			this.BTN_Entities_New.TabIndex = 1;
			this.BTN_Entities_New.Text = "New";
			this.BTN_Entities_New.UseVisualStyleBackColor = true;
			this.BTN_Entities_New.Click += new System.EventHandler(this.BTN_Entities_New_Click);
			// 
			// BTN_Entities_Delete
			// 
			this.BTN_Entities_Delete.Enabled = false;
			this.BTN_Entities_Delete.Location = new System.Drawing.Point(172, 5);
			this.BTN_Entities_Delete.Name = "BTN_Entities_Delete";
			this.BTN_Entities_Delete.Size = new System.Drawing.Size(75, 23);
			this.BTN_Entities_Delete.TabIndex = 0;
			this.BTN_Entities_Delete.Text = "Delete";
			this.BTN_Entities_Delete.UseVisualStyleBackColor = true;
			this.BTN_Entities_Delete.Click += new System.EventHandler(this.BTN_Entities_Delete_Click);
			// 
			// PG_Entities_Details
			// 
			this.PG_Entities_Details.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PG_Entities_Details.Location = new System.Drawing.Point(0, 0);
			this.PG_Entities_Details.Name = "PG_Entities_Details";
			this.PG_Entities_Details.Size = new System.Drawing.Size(633, 638);
			this.PG_Entities_Details.TabIndex = 0;
			this.PG_Entities_Details.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PG_Entities_Details_Click);
			this.PG_Entities_Details.SelectedGridItemChanged += new System.Windows.Forms.SelectedGridItemChangedEventHandler(this.PG_Entities_Details_Click);
			this.PG_Entities_Details.Validated += new System.EventHandler(this.PG_Entities_Details_Click);
			// 
			// TAB_Chat_Page
			// 
			this.TAB_Chat_Page.Controls.Add(this.splitContainer6);
			this.TAB_Chat_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Chat_Page.Name = "TAB_Chat_Page";
			this.TAB_Chat_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Chat_Page.Size = new System.Drawing.Size(943, 644);
			this.TAB_Chat_Page.TabIndex = 2;
			this.TAB_Chat_Page.Text = "Chat";
			this.TAB_Chat_Page.UseVisualStyleBackColor = true;
			// 
			// splitContainer6
			// 
			this.splitContainer6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer6.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer6.Location = new System.Drawing.Point(3, 3);
			this.splitContainer6.Name = "splitContainer6";
			this.splitContainer6.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer6.Panel1
			// 
			this.splitContainer6.Panel1.Controls.Add(this.splitContainer8);
			// 
			// splitContainer6.Panel2
			// 
			this.splitContainer6.Panel2.Controls.Add(this.splitContainer7);
			this.splitContainer6.Size = new System.Drawing.Size(937, 638);
			this.splitContainer6.SplitterDistance = 605;
			this.splitContainer6.TabIndex = 4;
			// 
			// splitContainer8
			// 
			this.splitContainer8.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer8.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer8.Location = new System.Drawing.Point(0, 0);
			this.splitContainer8.Name = "splitContainer8";
			// 
			// splitContainer8.Panel1
			// 
			this.splitContainer8.Panel1.Controls.Add(this.LST_Chat_Messages);
			// 
			// splitContainer8.Panel2
			// 
			this.splitContainer8.Panel2.Controls.Add(this.LST_Chat_ConnectedPlayers);
			this.splitContainer8.Size = new System.Drawing.Size(937, 605);
			this.splitContainer8.SplitterDistance = 750;
			this.splitContainer8.TabIndex = 4;
			// 
			// LST_Chat_Messages
			// 
			this.LST_Chat_Messages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LST_Chat_Messages.FormattingEnabled = true;
			this.LST_Chat_Messages.Location = new System.Drawing.Point(0, 0);
			this.LST_Chat_Messages.Name = "LST_Chat_Messages";
			this.LST_Chat_Messages.ScrollAlwaysVisible = true;
			this.LST_Chat_Messages.Size = new System.Drawing.Size(750, 605);
			this.LST_Chat_Messages.TabIndex = 3;
			// 
			// LST_Chat_ConnectedPlayers
			// 
			this.LST_Chat_ConnectedPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LST_Chat_ConnectedPlayers.FormattingEnabled = true;
			this.LST_Chat_ConnectedPlayers.Location = new System.Drawing.Point(0, 0);
			this.LST_Chat_ConnectedPlayers.Name = "LST_Chat_ConnectedPlayers";
			this.LST_Chat_ConnectedPlayers.Size = new System.Drawing.Size(183, 605);
			this.LST_Chat_ConnectedPlayers.TabIndex = 0;
			// 
			// splitContainer7
			// 
			this.splitContainer7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer7.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer7.Location = new System.Drawing.Point(0, 0);
			this.splitContainer7.Name = "splitContainer7";
			// 
			// splitContainer7.Panel1
			// 
			this.splitContainer7.Panel1.Controls.Add(this.TXT_Chat_Message);
			// 
			// splitContainer7.Panel2
			// 
			this.splitContainer7.Panel2.Controls.Add(this.BTN_Chat_Send);
			this.splitContainer7.Size = new System.Drawing.Size(937, 29);
			this.splitContainer7.SplitterDistance = 800;
			this.splitContainer7.TabIndex = 2;
			// 
			// TXT_Chat_Message
			// 
			this.TXT_Chat_Message.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TXT_Chat_Message.Location = new System.Drawing.Point(0, 0);
			this.TXT_Chat_Message.Name = "TXT_Chat_Message";
			this.TXT_Chat_Message.Size = new System.Drawing.Size(800, 20);
			this.TXT_Chat_Message.TabIndex = 0;
			this.TXT_Chat_Message.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TXT_Chat_Message_KeyDown);
			// 
			// BTN_Chat_Send
			// 
			this.BTN_Chat_Send.Location = new System.Drawing.Point(53, 3);
			this.BTN_Chat_Send.Name = "BTN_Chat_Send";
			this.BTN_Chat_Send.Size = new System.Drawing.Size(75, 23);
			this.BTN_Chat_Send.TabIndex = 1;
			this.BTN_Chat_Send.Text = "Send";
			this.BTN_Chat_Send.UseVisualStyleBackColor = true;
			this.BTN_Chat_Send.Click += new System.EventHandler(this.BTN_Chat_Send_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 3);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(951, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// SEServerExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(951, 699);
			this.Controls.Add(this.splitContainer1);
			this.Name = "SEServerExtender";
			this.Text = "SEServerExtender";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.TAB_MainTabs.ResumeLayout(false);
			this.TAB_Control_Page.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.splitContainer4.Panel1.ResumeLayout(false);
			this.splitContainer4.Panel1.PerformLayout();
			this.splitContainer4.Panel2.ResumeLayout(false);
			this.splitContainer4.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer4)).EndInit();
			this.splitContainer4.ResumeLayout(false);
			this.TAB_Entities_Page.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
			this.splitContainer5.ResumeLayout(false);
			this.TAB_Chat_Page.ResumeLayout(false);
			this.splitContainer6.Panel1.ResumeLayout(false);
			this.splitContainer6.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer6)).EndInit();
			this.splitContainer6.ResumeLayout(false);
			this.splitContainer8.Panel1.ResumeLayout(false);
			this.splitContainer8.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer8)).EndInit();
			this.splitContainer8.ResumeLayout(false);
			this.splitContainer7.Panel1.ResumeLayout(false);
			this.splitContainer7.Panel1.PerformLayout();
			this.splitContainer7.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer7)).EndInit();
			this.splitContainer7.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabControl TAB_MainTabs;
		private System.Windows.Forms.TabPage TAB_Control_Page;
		private System.Windows.Forms.Button BTN_ServerControl_Start;
		private System.Windows.Forms.TabPage TAB_Entities_Page;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.PropertyGrid PG_Entities_Details;
		private System.Windows.Forms.Button BTN_ServerControl_Stop;
		private System.Windows.Forms.TreeView TRV_Entities;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.SplitContainer splitContainer4;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.Button BTN_Entities_New;
		private System.Windows.Forms.Button BTN_Entities_Delete;
		private System.Windows.Forms.Label LBL_Control_Debugging;
		private System.Windows.Forms.CheckBox CHK_Control_Debugging;
		private System.Windows.Forms.Label LBL_Control_EnableFactions;
		private System.Windows.Forms.CheckBox CHK_Control_EnableFactions;
		private System.Windows.Forms.Button BTN_Entities_Export;
		private System.Windows.Forms.TabPage TAB_Chat_Page;
		private System.Windows.Forms.SplitContainer splitContainer6;
		private System.Windows.Forms.ListBox LST_Chat_Messages;
		private System.Windows.Forms.SplitContainer splitContainer7;
		private System.Windows.Forms.TextBox TXT_Chat_Message;
		private System.Windows.Forms.Button BTN_Chat_Send;
		private System.Windows.Forms.SplitContainer splitContainer8;
		private System.Windows.Forms.ListBox LST_Chat_ConnectedPlayers;
	}
}