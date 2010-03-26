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
using R3D.ConfigModel;

namespace R3D.Sound
{
    public enum EvaTpye
    {
        None,
        Queue,
    }

    public class EvaVoice : SoundBase, IConfigurable
    {
        EVAVoicesFactory factory;

        FMOD.System soundSystem;

        public EvaVoice(EVAVoicesFactory fac, string name)
            : base(fac.SoundMgr, name)
        {
            factory = fac;
            soundSystem = fac.SoundSystem;
        }


        public string Text
        {
            get;
            protected set;
        }

        string soundFile;
        string SoundFile
        {
            get { return soundFile; }
            set { soundFile = value; }
        }

        ResourceLocation soundLocation;
        bool isUnloadable;
        int sizeInBytes;
        FMOD.CHANNEL_CALLBACK stopHandler;
        FMOD.Sound fmodSound;

        FMOD.RESULT SoundEnd(IntPtr channelraw, FMOD.CHANNEL_CALLBACKTYPE type, int command, uint commanddata1, uint commanddata2)
        {
            isUnloadable = true;
            FMOD.Channel ch = new FMOD.Channel();
            ch.setRaw(channelraw);
            FMOD.RESULT res = ch.setCallback(FMOD.CHANNEL_CALLBACKTYPE.END, null, 0);

            //tracker.RemoveAt();
            //stopHandler = null;
            return res;
        }


        public override void LocateSounds(string[] sounds, params object[] para)
        {
            soundLocation = FileSystem.Instance.TryLocate(FileSystem.Audio_Mix + SoundFile, FileSystem.GameLangLR);

            if (soundLocation != null)
            {
                sizeInBytes = soundLocation.Size;
            }
            else
            {
                GameConsole.Instance.Write(ResourceAssembly.Instance.CM_SoundNotFound(SoundFile), ConsoleMessageType.Warning);
            }
        }

        public override void Play()
        {
            if (soundLocation != null)
            {
                isUnloadable = false;
                Use();

                FMOD.Channel ch = null;
                soundSystem.playSound(FMOD.CHANNELINDEX.FREE, fmodSound, false, ref ch);
                stopHandler = SoundEnd;
                ch.setCallback(FMOD.CHANNEL_CALLBACKTYPE.END, stopHandler, 0);
                //tracker.Add(stopHandler.GetHashCode(), stopHandler);
                ch.setVolume(1f);
            }
        }

        public override int GeiSize()
        {
            return sizeInBytes;
        }

        protected override void load()
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(soundLocation);
            byte[] data = br.ReadBytes(soundLocation.Size);
            br.Close();

            FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
            exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
            exinfo.length = (uint)data.Length;

            soundSystem.createSound(data, FMOD.MODE.CREATESAMPLE | FMOD.MODE.LOWMEM | FMOD.MODE._2D | FMOD.MODE.DEFAULT | FMOD.MODE.SOFTWARE | FMOD.MODE.OPENMEMORY, ref exinfo, ref fmodSound);
  
        }

        protected override void unload()
        {
            if (fmodSound != null)
            {
                fmodSound.release();
            }
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            Text = sect.GetString("Text", string.Empty);

            Priority = SoundManager.GetSoundPriority(sect.GetString("Priority", "NORMAL"));

            int sideId = factory.CurrentSideId;

            if (sideId > 2)
            {
                SoundFile = sect["Side" + (sideId + 1).ToString()] + FileSystem.dotWav;
            }
            else
            {
                string sideAlias = "Allied";
                switch (sideId)
                {
                    case 1:
                        sideAlias = "Russian";
                        break;
                    case 2:
                        sideAlias = "Yuri";
                        break;
                }

                if (!sect.TryGetValue(sideAlias, out soundFile))
                {
                    soundFile = sect["Side" + (sideId + 1).ToString()];
                }
                soundFile += FileSystem.dotWav;
            }
        }

        #endregion

        public override bool IsUnloadable
        {
            get
            {
                return isUnloadable;
            }
        }
    }
}
