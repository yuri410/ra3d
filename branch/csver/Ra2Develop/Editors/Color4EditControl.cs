﻿/*
 * Copyright (C) 2008 R3D Development Team
 * 
 * R3D is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * R3D is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with R3D.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using R3D.Design;
using R3D.UI;
using SlimDX;

namespace Ra2Develop.Editors
{
    public partial class Color4EditControl : UserControl, IEditControl<Color4>
    {
        public Color4EditControl()
        {
            InitializeComponent();
            LanguageParser.ParseLanguage(Program.StringTable, this);

            redBar.ValueChanged += this.refreshPreview;
            alphaBar.ValueChanged += this.refreshPreview;
            greenBar.ValueChanged += this.refreshPreview;
            blueBar.ValueChanged += this.refreshPreview;

        }
        Color4 value;
        //IWindowsFormsEditorService service;

        #region IEditControl<Color4> 成员

        public Color4 Value
        {
            get { return value; }
            set
            {
                this.value = value;

                alphaBar.Value = (int)(value.Alpha * 255);
                redBar.Value = (int)(value.Red * 255);
                greenBar.Value = (int)(value.Green * 255);
                blueBar.Value = (int)(value.Blue * 255);

            }
        }

        public IWindowsFormsEditorService Service
        {
            get { return null; }
            set {  }
        }

        #endregion

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.Clear(Color.White);


            Size cs = pictureBox1.ClientSize;
            Bitmap bmp = Ra2Develop.Properties.Resources.gird;

            for (int y = 0; y < cs.Height; y += bmp.Height)
            {
                for (int x = 0; x < cs.Width; x += bmp.Width)
                {
                    g.DrawImageUnscaled(bmp, x, y);
                }
            }

            SolidBrush brush = new SolidBrush(value.ToColor());
            g.FillRectangle(brush, new Rectangle(Point.Empty, pictureBox1.ClientSize));

            brush.Dispose();
        }

        private void redBar_ValueChanged(object sender, EventArgs e)
        {
            value.Red = redBar.Value / 255f;
            redLabel.Text = redBar.Value.ToString();
            if (checkBox1.Checked)
            {
                greenBar.Value = redBar.Value;
                blueBar.Value = redBar.Value;
            }
        }

        private void alphaBar_ValueChanged(object sender, EventArgs e)
        {
            value.Alpha = alphaBar.Value / 255f;
            alphaLabel.Text = alphaBar.Value.ToString();
        }

        private void greenBar_ValueChanged(object sender, EventArgs e)
        {
            value.Green = greenBar.Value / 255f;
            greenLabel.Text = greenBar.Value.ToString();
            if (checkBox1.Checked)
            {
                redBar.Value = greenBar.Value;
                blueBar.Value = greenBar.Value;
            }           
        }

        private void blueBar_ValueChanged(object sender, EventArgs e)
        {
            value.Blue = blueBar.Value / 255f;
            blueLabel.Text = blueBar.Value.ToString();
            if (checkBox1.Checked)
            {
                redBar.Value = blueBar.Value;
                greenBar.Value = blueBar.Value;
            }            
        }

        private void refreshPreview(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }


    }
}
