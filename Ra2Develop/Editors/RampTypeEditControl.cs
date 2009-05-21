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
using System.Windows.Forms.Design;
using R3D.Design;

namespace Ra2Develop.Editors
{
    partial class RampTypeEditControl : UserControl, IEditControl<byte>
    {
        byte value;
        IWindowsFormsEditorService service;
        public RampTypeEditControl()
        {
            InitializeComponent();
        }

        

        #region IEditControl<byte> 成员

        public byte Value
        {
            get { return value; }
            set
            {
                this.value = value;

                if (listView1.SelectedItems.Count > 0)
                {
                    listView1.SelectedItems[0].Selected = false;
                }
                listView1.Items[value].Selected = true;
            }
        }

        public IWindowsFormsEditorService Service
        {
            get { return service; }
            set { service = value; }
        }

        #endregion

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                value = (byte)listView1.SelectedIndices[0];
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            service.CloseDropDown();
            service = null;
        }

    }
}
