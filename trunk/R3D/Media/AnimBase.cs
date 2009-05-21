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
using System.Drawing;

using R3D.Base;
using R3D.IO;

namespace R3D.Media
{
    public abstract class AnimBase : UniqueObject  // : Ra2ReloadFile
    {
        protected int imgCount;
        protected int width;
        protected int height;

        //protected AnimBase(string file, int size, bool isinMix)
        //    : base(file, size, isinMix) { }

        protected AnimBase(string name)
            : base(name)
        { }

        protected AnimBase(int hash)
            : base(hash)
        { }
        protected AnimBase() { }

        public unsafe abstract ImageBase GetImage(int index);

        public int FrameCount
        {
            get { return imgCount; }
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

    }
}
