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

using R3D.IO;

namespace R3D.GraphicsEngine
{
    public unsafe class HeightMap
    {
        int width;
        int height;

        byte[][] map;

        public HeightMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            map = new byte[height][];
            for (int i = 0; i < height; i++)
                map[i] = new byte[width];            
        }

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public HeightMap(Stream stm)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(stm, Encoding.Default);

            width = br.ReadInt32();
            height = br.ReadInt32();

            map = new byte[height][];
            for (int i = 0; i < height; i++)
            {
                map[i] = br.ReadBytes(width);
            }

            br.Close();
        }

        public void* GetRow(int row)
        {
            fixed (byte* ptr = &map[row][0])
            {
                return ptr;// Helper.ldelema<byte>(map[row]);
            }
        }

        public byte this[int y, int x]
        {
            get { return map[y][x]; }
            set { map[y][x] = value; }
        }
        
        public void Save(string file)
        {
            Bitmap bmp = new Bitmap(width, height);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            uint* ptr = (uint*)data.Scan0.ToPointer();

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    ptr[i * width + j] = (uint)((map[i][j]) | (map[i][j] << 8) | (map[i][j] << 16));
                }

            bmp.UnlockBits(data);

            bmp.Save(file);

            bmp.Dispose();
        }
    }
}
