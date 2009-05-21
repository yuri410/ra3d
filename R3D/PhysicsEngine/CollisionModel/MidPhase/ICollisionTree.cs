using System;
using System.Collections.Generic;
using System.Text;
using R3D.Collections;
using R3D.MathLib;
using SlimDX;

namespace R3D.PhysicsEngine.CollisionModel
{
    public interface ICollisionTree : IDisposable
    {
        CollisionTreeType TreeType { get; }
        Vector3 Centre { get; }

        bool IsInDF(ref Vector3 p);
        void IntersectDF(ref Triangle t, FastList<DirectDetectData> res);
        void IntersectDF(ref LineSegment ra, FastList<DirectDetectData> res);
        void IntersectDF(ref BoundingSphere ball, FastList<DirectDetectData> res);

        void IntersectBF(BoundingSphere ball, FastList<DirectDetectData> res);

        //void Intersect(AABB aabb, List<DirectDetectData> res);
        //void Intersect(Quad plane, List<DirectDetectData> res);

        void IntersectDF(AABBTreeNode cdTree, FastList<DirectDetectData> res);
        void IntersectDF(BBTreeNode cdTree, FastList<DirectDetectData> res);

        void IntersectBF(AABBTreeNode cdTree, FastList<DirectDetectData> res);
        void IntersectBF(BBTreeNode cdTree, FastList<DirectDetectData> res);

        void SearchDF(ref BoundingSphere ball, FastList<Triangle> res);
        void SearchDF(ref BoundingBox aabb, FastList<Triangle> res);
    }
}
