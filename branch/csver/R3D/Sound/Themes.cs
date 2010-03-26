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
using System.IO;

using R3D.IO;
using R3D.ScriptEngine;
using R3D.Base;
using R3D.ConfigModel;

namespace R3D.Sound
{
    public sealed class Themes : ScriptableObject, IDisposable
    {
        public sealed class Track : IDisposable
        {
            string name;
            FMOD.Sound sound;
            FMOD.Channel chanel;
            FMOD.System sndSystem;

            bool normal;
            bool repeat;

            bool isPlaying;
            bool disposed;

            Themes themes;

            public Track(Themes thms, FMOD.System sndSys, ConfigurationSection sect)
            {
                themes = thms;
                sndSystem = sndSys;

                name = sect.GetUIString("Name", "GUI:Blank");

                normal = sect.GetBool("Normal", true);
                repeat = sect.GetBool("Repeat", false);

                string[] fn = sect.GetPaths("Sound");

                // 兼容性考虑
                if (fn.Length == 1 && !Path.HasExtension(fn[0]))
                {
                    fn[0] = fn[0] + FileSystem.dotWav;
                    if (!fn[0].Contains("<") || !fn[0].Contains(">"))
                        fn[0] = FileSystem.Theme_Mix + fn[0];
                }

                FileLocation fileLoc = FileSystem.Instance.Locate(fn, FileSystem.GameThemeLR);

                FMOD.CREATESOUNDEXINFO sndInfo = new FMOD.CREATESOUNDEXINFO();
                sndInfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(sndInfo);
                sndInfo.fileoffset = (uint)fileLoc.Offset;
                sndInfo.length = (uint)fileLoc.Size;


                FMOD.MODE mode = FMOD.MODE._2D | FMOD.MODE.DEFAULT | FMOD.MODE.HARDWARE | FMOD.MODE.CREATESTREAM;

                if (repeat)
                    mode |= FMOD.MODE.LOOP_NORMAL;

                if (!fileLoc.IsInArchive)
                    sndSys.createSound(fileLoc.Path, mode, ref sound);
                else
                {
                    //string fp = fileLoc.Path;                    
                    //sndSys.createSound(fp.Substring(0, fp.LastIndexOf(Path.DirectorySeparatorChar)), mode, ref sndInfo, ref sound);
                    sndSys.createSound(FileSystem.GetArchivePath(fileLoc.Path), mode, ref sndInfo, ref sound);
                }
            }

            public void Play()
            {
                sndSystem.playSound(FMOD.CHANNELINDEX.REUSE, sound, true, ref chanel);
                chanel.setVolume(themes.audioCons.ScoreVolume);
                //chanel.setDelay(FMOD.DELAYTYPE.MAX, 1000, 0);
                chanel.setPaused(false);
                isPlaying = true;
            }

            public void Stop()
            {
                if (isPlaying)
                {
                    chanel.stop();
                    isPlaying = false;
                }
            }

            public TimeSpan Length
            {
                get
                {
                    uint l = 0;
                    sound.getLength(ref l, FMOD.TIMEUNIT.MS);
                    return new TimeSpan(l);
                }
            }

            public string Name
            {
                get { return name; }
            }


            #region IDisposable 成员

            public void Dispose()
            {
                if (!disposed)
                {
                    sound.release();
                    sndSystem = null;
                    themes = null;
                    disposed = true;
                    //GC.SuppressFinalize(this);
                }
                else
                    throw new ObjectDisposedException(this.ToString());
            }

            #endregion

            ~Track()
            {
                if (!disposed)
                    Dispose();
            }
        }

        List<Track> tracks;

        Track intro;
        AudioConfigs audioCons;
        FMOD.System sndSys;

        public Themes(FMOD.System sndSys ,AudioConfigs aus)
            : base("Themes")
        {
            this.sndSys = sndSys;
            audioCons = aus;
        }

        void ObjectLoadImpl(object sender)
        {
            ResourceLocation fl = FileSystem.Instance.Locate(FileSystem.Theme_Ini, FileSystem.GameResLR);
            IniConfiguration ini = new IniConfiguration(fl);

            ConfigurationSection sect = ini["Themes"];

            tracks = new List<Track>(sect.Count);

            ConfigurationSection.ValueCollection vals = sect.Values;
            foreach (string sn in vals)
            {
                if (sn.Length > 0)
                {
                    ConfigurationSection tsect;
                    if (ini.TryGetValue(sn, out tsect))
                    {
                        Track tck = new Track(this, sndSys, tsect);
                        if (sn.ToUpper() == "INTRO")
                            intro = tck;
                        tracks.Add(tck);
                    }
                }
            }

            tracks.TrimExcess();
        }


        public List<KeyValuePair<string, string>> GetTrackInformation()
        {
            List<KeyValuePair<string, string>> res = new List<KeyValuePair<string, string>>(tracks.Count);

            for (int i = 0; i < tracks.Count; i++)
                res.Add(new KeyValuePair<string, string>(tracks[i].Name, tracks[i].Length.ToString()));
            return res;
        }



        public void PlayIntro()
        {
            if (!IsLoaded)            
            {
                OnLoad();
            }
            intro.Play();
        }
        public void StopIntro()
        {
            if (!IsLoaded)
            {
                OnLoad();
            }
            intro.Stop();
        }

        public void Play(int i)
        {
            Stop();
            tracks[i].Play();
        }

        public void Stop()
        {
            for (int i = 0; i < tracks.Count; i++)
                tracks[i].Stop();
        }

        public void Update(float dt)
        {
            if (!IsLoaded)
            {
                OnLoad();
            }
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                for (int i = 0; i < tracks.Count; i++)
                {
                    tracks[i].Stop();
                    tracks[i].Dispose();
                }
                tracks = null;
            }
        }


    }
}
