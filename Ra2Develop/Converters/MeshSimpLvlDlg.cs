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
using SlimDX.Direct3D9;

namespace Ra2Develop.Converters
{
    public partial class MeshSimpLvlDlg : Form
    {
        class Parameters
        {
            public Parameters()
            {
                levelStep = 0.1f;
                levelStart = 0.2f;
                Position = 1f;
                Normal = 1f;
                Boundary = 1f;
            }

            //AttributeWeights weights;
            float levelStep;
            float levelStart;

            [LocalizedCategory("PROP:MeshSimpLevel")]
            public float LevelStep
            {
                get { return levelStep; }
                set
                {
                    if (LevelStart + value * (MeshSimpDlg.SimplificationLevels - 1) > 1f + 0.00001)
                    {
                        throw new ArgumentOutOfRangeException(Program.StringTable["MSG:InvalidSimpParam"]);
                    }
                    levelStep = value;
                }
            }
            [LocalizedCategory("PROP:MeshSimpLevel")]
            public float LevelStart
            {
                get { return levelStart; }
                set
                {
                    if (value + LevelStep * (MeshSimpDlg.SimplificationLevels - 1) > 1f + 0.00001)
                    {
                        throw new ArgumentOutOfRangeException(Program.StringTable["MSG:InvalidSimpParam"]);
                    }
                    levelStart = value;
                }
            }

            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float Binormal { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float Boundary { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float Diffuse { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float Normal { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float Position { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float Specular { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float Tangent { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float TextureCoordinate1 { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float TextureCoordinate2 { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float TextureCoordinate3 { get; set; }
            [LocalizedCategory("PROP:MeshSimpWeights")]
            public float TextureCoordinate4 { get; set; }

        }


        Parameters param;

        public float LevelStep
        {
            get { return param.LevelStep; }
            //set { param.LevelStep = value; }
        }
        public float LevelStart
        {
            get { return param.LevelStart; }
           // set { param.LevelStart = value; }
        }

        public AttributeWeights Weights
        {
            get
            {
                AttributeWeights aw = new AttributeWeights();

                aw.Binormal = param.Binormal;
                aw.Boundary = param.Boundary;
                aw.Diffuse = param.Diffuse;
                aw.Normal = param.Normal;
                aw.Position = param.Position;
                aw.Specular = param.Specular;
                aw.Tangent = param.Tangent;
                aw.TextureCoordinate1 = param.TextureCoordinate1;
                aw.TextureCoordinate2 = param.TextureCoordinate2;
                aw.TextureCoordinate3 = param.TextureCoordinate3;
                aw.TextureCoordinate4 = param.TextureCoordinate4;
                return aw;
            }
        }

        public MeshSimpLvlDlg()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
            param = new Parameters();
            propertyGrid1.SelectedObject = param;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
