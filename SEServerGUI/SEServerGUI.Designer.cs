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
			this.LST_Entities = new System.Windows.Forms.ListBox();
			this.PG_Entity_Properties = new System.Windows.Forms.PropertyGrid();
			this.BTN_Connect = new System.Windows.Forms.Button();
			this.BTN_StartServer = new System.Windows.Forms.Button();
			this.BTN_StopServer = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
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
			this.splitContainer1.Size = new System.Drawing.Size(484, 462);
			this.splitContainer1.SplitterDistance = 427;
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
			this.splitContainer2.Panel1.Controls.Add(this.LST_Entities);
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.PG_Entity_Properties);
			this.splitContainer2.Size = new System.Drawing.Size(484, 427);
			this.splitContainer2.SplitterDistance = 200;
			this.splitContainer2.TabIndex = 1;
			// 
			// LST_Entities
			// 
			this.LST_Entities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LST_Entities.FormattingEnabled = true;
			this.LST_Entities.Location = new System.Drawing.Point(0, 0);
			this.LST_Entities.Name = "LST_Entities";
			this.LST_Entities.Size = new System.Drawing.Size(200, 427);
			this.LST_Entities.TabIndex = 0;
			this.LST_Entities.SelectedIndexChanged += new System.EventHandler(this.LST_Entities_SelectedIndexChanged);
			// 
			// PG_Entity_Properties
			// 
			this.PG_Entity_Properties.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PG_Entity_Properties.Location = new System.Drawing.Point(0, 0);
			this.PG_Entity_Properties.Name = "PG_Entity_Properties";
			this.PG_Entity_Properties.Size = new System.Drawing.Size(280, 427);
			this.PG_Entity_Properties.TabIndex = 0;
			// 
			// BTN_Connect
			// 
			this.BTN_Connect.Location = new System.Drawing.Point(406, 3);
			this.BTN_Connect.Name = "BTN_Connect";
			this.BTN_Connect.Size = new System.Drawing.Size(75, 23);
			this.BTN_Connect.TabIndex = 0;
			this.BTN_Connect.Text = "Connect";
			this.BTN_Connect.UseVisualStyleBackColor = true;
			this.BTN_Connect.Click += new System.EventHandler(this.BTN_Connect_Click);
			// 
			// BTN_StartServer
			// 
			this.BTN_StartServer.Location = new System.Drawing.Point(3, 3);
			this.BTN_StartServer.Name = "BTN_StartServer";
			this.BTN_StartServer.Size = new System.Drawing.Size(75, 23);
			this.BTN_StartServer.TabIndex = 1;
			this.BTN_StartServer.Text = "Start";
			this.BTN_StartServer.UseVisualStyleBackColor = true;
			this.BTN_StartServer.Click += new System.EventHandler(this.BTN_StartServer_Click);
			// 
			// BTN_StopServer
			// 
			this.BTN_StopServer.Location = new System.Drawing.Point(84, 3);
			this.BTN_StopServer.Name = "BTN_StopServer";
			this.BTN_StopServer.Size = new System.Drawing.Size(75, 23);
			this.BTN_StopServer.TabIndex = 2;
			this.BTN_StopServer.Text = "Stop";
			this.BTN_StopServer.UseVisualStyleBackColor = true;
			this.BTN_StopServer.Click += new System.EventHandler(this.BTN_StopServer_Click);
			// 
			// SEServerGUI
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(484, 462);
			this.Controls.Add(this.splitContainer1);
			this.Name = "SEServerGUI";
			this.Text = "SEServerGUI";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ListBox LST_Entities;
		private System.Windows.Forms.Button BTN_Connect;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.PropertyGrid PG_Entity_Properties;
		private System.Windows.Forms.Button BTN_StopServer;
		private System.Windows.Forms.Button BTN_StartServer;
	}
}

