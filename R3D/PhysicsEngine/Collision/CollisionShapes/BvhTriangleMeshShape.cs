/*
  Bullet for XNA Copyright (c) 2003-2007 Vsevolod Klementjev http://www.codeplex.com/xnadevru
  Bullet original C++ version Copyright (c) 2003-2007 Erwin Coumans http://bulletphysics.com

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.

  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Collections.Generic;
using System.Text;
using R3D.MathLib;
using R3D.Collections;

namespace R3D.PhysicsEngine
{
	[Serializable]
    class MyNodeOverlapCallback : INodeOverlapCallback
    {
        StridingMeshInterface _meshInterface;
        ITriangleCallback _callback;
        Vector3[] _triangle = new Vector3[3];

        public MyNodeOverlapCallback(ITriangleCallback callback, StridingMeshInterface meshInterface)
        {
            _meshInterface = meshInterface;
            _callback = callback;
        }

        public void ProcessNode(OptimizedBvhNode node)
        {
            FastList<Vector3> verts;
            FastList<int> indicies;
            int numtriangles;

            _meshInterface.GetLockedReadOnlyVertexIndexBase(out verts, out indicies, out numtriangles, node.SubPart);
            Vector3 meshScaling = _meshInterface.Scaling;

            for (int j = 0; j < 3; j++)
            {
                _triangle[j] = verts[indicies[j + node.TriangleIndex * 3]] * meshScaling;
            }

            _callback.ProcessTriangle(_triangle, node.SubPart, node.TriangleIndex);
            _meshInterface.UnLockReadOnlyVertexBase(node.SubPart);
        }
    }

	[Serializable]
    public class BvhTriangleMeshShape : TriangleMeshShape
    {
        private OptimizedBvh _bvh = new OptimizedBvh();
		private bool _useQuantizedAabbCompression;
		private bool[] _pad = new bool[12];

        public BvhTriangleMeshShape(StridingMeshInterface meshInterface) : base(meshInterface)
        {
            _bvh.Build(meshInterface);
        }

		public OptimizedBvh OptimizedBvh { get { return _bvh; } }
		public bool UseQuantizedAabbCompression { get { return _useQuantizedAabbCompression; } }

        public override void ProcessAllTriangles(ITriangleCallback callback, Vector3 aabbMin, Vector3 aabbMax)
        {
            MyNodeOverlapCallback myNodeCallback = new MyNodeOverlapCallback(callback, MeshInterface);
            _bvh.ReportAabbOverlappingNodex(myNodeCallback, aabbMin, aabbMax);
        }

        public override string Name
        {
            get
            {
                return "BvhTriangleMesh";
            }
        }
    }
}
