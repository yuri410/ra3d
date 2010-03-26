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
using R3D.PhysicsEngine.LinearMath;
using R3D.Collections;

namespace R3D.PhysicsEngine
{
	/// <summary>
	/// PHY_ScalarType enumerates possible scalar types.
	/// See the StridingMeshInterface for its use
	/// </summary>
	[Serializable]
	public enum PHY_ScalarType
	{
		PHY_FLOAT,
		PHY_DOUBLE,
		PHY_INTEGER,
		PHY_SHORT,
		PHY_FIXEDPOINT88
	}

	/// <summary>
	/// StridingMeshInterface is the interface class for high performance access to triangle meshes
	/// It allows for sharing graphics and collision meshes. Also it provides locking/unlocking of graphics meshes that are in gpu memory.
	/// </summary>
	[Serializable]
	public abstract class StridingMeshInterface
	{
		protected Vector3 scaling;

		public StridingMeshInterface()
		{
			scaling = new Vector3(1f, 1f, 1f);
		}

		public void InternalProcessAllTriangles(ITriangleIndexCallback callback, Vector3 aabbMin, Vector3 aabbMax)
		{
			int numtotalphysicsverts = 0;
            int numtriangles, gfxindex;
			int part, graphicssubparts = SubPartsCount();
            Vector3[] triangle = new Vector3[3];
            FastList<Vector3> verts;
            FastList<int> indicies;

			Vector3 meshScaling = Scaling;

			//if the number of parts is big, the performance might drop due to the innerloop switch on indextype
			for (part = 0; part < graphicssubparts; part++)
			{
                GetLockedReadOnlyVertexIndexBase(out verts, out indicies, out numtriangles, part);
			    numtotalphysicsverts += numtriangles * 3; //upper bound

                for (gfxindex = 0; gfxindex < numtriangles; gfxindex++)
                {
                    triangle[0] = verts[indicies[gfxindex * 3 + 0]];
                    triangle[1] = verts[indicies[gfxindex * 3 + 1]];
                    triangle[2] = verts[indicies[gfxindex * 3 + 2]];

                    callback.ProcessTriangleIndex(triangle, part, gfxindex);
                }

			    UnLockReadOnlyVertexBase(part);
			}
		}

		public void CalculateAabbBruteForce(out Vector3 aabbMin, out Vector3 aabbMax)
		{
			//first calculate the total aabb for all triangles
			AabbCalculationCallback aabbCallback = new AabbCalculationCallback();
			aabbMax = new Vector3(1e30f, 1e30f, 1e30f);
			aabbMin = new Vector3(-1e30f, -1e30f, -1e30f);
			InternalProcessAllTriangles(aabbCallback, aabbMin, aabbMax);

			aabbMin = aabbCallback.AabbMin;
			aabbMax = aabbCallback.AabbMax;
		}

		// get read and write access to a subpart of a triangle mesh
		// this subpart has a continuous array of vertices and indices
		// in this way the mesh can be handled as chunks of memory with striding
		// very similar to OpenGL vertexarray support
		// make a call to unLockVertexBase when the read and write access is finished	
		public abstract void GetLockedVertexIndexBase(out FastList<Vector3> verts, out FastList<int> indicies, out int numfaces, int subpart);

        public abstract void GetLockedReadOnlyVertexIndexBase(out FastList<Vector3> verts, out FastList<int> indicies, out int numfaces, int subpart);
	
		// unLockVertexBase finishes the access to a subpart of the triangle mesh
		// make a call to unLockVertexBase when the read and write access (using getLockedVertexIndexBase) is finished
		public abstract void UnLockVertexBase(int subpart);

		public abstract void UnLockReadOnlyVertexBase(int subpart);


		// getNumSubParts returns the number of seperate subparts
		// each subpart has a continuous array of vertices and indices
        public abstract int SubPartsCount();

		public abstract void PreallocateVertices(int numverts);
		public abstract void PreallocateIndices(int numindices);

		public Vector3 Scaling
        {           
			get { return scaling; }
            set { scaling = value; }
		}
	}

	class AabbCalculationCallback : ITriangleIndexCallback
	{
		private Vector3 _aabbMin;
		private Vector3 _aabbMax;

		public AabbCalculationCallback()
		{
			_aabbMin = new Vector3(1e30f, 1e30f, 1e30f);
			_aabbMax = new Vector3(-1e30f, -1e30f, -1e30f);
		}

		public Vector3 AabbMin { get { return _aabbMin; } set { _aabbMin = value; } }
		public Vector3 AabbMax { get { return _aabbMax; } set { _aabbMax = value; } }

		#region ITriangleIndexCallback Members
		public void ProcessTriangleIndex(Vector3[] triangle, int partId, int triangleIndex)
		{
			MathHelper.SetMin(ref _aabbMin, triangle[0]);
			MathHelper.SetMax(ref _aabbMax, triangle[0]);
			MathHelper.SetMin(ref _aabbMin, triangle[1]);
			MathHelper.SetMax(ref _aabbMax, triangle[1]);
			MathHelper.SetMin(ref _aabbMin, triangle[2]);
			MathHelper.SetMax(ref _aabbMax, triangle[2]);
		}
		#endregion
	}

}
