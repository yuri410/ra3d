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
using R3D.ConfigModel;

namespace R3D.Sound
{
    // 播放时再设置声音参数
    public class SoundManager : CachedObjectManager<SoundBase>, IDisposable
    {
        Dictionary<string, SoundBase> sounds;

        ISoundsFactory factory;

        AudioConfigs audioCons;

        public float SoundVolume
        {
            get { return audioCons.SoundVolume; }
            set { audioCons.SoundVolume = value; }
        }

        public float VoiceVolume
        {
            get { return audioCons.VoiceVolume; }
            set { audioCons.VoiceVolume = value; }
        }

        public SoundManager(AudioConfigs ac)
        {
            audioCons = ac;

            sounds = new Dictionary<string, SoundBase>();
        }

        public ISoundsFactory Creator
        {
            get { return factory; }
            set { factory = value; }
        }

        public void Load()
        {
            sounds = factory.CreateInstance();
        }


        public SoundBase this[string name]
        {
            get
            {
                SoundBase res;
                if (sounds.TryGetValue(name, out res))
                {
                    return res;
                }
                GameConsole.Instance.Write(ResourceAssembly.Instance.CM_SoundNotFound(name), ConsoleMessageType.Exclamation);
                return DummySound.Instance;
            }
        }

        public static SoundPriority GetSoundPriority(string value)
        {
            SoundPriority sp = SoundPriority.Normal;
            switch (value.ToUpper())
            {
                case "NORMAL":
                    sp = SoundPriority.Normal;
                    break;
                case "LOW":
                    sp = SoundPriority.Low;
                    break;
                case "HIGH":
                    sp = SoundPriority.High;
                    break;
                case "CRITICAL":
                    sp = SoundPriority.Critical;
                    break;
            }
            return sp;
        }

        public bool Disposed
        {
            get;
            protected set;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!Disposed)
            {
                base.ClearBuffer();

                Disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion
    }
}
