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
using R3D.IO;

namespace R3D.Base
{
    public delegate void ObjectDisposingHandler<T>(T obj);
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 一个Manager实例只能管理一个组
    /// </remarks>
    public abstract class UniqueObjectManager<S, T> : IUniqueObjManager<S, T> where T : UniqueObject
    {
        Dictionary<int, WeakReference> hashTable = new Dictionary<int, WeakReference>();

        public T Exists(int hashcode)
        {
            WeakReference res;
            if (hashTable.TryGetValue(hashcode, out res))
            {
                if (res.IsAlive)
                {
                    return (T)res.Target;
                }
                else
                {
                    hashTable.Remove(hashcode);
                    return null;
                }
            }
            return null;
        }

        void FlushDisposedEntry()
        {
            List<int> disposed = new List<int>();
            foreach (KeyValuePair<int, WeakReference> i in hashTable)
            {
                if (!i.Value.IsAlive)
                {
                    disposed.Add(i.Key);
                }
            }

            for (int i = 0; i < disposed.Count; i++)
            {
                hashTable.Remove(disposed[i]);
            }
        }
        void UnregisterObject(UniqueObject obj)
        {
            hashTable.Remove(obj.HashCode);
        }

        public T CreateInstance(S rl)
        {
            WeakReference entry;
            int hash = rl.GetHashCode();

            if (hashTable.TryGetValue(hash, out entry))
            {
                if (entry.IsAlive)
                {
                    return (T)entry.Target;
                }
                else
                {
                    hashTable.Remove(hash);
                }
            }

            T obj = create(rl);
            obj.Disposing += this.UnregisterObject;
            hashTable.Add(hash, new WeakReference(obj));
            return obj;
        }
        public void DestoryInstace(T obj)
        {
            hashTable.Remove(obj.HashCode);

            destroy(obj);
        }
        protected abstract T create(S rl);
        protected abstract void destroy(T obj);
    }

    public interface IUniqueObjManager<S, T>
    {
        T CreateInstance(S rl);
        void DestoryInstace(T obj);

        T Exists(int hashcode);
    }
}
