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

namespace R3D.PhysicsEngine
{
	[Serializable]
	public enum DebugDrawModes
	{
		NoDebug = 0,
		DrawWireframe = 1,
		DrawAabb = 2,
		DrawFeaturesText = 4,
		DrawContactPoints = 8,
		NoDeactivation = 16,
		NoHelpText = 32,
		DrawText = 64,
		ProfileTimings = 128,
		EnableSatComparison = 256,
		DisableBulletLcp = 512,
		EnableCcd = 1024,
		MaxDebugDrawMode
	}

    public interface IDebugDraw
    {
        void DrawLine(Vector3 from, Vector3 to, Vector3 color);

        void DrawContactPoint(
            Vector3 pointOnB,
            Vector3 normalOnB,
            float distance,
            int lifeTime,
            Vector3 color
        );

		DebugDrawModes DebugMode { get; set; }
    }
}