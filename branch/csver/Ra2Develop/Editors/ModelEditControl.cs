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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Ra2Develop.Editors.EditableObjects;
using R3D.Design;
using System.Windows.Forms.Design;
using Ra2Develop.Converters;
using R3D.IO;

namespace Ra2Develop.Editors
{
    public partial class ModelEditControl : UserControl, IEditControl<EditableModel>
    {
        public ModelEditControl()
        {
            InitializeComponent();
        }

        EditableModel value;
        IWindowsFormsEditorService service;
        #region IEditControl<EditableModel> 成员

        public EditableModel Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public IWindowsFormsEditorService Service
        {
            get
            {
                return service;
            }
            set
            {
                service = value;
            }
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            ConverterBase[] convs = ConverterManager.Instance.GetConvertersDest(".mesh");
            string[] subFilters = new string[convs.Length + 1];
            for (int i = 0; i < convs.Length; i++)
            {
                subFilters[i] = DevUtils.GetFilter(convs[i].SourceDesc, convs[i].SourceExt);
            }
            subFilters[convs.Length] = DevUtils.GetFilter(Program.StringTable["DOCS:MeshDesc"], new string[] { ".mesh" });

            openFileDialog1.Filter = DevUtils.GetFilter(subFilters);

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog1.FilterIndex == subFilters.Length)
                {
                    value = EditableModel.FromFile(new DevFileLocation(openFileDialog1.FileName));
                }
                else
                {
                    ConverterBase con = convs[openFileDialog1.FilterIndex - 1];

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(65536 * 4);
                    con.Convert(new DevFileLocation(openFileDialog1.FileName), new StreamedLocation(new VirtualStream(ms, 0)));
                    ms.Position = 0;

                    value = EditableModel.FromFile(new StreamedLocation(ms));
                }
                //// EditableModel.ImportFromXml(openFileDialog1.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (value != null)
            {
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    value.ImportEntityFromXml(openFileDialog1.FileName);
                }
            }
            else
            {
                button1_Click(sender, e);
            }
        }
    }
}
