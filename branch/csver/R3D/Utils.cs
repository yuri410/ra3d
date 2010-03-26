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
using System.Drawing.Imaging;
using System.Text;
using R3D.Base;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D
{
    public class CaseInsensitiveStringComparer : IEqualityComparer<string>
    {
        static CaseInsensitiveStringComparer singleton;

        public static CaseInsensitiveStringComparer Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new CaseInsensitiveStringComparer();
                }
                return singleton;
            }
        }

        public static bool Compare(string a, string b)
        {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase) == 0;
        }

        private CaseInsensitiveStringComparer() { }

        #region IEqualityComparer<string> 成员

        public bool Equals(string x, string y)
        {
            return string.Compare(x, y, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public int GetHashCode(string obj)
        {
            return R3D.Base.Resource.GetHashCode(obj);
        }

        #endregion
    }
    public unsafe static class Utils
    {

        public const int D3DFVF_TEXTUREFORMAT2 = 0;         // Two floating point values
        public const int D3DFVF_TEXTUREFORMAT1 = 3;         // One floating point value
        public const int D3DFVF_TEXTUREFORMAT3 = 1;         // Three floating point values
        public const int D3DFVF_TEXTUREFORMAT4 = 2;         // Four floating point values


        public static int GetTexCoordSize3Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT3 << (coordIndex * 2 + 16);
        }

        public static int GetTexCoordSize2Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT2;
        }
        public static int GetTexCoordSize1Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT1 << (coordIndex * 2 + 16);
        }
        public static int GetTexCoordSize4Format(int coordIndex)
        {
            return D3DFVF_TEXTUREFORMAT4 << (coordIndex * 2 + 16);
        }


        static Utils()
        {
            EmptyStringArray = new string[0];
        }

        public static string[] EmptyStringArray
        {
            get;
            private set;
        }



        public static string GetName(byte* chars)
        {
            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
            {
                if (chars[i] != 0)
                {
                    sb.Append((char)chars[i]);
                }
                else
                    break;
            }
            return sb.ToString();
        }
        public static Texture Bitmap2Texture(Device dev, Bitmap bmp, Usage usage, Pool pool)
        {
            Texture res = new Texture(dev, bmp.Width, bmp.Height, 1, usage, Format.A8R8G8B8, pool);
            DataRectangle rect = res.LockRectangle(0, LockFlags.None);

            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            Memory.Copy(data.Scan0.ToPointer(), rect.Data.DataPointer.ToPointer(), 4 * bmp.Width * bmp.Height);

            bmp.UnlockBits(data);

            res.UnlockRectangle(0);
            return res;
        }

        public static Bitmap Texture2Bitmap(Texture tex)
        {
            RawImage rawImg = RawImage.FromTexture(tex);
            Bitmap res = rawImg.ToBitmap();
            rawImg.Dispose();
            return res;
        }
    }
}
