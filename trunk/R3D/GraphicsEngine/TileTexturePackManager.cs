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
using System.Text;
using System.Drawing;

using R3D.Media;
using R3D.Base;

using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine
{

    public class TileTexturePackManager
    {
        Device device;

        //// tileRects[i][j] 表示第i地块的所有形态的纹理所占的区域
        //Rectangle[][] tileRects;
        
        /// <summary>
        /// tileRects[i][j][k] 表示第i地块的第j个subTile的第k种形态的纹理所占的区域
        /// </summary>
        Rectangle[][][] tileRects;

        /// <summary>
        /// tileOwnerTable[i][j][k] 表示i地块的第j个subTile的第k种纹理所在的纹理包的索引
        /// </summary>
        /// <remarks>使用int便于分类排序</remarks>
        int[][][] tileOwnerTable;

        /// <summary>
        /// hasTexture[i][j][k] 表示i地块的第j个subTile的第k种是否存在纹理
        /// </summary>
        bool[][][] hasTexture;

        //记录所在texturePack，tileOwnerTable[i]表示第i个地块的纹理所在的 所有纹理包
        //ITileTexturePack[][] tileOwnerTable;

        List<ITileTexturePack> textures;
        bool isLocked;

        public unsafe TileTexturePackManager(Device dev, int tileCount)
        {
            device = dev;
            textures = new List<ITileTexturePack>();
            tileOwnerTable = new int[tileCount][][];
            tileRects = new Rectangle[tileCount][][];
            hasTexture = new bool[tileCount][][];
            //textures.Add(new TileTexturePack(dev));
        }

        public ITileTexturePack GetTileTexture(int i)
        {
            return textures[i];
        }
        public Rectangle[][] this[int tileIndex]
        {
            get { return tileRects[tileIndex]; }
        }
        public bool[] GetHasTexture(int tileIndex, int subTile)
        {
            return hasTexture[tileIndex][subTile];
        }
        public int[] GetOwnerTexPack(int tileIndex, int subTile)
        {
            return tileOwnerTable[tileIndex][subTile];
        }
        public int[][] GetOwnerTexPack(int tileIndex)
        {
            return tileOwnerTable[tileIndex];
        }


        unsafe void append(int tileIdx, int subTile, ImageBase[] tileImg)
        {
            for (int i = 0; i < textures.Count; i++)
            {
                // 已经分配的纹理的区域
                Rectangle[] res = textures[i].Append(tileIdx, tileImg);

                int resLength = res.Length;
                int tileImgLength = tileImg.Length;

                // 如果都分配
                if (resLength == tileImgLength)
                {
                    //tileRects[tileIdx] = res; //记录分配的区域以便以后查询
                    //tileOwnerTable[tileIdx] = new ITileTexturePack[1] { textures[i] };
                    tileRects[tileIdx][subTile] = res; //记录分配的区域以便以后查询
                    tileOwnerTable[tileIdx][subTile] = new int[resLength];
                    hasTexture[tileIdx][subTile] = new bool[resLength];

                    for (int j = 0; j < resLength; j++)
                    {
                        hasTexture[tileIdx][subTile][j] = tileImg[j] != null;
                        tileOwnerTable[tileIdx][subTile][j] = i;
                    }
                    //fixed (int* dst = &tileOwnerTable[tileIdx][subTile][0])
                    //{
                    //    Helper.MemSet(dst, i, resLength * sizeof(int));
                    //}
                    return;
                }
                else if (resLength != 0) // 部分分配
                {
                    ImageBase[] remainData = new ImageBase[tileImgLength - resLength];

                    Array.Copy(tileImg, resLength, remainData, 0, tileImgLength - resLength);

                    // 准备新的纹理包
                    textures.Add(new TileTexturePack(device));
                    textures[textures.Count - 1].Lock();

                    Rectangle[] res2 = textures[textures.Count - 1].Append(tileIdx, remainData);
                    if (res2.Length == 0)
                        throw new OutOfMemoryException();



                    Rectangle[] toAdd = new Rectangle[tileImgLength];
                    Array.Copy(res, toAdd, resLength);
                    Array.Copy(res2, 0, toAdd, resLength, res2.Length);
                    tileRects[tileIdx][subTile] = toAdd;
                    tileOwnerTable[tileIdx][subTile] = new int[tileImgLength];

                    for (int k = 0; k < tileImgLength; k++)
                    {
                        tileOwnerTable[tileIdx][subTile][k] = k < resLength ? i : textures.Count - 1;
                        hasTexture[tileIdx][subTile][k] = tileImg[k] != null;
                    }
                    //tileRects[tileIdx] = toAdd;
                    //tileOwnerTable[tileIdx] = new ITileTexturePack[2]
                    //        {
                    //            textures[i],
                    //            textures[textures.Count - 1]
                    //        };

                    return;
                }
            }
            textures.Add(new TileTexturePack(device));
            textures[textures.Count - 1].Lock();
            append(tileIdx, subTile, tileImg);
        }

        public unsafe void Append(int tileIdx, ImageBase[][] tileImg)
        {
            if (isLocked)
            {
                int blockCount = tileImg.Length;
                // 分配数组
                tileRects[tileIdx] = new Rectangle[blockCount][];
                tileOwnerTable[tileIdx] = new int[blockCount][];
                hasTexture[tileIdx] = new bool[blockCount][];

                for (int i = 0; i < blockCount; i++)
                {
                    append(tileIdx, i, tileImg[i]);
                }

                //for (int i = 0; i < textures.Count; i++)
                //{

                //    bool passed = false;

                //    // 对于每个subTile
                //    for (int j = 0; j < blockCount; j++)
                //    {
                //        // 已经分配的纹理的区域
                //        Rectangle[] res = textures[i].Append(tileIdx, tileImg[j]);

                //        int resLength = res.Length;
                //        int tileImgLength = tileImg[j].Length; // 形态种类

                //        // 如果都分配
                //        if (resLength == tileImgLength)
                //        {
                //            tileRects[tileIdx][j] = res; //记录分配的区域以便以后查询
                //            tileOwnerTable[tileIdx][j] = new int[resLength];
                //            fixed (int* dst = &tileOwnerTable[tileIdx][j][0])
                //            {
                //                Helper.MemSet(dst, i, resLength * sizeof(int));
                //            }
                //            //tileOwnerTable[tileIdx] = new ITileTexturePack[1] { textures[i] };
                //            //return;                                                       
                //            passed = true;
                //        }
                //        else if (resLength != 0) // 部分分配
                //        {
                //            ImageBase[] remainData = new ImageBase[tileImgLength - resLength];

                //            Array.Copy(tileImg[j], resLength, remainData, 0, tileImgLength - resLength);

                //            // 准备新的纹理包
                //            textures.Add(new TileTexturePack(device));
                //            textures[textures.Count - 1].Lock();

                //            Rectangle[] res2 = textures[textures.Count - 1].Append(tileIdx, remainData);
                //            if (res2.Length == 0)
                //                throw new OutOfMemoryException();

                //            Rectangle[] toAdd = new Rectangle[tileImgLength];
                //            Array.Copy(res, toAdd, resLength);
                //            Array.Copy(res2, 0, toAdd, resLength, res2.Length);

                //            tileRects[tileIdx][j] = toAdd;
                //            tileOwnerTable[tileIdx][j] = new int[resLength];

                //            for (int k = 0; k < tileImgLength; k++)
                //            {
                //                tileOwnerTable[tileIdx][j][k] = k < resLength ? i : textures.Count - 1;
                //            }
                //            //return;
                //            passed = true;
                //        }
                //        else
                //        {
                //            break;
                //        }

                //    }  // for j

                //    if (passed)
                //    {
                //        return;
                //    }

                //}  // for i  
                //textures.Add(new TileTexturePack(device));
                //    textures[textures.Count - 1].Lock();
                //    Append(tileIdx, tileImg);

            }
            else
                throw new InvalidOperationException();
        }

        public int Count
        {
            get { return textures.Count; }
        }

        public bool IsLocked
        {
            get { return isLocked; }
        }

        public void Lock()
        {
            if (!isLocked)
            {
                for (int i = 0; i < textures.Count; i++)
                    textures[i].Lock();
                isLocked = true;
            }
            else
                throw new InvalidOperationException();
        }
        public void Unlock()
        {
            if (isLocked)
            {
                for (int i = 0; i < textures.Count; i++)
                    textures[i].Unlock();
            }
            else
                throw new InvalidOperationException();
        }

        public void Clear()
        {
            if (!isLocked)
            {
                for (int i = 0; i < textures.Count; i++)
                {
                    textures[i].Clear();
                }
                textures.Clear();
            }
            else
                throw new InvalidOperationException();
        }

    }
}
