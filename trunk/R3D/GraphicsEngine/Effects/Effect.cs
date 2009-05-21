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
using R3D.Core;
using SlimDX;

namespace R3D.GraphicsEngine.Effects
{
    public abstract class ModelEffectFactory
    {
        public abstract ModelEffect CreateInstance();

        public abstract void DestroyInstance(ModelEffect fx);
        //public abstract string Name
        //{
        //    get;
        //}
    }
    public abstract class PostEffectFactory
    {
        public abstract PostEffect CreateInstance();
        public abstract void DestroyInstance(PostEffect fx);
        //public abstract string Name
        //{
        //    get;
        //}
    }


    public abstract class EffectBase : IDisposable
    {      
        PassType[] passTypes;


        public EffectBase(PassType[] pts)
        {
            passTypes = pts;
        }

        public PassType GetPassType(int i)
        {
            return passTypes[i];
        }


        public bool Disposed
        {
            get;
            private set;
        }
        protected abstract void Dispose(bool disposing);

        #region IDisposable 成员

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion

        ~EffectBase()
        {
            if (!Disposed)
                Dispose();
        }
    }

    public abstract class ModelEffect : EffectBase
    {
        bool begun;
        public ModelEffect(bool supportsInstancing, PassType[] pts)
            : base(pts)
        {
            SupportsInstancing = supportsInstancing;
        }


        public abstract Camera GetPassCamera(int passId);

        protected abstract int begin();
        protected abstract void end();
        public abstract void BeginPass(int passId);
        public abstract void EndPass();
        public int Begin()
        {
            if (!begun)
            {
                begun = true;
                return begin();
            }
            return -1;                 
        }


        public void End()
        {
            if (begun)
            {
                end();
                begun = false;
            }
        }

        public bool SupportsInstancing
        {
            get;
            protected set;
        }
        public virtual void SetInstanceTransform(ref Matrix trans)
        {
            throw new NotSupportedException();
        }
        

        public abstract void Setup(SceneManagerBase sceMgr, MeshMaterial mat, ref RenderOperation op);
    }

    public abstract class PostEffect : EffectBase
    {
        public PostEffect(PassType[] pts)
            : base(pts)
        { }
    }
}
