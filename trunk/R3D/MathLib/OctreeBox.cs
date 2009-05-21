﻿/*
 * Copyright (C) 2008 R3D Development Team
 * 
 * R3D is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * R3D is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with R3D.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;

namespace R3D.MathLib
{
    public struct OctreeBox
    {
        public Vector3 Center;

        public float Length;

        public OctreeBox(ref BoundingSphere sph)
        {
            Center = sph.Center;
            Length = sph.Radius * 2;
        }

        public void GetBoundingSphere(out BoundingSphere sp)
        {
            sp.Center = Center;
            sp.Radius = Length * (MathEx.Root3 / 2f);  // 0.5f;
        }
    }
}
