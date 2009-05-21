namespace Ra2Develop.Designers
{
    partial class VertexElementDlg
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
            this.posCB = new System.Windows.Forms.CheckBox();
            this.nCB = new System.Windows.Forms.CheckBox();
            this.tex1CB = new System.Windows.Forms.CheckBox();
            this.tex2CB = new System.Windows.Forms.CheckBox();
            this.tex3CB = new System.Windows.Forms.CheckBox();
            this.tex4CB = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // posCB
            // 
            this.posCB.AutoSize = true;
            this.posCB.Checked = true;
            this.posCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.posCB, 4);
            this.posCB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.posCB.Location = new System.Drawing.Point(8, 8);
            this.posCB.Name = "posCB";
            this.posCB.Size = new System.Drawing.Size(276, 24);
            this.posCB.TabIndex = 0;
            this.posCB.Text = "GUI:VertexPosition";
            this.posCB.UseVisualStyleBackColor = true;
            // 
            // nCB
            // 
            this.nCB.AutoSize = true;
            this.nCB.Checked = true;
            this.nCB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.nCB, 4);
            this.nCB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nCB.Location = new System.Drawing.Point(8, 38);
            this.nCB.Name = "nCB";
            this.nCB.Size = new System.Drawing.Size(276, 24);
            this.nCB.TabIndex = 1;
            this.nCB.Text = "GUI:VertexNormal";
            this.nCB.UseVisualStyleBackColor = true;
            // 
            // tex1CB
            // 
            this.tex1CB.AutoSize = true;
            this.tex1CB.Checked = true;
            this.tex1CB.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tableLayoutPanel1.SetColumnSpan(this.tex1CB, 4);
            this.tex1CB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tex1CB.Location = new System.Drawing.Point(8, 68);
            this.tex1CB.Name = "tex1CB";
            this.tex1CB.Size = new System.Drawing.Size(276, 24);
            this.tex1CB.TabIndex = 2;
            this.tex1CB.Text = "GUI:VertexTex1";
            this.tex1CB.UseVisualStyleBackColor = true;
            // 
            // tex2CB
            // 
            this.tex2CB.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.tex2CB, 4);
            this.tex2CB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tex2CB.Location = new System.Drawing.Point(8, 98);
            this.tex2CB.Name = "tex2CB";
            this.tex2CB.Size = new System.Drawing.Size(276, 24);
            this.tex2CB.TabIndex = 3;
            this.tex2CB.Text = "GUI:VertexTex2";
            this.tex2CB.UseVisualStyleBackColor = true;
            // 
            // tex3CB
            // 
            this.tex3CB.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.tex3CB, 4);
            this.tex3CB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tex3CB.Location = new System.Drawing.Point(8, 128);
            this.tex3CB.Name = "tex3CB";
            this.tex3CB.Size = new System.Drawing.Size(276, 24);
            this.tex3CB.TabIndex = 4;
            this.tex3CB.Text = "GUI:VertexTex3";
            this.tex3CB.UseVisualStyleBackColor = true;
            // 
            // tex4CB
            // 
            this.tex4CB.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.tex4CB, 4);
            this.tex4CB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tex4CB.Location = new System.Drawing.Point(8, 158);
            this.tex4CB.Name = "tex4CB";
            this.tex4CB.Size = new System.Drawing.Size(276, 24);
            this.tex4CB.TabIndex = 5;
            this.tex4CB.Text = "GUI:VertexTex4";
            this.tex4CB.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.Controls.Add(this.posCB, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tex4CB, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.nCB, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tex3CB, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.tex1CB, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.tex2CB, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.button1, 2, 8);
            this.tableLayoutPanel1.Controls.Add(this.button2, 4, 8);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(292, 227);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(135, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(69, 24);
            this.button1.TabIndex = 6;
            this.button1.Text = "GUI:OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button2.Location = new System.Drawing.Point(215, 193);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(69, 24);
            this.button2.TabIndex = 7;
            this.button2.Text = "GUI:Cancel";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // VertexElementDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 227);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VertexElementDlg";
            this.Text = "GUI:VertexElementDlg";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox posCB;
        private System.Windows.Forms.CheckBox nCB;
        private System.Windows.Forms.CheckBox tex1CB;
        private System.Windows.Forms.CheckBox tex2CB;
        private System.Windows.Forms.CheckBox tex3CB;
        private System.Windows.Forms.CheckBox tex4CB;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}