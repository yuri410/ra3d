using System;
using System.Collections.Generic;
using System.Text;
using R3D.Collections;
using R3D.MathLib;
using SlimDX;

namespace R3D.PhysicsEngine.CollisionModel
{
    public abstract class BaseTreeNode : ICollisionTree, IDisposable
    {
        /// <summary>
        /// 获得 子节点数组 的上界。
        /// 如果为-1则 子节点数组 为null。并且具有faceData。
        /// 当然，子节点数就是
        /// </summary>
        /// <remarks>
        /// 即：遍历节点有两种情况（of course）：
        ///  1.末级节点〈=〉childrenCount==-1
        ///  2.中间级节点〈=〉childrenCount!=-1
        /// </remarks>
        public int iChildrenCount;
        /// <summary>
        /// 获得子节点数组
        /// </summary>
        public ICollisionTree[] cChildren;
        /// <summary>
        /// 获得表面数据
        /// </summary>
        public TreeFace tFaceData;

        public abstract Vector3 Centre { get; }
        public abstract CollisionTreeType TreeType { get; }

        public abstract bool IsInDF(ref Vector3 p);

        public abstract void IntersectDF(ref Triangle t, FastList<DirectDetectData> res);
        public abstract void IntersectDF(ref LineSegment ra, FastList<DirectDetectData> res);
        public abstract void IntersectDF(ref BoundingSphere ball, FastList<DirectDetectData> res);
        public abstract void IntersectBF(BoundingSphere ball, FastList<DirectDetectData> res);

        //public abstract void Intersect(Quad plane, List<DirectDetectData> res);
        //public abstract void Intersect(AABB aabb, List<DirectDetectData> res);

        public abstract void IntersectDF(AABBTreeNode cdTree, FastList<DirectDetectData> res);
        public abstract void IntersectDF(BBTreeNode cdTree, FastList<DirectDetectData> res);

        public abstract void IntersectBF(AABBTreeNode cdTree, FastList<DirectDetectData> res);
        public abstract void IntersectBF(BBTreeNode cdTree, FastList<DirectDetectData> res);


        public abstract void SearchDF(ref BoundingSphere ball, FastList<Triangle> res);
        public abstract void SearchDF(ref BoundingBox aabb, FastList<Triangle> res);

        public void Dispose()
        {
            if (iChildrenCount != -1)
            {
                for (int i = 0; i <= iChildrenCount; i++)
                {
                    ((IDisposable)cChildren[i]).Dispose();
                    cChildren[i] = null;
                }
                cChildren = null;
            }
            else
                tFaceData = null;
        }
    }

    /// <summary>
    /// 三角面
    /// </summary>
    public class TreeFace
    {
        public int iTri;
        public Vector3 vA, vB, vC;
        public Vector3 mCentre;
        //public TreeFace tAB, tBC, tCA;

        public TreeFace(int tri, Vector3 a, Vector3 b, Vector3 c)//,  TreeFace ab, TreeFace bc, TreeFace ca)
        {
            vA = a; vB = b; vC = c;
            mCentre = (a + b + c) / 3;
            iTri = tri;// mPhysMesh = mesh;
            //tAB = ab; tBC = bc; tCA = ca;
        }

        public static explicit operator Triangle(TreeFace src)
        {
            return new Triangle(ref src.vA, ref src.vB, ref src.vC);
        }
    }
}
