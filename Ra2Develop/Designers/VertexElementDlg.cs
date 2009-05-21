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
using Ra2Develop.Editors.EditableObjects;

namespace Ra2Develop.Designers
{
    public partial class VertexElementDlg : Form
    {
        EditableMesh.MeshVertexElement value;

        public VertexElementDlg()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }
        public VertexElementDlg(EditableMesh.MeshVertexElement v)
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            value = v;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            value = EditableMesh.MeshVertexElement.None;
            if (posCB.Checked)
            {
                value |= EditableMesh.MeshVertexElement.Position;
            }
            if (nCB.Checked)
            {
                value |= EditableMesh.MeshVertexElement.Normal;
            }
            if (tex1CB.Checked)
            {
                value |= EditableMesh.MeshVertexElement.Tex1;
            }
            if (tex2CB.Checked)
            {
                value |= EditableMesh.MeshVertexElement.Tex2;
            }
            if (tex3CB.Checked)
            {
                value |= EditableMesh.MeshVertexElement.Tex3;
            }
            if (tex4CB.Checked)
            {
                value |= EditableMesh.MeshVertexElement.Tex4;
            }


            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        public EditableMesh.MeshVertexElement Elements
        {
            get { return value; }
            private set
            {
                posCB.Checked = (value & EditableMesh.MeshVertexElement.Position) != 0;
                nCB.Checked = (value & EditableMesh.MeshVertexElement.Normal) != 0;
                tex1CB.Checked = (value & EditableMesh.MeshVertexElement.Tex1) != 0;
                tex2CB.Checked = (value & EditableMesh.MeshVertexElement.Tex2) != 0;
                tex3CB.Checked = (value & EditableMesh.MeshVertexElement.Tex3) != 0;
                tex4CB.Checked = (value & EditableMesh.MeshVertexElement.Tex4) != 0;
            }
        }
    }
}
