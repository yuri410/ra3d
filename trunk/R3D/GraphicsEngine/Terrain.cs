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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.Collections;
using R3D.Core;
using R3D.GraphicsEngine.Effects;
//using R3D.Graphics.Effects;
using R3D.IO;
using R3D.IsoMap;
using R3D.Logic;
using R3D.MathLib;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TerrainVertex
    {
        public Vector3 pos;
        ////public Vector3 n;
        public float u0, v0;
        public float u1, v1;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Texture2; }
        }
    }

    public struct CellHeight
    {
        public float top;
        public float left;
        public float right;
        public float bottom;
        public float centre;

        public Vector3 Normal;
    }

    public unsafe class Terrain : SceneObject, IDisposable
    {
        const int NMWidthScale = 3;
        const int NMHeightScale = 3;

        /// <summary>
        /// 水平方向上单元长度
        /// </summary>
        public const float HorizontalUnit = 4f;
        /// <summary>
        /// 垂直方向上单元长度
        /// </summary>
        public const float VerticalUnit = 1.634f;// MathEx.Root2 * 2;

        bool disposed;

        TerrainTreeNode terrainTree;


        VertexBuffer[] vtxBuffer;
        IndexBuffer[] idxBuffer;
        /// <summary>
        /// 如果元素为null则相应的group没有第二层地块的数据
        /// </summary>
        IndexBuffer[] idxBuffer2;

        int[] ibSizes;
        int[] ibSizes2;
        //int groupCount;

        MeshMaterial[] materials;
        MeshMaterial[] secMaterials;
        MeshMaterial defTerrainMat;
        MeshMaterial defSecTerrainMat;

        Device device;

        TileTexturePackManager tileTexture;

        TileBase[][] tiles;

        /// <summary>
        /// HeightMap中的三角形的数目
        /// </summary>
        int totalTriangleCount;
        /// <summary>
        /// HeightMap中的顶点数目
        /// </summary>
        int totalVertexCount;

        /// <summary>
        /// HeightMap中的地形宽度
        /// </summary>
        int width;
        /// <summary>
        /// HeightMap中的地形高度
        /// </summary>
        int height;

        Battle gameBattle;

        Queue<TerrainTreeNode> queue;
        /// <summary>
        /// 用于 渲染时 临时存储和相应索引的材质对应的cell
        /// </summary>
        FastList<TerrainTreeNodeData>[] renderIdxBuffer;
        FastList<TerrainTreeNodeData> renderIBNoTex;

        FastList<RenderOperation> bufferedModelOPs;

        //FastList<RenderOperation> bufferedOps;
        //FastList<TerrainTreeNodeData> tileModelBuffer;
        //TileModelList tileModelList;

        Texture normalMap;
        Texture secNormalMap;


        VertexDeclaration vtxDecl;

        //int batchState;
        // useState[i]表示第i个组是否被用到
        //bool[] useState;
        Vector3 camPos;


        RenderOperation[] bufferedOPs;
       
        // lod范围，超出该范围由节点判断
        const float lodDistanceLmt = 150;
        // lod因子，在lodDistanceLmt中一定范围内的单元块会被细化。在摄像机离单元块lodDistanceLmt远时，单元块的最大lod因子。
        const float lodFactorLmt = 0.4f;

        //BoundingBox boundingBox;
        //Matrix transformation;


        public TerrainPicker Picker
        {
            get;
            set;
        }

        CellHeight[][] CellHeights
        {
            get;
            set;
        }

        public CellHeight[][] GetCellHeight()
        {
            CellHeight[][] res = CellHeights;
            CellHeights = null;
            return res;
        }

        void FillNormal(int x, int y,
            int normalMapWidth, int normalMapHeight, int* norMapData,
            TerrainVertex* left, TerrainVertex* right, TerrainVertex* top, TerrainVertex* bottom, TerrainVertex* centre)
        {
            // normalMap
            int tx = y * NMHeightScale;
            int ty = x * NMWidthScale;


            int* cellN1 = norMapData + ty * normalMapWidth + tx;
            int* cellN2 = cellN1 + 1;
            int* cellN3 = cellN1 + 2;
            int* cellN4 = cellN1 + normalMapWidth;
            int* cellN5 = cellN4 + 1;
            int* cellN6 = cellN4 + 2;
            int* cellN7 = cellN4 + normalMapWidth;
            int* cellN8 = cellN7 + 1;
            int* cellN9 = cellN7 + 2;


            Vector3 pn410;
            MathEx.ComputePlaneNormal(ref centre->pos, ref left->pos, ref top->pos, out pn410);

            Vector3 pn240;
            MathEx.ComputePlaneNormal(ref right->pos, ref centre->pos, ref top->pos, out pn240);

            Vector3 pn143;
            MathEx.ComputePlaneNormal(ref left->pos, ref centre->pos, ref bottom->pos, out pn143);

            Vector3 pn423;
            MathEx.ComputePlaneNormal(ref centre->pos, ref right->pos, ref bottom->pos, out pn423);


            Vector3.Negate(ref pn410, out pn410);
            Vector3.Negate(ref pn240, out pn240);
            Vector3.Negate(ref pn143, out pn143);
            Vector3.Negate(ref pn423, out pn423);

            cellN4[0] = MathEx.Vector2ARGB(ref pn240);
            cellN2[0] = MathEx.Vector2ARGB(ref pn410);
            cellN8[0] = MathEx.Vector2ARGB(ref pn423);
            cellN6[0] = MathEx.Vector2ARGB(ref pn143);

            Vector3 cn;
            Vector3.Add(ref pn410, ref pn240, out cn);
            Vector3.Add(ref cn, ref pn143, out cn);
            Vector3.Add(ref cn, ref pn423, out cn);
            cn.Normalize();
            cellN5[0] = MathEx.Vector2ARGB(ref cn);

            Vector3 v1, v2, v3, v4;
            Vector3.Add(ref pn410, ref pn240, out v1);
            Vector3.Add(ref pn410, ref pn143, out v2);
            Vector3.Add(ref pn240, ref pn423, out v3);
            Vector3.Add(ref pn423, ref pn143, out v4);

            v1.Normalize();
            v2.Normalize();
            v3.Normalize();
            v4.Normalize();

            cellN1[0] = MathEx.Vector2ARGB(ref v1);
            cellN3[0] = MathEx.Vector2ARGB(ref v2);
            cellN7[0] = MathEx.Vector2ARGB(ref v3);
            cellN9[0] = MathEx.Vector2ARGB(ref v4);


            CellHeights[x][y].Normal = cn;
        }

        void CalculateTexCoord(int x, int y, float invWidth, float invHeight, ref Rectangle rect, float packw, float packh,
            TerrainVertex* left, TerrainVertex* right, TerrainVertex* top, TerrainVertex* bottom, TerrainVertex* centre)
        {
            // cell
            //top->pos = new Vector3();
            top->u0 = (float)rect.Left / packw;
            top->v0 = (float)rect.Top / packh;
            top->u1 = y * invHeight;
            top->v1 = x * invWidth;

            //left->pos = new Vector3();
            left->u0 = (float)rect.Left / packw;
            left->v0 = (float)(rect.Top + rect.Height) / packh;
            left->u1 = (y + 1) * invHeight;
            left->v1 = x * invWidth;

            //right->pos = new Vector3();
            right->u0 = (float)(rect.Left + rect.Width) / packw;
            right->v0 = (float)(rect.Top) / packh;
            right->u1 = y * invHeight;
            right->v1 = (x + 1) * invWidth;

            //bottom->pos = new Vector3();
            bottom->u0 = (float)(rect.Left + rect.Width) / packw;
            bottom->v0 = (float)(rect.Top + rect.Height) / packh;
            bottom->u1 = (y + 1) * invHeight;
            bottom->v1 = (x + 1) * invWidth;


            centre->u0 = (float)(rect.Left + rect.Width * 0.5f) / packw;
            centre->v0 = (float)(rect.Top + rect.Height * 0.5f) / packh;
            centre->u1 = ((float)y + 0.5f) * invHeight;
            centre->v1 = ((float)x + 0.5f) * invWidth;
        }


        public Terrain(Battle bat, Device dev, MapBase map, TheaterBase theater, CellData[] cellData)
            : base(true)
        {
            gameBattle = bat;
            queue = new Queue<TerrainTreeNode>();
            device = dev;

            VertexElement[] elements = D3DX.DeclaratorFromFVF(TerrainVertex.Format);
            vtxDecl = new VertexDeclaration(device, elements);

            //HeightMap heightMap = map.GetHeightMap();

            this.width = map.Width + map.Height - 1;// heightMap.Width;
            this.height = this.width;

            tileTexture = new TileTexturePackManager(device, theater.TileCount);
            tileTexture.Lock();
            for (int i = 0; i < theater.TileCount; i++)
            {
                ImageBase[][] images = theater.GetTileTexture(i);

                if (images != null) // 如果有地块
                {
                    tileTexture.Append(i, images);
                }
            }
            tileTexture.Unlock();


            //Texture.ToFile(tileTexture.GetTileTexture(0).Texture, @"D:\Documents\Desktop\x.png", ImageFileFormat.Png);

            tiles = theater.Tiles;
            //supportsModel = new bool[tiles.Length][];
            //tileModels = new GameModel[tiles.Length][][];

            int normalMapWidth = width * NMWidthScale;
            int normalMapHeight = height * NMHeightScale;

            normalMap = new Texture(device, normalMapWidth, normalMapHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            normalMap.AutoMipGenerationFilter = TextureFilter.Anisotropic;
            secNormalMap = new Texture(device, normalMapWidth, normalMapHeight, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            secNormalMap.AutoMipGenerationFilter = TextureFilter.Anisotropic;

            int* norMapData = (int*)normalMap.LockRectangle(0, LockFlags.None).Data.DataPointer.ToPointer();
            int* secNorMapData = (int*)secNormalMap.LockRectangle(0, LockFlags.None).Data.DataPointer.ToPointer();


            int totalCellCount = cellData.Length;

            CellHeights = new CellHeight[width][];
            for (int i = 0; i < width; i++)
            {
                CellHeights[i] = new CellHeight[height];
            }

            // create buffers used for rendering
            // new TerrainTreeNodeDataList(totalCellCount);// new List<TerrainTreeNodeData>();

            renderIdxBuffer = new FastList<TerrainTreeNodeData>[tileTexture.Count];
            for (int i = 0; i < renderIdxBuffer.Length; i++)
            {
                renderIdxBuffer[i] = new FastList<TerrainTreeNodeData>(totalCellCount);
            }
            renderIBNoTex = new FastList<TerrainTreeNodeData>(totalCellCount);


            totalVertexCount = map.CellCount * 5;
            totalTriangleCount = map.CellCount * 4;


            // build terrain tree
            TerrainTreeNodeData[] treeData = new TerrainTreeNodeData[totalCellCount];

            int tileModelCount = 0;

            for (int i = 0; i < totalCellCount; i++)
            {
                treeData[i] = new TerrainTreeNodeData();
                treeData[i].cellIndex = i;
                treeData[i].x = cellData[i].x;
                treeData[i].y = cellData[i].y;



                //// rects[i]表示这个cell对应的subTile的第i种形态所占的区域
                //Rectangle[] rects = tileTexture
                //    [cellData[i].tile]
                //    [cellData[i].subTile];
                // packIndex[i]表示这个cell对应的subTile的第i种形态所在的纹理包
                int[] packIndex = tileTexture.GetOwnerTexPack(
                    cellData[i].tile,
                    cellData[i].subTile);

                bool[] hasTexture = tileTexture.GetHasTexture(
                    cellData[i].tile,
                    cellData[i].subTile);

                int tileImageRectIndex;
                if (packIndex.Length > 1)
                {
                    tileImageRectIndex = Randomizer.GetRandomInt(packIndex.Length);
                }
                else
                {
                    tileImageRectIndex = 0;
                }
                treeData[i].varision = tileImageRectIndex;

                BlockBits bit = tiles[cellData[i].tile][tileImageRectIndex].GetBlockBits(cellData[i].subTile);

                if ((bit & BlockBits.HasTileModel) == BlockBits.HasTileModel)
                {
                    treeData[i].model = tiles[cellData[i].tile][tileImageRectIndex].GetTileModel(cellData[i].subTile);
                    tileModelCount++;
                }

                treeData[i].visible = (bit & BlockBits.Invisible) != BlockBits.Invisible;



                ITileTexturePack pack = tileTexture.GetTileTexture(packIndex[tileImageRectIndex]);
                if (hasTexture[tileImageRectIndex])
                {
                    int tpIdx = packIndex[tileImageRectIndex];

                    treeData[i].texturePack = tpIdx;
                    renderIdxBuffer[tpIdx].Add(treeData[i]);
                }
                else
                {
                    treeData[i].texturePack = -1;
                    renderIBNoTex.Add(treeData[i]);
                }
            }


            terrainTree = new TerrainTreeNode(treeData, 0, 0, width, height, null);


            vtxBuffer = new VertexBuffer[tileTexture.Count + 1];
            idxBuffer = new IndexBuffer[tileTexture.Count + 1];
            idxBuffer2 = new IndexBuffer[tileTexture.Count + 1];
            ibSizes = new int[tileTexture.Count + 1];
            ibSizes2 = new int[tileTexture.Count + 1];
            //useState = new bool[tileTexture.Count];
            bufferedOPs = new RenderOperation[(tileTexture.Count + 1) * 2];
            for (int i = 0; i < bufferedOPs.Length; i++)
            {
                bufferedOPs[i].Geomentry = new GeomentryData(this);
                bufferedOPs[i].Geomentry.Format = TerrainVertex.Format;
                bufferedOPs[i].Geomentry.VertexSize = sizeof(TerrainVertex);
                bufferedOPs[i].Geomentry.VertexDeclaration = vtxDecl;
                bufferedOPs[i].Geomentry.PrimitiveType = PrimitiveType.TriangleList;                
            }

            TerrainVertex*[] vbPointers = new TerrainVertex*[tileTexture.Count+1];

            float invHeight = 1.0f / height;
            float invWidth = 1.0f / width;



            for (int i = 0; i < tileTexture.Count + 1; i++)
            {
                FastList<TerrainTreeNodeData> tileTexCells;
                if (i == 0)
                {
                    tileTexCells = renderIBNoTex;
                }
                else
                {
                    tileTexCells = renderIdxBuffer[i - 1];
                }

                // 绘制时的IB和VB对应
                int cellCount = tileTexCells.Count;

                if (cellCount > 0)
                {
                    int dblLayerCount = 0;
                    int dblLayerIndex = cellCount;

                    // 预先检查双层block
                    for (int j = 0; j < cellCount; j++)
                    {
                        int cellIndex = tileTexCells[j].cellIndex;

                        BlockBits bit = tiles[cellData[cellIndex].tile][treeData[cellIndex].varision].GetBlockBits(cellData[cellIndex].subTile);
                        if ((bit & BlockBits.HasDoubleLayer) == BlockBits.HasDoubleLayer)
                        {
                            dblLayerCount++;
                        }
                    }

                    int vertexCount = (cellCount + dblLayerCount) * 5;

                    int ibSize = 4 * 3 * cellCount * 4;
                    idxBuffer[i] = new IndexBuffer(dev, ibSize, Usage.Dynamic | Usage.WriteOnly, Pool.Default, false);
                    ibSizes[i] = ibSize;

                    if (dblLayerCount > 0)
                    {
                        ibSize = 4 * 3 * dblLayerCount * 4;
                        idxBuffer2[i] = new IndexBuffer(dev, ibSize, Usage.Dynamic | Usage.WriteOnly, Pool.Default, false);
                        ibSizes2[i] = ibSize;
                    }

                    int vbSize = sizeof(TerrainVertex) * vertexCount;
                    vtxBuffer[i] = new VertexBuffer(device, vbSize, Usage.None, TerrainVertex.Format, Pool.Managed);

                    TerrainVertex* dst = (TerrainVertex*)vtxBuffer[i].Lock(0, 0, LockFlags.None).DataPointer.ToPointer();
                    vbPointers[i] = dst;

                    for (int j = 0; j < cellCount; j++)
                    {
                        // cell在treeData中的索引
                        int cellIndex = tileTexCells[j].cellIndex;

                        int tileImageRectIndex = treeData[cellIndex].varision;

                        // packIndex[i]表示这个cell对应的subTile的第i种形态所在的纹理包
                        int[] packIndex = tileTexture.GetOwnerTexPack(
                            cellData[cellIndex].tile,
                            cellData[cellIndex].subTile);
                        // rects[i]表示这个cell对应的subTile的第i种形态所占的区域
                        Rectangle[] rects = tileTexture
                            [cellData[cellIndex].tile]
                            [cellData[cellIndex].subTile];

                        ITileTexturePack pack = tileTexture.GetTileTexture(packIndex[tileImageRectIndex]);

                        float packw = pack.Width;
                        float packh = pack.Height;

                        int x = treeData[cellIndex].x;
                        int y = treeData[cellIndex].y;


                        TerrainVertex* top = dst + j * 5;
                        TerrainVertex* left = dst + j * 5 + 1;
                        TerrainVertex* right = dst + j * 5 + 2;
                        TerrainVertex* bottom = dst + j * 5 + 3;
                        TerrainVertex* centre = dst + j * 5 + 4;


                        CalculateTexCoord(x, y, invWidth, invHeight, ref rects[tileImageRectIndex], packw, packh,
                            left, right, top, bottom, centre);


                        float px = x * HorizontalUnit;
                        float py = cellData[cellIndex].z * VerticalUnit;
                        float pz = y * HorizontalUnit;

                        top->pos = new Vector3(px, py, pz);
                        left->pos = new Vector3(px, py, pz + HorizontalUnit);
                        right->pos = new Vector3(px + HorizontalUnit, py, pz);
                        bottom->pos = new Vector3(px + HorizontalUnit, py, pz + HorizontalUnit);
                        centre->pos = new Vector3(px + HorizontalUnit * 0.5f, py, pz + HorizontalUnit * 0.5f);

                        map.SetCellData(cellIndex, false, top, left, right, bottom, centre);

                        CellHeights[x][y].top = top->pos.Y;
                        CellHeights[x][y].left = left->pos.Y;
                        CellHeights[x][y].right = right->pos.Y;
                        CellHeights[x][y].bottom = bottom->pos.Y;
                        CellHeights[x][y].centre = centre->pos.Y;


                        FillNormal(x, y, normalMapWidth, normalMapHeight, norMapData, left, right, top, bottom, centre);

                        float avgHeight = 0.2f * (top->pos.Y + left->pos.Y + right->pos.Y + bottom->pos.Y + centre->pos.Y);

                        float lodFactor =
                            MathEx.Sqr(top->pos.Y + bottom->pos.Y - 2 * avgHeight) +
                            MathEx.Sqr(left->pos.Y + right->pos.Y - 2 * avgHeight) +
                            MathEx.Sqr(centre->pos.Y - avgHeight);

                        treeData[cellIndex].lodFactor = 1.0f / lodFactor;
                        treeData[cellIndex].lodHeight = avgHeight;

                        treeData[cellIndex].vbIndex = j * 5;

                        BlockBits bit = tiles[cellData[cellIndex].tile][tileImageRectIndex].GetBlockBits(cellData[cellIndex].subTile);
                        if ((bit & BlockBits.HasDoubleLayer) == BlockBits.HasDoubleLayer)
                        {
                            treeData[cellIndex].vbSecIndex = 5 * dblLayerIndex;

                            top = dst + dblLayerIndex * 5;
                            left = dst + dblLayerIndex * 5 + 1;
                            right = dst + dblLayerIndex * 5 + 2;
                            bottom = dst + dblLayerIndex * 5 + 3;
                            centre = dst + dblLayerIndex * 5 + 4;

                            CalculateTexCoord(x, y, invWidth, invHeight, ref rects[tileImageRectIndex], packw, packh,
                                left, right, top, bottom, centre);

                            top->pos = new Vector3(px, py, pz);
                            left->pos = new Vector3(px, py, pz + HorizontalUnit);
                            right->pos = new Vector3(px + HorizontalUnit, py, pz);
                            bottom->pos = new Vector3(px + HorizontalUnit, py, pz + HorizontalUnit);
                            centre->pos = new Vector3(px + HorizontalUnit * 0.5f, py, pz + HorizontalUnit * 0.5f);

                            map.SetCellData(cellIndex, true, top, left, right, bottom, centre);

                            FillNormal(x, y, normalMapWidth, normalMapHeight, secNorMapData, left, right, top, bottom, centre);

                            avgHeight = 0.2f * (top->pos.Y + left->pos.Y + right->pos.Y + bottom->pos.Y + centre->pos.Y);
                            lodFactor =
                                   MathEx.Sqr(top->pos.Y + bottom->pos.Y - 2 * avgHeight) +
                                   MathEx.Sqr(left->pos.Y + right->pos.Y - 2 * avgHeight) +
                                   MathEx.Sqr(centre->pos.Y - avgHeight);

                            treeData[cellIndex].lodFactor2 = 1.0f / lodFactor;
                            treeData[cellIndex].lodHeight2 = avgHeight;

                            dblLayerIndex++;
                        }
                    }
                }
            }

            normalMap.UnlockRectangle(0);
            secNormalMap.UnlockRectangle(0);

            //tileModelBuffer = new FastList<TerrainTreeNodeData>(tileModelCount);
            bufferedModelOPs = new FastList<RenderOperation>(tileModelCount);
            //bufferedOps = new FastList<RenderOperation>(tileModelCount);
            //Texture.ToFile(normalMap, @"C:\Documents and Settings\Yuri\桌面\test.png", ImageFileFormat.Png);


            CreateTerrainMaterials();
            //bufferedOps = new RenderOperation[groupCount * tileTexture.Count * 2];

            terrainTree.CalculateBoudingVolume(vbPointers);

            //Vector3 rVec = new Vector3(terrainTree.bounding.Radius);
            //Vector3.Add(ref terrainTree.bounding.Center, ref rVec, out BoundingBox.Maximum);
            //Vector3.Subtract(ref terrainTree.bounding.Center, ref rVec, out BoundingBox.Minimum);
            base.BoundingSphere = terrainTree.bounding;

            vbPointers = null;
            for (int i = 0; i < vtxBuffer.Length; i++)
            {
                if (vtxBuffer[i] != null)
                    vtxBuffer[i].Unlock();
            }
            //groups = null;

            renderIBNoTex.FastClear();
            for (int i = 0; i < renderIdxBuffer.Length; i++)
            {
                renderIdxBuffer[i].FastClear();
            }


        }

        void CreateTerrainMaterials()
        {
            defTerrainMat = new MeshMaterial(device);
            defTerrainMat.mat = BlockMaterial.DefBlockColor;
            defTerrainMat.SetTexture(0, normalMap);
            defTerrainMat.CullMode = Cull.Counterclockwise;
            defTerrainMat.SetEffect(EffectManager.Instance.GetModelEffect(TerrainLightingEffectFactory.Name));

            defSecTerrainMat = new MeshMaterial(device);
            defSecTerrainMat.mat = BlockMaterial.DefBlockColor;
            defSecTerrainMat.SetTexture(0, normalMap);
            defSecTerrainMat.CullMode = Cull.Counterclockwise;
            defSecTerrainMat.SetEffect(EffectManager.Instance.GetModelEffect(TerrainLightingEffectFactory.Name));

            materials = new MeshMaterial[tileTexture.Count];
            for (int i = 0; i < tileTexture.Count; i++)
            {
                materials[i] = new MeshMaterial(device);
                materials[i].mat = BlockMaterial.DefBlockColor;
                materials[i].SetTexture(0, normalMap);
                materials[i].SetTexture(1, tileTexture.GetTileTexture(i).Texture);
                materials[i].CullMode = Cull.Counterclockwise;
                materials[i].SetEffect(EffectManager.Instance.GetModelEffect(TerrainLightingEffectFactory.Name));
            }

            secMaterials = new MeshMaterial[tileTexture.Count];
            for (int i = 0; i < tileTexture.Count; i++)
            {
                secMaterials[i] = new MeshMaterial(device);
                secMaterials[i].mat = BlockMaterial.DefBlockColor;
                secMaterials[i].SetTexture(0, secNormalMap);
                secMaterials[i].SetTexture(1, tileTexture.GetTileTexture(i).Texture);
                secMaterials[i].CullMode = Cull.Counterclockwise;
                secMaterials[i].SetEffect(EffectManager.Instance.GetModelEffect(TerrainLightingEffectFactory.Name));
            }
        }

        public BoundingSphere BoundingVolume
        {
            get { return terrainTree.bounding; }
        }

        public static void SetCellVertex(ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4, ref Vector3 vc)
        {
            v2.Z += HorizontalUnit;

            v3.X += HorizontalUnit;

            v4.X += HorizontalUnit;
            v4.Z += HorizontalUnit;

            vc.X += HorizontalUnit * 0.5f;
            vc.Z += HorizontalUnit * 0.5f;
        }

        public override void PrepareVisibleObjects(Camera cam)
        {
            for (int i = 0; i < tileTexture.Count; i++)
            {
                renderIdxBuffer[i].FastClear();
            }
            renderIBNoTex.FastClear();
            //tileModelBuffer.FastClear();
            bufferedModelOPs.FastClear();

            Frustum frus = cam.Frustum;
            //Vector3 camDirection = cam.Front;
            camPos = cam.Position;


            // BFS，遍历时检查TexturePack
            queue.Enqueue(terrainTree);
            while (queue.Count > 0)
            {
                TerrainTreeNode node = queue.Dequeue();

                if (node.ChildrenCount != 0)
                {
                    if (node[0] != null)
                    {
                        if (frus.IsSphereIn(ref node[0].bounding.Center, node[0].bounding.Radius))
                        {
                            queue.Enqueue(node[0]);
                        }
                    }
                    if (node[1] != null)
                    {
                        if (frus.IsSphereIn(ref node[1].bounding.Center, node[1].bounding.Radius))
                        {
                            queue.Enqueue(node[1]);
                        }
                    }
                    if (node[2] != null)
                    {
                        if (frus.IsSphereIn(ref node[2].bounding.Center, node[2].bounding.Radius))
                        {
                            queue.Enqueue(node[2]);
                        }
                    }
                    if (node[3] != null)
                    {
                        if (frus.IsSphereIn(ref node[3].bounding.Center, node[3].bounding.Radius))
                        {
                            queue.Enqueue(node[3]);
                        }
                    }
                }
                else
                {
                    TerrainTreeNodeData[] data = node.GetData();

                    if (Picker != null)
                    {
                        Picker.Intersect(node);
                    }

                    bool ignoreLod =
                        (MathEx.Sqr(node.bounding.Center.X - camPos.X) +
                         MathEx.Sqr(node.bounding.Center.Y - camPos.Y) +
                         MathEx.Sqr(node.bounding.Center.Z - camPos.Z)) > MathEx.Sqr(lodDistanceLmt + node.bounding.Radius);

                    for (int i = 0; i < data.Length; i++)
                    {
                        TerrainTreeNodeData current = data[i];

                        if (current.visible)
                        {
                            if (current.texturePack != -1)
                            {
                                renderIdxBuffer[current.texturePack].Add(current);
                            }
                            else
                            {
                                renderIBNoTex.Add(current);
                            }
                            current.ignoreLod = ignoreLod;
                            //useState[current.groupIdx] = true;
                        }

                        if (current.model != null)
                        {
                            RenderOperation[] ops = current.model.GetRenderOperation();

                            Matrix trans = Matrix.Translation(current.x * HorizontalUnit, current.lodHeight, current.y * HorizontalUnit);

                            for (int k = 0; k < ops.Length; k++)
                            {
                                Matrix.Multiply(ref ops[k].Transformation, ref trans, out ops[k].Transformation);

                                bufferedModelOPs.Add(ops[k]);
                            }

                            //tileModelBuffer.Add(current);
                        }

                    }
                }
            }

            //BatchesNeeded = tileTexture.Count + 2;
            //batchState = -2;
        }

        public override RenderOperation[] GetRenderOperation()
        {
            int vtxCount = 0;
            int vtxCount2 = 0;
  

            for (int i = 0; i < tileTexture.Count + 1; i++)
            {
                FastList<TerrainTreeNodeData> currentNodeList = i == 0 ? renderIBNoTex : renderIdxBuffer[i - 1];
                if (currentNodeList.Count == 0)
                {
                    bufferedOPs[i * 2].Geomentry.VertexCount = 0;
                    bufferedOPs[i * 2 + 1].Geomentry.VertexCount = 0;
                    // oops，没有要画的cell
                    continue;
                }

                IndexStream32 stream =null ;
                if (idxBuffer[i] != null)
                {
                    stream = new IndexStream32((uint*)idxBuffer[i].Lock(0, ibSizes[i], LockFlags.None).DataPointer.ToPointer(), ibSizes[i]);
                }
                IndexStream32 stream2 = null;
                if (idxBuffer2[i] != null)
                {
                    stream2 = new IndexStream32((uint*)idxBuffer2[i].Lock(0, ibSizes2[i], LockFlags.None).DataPointer.ToPointer(), ibSizes2[i]);
                }

                for (int j = 0; j < currentNodeList.Count; j++)
                {
                    TerrainTreeNodeData current = currentNodeList[j];

                    bool passed = !current.ignoreLod && !float.IsInfinity(current.lodFactor);
                    float lodLeft = 0;
                    if (passed)
                    {
                        lodLeft = current.lodFactor *
                            (MathEx.Sqr(camPos.X - current.x * HorizontalUnit) +
                             MathEx.Sqr(camPos.Y - current.lodHeight) +
                             MathEx.Sqr(camPos.Z - current.y * HorizontalUnit));
                    }


                    if (passed &&
                        lodLeft < (lodFactorLmt * lodFactorLmt * lodDistanceLmt * lodDistanceLmt))
                    {
                        vtxCount += 5;

                        stream.Write((uint)(current.vbIndex + 4));
                        stream.Write((uint)(current.vbIndex + 1));
                        stream.Write((uint)(current.vbIndex));

                        stream.Write((uint)(current.vbIndex + 2));
                        stream.Write((uint)(current.vbIndex + 4));
                        stream.Write((uint)(current.vbIndex));

                        stream.Write((uint)(current.vbIndex + 1));
                        stream.Write((uint)(current.vbIndex + 4));
                        stream.Write((uint)(current.vbIndex + 3));

                        stream.Write((uint)(current.vbIndex + 4));
                        stream.Write((uint)(current.vbIndex + 2));
                        stream.Write((uint)(current.vbIndex + 3));
                    }
                    else
                    {
                        vtxCount += 4;

                        stream.Write((uint)(current.vbIndex));
                        stream.Write((uint)(current.vbIndex + 3));
                        stream.Write((uint)(current.vbIndex + 1));

                        stream.Write((uint)(current.vbIndex));
                        stream.Write((uint)(current.vbIndex + 2));
                        stream.Write((uint)(current.vbIndex + 3));
                    }
                    if (current.vbSecIndex != TerrainTreeNodeData.NoIndex)
                    {
                        passed = !current.ignoreLod && !float.IsInfinity(current.lodFactor2);
                        lodLeft = 0;
                        if (passed)
                        {
                            lodLeft = current.lodFactor2 *
                                (MathEx.Sqr(camPos.X - current.x * HorizontalUnit) +
                                 MathEx.Sqr(camPos.Y - current.lodHeight) +
                                 MathEx.Sqr(camPos.Z - current.y * HorizontalUnit));
                        }

                        if (passed &&
                            lodLeft < (lodFactorLmt * lodFactorLmt * lodDistanceLmt * lodDistanceLmt))                    
                        //if (!current.ignoreLod &&
                        //    (current.lodFactor2 *
                        //        (MathEx.Sqr(camPos.X - current.x * HorizontalUnit) +
                        //         MathEx.Sqr(camPos.Y - current.lodHeight2) +
                        //         MathEx.Sqr(camPos.Z - current.y * HorizontalUnit))
                        //     < (lodFactorLmt * lodFactorLmt * lodDistanceLmt * lodDistanceLmt)))
                        {
                            vtxCount2 += 5;

                            stream2.Write((uint)(current.vbSecIndex + 4));
                            stream2.Write((uint)(current.vbSecIndex + 1));
                            stream2.Write((uint)(current.vbSecIndex));

                            stream2.Write((uint)(current.vbSecIndex + 2));
                            stream2.Write((uint)(current.vbSecIndex + 4));
                            stream2.Write((uint)(current.vbSecIndex));

                            stream2.Write((uint)(current.vbSecIndex + 1));
                            stream2.Write((uint)(current.vbSecIndex + 4));
                            stream2.Write((uint)(current.vbSecIndex + 3));

                            stream2.Write((uint)(current.vbSecIndex + 4));
                            stream2.Write((uint)(current.vbSecIndex + 2));
                            stream2.Write((uint)(current.vbSecIndex + 3));
                        }
                        else
                        {
                            vtxCount2 += 4;

                            stream2.Write((uint)(current.vbSecIndex));
                            stream2.Write((uint)(current.vbSecIndex + 3));
                            stream2.Write((uint)(current.vbSecIndex + 1));

                            stream2.Write((uint)(current.vbSecIndex));
                            stream2.Write((uint)(current.vbSecIndex + 2));
                            stream2.Write((uint)(current.vbSecIndex + 3));
                        }
                    }
                }

                idxBuffer[i].Unlock();
                if (idxBuffer2[i] != null)
                {
                    idxBuffer2[i].Unlock();
                }


                int i2 = i * 2;

                if (idxBuffer[i] != null && stream.Position > 0)
                {
                    bufferedOPs[i2].Geomentry.IndexBuffer = idxBuffer[i];
                    if (i == 0)
                    {
                        bufferedOPs[i2].Material = defTerrainMat;
                    }
                    else
                    {
                        bufferedOPs[i2].Material = materials[i - 1];
                    }

                    bufferedOPs[i2].Geomentry.PrimCount = stream.Position / 3;
                    bufferedOPs[i2].Geomentry.VertexCount = vtxCount;
                    bufferedOPs[i2].Geomentry.VertexBuffer = vtxBuffer[i];
                    bufferedOPs[i2].Transformation = Transformation;
                }
                else
                {
                    bufferedOPs[i2].Geomentry.VertexCount = 0;
                }

                if (idxBuffer2[i] != null && stream2.Position > 0)
                {
                    bufferedOPs[i2 + 1].Geomentry.IndexBuffer = idxBuffer2[i];
                    if (i2 == 0)
                    {
                        bufferedOPs[i2 + 1].Material = defTerrainMat;
                    }
                    else
                    {
                        bufferedOPs[i2 + 1].Material = secMaterials[i - 1];
                    }

                    bufferedOPs[i2 + 1].Geomentry.PrimCount = stream.Position / 3;
                    bufferedOPs[i2 + 1].Geomentry.VertexCount = vtxCount2;
                    bufferedOPs[i2 + 1].Geomentry.VertexBuffer = vtxBuffer[i];
                    bufferedOPs[i2 + 1].Transformation = Transformation;
                }
                else
                {
                    bufferedOPs[i2 + 1].Geomentry.VertexCount = 0;
                }



            }

            //return bufferedOPs;
            //bufferedOps.FastClear ();

            RenderOperation[] ops = new RenderOperation[bufferedOPs.Length + bufferedModelOPs.Count];
            Array.Copy(bufferedOPs, 0, ops, 0, bufferedOPs.Length);
            Array.Copy(bufferedModelOPs.Elements, 0, ops, bufferedOPs.Length, bufferedModelOPs.Count);
            return ops;
        }



        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                bufferedModelOPs.Clear();
                bufferedModelOPs = null;
                //tileModelBuffer.Clear();
                for (int i = 0; i < tileTexture.Count; i++)
                {
                    renderIdxBuffer[i].Clear();
                }
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].Dispose(false);
                }
                for (int i = 0; i < secMaterials.Length; i++)
                {
                    secMaterials[i].Dispose(false);
                }

                normalMap.Dispose();
                secNormalMap.Dispose();
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion

        ~Terrain()
        {
            if (!disposed)
                Dispose();
        }
    }

}
