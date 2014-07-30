namespace SEServerExtender
{
	partial class InventoryItemDialog
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
			this.BTN_InventoryItem_Add = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.LBL_Amount = new System.Windows.Forms.Label();
			this.LBL_Type = new System.Windows.Forms.Label();
			this.TXT_ItemAmount = new System.Windows.Forms.TextBox();
			this.CMB_ItemType = new System.Windows.Forms.ComboBox();
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
			// BTN_InventoryItem_Add
			// 
			this.BTN_InventoryItem_Add.Location = new System.Drawing.Point(197, 3);
			this.BTN_InventoryItem_Add.Name = "BTN_InventoryItem_Add";
			this.BTN_InventoryItem_Add.Size = new System.Drawing.Size(75, 23);
			this.BTN_InventoryItem_Add.TabIndex = 0;
			this.BTN_InventoryItem_Add.Text = "Add";
			this.BTN_InventoryItem_Add.UseVisualStyleBackColor = true;
			this.BTN_InventoryItem_Add.Click += new System.EventHandler(this.BTN_InventoryItem_Add_Click);
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
			this.splitContainer1.Panel2.Controls.Add(this.BTN_InventoryItem_Add);
			this.splitContainer1.Size = new System.Drawing.Size(284, 262);
			this.splitContainer1.SplitterDistance = 228;
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
			this.splitContainer2.Panel1.Controls.Add(this.LBL_Amount);
			this.splitContainer2.Panel1.Controls.Add(this.LBL_Type);
			this.splitContainer2.Panel1MinSize = 100;
			// 
			// splitContainer2.Panel2
			// 
			this.splitContainer2.Panel2.Controls.Add(this.TXT_ItemAmount);
			this.splitContainer2.Panel2.Controls.Add(this.CMB_ItemType);
			this.splitContainer2.Size = new System.Drawing.Size(284, 228);
			this.splitContainer2.SplitterDistance = 100;
			this.splitContainer2.TabIndex = 0;
			// 
			// LBL_Amount
			// 
			this.LBL_Amount.AutoSize = true;
			this.LBL_Amount.Location = new System.Drawing.Point(12, 42);
			this.LBL_Amount.Name = "LBL_Amount";
			this.LBL_Amount.Size = new System.Drawing.Size(46, 13);
			this.LBL_Amount.TabIndex = 1;
			this.LBL_Amount.Text = "Amount:";
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
			// TXT_ItemAmount
			// 
			this.TXT_ItemAmount.Location = new System.Drawing.Point(3, 39);
			this.TXT_ItemAmount.Name = "TXT_ItemAmount";
			this.TXT_ItemAmount.Size = new System.Drawing.Size(125, 20);
			this.TXT_ItemAmount.TabIndex = 1;
			// 
			// CMB_ItemType
			// 
			this.CMB_ItemType.FormattingEnabled = true;
			this.CMB_ItemType.Location = new System.Drawing.Point(3, 12);
			this.CMB_ItemType.Name = "CMB_ItemType";
			this.CMB_ItemType.Size = new System.Drawing.Size(125, 21);
			this.CMB_ItemType.TabIndex = 0;
			// 
			// InventoryItemDialog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.Controls.Add(this.splitContainer1);
			this.Name = "InventoryItemDialog";
			this.Text = "InventoryItemDialog";
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

		private System.Windows.Forms.Button BTN_InventoryItem_Add;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.Label LBL_Amount;
		private System.Windows.Forms.Label LBL_Type;
		private System.Windows.Forms.TextBox TXT_ItemAmount;
		private System.Windows.Forms.ComboBox CMB_ItemType;
	}
}