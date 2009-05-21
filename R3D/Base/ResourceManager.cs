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
    public abstract class ResourceManager<T> : IResourceManager where T : Resource
    {
        Dictionary<int, T> hashTable;// = new Dictionary<int, WeakReference>();
        List<T> objects;

        int totalCacheSize;
        int curCacheSize;

        int manageFrequency;
        int manageTimes;

        protected ResourceManager()
        {
            objects = new List<T>();
            hashTable = new Dictionary<int, T>();
            totalCacheSize = 8 * 1048576;  // 8mb
            manageFrequency = 4;
        }
        protected ResourceManager(int cacheSize)
        {
            objects = new List<T>();
            hashTable = new Dictionary<int, T>();
            totalCacheSize = cacheSize;
            manageFrequency = 4;
        }
        protected ResourceManager(int cacheSize, int manageFreq)
        {
            objects = new List<T>();
            hashTable = new Dictionary<int, T>();
            totalCacheSize = cacheSize;
            manageFrequency = manageFreq;
        }

        public int TotalCacheSize
        {
            get { return totalCacheSize; }
            set { totalCacheSize = value; }
        }
        public int CurrentCacheSize
        {
            get { return curCacheSize; }
        }

        #region ICachedObjectManager 成员

        public void Manage()
        {
            manageTimes++;

            if (manageTimes >= manageFrequency)
            {
                manageTimes = 0;
                if (curCacheSize > totalCacheSize)
                {
                    objects.Sort();

                    int oc = objects.Count;
                    int k = oc - 1;
                    while (curCacheSize > totalCacheSize && k >= 0)
                    {
                        if (objects[k].State == ResourceState.Loaded && objects[k].IsUnloadable)
                        {
                            objects[k].Unload();
                        }
                        k--;
                    }
                }
            }
        }
        int ICachedObjectManager.CurrentCacheSize
        {
            get { return curCacheSize; }
            set { curCacheSize = value; }
        }

        #endregion

        void UnregisterObject(Resource obj)
        {
            hashTable.Remove(obj.HashCode);
        }

        protected abstract T create(ResourceLocation rl);
        protected abstract void destroy(T obj);

        public Resource CreateInstance(ResourceLocation rl)
        {
            T entry;
            int hash = rl.GetHashCode();

            if (hashTable.TryGetValue(hash, out entry))
            {
                return entry;
            }

            T obj = create(rl);
            obj.Disposing += this.UnregisterObject;
            hashTable.Add(hash, obj);
            return obj;
        }

        public void DestoryInstace(T obj)
        {
            hashTable.Remove(obj.HashCode);
            destroy(obj);
        }

        public Resource Exists(int hashcode)
        {
            T res;
            if (hashTable.TryGetValue(hashcode, out res))
            {
                return res;
            }
            return null;
        }

        #region IUniqueObjManager<Resource> 成员

        Resource IUniqueObjManager<ResourceLocation, Resource>.CreateInstance(ResourceLocation rl)
        {
            T entry;
            int hash = rl.GetHashCode();

            if (hashTable.TryGetValue(hash, out entry))
            {
                return entry;
            }

            T obj = create(rl);
            obj.Disposing += this.UnregisterObject;
            hashTable.Add(hash, obj);
            return obj;
        }

        void IUniqueObjManager<ResourceLocation, Resource>.DestoryInstace(Resource obj)
        {
            hashTable.Remove(obj.HashCode);
            destroy((T)obj);
        }

        Resource IUniqueObjManager<ResourceLocation, Resource>.Exists(int hashcode)
        {
            T res;
            if (hashTable.TryGetValue(hashcode, out res))
            {
                return res;
            }
            return null;
        }

        #endregion
    }

    public interface IResourceManager : ICachedObjectManager, IUniqueObjManager<ResourceLocation, Resource>
    { }
}
