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
    public class CachedObjectManager<T> : ICachedObjectManager
        where T : CachedObject
    {
        List<T> objects;

        int totalCacheSize;
        int curCacheSize;

        int manageFrequency;
        int manageTimes;

        public CachedObjectManager()
        {
            objects = new List<T>();
            totalCacheSize = 8 * 1048576;  // 8mb
            manageFrequency = 4;
        }
        public CachedObjectManager(int cacheSize)
        {
            objects = new List<T>();
            totalCacheSize = cacheSize;
            manageFrequency = 4;
        }
        public CachedObjectManager(int cacheSize, int manageFreq)
        {
            objects = new List<T>();
            totalCacheSize = cacheSize;
            manageFrequency = manageFreq;
        }

        public void RegisterObject(T obj)
        {
            objects.Add(obj);
        }
        public void UnregisterObject(T obj)        
        {
            objects.Remove(obj);
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

        protected T GetObject(int index)
        {
            return objects[index];
        }
        protected int CurrentCachedObjectCount
        {
            get { return objects.Count; }
        }
        protected void ClearBuffer()
        {
            for (int k = 0; k < objects.Count; k++)
            {
                if (objects[k].State == ResourceState.Loaded && objects[k].IsUnloadable)
                {
                    objects[k].Unload();
                }
            }
            objects.Clear();
        }

        #region IResourceCacheManager 成员

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

    }

    public interface ICachedObjectManager
    {
        void Manage();
        int CurrentCacheSize { get; set; }
    }
}
