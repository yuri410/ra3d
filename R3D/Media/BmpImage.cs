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
using System.Drawing.Imaging;
using System.IO;

using R3D.IO;
using R3D.Base;

using DevIl;

namespace R3D.Media
{
    public sealed class BmpImage : DevIlImage
    {
        private BmpImage(ResourceLocation fl)
            : base(fl.Name)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(fl);

            byte[] buffer = br.ReadBytes(fl.Size);
            Il.ilBindImage(ilImageId);
            Il.ilLoadL(Il.IL_BMP, buffer, buffer.Length);

            br.Close();

            UpdateImageInfo(); 
            //resState = ResourceState.Loaded;
        }

        private BmpImage() { }


        public static BmpImage FromFileUnmanaged(ResourceLocation fl)
        {
            BmpImage res = new BmpImage();
            ArchiveBinaryReader br = new ArchiveBinaryReader(fl);

            byte[] buffer = br.ReadBytes(fl.Size);
            Il.ilBindImage(res.ilImageId);
            Il.ilLoadL(Il.IL_BMP, buffer, buffer.Length);

            br.Close();

            res.UpdateImageInfo();
            return res;
        }

        public static BmpImage FromFile(string file)
        {
            return FromFile(new FileLocation(file));
        }

        public static BmpImage FromFile(ResourceLocation fl)
        {
            return new BmpImage(fl);
        }

        public override string Type
        {
            get { return "Windows Bitmap"; }
        }

    }
    public sealed class BmpImageFactory : IImageFactory
    {
        #region IImageFactory 成员

        public ImageBase CreateInstance(string file)
        {
            return BmpImage.FromFile(file);
        }

        public ImageBase CreateInstance(ResourceLocation fl)
        {
            return BmpImage.FromFile(fl);
        }

        public ImageBase CreateInstanceUnmanaged(ResourceLocation fl)
        {
            return BmpImage.FromFileUnmanaged(fl);
        }

        public string Type
        {
            get { return "Windows Bitmap"; }
        }

        public string[] Filters
        {
            get { return new string[] { ".BMP" }; }
        }

        #endregion
    }
}
