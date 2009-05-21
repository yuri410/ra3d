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
using System.Drawing.Design;
using System.Text;
using Ra2Develop.Designers;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Animating;
using R3D.IsoMap;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Develop.Editors.EditableObjects
{
    public delegate void EditObjChangedHandler();

    public class EditableBlock : BlockBase<EditableBlockMaterial, EditableModel>, IDisposable
    {
        struct TileVertex
        {
            public Vector3 pos;
            public Vector3 n;
            public float t0, u0;

            public static VertexFormat Format
            {
                get
                {
                    return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1;
                }
            }
        }

        static readonly int[] DrawIndex = new int[12] { 4, 1, 0, 2, 4, 0, 1, 4, 3, 4, 2, 3 };

        EditObjChangedHandler changed;
        TileVertex[] vertex = new TileVertex[5];
        TileVertex[] secVertex = new TileVertex[5];

        TileVertex[] renderVtx = new TileVertex[12];
        TileVertex[] renderSecVtx = new TileVertex[12];


        public Matrix transformation = Matrix.Identity;

        [Browsable(false)]
        public event EditObjChangedHandler StateChanged
        {
            add { changed += value; }
            remove { changed -= value; }
        }

        public EditableBlock(EditObjChangedHandler ccbk)
        {
            changed = ccbk;


        }

        public void Initialize()
        {  
            // calculate basic sounds
            Terrain.SetCellVertex(ref vertex[0].pos, ref vertex[1].pos, ref vertex[2].pos, ref vertex[3].pos, ref vertex[4].pos);
            Terrain.SetCellVertex(ref secVertex[0].pos, ref secVertex[1].pos, ref secVertex[2].pos, ref secVertex[3].pos, ref secVertex[4].pos);

            transformation = Matrix.Translation(X * Terrain.HorizontalUnit, 0, Y * Terrain.HorizontalUnit);

            vertex[0].t0 = 0;
            vertex[0].u0 = 0;

            vertex[1].t0 = 0;
            vertex[1].u0 = 1;

            vertex[2].t0 = 1;
            vertex[3].u0 = 0;

            vertex[3].t0 = 1;
            vertex[3].u0 = 1;

            vertex[4].t0 = 0.5f;
            vertex[4].u0 = 0.5f;



            secVertex[0].t0 = 0;
            secVertex[0].u0 = 0;

            secVertex[1].t0 = 0;
            secVertex[1].u0 = 1;

            secVertex[2].t0 = 1;
            secVertex[3].u0 = 0;

            secVertex[3].t0 = 1;
            secVertex[3].u0 = 1;

            secVertex[4].t0 = 0.5f;
            secVertex[4].u0 = 0.5f;

            vertex[0].pos.X = 0;
            vertex[0].pos.Z = 0;
            vertex[1].pos.X = 0;
            vertex[1].pos.Z = 0;
            vertex[2].pos.X = 0;
            vertex[2].pos.Z = 0;
            vertex[3].pos.X = 0;
            vertex[3].pos.Z = 0;
            vertex[4].pos.X = 0;
            vertex[4].pos.Z = 0;

            vertex[0].pos.Y = Height * Terrain.VerticalUnit;
            vertex[1].pos.Y = Height * Terrain.VerticalUnit;
            vertex[2].pos.Y = Height * Terrain.VerticalUnit;
            vertex[3].pos.Y = Height * Terrain.VerticalUnit;
            vertex[4].pos.Y = Height * Terrain.VerticalUnit;


            TileBase.SetRampVertex(ramp_type,
                ref vertex[0].pos,
                ref vertex[1].pos,
                ref vertex[2].pos,
                ref vertex[3].pos,
                ref vertex[4].pos);
            Terrain.SetCellVertex(ref vertex[0].pos, ref vertex[1].pos,
             ref vertex[2].pos, ref vertex[3].pos, ref vertex[4].pos);

            secVertex[0].pos.X = 0;
            secVertex[0].pos.Z = 0;
            secVertex[1].pos.X = 0;
            secVertex[1].pos.Z = 0;
            secVertex[2].pos.X = 0;
            secVertex[2].pos.Z = 0;
            secVertex[3].pos.X = 0;
            secVertex[3].pos.Z = 0;
            secVertex[4].pos.X = 0;
            secVertex[4].pos.Z = 0;

            secVertex[0].pos.Y = SecHeight * Terrain.VerticalUnit;
            secVertex[1].pos.Y = SecHeight * Terrain.VerticalUnit;
            secVertex[2].pos.Y = SecHeight * Terrain.VerticalUnit;
            secVertex[3].pos.Y = SecHeight * Terrain.VerticalUnit;
            secVertex[4].pos.Y = SecHeight * Terrain.VerticalUnit;

            TileBase.SetRampVertex(secRampType,
                ref secVertex[0].pos,
                ref secVertex[1].pos,
                ref secVertex[2].pos,
                ref secVertex[3].pos,
                ref secVertex[4].pos);
            Terrain.SetCellVertex(ref secVertex[0].pos, ref secVertex[1].pos,
                ref secVertex[2].pos, ref secVertex[3].pos, ref secVertex[4].pos);

            CalculateVertex();
        }

        void Changed()
        {
            if (changed != null)
                changed();
        }

        public bool Intersects(Vector3 v1, Vector3 v2, out Vector3 res)
        {
            Triangle ta = new Triangle(renderVtx[0].pos, renderVtx[1].pos, renderVtx[2].pos);
            Triangle tb = new Triangle(renderVtx[3].pos, renderVtx[4].pos, renderVtx[5].pos);
            Triangle tc = new Triangle(renderVtx[6].pos, renderVtx[7].pos, renderVtx[8].pos);
            Triangle td = new Triangle(renderVtx[9].pos, renderVtx[10].pos, renderVtx[11].pos);

            float offX = X * Terrain.HorizontalUnit;
            float offZ = Y * Terrain.HorizontalUnit;

            Vector3 tbegin = v1;
            Vector3 tend = v2;
            tbegin.X -= offX;
            tbegin.Z -= offZ;
            tend.X -= offX;
            tend.Z -= offZ;


            return
                (ta.RayTriCollision(out res, ref tbegin, ref tend) ||
                 tb.RayTriCollision(out res, ref tbegin, ref tend) ||
                 tc.RayTriCollision(out res, ref tbegin, ref tend) ||
                 td.RayTriCollision(out res, ref tbegin, ref tend));
        }

        public Vector3[] GetEdges()
        {
            return new Vector3[]
            {
                renderVtx[2].pos, 
                renderVtx[1].pos, 
                renderVtx[8].pos, 
                renderVtx[3].pos, 
                renderVtx[2].pos
            };
        }

        [LocalizedCategory("PROP:SHAPE")]
        [LocalizedDescription("PROP:BlockModel")]
        [Browsable(true)]
        public override EditableModel Model
        {
            get
            {
                if (model == null)
                    model = new EditableModel();
                return model;
            }
            set
            {
                if (model != value && model != null)
                    model.Dispose();
                model = value;
                Changed();
            }
        }

        [LocalizedCategory("PROP:SHAPE")]
        [LocalizedDescription("PROP:DblLayers")]
        [Browsable(true)]
        public override bool HasDoubleLayers
        {
            get { return (bits & BlockBits.HasDoubleLayer) != 0; }
            set
            {
                if (value)
                {
                    bits |= BlockBits.HasDoubleLayer;
                    if (Material2 == null)
                        Material2 = new EditableBlockMaterial();

                    CalculateVertex();
                }
                else
                {
                    bits ^= BlockBits.HasDoubleLayer;
                }
                Changed();
            }
        }

        [LocalizedCategory("PROP:SHAPE")]
        [LocalizedDescription("PROP:Tmp3DModel")]
        [Browsable(true)]
        public override bool Has3DModel
        {
            get { return (bits & BlockBits.HasTileModel) != 0; }
            set
            {
                if (value)
                {
                    bits |= BlockBits.HasTileModel;
                    CalculateVertex();
                }
                else
                {
                    bits ^= BlockBits.HasTileModel;
                }
                Changed();
            }
        }

        [LocalizedDescription("PROP:Tmp3DeformationData")]
        [Browsable(true)]
        public override bool HasDeformationData
        {
            get { return (bits & BlockBits.HasDaformationData) != 0; }
        }

        [LocalizedDescription("PROP:RadarColorLeft")]
        public Color RadarLeftColor
        {
            get
            {
                return Color.FromArgb(255, radar_red_left, radar_green_left, radar_blue_left);
            }
            set
            {
                radar_red_left = value.R;
                radar_green_left = value.G;
                radar_blue_left = value.B;
                Changed();
            }
        }
        [LocalizedDescription("PROP:RadarColorRight")]
        public Color RadarRightColor
        {
            get
            {
                return Color.FromArgb(255, radar_red_right, radar_green_right, radar_blue_right);
            }
            set
            {
                radar_red_right = value.R;
                radar_green_right = value.G;
                radar_blue_right = value.B;
                Changed();
            }
        }

        [LocalizedCategory("PROP:MATE")]
        [LocalizedDescription("PROP:MeshMat1")]
        [DefaultValue(null)]
        public EditableBlockMaterial Material1
        {
            get
            {
                return mat1;
            }
            set
            {
                mat1 = value;
                Changed();
            }
        }
        [LocalizedCategory("PROP:MATE")]
        [LocalizedDescription("PROP:MeshMat2")]
        [DefaultValue(null)]
        public EditableBlockMaterial Material2
        {
            get
            {
                return mat2;
            }
            set
            {
                mat2 = value;
                Changed();
            }
        }
        [LocalizedCategory("PROP:Position")]
        [LocalizedDescription("PROP:CoordX")]
        public int X
        {
            get { return x; }
            set
            {
                x = value;
                transformation = Matrix.Translation(value * Terrain.HorizontalUnit, 0, Y * Terrain.HorizontalUnit);
                Changed();
            }
        }
        [LocalizedCategory("PROP:Position")]
        [LocalizedDescription("PROP:CoordY")]
        public int Y
        {
            get { return y; }
            set
            {
                y = value;
                transformation = Matrix.Translation(X * Terrain.HorizontalUnit, 0, value * Terrain.HorizontalUnit);
                Changed();
            }
        }
        [LocalizedCategory("PROP:Position")]
        [LocalizedDescription("PROP:HeightL1")]
        public short Height
        {
            get { return height; }
            set
            {
                int offset = value - height;
                height = value;

                vertex[0].pos.Y += offset * Terrain.VerticalUnit;
                vertex[1].pos.Y += offset * Terrain.VerticalUnit;
                vertex[2].pos.Y += offset * Terrain.VerticalUnit;
                vertex[3].pos.Y += offset * Terrain.VerticalUnit;
                vertex[4].pos.Y += offset * Terrain.VerticalUnit;
                CalculateVertex();
                Changed();
            }
        }
        //[LocalizedCategory("PROP:SHAPE")]
        [LocalizedDescription("PROP:TerrTypeL1")]
        [DefaultValue(TerrainType.Clear)]
        public TerrainType TerrainType
        {
            get { return terrain_type; }
            set
            {
                terrain_type = value;
                Changed();
            }
        }
        [LocalizedCategory("PROP:SHAPE")]
        [LocalizedDescription("PROP:RampTypeL1")]
        [Editor(typeof(RampTypeEditor), typeof(UITypeEditor))]
        public byte RampType
        {
            get { return ramp_type; }
            set
            {
                if (value > 20)
                {
                    throw new ArgumentOutOfRangeException();
                }

                ramp_type = value;
                vertex[0].pos.X = 0;
                vertex[0].pos.Z = 0;
                vertex[1].pos.X = 0;
                vertex[1].pos.Z = 0;
                vertex[2].pos.X = 0;
                vertex[2].pos.Z = 0;
                vertex[3].pos.X = 0;
                vertex[3].pos.Z = 0;
                vertex[4].pos.X = 0;
                vertex[4].pos.Z = 0;

                vertex[0].pos.Y = Height * Terrain.VerticalUnit;
                vertex[1].pos.Y = Height * Terrain.VerticalUnit;
                vertex[2].pos.Y = Height * Terrain.VerticalUnit;
                vertex[3].pos.Y = Height * Terrain.VerticalUnit;
                vertex[4].pos.Y = Height * Terrain.VerticalUnit;


                TileBase.SetRampVertex(value,
                    ref vertex[0].pos,
                    ref vertex[1].pos,
                    ref vertex[2].pos,
                    ref vertex[3].pos,
                    ref vertex[4].pos);
                Terrain.SetCellVertex(ref vertex[0].pos, ref vertex[1].pos,
                 ref vertex[2].pos, ref vertex[3].pos, ref vertex[4].pos);
                CalculateVertex();
                Changed();
            }
        }
        [LocalizedCategory("PROP:Position")]
        [LocalizedDescription("PROP:HeightL2")]
        public short SecHeight
        {
            get { return secHeight; }
            set
            {
                int offset = value - secHeight;
                secHeight = value;
                secVertex[0].pos.Y += offset * Terrain.VerticalUnit;
                secVertex[1].pos.Y += offset * Terrain.VerticalUnit;
                secVertex[2].pos.Y += offset * Terrain.VerticalUnit;
                secVertex[3].pos.Y += offset * Terrain.VerticalUnit;
                secVertex[4].pos.Y += offset * Terrain.VerticalUnit;
                CalculateVertex();
                Changed();
            }
        }
        //[LocalizedCategory("PROP:SHAPE")]
        [LocalizedDescription("PROP:TerrTypeL2")]
        [DefaultValue(TerrainType.Clear)]
        public TerrainType SecTerrainType
        {
            get { return secTerrType; }
            set
            {
                secTerrType = value;
                Changed();
            }
        }
        [LocalizedCategory("PROP:SHAPE")]
        [LocalizedDescription("PROP:RampTypeL2")]
        [Editor(typeof(RampTypeEditor), typeof(UITypeEditor))]
        public byte SecRampType
        {
            get { return secRampType; }
            set
            {
                if (value > 20)
                {
                    throw new ArgumentOutOfRangeException();
                }
                secRampType = value;
                secVertex[0].pos.X = 0;
                secVertex[0].pos.Z = 0;
                secVertex[1].pos.X = 0;
                secVertex[1].pos.Z = 0;
                secVertex[2].pos.X = 0;
                secVertex[2].pos.Z = 0;
                secVertex[3].pos.X = 0;
                secVertex[3].pos.Z = 0;
                secVertex[4].pos.X = 0;
                secVertex[4].pos.Z = 0;

                secVertex[0].pos.Y = SecHeight * Terrain.VerticalUnit;
                secVertex[1].pos.Y = SecHeight * Terrain.VerticalUnit;
                secVertex[2].pos.Y = SecHeight * Terrain.VerticalUnit;
                secVertex[3].pos.Y = SecHeight * Terrain.VerticalUnit;
                secVertex[4].pos.Y = SecHeight * Terrain.VerticalUnit;

                TileBase.SetRampVertex(value,
                    ref secVertex[0].pos,
                    ref secVertex[1].pos,
                    ref secVertex[2].pos,
                    ref secVertex[3].pos,
                    ref secVertex[4].pos);
                Terrain.SetCellVertex(ref secVertex[0].pos, ref secVertex[1].pos,
                    ref secVertex[2].pos, ref secVertex[3].pos, ref secVertex[4].pos);
                CalculateVertex();
                Changed();
            }
        }

        void CalculateVertex()
        {
            for (int i = 0; i < 12; i++)
            {
                renderVtx[i] = vertex[DrawIndex[i]];
                if ((i + 1) % 3 == 0)
                {
                    Vector3 n;
                    MathEx.ComputePlaneNormal(ref renderVtx[i].pos, ref renderVtx[i - 1].pos, ref renderVtx[i - 2].pos, out n);

                    renderVtx[i].n = n;
                    renderVtx[i - 1].n = n;
                    renderVtx[i - 2].n = n;

                }
                if (HasDoubleLayers)
                {
                    renderSecVtx[i] = secVertex[DrawIndex[i]];
                    if ((i + 1) % 3 == 0)
                    {
                        Vector3 n;
                        MathEx.ComputePlaneNormal(ref renderSecVtx[i].pos, ref renderSecVtx[i - 1].pos, ref renderSecVtx[i - 2].pos, out n);

                        renderSecVtx[i].n = n;
                        renderSecVtx[i - 1].n = n;
                        renderSecVtx[i - 2].n = n;
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(7);
            sb.Append(Program.StringTable["PROP:TMP3BLOCK"]);
            sb.Append(": X=");
            sb.Append(X.ToString());
            sb.Append(" Y=");
            sb.Append(Y.ToString());
            return sb.ToString();
        }

        public void Dispose()
        {
            changed = null;
            if (mat1 != null)
                mat1.Dispose();
            if (mat2 != null)
                mat2.Dispose();
            if (model != null)
                model.Dispose();
        }

        public void Render()
        {
            Device dev = GraphicsDevice.Instance.Device;

            dev.VertexFormat = TileVertex.Format;
            dev.SetTransform(TransformState.World, transformation);
            dev.SetRenderState<Cull>(RenderState.CullMode, Cull.None);
            //dev.SetRenderState(RenderState.ZWriteEnable, !Material1.IsTransparent);
            //dev.SetRenderState<Cull>(RenderState.CullMode, Material1.IsTwoSided ? Cull.None : Cull.Clockwise);
            dev.SetTexture(0, Material1.Texture);
           
            dev.Material = BlockMaterial.DefBlockColor;// this.Material1.mat;
            dev.DrawUserPrimitives<TileVertex>(PrimitiveType.TriangleList, 0, 4, renderVtx);
            if (HasDoubleLayers)
            {
                //dev.SetRenderState(RenderState.ZWriteEnable, !Material2.IsTransparent);
                //dev.SetRenderState<Cull>(RenderState.CullMode, Material2.IsTwoSided ? Cull.None : Cull.Clockwise);
                dev.SetTexture(0, Material2.Texture);
                dev.Material = BlockMaterial.DefBlockColor;// Material2.mat;

                dev.DrawUserPrimitives<TileVertex>(PrimitiveType.TriangleList, 0, 4, renderSecVtx);
            }

            if (Model != null)
            {
                if (Has3DModel)
                {
                    Model.Render();
                }
            }
        }

    }
}
