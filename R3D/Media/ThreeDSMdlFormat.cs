// Title:	ThreeDSFile.cs
// Author: 	Scott Ellington <scott.ellington@gmail.com>
// Modified by Yuri
// 
// Copyright (C) 2006 Scott Ellington and authors
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ra2Reload.Graphics;
using Ra2Reload.IO;
using SlimDX;


namespace Ra2Reload.Media
{
    public class ThreeDSMdlFormat : ModelFormatBase
    {
        enum Groups
        {
            C_PRIMARY = 0x4D4D,
            C_OBJECTINFO = 0x3D3D,
            C_VERSION = 0x0002,
            C_EDITKEYFRAME = 0xB000,
            C_MATERIAL = 0xAFFF,
            C_MATNAME = 0xA000,
            C_MATAMBIENT = 0xA010,
            C_MATDIFFUSE = 0xA020,
            C_MATSPECULAR = 0xA030,
            C_MATSHININESS = 0xA040,
            C_MATMAP = 0xA200,
            C_MATMAPFILE = 0xA300,
            C_OBJECT = 0x4000,
            C_OBJECT_MESH = 0x4100,
            C_OBJECT_VERTICES = 0x4110,
            C_OBJECT_FACES = 0x4120,
            C_OBJECT_MATERIAL = 0x4130,
            C_OBJECT_UV = 0x4140
        }
        class ThreeDSChunk
        {
            public ushort ID;
            public uint Length;
            public int BytesRead;

            public ThreeDSChunk(BinaryReader reader)
            {
                // 2 byte ID
                ID = reader.ReadUInt16();
                //Console.WriteLine ("ID: {0}", ID.ToString("x"));

                // 4 byte length
                Length = reader.ReadUInt32();
                //Console.WriteLine ("Length: {0}", Length);

                // = 6
                BytesRead = 6;
            }
        }
        public override Ra2Reload.Graphics.MeshData Load(ResourceLocation rl)
        {
            throw new NotImplementedException();
        }

        void ProcessChunk(BinaryReader reader, ModelData model, ThreeDSChunk chunk)
        {
            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);

                switch ((Groups)child.ID)
                {
                    case Groups.C_VERSION:

                        int version = reader.ReadInt32();
                        child.BytesRead += 4;

                        Console.WriteLine("3DS File Version: {0}", version);
                        break;

                    case Groups.C_OBJECTINFO:

                        ThreeDSChunk obj_chunk = new ThreeDSChunk(reader);

                        // not sure whats up with this chunk
                        SkipChunk(reader, obj_chunk);
                        child.BytesRead += obj_chunk.BytesRead;

                        ProcessChunk(reader, model, child);

                        break;

                    case Groups.C_MATERIAL:

                        ProcessMaterialChunk(reader, child);
                        //SkipChunk ( child );
                        break;

                    case Groups.C_OBJECT:

                        //SkipChunk ( child );
                        string name = ProcessString(reader, child);

                        Ra2Reload.Graphics.MeshData e = ProcessObjectChunk(reader, child);
                        e.CalculateNormals();
                        model.Entities.Add(e);

                        break;

                    default:

                        SkipChunk(reader, child);
                        break;

                }

                chunk.BytesRead += child.BytesRead;
                //Console.WriteLine ( "ID: {0} Length: {1} Read: {2}", chunk.ID.ToString("x"), chunk.Length , chunk.BytesRead );
            }
        }

        void ProcessMaterialChunk(BinaryReader reader, ThreeDSChunk chunk)
        {
            string name = string.Empty;
            MaterialData m = new MaterialData(null);

            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);

                switch ((Groups)child.ID)
                {
                    case Groups.C_MATNAME:

                        name = ProcessString(reader, child);
                        break;

                    case Groups.C_MATAMBIENT:
                        float[] clrs = ProcessColorChunk(reader, child);
                        m.material.Ambient = new Color4(clrs[0], clrs[1], clrs[2], clrs[3]);
                        break;

                    case Groups.C_MATDIFFUSE:
                        clrs = ProcessColorChunk(reader, child);
                        m.material.Diffuse = new Color4(clrs[0], clrs[1], clrs[2], clrs[3]);
                        break;

                    case Groups.C_MATSPECULAR:
                        clrs = ProcessColorChunk(reader, child);

                        m.material.Specular = new Color4(clrs[0], clrs[1], clrs[2], clrs[3]);
                        break;

                    case Groups.C_MATSHININESS:

                        m.material.Power = ProcessPercentageChunk(reader, child);
                        //Console.WriteLine ( "SHININESS: {0}", m.Shininess );
                        break;

                    case Groups.C_MATMAP:

                        ProcessPercentageChunk(reader, child);

                        //SkipChunk ( child );
                        ProcessTexMapChunk(reader, child, m);

                        break;

                    default:

                        SkipChunk(reader, child);
                        break;
                }
                chunk.BytesRead += child.BytesRead;
            }
            materials.Add(name, m);
        }

        void ProcessTexMapChunk(BinaryReader reader, ThreeDSChunk chunk, MaterialData m)
        {
            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);
                switch ((Groups)child.ID)
                {
                    case Groups.C_MATMAPFILE:

                        string name = ProcessString(reader, child);
                        //Console.WriteLine("	Texture File: {0}", name);

                        //FileStream fStream;
                        //Bitmap bmp;
                        //try
                        //{
                        //    //fStream = new FileStream(base_dir + name, FileMode.Open, FileAccess.Read);
                        //    bmp = new Bitmap(base_dir + name);
                        //}
                        //catch (Exception e)
                        //{
                        //    // couldn't find the file
                        //    Console.WriteLine("	ERROR: could not load file '{0}'", base_dir + name);
                        //    break;
                        //}

                        //// Flip image (needed so texture are the correct way around!)
                        //bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

                        //System.Drawing.Imaging.BitmapData imgData = bmp.LockBits(new Rectangle(new Point(0, 0), bmp.Size),
                        //        System.Drawing.Imaging.ImageLockMode.ReadOnly,
                        //        System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                        ////								System.Drawing.Imaging.PixelFormat.Format24bppRgb ); 

                        //m.BindTexture(imgData.Width, imgData.Height, imgData.Scan0);

                        //bmp.UnlockBits(imgData);
                        //bmp.Dispose();

                        m.Texture = ImageManager.Instance.CreateImage(new FileLocation(Path.GetFullPath(name)));

                        break;

                    default:

                        SkipChunk(reader, child);
                        break;

                }
                chunk.BytesRead += child.BytesRead;
            }
        }

        float[] ProcessColorChunk(BinaryReader reader, ThreeDSChunk chunk)
        {
            ThreeDSChunk child = new ThreeDSChunk(reader);
            float[] c = new float[] { (float)reader.ReadByte() / 256, (float)reader.ReadByte() / 256, (float)reader.ReadByte() / 256 };
            //Console.WriteLine ( "R {0} G {1} B {2}", c.R, c.B, c.G );
            chunk.BytesRead += (int)child.Length;
            return c;
        }

        int ProcessPercentageChunk(BinaryReader reader, ThreeDSChunk chunk)
        {
            ThreeDSChunk child = new ThreeDSChunk(reader);
            int per = reader.ReadUInt16();
            child.BytesRead += 2;
            chunk.BytesRead += child.BytesRead;
            return per;
        }

        Ra2Reload.Graphics.MeshData ProcessObjectChunk(BinaryReader reader, ThreeDSChunk chunk)
        {
            return ProcessObjectChunk(reader, chunk, new MeshData(null));
        }

        Ra2Reload.Graphics.MeshData ProcessObjectChunk(BinaryReader reader, ThreeDSChunk chunk, Ra2Reload.Graphics.MeshData e)
        {
            while (chunk.BytesRead < chunk.Length)
            {
                ThreeDSChunk child = new ThreeDSChunk(reader);

                switch ((Groups)child.ID)
                {
                    case Groups.C_OBJECT_MESH:

                        ProcessObjectChunk(reader, child, e);
                        break;

                    case Groups.C_OBJECT_VERTICES:

                        e.Positions = ReadVertices(reader, child);
                        break;

                    case Groups.C_OBJECT_FACES:

                        e.Faces = ReadIndices(reader, child);

                        if (child.BytesRead < child.Length)
                            ProcessObjectChunk(reader, child, e);
                        break;

                    case Groups.C_OBJECT_MATERIAL:

                        string name2 = ProcessString(reader, child);                        

                        MaterialData mat;
                        if (materials.TryGetValue(name2, out mat))
                            e.Materials = mat;


                        SkipChunk(reader, child);
                        break;

                    case Groups.C_OBJECT_UV:

                        int cnt = reader.ReadUInt16();
                        child.BytesRead += 2;

                        Vector2[] tex = new Vector2[cnt];
                        for (int ii = 0; ii < cnt; ii++)
                            tex[ii] = new Vector2(reader.ReadSingle(), reader.ReadSingle());

                        e.Texture1 = tex;

                        child.BytesRead += (cnt * (4 * 2));

                        break;

                    default:

                        SkipChunk(reader, child);
                        break;

                }
                chunk.BytesRead += child.BytesRead;
                //Console.WriteLine ( "	ID: {0} Length: {1} Read: {2}", chunk.ID.ToString("x"), chunk.Length , chunk.BytesRead );
            }
            return e;
        }

        void SkipChunk(BinaryReader reader, ThreeDSChunk chunk)
        {
            int length = (int)chunk.Length - chunk.BytesRead;
            reader.ReadBytes(length);
            chunk.BytesRead += length;
        }

        string ProcessString(BinaryReader reader, ThreeDSChunk chunk)
        {
            StringBuilder sb = new StringBuilder();

            byte b = reader.ReadByte();
            int idx = 0;
            while (b != 0)
            {
                sb.Append((char)b);
                b = reader.ReadByte();
                idx++;
            }
            chunk.BytesRead += idx + 1;

            return sb.ToString();
        }

        Vector3[] ReadVertices(BinaryReader reader, ThreeDSChunk chunk)
        {
            ushort numVerts = reader.ReadUInt16();
            chunk.BytesRead += 2;
            // Console.WriteLine("	Vertices: {0}", numVerts);
            Vector3[] verts = new Vector3[numVerts];

            for (int ii = 0; ii < verts.Length; ii++)
            {
                float f1 = reader.ReadSingle();
                float f2 = reader.ReadSingle();
                float f3 = reader.ReadSingle();

                verts[ii] = new Vector3(f1, f3, -f2);
                //Console.WriteLine ( verts [ii] );
            }

            //Console.WriteLine ( "{0}   {1}", verts.Length * ( 3 * 4 ), chunk.Length - chunk.BytesRead );

            chunk.BytesRead += verts.Length * (3 * 4);
            //chunk.BytesRead = (int) chunk.Length;
            //SkipChunk ( chunk );

            return verts;
        }

        MeshFace[] ReadIndices(BinaryReader reader, ThreeDSChunk chunk)
        {
            ushort numIdcs = reader.ReadUInt16();
            chunk.BytesRead += 2;
            Console.WriteLine("	Indices: {0}", numIdcs);
            MeshFace[] idcs = new MeshFace[numIdcs];

            for (int ii = 0; ii < idcs.Length; ii++)
            {
                idcs[ii] = new MeshFace(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16());
                //Console.WriteLine ( idcs [ii] );

                // flags
                reader.ReadUInt16();
            }
            chunk.BytesRead += (2 * 4) * idcs.Length;
            //Console.WriteLine ( "b {0} l {1}", chunk.BytesRead, chunk.Length);

            //chunk.BytesRead = (int) chunk.Length;
            //SkipChunk ( chunk );

            return idcs;
        }

        public override string[] Filters
        {
            get { return new string[] { ".3DS" }; }
        }

        public override string Type
        {
            get { return "3DS"; }
        }
    }
}
