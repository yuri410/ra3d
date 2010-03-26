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
using System.IO;

using R3D.IO;
using R3D;
using Ra2Develop.Designers;

namespace Ra2Develop
{

    public unsafe class DevFileLocation : FileLocation
    {
        public DevFileLocation(string filePath)
            : base(filePath, 0)
        {
            if (File.Exists(filePath))
            {
                size = (int)new FileInfo(filePath).Length;
            }
        }

        public override Stream GetStream
        {
            get { return new FileStream(base.Path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

    }
    public unsafe class DevMemoryLocation : MemoryLocation
    {
        DocumentBase parent;

        public DevMemoryLocation(DocumentBase parent, void* data, int size) :
            base(data, size)        
        {
            this.parent = parent;
        }


    }
 
}
