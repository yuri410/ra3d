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
using System.Text;
using Ra2Develop.Converters;
using Ra2Develop.Designers;
using R3D;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;
using System.ComponentModel;
using System.Drawing.Design;

namespace Ra2Develop.Editors.EditableObjects
{
    public class EditableMesh : GameMeshDataBase<EditableMeshMaterial>, IDisposable
    {
        [Flags()]
        public enum MeshVertexElement
        {
            None = 0,
            Position = 1 << 0,
            Normal = 1 << 1,
            Tex1 = 1 << 2,
            Tex2 = 1 << 3,
            Tex3 = 1 << 4,
            Tex4 = 1 << 5
        }

        bool disposed;

        Mesh mesh;

        [Editor(typeof(VertexFormatEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(VertexFormatConverter))]
        public new VertexFormat Format
        {
            get { return base.Format; }
            set
            {
                if (base.Format != value)
                {
                    base.Format = value;
                    if ((value & VertexFormat.Normal) == VertexFormat.Normal)
                    {
                        if (normals == null) normals = new Vector3[positions.Length];
                    }
                    else
                    {
                        normals = null;
                    }

                    if ((value & VertexFormat.Texture1) == VertexFormat.Texture1)
                    {
                        if (tex1 == null) tex1 = new Vector2[positions.Length];
                    }
                    else if ((value & VertexFormat.Texture2) == VertexFormat.Texture2)
                    {
                        if (tex1 == null) tex1 = new Vector2[positions.Length];
                        if (tex2 == null) tex2 = new Vector2[positions.Length];
                    }
                    else if ((value & VertexFormat.Texture3) == VertexFormat.Texture3)
                    {
                        if (tex1 == null) tex1 = new Vector2[positions.Length];
                        if (tex2 == null) tex2 = new Vector2[positions.Length];
                        if (tex3 == null) tex3 = new Vector2[positions.Length];
                    }
                    else if ((value & VertexFormat.Texture4) == VertexFormat.Texture4)
                    {
                        if (tex1 == null) tex1 = new Vector2[positions.Length];
                        if (tex2 == null) tex2 = new Vector2[positions.Length];
                        if (tex3 == null) tex3 = new Vector2[positions.Length];
                        if (tex4 == null) tex4 = new Vector2[positions.Length];
                    }
                    else if ((value & VertexFormat.Texture0) == VertexFormat.Texture0)
                    {
                        tex1 = null;
                        tex2 = null;
                        tex3 = null;
                        tex4 = null;
                    }
                    Update();
                }
            }
        }

        public void SetVertexFormat(VertexFormat fmt)
        {
            base.Format = fmt;
        }

        protected Mesh Mesh
        {
            get
            {
                if (RequiresUpdate())
                {
                    Update();
                }
                return mesh;
            }
            private set
            {
                if (value != mesh && !IsReleased(mesh))
                {
                    mesh.Dispose();
                }
                mesh = value;
            }
        }

        public EditableMesh()
            : base(GraphicsDevice.Instance.Device)
        { }
        public EditableMesh(string name, Mesh mesh, EditableMeshMaterial[] mats)
            : base(GraphicsDevice.Instance.Device)
        {
            materials = mats;
            this.Name = name;

            GameMeshDataBase<EditableMeshMaterial>.BuildFromMesh(mesh, this, mats);
            this.mesh = mesh;
        }
        protected override EditableMeshMaterial LoadMaterial(Device device, BinaryDataReader matData)
        {
            return EditableMeshMaterial.FromBinary(device, matData);
        }
        protected override BinaryDataWriter SaveMaterial(EditableMeshMaterial mat)
        {
            return EditableMeshMaterial.ToBinary(mat);
        }

        static bool IsReleased(Mesh m)
        {
            return m == null || m.Disposed;
        }
        public void ComputeNormals()
        {
            Mesh.ComputeNormals();
            BuildFromMesh(mesh, this, this.materials);
        }

        public void ComputeFlatNormal()
        {
            Dictionary<string, int> table = new Dictionary<string, int>(faces.Length * 3);
            bool uv2 = tex2 != null;
            bool uv3 = tex3 != null;
            bool uv4 = tex4 != null;

            List<Vector3> newPos = new List<Vector3>(faces.Length * 3);
            List<Vector3> newN = new List<Vector3>(faces.Length * 3);
            List<Vector2> newTex1 = new List<Vector2>(faces.Length * 3);

            List<Vector2> newTex2 = null;
            if (uv2)
                newTex2 = new List<Vector2>(faces.Length * 3);

            List<Vector2> newTex3 = null;
            if (uv3)
                newTex3 = new List<Vector2>(faces.Length * 3);

            List<Vector2> newTex4 = null;
            if (uv4)
                newTex4 = new List<Vector2>(faces.Length * 3);

            for (int i = 0; i < faces.Length; i++)
            {
                int a = faces[i].IndexA;
                int b = faces[i].IndexB;
                int c = faces[i].IndexC;

                Vector3 n;
                MathEx.ComputePlaneNormal(ref positions[a], ref positions[b], ref positions[c], out n);

                int idx;
                string desc = GetVertexDescription(a, ref n, true, true, uv2, uv3, uv4);
                if (!table.TryGetValue(desc, out idx))
                {
                    idx = newPos.Count;

                    table.Add(desc, idx);
                    newPos.Add(positions[a]);
                    newN.Add(n);
                    newTex1.Add(tex1[a]);
                    if (uv2)
                        newTex2.Add(tex2[a]);
                    if (uv3)
                        newTex3.Add(tex3[a]);
                    if (uv4)
                        newTex4.Add(tex4[a]);
                }
                faces[i].IndexA = idx;

                desc = GetVertexDescription(b, ref n, true, true, uv2, uv3, uv4);
                if (!table.TryGetValue(desc, out idx))
                {
                    idx = newPos.Count;

                    table.Add(desc, idx);
                    newPos.Add(positions[b]);
                    newN.Add(n);
                    newTex1.Add(tex1[b]);
                    if (uv2)
                        newTex2.Add(tex2[b]);
                    if (uv3)
                        newTex3.Add(tex3[b]);
                    if (uv4)
                        newTex4.Add(tex4[b]);
                }
                faces[i].IndexB = idx;

                desc = GetVertexDescription(c, ref n, true, true, uv2, uv3, uv4);
                if (!table.TryGetValue(desc, out idx))
                {
                    idx = newPos.Count;

                    table.Add(desc, idx);
                    newPos.Add(positions[c]);
                    newN.Add(n);
                    newTex1.Add(tex1[c]);
                    if (uv2)
                        newTex2.Add(tex2[c]);
                    if (uv3)
                        newTex3.Add(tex3[c]);
                    if (uv4)
                        newTex4.Add(tex4[c]);
                }
                faces[i].IndexC = idx;

            }

            positions = newPos.ToArray();
            normals = newN.ToArray();

            tex1 = newTex1.ToArray();

            if (uv2)
                tex2 = newTex2.ToArray();

            if (uv3)
                tex3 = newTex3.ToArray();

            if (uv4)
                tex4 = newTex4.ToArray();
            Update();
        }

        [Obsolete()]
        private void ComputeFlatNormalsTex1(Dictionary<string, int> table)
        {
            List<VertexPNT1> vertices = new List<VertexPNT1>(faces.Length * 3);

            for (int i = 0; i < faces.Length; i++)
            {
                VertexPNT1 vtxA;
                VertexPNT1 vtxB;
                VertexPNT1 vtxC;

                int idx = faces[i].IndexA;
                vtxA.pos = positions[idx];
                vtxA.u = tex1[idx].X;
                vtxA.v = tex1[idx].Y;
                //vtxA.tex1 = tex1[idx];

                idx = faces[i].IndexB;
                vtxB.pos = positions[idx];
                vtxB.u = tex1[idx].X;
                vtxB.v = tex1[idx].Y;
                //vtxB.tex1 = tex1[idx];

                idx = faces[i].IndexC;
                vtxC.pos = positions[idx];
                vtxC.u = tex1[idx].X;
                vtxC.v = tex1[idx].Y;
                //vtxC.tex1 = tex1[idx];

                MathEx.ComputePlaneNormal(ref vtxA.pos, ref vtxB.pos, ref vtxC.pos, out vtxA.n);
                vtxB.n = vtxA.n;
                vtxC.n = vtxA.n;


                string desc = vtxA.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxA);
                }

                desc = vtxB.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexB = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexB = vertices.Count;
                    vertices.Add(vtxB);
                }

                desc = vtxC.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexC = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexC = vertices.Count;
                    vertices.Add(vtxC);
                }
            }

            positions = new Vector3[vertices.Count];
            normals = new Vector3[vertices.Count];
            tex1 = new Vector2[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                VertexPNT1 vtx = vertices[i];
                positions[i] = vtx.pos;
                normals[i] = vtx.n;
                tex1[i] = new Vector2(vtx.u, vtx.v);
            }
        }
        [Obsolete()]
        private void ComputeFlatNormalsTex2(Dictionary<string, int> table)
        {
            List<VertexPNT2> vertices = new List<VertexPNT2>(faces.Length * 3);

            for (int i = 0; i < faces.Length; i++)
            {
                VertexPNT2 vtxA;
                VertexPNT2 vtxB;
                VertexPNT2 vtxC;

                int idx = faces[i].IndexA;
                vtxA.pos = positions[idx];
                vtxA.u1 = tex1[idx].X;
                vtxA.v1 = tex1[idx].Y;
                vtxA.u2 = tex2[idx].X;
                vtxA.v2 = tex2[idx].Y;


                idx = faces[i].IndexB;
                vtxB.pos = positions[idx];
                vtxB.u1 = tex1[idx].X;
                vtxB.v1 = tex1[idx].Y;
                vtxB.u2 = tex2[idx].X;
                vtxB.v2 = tex2[idx].Y;


                idx = faces[i].IndexC;
                vtxC.pos = positions[idx];
                vtxC.u1 = tex1[idx].X;
                vtxC.v1 = tex1[idx].Y;
                vtxC.u2 = tex2[idx].X;
                vtxC.v2 = tex2[idx].Y;


                MathEx.ComputePlaneNormal(ref vtxA.pos, ref vtxB.pos, ref vtxC.pos, out vtxA.n);
                vtxB.n = vtxA.n;
                vtxC.n = vtxA.n;


                string desc = vtxA.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxA);
                }

                desc = vtxB.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxB);
                }

                desc = vtxC.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxC);
                }
            }

            positions = new Vector3[vertices.Count];
            normals = new Vector3[vertices.Count];
            tex1 = new Vector2[vertices.Count];
            tex2 = new Vector2[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                VertexPNT2 vtx = vertices[i];
                positions[i] = vtx.pos;
                normals[i] = vtx.n;
                tex1[i] = new Vector2(vtx.u1, vtx.v1);
                tex2[i] = new Vector2(vtx.u2, vtx.v2);
            }
        }
        [Obsolete()]
        private void ComputeFlatNormalsTex3(Dictionary<string, int> table)
        {
            List<VertexPNT3> vertices = new List<VertexPNT3>(faces.Length * 3);

            for (int i = 0; i < faces.Length; i++)
            {
                VertexPNT3 vtxA;
                VertexPNT3 vtxB;
                VertexPNT3 vtxC;

                int idx = faces[i].IndexA;
                vtxA.pos = positions[idx];
                vtxA.u1 = tex1[idx].X;
                vtxA.v1 = tex1[idx].Y;
                vtxA.u2 = tex2[idx].X;
                vtxA.v2 = tex2[idx].Y;
                vtxA.u3 = tex3[idx].X;
                vtxA.v3 = tex3[idx].Y;


                idx = faces[i].IndexB;
                vtxB.pos = positions[idx];
                vtxB.u1 = tex1[idx].X;
                vtxB.v1 = tex1[idx].Y;
                vtxB.u2 = tex2[idx].X;
                vtxB.v2 = tex2[idx].Y;
                vtxB.u3 = tex3[idx].X;
                vtxB.v3 = tex3[idx].Y;


                idx = faces[i].IndexC;
                vtxC.pos = positions[idx];
                vtxC.u1 = tex1[idx].X;
                vtxC.v1 = tex1[idx].Y;
                vtxC.u2 = tex2[idx].X;
                vtxC.v2 = tex2[idx].Y;
                vtxC.u3 = tex3[idx].X;
                vtxC.v3 = tex3[idx].Y;


                MathEx.ComputePlaneNormal(ref vtxA.pos, ref vtxB.pos, ref vtxC.pos, out vtxA.n);
                vtxB.n = vtxA.n;
                vtxC.n = vtxA.n;


                string desc = vtxA.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxA);
                }

                desc = vtxB.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxB);
                }

                desc = vtxC.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxC);
                }
            }

            positions = new Vector3[vertices.Count];
            normals = new Vector3[vertices.Count];
            tex1 = new Vector2[vertices.Count];
            tex2 = new Vector2[vertices.Count];
            tex3 = new Vector2[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                VertexPNT3 vtx = vertices[i];
                positions[i] = vtx.pos;
                normals[i] = vtx.n;
                tex1[i] = new Vector2(vtx.u1, vtx.v1);
                tex2[i] = new Vector2(vtx.u2, vtx.v2);
                tex3[i] = new Vector2(vtx.u3, vtx.v3);
            }
        }
        [Obsolete()]
        private void ComputeFlatNormalsTex4(Dictionary<string, int> table)
        {
            List<VertexPNT4> vertices = new List<VertexPNT4>(faces.Length * 3);

            for (int i = 0; i < faces.Length; i++)
            {
                VertexPNT4 vtxA;
                VertexPNT4 vtxB;
                VertexPNT4 vtxC;

                int idx = faces[i].IndexA;
                vtxA.pos = positions[idx];
                vtxA.u1 = tex1[idx].X;
                vtxA.v1 = tex1[idx].Y;
                vtxA.u2 = tex2[idx].X;
                vtxA.v2 = tex2[idx].Y;
                vtxA.u3 = tex3[idx].X;
                vtxA.v3 = tex3[idx].Y;
                vtxA.u4 = tex4[idx].X;
                vtxA.v4 = tex4[idx].Y;


                idx = faces[i].IndexB;
                vtxB.pos = positions[idx];
                vtxB.u1 = tex1[idx].X;
                vtxB.v1 = tex1[idx].Y;
                vtxB.u2 = tex2[idx].X;
                vtxB.v2 = tex2[idx].Y;
                vtxB.u3 = tex3[idx].X;
                vtxB.v3 = tex3[idx].Y;
                vtxB.u4 = tex4[idx].X;
                vtxB.v4 = tex4[idx].Y;


                idx = faces[i].IndexC;
                vtxC.pos = positions[idx];
                vtxC.u1 = tex1[idx].X;
                vtxC.v1 = tex1[idx].Y;
                vtxC.u2 = tex2[idx].X;
                vtxC.v2 = tex2[idx].Y;
                vtxC.u3 = tex3[idx].X;
                vtxC.v3 = tex3[idx].Y;
                vtxC.u4 = tex4[idx].X;
                vtxC.v4 = tex4[idx].Y;


                MathEx.ComputePlaneNormal(ref vtxA.pos, ref vtxB.pos, ref vtxC.pos, out vtxA.n);
                vtxB.n = vtxA.n;
                vtxC.n = vtxA.n;


                string desc = vtxA.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxA);
                }

                desc = vtxB.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxB);
                }

                desc = vtxC.ToString();
                if (table.TryGetValue(desc, out idx))
                {
                    faces[i].IndexA = idx;
                }
                else
                {
                    table.Add(desc, vertices.Count);
                    faces[i].IndexA = vertices.Count;
                    vertices.Add(vtxC);
                }
            }

            positions = new Vector3[vertices.Count];
            normals = new Vector3[vertices.Count];
            tex1 = new Vector2[vertices.Count];
            tex2 = new Vector2[vertices.Count];
            tex3 = new Vector2[vertices.Count];
            tex4 = new Vector2[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                VertexPNT4 vtx = vertices[i];
                positions[i] = vtx.pos;
                normals[i] = vtx.n;
                tex1[i] = new Vector2(vtx.u1, vtx.v1);
                tex2[i] = new Vector2(vtx.u2, vtx.v2);
                tex3[i] = new Vector2(vtx.u3, vtx.v3);
                tex4[i] = new Vector2(vtx.u4, vtx.v4);
            }
        }
        [Obsolete()]
        private void ComputeFlatNormals()
        {
            Dictionary<string, int> table = new Dictionary<string, int>(faces.Length * 3);

            if (Format == VertexPNT1.Format)
            {
                ComputeFlatNormalsTex1(table);
            }
            else if (Format == VertexPNT2.Format)
            {
                ComputeFlatNormalsTex2(table);
            }
            else if (Format == VertexPNT3.Format)
            {
                ComputeFlatNormalsTex3(table);
            }
            else if (Format == VertexPNT4.Format)
            {
                ComputeFlatNormalsTex4(table);
            }

            Update();
        }

        public void Optmize(MeshOptimizeFlags flags)
        {
            Mesh.OptimizeInPlace(flags);

            BuildFromMesh(mesh, this, this.materials);
        }

        public void Subdevide(float tessellation)
        {
            if (tessellation < 1f)
            {
                return;
            }
            if (tessellation > 32f)
            {
                tessellation = 32f;
            }

            PatchMesh patch = new PatchMesh(Mesh);

            float cubicTess = tessellation * tessellation * tessellation;
            int faceCount = (int)(mesh.FaceCount * cubicTess);
            int vtxCount = (int)(mesh.VertexCount * cubicTess);

            Mesh newMesh = GameMesh.BuildMesh(device, vtxCount, faceCount, mesh.VertexFormat);

            patch.Tessellate(tessellation, newMesh);

            patch.Dispose();

            Mesh = newMesh;
        }

       
        public void Simplify(MeshSimplifier ms, int[] map, int vtxCount)
        {
            List<Vector3> posList = new List<Vector3>(positions.Length);
            List<Vector3> nList = new List<Vector3>(normals.Length);
            List<Vector2> tex1List = new List<Vector2>(tex1.Length);
            List<Vector2> tex2List = null;
            if (tex2 != null)
            {
                tex2List = new List<Vector2>(tex2.Length);
            }
            List<Vector2> tex3List = null;
            if (tex3 != null)
            {
                tex3List = new List<Vector2>(tex3.Length);
            }
            List<Vector2> tex4List = null;
            if (tex4 != null)
            {
                tex4List = new List<Vector2>(tex4.Length);
            }

            List<MeshFace> faceList = new List<MeshFace>(faces.Length);
            Pair<bool, int>[] useState = new Pair<bool, int>[positions.Length];

            for (int i = 0; i < faces.Length; i++)
            {
                int p0 = ms.Map(map, faces[i].IndexA, vtxCount);
                int p1 = ms.Map(map, faces[i].IndexB, vtxCount);
                int p2 = ms.Map(map, faces[i].IndexC, vtxCount);

                if (p0 == p1 || p1 == p2 || p2 == p0)
                    continue;


                if (!useState[p0].a)
                {
                    useState[p0].b = posList.Count;
                    faces[i].IndexA = posList.Count;

                    posList.Add(positions[p0]);
                    nList.Add(normals[p0]);
                    tex1List.Add(tex1[p0]);
                    if (tex2 != null)
                    {
                        tex2List.Add(tex2[p0]);
                    }
                    if (tex3 != null)
                    {
                        tex3List.Add(tex3[p0]);
                    }
                    if (tex4 != null)
                    {
                        tex4List.Add(tex4[p0]);
                    }
                }
                else
                {
                    faces[i].IndexA = useState[p0].b;
                }

                if (!useState[p1].a)
                {
                    useState[p1].b = posList.Count;
                    faces[i].IndexB = posList.Count;

                    posList.Add(positions[p1]);
                    nList.Add(normals[p1]);
                    tex1List.Add(tex1[p1]);
                    if (tex2 != null)
                    {
                        tex2List.Add(tex2[p1]);
                    }
                    if (tex3 != null)
                    {
                        tex3List.Add(tex3[p1]);
                    }
                    if (tex4 != null)
                    {
                        tex4List.Add(tex4[p1]);
                    }
                }
                else
                {
                    faces[i].IndexB = useState[p1].b;
                }

                if (!useState[p2].a)
                {
                    useState[p2].b = posList.Count;
                    faces[i].IndexC = posList.Count;

                    posList.Add(positions[p2]);
                    nList.Add(normals[p2]);
                    tex1List.Add(tex1[p2]);
                    if (tex2 != null)
                    {
                        tex2List.Add(tex2[p2]);
                    }
                    if (tex3 != null)
                    {
                        tex3List.Add(tex3[p2]);
                    }
                    if (tex4 != null)
                    {
                        tex4List.Add(tex4[p2]);
                    }
                }
                else
                {
                    faces[i].IndexC = useState[p2].b;
                }

                faceList.Add(faces[i]);
            }

            if (faceList.Count > 0)
            {
                faces = faceList.ToArray();
                positions = posList.ToArray();
                normals = nList.ToArray();
                tex1 = tex1List.ToArray();

                if (tex2List != null)
                {
                    tex2 = tex2List.ToArray();
                }
                if (tex3List != null)
                {
                    tex3 = tex3List.ToArray();
                }
                if (tex4List != null)
                {
                    tex4 = tex4List.ToArray();
                }

                Update();
            }
            else
            {
                positions = null;
                faces = null;
                normals = null;
                tex1 = null;
                tex2 = null;
                tex3 = null;
                tex4 = null;
            }
        }

        [Obsolete()]
        private void SimplifyOld(int faceCount, int vtxCount, AttributeWeights[] weights)
        {
            if (faceCount < 0 || vtxCount < 0)
            {
                return;
            }

            //int[] adj = Mesh.GenerateAdjacency(float.Epsilon);
            //mesh.SetAdjacency(adj);

            //Mesh cleaned = mesh.Clean(CleanType.Simplification);

            //SimplificationMesh sm = new SimplificationMesh(cleaned, weights);

            //sm.ReduceFaces(faceCount);
            //sm.ReduceVertices(vtxCount);

            ////sm.

            //Mesh newMesh = sm.Clone(device, cleaned.CreationOptions, mesh.VertexFormat);

            //sm.Dispose();
            //mesh.Dispose();
            //cleaned.Dispose();

            //mesh = newMesh;

            MeshSimplifier ms = new MeshSimplifier();
            int[] map;
            int[] permutation;
            ms.ProgressiveMesh(positions, faces, out map, out permutation);



            Vector3[] tempPos = new Vector3[positions.Length];
            Vector3[] tempN = new Vector3[normals.Length];
            Vector2[] tempTex1 = new Vector2[tex1.Length];


            Vector2[] tempTex2 = null;
            if (tex2 != null)
                tempTex2 = new Vector2[tex2.Length];
            Vector2[] tempTex3 = null;
            if (tex3 != null)
                tempTex3 = new Vector2[tex3.Length];
            Vector2[] tempTex4 = null;
            if (tex4 != null)
                tempTex4 = new Vector2[tex4.Length];


            Array.Copy(positions, tempPos, positions.Length);
            Array.Copy(normals, tempN, normals.Length);
            Array.Copy(tex1, tempTex1, tex1.Length);

            for (int i = 0; i < positions.Length; i++)
            {
                positions[permutation[i]] = tempPos[i];
                normals[permutation[i]] = tempN[i];
                tex1[permutation[i]] = tempTex1[i];
                if (tex2 != null)
                    tex2[permutation[i]] = tempTex2[i];

                if (tex3 != null)
                    tex3[permutation[i]] = tempTex3[i];

                if (tex4 != null)
                    tex4[permutation[i]] = tempTex4[i];

            }
            // update the changes in the entries in the triangle list
            for (int i = 0; i < faces.Length; i++)
            {
                faces[i].IndexA = permutation[faces[i].IndexA];
                faces[i].IndexB = permutation[faces[i].IndexA];
                faces[i].IndexC = permutation[faces[i].IndexA];
            }

            Mesh = GameMesh.BuildMeshFromData(device, this);
            //BuildFromMesh(mesh, this, this.materials);
        }
        [Obsolete()]
        private void SimplifyOld(int faceCount, int vtxCount)
        {
            Simplify(faceCount, vtxCount, null);
            //int[] adj = Mesh.GenerateAdjacency(float.Epsilon);
            //mesh.SetAdjacency(adj);

            //Mesh cleaned = mesh.Clean(CleanType.Simplification);

            //SimplificationMesh sm = new SimplificationMesh(cleaned);

            //sm.ReduceFaces(faceCount);
            //sm.ReduceVertices(vtxCount);

            ////sm.

            //Mesh newMesh = sm.Clone(device, cleaned.CreationOptions, mesh.VertexFormat);

            //sm.Dispose();
            //mesh.Dispose();

            //mesh = newMesh;

            //BuildFromMesh(mesh, this, this.materials);
        }

        public SimplificationMesh GetSimplificationMesh()
        {
            Mesh.GenerateAdjacency(float.Epsilon);

            Mesh cleaned = mesh.Clean(CleanType.Simplification);

            SimplificationMesh sm = new SimplificationMesh(cleaned);
            cleaned.Dispose();
            return sm;
        }
        public SimplificationMesh GetSimplificationMesh(AttributeWeights[] weights)
        {
            Mesh.GenerateAdjacency(float.Epsilon);            

            Mesh cleaned = mesh.Clean(CleanType.Simplification);

            SimplificationMesh sm = new SimplificationMesh(cleaned, weights);
            cleaned.Dispose();
            return sm;
        }

        [Obsolete()]
        private void Simplify(int faceCount, int vtxCount, AttributeWeights[] weights)
        {
            if (faceCount < 0 || vtxCount < 0)
            {
                return;
            }

            Mesh.GenerateAdjacency(float.Epsilon);            

            Mesh cleaned = mesh.Clean(CleanType.Simplification);

            SimplificationMesh sm = new SimplificationMesh(cleaned, weights);

            sm.ReduceFaces(faceCount);
            sm.ReduceVertices(vtxCount);

            Mesh newMesh = sm.Clone(device, cleaned.CreationOptions, mesh.VertexFormat);

            sm.Dispose();
            mesh.Dispose();
            cleaned.Dispose();

            Mesh = newMesh;

            BuildFromMesh(mesh, this, this.materials);
        }
        [Obsolete ()]
        private void Simplify(int faceCount, int vtxCount)
        {
            Mesh.GenerateAdjacency(float.Epsilon);

            Mesh cleaned = mesh.Clean(CleanType.Simplification);

            SimplificationMesh sm = new SimplificationMesh(cleaned);

            sm.ReduceFaces(faceCount);
            sm.ReduceVertices(vtxCount);

            //sm.

            Mesh newMesh = sm.Clone(device, cleaned.CreationOptions, mesh.VertexFormat);

            sm.Dispose();

            Mesh = newMesh;

            BuildFromMesh(mesh, this, this.materials);
        }
        public void InverseNormals()
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i].X = -normals[i].X;
                normals[i].Y = -normals[i].Y;
                normals[i].Z = -normals[i].Z;
            }
            Update();
        }
        public void InverseNormalX()
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i].X = -normals[i].X;
            }
            Update();
        }
        public void InverseNormalY()
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i].Y = -normals[i].Y;
            }
            Update();
        }
        public void InverseNormalZ()
        {
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i].Z = -normals[i].Z;
            }
            Update();
        }
        public bool Intersects(Vector3 start, Vector3 end, out float dist)
        {
            return Mesh.Intersects(new Ray(start, end), out dist);
        }


        public string GetVertexDescription(int index, bool pos, bool n, bool uv1, bool uv2, bool uv3, bool uv4)
        {
            StringBuilder descSB = new StringBuilder(12);

            if (pos)
            {
                descSB.Append(positions[index].ToString());
                descSB.Append(' ');
            }

            if (n && normals != null)
            {
                descSB.Append(normals[index].ToString());
                descSB.Append(' ');
            }

            if (uv1 && tex1 != null)
            {
                descSB.Append(tex1[index].ToString());
                descSB.Append(' ');
            }
            if (uv2 && tex2 != null)
            {
                descSB.Append(tex2[index].ToString());
                descSB.Append(' ');
            }
            if (uv3 && tex3 != null)
            {
                descSB.Append(tex3[index].ToString());
                descSB.Append(' ');
            }
            if (uv4 && tex4 != null)
            {
                descSB.Append(tex4[index].ToString());
            }

            return descSB.ToString();
        }
        public string GetVertexDescription(int index, ref Vector3 norm, bool pos, bool uv1, bool uv2, bool uv3, bool uv4)
        {
            StringBuilder descSB = new StringBuilder(12);

            if (pos)
            {
                descSB.Append(positions[index].ToString());
                descSB.Append(' ');
            }

            
            descSB.Append(norm.ToString());
            descSB.Append(' ');

            if (uv1 && tex1 != null)
            {
                descSB.Append(tex1[index].ToString());
                descSB.Append(' ');
            }
            if (uv2 && tex2 != null)
            {
                descSB.Append(tex2[index].ToString());
                descSB.Append(' ');
            }
            if (uv3 && tex3 != null)
            {
                descSB.Append(tex3[index].ToString());
                descSB.Append(' ');
            }
            if (uv4 && tex4 != null)
            {
                descSB.Append(tex4[index].ToString());
            }

            return descSB.ToString();
        }

        public string GetVertexDescription(ref Vector3 pos, ref Vector3 n, ref Vector2 tex1, ref Vector2 tex2, ref Vector2 tex3, ref Vector2 tex4, MeshVertexElement elem)
        {
            StringBuilder descSB = new StringBuilder(12);

            if ((elem & MeshVertexElement.Position) !=0)
            {
                descSB.Append(pos.ToString());
                descSB.Append(' ');
            }

            if ((elem & MeshVertexElement.Normal) != 0)
            {
                descSB.Append(n.ToString());
                descSB.Append(' ');
            }

            if ((elem & MeshVertexElement.Tex1) != 0)
            {
                descSB.Append(tex1.ToString());
                descSB.Append(' ');
            }
            if ((elem & MeshVertexElement.Tex2) != 0)
            {
                descSB.Append(tex2.ToString());
                descSB.Append(' ');
            }
            if ((elem & MeshVertexElement.Tex3) != 0)
            {
                descSB.Append(tex3.ToString());
                descSB.Append(' ');
            }
            if ((elem & MeshVertexElement.Tex4) != 0)
            {
                descSB.Append(tex4.ToString());
            }
            return descSB.ToString();
        }
        public string GetVertexDescription(ref Vector3 pos, ref Vector3 n, ref Vector2 tex1)
        {
            StringBuilder sb = new StringBuilder(7);

            sb.Append(pos.ToString());
            sb.Append(' ');
            sb.Append(n.ToString());
            sb.Append(' ');
            sb.Append(tex1.ToString());

            return sb.ToString();
        }
        public void WeldVertices(bool pos, bool n, bool uv1)
        {
            MeshVertexElement flag = MeshVertexElement.None;
            if (pos)
                flag |= MeshVertexElement.Position;
            if (n)
                flag |= MeshVertexElement.Normal;
            if (uv1)
                flag |= MeshVertexElement.Tex1;
            WeldVertices(flag);
        }
        public void WeldVertices(MeshVertexElement elem)
        {
            bool pos = (elem & MeshVertexElement.Position) !=0;
            bool n = (elem & MeshVertexElement.Normal) != 0;
            bool uv1 = (elem & MeshVertexElement.Tex1) != 0;
            bool uv2 = (elem & MeshVertexElement.Tex2) != 0;
            bool uv3 = (elem & MeshVertexElement.Tex3) != 0;
            bool uv4 = (elem & MeshVertexElement.Tex4) != 0;

            Dictionary<string, int> table = new Dictionary<string, int>(positions.Length);

            // vertices[i]表示新顶点数据中，顶点i在旧定点数据中的索引
            List<int> vertices = new List<int>(positions.Length);

            for (int i = 0; i < faces.Length; i++)
            {
                int index;

                string desc = GetVertexDescription(faces[i].IndexA, pos, n, uv1, uv2, uv3, uv4);
                if (!table.TryGetValue(desc, out index))
                {
                    index = vertices.Count;

                    table.Add(desc, index);
                    vertices.Add(faces[i].IndexA);
                }
                faces[i].IndexA = index;


                desc = GetVertexDescription(faces[i].IndexB, pos, n, uv1, uv2, uv3, uv4);
                if (!table.TryGetValue(desc, out index))
                {
                    index = vertices.Count;

                    table.Add(desc, index);
                    vertices.Add(faces[i].IndexB);
                }
                faces[i].IndexB = index;


                desc = GetVertexDescription(faces[i].IndexC, pos, n, uv1, uv2, uv3, uv4);
                if (!table.TryGetValue(desc, out index))
                {
                    index = vertices.Count;

                    table.Add(desc, index);
                    vertices.Add(faces[i].IndexC);
                }
                faces[i].IndexC = index;
            }

            Vector3[] newPos = new Vector3[vertices.Count];
            Vector3[] newN = null;
            if (normals != null)
                newN = new Vector3[vertices.Count];

            Vector2[] newTex1 = null;
            if (tex1 != null)
                newTex1 = new Vector2[vertices.Count];

            Vector2[] newTex2 = null;
            if (tex2 != null)
                newTex2 = new Vector2[vertices.Count];

            Vector2[] newTex3 = null;
            if (tex3 != null)
                newTex3 = new Vector2[vertices.Count];

            Vector2[] newTex4 = null;
            if (tex4 != null)
                newTex4 = new Vector2[vertices.Count];

            for (int i = 0; i < vertices.Count; i++)
            {
                newPos[i] = positions[vertices[i]];

                if (normals != null)
                {
                    newN[i] = normals[vertices[i]];
                }
                if (tex1 != null)
                {
                    newTex1[i] = tex1[vertices[i]];
                }
                if (tex2 != null)
                {
                    newTex2[i] = tex2[vertices[i]];
                }
                if (tex3 != null)
                {
                    newTex3[i] = tex3[vertices[i]];
                }
                if (tex4 != null)                
                {
                    newTex4[i] = tex4[vertices[i]];
                }
            }
            positions = newPos;
            normals = newN;
            tex1 = newTex1;
            tex2 = newTex2;
            tex3 = newTex3;
            tex4 = newTex4;
        }

        bool RequiresUpdate()
        {
            if (IsReleased(mesh))
                return true;

            return mesh.VertexFormat != base.Format || mesh.VertexCount != positions.Length || mesh.FaceCount != faces.Length;
        }
        void Update()
        {
            if (!IsReleased(mesh))
            {
                mesh.Dispose();
            }
            mesh = GameMesh.BuildMeshFromData<EditableMeshMaterial>(device, this);
        }

        public void Render()
        {
            if (positions != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    DevUtils.SetMaterial(device, materials[i]);
                    Mesh.DrawSubset(i);
                }
            }
        }

        public void ExportAsObj(string file)
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write);
            fs.SetLength(0);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);


            sw.WriteLine('g');

            for (int i = 0; i < positions.Length; i++)
            {
                sw.Write("v ");
                sw.Write(positions[i].X.ToString());
                sw.Write(' ');
                sw.Write(positions[i].Y.ToString());
                sw.Write(' ');
                sw.WriteLine(positions[i].Z.ToString());

            }

            for (int i = 0; i < tex1.Length; i++)
            {
                sw.Write("vt ");
                sw.Write(tex1[i].X.ToString());
                sw.Write(' ');
                sw.Write(tex1[i].Y.ToString());
                sw.WriteLine(" 0.0");
            }

            for (int i = 0; i < normals.Length; i++)
            {
                sw.Write("vn ");
                sw.Write(normals[i].X.ToString());
                sw.Write(' ');
                sw.Write(normals[i].Y.ToString());
                sw.Write(' ');
                sw.WriteLine(normals[i].Z.ToString());
            }
            sw.WriteLine('g');
            for (int i = 0; i < faces.Length; i++)
            {
                int a = faces[i].IndexA + 1;
                int b = faces[i].IndexB + 1;
                int c = faces[i].IndexC + 1;

                string sa = a.ToString();
                string sb = b.ToString();
                string sc = c.ToString();


                sw.Write("f ");
                sw.Write(sa + "/" + sa + "/" + sa);
                sw.Write(' ');
                sw.Write(sb + "/" + sb + "/" + sb);
                sw.Write(' ');
                sw.WriteLine(sc + "/" + sc + "/" + sc);
            }
            sw.WriteLine('g');
            sw.Close();
        }
        //public static EditableMesh ImportFromXml(string file)
        //{
        //    Xml2ModelConverter conv = new Xml2ModelConverter();
        //    System.IO.MemoryStream ms = new System.IO.MemoryStream(65536);
        //    conv.Convert(new FileLocation(file), new StreamedLocation(new VirtualStream(ms, 0)));

        //    ms.Position = 0;


        //    EditableModel sounds = EditableModel.FromFile(new StreamedLocation(ms));// new EditableModel();                

        //    //content.Texture1 = sounds.Entities[0].Texture1;
        //    //content.Positions = sounds.Entities[0].Positions;
        //    //content.Normals = sounds.Entities[0].Normals;
        //    //content.Materials = sounds.Entities[0].Materials;
        //    //content.Faces = sounds.Entities[0].Faces;
        //    //content.Format = sounds.Entities[0].Format;

        //    return sounds.Entities[0];
        //}

        public EditableMesh Clone()
        {
            EditableMesh copy = new EditableMesh();
            copy.device = device;
            copy.faces = new MeshFace[faces.Length];
            Array.Copy(faces, copy.faces, faces.Length);

            copy.materials = new EditableMeshMaterial[materials.Length];
            Array.Copy(materials, copy.materials, materials.Length);

            copy.normals = new Vector3[normals.Length];
            Array.Copy(normals, copy.normals, normals.Length);

            copy.positions = new Vector3[positions.Length];
            Array.Copy(positions, copy.positions, positions.Length);

            copy.tex1 = new Vector2[tex1.Length];
            Array.Copy(tex1, copy.tex1, tex1.Length);

            copy.SetVertexFormat(Format);

            if (tex2 != null)
            {
                copy.tex2 = new Vector2[tex2.Length];
                Array.Copy(tex2, copy.tex2, tex2.Length);
            }
            if (tex3 != null)
            {
                copy.tex3 = new Vector2[tex3.Length];
                Array.Copy(tex3, copy.tex3, tex3.Length);
            }
            if (tex4 != null)
            {
                copy.tex4 = new Vector2[tex4.Length];
                Array.Copy(tex4, copy.tex4, tex4.Length);
            }
            copy.Name = Name;

            return copy;
        }
        public void CloneTo(EditableMesh m)
        {
            m.faces = new MeshFace[faces.Length];
            Array.Copy(faces, m.faces, faces.Length);

            m.materials = new EditableMeshMaterial[materials.Length];
            Array.Copy(materials, m.materials, materials.Length);

            m.normals = new Vector3[normals.Length];
            Array.Copy(normals, m.normals, normals.Length);

            m.positions = new Vector3[positions.Length];
            Array.Copy(positions, m.positions, positions.Length);

            m.tex1 = new Vector2[tex1.Length];
            Array.Copy(tex1, m.tex1, tex1.Length);

            m.SetVertexFormat(Format);

            if (tex2 != null)
            {
                m.tex2 = new Vector2[tex2.Length];
                Array.Copy(tex2, m.tex2, tex2.Length);
            }
            if (tex3 != null)
            {
                m.tex3 = new Vector2[tex3.Length];
                Array.Copy(tex3, m.tex3, tex3.Length);
            }
            if (tex4 != null)
            {
                m.tex4 = new Vector2[tex4.Length];
                Array.Copy(tex4, m.tex4, tex4.Length);
            }
            m.Name = Name;           
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (disposed)
            {
                if (!IsReleased(mesh))
                {
                    mesh.Dispose();
                    mesh = null;
                }
                positions = null;
                normals = null;
                tex1 = null;
                tex2 = null;
                tex3 = null;
                tex4 = null;

                if (materials != null)
                {
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i].Dispose();
                        materials[i] = null;
                    }
                    materials = null;
                }
                disposed = true;
            }
        }

        #endregion

        ~EditableMesh()
        {
            if (!disposed)
                Dispose();
        }

        public void Dispose(bool p)
        {
            if (!disposed)
            {
                if (!IsReleased(mesh))
                {
                    mesh.Dispose();
                    mesh = null;
                }
                positions = null;
                normals = null;
                tex1 = null;
                tex2 = null;
                tex3 = null;
                tex4 = null;
                if (p)
                {
                    if (materials != null)
                    {
                        for (int i = 0; i < materials.Length; i++)
                        {
                            materials[i].Dispose();
                            materials[i] = null;
                        }
                        materials = null;
                    }
                }
                disposed = true;
            }

        }

        public override string ToString()
        {
            return string.Format(Program.StringTable["GUI:MeshDesc"], Name);
        }
    }
}
