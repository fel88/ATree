namespace ATree
{
    partial class mdi
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(mdi));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.diagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checklistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.diagramToolStripMenuItem,
            this.checklistToolStripMenuItem,
            this.abourToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(998, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // diagramToolStripMenuItem
            // 
            this.diagramToolStripMenuItem.Name = "diagramToolStripMenuItem";
            this.diagramToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.diagramToolStripMenuItem.Text = "Diagram";
            this.diagramToolStripMenuItem.Click += new System.EventHandler(this.diagramToolStripMenuItem_Click);
            // 
            // checklistToolStripMenuItem
            // 
            this.checklistToolStripMenuItem.Name = "checklistToolStripMenuItem";
            this.checklistToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.checklistToolStripMenuItem.Text = "Checklist";
            this.checklistToolStripMenuItem.Click += new System.EventHandler(this.checklistToolStripMenuItem_Click);
            // 
            // abourToolStripMenuItem
            // 
            this.abourToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.abourToolStripMenuItem.Name = "abourToolStripMenuItem";
            this.abourToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.abourToolStripMenuItem.Text = "About";
            this.abourToolStripMenuItem.Click += new System.EventHandler(this.abourToolStripMenuItem_Click);
            // 
            // mdi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(998, 614);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "mdi";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ATree: planner";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem diagramToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem checklistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abourToolStripMenuItem;
    }
}