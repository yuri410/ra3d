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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using R3D.GraphicsEngine;
using R3D.MathLib;
using R3D.UI;
using Ra2Develop.Designers;
using Ra2Develop.Editors.EditableObjects;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Develop.Converters
{
    public partial class MeshSimpDlg : Form
    {
        public const int SimplificationLevels = 8;
        const float StartPec = 1.025f;

        Point lastPos;
        EditableMesh meshes;

        EditableMesh[] levels;
        EditableMesh selectedMesh;
        //EditableMesh simpMesh;

        View3D view3D;

        FillMode fillMode = FillMode.Solid;




        FillMode FillMode
        {
            get { return fillMode; }
            set
            {
                fillMode = value;
                Draw3D();
            }
        }

        bool isPorcessing;

        string GetPercentageString(float p)
        {           
            return p.ToString("P02");
        }
        //float GetPercentage(int i)
        //{
        //    return stepPec * (SimplificationLevels - i - 1) + startPec;
        //}

        EditableMesh Simplifize(MeshSimplifier ms, out int[] map, out int[] permutation)
        {
            EditableMesh simpMesh = meshes.Clone();

            Vector3[] positions = simpMesh.Positions;
            Vector3[] normals = simpMesh.Normals;
            Vector2[] tex1 = simpMesh.Texture1;
            Vector2[] tex2 = simpMesh.Texture2;
            Vector2[] tex3 = simpMesh.Texture3;
            Vector2[] tex4 = simpMesh.Texture4;

            MeshFace[] faces = simpMesh.Faces;

            ms.ProgressiveMesh(positions, faces, out map, out permutation);


            Vector3[] tempPos = new Vector3[positions.Length];
            Vector3[] tempN = new Vector3[normals.Length];
            Vector2[] tempTex1 = new Vector2[tex1.Length];
            Vector2[] tempTex2 = null;
            if (tex2 != null)
                tempTex2 = new Vector2[tex2.Length];
            Vector2[] tempTex3 = null;
            if (tex3 != null)
                tempTex3 = new Vector2[tex3.Length];
            Vector2[] tempTex4 = null;
            if (tex4 != null)
                tempTex4 = new Vector2[tex4.Length];

            Array.Copy(positions, tempPos, positions.Length);
            Array.Copy(normals, tempN, normals.Length);
            Array.Copy(tex1, tempTex1, tex1.Length);

            for (int i = 0; i < positions.Length; i++)
            {
                positions[permutation[i]] = tempPos[i];
                normals[permutation[i]] = tempN[i];
                tex1[permutation[i]] = tempTex1[i];
                if (tex2 != null)
                    tex2[permutation[i]] = tempTex2[i];
                if (tex3 != null)
                    tex3[permutation[i]] = tempTex3[i];
                if (tex4 != null)
                    tex4[permutation[i]] = tempTex4[i];
            }
            // update the changes in the entries in the triangle list
            for (int i = 0; i < faces.Length; i++)
            {
                faces[i].IndexA = permutation[faces[i].IndexA];
                faces[i].IndexB = permutation[faces[i].IndexB];
                faces[i].IndexC = permutation[faces[i].IndexC];
            }
            return simpMesh;
        }

        void BeginPorcessing()
        {
            isPorcessing = true;

            statusLabel.Text = Program.StringTable["STATUS:SimplifingMesh"] + meshes.Name;

            if (levels == null)
            {
                levels = new EditableMesh[SimplificationLevels];
            }
            else
            {
                for (int i = 0; i < SimplificationLevels; i++)
                {
                    levels[i].Dispose(false);
                }
            }

            slvlCombo.Items.Clear();
            slvlCombo.Enabled = false;
        }
        void EndPorcessing()
        {
            slvlCombo.Items.Add(Program.StringTable["GUI:OrgModel"]);
            slvlCombo.SelectedIndex = slvlCombo.Items.Count - 1;
            slvlCombo.Enabled = true;

            statusLabel.Text = Program.StringTable["Status:Ready"];

            isPorcessing = false;
        }
        void Simplifize2()
        {
            BeginPorcessing();
            int maxVtxCount = meshes.Positions.Length;

            simProgBar.Maximum = 2 * SimplificationLevels + 1;
            simProgBar.Value = 0;

            Application.DoEvents();
            int[] map;
            int[] permutation;

            MeshSimplifier ms = new MeshSimplifier();

            EditableMesh simpMesh = Simplifize(ms, out map, out permutation);
            simProgBar.Value++;
            Application.DoEvents();


            float percentage = StartPec;
            for (int i = 0; i < SimplificationLevels; i++)
            {
                if (!isPorcessing)
                {
                    simpMesh.Dispose(false);
                    return;
                }
                percentage -= 1f / (float)SimplificationLevels;
                if (percentage < 0)
                    percentage = 0;

                levels[i] = simpMesh.Clone();
                simProgBar.Value++;
                Application.DoEvents();

                levels[i].Simplify(ms, map, (int)(maxVtxCount * percentage));

                simProgBar.Value++;
                Application.DoEvents();

                slvlCombo.Items.Add(Program.StringTable["GUI:SimplificationLvl"] + GetPercentageString(percentage));

            }

            simpMesh.Dispose(false);

            EndPorcessing();
        }
        void SimplifizeAll()
        {
            SimplifizeAll(null);
        }
        void SimplifizeAll(AttributeWeights[] weight)
        {
            BeginPorcessing();


            simProgBar.Maximum = 2 * SimplificationLevels + 1;
            simProgBar.Value = 0;

            Application.DoEvents();
            SimplificationMesh sm;
            if (weight == null)
                sm = meshes.GetSimplificationMesh();
            else
                sm = meshes.GetSimplificationMesh(weight);
            simProgBar.Value++;
            Application.DoEvents();

            float percentage = StartPec;

            for (int i = 0; i < SimplificationLevels; i++)
            {
                if (!isPorcessing)
                {
                    return;
                }

                percentage -= 1f / (float)SimplificationLevels;
                if (percentage < 0)
                    percentage = 0;


                sm.ReduceFaces((int)(sm.MaximumFaceCount * percentage));
                //sm.ReduceVertices((int)(sm.MaximumVertexCount * percentage));

                simProgBar.Value++;
                Application.DoEvents();

                Mesh lvl = sm.Clone(GraphicsDevice.Instance.Device, sm.CreationOptions, sm.VertexFormat);                
                levels[i] = new EditableMesh(meshes.Name, lvl, meshes.Materials);
                //lvl.Dispose();

                simProgBar.Value++;
                Application.DoEvents();

                slvlCombo.Items.Add(Program.StringTable["GUI:SimplificationLvl"] + GetPercentageString(percentage));
            }
            sm.Dispose();


            EndPorcessing();
        }

        public MeshSimpDlg(EditableMesh mesh)
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(Program.StringTable, this);
            LanguageParser.ParseLanguage(Program.StringTable, toolStrip1);

            meshes = mesh;

            view3D = new View3D(
                ModelDocumentConfigs.Instance.EyeAngleX,
                ModelDocumentConfigs.Instance.EyeAngleY,
                ModelDocumentConfigs.Instance.EyeDistance,
                ModelDocumentConfigs.Instance.Fovy);
            view3D.ViewChanged += Draw3D;
            fillModeTool.Text = solidMenuItem.Text;
        }

        public EditableMesh[] SimplifiedMeshes
        {
            get { return levels; }
        }
        public EditableMesh SelectedMesh
        {
            get { return selectedMesh; }
        }

        private void V2MPrevDlg_MouseMove(object sender, MouseEventArgs e)
        {
            Point loc = e.Location;
            Point offset = new Point();
            offset.X = loc.X - lastPos.X;
            offset.Y = loc.Y - lastPos.Y;

            if (e.Button == MouseButtons.Left)
            {
                view3D.EyeAngleX += MathEx.PIf * (float)offset.X / 180f;
                view3D.EyeAngleY += MathEx.PIf * (float)offset.Y / 180f;

                lastPos = loc;
            }
            if (e.Button == MouseButtons.Right)
            {
                view3D.EyeDistance += offset.Y;
                lastPos = loc;
            }
        }


        private void V2MPrevDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            isPorcessing = false;

            for (int i = 0; i < SimplificationLevels; i++)
            {
                if (levels[i] != selectedMesh)
                {
                    levels[i].Dispose(false);
                }
                levels[i] = null;
            }
            if (selectedMesh != meshes)
            {
                meshes.Dispose(false);
                meshes = null;
            }
            levels = null;
            GraphicsDevice.Instance.Free(this);
        }

        public EditableMesh Mesh
        {
            get { return meshes; }
        }

        void Draw3D()
        {
            GraphicsDevice.Instance.BeginScene(this);

            Device dev = GraphicsDevice.Instance.Device;

            dev.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkBlue, 1, 0);
            dev.BeginScene();


            dev.SetRenderState<FillMode>(RenderState.FillMode, fillMode);
            dev.SetRenderState(RenderState.AlphaBlendEnable, true);
            dev.SetRenderState<Cull>(RenderState.CullMode, Cull.None);
            dev.SetRenderState(RenderState.SpecularEnable, true);
            dev.SetRenderState<ColorSource>(RenderState.SpecularMaterialSource, ColorSource.Material);
            dev.SetRenderState(RenderState.Lighting, true);

            if (!isPorcessing)
            {
                Light light = new Light();
                light.Type = LightType.Point;
                light.Position = ModelDocumentConfigs.Instance.LightPosition;
                light.Range = float.MaxValue;
                //light.Falloff = 0;
                light.Ambient = ModelDocumentConfigs.Instance.LightAmbient;
                light.Diffuse = ModelDocumentConfigs.Instance.LightDiffuse;
                light.Specular = ModelDocumentConfigs.Instance.LightSpecular;
                light.Attenuation0 = 1.0f;

                dev.EnableLight(0, true);

                dev.SetLight(0, light);

                view3D.SetTransform(dev, this);


                dev.SetTransform(TransformState.World, Matrix.Translation(-1f * 12f, 1f * 12f, 0));
                meshes.Render();


                for (int i = 0; i < SimplificationLevels; i++)
                {
                    int key = i + 1;

                    dev.SetTransform(TransformState.World, Matrix.Translation(((float)(key % 3) - 1f) * 12f, (-(float)(key / 3) + 1f) * 12f, 0));
                    levels[i].Render();
                }



                Sprite spr = GraphicsDevice.Instance.GetSprite;
                spr.Begin(SpriteFlags.DoNotSaveState | SpriteFlags.AlphaBlend);
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:EyeDistance"] + view3D.EyeDistance.ToString(), 5, toolStrip1.Height + 5, Color.White);

                Vector3 pos;
                Vector3 res;
                float percentage = StartPec;

                for (int i = 0; i < SimplificationLevels; i++)
                {
                    int key = i + 1;
                    pos = new Vector3(((float)(key % 3) - 1f) * 12f, (-(float)(key / 3) + 1f) * 12f, 0);
                    pos.Y -= 1.5f;

                    //Viewport vp = new Viewport(0, 0, ClientSize.Width, ClientSize.Height);
                    res = Vector3.Project(pos, dev.Viewport, view3D.Projection, view3D.View, Matrix.Identity);
                    percentage -= 1f / (float)SimplificationLevels;
                    if (percentage < 0)
                        percentage = 0;

                    GraphicsDevice.Instance.DefaultFont.DrawString(spr, GetPercentageString(percentage), (int)res.X, (int)res.Y, Color.White);
                }

                pos = new Vector3(-1f * 12f, 1f * 12f - 1.5f, 0);
                res = Vector3.Project(pos, dev.Viewport, view3D.Projection, view3D.View, Matrix.Identity);
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:OrgModel"], (int)res.X, (int)res.Y, Color.White);


                spr.End();
            }

            dev.EndScene();
            GraphicsDevice.Instance.EndScene();
        }

        private void V2MPrevDlg_Paint(object sender, PaintEventArgs e)
        {
            Draw3D();
        }


        private void slvlCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (slvlCombo.SelectedIndex != -1)
            {
                if (slvlCombo.SelectedIndex != slvlCombo.Items.Count - 1)
                {
                    selectedMesh = levels[slvlCombo.SelectedIndex];
                }
                else
                {
                    selectedMesh = meshes;
                }
            }
            else
            {
                selectedMesh = meshes;
            }
        }

        private void wireframeMenuItem_Click(object sender, EventArgs e)
        {
            fillModeTool.Text = wireframeMenuItem.Text;
            FillMode = FillMode.Wireframe;
        }

        private void solidMenuItem_Click(object sender, EventArgs e)
        {
            fillModeTool.Text = solidMenuItem.Text;
            FillMode = FillMode.Solid;
        }

        private void V2MPrevDlg_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right || e.Button == MouseButtons.Left)
                lastPos = e.Location;
        }

        private void V2MPrevDlg_Load(object sender, EventArgs e)
        {
            Show();
            SimplifizeAll();
        }

        private void reSimTool_Click(object sender, EventArgs e)
        {
            MeshSimpLvlDlg dlg = new MeshSimpLvlDlg();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                AttributeWeights[] weights = new AttributeWeights[meshes.Positions.Length];
                AttributeWeights aw = dlg.Weights;
                for (int i = 0; i < weights.Length; i++)
                {
                    weights[i] = aw;
                }

                SimplifizeAll(weights);
            }

            dlg.Dispose();
        }



    }
}
