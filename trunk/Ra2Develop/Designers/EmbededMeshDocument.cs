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
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ra2Develop.Converters;
using Ra2Develop.Editors.EditableObjects;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.MathLib;
using R3D.UI;
using SlimDX;
using SlimDX.Direct3D9;
using WeifenLuo.WinFormsUI.Docking;

namespace Ra2Develop.Designers
{
    public partial class EmbeddedMeshDocument : DocumentBase
    {
        EditableMesh content;

        //Light light;

        View3D view3D;

        Point lastPos;
        FillMode fillMode = FillMode.Solid;

        public EmbeddedMeshDocument(EditableMesh data)
        {
            InitializeComponent();


            LanguageParser.ParseLanguage(Program.StringTable, this);
            LanguageParser.ParseLanguage(Program.StringTable, toolStrip1);
            LanguageParser.ParseLanguage(Program.StringTable, toolStrip2);

            content = data;
            view3D = new View3D(
                ModelDocumentConfigs.Instance.EyeAngleX,
                ModelDocumentConfigs.Instance.EyeAngleY,
                ModelDocumentConfigs.Instance.EyeDistance,
                ModelDocumentConfigs.Instance.Fovy);
            view3D.ViewChanged += ViewChanged;

            this.MouseWheel += viewMouseWheel;

            if (content.Materials != null)
            {
                for (int i = 0; i < content.Materials.Length; i++)
                {
                    content.Materials[i].StateChanged = ViewChanged;
                }
            }
            MeshChanged();
        }

        public override ToolStrip[] ToolStrips
        {
            get
            {
                return new ToolStrip[] { view3D.ToolBar, toolStrip1, toolStrip2 };
            }
        }

        public override DocumentAbstractFactory Factory
        {
            get { return null; }
        }

        public override bool LoadRes() { return true; }

        public override bool SaveRes() { return true; }

        public override ResourceLocation ResourceLocation
        {
            get { return null; }
            set { }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override bool Saved
        {
            get { return true; }
            protected set { }
        }



        protected override void active()
        {
            //GraphicsDevice.Instance.Bind(this);
            //GraphicsDevice.Instance.MouseClick += viewMouseClick;
            //GraphicsDevice.Instance.MouseMove += viewMouseMove;
            //GraphicsDevice.Instance.MouseDown += viewMouseDown;
            //GraphicsDevice.Instance.MouseUp += viewMouseUp;

            if (propertyUpdated != null)
            {
                propertyUpdated(content, null);
            }
        }
        protected override void deactive()
        {
            //GraphicsDevice.Instance.MouseClick -= viewMouseClick;
            //GraphicsDevice.Instance.MouseMove -= viewMouseMove;
            //GraphicsDevice.Instance.MouseDown -= viewMouseDown;
            //GraphicsDevice.Instance.MouseUp -= viewMouseUp;
            //GraphicsDevice.Instance.Unbind(this);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Draw3D();
        }
        unsafe void Draw3D()
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
            dev.SetTransform(TransformState.World, Matrix.Identity);
            dev.SetLight(0, light);

            view3D.SetTransform(dev, this);

            //dev.SetTransform(TransformState.World, Matrix.Translation(light.Position));
            //dev.SetRenderState(RenderState.Lighting, false);
            //for (int i = 0; i < 4; i++)            
            //{
            //    dev.SetTexture(i, null);
            //}
            //GraphicsDevice.Instance.BallMesh.DrawSubset(0);
            //dev.SetRenderState(RenderState.Lighting, true);
            DevUtils.DrawLight(dev, light.Position);

            dev.SetTransform(TransformState.World, Matrix.Identity);
            
            //DevUtils.DrawEditMesh(dev, content);
            content.Render();

            Sprite spr = GraphicsDevice.Instance.GetSprite;

            spr.Begin(SpriteFlags.AlphaBlend | SpriteFlags.DoNotSaveState);

            Rectangle rect = GraphicsDevice.Instance.DefaultFont.MeasureString(spr, " ", DrawTextFormat.Left | DrawTextFormat.Top | DrawTextFormat.SingleLine);
            if (content.Positions != null)
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:MeshVtxCount"] + content.Positions.Length.ToString(), 5, 5, Color.White);
            }
            else
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:MeshVtxCount"] + 0.ToString(), 5, 5, Color.White);
            }
            if (content.Faces != null)
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:MeshFaceCount"] + content.Faces.Length.ToString(), 5, 10 + rect.Height, Color.White);
            }
            else
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:MeshFaceCount"] + 0.ToString(), 5, 5, Color.White);
            }
            if (content.Materials != null)
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:MaterialCount"] + content.Materials.Length.ToString(), 5, 15 + 2 * rect.Height, Color.White);
            }
            else
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:MaterialCount"] + 0.ToString(), 5, 5, Color.White);
            }
            spr.End();

            dev.EndScene();

            GraphicsDevice.Instance.EndScene();


        }

        void viewMouseWheel(object sender, MouseEventArgs e)
        {
            view3D.EyeDistance -= e.Delta * 0.02f;
        }

        void ViewChanged()
        {
            Draw3D();
        }
        void MeshChanged()        
        {
            if (propertyUpdated != null)
            {
                propertyUpdated(content, null);
            }
            ViewChanged();
        }
        private void importTool_Click(object sender, EventArgs e)
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
                EditableModel value;
                if (openFileDialog1.FilterIndex == convs.Length)
                {
                    value = EditableModel.FromFile(new DevFileLocation(openFileDialog1.FileName));
                }
                else
                {
                    ConverterBase con = convs[openFileDialog1.FilterIndex];

                    System.IO.MemoryStream ms = new System.IO.MemoryStream(65536 * 4);
                    con.Convert(new DevFileLocation(openFileDialog1.FileName), new StreamedLocation(new VirtualStream(ms, 0)));
                    ms.Position = 0;

                    value = EditableModel.FromFile(new StreamedLocation(ms));
                }
                content = value.Entities[0];
                MeshChanged();
            }
        }

        private void EmbededMeshDocument_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (content.Materials != null)
            {
                for (int i = 0; i < content.Materials.Length; i++)
                {
                    content.Materials[i].StateChanged = null;
                }
            }

            ModelDocumentConfigs.Instance.EyeAngleX = view3D.EyeAngleX;
            ModelDocumentConfigs.Instance.EyeAngleY = view3D.EyeAngleY;
            ModelDocumentConfigs.Instance.EyeDistance = view3D.EyeDistance;
            ModelDocumentConfigs.Instance.Fovy = view3D.Fovy;
            GraphicsDevice.Instance.Free(this);
            //ModelDocumentConfigs.Instance.LightAmbient = light.Ambient;
            //ModelDocumentConfigs.Instance.LightDiffuse = light.Diffuse;
            //ModelDocumentConfigs.Instance.LightPosition = 
        }

        private void optimizeTool_Click(object sender, EventArgs e)
        {
            
        }

        private void simplifyTool_Click(object sender, EventArgs e)
        {
            MeshSimpDlg prev = new MeshSimpDlg(content);

            prev.ShowDialog();

            if (content != prev.SelectedMesh)
            {
                content.Dispose(false);
                //content = prev.SelectedMesh;

                prev.SelectedMesh.CloneTo(content);

                prev.SelectedMesh.Dispose(false);
            }

            prev.Dispose();
        }

        private void normalTool_Click(object sender, EventArgs e)
        {
            content.ComputeNormals();
            MeshChanged();
        }

        private void flatNormalTool_ButtonClick(object sender, EventArgs e)
        {
            content.ComputeFlatNormal();
            MeshChanged();
        }

        private void EmbededMeshDocument_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void EmbededMeshDocument_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                lastPos = e.Location;


        }

        private void EmbededMeshDocument_MouseMove(object sender, MouseEventArgs e)
        {
            Point loc = e.Location;
            Point offset = new Point();
            offset.X = loc.X - lastPos.X;
            offset.Y = loc.Y - lastPos.Y;

            if (e.Button == MouseButtons.Right)
            {
                view3D.EyeAngleX += MathEx.PIf * (float)offset.X / 180f;
                view3D.EyeAngleY += MathEx.PIf * (float)offset.Y / 180f;

                lastPos = loc;
            }
        }

        private void EmbededMeshDocument_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void wireframeMenuItem_Click(object sender, EventArgs e)
        {
            fillMode = FillMode.Wireframe;
            fillModeTool.Text = wireframeMenuItem.Text;
            ViewChanged();
        }

        private void solidMenuItem_Click(object sender, EventArgs e)
        {
            fillMode = FillMode.Solid;
            fillModeTool.Text = solidMenuItem.Text;
            ViewChanged();
        }

        private void invNormalTool_Click(object sender, EventArgs e)
        {
            content.InverseNormals();
        }

        private void specialNormalTool_Click(object sender, EventArgs e)
        {
            content.InverseNormalX();
        }

        private void invNormalYTool_Click(object sender, EventArgs e)
        {
            content.InverseNormalY();
        }

        private void invNormalZTool_Click(object sender, EventArgs e)
        {
            content.InverseNormalZ();
        }

        private void exportTool_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                content.ExportAsObj(saveFileDialog1.FileName);
            }
        }

        private void subDevisionTool_Click(object sender, EventArgs e)
        {

        }

        private void weldingTool_Click(object sender, EventArgs e)
        {
            VertexElementDlg dlg = new VertexElementDlg();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                EditableMesh.MeshVertexElement element = dlg.Elements;

                content.WeldVertices(element);
            }
            dlg.Dispose();

        }


    }
}

