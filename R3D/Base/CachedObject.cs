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
    public enum ResourceState
    {
        /// Not loaded
        Unloaded,
        /// Loading is in progress
        Loading,
        /// Fully loaded
        Loaded,
        /// Currently unloading
        Unloading
    }
    public abstract class CachedObject : IComparable<CachedObject>
    {
        ResourceState resState;

        TimeSpan creationTime;
        TimeSpan lastUsed;
        int usedTimes;

        ICachedObjectManager resCM;

        float useFrequency;

        protected CachedObject(ICachedObjectManager rcm)
        {
            resCM = rcm;
        }

        public ResourceState State
        {
            get { return resState; }
        }
        public void Load()
        {
            creationTime = GameTimer.TimeSpan;
            lastUsed = creationTime;

            resCM.Manage();
            load();
            resCM.CurrentCacheSize += GeiSize();
            resState = ResourceState.Loaded;
        }
        public void Unload()
        {
            unload();
            resCM.CurrentCacheSize -= GeiSize();
            resState = ResourceState.Unloaded;
        }
        public void Use()
        {
            lastUsed = GameTimer.TimeSpan;
            usedTimes++;

            float seconds = (float)((lastUsed - creationTime).TotalSeconds);
            useFrequency = seconds < 1 ? float.MaxValue : ((float)usedTimes) / seconds;

            if (resState == ResourceState.Unloaded)
            {
                resCM.Manage();
                Load();
            }
        }

        public float UseFrequency
        {
            get { return useFrequency; }
        }
        public abstract int GeiSize();


        public TimeSpan CreationTime
        {
            get { return creationTime; }
        }
        public TimeSpan LastUsedTime
        {
            get { return lastUsed; }
        }
        protected abstract void load();
        protected abstract void unload();

        public virtual bool IsUnloadable
        {
            get { return true; }
        }

        #region IComparable<CachedObject> 成员

        public int CompareTo(CachedObject other)
        {
            return this.useFrequency.CompareTo(other.useFrequency);
        }

        #endregion
    }
}
