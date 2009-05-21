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

namespace R3D.Base
{
    public abstract class Resource : CachedObject
    {
        string name;
        int hashCode;

        protected ObjectDisposingHandler<Resource> disposing;

        /// <summary>
        /// 所有资源的名称都统一用该方法计算哈希代码
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int GetHashCode(string name)
        {
            return name.ToUpper().GetHashCode();
        }

        public unsafe override int GetHashCode()
        {
            return hashCode;
        }
        public int HashCode
        {
            get { return hashCode; }
        }

        protected string Name
        {
            get { return name; }
        }

        protected Resource(IResourceManager rMgr, int hash)
            : base(rMgr)
        {
            hashCode = hash;
        }
        protected Resource(IResourceManager rMgr, string name)
            : this(rMgr, Resource.GetHashCode(name))
        {
            this.name = name;
        }

        public event ObjectDisposingHandler<Resource> Disposing
        {
            add { disposing += value; }
            remove { disposing -= value; }
        }

        public virtual void dispose()
        { }

        public void Dispose()
        {            
            if (disposing != null)
            {
                disposing(this);
                disposing = null;
            }
            dispose();
        }

    }
}
