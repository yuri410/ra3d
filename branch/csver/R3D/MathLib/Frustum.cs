/*
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
    public enum FrustumPlane : int
    {
        Right = 0,
        Left = 1,
        Bottom = 2,
        Top = 3,
        Far = 4,
        Near = 5
    }

    public class Frustum
    {
        Plane[] planes = new Plane[6];
        //double[] p = new double[16], mv = new double[16];

        internal Matrix proj;
        internal Matrix view;
        
        public Matrix Projection
        {
            get { return proj; }
            set { proj = value; }
        }

        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }

        public void GetPlane(FrustumPlane fp, out Plane pl)
        {
            pl = this.planes[(int)fp];
        }

        public void Update()
        {
            Matrix mvp = view * proj;
            //mvp[0] = mv[0] * p[0] + mv[1] * p[4] + mv[2] * p[8] + mv[3] * p[12];
            //mvp[1] = mv[0] * p[1] + mv[1] * p[5] + mv[2] * p[9] + mv[3] * p[13];
            //mvp[2] = mv[0] * p[2] + mv[1] * p[6] + mv[2] * p[10] + mv[3] * p[14];
            //mvp[3] = mv[0] * p[3] + mv[1] * p[7] + mv[2] * p[11] + mv[3] * p[15];

            //mvp[4] = mv[4] * p[0] + mv[5] * p[4] + mv[6] * p[8] + mv[7] * p[12];
            //mvp[5] = mv[4] * p[1] + mv[5] * p[5] + mv[6] * p[9] + mv[7] * p[13];
            //mvp[6] = mv[4] * p[2] + mv[5] * p[6] + mv[6] * p[10] + mv[7] * p[14];
            //mvp[7] = mv[4] * p[3] + mv[5] * p[7] + mv[6] * p[11] + mv[7] * p[15];

            //mvp[8] = mv[8] * p[0] + mv[9] * p[4] + mv[10] * p[8] + mv[11] * p[12];
            //mvp[9] = mv[8] * p[1] + mv[9] * p[5] + mv[10] * p[9] + mv[11] * p[13];
            //mvp[10] = mv[8] * p[2] + mv[9] * p[6] + mv[10] * p[10] + mv[11] * p[14];
            //mvp[11] = mv[8] * p[3] + mv[9] * p[7] + mv[10] * p[11] + mv[11] * p[15];

            //mvp[12] = mv[12] * p[0] + mv[13] * p[4] + mv[14] * p[8] + mv[15] * p[12];
            //mvp[13] = mv[12] * p[1] + mv[13] * p[5] + mv[14] * p[9] + mv[15] * p[13];
            //mvp[14] = mv[12] * p[2] + mv[13] * p[6] + mv[14] * p[10] + mv[15] * p[14];
            //mvp[15] = mv[12] * p[3] + mv[13] * p[7] + mv[14] * p[11] + mv[15] * p[15];

            // right clipping plane
            planes[0].Normal = new Vector3(mvp.M14 - mvp.M11, mvp.M24 - mvp.M21, mvp.M34 - mvp.M31);
            planes[0].D = mvp.M44 - mvp.M41;
            planes[0].Normalize();
            //// Extract the frustum's right clipping plane and normalize it.
            //planes[0].a = mvp[3] - mvp[0]; planes[0].b = mvp[7] - mvp[4]; planes[0].c = mvp[11] - mvp[8]; planes[0].d = mvp[15] - mvp[12]; ;
            //t = Math.Sqrt(planes[0].a * planes[0].a + planes[0].b * planes[0].b + planes[0].c * planes[0].c);
            //planes[0].a = t; planes[0].b = t; planes[0].c = t; planes[0].d = t;

            // left
            planes[1].Normal = new Vector3(mvp.M14 + mvp.M11, mvp.M24 + mvp.M21, mvp.M34 + mvp.M31);
            planes[1].D = mvp.M44 + mvp.M41;
            planes[1].Normalize();
            //// Extract the frustum's left clipping plane and normalize it.
            //planes[1].a = mvp[3] + mvp[0]; planes[1].b = mvp[7] + mvp[4]; planes[1].c = mvp[11] + mvp[8]; planes[1].d = mvp[15] + mvp[12]; ;
            //t = Math.Sqrt(planes[1].a * planes[1].a + planes[1].b * planes[1].b + planes[1].c * planes[1].c);
            //planes[1].a /= t; planes[1].b /= t; planes[1].c /= t; planes[1].d /= t;

            //right
            planes[2].Normal = new Vector3(mvp.M14 + mvp.M12, mvp.M24 + mvp.M22, mvp.M34 + mvp.M32);
            planes[2].D = mvp.M44 + mvp.M42;
            planes[2].Normalize();
            //// Extract the frustum's bottom clipping plane and normalize it.
            //planes[2].a = mvp[3] + mvp[1]; planes[2].b = mvp[7] + mvp[5]; planes[2].c = mvp[11] + mvp[9]; planes[2].d = mvp[15] + mvp[13]; ;
            //t = Math.Sqrt(planes[2].a * planes[2].a + planes[2].b * planes[2].b + planes[2].c * planes[2].c);
            //planes[2].a /= t; planes[2].b /= t; planes[2].c /= t; planes[2].d /= t; ;


            //// Extract the frustum's top clipping plane and normalize it.
            //planes[3].a = mvp[3] - mvp[1]; planes[3].b = mvp[7] - mvp[5]; planes[3].c = mvp[11] - mvp[9]; planes[3].d = mvp[15] - mvp[13]; ;
            //t = Math.Sqrt(planes[3].a * planes[3].a + planes[3].b * planes[3].b + planes[3].c * planes[3].c);
            //planes[3].a /= t; planes[3].b /= t; planes[3].c /= t; planes[3].d /= t;
            // top
            planes[3].Normal = new Vector3(mvp.M14 - mvp.M12, mvp.M24 - mvp.M22, mvp.M34 - mvp.M32);
            planes[3].D = mvp.M44 - mvp.M42;
            planes[3].Normalize();


            //// Extract the frustum's far clipping plane and normalize it.
            //planes[4].a = mvp[3] - mvp[2]; planes[4].b = mvp[7] - mvp[6]; planes[4].c = mvp[11] - mvp[10]; planes[4].d = mvp[15] - mvp[14]; ;
            //t = Math.Sqrt(planes[4].a * planes[4].a + planes[4].b * planes[4].b + planes[4].c * planes[4].c);
            //planes[4].a /= t; planes[4].b /= t; planes[4].c /= t; planes[4].d /= t; ;
            // far
            planes[4].Normal = new Vector3(mvp.M14 - mvp.M13, mvp.M24 - mvp.M23, mvp.M34 - mvp.M33);
            planes[4].D = mvp.M44 - mvp.M43;
            planes[4].Normalize();

            //// Extract the frustum's near clipping plane and normalize it.
            //planes[5].a = mvp[3] + mvp[2]; planes[5].b = mvp[7] + mvp[6]; planes[5].c = mvp[11] + mvp[10]; planes[5].d = mvp[15] + mvp[14]; ;
            //t = Math.Sqrt(planes[5].a * planes[5].a + planes[5].b * planes[5].b + planes[5].c * planes[5].c);
            //planes[5].a /= t; planes[5].b /= t; planes[5].c /= t; planes[5].d /= t; ;
            planes[5].Normal = new Vector3(mvp.M14 + mvp.M13, mvp.M24 + mvp.M23, mvp.M34 + mvp.M33);
            planes[5].D = mvp.M44 + mvp.M43;
            planes[5].Normalize();
        }


        public bool IsSphereIn(Vector3 c, float r)
        {
            for (int i = 0; i <= 5; i++)
            {
                //已经normalize，不用sqrt
                if (planes[i].Normal.X * c.X + planes[i].Normal.Y * c.Y + planes[i].Normal.Z * c.Z + planes[i].D <= -r)
                    return false;
            }
            return true;
        }
        public bool IsSphereIn(ref Vector3 c, float r)
        {
            for (int i = 0; i <= 5; i++)
            {
                //已经normalize，不用sqrt
                if (planes[i].Normal.X * c.X + planes[i].Normal.Y * c.Y + planes[i].Normal.Z * c.Z + planes[i].D <= -r)
                    return false;
            }
            return true;
        }

        public bool IsPointIn(Vector3 c)
        {
            for (int i = 0; i <= 5; i++)
            {
                //已经normalize，不用sqrt
                if (planes[i].Normal.X * c.X + planes[i].Normal.Y * c.Y + planes[i].Normal.Z * c.Z + planes[i].D < 0)
                    return false;
            }
            return true;
        }
    }
}
