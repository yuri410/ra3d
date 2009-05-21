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
using R3D.Base;
using R3D.IO;
using SlimDX;

namespace R3D.Media
{
    public unsafe class HVA
    {
        struct Header
        {
            public fixed byte path[16];
            public int frameCount;
            public int sectionCount;
        }

        Matrix[][] trans;
        Header header;
        string[] sectionNames;


        public HVA(ResourceLocation rl)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(rl);

            byte[] buffer = br.ReadBytes(16);
            fixed (void* src = &buffer[0], dst = header.path)
            {
                Memory.Copy(src, dst, 16);
            }
            header.frameCount = br.ReadInt32();
            header.sectionCount = br.ReadInt32();

            //trans = new Matrix[header.sectionCount][];
            sectionNames = new string[header.sectionCount];

            for (int i = 0; i < header.sectionCount; i++)
            {
                buffer = br.ReadBytes(16);

                fixed (byte* src = &buffer[0])
                {
                    sectionNames[i] = Utils.GetName(src);
                }
            }


            trans = new Matrix[header.frameCount][];
            for (int i = 0; i < header.frameCount; i++)
            {
                trans[i] = new Matrix[header.sectionCount];
                for (int j = 0; j < header.sectionCount; j++)
                {
                    Matrix mat;
                    mat.M11 = br.ReadSingle();
                    mat.M21 = br.ReadSingle();
                    mat.M31 = br.ReadSingle();
                    mat.M41 = br.ReadSingle();

                    mat.M12 = br.ReadSingle();
                    mat.M22 = br.ReadSingle();
                    mat.M32 = br.ReadSingle();
                    mat.M42 = br.ReadSingle();

                    mat.M13 = br.ReadSingle();
                    mat.M23 = br.ReadSingle();
                    mat.M33 = br.ReadSingle();
                    mat.M43 = br.ReadSingle();

                    mat.M14 = 0;
                    mat.M24 = 0;
                    mat.M34 = 0;
                    mat.M44 = 1;
                    trans[i][j] = mat;
                }
            }

            Matrix[][] trans2 = new Matrix[header.sectionCount][];
            for (int i = 0; i < header.sectionCount; i++)
            {
                trans2[i] = new Matrix[header.frameCount];
                for (int j = 0; j < header.frameCount; j++)
                {
                    trans2[i][j] = trans[j][i];
                }
            }

            trans = trans2;

            //for (int i = 0; i < header.sectionCount; i++)
            //{
            //    trans[i] = new Matrix[header.frameCount];
            //    for (int j = 0; j < header.frameCount; j++)
            //    {
            //        Matrix mat;
            //        mat.M11 = br.ReadSingle();
            //        mat.M21 = br.ReadSingle();
            //        mat.M31 = br.ReadSingle();
            //        mat.M41 = br.ReadSingle();

            //        mat.M12 = br.ReadSingle();
            //        mat.M22 = br.ReadSingle();
            //        mat.M32 = br.ReadSingle();
            //        mat.M42 = br.ReadSingle();

            //        mat.M13 = br.ReadSingle();
            //        mat.M23 = br.ReadSingle();
            //        mat.M33 = br.ReadSingle();
            //        mat.M43 = br.ReadSingle();

            //        mat.M14 = 0;
            //        mat.M24 = 0;
            //        mat.M34 = 0;
            //        mat.M44 = 1;
            //        trans[i][j] = mat;
            //    }
            //}
            br.Close();
        }

        public string GetName(int i)
        {
            return sectionNames[i];
        }

        public int SectionCount
        {
            get { return header.sectionCount; }
        }
        public int FrameCount
        {
            get { return header.frameCount; }
        }
        public Matrix[] GetTransform(int secIdx)
        {
            return trans[secIdx];
        }
    }
}
