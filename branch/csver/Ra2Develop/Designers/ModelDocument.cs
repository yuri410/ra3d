﻿/*
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
using Ra2Develop.Converters;
using Ra2Develop.Editors.EditableObjects;
using R3D.IO;
using R3D.MathLib;
using R3D.UI;
using SlimDX;
using SlimDX.Direct3D9;
using WeifenLuo.WinFormsUI.Docking;

namespace Ra2Develop.Designers
{
    public partial class ModelDocument : GeneralDocumentBase
    {
        public const string Extension = ".mesh";

        View3D view3D;

        Point lastPos;
        FillMode fillMode = FillMode.Solid;

        EditableModel content;

        void propChangeSaveState()
        {
            Saved = false;
        }

        public ModelDocument(DocumentAbstractFactory fac, ResourceLocation rl)
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(Program.StringTable, this);
            LanguageParser.ParseLanguage(Program.StringTable, toolStrip1);

            Init(fac, rl);

            view3D = new View3D(
              ModelDocumentConfigs.Instance.EyeAngleX,
              ModelDocumentConfigs.Instance.EyeAngleY,
              ModelDocumentConfigs.Instance.EyeDistance,
              ModelDocumentConfigs.Instance.Fovy);
            view3D.ViewChanged += ViewChanged;

            this.MouseWheel += viewMouseWheel;
            Saved = true;

            content = new EditableModel();
            //content.StateChanged += propChangeSaveState;
        }


        public override ToolStrip[] ToolStrips
        {
            get
            {
                return new ToolStrip[] { toolStrip1, view3D.ToolBar };
            }
        }


        public override bool LoadRes()
        {
            if (ResourceLocation != null)
            {
                content = EditableModel.FromFile(ResourceLocation);

                Saved = true;
                ViewChanged();
            }
            return true;
        }

        public override bool SaveRes()
        {
            if (ResourceLocation.IsReadOnly)
                throw new InvalidOperationException();

            EditableModel.ToFile(content, ResourceLocation);

            Saved = true;
            return true;
        }

        public override bool Saved
        {
            get
            {
                return false;// base.Saved;
            }
            protected set
            {
                base.Saved = value;
                ViewChanged();
            }
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

        private void Draw3D()
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

            DevUtils.DrawLight(dev, light.Position);

            dev.SetTransform(TransformState.World, Matrix.Identity);


            if (content != null)
            {
                content.Render();
            }

            Sprite spr = GraphicsDevice.Instance.GetSprite;

            spr.Begin(SpriteFlags.AlphaBlend | SpriteFlags.DoNotSaveState);

            //Rectangle rect = GraphicsDevice.Instance.DefaultFont.MeasureString(spr, " ", DrawTextFormat.Left | DrawTextFormat.Top | DrawTextFormat.SingleLine);
            if (content != null && content.Entities != null)
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:ModelEntCount"] + content.Entities.Length.ToString(), 5, 5, Color.White);
            }
            else
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:ModelEntCount"] + 0.ToString(), 5, 5, Color.White);
            }
            spr.End();

            dev.EndScene();

            GraphicsDevice.Instance.EndScene();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Draw3D();
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

        private void ModelDocument_MouseMove(object sender, MouseEventArgs e)
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

        private void ModelDocument_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                lastPos = e.Location;
        }

        void viewMouseWheel(object sender, MouseEventArgs e)
        {
            view3D.EyeDistance -= e.Delta * 0.02f;
        }

        private void ModelDocument_FormClosing(object sender, FormClosingEventArgs e)
        {
            content.Dispose();

            ModelDocumentConfigs.Instance.EyeAngleX = view3D.EyeAngleX;
            ModelDocumentConfigs.Instance.EyeAngleY = view3D.EyeAngleY;
            ModelDocumentConfigs.Instance.EyeDistance = view3D.EyeDistance;
            ModelDocumentConfigs.Instance.Fovy = view3D.Fovy;
            GraphicsDevice.Instance.Free(this);
        }

        protected override void active()
        {
            if (propertyUpdated != null)
            {
                propertyUpdated(content, null);
            }
        }
        protected override void deactive()
        {
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

                if (content != null && content.Entities != null && content.Entities.Length > 0)
                {
                    EditableMesh[] newEnts = new EditableMesh[content.Entities.Length + value.Entities.Length];

                    Array.Copy(content.Entities, newEnts, content.Entities.Length);
                    Array.Copy(value.Entities, 0, newEnts, content.Entities.Length, value.Entities.Length);

                    content.Entities = newEnts;
                }
                else
                {
                    if (content != null)
                    {
                        content.Dispose();
                    }
                    content = value;
                }

                MeshChanged();
            }
        }

    }
}
