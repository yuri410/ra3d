using System;
using System.Collections.Generic;
using System.Text;
using R3D.Collections;
using R3D.MathLib;
using SlimDX;

namespace R3D.PhysicsEngine.CollisionModel
{
    public class AABBTreeNode : BaseTreeNode, ICollisionTree
    {
        public BoundingBox aAABB;

        public AABBTreeNode(FastList<TreeFace> remains)
        {
            aAABB.Minimum = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            aAABB.Maximum = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            
            int rcount = remains.Count;
            for (int i = 0; i < rcount; i++)
            {
                Vector3 va = remains[i].vA;
                Vector3 vb = remains[i].vB;
                Vector3 vc = remains[i].vC;

                if (va.X < aAABB.Minimum.X) aAABB.Minimum.X = va.X;
                if (va.X > aAABB.Maximum.X) aAABB.Maximum.X = va.X;
                if (va.Y < aAABB.Minimum.Y) aAABB.Minimum.Y = va.Y;
                if (va.Y > aAABB.Maximum.Y) aAABB.Maximum.Y = va.Y;
                if (va.Z < aAABB.Minimum.Z) aAABB.Minimum.Z = va.Z;
                if (va.Z > aAABB.Maximum.Z) aAABB.Maximum.Z = va.Z;

                if (vb.X < aAABB.Minimum.X) aAABB.Minimum.X = vb.X;
                if (vb.X > aAABB.Maximum.X) aAABB.Maximum.X = vb.X;
                if (vb.Y < aAABB.Minimum.Y) aAABB.Minimum.Y = vb.Y;
                if (vb.Y > aAABB.Maximum.Y) aAABB.Maximum.Y = vb.Y;
                if (vb.Z < aAABB.Minimum.Z) aAABB.Minimum.Z = vb.Z;
                if (vb.Z > aAABB.Maximum.Z) aAABB.Maximum.Z = vb.Z;

                if (vc.X < aAABB.Minimum.X) aAABB.Minimum.X = vc.X;
                if (vc.X > aAABB.Maximum.X) aAABB.Maximum.X = vc.X;
                if (vc.Y < aAABB.Minimum.Y) aAABB.Minimum.Y = vc.Y;
                if (vc.Y > aAABB.Maximum.Y) aAABB.Maximum.Y = vc.Y;
                if (vc.Z < aAABB.Minimum.Z) aAABB.Minimum.Z = vc.Z;
                if (vc.Z > aAABB.Maximum.Z) aAABB.Maximum.Z = vc.Z;
            }

            if (rcount > 1)
            {
                //centre可能不在AABB内
                Vector3 centre = new Vector3();
                for (int i = 0; i < rcount; i++)
                {
                    Vector3.Add(ref centre,ref remains[i].mCentre, out centre);
                }
                Vector3.Multiply(ref centre, 1f / (float)rcount, out centre);

                float xoy = centre.Z;
                float yoz = centre.X;
                float xoz = centre.Y;

                FastList<TreeFace>[] lim = new FastList<TreeFace>[8] 
                    { new FastList<TreeFace>(), new FastList<TreeFace>(), 
                      new FastList<TreeFace>(), new FastList<TreeFace>(), 
                      new FastList<TreeFace>(), new FastList<TreeFace>(), 
                      new FastList<TreeFace>(), new FastList<TreeFace>() };

                for (int i = 0; i < rcount; i++)
                {
                    float fxy = remains[i].mCentre.Z - xoy;// xoy[remains[i].mCentre];
                    float fyz = remains[i].mCentre.X - yoz;//yoz[remains[i].mCentre];
                    float fxz = remains[i].mCentre.Y - xoz;//xoz[remains[i].mCentre];

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
                for (int i = 0; i < 8; i++)
                    if (lim[i].Count > 0)
                    {
                        iChildrenCount++;
                        usestat[i] = true;
                    }
                    else
                        usestat[i] = false;

                cChildren = new ICollisionTree[iChildrenCount + 1];

                int k = 0;
                for (int i = 0; i < 8; i++)
                    if (usestat[i])
                    {
                        cChildren[k] = new AABBTreeNode(lim[i]);
                        k++;
                    }
            }
            else
            {
                iChildrenCount = -1;
                tFaceData = remains[0];
                remains.FastClear();
            }
        }

        public override CollisionTreeType TreeType
        {
            get { return CollisionTreeType.AABB; }
        }
        public override Vector3 Centre
        {
            get
            {
                return new Vector3((aAABB.Minimum.X + aAABB.Maximum.X) * 0.5f,
                                   (aAABB.Minimum.Y + aAABB.Maximum.Y) * 0.5f,
                                   (aAABB.Minimum.Z + aAABB.Maximum.Z) * 0.5f);
            }
        }

        public override bool IsInDF(ref Vector3 p)
        {
            if (MathEx.AABBIsIn(ref aAABB, ref p))
                if (iChildrenCount != -1)
                {
                    bool isin = new bool();
                    for (int i = 0; i <= iChildrenCount; i++)
                        isin |= cChildren[i].IsInDF(ref p);
                    return isin;
                }
                else
                {
                    Plane pl = new Plane(tFaceData.vA,  tFaceData.vB, tFaceData.vC);
                    pl.Normalize();
                    return Math.Abs(MathEx.PlaneRelative(ref pl, ref p)) <= 2;
                }
            return false;
        }

        public override void IntersectDF(ref Triangle t, FastList<DirectDetectData> res)
        {
            if (MathEx.AABBIntersects(ref aAABB, ref t))//aAABB.Intersect(ref t))
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
            if (iChildrenCount != -1)
            {
                if (MathEx.AABBIntersects(ref aAABB, ref ra.Start, ref ra.End))//aAABB.Intersect(ra.vStart, ra.vEnd))
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].IntersectDF(ref ra, res);
            }
            else
            {
                //if (aAABB.Intersect(ra.vStart, ra.vEnd))
                //{
                Vector3 pos;
                Triangle face = (Triangle)tFaceData;
                if (face.RayTriCollision(out pos, ref ra.Start, ref ra.End))
                    res.Add(new DirectDetectData(pos, face.vN, 0));
                //}

            }
        }
        public override void IntersectDF(ref BoundingSphere ball, FastList<DirectDetectData> res)
        {
            if (MathEx.AABBIntersects(ref aAABB, ref ball))// aAABB.Intersect(ref ball.Center, ref ball.Radius))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].IntersectDF(ref ball, res);
                }
                else
                {
                    DirectDetectData data;
                    Triangle face = (Triangle)tFaceData;
                    if (MathEx.BoundingSphereIntersects(ref ball, ref face, out data.vPos, out data.vNormal, out data.dDepth))//(ball.Intersect(ref face, out data.vPos, out data.vNormal, out data.dDepth))
                        res.Add(ref data);

                    //Simulation.PhysMT.testtest2++;
                }
        }

        public override void IntersectBF(BoundingSphere ball, FastList<DirectDetectData> res)
        {
            Queue<AABBTreeNode> q = new Queue<AABBTreeNode>();
            q.Enqueue(this);
            while (q.Count > 0)
            {
                AABBTreeNode node = q.Dequeue();
                if (MathEx.AABBIntersects(ref node.aAABB, ref ball))//(node.aAABB.Intersect(ref ball.Center, ref ball.Radius))
                    if (node.iChildrenCount != -1)
                    {
                        for (int i = 0; i <= node.iChildrenCount; i++)
                            q.Enqueue((AABBTreeNode)node.cChildren[i]);
                    }
                    else
                    {
                        DirectDetectData data;
                        Triangle face = (Triangle)node.tFaceData;
                        if (MathEx.BoundingSphereIntersects(ref ball, ref face, out data.vPos, out data.vNormal, out data.dDepth))//(ball.Intersect(ref face, out data.vPos, out data.vNormal, out data.dDepth))
                            res.Add(ref data);
                    }
            }
            q.Clear();
        }

        public override void SearchDF(ref BoundingBox aabb, FastList<Triangle> res)
        {
            if (MathEx.AABBIntersects(ref aAABB, ref aabb))// aAABB.Intersect(ref aabb))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].SearchDF(ref aabb, res);
                }
                else
                {
                    Triangle t = (Triangle)tFaceData;
                    //if (aabb.Intersect(ref t)) res.Add(t);
                    if (MathEx.AABBIntersects(ref aabb, ref t))
                        res.Add(ref t);
                }
        }
        public override void SearchDF(ref BoundingSphere ball, FastList<Triangle> res)
        {
            if (MathEx.BoundingSphereIntersects(ref ball, ref aAABB))// ball.Intersect(ref aAABB))
                if (iChildrenCount != -1)
                {
                    for (int i = 0; i <= iChildrenCount; i++)
                        cChildren[i].SearchDF(ref ball, res);
                }
                else
                {
                    Triangle t = (Triangle)tFaceData;
                    if (MathEx.BoundingSphereIntersects(ref ball, ref t))
                    {
                        res.Add(ref t);
                    }
                    //if (ball.Intersect(ref t)) res.Add(t);
                }
        }


        public override void IntersectDF(AABBTreeNode cdTree, FastList<DirectDetectData> res)
        {
            //if (aAABB.Intersect(cdTree.aAABB))
            if (iChildrenCount != -1)
                for (int i = 0; i <= iChildrenCount; i++)
                {
                    AABBTreeNode ch = (AABBTreeNode)cChildren[i];
                    if (MathEx.AABBIntersects(ref cdTree.aAABB, ref ch.aAABB))//(cdTree.aAABB.Intersect(ref ch.aAABB))
                        cdTree.IntersectDF(ch, res); //相互遍历
                }
            else
            {
                Triangle t = (Triangle)tFaceData;
                cdTree.IntersectDF(ref t, res);
            }
        }
        public override void IntersectDF(BBTreeNode cdTree, FastList<DirectDetectData> res)
        {
            //if (aAABB.Intersect(cdTree.bBall.vCentre, cdTree.bBall.dRange))
            if (iChildrenCount != -1)
                for (int i = 0; i <= iChildrenCount; i++)
                {
                    AABBTreeNode ch = (AABBTreeNode)cChildren[i];
                    if (MathEx.BoundingSphereIntersects(ref cdTree.bBall, ref ch.aAABB))//(cdTree.bBall.Intersect(ref ch.aAABB))
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
            Queue<AABBTreeNode> a = new Queue<AABBTreeNode>();
            Queue<AABBTreeNode> b = new Queue<AABBTreeNode>();
            a.Enqueue(this);
            b.Enqueue(cdTree);

            throw new Exception("The method or operation is not implemented.");
        }
        //[Obsolete()]
        public override void IntersectBF(BBTreeNode cdTree, FastList<DirectDetectData> res)
        {
            Queue<AABBTreeNode> a = new Queue<AABBTreeNode>();
            Queue<BBTreeNode> b = new Queue<BBTreeNode>();

            a.Enqueue(this);
            b.Enqueue(cdTree);

            while (a.Count > 0)
            {
                AABBTreeNode nodea = a.Dequeue();
                //AABBTreeNode nodeb = b.Dequeue();

                // a/b anainst each b/a , if overlaps add a/b
                // a or b depends on Queue.Count

            }

            throw new Exception("The method or operation is not implemented.");
        }
    }
}
