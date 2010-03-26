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
    public interface ISoundsFactory
    {
        Dictionary<string, SoundBase> CreateInstance();
        string Type { get; }
    }

    public class SoundsFactory : ISoundsFactory
    {
        static readonly char[] SpaceArray = new char[] { ' ' };

        //AudioBag audioBag;
        FMOD.System sndSystem;
        SoundManager manager;

        public SoundsFactory(SoundManager mgr, FMOD.System ss)
        {
            sndSystem = ss;
            manager = mgr;
        }

        public FMOD.System SoundSystem
        {
            get { return sndSystem; }
        }

        #region ISoundFactory 成员
        public Dictionary<string, SoundBase> CreateInstance()
        {
            Dictionary<string, SoundBase> data = new Dictionary<string, SoundBase>();

            AudioBag audioIdx = AudioBag.FromFile(FileSystem.Instance.Locate(FileSystem.Audio_Idx, FileSystem.GameLangLR));
            ResourceLocation audioBag = FileSystem.Instance.Locate(FileSystem.Audio_Bag, FileSystem.GameLangLR);

            Configuration ini = ConfigurationManager.Instance.CreateInstance(FileSystem.Instance.Locate(FileSystem.Sound_Ini, FileSystem.GameResLR));

            ConfigurationSection sect = ini["SoundList"];

            ConfigurationSection.ValueCollection vals = sect.Values;
            foreach (string name in vals)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    ConfigurationSection sndSect = ini[name];

                    Sfx snd = new Sfx(manager, sndSystem, name);

                    //snd.Delay = sndSect.GetInt("Delay", 50);
                    string tmp;
                    if (sndSect.TryGetValue("Delay", out tmp))
                    {
                        string[] v = tmp.Split(SpaceArray, StringSplitOptions.RemoveEmptyEntries);
                        if (v.Length == 1)
                        {
                            snd.MinDelay = int.Parse(v[0]);
                            snd.MaxDelay = snd.MinDelay;
                        }
                        else
                        {
                            snd.MinDelay = int.Parse(v[0]);
                            snd.MaxDelay = int.Parse(v[1]);
                        }
                    }

                    snd.Limit = sndSect.GetInt("Limit", 5);
                    snd.Volume = sndSect.GetInt("Volume", 80);
                    snd.Range = sndSect.GetInt("Range", 10);
                    snd.MinVolume = sndSect.GetInt("MinVolume", 50);

                    SoundType stype = SoundType.Normal;

                    if (sndSect.TryGetValue("Type", out tmp))
                    {
                        string[] type = tmp.Split(SpaceArray, StringSplitOptions.RemoveEmptyEntries);
                        for (int j = 0; j < type.Length; j++)
                        {
                            switch (type[j].ToUpper())
                            {
                                case "NORMAL":
                                    stype |= SoundType.Normal;
                                    break;
                                case "SHROUD":
                                    stype |= SoundType.Shroud;
                                    break;
                                case "UNSHROUD":
                                    stype |= SoundType.Unshroud;
                                    break;
                                case "PLAYER":
                                    stype |= SoundType.Player;
                                    break;
                                case "LOCAL":
                                    stype |= SoundType.Local;
                                    break;
                                case "SCREEN":
                                    stype |= SoundType.Screen;
                                    break;
                                case "GLOBAL":
                                    stype |= SoundType.Global;
                                    break;
                            }
                        }
                        stype |= SoundType.Normal | SoundType.Screen | SoundType.Unshroud;
                    }
                    else
                    {
                        stype = SoundType.Normal | SoundType.Screen | SoundType.Unshroud;
                    }

                    snd.Type = stype;

                    //SoundPriority sp = SoundPriority.Normal;
                    //string pri = sndSect.GetString("Priority", "NORMAL");
                    //switch (pri.ToUpper())
                    //{
                    //    case "NORMAL":
                    //        sp = SoundPriority.Normal;
                    //        break;
                    //    case "LOW":
                    //        sp = SoundPriority.Low;
                    //        break;
                    //    case "HIGH":
                    //        sp = SoundPriority.High;
                    //        break;
                    //    case "CRITICAL":
                    //        sp = SoundPriority.Critical;
                    //        break;
                    //}
                    snd.Priority = SoundManager.GetSoundPriority(sndSect.GetString("Priority", "NORMAL"));// sp;

                    SoundControl sc = SoundControl.None;
                    if (sndSect.TryGetValue("Control", out tmp))
                    {
                        string ctrl = tmp.Trim();
                        switch (ctrl.ToUpper())
                        {
                            case "LOOP":
                                sc |= SoundControl.Loop;
                                break;
                            case "PREDELAY":
                                sc |= SoundControl.Predelay;
                                break;
                            case "RANDOM":
                                sc |= SoundControl.Random;
                                break;
                            case "ALL":
                                sc |= SoundControl.All;
                                break;
                            case "DECAY":
                                sc |= SoundControl.Decay;
                                break;
                            case "AMBIENT":
                                sc |= SoundControl.Ambient;
                                break;
                            case "ATTACH":
                                sc |= SoundControl.Attach;
                                break;
                        }
                    }


                    snd.Control = sc;

                    string sounds;
                    if (sndSect.TryGetValue("Sounds", out sounds))
                    {
                        snd.LocateSounds(sounds.Trim().Split(SpaceArray, StringSplitOptions.RemoveEmptyEntries), audioIdx, audioBag);
                    }


                    try
                    {
                        data.Add(name, snd);
                    }
                    catch (ArgumentException ex)
                    {
                        GameConsole.Instance.Write("[Sound] " + ex.Message + name, ConsoleMessageType.Warning);
                    }
                }

            }

            return data;
        }

        public string Type
        {
            get { return "Ra2 Sound(Old)"; }
        }

        #endregion
    }
}
