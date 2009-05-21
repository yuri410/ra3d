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
using System.Text;
using R3D.Base;
using R3D.Core;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine
{
    /// <summary>
    /// 一张纹理，打包了一些地块的纹理
    /// </summary>
    /// <remarks>用四叉树管理，指数差7</remarks>
    public class TileTexturePack : ITileTexturePack, IDisposable
    {
        Texture texture;

        ImageQuadTreeNode manager;

        bool isLocked;

        DataRectangle locked;
        Device device;

        /// <summary>
        /// 打包纹理的宽度
        /// </summary>
        int width;
        /// <summary>
        /// 打包纹理的高度
        /// </summary>
        int height;

        bool disposed;

        //List<int> tiles;

        public TileTexturePack(Device dev)
        {
            int w = dev.Capabilities.MaxTextureWidth;
            int h = dev.Capabilities.MaxTextureHeight;

            if (w > 1024)
                w = 1024;
            if (h > 1024)
                h = 1024;

            if (w != h)
            {
                if (w > h)
                {
                    w = h;
                }
                else
                {
                    h = w;
                }
            }

            width = w;
            height = h;

            device = dev;

            manager = new ImageQuadTreeNode((ushort)w, (ushort)h, 32);
            texture = new Texture(dev, width, height, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            texture.AutoMipGenerationFilter = TextureFilter.Anisotropic;
            //tiles = new List<int>();
        }

        public bool IsLocked
        {
            get { return isLocked; }
        }


        #region ITileTexturePack 成员

        public Texture Texture
        {
            get { return texture; }
        }

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

      
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tileImages"></param>
        /// <returns>分配的纹理</returns>
        public unsafe Rectangle[] Append(int index, ImageBase[] tileImages)
        {
            if (isLocked)
            {
                //int processed = 0;
                Point pos;

                // 分配到的图像存放区域
                List<Rectangle> rects = new List<Rectangle>(tileImages.Length);

                for (int i = 0; i < tileImages.Length; i++)
                {
                    ImageBase tileImage = tileImages[i];

                    if (tileImage != null)
                    {
                        // 申请一个大一圈的区域
                        if (manager.Find(tileImage.Width + 2, tileImage.Height + 2, out pos))
                        {

                            // 实际纹理区域
                            rects.Add(new Rectangle(pos.X + 1, pos.Y + 1, tileImage.Width, tileImage.Height));



                            uint[] buffer = new uint[tileImage.Width * tileImage.Height];

                            fixed (void* dst = &buffer[0])
                            {
                                tileImage.CopyDataARGB(dst);

                                uint* src = (uint*)dst;

                                // 开始地址
                                uint* ptr2 = (uint*)locked.Data.DataPointer.ToPointer() + pos.X + 1 + (pos.Y + 1) * width;

                                for (int j = 0; j < tileImage.Height; j++)
                                {
                                    Memory.Copy(src + j * tileImage.Width, ptr2, tileImage.Width * 4);// Helper.MemCopy(ptr2, src + j * tileImage.Width, tileImage.Width * 4);

                                    // 填两头的像素
                                    ptr2[-1] = ptr2[0];
                                    ptr2[tileImage.Width] = ptr2[tileImage.Width - 1];
                                    ptr2 += width;
                                }

                                // 上边
                                ptr2 = (uint*)locked.Data.DataPointer.ToPointer() + pos.X + 1 + pos.Y * width;
                                Memory.Copy(src, ptr2, tileImage.Width * 4);    //Helper.MemCopy(ptr2, src, tileImage.Width * 4);
                                ptr2[-1] = ptr2[0];
                                ptr2[tileImage.Width] = ptr2[tileImage.Width - 1];

                                // 下边
                                ptr2 = (uint*)locked.Data.DataPointer.ToPointer() + pos.X + 1 + (pos.Y + tileImage.Height + 1) * width;
                                Memory.Copy(src + +(tileImage.Height - 1) * tileImage.Width, ptr2, tileImage.Width * 4);
                                //Helper.MemCopy(ptr2, src + (tileImage.Height - 1) * tileImage.Width, tileImage.Width * 4);
                                ptr2[-1] = ptr2[0];
                                ptr2[tileImage.Width] = ptr2[tileImage.Width - 1];

                            }
                        }
                        else
                            break;
                    }
                    else
                    {
                        // Add a fake rectangle for nulls.
                        // The manager will know if it have a texture. This rect is a placeholder.
                        rects.Add(new Rectangle());
                    }
                }

                return rects.ToArray();
            }
            else
                throw new InvalidOperationException();
        }

        public void Lock()
        {
            if (!isLocked)
            {
                locked = texture.LockRectangle(0, LockFlags.None);
                isLocked = true;
            }
            else
                throw new InvalidOperationException();
        }

        public void Unlock()
        {
            if (isLocked)
            {
                texture.UnlockRectangle(0);
                isLocked = false;
            }
            else
                throw new InvalidOperationException();
        }

        public void Clear()
        {
            //tiles.Clear();
            manager.Reset();
        }
        #endregion

        public void Dispose()
        {
            if (!disposed)
            {
                texture.Dispose();
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        ~TileTexturePack()
        {
            if (!disposed)
                Dispose();
        }
    }
}
