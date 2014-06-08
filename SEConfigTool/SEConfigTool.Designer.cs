namespace SEConfigTool
{
    partial class SEConfigTool
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
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
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.LayoutPages = new System.Windows.Forms.TabControl();
            this.ConfigurationPage = new System.Windows.Forms.TabPage();
            this.GBX_Blocks = new System.Windows.Forms.GroupBox();
            this.BlockList = new System.Windows.Forms.ListBox();
            this.Temp = new System.Windows.Forms.TabPage();
            this.LayoutPages.SuspendLayout();
            this.ConfigurationPage.SuspendLayout();
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
            this.LayoutPages.Size = new System.Drawing.Size(560, 435);
            this.LayoutPages.TabIndex = 0;
            // 
            // ConfigurationPage
            // 
            this.ConfigurationPage.Controls.Add(this.GBX_Blocks);
            this.ConfigurationPage.Location = new System.Drawing.Point(4, 22);
            this.ConfigurationPage.Name = "ConfigurationPage";
            this.ConfigurationPage.Padding = new System.Windows.Forms.Padding(3);
            this.ConfigurationPage.Size = new System.Drawing.Size(552, 409);
            this.ConfigurationPage.TabIndex = 0;
            this.ConfigurationPage.Text = "Configuration";
            this.ConfigurationPage.UseVisualStyleBackColor = true;
            // 
            // GBX_Blocks
            // 
            this.GBX_Blocks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GBX_Blocks.Controls.Add(this.BlockList);
            this.GBX_Blocks.Location = new System.Drawing.Point(6, 6);
            this.GBX_Blocks.Name = "GBX_Blocks";
            this.GBX_Blocks.Size = new System.Drawing.Size(538, 395);
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
            this.BlockList.Size = new System.Drawing.Size(526, 368);
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
            this.ClientSize = new System.Drawing.Size(560, 435);
            this.Controls.Add(this.LayoutPages);
            this.Name = "SEConfigTool";
            this.Text = "SEConfigTool";
            this.Load += new System.EventHandler(this.SEConfigTool_Load);
            this.LayoutPages.ResumeLayout(false);
            this.ConfigurationPage.ResumeLayout(false);
            this.GBX_Blocks.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl LayoutPages;
        private System.Windows.Forms.TabPage ConfigurationPage;
        private System.Windows.Forms.TabPage Temp;
        private System.Windows.Forms.GroupBox GBX_Blocks;
        private System.Windows.Forms.ListBox BlockList;
    }
}

