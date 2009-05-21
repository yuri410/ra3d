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
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.IsoMap;
using R3D.MathLib;
using R3D.Media;
using R3D.UI;
using Ra2Develop.Editors;
using Ra2Develop.Editors.EditableObjects;
using SlimDX;
using SlimDX.Direct3D9;
using WeifenLuo.WinFormsUI.Docking;

namespace Ra2Develop.Designers
{
    public partial class Tile3DDocument : GeneralDocumentBase
    {
        public const string Extension = ".tmp3";

        //delegate void SaveStateChangeCallback();
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <remarks>不继承，以免过多的内存复制</remarks>
        //class EditableBlockData
        //{
        //    static readonly int[] DrawIndex = new int[12] { 4, 1, 0, 2, 4, 0, 1, 4, 3, 4, 2, 3 };

        //    SaveStateChangeCallback changed;

        //    Block sounds;

        //    TileVertex[] vertex = new TileVertex[5];
        //    TileVertex[] secVertex = new TileVertex[5];

        //    public TileVertex[] renderVtx = new TileVertex[12];
        //    public TileVertex[] renderSecVtx = new TileVertex[12];


        //    public Matrix transformation = Matrix.Identity;

        //    public Texture tex1;
        //    public Texture tex2;

        //    [Browsable(false)]
        //    public Block Data
        //    {
        //        get { return sounds; }
        //    }

        //    void Changed()
        //    {
        //        if (changed != null)
        //            changed();
        //    }

        //    public EditableBlockData(Block d, SaveStateChangeCallback ccbk)
        //    {
        //        if (d == null)
        //            throw new ArgumentNullException();
        //        sounds = d;
        //        changed = ccbk;

        //        // calculate basic sounds
        //        if (sounds.mat1 != null)
        //            UpdateTexture1();
        //        if (sounds.mat2 != null)
        //            UpdateTexture2();

        //        Terrain.SetCellVertex(ref vertex[0].pos, ref vertex[1].pos, ref vertex[2].pos, ref vertex[3].pos, ref vertex[4].pos);
        //        Terrain.SetCellVertex(ref secVertex[0].pos, ref secVertex[1].pos, ref secVertex[2].pos, ref secVertex[3].pos, ref secVertex[4].pos);

        //        vertex[0].t0 = 0;
        //        vertex[0].u0 = 0;

        //        vertex[1].t0 = 0;
        //        vertex[1].u0 = 1;

        //        vertex[2].t0 = 1;
        //        vertex[3].u0 = 0;

        //        vertex[3].t0 = 1;
        //        vertex[3].u0 = 1;

        //        vertex[4].t0 = 0.5f;
        //        vertex[4].u0 = 0.5f;



        //        secVertex[0].t0 = 0;
        //        secVertex[0].u0 = 0;

        //        secVertex[1].t0 = 0;
        //        secVertex[1].u0 = 1;

        //        secVertex[2].t0 = 1;
        //        secVertex[3].u0 = 0;

        //        secVertex[3].t0 = 1;
        //        secVertex[3].u0 = 1;

        //        secVertex[4].t0 = 0.5f;
        //        secVertex[4].u0 = 0.5f;

        //        vertex[0].pos.X = 0;
        //        vertex[0].pos.Z = 0;
        //        vertex[1].pos.X = 0;
        //        vertex[1].pos.Z = 0;
        //        vertex[2].pos.X = 0;
        //        vertex[2].pos.Z = 0;
        //        vertex[3].pos.X = 0;
        //        vertex[3].pos.Z = 0;
        //        vertex[4].pos.X = 0;
        //        vertex[4].pos.Z = 0;

        //        vertex[0].pos.Y = Height * Terrain.VerticalUnit;
        //        vertex[1].pos.Y = Height * Terrain.VerticalUnit;
        //        vertex[2].pos.Y = Height * Terrain.VerticalUnit;
        //        vertex[3].pos.Y = Height * Terrain.VerticalUnit;
        //        vertex[4].pos.Y = Height * Terrain.VerticalUnit;


        //        TileBase.SetRampVertex(sounds.ramp_type,
        //            ref vertex[0].pos,
        //            ref vertex[1].pos,
        //            ref vertex[2].pos,
        //            ref vertex[3].pos,
        //            ref vertex[4].pos);
        //        Terrain.SetCellVertex(ref vertex[0].pos, ref vertex[1].pos,
        //         ref vertex[2].pos, ref vertex[3].pos, ref vertex[4].pos);

        //        secVertex[0].pos.X = 0;
        //        secVertex[0].pos.Z = 0;
        //        secVertex[1].pos.X = 0;
        //        secVertex[1].pos.Z = 0;
        //        secVertex[2].pos.X = 0;
        //        secVertex[2].pos.Z = 0;
        //        secVertex[3].pos.X = 0;
        //        secVertex[3].pos.Z = 0;
        //        secVertex[4].pos.X = 0;
        //        secVertex[4].pos.Z = 0;

        //        secVertex[0].pos.Y = SecHeight * Terrain.VerticalUnit;
        //        secVertex[1].pos.Y = SecHeight * Terrain.VerticalUnit;
        //        secVertex[2].pos.Y = SecHeight * Terrain.VerticalUnit;
        //        secVertex[3].pos.Y = SecHeight * Terrain.VerticalUnit;
        //        secVertex[4].pos.Y = SecHeight * Terrain.VerticalUnit;

        //        TileBase.SetRampVertex(sounds.secRampType,
        //            ref secVertex[0].pos,
        //            ref secVertex[1].pos,
        //            ref secVertex[2].pos,
        //            ref secVertex[3].pos,
        //            ref secVertex[4].pos);
        //        Terrain.SetCellVertex(ref secVertex[0].pos, ref secVertex[1].pos,
        //            ref secVertex[2].pos, ref secVertex[3].pos, ref secVertex[4].pos);


        //        CalculateVertex();
        //        transformation = Matrix.Translation(X * Terrain.HorizontalUnit, 0, Y * Terrain.HorizontalUnit);

        //    }

        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:Mesh")]
        //    [TypeConverter(typeof(BlockModelTypeConverter))]
        //    public virtual EditableBlockModel Model
        //    {
        //        get
        //        {
        //            if (sounds.Model == null)
        //            {
        //                sounds.Model = new BlockModel.Data();
        //            }
        //            return new EditableBlockModel(sounds.Model);
        //        }
        //        set
        //        {
        //            if (value != null)
        //            {
        //                sounds.Model = value.Data;
        //                Changed();
        //            }
        //        }
        //    }

        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:DblLayers")]
        //    public virtual bool HasDoubleLayers
        //    {
        //        get { return (sounds.bits & BlockBits.HasDoubleLayer) != 0; }
        //        set
        //        {
        //            if (value)
        //            {
        //                sounds.bits |= BlockBits.HasDoubleLayer;
        //                CalculateVertex();
        //            }
        //            else
        //            {
        //                sounds.bits ^= BlockBits.HasDoubleLayer;
        //            }
        //            Changed();
        //        }
        //    }

        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:Tmp3DModel")]
        //    public virtual bool Has3DModel
        //    {
        //        get { return (sounds.bits & BlockBits.HasTileModel) != 0; }
        //        set
        //        {
        //            if (value)
        //            {
        //                sounds.bits |= BlockBits.HasTileModel;
        //                CalculateVertex();
        //            }
        //            else
        //            {
        //                sounds.bits ^= BlockBits.HasTileModel;
        //            }
        //            Changed();
        //        }
        //    }

        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:Tmp3DeformationData")]
        //    public virtual bool HasDeformationData
        //    {
        //        get { return (sounds.bits & BlockBits.HasDaformationData) != 0; }
        //    }

        //    [LocalizedDescription("PROP:RadarColorLeft")]
        //    public Color RadarLeftColor
        //    {
        //        get
        //        {
        //            return Color.FromArgb(255, sounds.radar_red_left, sounds.radar_green_left, sounds.radar_blue_left);
        //        }
        //        set
        //        {
        //            sounds.radar_red_left = value.R;
        //            sounds.radar_green_left = value.G;
        //            sounds.radar_blue_left = value.B;
        //            Changed();
        //        }
        //    }
        //    [LocalizedDescription("PROP:RadarColorRight")]
        //    public Color RadarRightColor
        //    {
        //        get
        //        {
        //            return Color.FromArgb(255, sounds.radar_red_right, sounds.radar_green_right, sounds.radar_blue_right);
        //        }
        //        set
        //        {
        //            sounds.radar_red_right = value.R;
        //            sounds.radar_green_right = value.G;
        //            sounds.radar_blue_right = value.B;
        //            Changed();
        //        }
        //    }

        //    [LocalizedCategory("PROP:MATE")]
        //    [LocalizedDescription("PROP:MeshMat1")]
        //    [DefaultValue(null)]
        //    public EditableBlockMaterial Material1
        //    {
        //        get
        //        {
        //            if (sounds.mat1 == null)
        //                sounds.mat1 = new MeshMaterial.DataTextureEmbeded();
        //            return new EditableBlockMaterial(sounds.mat1, UpdateTexture1);
        //        }
        //        set
        //        {
        //            sounds.mat1 = value.Data;
        //            Changed();
        //        }
        //    }

        //    void UpdateTexture1()
        //    {
        //        if (tex1 != null)
        //        {
        //            tex1.Dispose();
        //            tex1 = null;
        //        }
        //        if (sounds.mat1.Texture != null)
        //        {
        //            tex1 = sounds.mat1.Texture.GetTexture(GraphicsDevice.Instance.Device, Usage.None, Pool.Managed);
        //        }
        //        Changed();
        //    }
        //    void UpdateTexture2()
        //    {
        //        if (tex2 != null)
        //        {
        //            tex2.Dispose();
        //            tex2 = null;
        //        }
        //        if (sounds.mat2.Texture != null)
        //        {
        //            tex2 = sounds.mat2.Texture.GetTexture(GraphicsDevice.Instance.Device, Usage.None, Pool.Managed);
        //        }
        //        Changed();
        //    }


        //    [LocalizedCategory("PROP:MATE")]
        //    [LocalizedDescription("PROP:MeshMat2")]
        //    [DefaultValue(null)]
        //    public EditableBlockMaterial Material2
        //    {
        //        get
        //        {
        //            if (sounds.mat2 == null)
        //                sounds.mat2 = new MeshMaterial.DataTextureEmbeded();
        //            return new EditableBlockMaterial(sounds.mat2, UpdateTexture2);
        //        }
        //        set
        //        {
        //            sounds.mat2 = value.Data;
        //            Changed();
        //        }
        //    }



        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:CoordX")]
        //    public int X
        //    {
        //        get { return sounds.x; }
        //        set
        //        {
        //            sounds.x = value;
        //            transformation = Matrix.Translation(value * Terrain.HorizontalUnit, 0, Y * Terrain.HorizontalUnit);
        //            Changed();
        //        }
        //    }
        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:CoordY")]
        //    public int Y
        //    {
        //        get { return sounds.y; }
        //        set
        //        {
        //            sounds.y = value;
        //            transformation = Matrix.Translation(X * Terrain.HorizontalUnit, 0, value * Terrain.HorizontalUnit);
        //            Changed();
        //        }
        //    }

        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:HeightL1")]
        //    public short Height
        //    {
        //        get { return sounds.height; }
        //        set
        //        {
        //            int offset = value - sounds.height;
        //            sounds.height = value;

        //            vertex[0].pos.Y += offset * Terrain.VerticalUnit;
        //            vertex[1].pos.Y += offset * Terrain.VerticalUnit;
        //            vertex[2].pos.Y += offset * Terrain.VerticalUnit;
        //            vertex[3].pos.Y += offset * Terrain.VerticalUnit;
        //            vertex[4].pos.Y += offset * Terrain.VerticalUnit;
        //            CalculateVertex();
        //            Changed();
        //        }
        //    }
        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:TerrTypeL1")]
        //    [DefaultValue(TerrainType.Clear)]
        //    public TerrainType TerrainType
        //    {
        //        get { return sounds.terrain_type; }
        //        set
        //        {
        //            sounds.terrain_type = value;
        //            Changed();
        //        }
        //    }
        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:RampTypeL1")]
        //    [Editor(typeof(RampTypeEditor), typeof(UITypeEditor))]
        //    public byte RampType
        //    {
        //        get { return sounds.ramp_type; }
        //        set
        //        {
        //            if (value > 20)
        //            {
        //                throw new ArgumentOutOfRangeException();
        //            }

        //            sounds.ramp_type = value;
        //            vertex[0].pos.X = 0;
        //            vertex[0].pos.Z = 0;
        //            vertex[1].pos.X = 0;
        //            vertex[1].pos.Z = 0;
        //            vertex[2].pos.X = 0;
        //            vertex[2].pos.Z = 0;
        //            vertex[3].pos.X = 0;
        //            vertex[3].pos.Z = 0;
        //            vertex[4].pos.X = 0;
        //            vertex[4].pos.Z = 0;

        //            vertex[0].pos.Y = Height * Terrain.VerticalUnit;
        //            vertex[1].pos.Y = Height * Terrain.VerticalUnit;
        //            vertex[2].pos.Y = Height * Terrain.VerticalUnit;
        //            vertex[3].pos.Y = Height * Terrain.VerticalUnit;
        //            vertex[4].pos.Y = Height * Terrain.VerticalUnit;


        //            TileBase.SetRampVertex(value,
        //                ref vertex[0].pos,
        //                ref vertex[1].pos,
        //                ref vertex[2].pos,
        //                ref vertex[3].pos,
        //                ref vertex[4].pos);
        //            Terrain.SetCellVertex(ref vertex[0].pos, ref vertex[1].pos,
        //             ref vertex[2].pos, ref vertex[3].pos, ref vertex[4].pos);
        //            CalculateVertex();
        //            Changed();
        //        }
        //    }


        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:HeightL2")]
        //    public short SecHeight
        //    {
        //        get { return sounds.secHeight; }
        //        set
        //        {
        //            int offset = value - sounds.secHeight;
        //            sounds.secHeight = value;
        //            secVertex[0].pos.Y += offset * Terrain.VerticalUnit;
        //            secVertex[1].pos.Y += offset * Terrain.VerticalUnit;
        //            secVertex[2].pos.Y += offset * Terrain.VerticalUnit;
        //            secVertex[3].pos.Y += offset * Terrain.VerticalUnit;
        //            secVertex[4].pos.Y += offset * Terrain.VerticalUnit;
        //            CalculateVertex();
        //            Changed();
        //        }
        //    }
        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:TerrTypeL2")]
        //    [DefaultValue(TerrainType.Clear)]
        //    public TerrainType SecTerrainType
        //    {
        //        get { return sounds.secTerrType; }
        //        set
        //        {
        //            sounds.secTerrType = value;
        //            Changed();
        //        }
        //    }
        //    [LocalizedCategory("PROP:SHAPE")]
        //    [LocalizedDescription("PROP:RampTypeL2")]
        //    [Editor(typeof(RampTypeEditor), typeof(UITypeEditor))]
        //    public byte SecRampType
        //    {
        //        get { return sounds.secRampType; }
        //        set
        //        {
        //            if (value > 20)
        //            {
        //                throw new ArgumentOutOfRangeException();
        //            }
        //            sounds.secRampType = value;
        //            secVertex[0].pos.X = 0;
        //            secVertex[0].pos.Z = 0;
        //            secVertex[1].pos.X = 0;
        //            secVertex[1].pos.Z = 0;
        //            secVertex[2].pos.X = 0;
        //            secVertex[2].pos.Z = 0;
        //            secVertex[3].pos.X = 0;
        //            secVertex[3].pos.Z = 0;
        //            secVertex[4].pos.X = 0;
        //            secVertex[4].pos.Z = 0;

        //            secVertex[0].pos.Y = SecHeight * Terrain.VerticalUnit;
        //            secVertex[1].pos.Y = SecHeight * Terrain.VerticalUnit;
        //            secVertex[2].pos.Y = SecHeight * Terrain.VerticalUnit;
        //            secVertex[3].pos.Y = SecHeight * Terrain.VerticalUnit;
        //            secVertex[4].pos.Y = SecHeight * Terrain.VerticalUnit;

        //            TileBase.SetRampVertex(value,
        //                ref secVertex[0].pos,
        //                ref secVertex[1].pos,
        //                ref secVertex[2].pos,
        //                ref secVertex[3].pos,
        //                ref secVertex[4].pos);
        //            Terrain.SetCellVertex(ref secVertex[0].pos, ref secVertex[1].pos,
        //                ref secVertex[2].pos, ref secVertex[3].pos, ref secVertex[4].pos);
        //            CalculateVertex();
        //            Changed();
        //        }
        //    }

        //    void CalculateVertex()
        //    {
        //        for (int i = 0; i < 12; i++)
        //        {
        //            renderVtx[i] = vertex[DrawIndex[i]];
        //            if ((i + 1) % 3 == 0)
        //            {
        //                Vector3 n;
        //                MathEx.ComputePlaneNormal(ref renderVtx[i].pos, ref renderVtx[i - 1].pos, ref renderVtx[i - 2].pos, out n);

        //                renderVtx[i].n = n;
        //                renderVtx[i - 1].n = n;
        //                renderVtx[i - 2].n = n;

        //            }
        //            if (HasDoubleLayers)
        //            {
        //                renderSecVtx[i] = secVertex[DrawIndex[i]];
        //                if ((i + 1) % 3 == 0)
        //                {
        //                    Vector3 n;
        //                    MathEx.ComputePlaneNormal(ref renderSecVtx[i].pos, ref renderSecVtx[i - 1].pos, ref renderSecVtx[i - 2].pos, out n);

        //                    renderSecVtx[i].n = n;
        //                    renderSecVtx[i - 1].n = n;
        //                    renderSecVtx[i - 2].n = n;
        //                }
        //            }
        //        }
        //    }

        //    public override string ToString()
        //    {
        //        StringBuilder sb = new StringBuilder(7);
        //        sb.Append("{ ");
        //        sb.Append(Program.StringTable["PROP:TMP3BLOCK"]);
        //        sb.Append(": X=");
        //        sb.Append(X.ToString());
        //        sb.Append(" Y=");
        //        sb.Append(Y.ToString());
        //        sb.Append("}");
        //        return sb.ToString();
        //    }

        //    public void Dispose()
        //    {
        //        if (tex1 != null)
        //        {
        //            tex1.Dispose();
        //            tex1 = null;
        //        }
        //        if (tex2 != null)
        //        {
        //            tex2.Dispose();
        //            tex2 = null;
        //        }
        //        changed = null;
        //    }
        //}

        //struct TileVertex
        //{
        //    public Vector3 pos;
        //    public Vector3 n;
        //    public float t0, u0;

        //    public static VertexFormat Format
        //    {
        //        get
        //        {
        //            return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;
        //        }
        //    }
        //}
        struct LineVertex
        {
            public Vector3 pos;
            public int diffuse;

            public static VertexFormat Format
            {
                get { return VertexFormat.Position | VertexFormat.Diffuse; }
            }
        }

        List<EditableBlock> content;

        View3D view3D;
        
        Point lastPos;
        

        EditableBlock selectedBlock;
        bool isDraging;


        void propChangeSaveState()
        {
            Saved = false;
        }

        public Tile3DDocument(DocumentAbstractFactory fac, ResourceLocation rl)
        {
            InitializeComponent();

            MouseWheel += viewMouseWheel;

            LanguageParser.ParseLanguage(Program.StringTable, this);
            LanguageParser.ParseLanguage(Program.StringTable, toolStrip1);

            Init(fac, rl);

            view3D = new View3D(
                Tile3DDocumentConfigs.Instance.EyeAngleX, 
                Tile3DDocumentConfigs.Instance.EyeAngleY,
                Tile3DDocumentConfigs.Instance.EyeDistance,
                Tile3DDocumentConfigs.Instance.Fovy);
            view3D.ViewChanged += ViewChanged;
        }

        private void Tile3DDocument_Load(object sender, EventArgs e)
        {            
            Saved = true;
        }

        public override ToolStrip[] ToolStrips
        {
            get
            {
                return new ToolStrip[] { view3D.ToolBar, toolStrip1 };
            }
        }



        public override bool LoadRes()
        {
            if (ResourceLocation != null)
            {
                //Tile3D.Data sounds = new Tile3D.Data();
                //sounds.ResourceLocation = ResourceLocation;
                //sounds.Load();
                EditableTile3D data = EditableTile3D.FromFile(ResourceLocation);

                EditableBlock[] blks = data.Blocks;

                content = new List<EditableBlock>(data.BlockCount + 5);
                for (int i = 0; i < data.BlockCount; i++)
                {
                    blks[i].StateChanged += propChangeSaveState;
                    blks[i].Material1.StateChanged += propChangeSaveState;
                    if (blks[i].Material2 != null)
                        blks[i].Material2.StateChanged += propChangeSaveState;
                    content.Add(blks[i]);
                }
                Saved = true;
                ViewChanged();
                return true;
            }
            content = new List<EditableBlock>();
            ViewChanged();
            Saved = true;
            return true;
        }

        public override bool SaveRes()
        {
            if (ResourceLocation.IsReadOnly)
                throw new InvalidOperationException();

            EditableTile3D data = new EditableTile3D();

            EditableBlock[] blks = new EditableBlock[content.Count];
            for (int i = 0; i < content.Count; i++)
            {
                blks[i] = content[i];
            }
            data.Blocks = blks;

            EditableTile3D.ToResLoc(data, ResourceLocation);

            Saved = true;
            return true;
        }

        void SelectBlock(EditableBlock blk)
        {
            selectedBlock = blk;
            if (propertyUpdated != null)
            {
                if (selectedBlock != null)
                {
                    propertyUpdated(selectedBlock, content.ToArray());
                }
                else
                {
                    propertyUpdated(null, null);
                }
            }
            ViewChanged();
        }

        void viewMouseWheel(object sender, MouseEventArgs e)
        {
            view3D.EyeDistance -= e.Delta * 0.02f;
        }

        unsafe void Draw3D()
        {
            GraphicsDevice.Instance.BeginScene(this);

            Device dev = GraphicsDevice.Instance.Device;

            dev.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.DarkBlue, 1, 0);

            dev.BeginScene();

            dev.SetRenderState<FillMode>(RenderState.FillMode, FillMode.Solid);
            dev.SetRenderState(RenderState.AlphaBlendEnable, true);
            
            dev.SetRenderState(RenderState.SpecularEnable, true);
            dev.SetRenderState(RenderState.ColorVertex, false);

            //dev.SetRenderState<ColorSource>(RenderState.AmbientMaterialSource, ColorSource.Material);
            //dev.SetRenderState<ColorSource>(RenderState.DiffuseMaterialSource, ColorSource.Material);
            dev.SetRenderState<ColorSource>(RenderState.SpecularMaterialSource, ColorSource.Material);
            //dev.SetRenderState<ColorSource>(RenderState.EmissiveMaterialSource, ColorSource.Material);
            
            dev.SetRenderState(RenderState.Lighting, true);

            Light light = new Light();
            light.Type = LightType.Point;
            light.Position = Tile3DDocumentConfigs.Instance.LightPosition;
            light.Range = float.MaxValue;
            //light.Falloff = 0;
            light.Ambient = Tile3DDocumentConfigs.Instance.LightAmbient;
            light.Diffuse = Tile3DDocumentConfigs.Instance.LightDiffuse;
            light.Specular = Tile3DDocumentConfigs.Instance.LightSpecular;
            light.Attenuation0 = 1.0f;

            
            dev.SetTransform(TransformState.World, Matrix.Identity);
            dev.SetLight(0, light);

            dev.EnableLight(0, true);



            view3D.SetTransform(dev, this);


            DevUtils.DrawLight(dev, light.Position);
            

            for (int i = 0; i < content.Count; i++)
            {
                content[i].Render();
            }

            dev.SetTexture(0, null);

            const int gridRange = 10;
            const float halfLineLength = (gridRange / 2 + 1) * Terrain.HorizontalUnit;

            Vector3 offset = new Vector3();
            if (selectedBlock != null)
            {
                offset.Y = selectedBlock.Height * Terrain.VerticalUnit;
                offset.X = selectedBlock.X * Terrain.HorizontalUnit;
                offset.Z = selectedBlock.Y * Terrain.HorizontalUnit;
            }

            LineVertex[] line = new LineVertex[2];
            Matrix trans;

            dev.SetRenderState(RenderState.Lighting, false);
            dev.SetRenderState(RenderState.DepthBias, 0.5f);
           
            line[0].diffuse = int.MaxValue;
            line[1].diffuse = int.MaxValue;


            line[0].pos = new Vector3(halfLineLength, 0, 0);
            line[1].pos = new Vector3(-halfLineLength, 0, 0);

            for (int i = 0; i <= gridRange; i++)
            {
                Matrix.Translation(offset.X, offset.Y, offset.Z + (i - (gridRange / 2)) * Terrain.HorizontalUnit, out trans);

                dev.SetTransform(TransformState.World, trans);
                dev.VertexFormat = LineVertex.Format;
                dev.DrawUserPrimitives<LineVertex>(PrimitiveType.LineList, 0, 1, line);
            }


            line[0].pos = new Vector3(0, 0, halfLineLength);
            line[1].pos = new Vector3(0, 0, -halfLineLength);

            for (int i = 0; i <= gridRange; i++)
            {
                Matrix.Translation(offset.X + (i - (gridRange / 2)) * Terrain.HorizontalUnit, offset.Y, offset.Z, out trans);

                dev.SetTransform(TransformState.World, trans);
                dev.VertexFormat = LineVertex.Format;
                dev.DrawUserPrimitives<LineVertex>(PrimitiveType.LineList, 0, 1, line);
            }
            dev.SetRenderState(RenderState.DepthBias, 1f);
            if (selectedBlock != null)
            {
                line = new LineVertex[5];

                line[0].diffuse = (0xff << 24) | (0xff << 16);
                line[1].diffuse = (0xff << 24) | (0xff << 16);
                line[2].diffuse = (0xff << 24) | (0xff << 16);
                line[3].diffuse = (0xff << 24) | (0xff << 16);
                line[4].diffuse = (0xff << 24) | (0xff << 16);

                Vector3[] edge = selectedBlock.GetEdges();
                line[0].pos = edge[0];
                line[1].pos = edge[1];
                line[2].pos = edge[2];
                line[3].pos = edge[3];
                line[4].pos = edge[4];

                dev.SetTransform(TransformState.World, selectedBlock.transformation);
                dev.VertexFormat = LineVertex.Format;
                dev.DrawUserPrimitives<LineVertex>(PrimitiveType.LineStrip, 0, 4, line);
            }

            dev.SetRenderState(RenderState.DepthBias, 0f);



            Sprite spr = GraphicsDevice.Instance.GetSprite;

            spr.Begin(SpriteFlags.AlphaBlend | SpriteFlags.DoNotSaveState);
            if (content != null)
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:BlockCount"] + content.Count.ToString(), 5, 5, Color.White);
            }
            else
            {
                GraphicsDevice.Instance.DefaultFont.DrawString(spr, Program.StringTable["GUI:BlockCount"] + 0.ToString(), 5, 5, Color.White);
            }
            spr.End();


            dev.EndScene();
            GraphicsDevice.Instance.EndScene();
            //dev.Present();
        }

        protected override void active()
        {
            //GraphicsDevice.Instance.Bind(this);
            //GraphicsDevice.Instance.MouseClick += viewMouseClick;
            //GraphicsDevice.Instance.MouseMove += viewMouseMove;
            //GraphicsDevice.Instance.MouseDown += viewMouseDown;
            //GraphicsDevice.Instance.MouseUp += viewMouseUp;
            SelectBlock(selectedBlock);
        }
        protected override void deactive()
        {
            //GraphicsDevice.Instance.MouseClick -= viewMouseClick;
            //GraphicsDevice.Instance.MouseMove -= viewMouseMove;
            //GraphicsDevice.Instance.MouseDown -= viewMouseDown;
            //GraphicsDevice.Instance.MouseUp -= viewMouseUp;
            //GraphicsDevice.Instance.Unbind(this);

        }

        public override bool Saved
        {
            get
            {
                return base.Saved;
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
        protected override void OnPaint(PaintEventArgs e)
        {
            Draw3D();
        }

        private void addTool_Click(object sender, EventArgs e)
        {
            EditableBlock d = new EditableBlock(propChangeSaveState);
            d.bits = BlockBits.None;
            d.mat1 = new EditableBlockMaterial();
            d.mat2 = new EditableBlockMaterial();

            //d.mat1.mat.Ambient = new Color4(1f, 0.5f, 0.5f, 0.5f);
            //d.mat1.mat.Diffuse = new Color4(1, 0.8f, 0.8f, 0.8f);
            //d.mat1.mat.Emissive = new Color4(0, 0, 0, 0);
            //d.mat1.mat.Specular = new Color4(1, 0.8f, 0.8f, 0.8f);
            //d.mat1.mat.Power = 16;

            //d.mat2.mat.Ambient = new Color4(1f, 0.5f, 0.5f, 0.5f);
            //d.mat2.mat.Diffuse = new Color4(1, 0.8f, 0.8f, 0.8f);
            //d.mat2.mat.Emissive = new Color4(0, 0, 0, 0);
            //d.mat2.mat.Specular = new Color4(1, 0.8f, 0.8f, 0.8f);
            //d.mat2.mat.Power = 16;

            d.Initialize();
            content.Add(d);
            ViewChanged();
        }
        private void removeTool_Click(object sender, EventArgs e)
        {
            if (selectedBlock != null)
            {
                content.Remove(selectedBlock);
                SelectBlock(null);
            }            
            ViewChanged();
        }
        private void Tile3DDocument_FormClosed(object sender, FormClosedEventArgs e)
        {
            Tile3DDocumentConfigs.Instance.EyeAngleX = view3D.EyeAngleX;
            Tile3DDocumentConfigs.Instance.EyeAngleY = view3D.EyeAngleY;
            Tile3DDocumentConfigs.Instance.EyeDistance = view3D.EyeDistance;
            Tile3DDocumentConfigs.Instance.Fovy = view3D.Fovy;
            GraphicsDevice.Instance.Free(this);
            selectedBlock = null;
            for (int i = 0; i < content.Count; i++)
            {
                content[i].Dispose();
            }
            content.Clear();
            content = null;
        }

        private void prevBlockTool_Click(object sender, EventArgs e)
        {
            if (selectedBlock != null)
            {
                int idx = content.IndexOf(selectedBlock);
                if (idx > 0)
                {
                    SelectBlock(content[idx - 1]);
                }
                else 
                {
                    SelectBlock(content[content.Count - 1]);
                }
            }
            else if (content.Count > 0)
            {
                SelectBlock(content[0]);
            }

        }

        private void nextBlockTool_Click(object sender, EventArgs e)
        {
            if (selectedBlock != null)
            {
                int idx = content.IndexOf(selectedBlock);
                if (idx < content.Count - 1)
                {
                    SelectBlock(content[idx + 1]);
                }
                else
                {
                    SelectBlock(content[0]);
                }
            }
            else if (content.Count > 0)
            {
                SelectBlock(content[0]);
            }
        }

        private void Tile3DDocument_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Tile3DDocument_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                lastPos = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                Vector3 eye = view3D.EyePosition;

                Vector3 pNear = new Vector3(e.X, e.Y, 0);
                Vector3 pFar = new Vector3(e.X, e.Y, 1);

                Viewport vp = GraphicsDevice.Instance.Device.Viewport;
                Matrix i4 = Matrix.Identity;
                Matrix projection = view3D.Projection;
                Matrix view = view3D.View;

                Vector3 begin;//= Vector3.Unproject(new Vector3(e.X, e.Y, 0), GraphicsDevice.Instance.Device.Viewport, projection, view, Matrix.Identity);
                Vector3 end;//= Vector3.Unproject(new Vector3(e.X, e.Y, 1), GraphicsDevice.Instance.Device.Viewport, projection, view, Matrix.Identity);

                Vector3.Unproject(ref pNear, ref vp, ref projection, ref view, ref i4, out begin);
                Vector3.Unproject(ref pFar, ref vp, ref projection, ref view, ref i4, out end);


                float closest = float.MaxValue;
                //float meshClosest=float.MaxValue;

                EditableBlock data = null;
                for (int i = 0; i < content.Count; i++)
                {
                    Vector3 res;
                    if (content[i].Intersects(begin, end, out res))
                    {
                        float dist = MathEx.DistanceSquared(ref res, ref eye);

                        Vector3 v2;
                        Vector3.Subtract(ref res, ref eye, out v2);
                        float dire = -MathEx.Vec3Dot(ref v2, ref eye);

                        if (dist < closest && dire > 0)
                        {
                            data = content[i];

                            closest = dist;
                        }
                    }

                    //if (content[i].Has3DModel && content[i].Model != null)
                    //{
                    //    for (int j = 0; j < content[i].Model.Entities.Length; j++)
                    //    {
                    //        float dist;
                    //        if (content[i].Model.Entities[i].Intersects(begin, end, out dist))
                    //        {
                    //            if (dist < meshClosest)
                    //            {

                    //            }
                    //        }
                    //    }
                    //}
                }
                SelectBlock(data);


                //if (propertyUpdated != null)
                //{
                //    if (sounds != null)
                //    {
                //        propertyUpdated(sounds, content.ToArray());
                //    }
                //    else
                //    {
                //        propertyUpdated(null, null);
                //    }
                //}
            }
        }

        private void Tile3DDocument_MouseMove(object sender, MouseEventArgs e)
        {
            Point loc = e.Location;
            Point offset = new Point();
            offset.X = loc.X - lastPos.X;
            offset.Y = loc.Y - lastPos.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (selectedBlock != null)
                {
                    if (!isDraging)
                    {
                        if (Math.Abs(offset.X) > 2 && Math.Abs(offset.Y) > 2)
                        {
                            isDraging = true;
                        }
                    }

                    if (isDraging)
                    {
                        Vector3 pNear = new Vector3(e.X, e.Y, 0);
                        Vector3 pFar = new Vector3(e.X, e.Y, 1);

                        Viewport vp = GraphicsDevice.Instance.Device.Viewport;
                        Matrix i4 = Matrix.Identity;
                        Matrix projection = view3D.Projection;
                        Matrix view = view3D.View;

                        Vector3 begin;//= Vector3.Unproject(new Vector3(e.X, e.Y, 0), GraphicsDevice.Instance.Device.Viewport, projection, view, Matrix.Identity);
                        Vector3 end;//= Vector3.Unproject(new Vector3(e.X, e.Y, 1), GraphicsDevice.Instance.Device.Viewport, projection, view, Matrix.Identity);

                        Vector3.Unproject(ref pNear, ref vp, ref projection, ref view, ref i4, out begin);
                        Vector3.Unproject(ref pFar, ref vp, ref projection, ref view, ref i4, out end);

                        Plane heightPlane = new Plane(0, 1, 0, -selectedBlock.Height * Terrain.VerticalUnit);

                        Vector3 res;
                        if (Plane.Intersects(heightPlane, begin, end, out res))
                        {
                            res.X /= Terrain.HorizontalUnit;
                            res.Z /= Terrain.HorizontalUnit;



                            int x = (int)Math.Truncate(res.X);
                            int y = (int)Math.Truncate(res.Z);

                            if (res.X < 0)
                            {
                                x--;
                            }
                            if (res.Z < 0)
                            {
                                y--;
                            }


                            selectedBlock.X = x;
                            selectedBlock.Y = y;
                        }
                    }
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                view3D.EyeAngleX += MathEx.PIf * (float)offset.X / 180f;
                view3D.EyeAngleY += MathEx.PIf * (float)offset.Y / 180f;

                lastPos = loc;
            }
        }

        private void Tile3DDocument_MouseUp(object sender, MouseEventArgs e)
        {
            isDraging = false;

        }

    }
}
