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
using System.Text;
using System.Windows.Forms;
using Ra2Develop.Editors;
using R3D.UI;
using R3D.GraphicsEngine;
using Ra2Develop.Designers;
using R3D.IsoMap;
using Ra2Develop.Editors.EditableObjects;

namespace Ra2Develop.Converters
{
    public partial class TmpConvDlg : Form
    {
    
        //EditableBlockMaterial mate;
        Tmp2Tile3DConverter.MaterialParams mateParam;

        public TmpConvDlg()
        {
            InitializeComponent();
            this.DialogResult = DialogResult.Cancel;

            LanguageParser.ParseLanguage(Program.StringTable, this);

            textBox2.Text = Tile3DDocument.Extension;

            mateParam = new Tmp2Tile3DConverter.MaterialParams();

            propertyGrid1.SelectedObject = mateParam;
        }

        public string FileExtension
        {
            get { return textBox2.Text; }
        }
        public Tmp2Tile3DConverter.MaterialParams MaterialParameters
        {
            get { return mateParam; }
        }

        public string PaletteFile
        {
            get { return textBox1.Text; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {            
            this.Close();
        }
    }
}
