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
using R3D.Design;
using SlimDX.Direct3D9;
using System.Windows.Forms.Design;
using R3D.UI;

namespace Ra2Develop.Editors
{
    public partial class VertexFormatEditorControl : UserControl, IEditControl<VertexFormat>
    {
        public VertexFormatEditorControl()
        {
            InitializeComponent();
            LanguageParser.ParseLanguage(Program.StringTable, this);
        }


        #region IEditControl<VertexFormat> 成员

        public VertexFormat Value
        {
            get
            {
                VertexFormat res = VertexFormat.Position;
                if (checkBox2.Checked)                
                {
                    res |= VertexFormat.Normal;
                }
                if (radioButton1.Checked)
                {
                    res |= VertexFormat.Texture1;
                }
                else if (radioButton2.Checked)                
                {
                    res |= VertexFormat.Texture2;
                }
                else if (radioButton3.Checked)
                {
                    res |= VertexFormat.Texture3;
                }
                else if (radioButton4.Checked)
                {
                    res |= VertexFormat.Texture4;
                }
                return res;
            }
            set
            {
                if ((value & VertexFormat.Normal) == VertexFormat.Normal)                
                {
                    checkBox2.Checked = true;
                }

                if ((value & VertexFormat.Texture1) == VertexFormat.Texture1)
                {
                    radioButton1.Checked = true;
                }
                else if ((value & VertexFormat.Texture2) == VertexFormat.Texture2)
                {
                    radioButton2.Checked = true;
                }
                else if ((value & VertexFormat.Texture3) == VertexFormat.Texture3)
                {
                    radioButton3.Checked = true;
                }
                else if ((value & VertexFormat.Texture4) == VertexFormat.Texture4)
                {
                    radioButton4.Checked = true;
                }
            }
        }

        public IWindowsFormsEditorService Service
        {
            get;
            set;
        }

        #endregion
    }
}
