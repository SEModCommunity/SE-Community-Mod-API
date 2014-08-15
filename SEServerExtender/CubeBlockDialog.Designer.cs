namespace SEServerExtender
{
	partial class CubeBlockDialog
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
			this.BTN_CubeBlock_Add = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.LBL_SubType = new System.Windows.Forms.Label();
			this.LBL_Type = new System.Windows.Forms.Label();
			this.TXT_Position_Z = new System.Windows.Forms.TextBox();
			this.TXT_Position_Y = new System.Windows.Forms.TextBox();
			this.TXT_Position_X = new System.Windows.Forms.TextBox();
			this.CMB_BlockType = new System.Windows.Forms.ComboBox();
			this.CMB_BlockSubType = new System.Windows.Forms.ComboBox();
			this.LBL_Position = new System.Windows.Forms.Label();
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
			// BTN_CubeBlock_Add
			// 
			this.BTN_CubeBlock_Add.Location = new System.Drawing.Point(260, 2);
			this.BTN_CubeBlock_Add.Name = "BTN_CubeBlock_Add";
			this.BTN_CubeBlock_Add.Size = new System.Drawing.Size(75, 23);
			this.BTN_CubeBlock_Add.TabIndex = 0;
			this.BTN_CubeBlock_Add.Text = "Add";
			this.BTN_CubeBlock_Add.UseVisualStyleBackColor = true;
			this.BTN_CubeBlock_Add.Click += new System.EventHandler(this.BTN_CubeBlock_Add_Click);
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
			this.splitContainer1.Panel2.Controls.Add(this.BTN_CubeBlock_Add);
			this.splitContainer1.Size = new System.Drawing.Size(347, 200);
			this.splitContainer1.SplitterDistance = 166;
			this.splitContainer1.TabIndex = 1;
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
			this.splitContainer2.Panel1.Controls.Add(this.LBL_Position);
			this.splitContainer2.Panel1.Controls.Add(this.LBL_SubType);
			this.splitContainer2.Panel1.Controls.Add(this.LBL_Type);
			this.splitContainer2.Panel1MinSize = 100;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.CMB_BlockSubType);
			this.splitContainer2.Panel2.Controls.Add(this.TXT_Position_Z);
			this.splitContainer2.Panel2.Controls.Add(this.TXT_Position_Y);
			this.splitContainer2.Panel2.Controls.Add(this.TXT_Position_X);
			this.splitContainer2.Panel2.Controls.Add(this.CMB_BlockType);
			this.splitContainer2.Size = new System.Drawing.Size(347, 166);
			this.splitContainer2.SplitterDistance = 100;
			this.splitContainer2.TabIndex = 0;
			// 
			// LBL_SubType
			// 
			this.LBL_SubType.AutoSize = true;
			this.LBL_SubType.Location = new System.Drawing.Point(12, 42);
			this.LBL_SubType.Name = "LBL_SubType";
			this.LBL_SubType.Size = new System.Drawing.Size(56, 13);
			this.LBL_SubType.TabIndex = 1;
			this.LBL_SubType.Text = "Sub-Type:";
			// 
			// LBL_Type
			// 
			this.LBL_Type.AutoSize = true;
			this.LBL_Type.Location = new System.Drawing.Point(12, 15);
			this.LBL_Type.Name = "LBL_Type";
			this.LBL_Type.Size = new System.Drawing.Size(34, 13);
			this.LBL_Type.TabIndex = 0;
			this.LBL_Type.Text = "Type:";
			// 
			// TXT_Position_Z
			// 
			this.TXT_Position_Z.Location = new System.Drawing.Point(95, 66);
			this.TXT_Position_Z.Name = "TXT_Position_Z";
			this.TXT_Position_Z.Size = new System.Drawing.Size(40, 20);
			this.TXT_Position_Z.TabIndex = 3;
			// 
			// TXT_Position_Y
			// 
			this.TXT_Position_Y.Location = new System.Drawing.Point(49, 66);
			this.TXT_Position_Y.Name = "TXT_Position_Y";
			this.TXT_Position_Y.Size = new System.Drawing.Size(40, 20);
			this.TXT_Position_Y.TabIndex = 2;
			// 
			// TXT_Position_X
			// 
			this.TXT_Position_X.Location = new System.Drawing.Point(3, 66);
			this.TXT_Position_X.Name = "TXT_Position_X";
			this.TXT_Position_X.Size = new System.Drawing.Size(40, 20);
			this.TXT_Position_X.TabIndex = 1;
			// 
			// CMB_BlockType
			// 
			this.CMB_BlockType.FormattingEnabled = true;
			this.CMB_BlockType.Location = new System.Drawing.Point(3, 12);
			this.CMB_BlockType.Name = "CMB_BlockType";
			this.CMB_BlockType.Size = new System.Drawing.Size(200, 21);
			this.CMB_BlockType.TabIndex = 0;
			this.CMB_BlockType.SelectedIndexChanged += new System.EventHandler(this.CMB_BlockType_SelectedIndexChanged);
			// 
			// CMB_BlockSubType
			// 
			this.CMB_BlockSubType.FormattingEnabled = true;
			this.CMB_BlockSubType.Location = new System.Drawing.Point(3, 39);
			this.CMB_BlockSubType.Name = "CMB_BlockSubType";
			this.CMB_BlockSubType.Size = new System.Drawing.Size(200, 21);
			this.CMB_BlockSubType.TabIndex = 4;
			// 
			// LBL_Position
			// 
			this.LBL_Position.AutoSize = true;
			this.LBL_Position.Location = new System.Drawing.Point(12, 69);
			this.LBL_Position.Name = "LBL_Position";
			this.LBL_Position.Size = new System.Drawing.Size(47, 13);
			this.LBL_Position.TabIndex = 2;
			this.LBL_Position.Text = "Position:";
			// 
			// CubeBlockDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(347, 200);
			this.Controls.Add(this.splitContainer1);
			this.Name = "CubeBlockDialog";
			this.Text = "Add New Cubeblock";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel1.PerformLayout();
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
			this.splitContainer2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button BTN_CubeBlock_Add;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label LBL_SubType;
		private System.Windows.Forms.Label LBL_Type;
		private System.Windows.Forms.ComboBox CMB_BlockType;
		private System.Windows.Forms.TextBox TXT_Position_X;
		private System.Windows.Forms.TextBox TXT_Position_Y;
		private System.Windows.Forms.TextBox TXT_Position_Z;
		private System.Windows.Forms.Label LBL_Position;
		private System.Windows.Forms.ComboBox CMB_BlockSubType;
	}
}