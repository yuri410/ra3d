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
using R3D.GraphicsEngine;
using R3D.MathLib;
using SlimDX;

namespace R3D.Core
{
    public abstract class SceneObject : IRenderable
    {
        /// <summary>
        ///  true when the transformation bv is changed
        /// </summary>
        public bool RequiresUpdate
        {
            get;
            set;
        }

        protected SceneObject(bool hasSubObject)
        {
            HasSubObjects = hasSubObject;
        }
        //protected SceneObject(bool hasSubObject,bool multiBatch)
        //{
        //    HasSubObjects = hasSubObject;
        //    //RequireMultiBatches = multiBatch;
        //}

        public bool HasSubObjects
        {
            get;
            protected set;
        }
        //public bool RequireMultiBatches
        //{
        //    get;
        //    protected set;
        //}


        //public BoundingBox BoundingBox;
        public BoundingSphere BoundingSphere;

        public Matrix Transformation = Matrix.Identity;


        //public int BatchesNeeded
        //{
        //    get;
        //    protected set;
        //}

        public virtual void PrepareVisibleObjects(Camera cam)
        {
            throw new NotSupportedException();
        }

        
        #region IRenderable 成员

        public abstract RenderOperation[] GetRenderOperation();


        #endregion

        public virtual bool IntersectsSelectionRay(ref Ray ray)
        {
            return MathEx.BoundingSphereIntersects(ref  BoundingSphere, ref ray);
        }
    }
}
