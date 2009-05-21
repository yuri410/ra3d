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
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace R3D.Base
{
    public abstract class UniqueObject :IDisposable
    {
        string name;
        int hashCode;
        protected ObjectDisposingHandler<UniqueObject> disposing;

        public unsafe override int GetHashCode()
        {
            return hashCode;
        }
        public int HashCode
        {
            get { return hashCode; }
        }

        public string Name
        {
            get { return name; }
        }

        protected UniqueObject() { }

        protected UniqueObject(int hash)
        {
            hashCode = hash;
        }
        protected UniqueObject(string name)
            : this(Resource.GetHashCode(name))
        {
            this.name = name;
        }

        public event ObjectDisposingHandler<UniqueObject> Disposing
        {
            add { disposing += value; }
            remove { disposing -= value; }
        }


        protected virtual void dispose() { }

        #region IDisposable 成员

        public void Dispose()
        {
            if (disposing != null)
            {
                disposing(this);
                disposing = null;
            }
            dispose();
        }

        #endregion
    }
}