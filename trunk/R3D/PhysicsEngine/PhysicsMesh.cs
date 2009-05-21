using System;
using System.Collections.Generic;
using System.Text;
using R3D.Collections;
using R3D.GraphicsEngine;
using R3D.MathLib;
using R3D.PhysicsEngine.CollisionModel;
using SlimDX;

namespace R3D.PhysicsEngine
{
    public class PhysicsMesh
    {
        ICollisionTree cdTree;

        Vector3[] positions;
        MeshFace[] faces;
        //FastList<TreeFace> trFaces;

        public PhysicsMesh(MeshFace[] faces, Vector3[] vtxPos, CollisionTreeType cdTreeType)
        {
            positions = vtxPos;
            this.faces = faces;


            int faceCount = faces.Length;

            FastList<TreeFace> trFaces = new FastList<TreeFace>(faces.Length);// new TreeFace[faces.Length];

            for (int i = 0; i < faceCount; i++)
            {
                trFaces.Add(
                    new TreeFace(i, vtxPos[faces[i].IndexA],
                        vtxPos[faces[i].IndexB],
                        vtxPos[faces[i].IndexC])
                    );
            }

//#warning 未决定：三角形邻接图由谁创建。临时PhysMesh。pushed
            //相关：顶点邻接图（用于 破裂、质点弹簧柔体）

            //for (int i = 0; i < faceCount; i++)
            //    for (int j = i + 1; j < faceCount; j++)
            //    {
            //        Edge aAB = new Edge(fFaces[i].iVertexA, fFaces[i].iVertexB);
            //        Edge aBC = new Edge(fFaces[i].iVertexB, fFaces[i].iVertexC);
            //        Edge aCA = new Edge(fFaces[i].iVertexC, fFaces[i].iVertexA);

            //        Edge bAB = new Edge(fFaces[j].iVertexA, fFaces[j].iVertexB);
            //        Edge bBC = new Edge(fFaces[j].iVertexB, fFaces[j].iVertexC);
            //        Edge bCA = new Edge(fFaces[j].iVertexC, fFaces[j].iVertexA);

            //       // ((a.iVertexA == b.iVertexA) & (a.iVertexB == b.iVertexB)) |
            //       //((a.iVertexA == b.iVertexB) & (a.iVertexB == b.iVertexA));
            //        bool aABbAB = ((aAB.iVertexA == bAB.iVertexA) & (aAB.iVertexB == bAB.iVertexB)) |
            //                      ((aAB.iVertexA == bAB.iVertexB) & (aAB.iVertexB == bAB.iVertexA));
            //        bool aABbBC = ((aAB.iVertexA == bBC.iVertexA) & (aAB.iVertexB == bBC.iVertexB)) |
            //                      ((aAB.iVertexA == bBC.iVertexB) & (aAB.iVertexB == bBC.iVertexA));
            //        bool aABbCA = ((aAB.iVertexA == bCA.iVertexA) & (aAB.iVertexB == bCA.iVertexB)) |
            //                      ((aAB.iVertexA == bCA.iVertexB) & (aAB.iVertexB == bCA.iVertexA));

            //        bool aBCbAB = ((aBC.iVertexA == bAB.iVertexA) & (aBC.iVertexB == bAB.iVertexB)) |
            //                      ((aBC.iVertexA == bAB.iVertexB) & (aBC.iVertexB == bAB.iVertexA));
            //        bool aBCbBC = ((aBC.iVertexA == bBC.iVertexA) & (aBC.iVertexB == bBC.iVertexB)) |
            //                      ((aBC.iVertexA == bBC.iVertexB) & (aBC.iVertexB == bBC.iVertexA));
            //        bool aBCbCA = ((aBC.iVertexA == bCA.iVertexA) & (aBC.iVertexB == bCA.iVertexB)) |
            //                      ((aBC.iVertexA == bCA.iVertexB) & (aBC.iVertexB == bCA.iVertexA));

            //        bool aCAbAB = ((aCA.iVertexA == bAB.iVertexA) & (aCA.iVertexB == bAB.iVertexB)) |
            //                      ((aCA.iVertexA == bAB.iVertexB) & (aCA.iVertexB == bAB.iVertexA));
            //        bool aCAbBC = ((aCA.iVertexA == bBC.iVertexA) & (aCA.iVertexB == bBC.iVertexB)) |
            //                      ((aCA.iVertexA == bBC.iVertexB) & (aCA.iVertexB == bBC.iVertexA));
            //        bool aCAbCA = ((aCA.iVertexA == bCA.iVertexA) & (aCA.iVertexB == bCA.iVertexB)) |
            //                      ((aCA.iVertexA == bCA.iVertexB) & (aCA.iVertexB == bCA.iVertexA));


            //        if (aABbAB | aABbBC | aABbCA) faces[i].tAB = faces[j];
            //        if (aBCbAB | aBCbBC | aBCbCA) faces[i].tBC = faces[j];
            //        if (aCAbAB | aCAbBC | aCAbCA) faces[i].tCA = faces[j];

            //        if (aABbAB | aBCbAB | aCAbAB) faces[j].tAB = faces[i];
            //        if (aABbBC | aBCbBC | aCAbBC) faces[j].tBC = faces[i];
            //        if (aABbCA | aBCbCA | aCAbCA) faces[j].tCA = faces[i];
            //    }

            switch (cdTreeType)
            {
                case CollisionTreeType.BoundingBall:
                    cdTree = new BBTreeNode(trFaces);
                    break;
                case CollisionTreeType.AABB:
                    cdTree = new AABBTreeNode(trFaces);
                    break;
                case CollisionTreeType.OBB:
                    break;
            }

        }

        public PhysicsMesh(GameMesh.Data data, CollisionTreeType cdTreeType)
            : this(data.Faces, data.Positions, cdTreeType)
        {
        }

        /// <summary>
        /// 计算物理网格惯性矩
        /// </summary>
        /// <param name="vCentre">定坐标系物理中心</param>
        /// <returns></returns>
        public Matrix3x3 CacInertia()
        {
            Matrix3x3 mInertiaIdentity;

            ///要算4层，所以1.4=0.25
            float mass = 0.25f / (float)positions.Length;
            //先按定义计算，然后按转动轴定理平移到物理重心（它是旋转中心）

            float Ixx = 0f;
            float Iyy = 0f;
            float Izz = 0f;
            float Ixy = 0f;
            float Ixz = 0f;
            float Iyz = 0f;

            Vector3 vCentre = cdTree.Centre;

            //y=-(1/a)x^2+1
            float p = 1;
            int s = 0;
            for (s = 0; p > 0; s--)
            {
                p = -(1.0f / 9.0f) * ((float)s * (float)s) + 1.0f;
                for (int i = 0; i < positions.Length; i++)
                {
                    Vector3 r = (positions[i] - vCentre) * p;
                    Ixx += (r.Y * r.Y + r.Z * r.Z) * mass;
                    Iyy += (r.Z * r.Z + r.X * r.X) * mass;
                    Izz += (r.X * r.X + r.Y * r.Y) * mass;
                    Ixy += (r.X * r.Y) * mass;
                    Ixz += (r.X * r.Z) * mass;
                    Iyz += (r.Y * r.Z) * mass;
                }
            }

            mInertiaIdentity.M11 = Ixx;
            mInertiaIdentity.M12 = -Ixy;
            mInertiaIdentity.M13 = -Ixz;
            mInertiaIdentity.M21 = -Ixy;
            mInertiaIdentity.M22 = Iyy;
            mInertiaIdentity.M23 = -Iyz;
            mInertiaIdentity.M31 = -Ixz;
            mInertiaIdentity.M32 = -Iyz;
            mInertiaIdentity.M33 = Izz;

            return mInertiaIdentity;
        }


        public ICollisionTree CDTree
        {
            get { return cdTree; }
        }
        
        /// <summary>
        /// 定坐标系几何重心
        /// </summary>
        public Vector3 GeomentryCentre
        {
            get { return cdTree.Centre; }
        }



        public bool IsIn(Vector3 p)
        {
            return cdTree.IsInDF(ref p);
        }
        public void Intersect(Triangle t, FastList<DirectDetectData> res)
        {
            cdTree.IntersectDF(ref t, res);
        }
        public void Intersect(BoundingSphere ball, FastList<DirectDetectData> res)
        {
            cdTree.IntersectDF(ref ball, res);
        }
        public void Intersect(LineSegment ra, FastList<DirectDetectData> res)
        {
            cdTree.IntersectDF(ref ra, res);
        }

        public void Search(BoundingBox aabb, FastList<Triangle> res)
        {
            cdTree.SearchDF(ref aabb, res);
        }
        public void Search(BoundingSphere ball, FastList<Triangle> res)
        {
            cdTree.SearchDF(ref ball, res);
        }

        public void Intersect(ICollisionTree cdTree, FastList<DirectDetectData> res)//, Matrix4x4 trans
        {

            switch (cdTree.TreeType)
            {
                case CollisionTreeType.BoundingBall:
                    //BBTreeNode.test = 0;
                    this.cdTree.IntersectDF((BBTreeNode)cdTree, res);
                    break;
                case CollisionTreeType.AABB:
                    //BBTreeNode.test = 0;
                    this.cdTree.IntersectDF((AABBTreeNode)cdTree, res);
                    break;
                case CollisionTreeType.OBB:
                    break;
            }

        }
    }
}
