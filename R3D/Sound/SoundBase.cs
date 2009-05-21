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
using R3D.Base;


namespace R3D.Sound
{
    public enum SoundPriority
    {
        Low,
        Normal,
        High,
        Critical
    }
    [Flags()]
    public enum SoundControl
    {
        None = 0,
        Loop = 1 << 0,
        Predelay = 1 << 1,
        Random = 1 << 2,
        Attach = 1 << 3,
        Decay = 1 << 4,
        All = 1 << 5,
        Ambient = 1 << 6
    }
    [Flags()]
    public enum SoundType
    {
        Normal = 0,
        Shroud = 1 << 0,
        Unshroud = 1 << 1,
        Player = 1 << 2,
        Local = 1 << 3,
        Screen = 1 << 4,
        Global = 1 << 5
    }

    public delegate void SoundStoppedHandler(SoundBase snd);

    public abstract class SoundBase : CachedObject
    {
        //protected FileLocation[] sounds;


        protected string sndName;
        protected SoundPriority priority;

        protected SoundBase(ICachedObjectManager mgr, string name)
            : base(mgr)
        {
            sndName = name;
        }

        //public FileLocation[] Sounds
        //{
        //    get { return sounds; }
        //}
        public abstract void LocateSounds(string[] sounds, params object[] para);
        public string Name
        {
            get { return sndName; }
        }

        public SoundPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }


        public abstract void Play();

        public event SoundStoppedHandler SoundStopped;

        protected void OnSoundStopped()
        {
            if (SoundStopped != null)
            {
                SoundStopped(this);
            }
        }
    }
}
