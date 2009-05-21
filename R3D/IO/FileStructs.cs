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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace R3D.IO
{
    /// <summary>
    /// 类型标志
    /// </summary>
    public enum FileID : int
    {
        AudioIdx = ((byte)'A' << 24) | ((byte)'B' << 16) | ((byte)'A' << 8) | (byte)'G',
        Csf = ((byte)'C' << 24) | ((byte)'S' << 16) | ((byte)'F' << 8) | (byte)' ',
        Lmd = ((byte)'M' << 24) | ((byte)'X' << 16) | ((byte)'D' << 8) | ((byte)'B'),
        Tmp3D = ((byte)'T' << 24) | ((byte)'M' << 16) | ((byte)'3' << 8) | ((byte)'D'),
        Mesh = ((byte)'M' << 24) | ((byte)'E' << 16) | ((byte)'S' << 8) | ((byte)'H'),
        Material = ((byte)'M' << 24) | ((byte)'A' << 16) | ((byte)'T' << 8) | ((byte)'E'),
        Model = ((byte)'M' << 24) | ((byte)'D' << 16) | ((byte)'L' << 8) | ((byte)' ')
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ArchiveFileEntry
    {
        public uint id;
        public int offset;
        public int size;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(8);
            sb.Append("{ ID= ");
            sb.Append(Convert.ToString(id, 16));
            sb.Append(", Offset= ");
            sb.Append(offset.ToString());
            sb.Append('}');
            return sb.ToString();
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct PcxHeader
    {
        public sbyte manufacturer;
        public sbyte version;
        public sbyte encoding;
        public sbyte cbits_pixel;
        public Int16 xmin;
        public Int16 ymin;
        public Int16 xmax;
        public Int16 ymax;
        public Int16 cx_inch;
        public Int16 cy_inch;
        public fixed sbyte colorpap[16 * 3];
        public sbyte reserved;
        public sbyte c_planes;
        public Int16 cb_line;
        public fixed sbyte filler[60];
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct XccLmdHeader
    {
        public readonly static char[] xcc_id;

        static XccLmdHeader()
        {
            string s = "XCC by Olaf van der Spek\x1a\x04\x17\x27\x10\x19\x80";
            xcc_id = new char[32];
            for (int i = 0; i < s.Length; i++)
                xcc_id[i] = s[i];
        }

        public fixed char id[32];
        public int size;
        public int type;
        public int version;
        public int game; //2
    }



}