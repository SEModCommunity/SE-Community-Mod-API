namespace SEServerGUI
{
	partial class SEServerGUIForm
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
			this.splitContainer3 = new System.Windows.Forms.SplitContainer();
			this.CHK_Control_Debugging = new System.Windows.Forms.CheckBox();
			this.BTN_StopServer = new System.Windows.Forms.Button();
			this.LBL_Control_AutosaveInterval = new System.Windows.Forms.Label();
			this.BTN_StartServer = new System.Windows.Forms.Button();
			this.CMB_Control_AutosaveInterval = new System.Windows.Forms.ComboBox();
			this.BTN_Connect = new System.Windows.Forms.Button();
			this.TAB_MainTabs = new System.Windows.Forms.TabControl();
			this.TAB_Control_Page = new System.Windows.Forms.TabPage();
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
			this.TAB_Factions_Page = new System.Windows.Forms.TabPage();
			this.TAB_Plugins_Page = new System.Windows.Forms.TabPage();
			this.splitContainer11 = new System.Windows.Forms.SplitContainer();
			this.splitContainer12 = new System.Windows.Forms.SplitContainer();
			this.LST_Plugins = new System.Windows.Forms.ListBox();
			this.BTN_Plugins_Refresh = new System.Windows.Forms.Button();
			this.BTN_Plugins_Load = new System.Windows.Forms.Button();
			this.BTN_Plugins_Unload = new System.Windows.Forms.Button();
			this.PG_Plugins = new System.Windows.Forms.PropertyGrid();
			this.TAB_Utilities_Page = new System.Windows.Forms.TabPage();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
			this.splitContainer3.Panel1.SuspendLayout();
			this.splitContainer3.SuspendLayout();
			this.TAB_MainTabs.SuspendLayout();
			this.TAB_Control_Page.SuspendLayout();
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
			this.TAB_Plugins_Page.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer11)).BeginInit();
			this.splitContainer11.Panel1.SuspendLayout();
			this.splitContainer11.Panel2.SuspendLayout();
			this.splitContainer11.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer12)).BeginInit();
			this.splitContainer12.Panel1.SuspendLayout();
			this.splitContainer12.Panel2.SuspendLayout();
			this.splitContainer12.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer3);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.BTN_Connect);
			this.splitContainer1.Size = new System.Drawing.Size(570, 530);
			this.splitContainer1.SplitterDistance = 495;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer3
			// 
			this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer3.Location = new System.Drawing.Point(0, 0);
			this.splitContainer3.Name = "splitContainer3";
			// 
			// splitContainer3.Panel1
			// 
			this.splitContainer3.Panel1.Controls.Add(this.CHK_Control_Debugging);
			this.splitContainer3.Panel1.Controls.Add(this.BTN_StopServer);
			this.splitContainer3.Panel1.Controls.Add(this.LBL_Control_AutosaveInterval);
			this.splitContainer3.Panel1.Controls.Add(this.BTN_StartServer);
			this.splitContainer3.Panel1.Controls.Add(this.CMB_Control_AutosaveInterval);
			this.splitContainer3.Size = new System.Drawing.Size(570, 495);
			this.splitContainer3.SplitterDistance = 165;
			this.splitContainer3.TabIndex = 0;
			// 
			// CHK_Control_Debugging
			// 
			this.CHK_Control_Debugging.AutoSize = true;
			this.CHK_Control_Debugging.Enabled = false;
			this.CHK_Control_Debugging.Location = new System.Drawing.Point(5, 61);
			this.CHK_Control_Debugging.Name = "CHK_Control_Debugging";
			this.CHK_Control_Debugging.Size = new System.Drawing.Size(87, 17);
			this.CHK_Control_Debugging.TabIndex = 10;
			this.CHK_Control_Debugging.Text = "Debug mode";
			this.CHK_Control_Debugging.UseVisualStyleBackColor = true;
			this.CHK_Control_Debugging.CheckedChanged += new System.EventHandler(this.CHK_Control_Debugging_CheckedChanged);
			// 
			// BTN_StopServer
			// 
			this.BTN_StopServer.Enabled = false;
			this.BTN_StopServer.Location = new System.Drawing.Point(5, 32);
			this.BTN_StopServer.Name = "BTN_StopServer";
			this.BTN_StopServer.Size = new System.Drawing.Size(87, 23);
			this.BTN_StopServer.TabIndex = 2;
			this.BTN_StopServer.Text = "Stop Server";
			this.BTN_StopServer.UseVisualStyleBackColor = true;
			this.BTN_StopServer.Click += new System.EventHandler(this.BTN_StopServer_Click);
			// 
			// LBL_Control_AutosaveInterval
			// 
			this.LBL_Control_AutosaveInterval.AutoSize = true;
			this.LBL_Control_AutosaveInterval.Location = new System.Drawing.Point(2, 81);
			this.LBL_Control_AutosaveInterval.Name = "LBL_Control_AutosaveInterval";
			this.LBL_Control_AutosaveInterval.Size = new System.Drawing.Size(92, 13);
			this.LBL_Control_AutosaveInterval.TabIndex = 9;
			this.LBL_Control_AutosaveInterval.Text = "Auto-save interval";
			// 
			// BTN_StartServer
			// 
			this.BTN_StartServer.Enabled = false;
			this.BTN_StartServer.Location = new System.Drawing.Point(5, 3);
			this.BTN_StartServer.Name = "BTN_StartServer";
			this.BTN_StartServer.Size = new System.Drawing.Size(87, 23);
			this.BTN_StartServer.TabIndex = 1;
			this.BTN_StartServer.Text = "Start Server";
			this.BTN_StartServer.UseVisualStyleBackColor = true;
			this.BTN_StartServer.Click += new System.EventHandler(this.BTN_StartServer_Click);
			// 
			// CMB_Control_AutosaveInterval
			// 
			this.CMB_Control_AutosaveInterval.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CMB_Control_AutosaveInterval.Enabled = false;
			this.CMB_Control_AutosaveInterval.FormattingEnabled = true;
			this.CMB_Control_AutosaveInterval.Location = new System.Drawing.Point(5, 97);
			this.CMB_Control_AutosaveInterval.Name = "CMB_Control_AutosaveInterval";
			this.CMB_Control_AutosaveInterval.Size = new System.Drawing.Size(150, 21);
			this.CMB_Control_AutosaveInterval.TabIndex = 8;
			this.CMB_Control_AutosaveInterval.SelectedIndexChanged += new System.EventHandler(this.CMB_Control_AutosaveInterval_SelectedIndexChanged);
			// 
			// BTN_Connect
			// 
			this.BTN_Connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.BTN_Connect.Location = new System.Drawing.Point(492, 5);
			this.BTN_Connect.Name = "BTN_Connect";
			this.BTN_Connect.Size = new System.Drawing.Size(75, 23);
			this.BTN_Connect.TabIndex = 0;
			this.BTN_Connect.Text = "Connect";
			this.BTN_Connect.UseVisualStyleBackColor = true;
			this.BTN_Connect.Click += new System.EventHandler(this.BTN_Connect_Click);
			// 
			// TAB_MainTabs
			// 
			this.TAB_MainTabs.Controls.Add(this.TAB_Control_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Entities_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Chat_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Factions_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Plugins_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Utilities_Page);
			this.TAB_MainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TAB_MainTabs.Location = new System.Drawing.Point(0, 0);
			this.TAB_MainTabs.Name = "TAB_MainTabs";
			this.TAB_MainTabs.SelectedIndex = 0;
			this.TAB_MainTabs.Size = new System.Drawing.Size(584, 562);
			this.TAB_MainTabs.TabIndex = 1;
			// 
			// TAB_Control_Page
			// 
			this.TAB_Control_Page.Controls.Add(this.splitContainer1);
			this.TAB_Control_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Control_Page.Name = "TAB_Control_Page";
			this.TAB_Control_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Control_Page.Size = new System.Drawing.Size(576, 536);
			this.TAB_Control_Page.TabIndex = 0;
			this.TAB_Control_Page.Text = "Control";
			this.TAB_Control_Page.UseVisualStyleBackColor = true;
			// 
			// TAB_Entities_Page
			// 
			this.TAB_Entities_Page.Controls.Add(this.splitContainer2);
			this.TAB_Entities_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Entities_Page.Name = "TAB_Entities_Page";
			this.TAB_Entities_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Entities_Page.Size = new System.Drawing.Size(576, 536);
			this.TAB_Entities_Page.TabIndex = 2;
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
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.PG_Entities_Details);
			this.splitContainer2.Size = new System.Drawing.Size(570, 530);
			this.splitContainer2.SplitterDistance = 257;
			this.splitContainer2.TabIndex = 2;
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
			this.splitContainer5.Size = new System.Drawing.Size(257, 530);
			this.splitContainer5.SplitterDistance = 495;
			this.splitContainer5.TabIndex = 1;
			// 
			// TRV_Entities
			// 
			this.TRV_Entities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TRV_Entities.Location = new System.Drawing.Point(0, 0);
			this.TRV_Entities.Name = "TRV_Entities";
			this.TRV_Entities.Size = new System.Drawing.Size(257, 495);
			this.TRV_Entities.TabIndex = 0;
			this.TRV_Entities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TRV_Entities_AfterSelect);
			this.TRV_Entities.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TRV_Entities_NodeRefresh);
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
			// 
			// PG_Entities_Details
			// 
			this.PG_Entities_Details.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PG_Entities_Details.Location = new System.Drawing.Point(0, 0);
			this.PG_Entities_Details.Name = "PG_Entities_Details";
			this.PG_Entities_Details.Size = new System.Drawing.Size(309, 530);
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
			this.TAB_Chat_Page.Size = new System.Drawing.Size(576, 536);
			this.TAB_Chat_Page.TabIndex = 1;
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
			this.splitContainer6.Size = new System.Drawing.Size(570, 530);
			this.splitContainer6.SplitterDistance = 497;
			this.splitContainer6.TabIndex = 5;
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
			this.splitContainer8.Size = new System.Drawing.Size(570, 497);
			this.splitContainer8.SplitterDistance = 383;
			this.splitContainer8.TabIndex = 4;
			// 
			// LST_Chat_Messages
			// 
			this.LST_Chat_Messages.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LST_Chat_Messages.FormattingEnabled = true;
			this.LST_Chat_Messages.Location = new System.Drawing.Point(0, 0);
			this.LST_Chat_Messages.Name = "LST_Chat_Messages";
			this.LST_Chat_Messages.ScrollAlwaysVisible = true;
			this.LST_Chat_Messages.Size = new System.Drawing.Size(383, 497);
			this.LST_Chat_Messages.TabIndex = 3;
			// 
			// LST_Chat_ConnectedPlayers
			// 
			this.LST_Chat_ConnectedPlayers.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LST_Chat_ConnectedPlayers.FormattingEnabled = true;
			this.LST_Chat_ConnectedPlayers.Location = new System.Drawing.Point(0, 0);
			this.LST_Chat_ConnectedPlayers.Name = "LST_Chat_ConnectedPlayers";
			this.LST_Chat_ConnectedPlayers.Size = new System.Drawing.Size(183, 497);
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
			this.splitContainer7.Size = new System.Drawing.Size(570, 29);
			this.splitContainer7.SplitterDistance = 433;
			this.splitContainer7.TabIndex = 2;
			// 
			// TXT_Chat_Message
			// 
			this.TXT_Chat_Message.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TXT_Chat_Message.Enabled = false;
			this.TXT_Chat_Message.Location = new System.Drawing.Point(0, 0);
			this.TXT_Chat_Message.Name = "TXT_Chat_Message";
			this.TXT_Chat_Message.Size = new System.Drawing.Size(433, 20);
			this.TXT_Chat_Message.TabIndex = 0;
			this.TXT_Chat_Message.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TXT_Chat_Message_KeyDown);
			// 
			// BTN_Chat_Send
			// 
			this.BTN_Chat_Send.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BTN_Chat_Send.Enabled = false;
			this.BTN_Chat_Send.Location = new System.Drawing.Point(53, 3);
			this.BTN_Chat_Send.Name = "BTN_Chat_Send";
			this.BTN_Chat_Send.Size = new System.Drawing.Size(75, 23);
			this.BTN_Chat_Send.TabIndex = 1;
			this.BTN_Chat_Send.Text = "Send";
			this.BTN_Chat_Send.UseVisualStyleBackColor = true;
			this.BTN_Chat_Send.Click += new System.EventHandler(this.BTN_Chat_Send_Click);
			// 
			// TAB_Factions_Page
			// 
			this.TAB_Factions_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Factions_Page.Name = "TAB_Factions_Page";
			this.TAB_Factions_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Factions_Page.Size = new System.Drawing.Size(576, 536);
			this.TAB_Factions_Page.TabIndex = 3;
			this.TAB_Factions_Page.Text = "Factions";
			this.TAB_Factions_Page.UseVisualStyleBackColor = true;
			// 
			// TAB_Plugins_Page
			// 
			this.TAB_Plugins_Page.Controls.Add(this.splitContainer11);
			this.TAB_Plugins_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Plugins_Page.Name = "TAB_Plugins_Page";
			this.TAB_Plugins_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Plugins_Page.Size = new System.Drawing.Size(576, 536);
			this.TAB_Plugins_Page.TabIndex = 4;
			this.TAB_Plugins_Page.Text = "Plugins";
			this.TAB_Plugins_Page.UseVisualStyleBackColor = true;
			// 
			// splitContainer11
			// 
			this.splitContainer11.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer11.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer11.Location = new System.Drawing.Point(3, 3);
			this.splitContainer11.Name = "splitContainer11";
			// 
			// splitContainer11.Panel1
			// 
			this.splitContainer11.Panel1.Controls.Add(this.splitContainer12);
			this.splitContainer11.Panel1MinSize = 300;
			// 
			// splitContainer11.Panel2
			// 
			this.splitContainer11.Panel2.Controls.Add(this.PG_Plugins);
			this.splitContainer11.Size = new System.Drawing.Size(570, 530);
			this.splitContainer11.SplitterDistance = 300;
			this.splitContainer11.TabIndex = 1;
			// 
			// splitContainer12
			// 
			this.splitContainer12.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer12.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer12.Location = new System.Drawing.Point(0, 0);
			this.splitContainer12.Name = "splitContainer12";
			this.splitContainer12.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer12.Panel1
			// 
			this.splitContainer12.Panel1.Controls.Add(this.LST_Plugins);
			// 
			// splitContainer12.Panel2
			// 
			this.splitContainer12.Panel2.Controls.Add(this.BTN_Plugins_Refresh);
			this.splitContainer12.Panel2.Controls.Add(this.BTN_Plugins_Load);
			this.splitContainer12.Panel2.Controls.Add(this.BTN_Plugins_Unload);
			this.splitContainer12.Size = new System.Drawing.Size(300, 530);
			this.splitContainer12.SplitterDistance = 495;
			this.splitContainer12.TabIndex = 0;
			// 
			// LST_Plugins
			// 
			this.LST_Plugins.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LST_Plugins.FormattingEnabled = true;
			this.LST_Plugins.Location = new System.Drawing.Point(0, 0);
			this.LST_Plugins.Name = "LST_Plugins";
			this.LST_Plugins.Size = new System.Drawing.Size(300, 495);
			this.LST_Plugins.TabIndex = 0;
			this.LST_Plugins.SelectedIndexChanged += new System.EventHandler(this.LST_Plugins_SelectedIndexChanged);
			// 
			// BTN_Plugins_Refresh
			// 
			this.BTN_Plugins_Refresh.Enabled = false;
			this.BTN_Plugins_Refresh.Location = new System.Drawing.Point(3, 3);
			this.BTN_Plugins_Refresh.Name = "BTN_Plugins_Refresh";
			this.BTN_Plugins_Refresh.Size = new System.Drawing.Size(75, 23);
			this.BTN_Plugins_Refresh.TabIndex = 2;
			this.BTN_Plugins_Refresh.Text = "Refresh";
			this.BTN_Plugins_Refresh.UseVisualStyleBackColor = true;
			this.BTN_Plugins_Refresh.Click += new System.EventHandler(this.BTN_Plugins_Refresh_Click);
			// 
			// BTN_Plugins_Load
			// 
			this.BTN_Plugins_Load.Enabled = false;
			this.BTN_Plugins_Load.Location = new System.Drawing.Point(141, 3);
			this.BTN_Plugins_Load.Name = "BTN_Plugins_Load";
			this.BTN_Plugins_Load.Size = new System.Drawing.Size(75, 23);
			this.BTN_Plugins_Load.TabIndex = 1;
			this.BTN_Plugins_Load.Text = "Load";
			this.BTN_Plugins_Load.UseVisualStyleBackColor = true;
			this.BTN_Plugins_Load.Click += new System.EventHandler(this.BTN_Plugins_Load_Click);
			// 
			// BTN_Plugins_Unload
			// 
			this.BTN_Plugins_Unload.Enabled = false;
			this.BTN_Plugins_Unload.Location = new System.Drawing.Point(222, 3);
			this.BTN_Plugins_Unload.Name = "BTN_Plugins_Unload";
			this.BTN_Plugins_Unload.Size = new System.Drawing.Size(75, 23);
			this.BTN_Plugins_Unload.TabIndex = 0;
			this.BTN_Plugins_Unload.Text = "Unload";
			this.BTN_Plugins_Unload.UseVisualStyleBackColor = true;
			this.BTN_Plugins_Unload.Click += new System.EventHandler(this.BTN_Plugins_Unload_Click);
			// 
			// PG_Plugins
			// 
			this.PG_Plugins.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PG_Plugins.Enabled = false;
			this.PG_Plugins.Location = new System.Drawing.Point(0, 0);
			this.PG_Plugins.Name = "PG_Plugins";
			this.PG_Plugins.Size = new System.Drawing.Size(266, 530);
			this.PG_Plugins.TabIndex = 0;
			// 
			// TAB_Utilities_Page
			// 
			this.TAB_Utilities_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Utilities_Page.Name = "TAB_Utilities_Page";
			this.TAB_Utilities_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Utilities_Page.Size = new System.Drawing.Size(576, 536);
			this.TAB_Utilities_Page.TabIndex = 5;
			this.TAB_Utilities_Page.Text = "Utilities";
			this.TAB_Utilities_Page.UseVisualStyleBackColor = true;
			// 
			// SEServerGUIForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(584, 562);
			this.Controls.Add(this.TAB_MainTabs);
			this.Name = "SEServerGUIForm";
			this.Text = "SEServerGUI";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer3.Panel1.ResumeLayout(false);
			this.splitContainer3.Panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
			this.splitContainer3.ResumeLayout(false);
			this.TAB_MainTabs.ResumeLayout(false);
			this.TAB_Control_Page.ResumeLayout(false);
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
			this.TAB_Plugins_Page.ResumeLayout(false);
			this.splitContainer11.Panel1.ResumeLayout(false);
			this.splitContainer11.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer11)).EndInit();
			this.splitContainer11.ResumeLayout(false);
			this.splitContainer12.Panel1.ResumeLayout(false);
			this.splitContainer12.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer12)).EndInit();
			this.splitContainer12.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button BTN_Connect;
		private System.Windows.Forms.Button BTN_StopServer;
		private System.Windows.Forms.Button BTN_StartServer;
		private System.Windows.Forms.TabControl TAB_MainTabs;
		private System.Windows.Forms.TabPage TAB_Control_Page;
		private System.Windows.Forms.TabPage TAB_Chat_Page;
		private System.Windows.Forms.SplitContainer splitContainer6;
		private System.Windows.Forms.SplitContainer splitContainer8;
		private System.Windows.Forms.ListBox LST_Chat_Messages;
		private System.Windows.Forms.ListBox LST_Chat_ConnectedPlayers;
		private System.Windows.Forms.SplitContainer splitContainer7;
		private System.Windows.Forms.TextBox TXT_Chat_Message;
		private System.Windows.Forms.Button BTN_Chat_Send;
		private System.Windows.Forms.TabPage TAB_Entities_Page;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.TreeView TRV_Entities;
		private System.Windows.Forms.Button BTN_Entities_Export;
		private System.Windows.Forms.Button BTN_Entities_New;
		private System.Windows.Forms.Button BTN_Entities_Delete;
		private System.Windows.Forms.PropertyGrid PG_Entities_Details;
		private System.Windows.Forms.TabPage TAB_Factions_Page;
		private System.Windows.Forms.TabPage TAB_Plugins_Page;
		private System.Windows.Forms.TabPage TAB_Utilities_Page;
		private System.Windows.Forms.SplitContainer splitContainer11;
		private System.Windows.Forms.SplitContainer splitContainer12;
		private System.Windows.Forms.ListBox LST_Plugins;
		private System.Windows.Forms.Button BTN_Plugins_Refresh;
		private System.Windows.Forms.Button BTN_Plugins_Load;
		private System.Windows.Forms.Button BTN_Plugins_Unload;
		private System.Windows.Forms.PropertyGrid PG_Plugins;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.ComboBox CMB_Control_AutosaveInterval;
		private System.Windows.Forms.Label LBL_Control_AutosaveInterval;
		private System.Windows.Forms.CheckBox CHK_Control_Debugging;
	}
}

