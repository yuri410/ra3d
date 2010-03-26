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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using R3D.IO;
using R3D.Base;

namespace R3D.Media
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 由于需要调色板，Bitmap不能在构造时生成
    /// </remarks>
    public class ShpAnim : AnimBase
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct ShpOffsetUnit
        {
            public ushort x, y;
            public ushort cx, cy;
            public int compression;
            //public int unk;
            public byte radar_red;
            public byte radar_green;
            public byte radar_blue;
            public byte unknown;
            public int zero;
            public int offset;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct ShpHeader
        {
            public ushort zero;
            public ushort cx, cy;
            public ushort imgCount;

        }
        byte[][][] frames;
        ShpOffsetUnit[] frameHeaders;

        Palette pal;
        bool disposed;

        unsafe void Load(ResourceLocation fl)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(fl);

            if (br.ReadInt16() != 0)
            {
                br.Close();
                throw new DataFormatException(fl);
            }

            width = (int)br.ReadUInt16();
            height = (int)br.ReadUInt16();
            imgCount = (int)br.ReadUInt16();


            frameHeaders = new ShpOffsetUnit[imgCount];
            for (int i = 0; i < imgCount; i++)
            {
                frameHeaders[i].x = br.ReadUInt16();
                frameHeaders[i].y = br.ReadUInt16();
                frameHeaders[i].cx = br.ReadUInt16();
                frameHeaders[i].cy = br.ReadUInt16();

                frameHeaders[i].compression = br.ReadInt32();
                frameHeaders[i].radar_red = br.ReadByte();
                frameHeaders[i].radar_green = br.ReadByte();
                frameHeaders[i].radar_blue = br.ReadByte();
                frameHeaders[i].unknown = br.ReadByte();

                frameHeaders[i].zero = br.ReadInt32();
                frameHeaders[i].offset = br.ReadInt32();
            }

            frames = new byte[imgCount][][];
            for (int i = 0; i < imgCount; i++)
            {
                frames[i] = new byte[height][];
                for (int j = 0; j < height; j++)
                    frames[i][j] = new byte[width];
            }

            for (int i = 0; i < imgCount; i++)
            {
                int cx = frameHeaders[i].cx;
                int cy = frameHeaders[i].cy;

                byte[] img;
                if ((frameHeaders[i].compression & 2) != 0)
                {
                    img = new byte[cx * cy];
                    br.BaseStream.Position = frameHeaders[i].offset;
                    Decode3(br.BaseStream, img, cx, cy);
                }
                else
                {
                    br.BaseStream.Position = frameHeaders[i].offset;
                    img = br.ReadBytes(cx * cy);
                }

                int x = frameHeaders[i].x;
                int y = frameHeaders[i].y;
                for (int j = 0; j < cy; j++)
                    for (int k = 0; k < cx; k++)
                    {
                        frames[i][j + y][k + x] = img[j * cx + k];
                    }
            }
            br.Close();

        }

        private ShpAnim(ResourceLocation fl)
            : base(fl.Name)
        {
            Load(fl);
        }

        private ShpAnim(ResourceLocation fl, bool placeHolder)
        {
            Load(fl);
        }

        public static ShpAnim FromFile(string file)
        {
            return FromFile(new FileLocation(file));
        }
        public static ShpAnim FromFile(ResourceLocation fl)
        {
            return new ShpAnim(fl);
        }
        public static ShpAnim FromFileUnmanaged(ResourceLocation fl)
        {
            return new ShpAnim(fl, true);
        }

        int Decode3(Stream src, byte[] dest, int cx, int cy)
        {
            //int i = 0;
            int j = 0;
            for (int y = 0; y < cy; y++)
            {
                ushort count = (ushort)(((byte)src.ReadByte() | ((byte)src.ReadByte() << 8)) - 2);
                //i += 2;
                int x = 0;
                while (count-- > 0)
                {
                    int v = src.ReadByte();// src[i++];
                    if (v != 0)
                    {
                        x++;
                        dest[j++] = (byte)v;
                    }
                    else
                    {
                        count--;
                        v = src.ReadByte();// src[i++];
                        if (x + v > cx)
                            v = cx - x;
                        x += v;
                        while (v-- > 0)
                            dest[j++] = 0;
                    }
                }
            }
            return j;
        }


        public Palette Palette
        {
            get { return pal; }
            set { pal = value; }
        }


        public unsafe override ImageBase GetImage(int idx)
        {
            if (idx >= imgCount || idx < 0)
                throw new IndexOutOfRangeException();

            //Bitmap img = new Bitmap(width, height);

            //BitmapData sounds = img.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            ImageBase img = ImageManager.Instance.CreateInstance(width, height, ImagePixelFormat.A8R8G8B8);


            int* ptr = (int*)img.GetData();// sounds.Scan0.ToPointer();

            Color[] color = pal.Data;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    ptr[i * width + j] = color[frames[idx][i][j]].ToArgb();
                }
            //img.UnlockBits(sounds);
            return img;
        }

        protected override void dispose()
        {
            if (!disposed)
            {
                frames = null;
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(ToString());

        }

    }

    public sealed class ShpAnimFactory : IAnimFactory
    {

        #region IAnimFactory 成员

        public string[] Filters
        {
            get { return new string[] { ".shp", ".sha" }; }
        }

        #endregion

        #region IAbstractFactory<AnimBase> 成员

        public AnimBase CreateInstance(string file)
        {
            return ShpAnim.FromFile(file);
        }

        public AnimBase CreateInstance(ResourceLocation fl)
        {
            return ShpAnim.FromFile(fl);
        }

        public string Type
        {
            get { return "Shp Animation"; }
        }

        #endregion
    }
}
