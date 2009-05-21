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
using System.Runtime.InteropServices;
using System.Text;
using R3D.Base;
using R3D.IO;
using SlimDX;

namespace R3D.Media
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VoxelSectionHeader
    {
        public fixed byte name[16];
        public int number;
        public int unk1; //1
        public int unk2; //2
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct VoxelSectionTailer
    {
        public int spanStartOffset;
        public int spanEndOffset;
        public int spanDataOffset;
        public float det;

        public Vector4 line1;
        public Vector4 line2;
        public Vector4 line3;
        //public Vector4 line4;

        public Vector3 min;
        public Vector3 max;
        public byte xSize;
        public byte ySize;
        public byte zSize;
        public byte unk;
    }

    public unsafe  class Voxel
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct VoxelHeader
        {
            public fixed byte id[16];
            public int unk;//1
            public int sectCount;
            public int sectCount2;
            public int bodySize;
            public byte startPaletteRamp;
            public byte endPaletteRamp;
            public fixed byte palData[256 * 3];
        }

        VoxelSection[] sections;
        
        public Voxel(int sectCount)
        {
            sections = new VoxelSection[sectCount];
        }
        public Voxel(ResourceLocation rl)
        {
            VoxelHeader header;// = new VoxelHeader();

            ArchiveBinaryReader br = new ArchiveBinaryReader(rl);

            byte[] buffer = br.ReadBytes(16);
            fixed (byte* src = &buffer[0])
            {
                Memory.Copy(src, header.id, buffer.Length);
            }

            bool fc1 = header.id[0] == (byte)'V' && header.id[1] == (byte)'o' && header.id[2] == (byte)'x' && header.id[3] == (byte)'e';
            bool fc2 = header.id[4] == (byte)'l' && header.id[5] == (byte)' ' && header.id[6] == (byte)'A' && header.id[7] == (byte)'n';
            bool fc3 = header.id[8] == (byte)'i' && header.id[9] == (byte)'m' && header.id[10] == (byte)'a' && header.id[11] == (byte)'t';
            bool fc4 = header.id[12] == (byte)'i' && header.id[13] == (byte)'o' && header.id[14] == (byte)'n' && header.id[15] == 0;

            if (fc1 && fc2 && fc3 && fc4)
            {
                header.unk = br.ReadInt32();
                header.sectCount = br.ReadInt32();
                header.sectCount2 = br.ReadInt32();

                header.bodySize = br.ReadInt32();

                header.startPaletteRamp = br.ReadByte();
                header.endPaletteRamp = br.ReadByte();

                buffer = br.ReadBytes(256 * 3);
                fixed (byte* src = &buffer[0])
                {
                    Memory.Copy(src, header.palData, buffer.Length);
                }

                sections = new VoxelSection[header.sectCount];

                int headOffset = sizeof(VoxelHeader);
                int bodyOffset = headOffset + header.sectCount * sizeof(VoxelSectionHeader);
                int tailOffset = bodyOffset + header.bodySize;


                for (int i = 0; i < header.sectCount; i++)
                {
                    sections[i] = new VoxelSection(br, headOffset, tailOffset, bodyOffset);

                    headOffset += sizeof(VoxelSectionHeader);
                    tailOffset += sizeof(VoxelSectionTailer);
                }
            }
            else
            {
                br.Close();
                throw new DataFormatException(rl);
            }

            br.Close();
        }

        public VoxelSection[] Section
        {
            get { return sections; }
        }


    }
}
