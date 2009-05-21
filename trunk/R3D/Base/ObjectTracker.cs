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
using R3D.IO;

namespace R3D.Base
{
    public abstract class ObjectTracker<T> where T : class
    {
        protected class InstanceEntry : IDisposable
        {
            string name;

            WeakReference target;

            public InstanceEntry(string name, T obj)
            {
                this.name = name;
                this.target = new WeakReference(obj);
            }

            public string Name
            {
                get { return name; }
            }

            public T Object
            {
                get { return (T)target.Target; }
            }
            public object RawObject
            {
                get { return target.Target; }
            }
            public bool IsAlive
            {
                get { return target.IsAlive; }
            }

            #region IDisposable 成员

            public void Dispose()
            {
                target = null;
            }

            #endregion
        }

        protected Dictionary<string, InstanceEntry> instances = new Dictionary<string, InstanceEntry>(CaseInsensitiveStringComparer.Instance);
        protected Dictionary<int, InstanceEntry> instancesByHash = new Dictionary<int, InstanceEntry>();

        protected abstract T create(ResourceLocation rl);

        protected abstract void destroy(T obj);

        public T CreateInstance(ResourceLocation rl)
        {
            InstanceEntry entry;
            if (instances.TryGetValue(rl.Name, out entry))
            {
                if (entry.IsAlive)
                {
                    return entry.Object;
                }
                else
                {
                    instances.Remove(entry.Name);
                    entry.Dispose();
                }
            }

            T obj = create(rl);
            InstanceEntry inst = new InstanceEntry(rl.Name, obj);
            instances.Add(rl.Name, inst);
            instancesByHash.Add(obj.GetHashCode(), inst);
            return obj;
        }
        public void DestoryInstance(T obj)
        {
            InstanceEntry entry = null;

            int hash = obj.GetHashCode();
            if (instancesByHash.TryGetValue(hash, out entry))
            {
                instances.Remove(entry.Name);
                instancesByHash.Remove(hash);
                entry.Dispose();
            }
            destroy(obj);
        }


    }

}
