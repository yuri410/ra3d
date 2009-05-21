using System;
using System.Collections.Generic;
using System.Text;

namespace R3D.PhysicsEngine
{
    public interface IOverlapFilterCallback
    {
		/// <summary>
		/// Checks if pairs need collision.
		/// </summary>
		/// <param name="proxyA">First proxy.</param>
		/// <param name="proxyB">Second proxy.</param>
		/// <returns>Returns true when pairs need collision.</returns>
		bool NeedBroadphaseCollision(BroadphaseProxy proxyA, BroadphaseProxy proxyB);
    }
}
