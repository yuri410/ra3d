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
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Media
{
    public unsafe class RawImage : ImageBase 
    {
        byte[] data;
        public RawImage(int width, int height, ImagePixelFormat format)
        {
            this.width = width;
            this.height = height;
            this.format = format;

            switch (format)
            {
                case ImagePixelFormat.A8R8G8B8:
                case ImagePixelFormat.A8B8G8R8:
                case ImagePixelFormat.B8G8R8A8:
                case ImagePixelFormat.R8G8B8A8:
                case ImagePixelFormat.X8B8G8R8:
                case ImagePixelFormat.X8R8G8B8:
                    bytesPerPixel = 4;

                    break;
                case ImagePixelFormat.R8G8B8:
                case ImagePixelFormat.B8G8R8:
                    bytesPerPixel = 3;
                    break;
                //case ImagePixelFormat.B5G6R5:
                //case ImagePixelFormat.R5G6B5:
                //    bytesPerPixel = 2;
                //    break;
                case ImagePixelFormat.L8:
                case ImagePixelFormat.A8:
                    bytesPerPixel = 1;
                    break;
                default:
                    throw new NotSupportedException();
            }

            data = new byte[width * height * bytesPerPixel];
        }

        public override string Type
        {
            get { return  "raw"; }
        }

        public byte[] InternalData
        {
            get { return data; }
        }

        public override void GetData(out IntPtr ptr)
        {
            fixed (byte* p = &data[0])
            {
                ptr = new IntPtr(p);
            }
        }

        public override void* GetData()
        {
            fixed (byte* ptr = &data[0])
            {
                return ptr;
            }
        }

        public override void CopyDataARGB(void* dst)
        {
            int* d = (int*)dst;

            switch (format)
            {
                case ImagePixelFormat.A8R8G8B8:
                    Memory.Copy(GetData(), dst, width * height * bytesPerPixel);
                    break;
                case ImagePixelFormat.A8B8G8R8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (data[i * 4] << 24) | (data[i * 4 + 1]) | (data[i * 4 + 2] << 8) | (data[i * 4 + 3] << 16);
                    }
                    break;
                case ImagePixelFormat.B8G8R8A8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (data[i * 4]) | (data[i * 4 + 1] << 8) | (data[i * 4 + 2] << 16) | (data[i * 4 + 3] << 24);
                    }
                    break;
                case ImagePixelFormat.R8G8B8A8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (data[i * 4] << 16) | (data[i * 4 + 1] << 8) | (data[i * 4 + 2]) | (data[i * 4 + 3] << 24);
                    }
                    break;
                case ImagePixelFormat.X8B8G8R8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (0xff << 24) | (data[i * 4 + 1]) | (data[i * 4 + 2] << 8) | (data[i * 4 + 3] << 16);
                    }
                    break;
                case ImagePixelFormat.X8R8G8B8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (0xff << 24) | (data[i * 4 + 1] << 16) | (data[i * 4 + 2] << 8) | (data[i * 4 + 3]);
                    }
                    break;
                case ImagePixelFormat.R8G8B8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (0xff << 24) | (data[i * 3] << 16) | (data[i * 3 + 1] << 8) | (data[i * 3 + 2]);
                    }
                    break;
                case ImagePixelFormat.B8G8R8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (0xff << 24) | (data[i * 3]) | (data[i * 3 + 1] << 8) | (data[i * 3 + 2] << 16);
                    }
                    break;
                case ImagePixelFormat.L8 :
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (data[i] << 24) | (data[i] << 16) | (data[i] << 8) | (data[i]);
                    }
                    break;
                case ImagePixelFormat.A8:
                    for (int i = 0; i < width * height; i++)
                    {
                        d[i] = (data[i] << 24) | (data[i] << 16) | (data[i] << 8) | (data[i]);
                    }
                    break;
            }
        }

        public override Texture GetTexture(Device dev, Usage usage, Pool pool)
        {
            Texture tex = new Texture(dev, width, height, 1, usage, Format.A8R8G8B8, pool);
            DataRectangle rect = tex.LockRectangle(0, new Rectangle(0, 0, width, height), LockFlags.None);

            CopyDataARGB(rect.Data.DataPointer.ToPointer());
            //Il.ilBindImage(ilImageId);
            //Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_BYTE, rect.Data.DataPointer);

            tex.UnlockRectangle(0);

            return tex;
        }

        public override void MakeTransparent(Color color)
        {
            Color tclr = Color.Transparent;

            switch (format)
            {
                case ImagePixelFormat.A8R8G8B8:
                    int tc = tclr.ToArgb();
                    int c = color.ToArgb();

                    fixed (byte* ptr = &data[0])
                    {
                        int* d = (int*)ptr;
                        for (int i = 0; i < width * height; i++)
                        {
                            if (d[i] == c)
                                d[i] = tc;
                        }
                    }
                    break;

                case ImagePixelFormat.A8B8G8R8:
                    tc = (tclr.A << 24) | (tclr.B << 16) | (tclr.G << 8) | tclr.R;
                    c = (color.A << 24) | (color.B << 16) | (color.G << 8) | color.R;

                    fixed (byte* ptr = &data[0])
                    {
                        int* d = (int*)ptr;
                        for (int i = 0; i < width * height; i++)
                        {
                            if (d[i] == c)
                                d[i] = tc;
                        }
                    }
                    break;
                case ImagePixelFormat.B8G8R8A8:
                    tc = (tclr.B << 24) | (tclr.G << 16) | (tclr.R << 8) | tclr.A;
                    c = (color.B << 24) | (color.G << 16) | (color.R << 8) | color.A;

                    fixed (byte* ptr = &data[0])
                    {
                        int* d = (int*)ptr;
                        for (int i = 0; i < width * height; i++)
                        {
                            if (d[i] == c)
                                d[i] = tc;
                        }
                    }
                    break;
                case ImagePixelFormat.R8G8B8A8:
                    tc = (tclr.R << 24) | (tclr.G << 16) | (tclr.B << 8) | tclr.A;
                    c = (color.R << 24) | (color.G << 16) | (color.B << 8) | color.A;

                    fixed (byte* ptr = &data[0])
                    {
                        int* d = (int*)ptr;
                        for (int i = 0; i < width * height; i++)
                        {
                            if (d[i] == c)
                                d[i] = tc;
                        }
                    }
                    break;
                case ImagePixelFormat.X8B8G8R8:
                    tc = (tclr.B << 16) | (tclr.G << 8) | (tclr.R);
                    c = (color.B << 16) | (color.G << 8) | (color.R);

                    fixed (byte* ptr = &data[0])
                    {
                        int* d = (int*)ptr;
                        for (int i = 0; i < width * height; i++)
                        {
                            if ((d[i] & 0x00ffffff) == c)
                                d[i] = tc;
                        }
                    }
                    break;
                case ImagePixelFormat.X8R8G8B8:
                    tc = (tclr.R << 16) | (tclr.G << 8) | (tclr.B);
                    c = (color.R << 16) | (color.G << 8) | (color.B);

                    fixed (byte* ptr = &data[0])
                    {
                        int* d = (int*)ptr;
                        for (int i = 0; i < width * height; i++)
                        {
                            if ((d[i] & 0x00ffffff) == c)
                                d[i] = tc;
                        }
                    }
                    break;

                //case ImagePixelFormat .R8G8B8:
                //    int tc = (tclr.R << 16) | (tclr.G << 8) | (tclr.B);
                //    int c = (c.R << 16) | (c.G << 8) | (c.B);

                //    fixed (byte* ptr = &sounds[0])
                //    {
                //        //int* d = (int*)ptr;
                //        for (int i = 0; i < width * height; i++)
                //        {
                //            if (ptr[i * 3]  == c)
                //                ptr[i * 3] = tc;
                //        }
                //    }
                //    break;
                //case ImagePixelFormat.B8G8R8:

                //    break;
                default:
                    throw new NotSupportedException();
            }
        }

        protected override void dispose()
        {
            data = null;
        }

        public override string ToString()
        {
            return "{ RawImage: " + "Width=" + Width.ToString() + ", Height=" + Height.ToString() + " }";
        }
        public Bitmap ToBitmap()
        {
            Bitmap bmp = new Bitmap(Width, Height);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            void* dst = data.Scan0.ToPointer();
            CopyDataARGB(dst);

            bmp.UnlockBits(data);
            return bmp;
        }
        public static RawImage FromTexture(Texture tex)
        {
            SurfaceDescription desc = tex.GetLevelDescription(0);

            RawImage res = new RawImage(desc.Width, desc.Height, Format2ImagePixelFormat(desc.Format));

            DataRectangle rect = tex.LockRectangle(0, LockFlags.ReadOnly);
            Memory.Copy(rect.Data.DataPointer.ToPointer(), res.GetData(), res.ContentSize);

            tex.UnlockRectangle(0);

            return res;
        }
    }
}
