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
using System.Windows.Forms;
using Ra2Develop.Editors.EditableObjects;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.MathLib;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Develop.Converters
{
    [Obsolete ()]
    class Obj2ModelConverter : ConverterBase
    {
        class ObjFace
        {
            public bool bHasNormal;
            public Vector3 CacNormal;

            public string sAssignedMat;
            public int iAssignedMatIdx;

            public int iNormalA, iNormalB, iNormalC;

            public int iVertexA, iVertexB, iVertexC;

            public int iTexCoordA, iTexCoordB, iTexCoordC;

            public ObjFace()
            {
                iTexCoordA = 1; iTexCoordB = 1; iTexCoordC = 1;
            }
        }
        class ObjMaterial : EditableMeshMaterial
        {
            public ObjMaterial()
            { }

            public string sName;
            //public string sTextureFile;
            public Vector3 vTiling;
            public bool bHasTexture;

            public List<int> iDervedFaces;

        }
        enum FaceVertex
        {
            A, B, C
        }

        sealed class ObjMesh
        {

            const string VertexTag = "v";
            const string TexCoordTag = "vt";
            const string NormalTag = "vn";
            const string GruopTag = "g";
            const string FaceTag = "f";
            const string MaterialTag = "usemtl";
            const string MtlTag = "mtllib";
            const string Comment = "#";

            const string Group = "g";


            const string NewMaterialTag = "newmtl";
            const string AmbientTag = "ka";
            const string DiffuseTag = "kd";
            const string SpecularTag = "ks";
            //const string IllumTag = "illum";
            const string ShininessTag = "ns";
            const string MapKaTag = "map_ka";
            const string MapKdTag = "map_kd";
            const string MapKsTag = "map_ks";
            const string MapBumpTag = "map_bump";
            //const string DissolveTag = "d";
            //const string RefractionTag = "Ni";


            List<Vector3> vVertex = new List<Vector3>();
            List<Vector3> vNormal = new List<Vector3>();
            List<Vector2> vTexCoords = new List<Vector2>();
            List<ObjFace> fFaces = new List<ObjFace>();
            List<ObjMaterial> mMaterial = new List<ObjMaterial>();
            List<int> iUngroupedMatFace = new List<int>();

            string name;

            string sFilePath;
            string sCurMaterial = "";

            public ObjMesh(string name, string path)
            {
                this.name = name;
                sFilePath = path;
            }

            public void ReadLine(string[] arg)
            {
                string pf = arg[0];
                switch (pf)
                {
                    case VertexTag:
                        vVertex.Add(new Vector3(float.Parse(arg[1]),
                                                float.Parse(arg[2]),
                                                float.Parse(arg[3])));
                        break;
                    case TexCoordTag:
                        vTexCoords.Add(new Vector2(float.Parse(arg[1]),
                                                   float.Parse(arg[2])));
                        break;
                    case NormalTag:
                        Vector3 n = new Vector3(float.Parse(arg[1]),
                                                float.Parse(arg[2]),
                                                float.Parse(arg[3]));
                        n.Normalize();
                        vNormal.Add(n);

                        break;
                    case FaceTag:
                        fFaces.Add(new ObjFace());
                        int c = fFaces.Count - 1;
                        AddFace(arg[1], c, FaceVertex.A);
                        AddFace(arg[2], c, FaceVertex.B);
                        AddFace(arg[3], c, FaceVertex.C);
                        fFaces[c].sAssignedMat = sCurMaterial;// a[3];

                        break;
                    case MtlTag:
                        LoadMtl(Path.Combine(sFilePath, arg[1]));
                        break;
                    case MaterialTag:
                        sCurMaterial = arg[1];
                        break;

                }
            }
            private void AddFace(string part, int faceID, FaceVertex v)
            {
                int[] numArray = new int[3];
                string[] textArray = part.Split('/');
                int upperBound = textArray.GetUpperBound(0);
                for (int i = 0; i <= upperBound; i++)
                {
                    if (textArray[i].Length > 0)
                    {
                        numArray[i] = int.Parse(textArray[i]);
                    }
                }
                if (!part.Contains("//"))
                {
                    if (textArray.GetUpperBound(0) == 1)
                    {
                        fFaces[faceID].bHasNormal = false;
                        switch (v)
                        {
                            case FaceVertex.A:
                                fFaces[faceID].iVertexA = numArray[0];
                                fFaces[faceID].iTexCoordA = numArray[1];
                                return;

                            case FaceVertex.B:
                                fFaces[faceID].iVertexB = numArray[0];
                                fFaces[faceID].iTexCoordB = numArray[1];
                                return;

                            case FaceVertex.C:
                                fFaces[faceID].iVertexC = numArray[0];
                                fFaces[faceID].iTexCoordC = numArray[1];
                                return;
                        }
                        return;
                    }
                    if (!part.Contains("/"))
                    {
                        switch (v)
                        {
                            case FaceVertex.A:
                                fFaces[faceID].iVertexA = numArray[0];
                                fFaces[faceID].bHasNormal = false;
                                return;

                            case FaceVertex.B:
                                fFaces[faceID].iVertexB = numArray[0];
                                fFaces[faceID].bHasNormal = false;
                                return;

                            case FaceVertex.C:
                                fFaces[faceID].iVertexC = numArray[0];
                                fFaces[faceID].bHasNormal = false;
                                return;
                        }
                        return;
                    }

                    switch (v)
                    {
                        case FaceVertex.A:
                            fFaces[faceID].iVertexA = numArray[0];
                            fFaces[faceID].iTexCoordA = numArray[1];
                            fFaces[faceID].iNormalA = numArray[2];
                            fFaces[faceID].bHasNormal = true;
                            return;

                        case FaceVertex.B:
                            fFaces[faceID].iVertexB = numArray[0];
                            fFaces[faceID].iTexCoordB = numArray[1];
                            fFaces[faceID].iNormalB = numArray[2];
                            fFaces[faceID].bHasNormal = true;
                            return;

                        case FaceVertex.C:
                            fFaces[faceID].iVertexC = numArray[0];
                            fFaces[faceID].iTexCoordC = numArray[1];
                            fFaces[faceID].iNormalC = numArray[2];
                            fFaces[faceID].bHasNormal = true;
                            return;
                    }
                }
                else
                {
                    fFaces[faceID].bHasNormal = true;
                    switch (v)
                    {
                        case FaceVertex.A:
                            fFaces[faceID].iVertexA = numArray[0];
                            fFaces[faceID].iNormalA = numArray[2];
                            break;

                        case FaceVertex.B:
                            fFaces[faceID].iVertexB = numArray[0];
                            fFaces[faceID].iNormalB = numArray[2];
                            break;

                        case FaceVertex.C:
                            fFaces[faceID].iVertexC = numArray[0];
                            fFaces[faceID].iNormalC = numArray[2];
                            break;
                    }
                    return;
                }
            }

            private void LoadMtl(string file)
            {
                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader sr = new StreamReader(file, Encoding.Default);

                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine().Trim();
                    if (line.Length > 0)
                    {
                        ReadMtlLine(GetObjLineArg(line));
                    }
                }
                sr.Close();
                sr.Dispose();
            }
            private void ReadMtlLine(string[] args)
            {
                string pf = args[0];
                int c = mMaterial.Count - 1;

                switch (pf)
                {
                    case NewMaterialTag:
                        mMaterial.Add(new ObjMaterial());
                        mMaterial[mMaterial.Count - 1].sName = args[1];
                        break;
                    case AmbientTag:
                        mMaterial[c].mat.Ambient = new Color4(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        break;
                    case DiffuseTag:
                        mMaterial[c].mat.Diffuse = new Color4(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        break;
                    case SpecularTag:
                        mMaterial[c].mat.Specular = new Color4(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]));
                        break;
                    case ShininessTag:
                        float s = float.Parse(args[1]);
                        mMaterial[c].mat.Power = s * 1.2812812812812812812812812812813f;
                        break;
                    case MapKdTag:
                        if (args[1] == "-s")
                        {
                            mMaterial[c].vTiling = new Vector3(float.Parse(args[2]),
                                                       float.Parse(args[3]),
                                                       float.Parse(args[4]));
                            mMaterial[c].TextureFile1 = args[5];
                            mMaterial[c].bHasTexture = true;
                        }
                        else
                        {
                            mMaterial[c].vTiling = new Vector3(1, 1, 1);
                            mMaterial[c].TextureFile1 = args[1];
                            mMaterial[c].bHasTexture = true;
                        }

                        break;
                    case MapKsTag:
                        break;
                }

            }

            public VertexFormat Format
            {
                get { return VertexPNT1.Format; }
            }

            public string Name
            {
                get { return name; }
            }
            public List<Vector3> Positions
            {
                get { return vVertex; }
            }
            public List<Vector3> Normals
            {
                get { return vNormal; }
            }
            public List<Vector2> TextureCoords
            {
                get { return vTexCoords; }
            }
            public List<ObjFace> Faces
            {
                get { return fFaces; }
            }
            public List<ObjMaterial> Materials
            {
                get { return mMaterial; }
            }
            public List<int> UngroupedFaces
            {
                get { return iUngroupedMatFace; }
            }

            public void PostProcess(bool flatNormals)
            {
                int iMatCount = mMaterial.Count;


                for (int i = 0; i < fFaces.Count; i++)
                {
                    if (!flatNormals)
                    {
                        if (!fFaces[i].bHasNormal)
                        {
                            fFaces[i].CacNormal = MathEx.ComputePlaneNormal(vVertex[fFaces[i].iVertexA - 1], vVertex[fFaces[i].iVertexB - 1], vVertex[fFaces[i].iVertexC - 1]);
                        }
                    }
                    else
                    {
                        fFaces[i].CacNormal = MathEx.ComputePlaneNormal(vVertex[fFaces[i].iVertexA - 1], vVertex[fFaces[i].iVertexB - 1], vVertex[fFaces[i].iVertexC - 1]);
                        fFaces[i].bHasNormal = false;

                    }
                    if (fFaces[i].sAssignedMat.Length > 0)
                    {
                        for (int j = 0; j < iMatCount; j++)
                        {
                            if (fFaces[i].sAssignedMat == mMaterial[j].sName)
                            {
                                fFaces[i].iAssignedMatIdx = j;
                                if (mMaterial[j].iDervedFaces == null)
                                    mMaterial[j].iDervedFaces = new List<int>();
                                mMaterial[j].iDervedFaces.Add(i);
                                break;
                            }
                        }
                    }
                    else
                    {
                        iUngroupedMatFace.Add(i);
                    }
                }
            }

        }
        sealed class ObjReader
        {
            List<ObjMesh> meshes = new List<ObjMesh>();

            string sFile;
            string sFilePath;

            bool flatNormals;

            public ObjReader(ResourceLocation rl)
            {
                FileLocation fl = rl as FileLocation;
                if (fl != null)
                {
                    sFile = fl.Path;
                }
                else
                {
                    throw new NotSupportedException(rl.GetType().ToString());
                }
            }

            public void Load()
            {
                sFilePath = Path.GetDirectoryName(sFile);
                StreamReader sr = new StreamReader(sFile, Encoding.Default);

                ObjMesh commonData = new ObjMesh(string.Empty, sFilePath);
                meshes.Add(commonData);
                //ObjMesh commonData = new ObjMesh("", sFilePath);

                ObjMesh objMesh = null;
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine().Trim();
                    //commonData.ReadLine(GetObjLineArg(line));

                    if (line.Length > 0)
                    {
                        string[] args = GetObjLineArg(line);

                        if (args[0] == "g")
                        {
                            string meshName;
                            if (args.Length > 1)
                            {
                                meshName = args[1];
                            }
                            else
                            {
                                meshName = string.Empty;
                            }

                            bool found = false;
                            for (int i = 0; i < meshes.Count; i++)
                            {
                                if (meshName == meshes[i].Name)
                                {
                                    objMesh = meshes[i];
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                            {
                                objMesh = new ObjMesh(meshName, sFilePath);
                                objMesh.Materials.AddRange(commonData.Materials);
                                meshes.Add(objMesh);
                            }
                        }
                        else
                        {
                            if (objMesh == null)
                            {
                                objMesh = commonData;
                            }
                            objMesh.ReadLine(args);
                        }
                    }
                }

                for (int i = 0; i < meshes.Count; i++)
                {
                    if (meshes[i] != commonData)
                    {
                        if (meshes[i].Positions.Count == 0)
                        {
                            meshes[i].Positions.AddRange(commonData.Positions);
                        }
                        if (meshes[i].Normals.Count == 0)
                        {
                            meshes[i].Normals.AddRange(commonData.Normals);
                        }
                        if (meshes[i].TextureCoords.Count == 0)
                        {
                            meshes[i].TextureCoords.AddRange(commonData.TextureCoords);
                        }
                        meshes[i].PostProcess(flatNormals);
                    }
                }
                sr.Close();
                sr.Dispose();
            }

            public int EntityCount
            {
                get { return meshes.Count; }
            }
            public List<ObjMesh> Entities
            {
                get { return meshes; }
            }
            public bool FlatNormals
            {
                get { return flatNormals; }
                set { flatNormals = value; }
            }
        }
        //sealed class SortedFaceVertex
        //{
        //    public ObjFace face;
        //    public FaceVertex vtx;
        //    public string vtxDesc;

        //}

        bool flatNormals = true;



        public bool FlatNormals
        {
            get { return flatNormals; }
            set { flatNormals = value; }
        }

        static string[] GetObjLineArg(string line)
        {
            return line.ToLower().Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
        }

        public override void ShowDialog(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        //string GetFaceDesc(ObjFace face, FaceVertex vtx)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (vtx == FaceVertex.A)
        //    {
        //        sb.Append(face.iVertexA.ToString());
        //        sb.Append('-');
        //        sb.Append(face.iTexCoordA.ToString());
        //        if (face.bHasNormal)
        //        {
        //            sb.Append('-');
        //            sb.Append(face.iNormalA);
        //        }
        //        else
        //        {
        //            sb.Append('-');
        //            sb.Append(face.CacNormal.GetHashCode());
        //        }
        //    }
        //    else if (vtx == FaceVertex.B)
        //    {
        //        sb.Append(face.iVertexB.ToString());
        //        sb.Append('-');
        //        sb.Append(face.iTexCoordB.ToString());
        //        if (face.bHasNormal)
        //        {
        //            sb.Append('-');
        //            sb.Append(face.iNormalB);
        //        }
        //        else
        //        {
        //            sb.Append('-');
        //            sb.Append(face.CacNormal.GetHashCode());
        //        }
        //    }
        //    else
        //    {
        //        sb.Append(face.iVertexC.ToString());
        //        sb.Append('-');
        //        sb.Append(face.iTexCoordC.ToString());
        //        if (face.bHasNormal)
        //        {
        //            sb.Append('-');
        //            sb.Append(face.iNormalC);
        //        }
        //        else
        //        {
        //            sb.Append('-');
        //            sb.Append(face.CacNormal.GetHashCode());
        //        }
        //    }
        //    return sb.ToString();
        //}

        //int FaceVertexComparison(SortedFaceVertex a, SortedFaceVertex b)
        //{
        //    return a.vtxDesc.CompareTo(b.vtxDesc);
        //}

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            ObjReader obj = new ObjReader(source);
            obj.FlatNormals = flatNormals;
            obj.Load();

            List<ObjMesh> meshes = obj.Entities;

            EditableModel model = new EditableModel();
            model.Entities = new EditableMesh[meshes.Count];

            int i = 0;
            for (int k = 0; k < meshes.Count; k++)
            {
                ObjMesh mesh = meshes[i];
                if (mesh.Name.Length > 0)
                {
                    List<ObjFace> objFaces = mesh.Faces;

                    Vector3[] positions = new Vector3[objFaces.Count * 3];
                    Vector3[] normals = new Vector3[objFaces.Count * 3];
                    Vector2[] texCoords = new Vector2[objFaces.Count * 3];
                    MeshFace[] faces = new MeshFace[objFaces.Count];

                    ObjMaterial[] mats = mesh.Materials.ToArray();

                    for (int j = 0; j < objFaces.Count; j++)
                    {
                        ObjFace face = objFaces[j];
                        int j3 = j * 3;

                        int matIdx = face.iAssignedMatIdx;
                        faces[j] = new MeshFace(j3, j3 + 1, j3 + 2);
                        faces[j].MaterialIndex = matIdx;

                        positions[j3] = mesh.Positions[face.iVertexA - 1];
                        positions[j3 + 1] = mesh.Positions[face.iVertexB - 1];
                        positions[j3 + 2] = mesh.Positions[face.iVertexC - 1];

                        if (face.bHasNormal)
                        {
                            normals[j3] = mesh.Normals[face.iNormalA - 1];
                            normals[j3 + 1] = mesh.Normals[face.iNormalB - 1];
                            normals[j3 + 2] = mesh.Normals[face.iNormalC - 1];
                        }
                        else
                        {
                            normals[j3] = face.CacNormal;
                            normals[j3 + 1] = face.CacNormal;
                            normals[j3 + 2] = face.CacNormal;
                        }

                        Vector3 tilling = mats[matIdx].vTiling;

                        Vector2 texCoord = mesh.TextureCoords[face.iTexCoordA - 1];
                        texCoord.X *= tilling.X;
                        texCoord.Y *= tilling.Y;

                        texCoords[j3] = texCoord;

                        texCoord = mesh.TextureCoords[face.iTexCoordB - 1];
                        texCoord.X *= tilling.X;
                        texCoord.Y *= tilling.Y;
                        texCoords[j3 + 1] = texCoord;

                        texCoord = mesh.TextureCoords[face.iTexCoordC - 1];
                        texCoord.X *= tilling.X;
                        texCoord.Y *= tilling.Y;
                        texCoords[j3 + 2] = texCoord;

                    }

                    EditableMesh data = new EditableMesh();
                    data.Positions = positions;
                    data.Faces = faces;
                    data.Normals = normals;
                    data.Texture1 = texCoords;
                    data.Materials = mesh.Materials.ToArray();
                    data.SetVertexFormat(VertexPNT1.Format);

                    model.Entities[i++] = data;
                }
            }
            //for (int i = 0; i < meshes.Count; i++)
            //{
            //    ObjMesh mesh = meshes[i];
            //    if (mesh.Name.Length > 0)
            //    {
            //        //列出所有顶点并排序
            //        List<SortedFaceVertex> faceVtx = new List<SortedFaceVertex>();
            //        for (int j = 0; j < mesh.Faces.Count; j++)
            //        {
            //            ObjFace face = mesh.Faces[j];

            //            SortedFaceVertex vtx;
            //            vtx = new SortedFaceVertex();
            //            vtx.vtxDesc = GetFaceDesc(face, FaceVertex.A);
            //            vtx.vtx = FaceVertex.A;
            //            vtx.face = face;
            //            faceVtx.Add(vtx);

            //            vtx = new SortedFaceVertex();
            //            vtx.vtxDesc = GetFaceDesc(face, FaceVertex.B);
            //            vtx.vtx = FaceVertex.B;
            //            vtx.face = face;
            //            faceVtx.Add(vtx);

            //            vtx = new SortedFaceVertex();
            //            vtx.vtxDesc = GetFaceDesc(face, FaceVertex.C);
            //            vtx.vtx = FaceVertex.C;
            //            vtx.face = face;
            //            faceVtx.Add(vtx);
            //        }
            //        faceVtx.Sort(FaceVertexComparison);

            //        List<SortedFaceVertex> cleaned = new List<SortedFaceVertex>();
            ////剔除相同顶点
            //        SortedFaceVertex lastItem = null;
            //        for (int j = 0; j < faceVtx.Count; j++)
            //        {
            //            if (lastItem == null)
            //            {
            //                lastItem = faceVtx[j];
            //            }
            //            else
            //            {
            //                if (faceVtx[j].vtxDesc != lastItem.vtxDesc)
            //                {
            //                    cleaned.Add(lastItem);
            //                    lastItem = faceVtx[j];
            //                }
            //            }
            //        }
            //        cleaned.Add(lastItem);

            //        Vector3[] pos = new Vector3[cleaned.Count];
            //        Vector3[] n = new Vector3[cleaned.Count];
            //        Vector2[] texc = new Vector2[cleaned.Count];
            //        for (int j = 0; j < cleaned.Count; j++)
            //        {
            //            ObjFace face = cleaned[j].face;
            //            if (cleaned[j].vtx == FaceVertex.A)
            //            {
            //                pos[j] = mesh.Positions[face.iVertexA - 1];
            //                if (face.bHasNormal)
            //                    n[j] = mesh.Normals[face.iNormalA - 1];
            //                else
            //                    n[j] = face.CacNormal;

            //                texc[j] = mesh.TextureCoords[face.iTexCoordA - 1];

            //                face.iVertexA = j;
            //            }
            //            else if (cleaned[j].vtx == FaceVertex.B)
            //            {
            //                pos[j] = mesh.Positions[face.iVertexB - 1];
            //                if (face.bHasNormal)
            //                    n[j] = mesh.Normals[face.iNormalB - 1];
            //                else
            //                    n[j] = face.CacNormal;
            //                texc[j] = mesh.TextureCoords[face.iTexCoordB - 1];

            //                face.iVertexB = j;
            //            }
            //            else
            //            {
            //                pos[j] = mesh.Positions[face.iVertexC - 1];
            //                if (face.bHasNormal)
            //                    n[j] = mesh.Normals[face.iNormalC - 1];
            //                else
            //                    n[j] = face.CacNormal;
            //                texc[j] = mesh.TextureCoords[face.iTexCoordC - 1];

            //                face.iVertexC = j;
            //            }

            //        }

            //        MeshFace[] faces = new MeshFace[mesh.Faces.Count];
            //        for (int j = 0; j < mesh.Faces.Count; j++)
            //        {
            //            faces[j].IndexA = mesh.Faces[j].iVertexA;
            //            faces[j].IndexB = mesh.Faces[j].iVertexB;
            //            faces[j].IndexC = mesh.Faces[j].iVertexC;
            //            faces[j].MaterialIndex = mesh.Faces[j].iAssignedMatIdx;
            //        }

            //        R3D.Graphics.MeshMaterial.Data[] mats = mesh.Materials.ToArray();

            //        GameMesh.Data sounds = new GameMesh.Data();
            //        sounds.Positions = pos;
            //        sounds.Faces = faces;
            //        sounds.Normals = n;
            //        sounds.Texture1 = texc;
            //        sounds.Materials = mats;
            //        sounds.Format = StdMeshVertex.Format;
            //        model.Entities.Add(sounds);
            //    }
            //}

            ArchiveBinaryWriter bw = new ArchiveBinaryWriter(dest);
            bw.Write(EditableModel.ToBinary(model));
            bw.Close();
        }

        public override string Name
        {
            get { return Program.StringTable["GUI:OBJ2MESH"]; }
        }
        public override string[] SourceExt
        {
            get { return new string[] { ".obj" }; }
        }
        public override string[] DestExt
        {
            get { return new string[] { ".mesh" }; }
        }

        public override string SourceDesc
        {
            get { throw new NotImplementedException(); }
        }

        public override string DestDesc
        {
            get { throw new NotImplementedException(); }
        }
    }
}
