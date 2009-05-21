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
using Ra2Develop.Designers;
using Ra2Develop.Editors.EditableObjects;
using R3D.Design;
using R3D.GraphicsEngine;
using R3D.IsoMap;
using R3D.UI;

namespace Ra2Develop.Editors
{
    public partial class MeshArrayEditControl : UserControl, IEditControl<EditableMesh[]>
    {
        public MeshArrayEditControl()
        {
            InitializeComponent();
            LanguageParser.ParseLanguage(Program.StringTable, this);
        }

        #region IEditControl<EditableMesh[]> 成员
        EditableMesh[] value;
        IWindowsFormsEditorService service;

        public EditableMesh[] Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
                lengthLabel.Text = Program.StringTable["PROP:TMP3MDLCOUNT"] + ' ';
                if (value != null)
                {
                    lengthLabel.Text += value.Length.ToString();
                }
                else
                {
                    lengthLabel.Text += 0.ToString();
                }

            }
        }

        public IWindowsFormsEditorService Service
        {
            get { return service; }
            set { service = value; }
        }

        #endregion


        /// <summary>
        /// Add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (value == null)
            {
                value = new EditableMesh[1] { new EditableMesh() };
                //BlockModel.Data mdl = new BlockModel.Data();

                //value = new List<GameMesh.Data>();
                //value.Add(new GameMesh.Data(GraphicsDevice.Instance.Device));
            }
            else
            {
                EditableMesh[] res = new EditableMesh[value.Length + 1];

                Array.Copy(value, res, value.Length);

                res[value.Length] = new EditableMesh();

                value = res;
                //value.Add(new GameMesh.Data(GraphicsDevice.Instance.Device));
                //value = new EditableBlockModel[1];
                //BlockModel d = new BlockModel();
                //d.Data = new GameModel.Data();
                //value[0] = new EditableBlockModel(d);
            }
            service.CloseDropDown();
            service = null;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int[] res;
            if (ArrayView<EditableMesh>.ShowDialog(Program.StringTable["MSG:RemoveTileModel"], value, out res) == DialogResult.OK)
            {
                bool[] isDel = new bool[value.Length];

                for (int i = 0; i < res.Length; i++)
                {
                    isDel[res[i]] = true;
                }

                List<EditableMesh> list = new List<EditableMesh>(value.Length);
                for (int i = 0; i < value.Length; i++)
                {
                    if (!isDel[i])
                    {
                        list.Add(value[i]);
                    }
                }

                //for (int i = res.Length - 1; i >= 0; i--)
                //{
                //    value.RemoveAt(res[i]);
                //}
                value = list.ToArray();


                service.CloseDropDown();
                service = null;
            }
        }
    }
}
