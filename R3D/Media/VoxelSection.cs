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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.Base;
using R3D.IO;
using SlimDX;

namespace R3D.Media
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VoxelData
    {
        public byte colour;
        public byte normal;
        public byte flag;
        public bool used;
    }
    public unsafe class VoxelSection
    {
        VoxelData[][][] data;
        VoxelSectionTailer tailer;
        VoxelSectionHeader header;

        void CreateData(int x, int y, int z)
        {
            data = new VoxelData[x][][];
            for (int i = 0; i < x; i++)
            {
                data[i] = new VoxelData[y][];
                for (int j = 0; j < y; j++)
                {
                    data[i][j] = new VoxelData[z];
                }
            }
        }

        public VoxelData[][][] Data
        {
            get { return data; }
        }

        
        public string Name
        {
            get
            {
                fixed (byte* ptr = header.name)
                {
                    return Utils.GetName(ptr);
                }
            }
        }

        public int XSize
        {
            get { return tailer.xSize; }
        }
        public int YSize
        {
            get { return tailer.ySize; }
        }
        public int ZSize
        {
            get { return tailer.zSize; }
        }

        public void GetTransform(out Matrix mat)
        {
            //float scaleX = (tailer.max.X - tailer.min.X) / (float)tailer.xSize;
            //float scaleY = (tailer.max.X - tailer.min.X) / (float)tailer.xSize;
            //float scaleZ = (tailer.max.X - tailer.min.X) / (float)tailer.xSize;

            //Vector3 offset;
            //Vector3.Add(ref tailer.min, ref tailer.max, out offset);
            //Vector3.Multiply(ref offset, 0.5f, out offset);

            //Matrix tn = Matrix.Translation((tailer.max.X + tailer.min.X) * 0.5f, (tailer.max.Y + tailer.min.Y) * 0.5f, (tailer.max.Z + tailer.min.Z) * 0.5f);

            //Matrix scale

            
            mat.M11 = tailer.line1.X;
            mat.M21 = tailer.line1.Y;
            mat.M31 = tailer.line1.Z;
            mat.M41 = tailer.line1.W;

            mat.M12 = tailer.line2.X;
            mat.M22 = tailer.line2.Y;
            mat.M32 = tailer.line2.Z;
            mat.M42 = tailer.line2.W;

            mat.M13 = tailer.line3.X;
            mat.M23 = tailer.line3.Y;
            mat.M33 = tailer.line3.Z;
            mat.M43 = tailer.line3.W;

            mat.M14 = 0;// (tailer.max.X + tailer.min.X) * 0.5f;
            mat.M24 = 0;// (tailer.max.Y + tailer.min.Y) * 0.5f;
            mat.M34 = 0;// (tailer.max.Z + tailer.min.Z) * 0.5f;
            mat.M44 = 1;

            //Matrix.Multiply(ref mat, ref tn, out mat);
        }

        public VoxelSection(ArchiveBinaryReader br, int headOffset, int tailOffset, int bodyOffset)
        {
            //VoxelSectionHeader header;
            br.BaseStream.Position = tailOffset;

            tailer.spanStartOffset = br.ReadInt32();
            tailer.spanEndOffset = br.ReadInt32();
            tailer.spanDataOffset = br.ReadInt32();
            tailer.det = br.ReadSingle();

            tailer.line1.X = br.ReadSingle();
            tailer.line1.Y = br.ReadSingle();
            tailer.line1.Z = br.ReadSingle();
            tailer.line1.W = br.ReadSingle();

            tailer.line2.X = br.ReadSingle();
            tailer.line2.Y = br.ReadSingle();
            tailer.line2.Z = br.ReadSingle();
            tailer.line2.W = br.ReadSingle();

            tailer.line3.X = br.ReadSingle();
            tailer.line3.Y = br.ReadSingle();
            tailer.line3.Z = br.ReadSingle();
            tailer.line3.W = br.ReadSingle();

            tailer.min.X = br.ReadSingle();
            tailer.min.Y = br.ReadSingle();
            tailer.min.Z = br.ReadSingle();

            tailer.max.X = br.ReadSingle();
            tailer.max.Y = br.ReadSingle();
            tailer.max.Z = br.ReadSingle();

            tailer.xSize = br.ReadByte();
            tailer.ySize = br.ReadByte();
            tailer.zSize = br.ReadByte();
            tailer.unk = br.ReadByte();


            br.BaseStream.Position = headOffset;
            byte[] buffer = br.ReadBytes(16);

            fixed (void* src = &buffer[0])
            {
                fixed (byte* dst = header.name)
                {
                    Memory.Copy(src, dst, buffer.Length);
                }
            }

            header.number = br.ReadInt32();
            header.unk1 = br.ReadInt32();
            header.unk2 = br.ReadInt32();


            CreateData(tailer.xSize, tailer.ySize, tailer.zSize);

            int spanCount = tailer.xSize * tailer.ySize;
            int i;

            int[] spanStart = new int[spanCount];
            int[] spanEnd = new int[spanCount];


            br.BaseStream.Position = bodyOffset + tailer.spanStartOffset;
            for (i = 0; i < spanCount; i++)
            {
                spanStart[i] = br.ReadInt32();
            }

            br.BaseStream.Position = bodyOffset + tailer.spanEndOffset;
            for (i = 0; i < spanCount; i++)
            {
                spanEnd[i] = br.ReadInt32();
            }

            i = spanCount - 1;
            while (spanEnd[i] == -1 && i > 0)
            {
                i--;
            }
            int spanDataLength = spanEnd[i];

            i = 0;
            while (spanStart[i] == -1 && (i + 1) < spanCount)
            {
                i++;
            }
            spanDataLength -= spanStart[i];
            spanDataLength++; // safety


            byte[] spanData = br.ReadBytes(spanDataLength);

            i = -1;
            VoxelData vxl;
            vxl.used = true;
            vxl.flag = 0;

            for (int y = 0; y < tailer.ySize; y++)
            {
                for (int x = 0; x < tailer.xSize; x++)
                {
                    i++;
                    if (spanStart[i] == -1 || spanEnd[i] == -1)
                    {
                        continue;
                    }

                    int z = 0;
                    int offset = spanStart[i];
                    while (z < tailer.zSize && z < spanEnd[i])
                    {
                        int k;

                        z += spanData[offset];
                        offset++;
                        int voxelsCount = spanData[offset];
                        offset++;

                        if (z > tailer.zSize)
                            break;
                        //if (z + num_voxels > tailer.zSize)
                        //{

                        //}

                        for (k = 1; k <= voxelsCount; k++)
                        {
                            vxl.colour = spanData[offset];
                            offset++;

                            vxl.normal = spanData[offset];
                            offset++;

                            data[x][y][z] = vxl;
                            z++;
                        }

                        k = spanData[offset];
                        offset++;
                        if (k != voxelsCount)
                        {
                            throw new DataFormatException("BadSpan_SecondVoxelCount");
                        }
                    }

                }
            }

            //this.header = header;
        }
        public VoxelSection(int x, int y, int z)
        {
            CreateData(x, y, z);
        }
    }
}
