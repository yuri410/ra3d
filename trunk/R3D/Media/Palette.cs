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
using System.IO;
using R3D.IO;
using R3D.Base;

namespace R3D.Media
{
    public class Palette
    {
        const int ColorCount = 256;

        Color[] colors;
        uint[] argbClrs;

        public Palette()
        {
            colors = new Color[ColorCount];
        }
        public Palette(Color[] clrs)            
        {
            if (clrs.Length != ColorCount)
                throw new ArgumentException();
            colors = clrs;
        }
        public Palette(int[] clrs)
        {
            if (clrs.Length != ColorCount)
                throw new ArgumentException();
            for (int i = 0; i < ColorCount; i++)
                colors[i] = Color.FromArgb(clrs[i]);
        }

        //public Palette(FileLocation file)
        //    : this(file.GetStream, !file.IsInArchive)
        //{ }

        //private Palette(Stream src, bool autoCloseStream)
        //{
        //    isInMix = !autoCloseStream;
        //    Read(src);
        //}

        //public Palette(string file)
        //    : this(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), true)
        //{ }

        private Palette(ResourceLocation fl)
        {
            colors = new Color[256];

            ArchiveBinaryReader br = new ArchiveBinaryReader(fl);
            for (int i = 0; i < 256; i++)
            {
                byte[] clr = br.ReadBytes(3);
                clr[0] = (byte)((clr[0] & 63) * 255 / 63);
                clr[1] = (byte)((clr[1] & 63) * 255 / 63);
                clr[2] = (byte)((clr[2] & 63) * 255 / 63);

                colors[i] = Color.FromArgb((0xFF << 24) | (clr[0] << 16) | (clr[1] << 8) | (clr[2]));
            }
            br.Close();

            argbClrs = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                unchecked
                {
                    argbClrs[i] = (uint)(colors[i].A << 24) | (uint)(colors[i].R << 16) | (uint)(colors[i].G << 8) | (uint)colors[i].B;
                }
            }
        }

        public static Palette FromFile(string file)
        {
            return FromFile(new FileLocation(file));
        }
        public static Palette FromFile(ResourceLocation fl)
        {
            return new Palette(fl);
        }

        public Color[] Data
        {
            get { return colors; }
        }
        public uint[] ARGBData
        {
            get { return argbClrs; }
        }

    }
}
