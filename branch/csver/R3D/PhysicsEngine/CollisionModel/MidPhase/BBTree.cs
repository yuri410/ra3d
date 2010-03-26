using System;
using System.Collections.Generic;
using System.Text;
using R3D.Collections;
using R3D.MathLib;
using SlimDX;

namespace R3D.PhysicsEngine.CollisionModel
{
    public enum CollisionTreeType
    {
        BoundingBall,
        AABB,
        OBB
    }

    /// <summary>
    /// 碰撞包围球树
    /// </summary>
    /// <remarks>
    /// 该树的任意节点最多有8个子节点
    /// 
    /// 构造节点时，计算传给的所有三角形的包围球并将尝试将它们分成8份，然后将分好的三角形传下去
    /// 该树不是对称的，不一定是满树。树的形态由几何形态决定。
    /// 最后一级节点将只有一个三角形，以及三角形的剪枝包围球
    /// </remarks>
    public class BBTreeNode : BaseTreeNode, ICollisionTree
    {
        /// <summary>
        /// 剪枝包围球
        /// </summary>
        public BoundingSphere bBall;

        public BBTreeNode(FastList<TreeFace> remains)
        {
            int i;
            int rcount = remains.Count;

            for (i = 0; i < rcount; i++)
                bBall.Center += remains[i].mCentre;
            //for (i = 0; i < rcount; i++)
            //    bBall.vCentre += remains[i].vA;

            bBall.Center *= (1f / (float)i);// /= (float)i;

            bBall.Radius = 0;
            for (i = 0; i < rcount; i++)
            {
                float dist = MathEx.Distance(ref bBall.Center, ref remains [i].vA ); // bBall.vCentre & remains[i].vA;
                if (dist > bBall.Radius) bBall.Radius = dist;

                dist = MathEx.Distance(ref bBall.Center, ref remains[i].vB); // bBall.vCentre & remains[i].vB;
                if (dist > bBall.Radius) bBall.Radius = dist;

                dist = MathEx.Distance(ref bBall.Center, ref remains[i].vC); // bBall.vCentre & remains[i].vC;
                if (dist > bBall.Radius) bBall.Radius = dist;
            }

            if (rcount > 1)
            {
                double xoy = bBall.Center.Z;
                double yoz = bBall.Center.X;
                double xoz = bBall.Center.Y;


                FastList<TreeFace>[] lim = new FastList<TreeFace>[8] 
                    { new FastList<TreeFace>(), new FastList<TreeFace>(), 
                      new FastList<TreeFace>(), new FastList<TreeFace>(), 
                      new FastList<TreeFace>(), new FastList<TreeFace>(), 
                      new FastList<TreeFace>(), new FastList<TreeFace>() };


                for (i = 0; i < rcount; i++)
                {
                    double fxy = remains[i].mCentre.Z - xoy;// xoy[remains[i].mCentre];
                    double fyz = remains[i].mCentre.X - yoz;//yoz[remains[i].mCentre];
                    double fxz = remains[i].mCentre.Y - xoz;//xoz[remains[i].mCentre];

                    if (fxz > 0) // y>0
                        //1-4卦限
                        if (fyz > 0) // x>0
                            if (fxy > 0) // z>0
                                lim[0].Add(remains[i]);
                            else
                                lim[3].Add(remains[i]);
                        else
                            if (fxy > 0) // z>0
                                lim[1].Add(remains[i]);
                            else
                                lim[2].Add(remains[i]);
                    else

                        //5-8卦限
                        if (fyz > 0) // x>0
                            if (fxy > 0) // z>0
                                lim[4].Add(remains[i]);
                            else
                                lim[7].Add(remains[i]);

                        else
                            if (fxy > 0) // z>0
                                lim[5].Add(remains[i]);
                            else
                                lim[6].Add(remains[i]);
                }
                remains.FastClear();

                iChildrenCount = -1;
                bool[] usestat = new bool[8];
                for (i = 0; i < 8; i++)
                    if (lim[i].Count > 0)
                    {
                        iChildrenCount++;
                        usestat[i] = true;
                    }
                    else
                        usestat[i] = false;


                cChildren = new ICollisionTree[iChildrenCount + 1];

                int k = 0;
                for (i = 0; i < 8; i++)
                    if (usestat[i])
                    {
                        cChildren[k] = new BBTreeNode(lim[i]);
                        k++;
                    }
            }
            else
            {
                iChildrenCount = -1;
                tFaceData = remains[0];
                remains.Clear();
            }
        }

        public override bool IsInDF(ref Vector3 p)
        {
            if (iChildrenCount != -1)
            {
                if (MathEx.DistanceSquared(ref p, ref bBall.Center) < bBall.Radius * bBall.Radius)//(p ^ bBall.vCentre)
                {
                    bool isin = new bool();
                    for (int i = 0; i <= iChildrenCount; i++)
                        isin |= cChildren[i].IsInDF(ref p);
                    return isin;
                }
                return false;
            }
            else
            {
                if (MathEx.DistanceSquared(ref p, ref bBall.Center) < bBall.Radius * bBall.Radius)
                {
                    Plane pl = new Plane( tFaceData.vA,  tFaceData.vB,  tFaceData.vC);
                    pl.Normalize();
                    return Math.Abs(MathEx.PlaneRelative(ref pl, ref p)) <= 2;
                }
            }
            return false;
        }


        public override void IntersectDF(ref Triangle t, FastList<DirectDetectData> res)
        {
            if (MathEx.BoundingSphereIntersects(ref bBall, ref t))// bBall.Intersect(ref t))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].IntersectDF(ref t, res);
                }
                else
                {
                    FastList<DirectDetectData> data;
                    Triangle face = (Triangle)tFaceData;
                    if (Triangle.TriangleIntersect(ref t, ref face, out data))
                        res.Add(data);
                }
        }
        public override void IntersectDF(ref LineSegment ra, FastList<DirectDetectData> res)
        {

            if (MathEx.BoundingSphereIntersects(ref bBall, ref ra.Start, ref ra.End))// bBall.Intersect(ra.vStart, ra.vEnd))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].IntersectDF(ref ra, res);
                }
                else
                {
                    Vector3 pos;
                    Triangle face = (Triangle)tFaceData;
                    if (face.RayTriCollision(out pos, ref ra.Start, ref ra.End))
                        res.Add(new DirectDetectData(pos, face.vN, 0));
                }
        }
        public override void IntersectDF(ref BoundingSphere ball, FastList<DirectDetectData> res)
        {
            if (MathEx.BoundingSphereIntersects(ref bBall, ref ball))// bBall.Intersect(ref ball.Center, ref ball.Radius))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].IntersectDF(ref ball, res);
                }
                else
                {
                    DirectDetectData data;
                    Triangle face = (Triangle)tFaceData;
                    if (MathEx.BoundingSphereIntersects(ref ball, ref face, out data.vPos, out data.vNormal, out data.dDepth))//)ball.Intersect(ref face, out data.vPos, out data.vNormal, out data.dDepth))
                        res.Add(ref data);

                }
        }

        public override void IntersectBF(BoundingSphere ball, FastList<DirectDetectData> res)
        {
            Queue<BBTreeNode> q = new Queue<BBTreeNode>();
            q.Enqueue(this);
            while (q.Count > 0)
            {
                BBTreeNode node = q.Dequeue();
                if (MathEx.BoundingSphereIntersects(ref ball, ref node.bBall))// ball.Intersect(ref node.bBall.vCentre, ref node.bBall.dRange))
                    if (node.iChildrenCount != -1)
                    {
                        for (int i = 0; i <= node.iChildrenCount; i++)
                            q.Enqueue((BBTreeNode)node.cChildren[i]);
                    }
                    else
                    {
                        DirectDetectData data;
                        Triangle face = (Triangle)node.tFaceData;
                        //if (ball.Intersect(ref face, out data.vPos, out data.vNormal, out data.dDepth))
                        if (MathEx.BoundingSphereIntersects(ref ball, ref face, out data.vPos, out data.vNormal, out data.dDepth))
                            res.Add(ref data);
                    }
            }
            q.Clear();
        }


        public override void SearchDF(ref BoundingBox aabb, FastList<Triangle> res)
        {
            if (MathEx.BoundingSphereIntersects(ref bBall, ref aabb))//(bBall.Intersect(ref aabb))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].SearchDF(ref aabb, res);
                }
                else
                {
                    Triangle t = (Triangle)tFaceData;
                    //if (aabb.Intersect(ref t)) res.Add(ref t);
                    if (MathEx.AABBIntersects(ref aabb, ref t))
                        res.Add(ref t);
                }
        }
        public override void SearchDF(ref BoundingSphere ball, FastList<Triangle> res)
        {
            if (MathEx.BoundingSphereIntersects(ref bBall, ref ball))//  bBall.Intersect(ref ball.Center, ref ball.Radius))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].SearchDF(ref ball, res);
                }
                else
                {
                    Triangle t = (Triangle)tFaceData;
                    //if (ball.Intersect(ref t)) res.Add(t);
                    if (MathEx.BoundingSphereIntersects(ref ball, ref t))                    
                    {
                        res.Add(ref t);
                    }
                }
        }

        public override void IntersectDF(BBTreeNode cdTree, FastList<DirectDetectData> res)
        {
            //if ((cdTree.bBall.vCentre ^ bBall.vCentre) <= r * r)
            if (iChildrenCount != -1)
                for (int i = 0; i <= iChildrenCount; i++)
                {
                    BBTreeNode ch = (BBTreeNode)cChildren[i];
                    float r = (cdTree.bBall.Radius + ch.bBall.Radius);
                    if (MathEx.DistanceSquared(ref cdTree.bBall.Center, ref ch.bBall.Center) <= r * r)
                        cdTree.IntersectDF(ch, res); //相互遍历
                }
            else
            {
                // 有一个到了根节点
                // 那就就让另一个直接到根节点判断，就是
                Triangle t = (Triangle)tFaceData;
                cdTree.IntersectDF(ref t, res);
            }
        }
        public override void IntersectDF(AABBTreeNode cdTree, FastList<DirectDetectData> res)
        {
            //if (bBall.Intersect(cdTree.aAABB))
            if (iChildrenCount != -1)
                for (int i = 0; i <= iChildrenCount; i++)
                {
                    BBTreeNode ch = (BBTreeNode)cChildren[i];
                    //if (ch.bBall.Intersect(ref cdTree.aAABB))
                    if (MathEx.BoundingSphereIntersects(ref ch.bBall, ref cdTree.aAABB))
                        cdTree.IntersectDF(ch, res); //相互遍历
                }
            else
            {
                Triangle t = (Triangle)tFaceData;
                cdTree.IntersectDF(ref t, res);
            }
        }

        //[Obsolete()]
        public override void IntersectBF(AABBTreeNode cdTree, FastList<DirectDetectData> res)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        //[Obsolete()]
        public override void IntersectBF(BBTreeNode cdTree, FastList<DirectDetectData> res)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override Vector3 Centre
        {
            get { return bBall.Center; }
        }
        public override CollisionTreeType TreeType
        {
            get { return CollisionTreeType.BoundingBall; }
        }


    }
}
