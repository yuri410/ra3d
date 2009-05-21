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
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.Base;
using R3D.Design;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine
{
    [TypeConverter(typeof(MeshFaceConverter))]
    public struct MeshFace
    {
        int a;
        int b;
        int c;

        int materialIdx;

        public MeshFace(int A, int B, int C)
        {
            a = A;
            b = B;
            c = C;
            materialIdx = -1;
        }
        public MeshFace(int A, int B, int C, int matId)
        {
            a = A;
            b = B;
            c = C;
            materialIdx = matId;
        }

        public int IndexA
        {
            get { return a; }
            set { a = value; }
        }
        public int IndexB
        {
            get { return b; }
            set { b = value; }
        }
        public int IndexC
        {
            get { return c; }
            set { c = value; }
        }

        public int MaterialIndex
        {
            get { return materialIdx; }
            set { materialIdx = value; }
        }
    }

    [Flags()]
    public enum VertexFlags : int
    {
        None = 0,
        MergeVertex = 1
    }

    #region Vertex Formats
    //[StructLayout(LayoutKind.Explicit)]
    public struct VertexPNT1
    {
        //[FieldOffset(0)]
        public Vector3 pos;
        //[FieldOffset(12)]
        public Vector3 n;

        //[FieldOffset(24)]
        public float u;
        //[FieldOffset(28)]
        public float v;
        //[FieldOffset(24)]
        //public Vector2 tex1;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1; }
        }

        public override int GetHashCode()
        {
            return pos.GetHashCode() ^ n.GetHashCode() ^ u.GetHashCode() ^ v.GetHashCode();
        }

        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString() + "uv: " + u.ToString() + "," + v.ToString();
        }
    }
    public struct VertexPNT2
    {
        public Vector3 pos;
        public Vector3 n;
        public float u1, v1;
        public float u2, v2;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture2; }
        }


        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString();
        }
    }
    public struct VertexPNT3
    {
        public Vector3 pos;
        public Vector3 n;
        public float u1, v1;
        public float u2, v2;
        public float u3, v3;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture3; }
        }

        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString()
                + "uv3: " + u3.ToString() + "," + v3.ToString();
        }
    }
    public struct VertexPNT4
    {
        public Vector3 pos;
        public Vector3 n;
        public float u1, v1;
        public float u2, v2;
        public float u3, v3;
        public float u4, v4;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture4; }
        }

        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "N: " + n.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString()
                + "uv3: " + u3.ToString() + "," + v3.ToString()
                + "uv4: " + u4.ToString() + "," + v4.ToString();
        }
    }

    public struct VertexPT1
    {
        public Vector3 pos;
        public float u1, v1;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Texture1; }
        }

        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "uv: " + u1.ToString() + "," + v1.ToString();
        }
    }
    public struct VertexPT2
    {
        public Vector3 pos;
        public float u1, v1;
        public float u2, v2;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Texture2; }
        }

        public override string ToString()
        {
            return "Pos: " + pos.ToString()
                + "uv1: " + u1.ToString() + "," + v1.ToString()
                + "uv2: " + u2.ToString() + "," + v2.ToString();
        }
    }
    public struct VertexPT3
    {
        public Vector3 pos;
        public float u1, v1;
        public float u2, v2;
        public float u3, v3;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Texture3; }
        }

        public override string ToString()
        {
            return "Pos: " + pos.ToString()
                 + "uv1: " + u1.ToString() + "," + v1.ToString()
                 + "uv2: " + u2.ToString() + "," + v2.ToString()
                 + "uv3: " + u3.ToString() + "," + v3.ToString();
        }
    }
    public struct VertexPT4
    {
        public Vector3 pos;
        public float u1;
        public float v1;
        public float u2;
        public float v2;
        public float u3;
        public float v3;

        public float u4;
        public float v4;
        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Texture4; }
        }

        public override string ToString()
        {
            return "Pos: " + pos.ToString()
               + "uv1: " + u1.ToString() + "," + v1.ToString()
               + "uv2: " + u2.ToString() + "," + v2.ToString()
               + "uv3: " + u3.ToString() + "," + v3.ToString()
               + "uv4: " + u4.ToString() + "," + v4.ToString();
        }
    }

    public struct VertexPC
    {
        public Vector3 pos;
        public int diffuse;

        public static VertexFormat Format
        {
            get { return VertexFormat.Position | VertexFormat.Diffuse; }
        }
        public override string ToString()
        {
            return "Pos: " + pos.ToString() + "clr: " + diffuse.ToString();
        }
    }
    public struct VertexP
    {
        public Vector3 pos;

        public VertexFormat Format
        {
            get { return VertexFormat.Position; }
        }

        public override string ToString()
        {
            return pos.ToString();
        }
    }
    #endregion


    public abstract class GameMeshDataBase<MType> where MType : class//MeshMaterialBase<Texture>
    {
        protected static readonly string ChunkMaterialCount = "MaterialCount";
        protected static readonly string ChunkMaterials = "Materials";
        protected static readonly string ChunkFaceCount = "FaceCount";
        protected static readonly string ChunkFaces = "Faces";
        protected static readonly string ChunkVertexFormat = "VertexFormat";
        protected static readonly string ChunkVertexCount = "VertexCount";

        protected static readonly string ChunkPositions = "Positions";
        protected static readonly string ChunkNormals = "Normals";
        protected static readonly string ChunkTextureCoords = "TextureCoords";
        protected static readonly string ChunkName = "Name";

        protected Device device;

        protected MType[] materials;

        /// <summary>
        /// elements分开储存，一一对应，以适应不同的顶点格式
        /// </summary>
        protected Vector3[] positions;

        protected Vector3[] normals;

        protected Vector2[] tex1;
        protected Vector2[] tex2;
        protected Vector2[] tex3;
        protected Vector2[] tex4;

        protected MeshFace[] faces;

        VertexFormat format;

        string name;


        /// <summary>
        /// Build mesh sounds in RAM from a Mesh object.
        /// </summary>
        /// <param name="mesh">The Mesh object</param>
        /// <param name="sounds">The mesh sounds to write to</param>
        /// <param name="materials"></param>
        public unsafe static void BuildFromMesh(Mesh mesh, GameMeshDataBase<MType> data, MType[] materials)
        {
            data.device = mesh.Device;

            int faceCount = mesh.FaceCount;
            int vertexCount = mesh.VertexCount;

            data.Faces = new MeshFace[faceCount];
            data.Format = mesh.VertexFormat;

            data.materials = materials;

            Vector3[] positions = new Vector3[vertexCount];

            if (data.format == VertexPNT1.Format)
            {
                Vector3[] normals = new Vector3[vertexCount];
                Vector2[] tex1 = new Vector2[vertexCount];

                VertexPNT1* vb = (VertexPNT1*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT1* vtx = vb + i;

                    positions[i] = vtx->pos;
                    normals[i] = vtx->n;
                    normals[i].Normalize();
                    tex1[i] = new Vector2(vtx->u, vtx->v);
                }

                mesh.UnlockVertexBuffer();

                data.tex1 = tex1;
                data.normals = normals;
            }
            else if (data.format == VertexPNT2.Format)
            {
                Vector3[] normals = new Vector3[vertexCount];
                Vector2[] tex1 = new Vector2[vertexCount];
                Vector2[] tex2 = new Vector2[vertexCount];

                VertexPNT2* vb = (VertexPNT2*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT2* vtx = vb + i;

                    positions[i] = vtx->pos;
                    normals[i] = vtx->n;
                    normals[i].Normalize(); 
                    tex1[i] = new Vector2(vtx->u1, vtx->v1);
                    tex2[i] = new Vector2(vtx->u2, vtx->v2);
                }

                mesh.UnlockVertexBuffer();
                data.tex1 = tex1;
                data.tex2 = tex2;
                data.normals = normals;
            }
            else if (data.format == VertexPNT3.Format)
            {
                Vector3[] normals = new Vector3[vertexCount];
                Vector2[] tex1 = new Vector2[vertexCount];
                Vector2[] tex2 = new Vector2[vertexCount];
                Vector2[] tex3 = new Vector2[vertexCount];

                VertexPNT3* vb = (VertexPNT3*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT3* vtx = vb + i;

                    positions[i] = vtx->pos;
                    normals[i] = vtx->n;
                    normals[i].Normalize();
                    tex1[i] = new Vector2(vtx->u1, vtx->v1);
                    tex2[i] = new Vector2(vtx->u2, vtx->v2);
                    tex3[i] = new Vector2(vtx->u3, vtx->v3);
                }

                mesh.UnlockVertexBuffer();
                data.tex1 = tex1;
                data.tex2 = tex2;
                data.tex3 = tex3;
                data.normals = normals;
            }
            else if (data.format == VertexPNT4.Format)
            {
                Vector3[] normals = new Vector3[vertexCount];
                Vector2[] tex1 = new Vector2[vertexCount];
                Vector2[] tex2 = new Vector2[vertexCount];
                Vector2[] tex3 = new Vector2[vertexCount];
                Vector2[] tex4 = new Vector2[vertexCount];

                VertexPNT4* vb = (VertexPNT4*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT4* vtx = vb + i;

                    positions[i] = vtx->pos;
                    normals[i] = vtx->n;
                    normals[i].Normalize();
                    tex1[i] = new Vector2(vtx->u1, vtx->v1);
                    tex2[i] = new Vector2(vtx->u2, vtx->v2);
                    tex3[i] = new Vector2(vtx->u3, vtx->v3);
                    tex4[i] = new Vector2(vtx->u4, vtx->v4);
                }

                mesh.UnlockVertexBuffer();
                data.tex1 = tex1;
                data.tex2 = tex2;
                data.tex3 = tex3;
                data.tex4 = tex4;
                data.normals = normals;
            }
            else if (data.format == VertexPC.Format)
            {
                throw new NotSupportedException(data.format.ToString());
            }
            else if (data.format == VertexPT1.Format)
            {
                Vector2[] tex1 = new Vector2[vertexCount];

                VertexPT1* vb = (VertexPT1*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT1* vtx = vb + i;

                    positions[i] = vtx->pos;
                    tex1[i] = new Vector2(vtx->u1, vtx->v1);
                }

                mesh.UnlockVertexBuffer();
                data.tex1 = tex1;
            }
            else if (data.format == VertexPT2.Format)
            {
                Vector2[] tex1 = new Vector2[vertexCount];
                Vector2[] tex2 = new Vector2[vertexCount];

                VertexPT2* vb = (VertexPT2*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT2* vtx = vb + i;

                    positions[i] = vtx->pos;
                    tex1[i] = new Vector2(vtx->u1, vtx->v1);
                    tex2[i] = new Vector2(vtx->u2, vtx->v2);
                }

                mesh.UnlockVertexBuffer();
                data.tex1 = tex1;
                data.tex2 = tex2;
            }
            else if (data.format == VertexPT3.Format)
            {
                Vector2[] tex1 = new Vector2[vertexCount];
                Vector2[] tex2 = new Vector2[vertexCount];
                Vector2[] tex3 = new Vector2[vertexCount];

                VertexPT3* vb = (VertexPT3*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT3* vtx = vb + i;

                    positions[i] = vtx->pos;
                    tex1[i] = new Vector2(vtx->u1, vtx->v1);
                    tex2[i] = new Vector2(vtx->u2, vtx->v2);
                    tex3[i] = new Vector2(vtx->u3, vtx->v3);
                }

                mesh.UnlockVertexBuffer();
                data.tex1 = tex1;
                data.tex2 = tex2;
                data.tex3 = tex3;
            }
            else if (data.format == VertexPT4.Format)
            {
                Vector2[] tex1 = new Vector2[vertexCount];
                Vector2[] tex2 = new Vector2[vertexCount];
                Vector2[] tex3 = new Vector2[vertexCount];
                Vector2[] tex4 = new Vector2[vertexCount];

                VertexPNT4* vb = (VertexPNT4*)mesh.LockVertexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT4* vtx = vb + i;

                    positions[i] = vtx->pos;
                    tex1[i] = new Vector2(vtx->u1, vtx->v1);
                    tex2[i] = new Vector2(vtx->u2, vtx->v2);
                    tex3[i] = new Vector2(vtx->u3, vtx->v3);
                    tex4[i] = new Vector2(vtx->u4, vtx->v4);
                }

                mesh.UnlockVertexBuffer();
                data.tex1 = tex1;
                data.tex2 = tex2;
                data.tex3 = tex3;
                data.tex4 = tex4;
            }

            data.positions = positions;


            if (string.IsNullOrEmpty(data.name))
                data.name = string.Empty;


            uint* ab = (uint*)mesh.LockAttributeBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();


            if ((mesh.CreationOptions & MeshFlags.Use32Bit) == MeshFlags.Use32Bit)
            {
                uint* ib = (uint*)mesh.LockIndexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < faceCount; i++)
                {
                    int idxId = i * 3;

                    data.faces[i] = new MeshFace((int)ib[idxId], (int)ib[idxId + 1], (int)ib[idxId + 2], (int)ab[i]);
                }
                mesh.UnlockIndexBuffer();
            }
            else
            {
                ushort* ib = (ushort*)mesh.LockIndexBuffer(LockFlags.ReadOnly).DataPointer.ToPointer();
                for (int i = 0; i < faceCount; i++)
                {
                    int idxId = i * 3;

                    data.faces[i] = new MeshFace(ib[idxId], ib[idxId + 1], ib[idxId + 2], (int)ab[i]);
                }
                mesh.UnlockIndexBuffer();
            }

            mesh.UnlockAttributeBuffer();
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public VertexFormat Format
        {
            get { return format; }
            set { format = value; }
        }
        public MType[] Materials
        {
            get { return materials; }
            set { materials = value; }
        }

        public Vector3[] Positions
        {
            get { return positions; }
            set { positions = value; }
        }

        public Vector3[] Normals
        {
            get { return normals; }
            set { normals = value; }
        }
        public Vector2[] Texture1
        {
            get { return tex1; }
            set { tex1 = value; }
        }
        public Vector2[] Texture2
        {
            get { return tex2; }
            set { tex2 = value; }
        }
        public Vector2[] Texture3
        {
            get { return tex3; }
            set { tex3 = value; }
        }
        public Vector2[] Texture4
        {
            get { return tex4; }
            set { tex4 = value; }
        }
        public MeshFace[] Faces
        {
            get { return faces; }
            set { faces = value; }
        }

        protected GameMeshDataBase(Device dev)
        {
            device = dev;
        }

        protected abstract MType LoadMaterial(Device device, BinaryDataReader matData);
        protected abstract BinaryDataWriter SaveMaterial(MType mat);

        public void Load(BinaryDataReader data)
        {
            int materialCount = data.GetDataInt32(ChunkMaterialCount);
            materials = new MType[materialCount];

            ArchiveBinaryReader br = data.GetData(ChunkMaterials);
            for (int i = 0; i < materialCount; i++)
            {
                BinaryDataReader matData = br.ReadBinaryData();
                materials[i] = LoadMaterial(device, matData);
                matData.Close();
            }
            br.Close();

            br = data.GetData(ChunkName);
            name = br.ReadStringUnicode();
            br.Close();

            int faceCount = data.GetDataInt32(ChunkFaceCount);
            faces = new MeshFace[faceCount];

            br = data.GetData(ChunkFaces);
            for (int i = 0; i < faceCount; i++)
            {
                faces[i].IndexA = br.ReadInt32();
                faces[i].IndexB = br.ReadInt32();
                faces[i].IndexC = br.ReadInt32();

                faces[i].MaterialIndex = br.ReadInt32();
            }
            br.Close();


            format = (VertexFormat)data.GetDataInt32(ChunkVertexFormat);
            int vertexCount = data.GetDataInt32(ChunkVertexCount);

            positions = new Vector3[vertexCount];

            br = data.GetData(ChunkPositions);
            for (int i = 0; i < vertexCount; i++)
            {
                positions[i].X = br.ReadSingle();
                positions[i].Y = br.ReadSingle();
                positions[i].Z = br.ReadSingle();
            }
            br.Close();

            //VertexFormat.
            if ((format & VertexFormat.Normal) != 0)
            {
                br = data.GetData(ChunkNormals);
                normals = new Vector3[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    normals[i].X = br.ReadSingle();
                    normals[i].Y = br.ReadSingle();
                    normals[i].Z = br.ReadSingle();
                }
                br.Close();
            }

            if ((format & VertexFormat.Texture1) == VertexFormat.Texture1)
            {
                br = data.GetData(ChunkTextureCoords);
                tex1 = new Vector2[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    tex1[i].X = br.ReadSingle();
                    tex1[i].Y = br.ReadSingle();
                }
                br.Close();
            }
            else if ((format & VertexFormat.Texture2) == VertexFormat.Texture2)
            {
                br = data.GetData(ChunkTextureCoords);
                tex1 = new Vector2[vertexCount];
                tex2 = new Vector2[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    tex1[i].X = br.ReadSingle();
                    tex1[i].Y = br.ReadSingle();
                    tex2[i].X = br.ReadSingle();
                    tex2[i].Y = br.ReadSingle();
                }
                br.Close();
            }
            else if ((format & VertexFormat.Texture3) == VertexFormat.Texture3)
            {
                br = data.GetData(ChunkTextureCoords);
                tex1 = new Vector2[vertexCount];
                tex2 = new Vector2[vertexCount];
                tex3 = new Vector2[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    tex1[i].X = br.ReadSingle();
                    tex1[i].Y = br.ReadSingle();
                    tex2[i].X = br.ReadSingle();
                    tex2[i].Y = br.ReadSingle();
                    tex3[i].X = br.ReadSingle();
                    tex3[i].Y = br.ReadSingle();
                }
                br.Close();
            }
            else if ((format & VertexFormat.Texture4) == VertexFormat.Texture4)
            {
                br = data.GetData(ChunkTextureCoords);
                tex1 = new Vector2[vertexCount];
                tex2 = new Vector2[vertexCount];
                tex3 = new Vector2[vertexCount];
                tex4 = new Vector2[vertexCount];
                for (int i = 0; i < vertexCount; i++)
                {
                    tex1[i].X = br.ReadSingle();
                    tex1[i].Y = br.ReadSingle();
                    tex2[i].X = br.ReadSingle();
                    tex2[i].Y = br.ReadSingle();
                    tex3[i].X = br.ReadSingle();
                    tex3[i].Y = br.ReadSingle();
                    tex4[i].X = br.ReadSingle();
                    tex4[i].Y = br.ReadSingle();
                }
                br.Close();
            }
            //#warning todo: add more vertex elements
        }

        public BinaryDataWriter Save()
        {
            BinaryDataWriter data = new BinaryDataWriter();

            data.AddEntry(ChunkMaterialCount, materials.Length);

            ArchiveBinaryWriter bw = data.AddEntry(ChunkMaterials);
            for (int i = 0; i < materials.Length; i++)
            {
                bw.Write(SaveMaterial(materials[i]));    //bw.Write(materials[i]);
            }
            bw.Close();

            bw = data.AddEntry(ChunkName);
            bw.WriteStringUnicode(name);
            bw.Close();

            data.AddEntry(ChunkFaceCount, faces.Length);

            bw = data.AddEntry(ChunkFaces);
            for (int i = 0; i < faces.Length; i++)
            {
                bw.Write(faces[i].IndexA);
                bw.Write(faces[i].IndexB);
                bw.Write(faces[i].IndexC);
                bw.Write(faces[i].MaterialIndex);
            }
            bw.Close();

            data.AddEntry(ChunkVertexFormat, (int)format);
            data.AddEntry(ChunkVertexCount, positions.Length);

            bw = data.AddEntry(ChunkPositions);
            for (int i = 0; i < positions.Length; i++)
            {
                bw.Write(positions[i].X);
                bw.Write(positions[i].Y);
                bw.Write(positions[i].Z);
            }
            bw.Close();

            if ((format & VertexFormat.Normal) != 0)
            {
                bw = data.AddEntry(ChunkNormals);
                for (int i = 0; i < normals.Length; i++)
                {
                    bw.Write(normals[i].X);
                    bw.Write(normals[i].Y);
                    bw.Write(normals[i].Z);
                }
                bw.Close();
            }

            if ((format & VertexFormat.Texture1) == VertexFormat.Texture1)
            {
                bw = data.AddEntry(ChunkTextureCoords);
                for (int i = 0; i < tex1.Length; i++)
                {
                    bw.Write(tex1[i].X);
                    bw.Write(tex1[i].Y);
                }
                bw.Close();
            }
            else if ((format & VertexFormat.Texture2) == VertexFormat.Texture2)
            {
                bw = data.AddEntry(ChunkTextureCoords);
                for (int i = 0; i < tex2.Length; i++)
                {
                    bw.Write(tex2[i].X);
                    bw.Write(tex2[i].Y);
                }
                bw.Close();
            }
            else if ((format & VertexFormat.Texture3) == VertexFormat.Texture3)
            {
                bw = data.AddEntry(ChunkTextureCoords);
                for (int i = 0; i < tex3.Length; i++)
                {
                    bw.Write(tex3[i].X);
                    bw.Write(tex3[i].Y);
                }
                bw.Close();
            }
            else if ((format & VertexFormat.Texture4) == VertexFormat.Texture4)
            {
                bw = data.AddEntry(ChunkTextureCoords);
                for (int i = 0; i < tex4.Length; i++)
                {
                    bw.Write(tex4[i].X);
                    bw.Write(tex4[i].Y);
                }
                bw.Close();
            }
            return data;
        }

        public void Load(Stream stm)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(stm, Encoding.Default);

            int id = br.ReadInt32();

            if (id == (int)FileID.Mesh)
            {
                BinaryDataReader data = br.ReadBinaryData();
                Load(data);
                data.Close();
            }
            else
            {
                br.Close();
                throw new DataFormatException(stm.ToString());
            }

            br.Close();
        }

        public void Save(Stream stm)
        {
            ArchiveBinaryWriter bw = new ArchiveBinaryWriter(stm, Encoding.Default);

            bw.Write((int)FileID.Mesh);
            bw.Write(Save());

            bw.Close();
        }
    }

    public unsafe class GameMesh : IRenderable, IDisposable
    {
        public class Data : GameMeshDataBase<MeshMaterial>
        {
            public Data(Device dev)
                : base(dev)
            { }
            public Data(GameMesh mesh)
                : base(mesh.dev)
            { }

            protected override MeshMaterial LoadMaterial(Device device, BinaryDataReader matData)
            {
                return MeshMaterial.FromBinary(device, matData);
            }
            protected override BinaryDataWriter SaveMaterial(MeshMaterial mat)
            {
                return MeshMaterial.ToBinary(mat);
            }
        }

        VertexDeclaration vtxDecl;
        int vertexSize;

        protected VertexFormat vtxFormat;

        //GeomentryData[] bufferedGm;
        RenderOperation[] bufferedOp;

        protected MeshMaterial[] materials;

        protected VertexBuffer vertexBuffer;
        protected IndexBuffer[] indexBuffers;
        protected int[] partPrimCount;
        protected int[] partVtxCount;

        Device dev;
        string name;

        bool disposed;

        #region APIMesh
        public static Mesh BuildMesh(Device dev, int vertexCount, int faceCount, VertexFormat format)
        {
            bool useIndex16 = vertexCount <= ushort.MaxValue;
            Mesh mesh;
            if (useIndex16)
            {
                mesh = new Mesh(dev, faceCount, vertexCount, MeshFlags.Managed, format);
            }
            else
            {
                mesh = new Mesh(dev, faceCount, vertexCount, MeshFlags.Managed | MeshFlags.Use32Bit, format);
            }
            return mesh;
        }
        /// <summary>
        /// Build a Mesh object from GameMeshDataBase
        /// </summary>
        /// <typeparam name="MType"></typeparam>
        /// <param name="dev"></param>
        /// <param name="sounds"></param>
        /// <returns></returns>
        public static Mesh BuildMeshFromData<MType>(Device dev, GameMeshDataBase<MType> data)
            where MType : class//MeshMaterialBase<Texture>
        {
            Mesh mesh;

            int matCount = data.Materials.Length;
            int vertexCount = data.Positions.Length;
            int faceCount = data.Faces.Length;

            bool useIndex16 = vertexCount <= ushort.MaxValue;

            if (useIndex16)
            {
                mesh = new Mesh(dev, faceCount, vertexCount, MeshFlags.Managed, data.Format);
            }
            else
            {
                mesh = new Mesh(dev, faceCount, vertexCount, MeshFlags.Managed | MeshFlags.Use32Bit, data.Format);
            }
            //int* attr = (int*)mesh.LockAttributeBuffer(LockFlags.None).DataPointer.ToPointer();


            //indexBuffers = new IndexBuffer[matCount];
            //vertexCounts = new int[matCount];

            Vector3[] pos = data.Positions;
            Vector3[] n = data.Normals;
            Vector2[] tex1 = data.Texture1;
            Vector2[] tex2 = data.Texture2;
            Vector2[] tex3 = data.Texture3;
            Vector2[] tex4 = data.Texture4;

            //format = sounds.Format;

            if (data.Format == VertexPNT1.Format)
            {
                VertexPNT1* dst = (VertexPNT1*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT1* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u = tex1[i].X;
                    vtx->v = tex1[i].Y;
                }

                mesh.UnlockVertexBuffer();
            }
            else if (data.Format == VertexPNT2.Format)
            {
                VertexPNT2* dst = (VertexPNT2*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT2* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                }
                mesh.UnlockVertexBuffer();

            }
            else if (data.Format == VertexPNT3.Format)
            {
                VertexPNT3* dst = (VertexPNT3*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT3* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;
                }
                mesh.UnlockVertexBuffer();
            }
            else if (data.Format == VertexPNT4.Format)
            {
                VertexPNT4* dst = (VertexPNT4*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT4* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;

                    vtx->u4 = tex4[i].X;
                    vtx->v4 = tex4[i].Y;
                }
                mesh.UnlockVertexBuffer();
            }
            else if (data.Format == VertexPT1.Format)
            {
                VertexPT1* dst = (VertexPT1*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT1* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;
                }
                mesh.UnlockVertexBuffer();
            }
            else if (data.Format == VertexPT2.Format)
            {
                VertexPT2* dst = (VertexPT2*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT2* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;
                }
                mesh.UnlockVertexBuffer();
            }
            else if (data.Format == VertexPT3.Format)
            {
                VertexPT3* dst = (VertexPT3*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT3* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;
                }
                mesh.UnlockVertexBuffer();
            }
            else if (data.Format == VertexPT4.Format)
            {
                VertexPT4* dst = (VertexPT4*)mesh.LockVertexBuffer(LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT4* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;

                    vtx->u4 = tex4[i].X;
                    vtx->v4 = tex4[i].Y;
                }
                mesh.UnlockVertexBuffer();
            }
            else
            {
                throw new NotSupportedException(data.Format.ToString());
            }
            //vtxDecl = new VertexDeclaration(dev, D3DX.DeclaratorFromFVF(format));

            List<int>[] indices = new List<int>[matCount];
            for (int i = 0; i < matCount; i++)
            {
                indices[i] = new List<int>();
            }


            MeshFace[] faces = data.Faces;
            for (int i = 0; i < faces.Length; i++)
            {
                MeshFace face = faces[i];
                int matId = face.MaterialIndex;

                indices[matId].Add(face.IndexA);
                indices[matId].Add(face.IndexB);
                indices[matId].Add(face.IndexC);
            }


            uint* ab = (uint*)mesh.LockAttributeBuffer(LockFlags.None).DataPointer.ToPointer();

            if (useIndex16)
            {
                ushort* ib = (ushort*)mesh.LockIndexBuffer(LockFlags.None).DataPointer.ToPointer();

                int faceIdx = 0;
                for (int i = 0; i < matCount; i++)
                {
                    List<int> idx = indices[i];
                    for (int j = 0; j < idx.Count; j += 3)
                    {
                        ab[faceIdx] = (uint)i;

                        int ibIdx = faceIdx * 3;
                        ib[ibIdx] = (ushort)idx[j];
                        ib[ibIdx + 1] = (ushort)idx[j + 1];
                        ib[ibIdx + 2] = (ushort)idx[j + 2];

                        faceIdx++;
                    }
                }

                mesh.UnlockIndexBuffer();
            }
            else
            {
                uint* ib = (uint*)mesh.LockIndexBuffer(LockFlags.None).DataPointer.ToPointer();

                int faceIdx = 0;
                for (int i = 0; i < matCount; i++)
                {
                    List<int> idx = indices[i];
                    for (int j = 0; j < idx.Count; j += 3)
                    {
                        ab[faceIdx] = (uint)i;

                        int ibIdx = faceIdx * 3;
                        ib[ibIdx] = (ushort)idx[j];
                        ib[ibIdx + 1] = (ushort)idx[j + 1];
                        ib[ibIdx + 2] = (ushort)idx[j + 2];

                        faceIdx++;
                    }
                }

                mesh.UnlockIndexBuffer();
            }
            mesh.UnlockAttributeBuffer();
            return mesh;
        }
        #endregion


        public VertexFormat Format
        {
            get { return vtxFormat; }
        }

        public unsafe void BuildFromData(Device dev, GameMeshDataBase<MeshMaterial> data)
        {
            materials = data.Materials;

            int matCount = data.Materials.Length;
            int vertexCount = data.Positions.Length;
            int faceCount = data.Faces.Length;

            bool useIndex16 = vertexCount <= ushort.MaxValue;

            Vector3[] pos = data.Positions;
            Vector3[] n = data.Normals;
            Vector2[] tex1 = data.Texture1;
            Vector2[] tex2 = data.Texture2;
            Vector2[] tex3 = data.Texture3;
            Vector2[] tex4 = data.Texture4;

            vtxFormat = data.Format;
            vtxDecl = new VertexDeclaration(dev, D3DX.DeclaratorFromFVF(data.Format));

            if (data.Format == VertexPNT1.Format)
            {
                vertexSize = sizeof(VertexPNT1);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPNT1.Format, Pool.Managed);
                VertexPNT1* dst = (VertexPNT1*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT1* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u = tex1[i].X;
                    vtx->v = tex1[i].Y;
                }

                vertexBuffer.Unlock();
            }
            else if (data.Format == VertexPNT2.Format)
            {
                vertexSize = sizeof(VertexPNT2);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPNT2.Format, Pool.Managed);
                VertexPNT2* dst = (VertexPNT2*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT2* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                }
                vertexBuffer.Unlock();
            }
            else if (data.Format == VertexPNT3.Format)
            {
                vertexSize = sizeof(VertexPNT3);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPNT3.Format, Pool.Managed);
                VertexPNT3* dst = (VertexPNT3*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT3* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;
                }
                vertexBuffer.Unlock();
            }
            else if (data.Format == VertexPNT4.Format)
            {
                vertexSize = sizeof(VertexPNT4);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPNT4.Format, Pool.Managed);
                VertexPNT4* dst = (VertexPNT4*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPNT4* vtx = dst + i;
                    vtx->pos = pos[i];
                    vtx->n = n[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;

                    vtx->u4 = tex4[i].X;
                    vtx->v4 = tex4[i].Y;
                }
                vertexBuffer.Unlock();
            }
            else if (data.Format == VertexPT1.Format)
            {
                vertexSize = sizeof(VertexPT1);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPT1.Format, Pool.Managed);
                VertexPT1* dst = (VertexPT1*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT1* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;
                }
                vertexBuffer.Unlock();
            }
            else if (data.Format == VertexPT2.Format)
            {
                vertexSize = sizeof(VertexPT2);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPT2.Format, Pool.Managed);
                VertexPT2* dst = (VertexPT2*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT2* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;
                }
                vertexBuffer.Unlock();
            }
            else if (data.Format == VertexPT3.Format)
            {
                vertexSize = sizeof(VertexPT3);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPT3.Format, Pool.Managed);
                VertexPT3* dst = (VertexPT3*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT3* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;
                }
                vertexBuffer.Unlock();
            }
            else if (data.Format == VertexPT4.Format)
            {
                vertexSize = sizeof(VertexPT4);
                vertexBuffer = new VertexBuffer(dev, vertexSize * vertexCount, Usage.WriteOnly, VertexPT4.Format, Pool.Managed);
                VertexPT4* dst = (VertexPT4*)vertexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                for (int i = 0; i < vertexCount; i++)
                {
                    VertexPT4* vtx = dst + i;
                    vtx->pos = pos[i];

                    vtx->u1 = tex1[i].X;
                    vtx->v1 = tex1[i].Y;

                    vtx->u2 = tex2[i].X;
                    vtx->v2 = tex2[i].Y;

                    vtx->u3 = tex3[i].X;
                    vtx->v3 = tex3[i].Y;

                    vtx->u4 = tex4[i].X;
                    vtx->v4 = tex4[i].Y;
                } 
                vertexBuffer.Unlock();
            }
            else
            {
                throw new NotSupportedException(data.Format.ToString());
            }

            List<int>[] indices = new List<int>[matCount];
            for (int i = 0; i < matCount; i++)
            {
                indices[i] = new List<int>();
            }

            partPrimCount = new int[matCount];
            partVtxCount = new int[matCount];

            MeshFace[] faces = data.Faces;
            for (int i = 0; i < faces.Length; i++)
            {
                MeshFace face = faces[i];
                int matId = face.MaterialIndex;

                indices[matId].Add(face.IndexA);
                indices[matId].Add(face.IndexB);
                indices[matId].Add(face.IndexC);
                partPrimCount[matId]++;
            }

            bool[] passed = new bool[vertexCount];
            if (useIndex16)
            {
                indexBuffers = new IndexBuffer[matCount];
               
                for (int i = 0; i < matCount; i++)
                {
                    fixed (void* dst = &passed[0])
                    {
                        Memory.Zero(dst, vertexCount);
                    }

                    List<int> idx = indices[i];
                    indexBuffers[i] = new IndexBuffer(dev, idx.Count * sizeof(ushort), Usage.WriteOnly, Pool.Managed, true);

                    ushort* ib = (ushort*)indexBuffers[i].Lock(0, 0, LockFlags.None).DataPointer.ToPointer();
                    for (int j = 0; j < idx.Count; j++)
                    {
                        ib[j] = (ushort)idx[j];
                        passed[idx[j]] = true;
                    }
                    indexBuffers[i].Unlock();


                    int vtxCount = 0;
                    for (int j = 0; j < vertexCount; j++)
                        if (passed[j])
                            vtxCount++;
                    partVtxCount[i] = vtxCount;
                }
            }
            else
            {
                indexBuffers = new IndexBuffer[matCount];

                for (int i = 0; i < matCount; i++)
                {
                    fixed (void* dst = &passed[0])
                    {
                        Memory.Zero(dst, vertexCount);
                    }
                    List<int> idx = indices[i];
                    indexBuffers[i] = new IndexBuffer(dev, idx.Count * sizeof(uint), Usage.WriteOnly, Pool.Managed, false);

                    uint* ib = (uint*)indexBuffers[i].Lock(0, 0, LockFlags.None).DataPointer.ToPointer();
                    for (int j = 0; j < idx.Count; j++)
                    {
                        ib[j] = (uint)idx[j];
                    }
                    indexBuffers[i].Unlock();

                    int vtxCount = 0;
                    for (int j = 0; j < vertexCount; j++)
                        if (passed[j])
                            vtxCount++;
                    partVtxCount[i] = vtxCount;
                }
            }

        }


        public static GameMesh FromStream(Device dev, Stream stm)
        {
            Data data = new Data(dev);
            data.Load(stm);
            return new GameMesh(dev, data);
        }

        public static void ToStream(GameMesh mesh, Stream stm)
        {
            Data data = new Data(mesh);
            data.Save(stm);
        }

        //public GameMesh(Mesh mesh)
        //{
        //    this.dev = mesh.Device;
        //    this.mesh = mesh;
        //    name = string.Empty;
        //    ExtendedMaterial[] mmat = mesh.GetMaterials();

        //    if (mmat != null)
        //    {
        //        materials = new MeshMaterial[mmat.Length];
        //        for (int i = 0; i < mmat.Length; i++)
        //        {
        //            materials[i] = new MeshMaterial(dev);
        //            materials[i].mat = mmat[i].MaterialD3D;
        //        }
        //    }
        //    else
        //    {
        //        materials = new MeshMaterial[1];
        //        materials[0] = new MeshMaterial(dev);
        //        materials[0].mat = MeshMaterial.DefaultMatColor;
        //    }
        //}
        public GameMesh(Device dev, Data data)
        {
            this.dev = dev;

            BuildFromData(dev, data);
            //mesh = BuildMeshFromData(dev, sounds);
            //for (int i = 0; i < matCount; i++)
            //{
            //    bool useIndex16 = indices[i].Count <= ushort.MaxValue;

            //    List<int> idx = indices[i];
            //    vertexCounts[i] = idx.Count;

            //    int ibSize = idx.Count * (useIndex16 ? sizeof(ushort) : sizeof(uint));
            //    indexBuffers[i] = new IndexBuffer(dev, ibSize, Usage.None, Pool.Managed, useIndex16);


            //    if (useIndex16)
            //    {
            //        ushort* idst = (ushort*)indexBuffers[i].Lock(0, ibSize, LockFlags.None).DataPointer.ToPointer();

            //        for (int j = 0; j < idx.Count; j++)
            //        {
            //            idst[j] = (ushort)idx[j];
            //        }
            //        indexBuffers[i].Unlock();
            //    }
            //    else
            //    {
            //        uint* idst = (uint*)indexBuffers[i].Lock(0, ibSize, LockFlags.None).DataPointer.ToPointer();

            //        for (int j = 0; j < idx.Count; j++)
            //        {
            //            idst[j] = (uint)idx[j];
            //        }
            //        indexBuffers[i].Unlock();
            //    }

            //}
        }
        //public GameMesh(Device dev)
        //{
        //    this.dev = dev;
        //}

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        //public GameMesh(ResourceLocation fl)
        //{
        //    GameMesh.Data sounds = new GameMesh.Data();
        //    sounds.ResourceLocation = fl;
        //    sounds.Load();

        //    indexBuffers = new List<IndexBuffer>();
        //    vertexBuffers = new List<VertexBuffer>();
        //}

        //#region IRenderable 成员

        //public RenderOperation[] GetRenderOperation()
        //{
        //    RenderOperation[] op = new RenderOperation[materials.Length];

        //    for (int i = 0; i < materials.Length; i++)
        //    {
        //        RenderOperation cop = new RenderOperation(this);

        //        cop.Material = materials[i];
        //        cop.IndexBuffer = indexBuffers[i];
        //        cop.VertexBuffer = vertexBuffer;
        //        cop.VertexDeclaration = vtxDecl;
        //        cop.PrimitiveType = PrimitiveType.TriangleList;
        //        op[i] = cop;
        //    }

        //    return op;
        //}

        //#endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                //if (mesh != null)
                //{
                //    mesh.Dispose();
                //    mesh = null;
                //}
                bufferedOp = null;
                if (vertexBuffer != null)
                {
                    vertexBuffer.Dispose();
                    vertexBuffer = null;
                }
                if (indexBuffers != null)
                {
                    for (int i = 0; i < indexBuffers.Length; i++)
                    {
                        indexBuffers[i].Dispose();
                    }
                    indexBuffers = null;
                }
                if (vtxDecl != null)
                    vtxDecl.Dispose();

                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].Dispose();

                }
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion

        ~GameMesh()
        {
            if (!disposed)
                Dispose();
        }


        #region IRenderable 成员


        public RenderOperation[] GetRenderOperation()
        {
            if (bufferedOp == null)
            {
                //bufferedGm = new GeomentryData[materials.Length];
                bufferedOp = new RenderOperation[materials.Length];
                for (int i = 0; i < bufferedOp.Length; i++)
                {
                    GeomentryData gd = new GeomentryData(this);
                    gd.Format = vtxFormat;
                    //bufferedGm[i].Material = materials[i];
                    gd.IndexBuffer = indexBuffers[i];
                    gd.PrimCount = partPrimCount[i];
                    gd.PrimitiveType = PrimitiveType.TriangleList;
                    gd.VertexBuffer = vertexBuffer;
                    gd.VertexCount = partVtxCount[i];
                    gd.VertexDeclaration = vtxDecl;
                    gd.VertexSize = vertexSize;

                    bufferedOp[i].Material = materials[i];
                    bufferedOp[i].Geomentry = gd;
                    //bufferedOp[i].Transformation 
                }               
            }
            return bufferedOp;
        }

        //public void Render()
        //{
           
            //if (mesh != null)
            //{
            //    for (int i = 0; i < materials.Length; i++)
            //    {
            //        dev.SetRenderState(RenderState.ZWriteEnable, !materials[i].IsTransparent);
            //        dev.SetRenderState<Cull>(RenderState.CullMode, materials[i].IsTwoSided ? Cull.None : Cull.Clockwise);

            //        dev.Material = materials[i].mat;

            //        for (int j = 0; j < MaterialBase.MaxTexLayers; j++)
            //        {
            //            dev.SetTexture(j, materials[i].GetTexture(j));
            //        }
            //        mesh.DrawSubset(i);
            //    }
            //}

            //dev.VertexDeclaration = vtxDecl;
            //dev.VertexFormat = format;

            //for (int i = 0; i < materials.Length; i++)
            //{
            //    //#warning material
            //    dev.Material = materials[i].mat;
            //    dev.SetTexture(0, materials[i].Texture1);
            //    dev.SetTexture(1, materials[i].Texture2);
            //    dev.SetTexture(2, materials[i].Texture3);
            //    dev.SetTexture(3, materials[i].Texture4);


            //    dev.Indices = indexBuffers[i];
            //    dev.SetStreamSource(0, vertexBuffer, 0, vertexSize);
            //    dev.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertexCounts[i], 0, vertexCounts[i] / 3);
            //}

        //}
        //public int BatchCount
        //{
        //    get { return materials.Length; }
        //}

        //public MeshMaterial[] Materials
        //{
        //    get { return materials; }
        //}

        //public void RenderBatch(int index)
        //{
        //    mesh.DrawSubset(index);
        //}

        #endregion

    }
}
