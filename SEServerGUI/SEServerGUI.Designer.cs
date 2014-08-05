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
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.splitContainer5 = new System.Windows.Forms.SplitContainer();
			this.TRV_Entities = new System.Windows.Forms.TreeView();
			this.BTN_Entities_Export = new System.Windows.Forms.Button();
			this.BTN_Entities_New = new System.Windows.Forms.Button();
			this.BTN_Entities_Delete = new System.Windows.Forms.Button();
			this.PG_Entities_Details = new System.Windows.Forms.PropertyGrid();
			this.BTN_StopServer = new System.Windows.Forms.Button();
			this.BTN_StartServer = new System.Windows.Forms.Button();
			this.BTN_Connect = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).BeginInit();
			this.splitContainer5.Panel1.SuspendLayout();
			this.splitContainer5.Panel2.SuspendLayout();
			this.splitContainer5.SuspendLayout();
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
			this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.BTN_StopServer);
			this.splitContainer1.Panel2.Controls.Add(this.BTN_StartServer);
			this.splitContainer1.Panel2.Controls.Add(this.BTN_Connect);
			this.splitContainer1.Size = new System.Drawing.Size(653, 580);
			this.splitContainer1.SplitterDistance = 545;
			this.splitContainer1.TabIndex = 0;
			// 
			// splitContainer2
			// 
			this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer2.Location = new System.Drawing.Point(0, 0);
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.splitContainer5);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.PG_Entities_Details);
			this.splitContainer2.Size = new System.Drawing.Size(653, 545);
			this.splitContainer2.SplitterDistance = 257;
			this.splitContainer2.TabIndex = 1;
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
			this.splitContainer5.Size = new System.Drawing.Size(257, 545);
			this.splitContainer5.SplitterDistance = 510;
			this.splitContainer5.TabIndex = 1;
			// 
			// TRV_Entities
			// 
			this.TRV_Entities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TRV_Entities.Location = new System.Drawing.Point(0, 0);
			this.TRV_Entities.Name = "TRV_Entities";
			this.TRV_Entities.Size = new System.Drawing.Size(257, 510);
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
			this.PG_Entities_Details.Size = new System.Drawing.Size(392, 545);
			this.PG_Entities_Details.TabIndex = 0;
			this.PG_Entities_Details.Click += new System.EventHandler(this.PG_Entities_Details_Click);
			// 
			// BTN_StopServer
			// 
			this.BTN_StopServer.Enabled = false;
			this.BTN_StopServer.Location = new System.Drawing.Point(84, 3);
			this.BTN_StopServer.Name = "BTN_StopServer";
			this.BTN_StopServer.Size = new System.Drawing.Size(75, 23);
			this.BTN_StopServer.TabIndex = 2;
			this.BTN_StopServer.Text = "Stop";
			this.BTN_StopServer.UseVisualStyleBackColor = true;
			this.BTN_StopServer.Click += new System.EventHandler(this.BTN_StopServer_Click);
			// 
			// BTN_StartServer
			// 
			this.BTN_StartServer.Enabled = false;
			this.BTN_StartServer.Location = new System.Drawing.Point(3, 3);
			this.BTN_StartServer.Name = "BTN_StartServer";
			this.BTN_StartServer.Size = new System.Drawing.Size(75, 23);
			this.BTN_StartServer.TabIndex = 1;
			this.BTN_StartServer.Text = "Start";
			this.BTN_StartServer.UseVisualStyleBackColor = true;
			this.BTN_StartServer.Click += new System.EventHandler(this.BTN_StartServer_Click);
			// 
			// BTN_Connect
			// 
			this.BTN_Connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.BTN_Connect.Location = new System.Drawing.Point(575, 2);
			this.BTN_Connect.Name = "BTN_Connect";
			this.BTN_Connect.Size = new System.Drawing.Size(75, 23);
			this.BTN_Connect.TabIndex = 0;
			this.BTN_Connect.Text = "Connect";
			this.BTN_Connect.UseVisualStyleBackColor = true;
			this.BTN_Connect.Click += new System.EventHandler(this.BTN_Connect_Click);
			// 
			// SEServerGUIForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(653, 580);
			this.Controls.Add(this.splitContainer1);
			this.Name = "SEServerGUIForm";
			this.Text = "SEServerGUI";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.splitContainer5.Panel1.ResumeLayout(false);
			this.splitContainer5.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer5)).EndInit();
			this.splitContainer5.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.Button BTN_Connect;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.PropertyGrid PG_Entities_Details;
		private System.Windows.Forms.Button BTN_StopServer;
		private System.Windows.Forms.Button BTN_StartServer;
		private System.Windows.Forms.SplitContainer splitContainer5;
		private System.Windows.Forms.TreeView TRV_Entities;
		private System.Windows.Forms.Button BTN_Entities_Export;
		private System.Windows.Forms.Button BTN_Entities_New;
		private System.Windows.Forms.Button BTN_Entities_Delete;
	}
}

