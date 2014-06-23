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
			this.LBL_Control_WorldName = new System.Windows.Forms.Label();
			this.TXT_Control_WorldName = new System.Windows.Forms.TextBox();
			this.TAB_Entities_Page = new System.Windows.Forms.TabPage();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.TRV_Entities = new System.Windows.Forms.TreeView();
			this.PG_Entities_Details = new System.Windows.Forms.PropertyGrid();
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
			this.splitContainer1.Size = new System.Drawing.Size(798, 656);
			this.splitContainer1.SplitterDistance = 627;
			this.splitContainer1.TabIndex = 0;
			// 
			// TAB_MainTabs
			// 
			this.TAB_MainTabs.Controls.Add(this.TAB_Control_Page);
			this.TAB_MainTabs.Controls.Add(this.TAB_Entities_Page);
			this.TAB_MainTabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TAB_MainTabs.Location = new System.Drawing.Point(0, 0);
			this.TAB_MainTabs.Name = "TAB_MainTabs";
			this.TAB_MainTabs.SelectedIndex = 0;
			this.TAB_MainTabs.Size = new System.Drawing.Size(798, 627);
			this.TAB_MainTabs.TabIndex = 0;
			// 
			// TAB_Control_Page
			// 
			this.TAB_Control_Page.Controls.Add(this.splitContainer3);
			this.TAB_Control_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Control_Page.Name = "TAB_Control_Page";
			this.TAB_Control_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Control_Page.Size = new System.Drawing.Size(621, 465);
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
			this.splitContainer3.Size = new System.Drawing.Size(615, 459);
			this.splitContainer3.SplitterDistance = 300;
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
			this.BTN_ServerControl_Start.Click += new System.EventHandler(this.button1_Click);
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
			this.splitContainer4.Panel1.Controls.Add(this.LBL_Control_WorldName);
			this.splitContainer4.Panel1MinSize = 100;
			// 
			// splitContainer4.Panel2
			// 
			this.splitContainer4.Panel2.Controls.Add(this.TXT_Control_WorldName);
			this.splitContainer4.Size = new System.Drawing.Size(311, 459);
			this.splitContainer4.SplitterDistance = 100;
			this.splitContainer4.TabIndex = 0;
			// 
			// LBL_Control_WorldName
			// 
			this.LBL_Control_WorldName.AutoSize = true;
			this.LBL_Control_WorldName.Location = new System.Drawing.Point(3, 6);
			this.LBL_Control_WorldName.Name = "LBL_Control_WorldName";
			this.LBL_Control_WorldName.Size = new System.Drawing.Size(69, 13);
			this.LBL_Control_WorldName.TabIndex = 3;
			this.LBL_Control_WorldName.Text = "World Name:";
			this.LBL_Control_WorldName.Visible = false;
			// 
			// TXT_Control_WorldName
			// 
			this.TXT_Control_WorldName.Location = new System.Drawing.Point(3, 3);
			this.TXT_Control_WorldName.Name = "TXT_Control_WorldName";
			this.TXT_Control_WorldName.Size = new System.Drawing.Size(150, 20);
			this.TXT_Control_WorldName.TabIndex = 2;
			this.TXT_Control_WorldName.Text = "DotS - Survival World 1";
			this.TXT_Control_WorldName.Visible = false;
			// 
			// TAB_Entities_Page
			// 
			this.TAB_Entities_Page.Controls.Add(this.splitContainer2);
			this.TAB_Entities_Page.Location = new System.Drawing.Point(4, 22);
			this.TAB_Entities_Page.Name = "TAB_Entities_Page";
			this.TAB_Entities_Page.Padding = new System.Windows.Forms.Padding(3);
			this.TAB_Entities_Page.Size = new System.Drawing.Size(790, 601);
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
			this.splitContainer2.Panel1.Controls.Add(this.TRV_Entities);
			this.splitContainer2.Panel1MinSize = 250;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.PG_Entities_Details);
			this.splitContainer2.Size = new System.Drawing.Size(784, 595);
			this.splitContainer2.SplitterDistance = 250;
			this.splitContainer2.TabIndex = 0;
			// 
			// TRV_Entities
			// 
			this.TRV_Entities.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TRV_Entities.Location = new System.Drawing.Point(0, 0);
			this.TRV_Entities.Name = "TRV_Entities";
			this.TRV_Entities.Size = new System.Drawing.Size(250, 595);
			this.TRV_Entities.TabIndex = 0;
			this.TRV_Entities.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TRV_Entities_AfterSelect);
			// 
			// PG_Entities_Details
			// 
			this.PG_Entities_Details.Dock = System.Windows.Forms.DockStyle.Fill;
			this.PG_Entities_Details.Location = new System.Drawing.Point(0, 0);
			this.PG_Entities_Details.Name = "PG_Entities_Details";
			this.PG_Entities_Details.Size = new System.Drawing.Size(530, 595);
			this.PG_Entities_Details.TabIndex = 0;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Location = new System.Drawing.Point(0, 3);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(798, 22);
			this.statusStrip1.TabIndex = 0;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// SEServerExtender
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(798, 656);
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
		private System.Windows.Forms.Label LBL_Control_WorldName;
		private System.Windows.Forms.TextBox TXT_Control_WorldName;
	}
}