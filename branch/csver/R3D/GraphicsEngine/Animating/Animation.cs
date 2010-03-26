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
using System.ComponentModel;
using System.Text;
using R3D.Design;
using R3D.GraphicsEngine;
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine.Animating
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public abstract class Animation
    {
        string name;
        protected Device device;
        int curFrame;

        bool hasSuggestedRenderOrder;

        //protected Matrix baseTranform;

        protected Animation(Device dev, bool hasSuggestedRenderOrder)
        {
            this.hasSuggestedRenderOrder = hasSuggestedRenderOrder;
            device = dev;
        }

        [Browsable(false)]
        public int CurrentFrame
        {
            get { return curFrame; }
            protected set { curFrame = value; }
        }

        public abstract string AnimationType { get; }
        public abstract bool IsAvailable(int index);
        //public abstract void SetAnimation(int index);

        public abstract void GetTransform(int entityId, out Matrix mat);
        

        

        public abstract void ReadData(BinaryDataReader data);
        public abstract void WriteData(BinaryDataWriter data);

        //public void Begin()
        //{
        //    baseTranform = device.GetTransform(TransformState.World);
        //    begin();
        //}
        //public void End()
        //{
        //    end();
        //}

        [Browsable(false)]
        public bool HasSuggestedRenderOrder
        {
            get { return hasSuggestedRenderOrder; }
        }

        //protected virtual void begin() { }
        //protected virtual void end() { }

        internal void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
