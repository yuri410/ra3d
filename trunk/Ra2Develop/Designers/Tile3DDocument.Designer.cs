namespace Ra2Develop.Designers
{
    partial class Tile3DDocument
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tile3DDocument));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addTool = new System.Windows.Forms.ToolStripButton();
            this.removeTool = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.prevBlockTool = new System.Windows.Forms.ToolStripButton();
            this.nextBlockTool = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTool,
            this.removeTool,
            this.toolStripSeparator1,
            this.prevBlockTool,
            this.nextBlockTool});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(110, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addTool
            // 
            this.addTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addTool.Image = ((System.Drawing.Image)(resources.GetObject("addTool.Image")));
            this.addTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addTool.Name = "addTool";
            this.addTool.Size = new System.Drawing.Size(23, 22);
            this.addTool.Text = "GUI:AddBlock";
            this.addTool.Click += new System.EventHandler(this.addTool_Click);
            // 
            // removeTool
            // 
            this.removeTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.removeTool.Image = ((System.Drawing.Image)(resources.GetObject("removeTool.Image")));
            this.removeTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeTool.Name = "removeTool";
            this.removeTool.Size = new System.Drawing.Size(23, 22);
            this.removeTool.Text = "GUI:RemoveBlock";
            this.removeTool.Click += new System.EventHandler(this.removeTool_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // prevBlockTool
            // 
            this.prevBlockTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prevBlockTool.Image = ((System.Drawing.Image)(resources.GetObject("prevBlockTool.Image")));
            this.prevBlockTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prevBlockTool.Name = "prevBlockTool";
            this.prevBlockTool.Size = new System.Drawing.Size(23, 22);
            this.prevBlockTool.Text = "toolStripButton3";
            this.prevBlockTool.Click += new System.EventHandler(this.prevBlockTool_Click);
            // 
            // nextBlockTool
            // 
            this.nextBlockTool.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextBlockTool.Image = ((System.Drawing.Image)(resources.GetObject("nextBlockTool.Image")));
            this.nextBlockTool.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextBlockTool.Name = "nextBlockTool";
            this.nextBlockTool.Size = new System.Drawing.Size(23, 22);
            this.nextBlockTool.Text = "toolStripButton4";
            this.nextBlockTool.Click += new System.EventHandler(this.nextBlockTool_Click);
            // 
            // Tile3DDocument
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 331);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Tile3DDocument";
            this.TabText = "Tile3DDocument";
            this.Text = "Tile3DDocument";
            this.Load += new System.EventHandler(this.Tile3DDocument_Load);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Tile3DDocument_MouseUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Tile3DDocument_MouseClick);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Tile3DDocument_FormClosed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Tile3DDocument_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Tile3DDocument_MouseMove);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton addTool;
        private System.Windows.Forms.ToolStripButton removeTool;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton prevBlockTool;
        private System.Windows.Forms.ToolStripButton nextBlockTool;
    }
}