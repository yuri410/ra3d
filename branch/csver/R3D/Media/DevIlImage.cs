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

using SlimDX;
using SlimDX.Direct3D9;
using DevIl;

namespace R3D.Media
{
    public unsafe abstract class DevIlImage : ImageBase, IDisposable
    {
        protected int ilImageId;
        protected bool disposed;

        protected DevIlImage(string name)
            : base(name)
        {
            ilImageId = Il.ilGenImage();
            //allowDynLoad = false;
        }
        protected  DevIlImage(int hash)
            :base(hash )
        {
            ilImageId = Il.ilGenImage();
            //allowDynLoad = false;

        }

        protected DevIlImage()
            : base()
        { ilImageId = Il.ilGenImage(); }

        //protected DevIlImage(int w, int h, ImagePixelFormat fmt)
        //    : base(w.GetHashCode() ^ h.GetHashCode() ^ fmt.GetHashCode() ^ new Random().Next(int.MaxValue))
        //{
        //    ilImageId = Il.ilGenImage();
        //    //allowDynLoad = false;
        //    Il.ilBindImage(ilImageId);
        //    Pair<int, int> p = Format2ILFormat(fmt);
        //    if (p.a != -1 && p.b != -1)
        //    {
        //        Il.ilSetInteger(Il.IL_IMAGE_FORMAT, p.a);
        //        Il.ilSetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL, p.b);
        //    }
        //    else
        //        throw new NotSupportedException();
        //    Il.ilSetInteger(Il.IL_IMAGE_WIDTH, w);
        //    Il.ilSetInteger(Il.IL_IMAGE_HEIGHT, h);
            
        //    int ilFmt = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);
        //    bytesPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
        //    width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
        //    height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
        //    format = ILFormat2Format(ilFmt, bytesPerPixel);          
        //}
        //protected int PixelFormat2ILFormat(PixelFormat format) 
        //{
        //    switch (format )
        //    {
        //        case PixelFormat.Format16bppRgb565:
        //            return Il.IL_RGB; //, Il.IL_UNSIGNED_SHORT);
        //        case PixelFormat.Format24bppRgb:
        //            return Il.IL_RGB;
        //        case PixelFormat.Format32bppArgb:
        //            return Il.IL_RGBA; //, Il.IL_UNSIGNED_INT);
        //    }
        //    return Il.IL_FORMAT_NOT_SUPPORTED;
        //}

        /// <summary>
        ///    Converts a DevIL format enum to a Format enum.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="bytesPerPixel"></param>
        /// <returns></returns>
        protected ImagePixelFormat ILFormat2Format(int format, int bytesPerPixel)
        {
            switch (bytesPerPixel)
            {
                case 1:
                    return ImagePixelFormat.L8;

                case 2:
                    switch (format)
                    {
                        case Il.IL_BGR:
                            return ImagePixelFormat.B5G6R5; // Format.B5G6R5;
                        case Il.IL_RGB:
                            return ImagePixelFormat.R5G6B5; // Format.R5G6B5;
                        case Il.IL_BGRA:
                            return ImagePixelFormat.B4G4R4A4;
                        case Il.IL_RGBA:
                            return ImagePixelFormat.A4R4G4B4;
                    }
                    break;

                case 3:
                    switch (format)
                    {
                        case Il.IL_BGR:
                            return ImagePixelFormat.B8G8R8;
                        case Il.IL_RGB:
                            return ImagePixelFormat.R8G8B8;// Format.R8G8B8;
                    }
                    break;

                case 4:
                    switch (format)
                    {
                        case Il.IL_BGRA:
                            return ImagePixelFormat.A8B8G8R8; // Format.B8G8R8A8;
                        case Il.IL_RGBA:
                            return ImagePixelFormat.A8R8G8B8;//Format.A8R8G8B8;
                        case Il.IL_DXT1:
                            return ImagePixelFormat.DXT1;
                        case Il.IL_DXT2:
                            return ImagePixelFormat.DXT2;
                        case Il.IL_DXT3:
                            return ImagePixelFormat.DXT3;
                        case Il.IL_DXT4:
                            return ImagePixelFormat.DXT4;
                        case Il.IL_DXT5:
                            return ImagePixelFormat.DXT5;
                    }
                    break;
            }

            return ImagePixelFormat.Unknown;
        }
        protected Pair <int,int> Format2ILFormat(ImagePixelFormat format)
        {
            switch (format)
            {
                case ImagePixelFormat.L8 :
                    return new Pair<int,int> (Il.IL_LUMINANCE, 1);
                case ImagePixelFormat.B5G6R5 :
                    return new Pair<int, int>(Il.IL_BGR, 2);
                case ImagePixelFormat.R5G6B5 :
                    return new Pair<int, int>(Il.IL_RGB, 2);
                case ImagePixelFormat.B4G4R4A4:
                    return new Pair<int, int>(Il.IL_BGRA, 2);
                case ImagePixelFormat.A4R4G4B4 :
                    return new Pair<int, int>(Il.IL_RGBA, 2);
                case ImagePixelFormat.B8G8R8 :
                    return new Pair<int, int>(Il.IL_BGR, 3);
                case ImagePixelFormat.R8G8B8 :
                    return new Pair<int, int>(Il.IL_RGBA, 3);
                case ImagePixelFormat.A8B8G8R8 :
                    return new Pair<int, int>(Il.IL_BGRA, 4);
                case ImagePixelFormat.A8R8G8B8 :
                    return new Pair<int, int>(Il.IL_RGBA, 4);
                case ImagePixelFormat.DXT1:
                    return new Pair<int, int>(Il.IL_DXT1, 4);
                case ImagePixelFormat.DXT2:
                    return new Pair<int, int>(Il.IL_DXT2, 4);
                case ImagePixelFormat.DXT3:
                    return new Pair<int, int>(Il.IL_DXT3, 4);
                case ImagePixelFormat.DXT4:
                    return new Pair<int, int>(Il.IL_DXT4, 4);
                case ImagePixelFormat.DXT5:
                    return new Pair<int, int>(Il.IL_DXT5, 4);
            }
            return new Pair<int, int>(-1, -1);
        }

        //protected PixelFormat Format2PixelFormat(ImagePixelFormat format)
        //{
        //    switch (format)
        //    {
        //        case ImagePixelFormat.R5G6B5:
        //            return PixelFormat.Format16bppRgb565;
        //        case ImagePixelFormat.R8G8B8:
        //            return PixelFormat.Format24bppRgb;
        //        case ImagePixelFormat.A8R8G8B8:// Format.A8B8G8R8:
        //            return PixelFormat.Format32bppArgb;
        //    }
        //    return PixelFormat.Undefined;
        //}

        public override void MakeTransparent(Color color)
        {
            int[] buffer = new int[width * height];
            fixed (int* dst = &buffer[0])
            {
                Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_BYTE, new IntPtr(dst));
                
                int tc = color.ToArgb();
                int trc = Color.Transparent.ToArgb();

                for (int i = 0; i < width * height; i++)
                {
                    if (buffer[i] == tc)
                    {
                        buffer[i] = trc;
                    }
                }

                Il.ilSetPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_BYTE, new IntPtr(dst));
            }
        }

        protected void UpdateImageInfo()
        {
            int ilFmt = Il.ilGetInteger(Il.IL_IMAGE_FORMAT);

            width = Il.ilGetInteger(Il.IL_IMAGE_WIDTH);
            height = Il.ilGetInteger(Il.IL_IMAGE_HEIGHT);
            bytesPerPixel = Il.ilGetInteger(Il.IL_IMAGE_BYTES_PER_PIXEL);
            format = ILFormat2Format(ilFmt, bytesPerPixel);

            //if (ilFmt == Il.IL_RGB || ilFmt == Il.IL_RGBA)
            //{
            //    Ilu.iluSwapColours();
            //}
        }

        //public override Bitmap GetBitmap(PixelFormat fmt)
        //{
        //    int ilfmt = PixelFormat2ILFormat(fmt);

        //    if (ilfmt == Il.IL_FORMAT_NOT_SUPPORTED)
        //        throw new NotSupportedException();

        //    Bitmap bmp = new Bitmap(width, height);
        //    BitmapData sounds = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, fmt);

        //    Il.ilBindImage(ilImageId);
        //    Il.ilCopyPixels(0, 0, 0, width, height, 1, ilfmt, Il.IL_BYTE, sounds.Scan0);

        //    bmp.UnlockBits(sounds);
        //    return bmp;
        //}
        //public override Bitmap GetBitmap()
        //{
        //    PixelFormat pfmt = Format2PixelFormat(format);

        //    if (pfmt == PixelFormat.Undefined)
        //        throw new NotSupportedException(format.ToString());
        //    Bitmap bmp = new Bitmap(width, height);

        //    BitmapData sounds = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, pfmt);

        //    //Il.ilBindImage(ilImageId);
        //    void* dst = sounds.Scan0.ToPointer();
        //    void* src = GetData();

        //    Helper.MemCopy(dst, src, ContentSize);

        //    bmp.UnlockBits(sounds);

        //    return bmp;
        //}

        public override Texture GetTexture(Device dev, Usage usage, Pool pool)
        {
            Texture tex = new Texture(dev, width, height, 1, usage, Format.A8R8G8B8, pool);
            DataRectangle rect = tex.LockRectangle(0, new Rectangle(0, 0, width, height), LockFlags.None);

            Il.ilBindImage(ilImageId);
            Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_BYTE, rect.Data.DataPointer);

            tex.UnlockRectangle(0);

            return tex;
        }
        public override void CopyDataARGB(void * dst)
        {
            Il.ilBindImage(ilImageId);
            Il.ilCopyPixels(0, 0, 0, width, height, 1, Il.IL_BGRA, Il.IL_BYTE, new IntPtr(dst));
        }

        public override void GetData(out IntPtr ptr)
        {
            Il.ilBindImage(ilImageId);
            ptr = Il.ilGetData();
        }
        public override void* GetData()
        {
            Il.ilBindImage(ilImageId);
            return Il.ilGetData().ToPointer();
        }

        protected override void dispose()
        {
            if (!disposed)
            {
                Il.ilDeleteImage(ilImageId);
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        ~DevIlImage()
        {
            if (!disposed)
                Dispose();
        }

    }
}
