/*
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
using Ra2Develop.Designers;
using R3D;
using R3D.Design;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.Media;
using R3D.UI;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Develop.Editors
{
    public partial class TextureEditControl : UserControl, IEditControl<Texture>
    {
        public TextureEditControl()
        {
            InitializeComponent();
            LanguageParser.ParseLanguage(Program.StringTable, this);
        }

        Texture image;
        IWindowsFormsEditorService service;
        #region IEditControl<Texture> 成员

        public unsafe Texture Value
        {
            get
            {
                return image;
            }
            set
            {
                image = value;


                widthLabel.Text = Program.StringTable["PROP:IMGWIDTH"];
                heightLabel.Text = Program.StringTable["PROP:IMGHEIGHT"];
                formatLabel.Text = Program.StringTable["PROP:IMGPixFmt"];

                if (value != null)
                {
                    try
                    {
                        //RawImage rawImg = RawImage.FromTexture (tex
                        Bitmap bmp = Utils.Texture2Bitmap(value);
                        pictureBox1.Image = bmp;
                    }
                    catch (NotSupportedException)
                    {
                        pictureBox1.Image = null;
                    }
                    finally
                    {
                        SurfaceDescription desc = value.GetLevelDescription(0);
                        widthLabel.Text += desc.Width.ToString();
                        heightLabel.Text += desc.Height.ToString();
                        formatLabel.Text += desc.Format.ToString();
                    }
                    
                }
            }
        }

        public IWindowsFormsEditorService Service
        {
            get { return service; }
            set { service = value; }
        }

        #endregion

        private unsafe void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowReadOnly = false;
            openFileDialog1.Filter = TextureEditor.GetFilter(); // ImageManager.Instance.GetFilter();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (image != null)
                {
                    image.Dispose();
                }



                //image = Texture.FromFile(GraphicsDevice.Instance.Device, openFileDialog1.FileName, Usage.None, Pool.Managed);
                ImageBase img = ImageManager.Instance.CreateInstaceUnmanaged(new DevFileLocation(openFileDialog1.FileName));

                image = img.GetTexture(GraphicsDevice.Instance.Device, Usage.None, Pool.Managed);

                img.Dispose();
            }
            service.CloseDropDown();
            service = null;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (image == null)
            {
                e.Graphics.Clear(SystemColors.Window);

                Pen cross = new Pen(Color.Red);                

                Size cs = pictureBox1.ClientSize;
                e.Graphics.DrawLine(cross, Point.Empty, new Point(cs));
                e.Graphics.DrawLine(cross, new Point(cs.Width, 0), new Point(0, cs.Height));

                cross.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            image = null;
            service.CloseDropDown();
            service = null;
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
