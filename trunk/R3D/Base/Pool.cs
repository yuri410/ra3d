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
    public abstract class ObjectPool<T>
        where T : IDisposable 
    {
        Queue<T> buffered = new Queue<T>();

        protected abstract T CreateInstance();

        protected T Create()
        {
            if (buffered.Count == 0)
            {
                return CreateInstance();
            }
            return buffered.Dequeue();
        }

        protected void Release(T obj)
        {
            obj.Dispose();
            buffered.Enqueue(obj);
        }
    }
}