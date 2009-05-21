using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;

namespace R3D.PhysicsEngine.CollisionModel
{

    public struct DirectDetectData
    {
        public Vector3 vPos;
        public Vector3 vNormal;
        public float dDepth;

        public DirectDetectData(Vector3 p, Vector3 n, float depth)
        {
            vPos = p;
            vNormal = n;
            dDepth = depth;
        }
    }

}
