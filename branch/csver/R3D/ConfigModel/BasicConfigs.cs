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
using System.Drawing;
using System.IO;
using System.Text;
using R3D.ConfigModel;
using R3D.IO;
using R3D.ScriptEngine;

namespace R3D.ConfigModel
{
    public abstract class BasicConfigFactory
    {
        public abstract BasicConfigBase CreateInstance(ConfigurationSection sect);

        public abstract string Name { get; }
    }

    public sealed class AudioConfigFacotry : BasicConfigFactory
    {
        public static string AudioConfigName
        {
            get;
            private set;
        }

        static AudioConfigFacotry()
        {
            AudioConfigName = "Audio";
        }

        public override BasicConfigBase CreateInstance(ConfigurationSection sect)
        {
            return new AudioConfigs(sect);
        }

        public override string Name
        {
            get { return AudioConfigName; }
        }
    }
    public sealed class VideoConfigFactory : BasicConfigFactory
    {
        public static string VideoConfigName
        {
            get;
            private set;
        }

        static VideoConfigFactory()
        {
            VideoConfigName = "Video";
        }

        public override BasicConfigBase CreateInstance(ConfigurationSection sect)
        {
            return new VideoConfigs(sect);
        }

        public override string Name
        {
            get { return VideoConfigName; }
        }
    }
    public sealed class SkirmishConfigFactory : BasicConfigFactory
    {
        public static string SkirmishConfigName
        {
            get;
            private set;
        }

        static SkirmishConfigFactory()
        {
            SkirmishConfigName = "Skirmish";
        }

        public override BasicConfigBase CreateInstance(ConfigurationSection sect)
        {
            return new SkirmishConfigs(sect);
        }

        public override string Name
        {
            get { return SkirmishConfigName; }
        }
    }
    public abstract class BasicConfigBase
    {
        //public abstract void Write(StreamWriter sw);
        public abstract void Write(ConfigurationSection sect);

        public abstract string Name
        {
            get;
        }
    }

    public sealed class AudioConfigs : BasicConfigBase
    {
        static readonly string SoundVolumeKey = "SoundVolume";
        static readonly string VoiceVolumeKey = "VoiceVolume";
        static readonly string ScoreVolumeKey = "ScoreVolume";
        static readonly string InGameMusicKey = "InGameMusic";
        static readonly string IsScoreRepeatKey = "IsScoreRepeat";
        static readonly string IsScoreShuffleKey = "IsScoreShuffle";


        public AudioConfigs(ConfigurationSection sect)
        {
            SoundVolume = sect.GetFloat(SoundVolumeKey, 0.7f);
            VoiceVolume = sect.GetFloat(VoiceVolumeKey, 0.7f);
            ScoreVolume = sect.GetFloat(ScoreVolumeKey, 0.3f);

            InGameMusic = sect.GetBool(InGameMusicKey, true);
            IsScoreRepeat = sect.GetBool(IsScoreRepeatKey, false);
            IsScoreShuffle = sect.GetBool(IsScoreShuffleKey, true);

        }

        public override string Name
        {
            get { return AudioConfigFacotry.AudioConfigName; }
        }

        public float SoundVolume
        {
            get;
            set;
        }
        public float VoiceVolume
        {
            get;
            set;
        }
        public float ScoreVolume
        {
            get;
            set;
        }
        public bool IsScoreRepeat
        {
            get;
            set;
        }
        public bool IsScoreShuffle
        {
            get;
            set;
        }
        public bool InGameMusic
        {
            get;
            set;
        }

        public override void Write(ConfigurationSection sect)
        {
            sect.Add(SoundVolumeKey, SoundVolume.ToString("#.######"));
            sect.Add(VoiceVolumeKey, VoiceVolume.ToString("#.######"));
            sect.Add(ScoreVolumeKey, ScoreVolume.ToString("#.######"));
            sect.Add(IsScoreRepeatKey, IsScoreRepeat.ToString());
            sect.Add(IsScoreShuffleKey, IsScoreShuffle.ToString());
            sect.Add(InGameMusicKey, InGameMusic.ToString());

        }

    }
    public sealed class VideoConfigs : BasicConfigBase
    {
        static readonly string ScreenHeightKey = "ScreenHeight";
        static readonly string ScreenWidthKey = "ScreenWidth";
        static readonly string StretchMoviestKey = "StretchMovies";
        static readonly string FullScreenKey = "FullScreen";

        public VideoConfigs(ConfigurationSection sect)
        {
            try
            {
                ScreenHeight = int.Parse(sect[ScreenHeightKey]);
                ScreenWidth = int.Parse(sect[ScreenWidthKey]);
            }
            catch (KeyNotFoundException)
            {
                GameConsole.Instance.Write(ResourceAssembly.Instance.CM_InvaildResol(Game.Suffix), ConsoleMessageType.Warning);
                ScreenWidth = 800;
                ScreenHeight = 600;
            }

            StretchMovies = sect.GetBool(StretchMoviestKey, true);
            FullScreen = sect.GetBool(FullScreenKey, true);
        }

        public override string Name
        {
            get { return VideoConfigFactory.VideoConfigName; }
        }


        public int ScreenWidth
        {
            get;
            set;
        }
        public int ScreenHeight
        {
            get;
            set;
        }
        public bool StretchMovies
        {
            get;
            set;
        }
        public bool FullScreen
        {
            get;
            set;
        }


        public override void Write(ConfigurationSection sect)
        {
            sect.Add(ScreenHeightKey, ScreenHeight.ToString());
            sect.Add(ScreenWidthKey, ScreenWidth.ToString());
            sect.Add(StretchMoviestKey, StretchMovies.ToString());
            sect.Add(FullScreenKey, FullScreen.ToString());

        }
    }

    public sealed class SkirmishConfigs : BasicConfigBase
    {
        public enum PlayerType : int
        {
            Closed = 1,
            Human = 2,
            Observer = 3,
            HardAI = 4,
            MediumAI = 5,
            EasyAI = 6
        }
        static readonly string MultiEngineerKey = "MultiEngineer";
        static readonly string TechLevelKey = "TechLevel";
        static readonly string GameModeKey = "GameMode";
        static readonly string ScenIndexKey = "ScenIndex";
        static readonly string GameSpeedKey = "GameSpeed";
        static readonly string CreditsKey = "Credits";
        static readonly string UnitCountKey = "UnitCount";
        static readonly string ShortGameKey = "ShortGame";
        static readonly string SuperWeaponsAllowedKey = "SuperWeaponsAllowed";
        static readonly string BuildOffAllyKey = "BuildOffAlly";
        static readonly string MCVRepacksKey = "MCVRepacks";
        static readonly string CratesAppearKey = "CratesAppear";

        static readonly string SlotKey = "Slot0";

        PlayerType[] playerTypes;
        int[] playerCountries;
        int[] playerColors;

        #region Setttings
        public bool MultiEngineer
        {
            get;
            set;
        }
        public int TechLevel
        {
            get;
            set;
        }
        public int GameMode
        {
            get;
            set;
        }

        public int ScenIndex
        {
            get;
            set;
        }

        public int GameSpeed
        {
            get;
            set;
        }

        public int Credits
        {
            get;
            set;
        }

        public int UnitCount
        {
            get;
            set;
        }
        public bool ShortGame
        {
            get;
            set;
        }
        public bool SuperWeaponsAllowed
        {
            get;
            set;
        }
        public bool BuildOffAlly
        {
            get;
            set;
        }
        public bool MCVRepacks
        {
            get;
            set;
        }

        public bool CratesAppear
        {
            get;
            set;
        }

        public PlayerType GetPlayerType(int index)
        {
            return playerTypes[index];
        }
        public int GetPlayerCountry(int index)
        {
            return playerCountries[index];
        }
        public int GetPlayerColor(int index)
        {
            return playerColors[index];
        }

        public void SetPlayerType(int index, PlayerType plt)
        {
            playerTypes[index] = plt;
        }
        public void SetPlayerCountry(int index, int cidx)
        {
            playerCountries[index] = cidx;
        }
        public void SetPlayerColor(int index, int cidx)
        {
            playerColors[index] = cidx;
        }
        #endregion

        public SkirmishConfigs(ConfigurationSection sect)
        {
            playerTypes = new PlayerType[8];
            playerCountries = new int[8];
            playerColors = new int[8];

            MultiEngineer = sect.GetBool(MultiEngineerKey, false);
            TechLevel = sect.GetInt(TechLevelKey, 10);
            GameMode = sect.GetInt(GameModeKey, 1);
            ScenIndex = sect.GetInt(ScenIndexKey, 1);

            GameSpeed = sect.GetInt(GameSpeedKey, 1);
            Credits = sect.GetInt(CreditsKey, 10000);
            UnitCount = sect.GetInt(UnitCountKey, 10);
            ShortGame = sect.GetBool(ShortGameKey, true);
            SuperWeaponsAllowed = sect.GetBool(SuperWeaponsAllowedKey, true);
            BuildOffAlly = sect.GetBool(BuildOffAllyKey, true);
            MCVRepacks = sect.GetBool(MCVRepacksKey, true);
            CratesAppear = sect.GetBool(CratesAppearKey, true);

            for (int i = 0; i < 8; i++)
            {
                string keyword = SlotKey + i.ToString();
                string[] res = sect.GetStringArray(keyword, null);
                if (res != null)
                {
                    playerTypes[i] = (PlayerType)int.Parse(res[0]);
                    playerCountries[i] = int.Parse(res[1]);
                    playerColors[i] = int.Parse(res[2]);
                }
                else
                {
                    playerTypes[i] = PlayerType.Closed;
                    playerCountries[i] = -2;
                    playerColors[i] = -2;
                }
            }
        }

        public override string Name
        {
            get { return SkirmishConfigFactory.SkirmishConfigName; }
        }
        public override void Write(ConfigurationSection sect)
        {
            sect.Add(MultiEngineerKey, MultiEngineer.ToString());
            sect.Add(TechLevelKey, TechLevel.ToString());
            sect.Add(GameModeKey, GameMode.ToString());
            sect.Add(ScenIndexKey, ScenIndex.ToString());

            sect.Add(GameSpeedKey, GameSpeed.ToString());
            sect.Add(CreditsKey, Credits.ToString());
            sect.Add(UnitCountKey, UnitCount.ToString());
            sect.Add(ShortGameKey, ShortGame.ToString());
            sect.Add(SuperWeaponsAllowedKey, SuperWeaponsAllowed.ToString());
            sect.Add(BuildOffAllyKey, BuildOffAlly.ToString());
            sect.Add(MCVRepacksKey, MCVRepacks.ToString());
            sect.Add(MCVRepacksKey, CratesAppear.ToString());


            for (int i = 0; i < 8; i++)
            {
                string keyword = SlotKey + i.ToString();

                sect.Add(keyword, ((int)playerTypes[i]).ToString() + ", " + playerCountries[i].ToString() + ", " + playerColors[i].ToString());
            }
        }
    }

    /// <summary>
    /// ¥Ê¥¢Ra2.ini÷–µƒ…Ë÷√
    /// </summary>
    public class BasicConfigs
    {
        static BasicConfigs singleton;

        public static BasicConfigs Instance
        {
            get { return singleton; }
        }

        public static void Initialize()
        {
            singleton = new BasicConfigs();
        }


        Dictionary<string, BasicConfigBase> configs;

        Dictionary<string, BasicConfigFactory> configTypes;

        string ra2IniPath;

        private BasicConfigs()
        {
            configTypes = new Dictionary<string, BasicConfigFactory>(20);
        }


        public void RegisterConfigType(string name, BasicConfigFactory fac)
        {
            configTypes.Add(name, fac);
        }
        public void UnregisterConfigType(string name)
        {
            configTypes.Remove(name);
        }

        public void Load()
        {
            FileLocation fileLoc = FileSystem.Instance.Locate(FileSystem.Ra2_Ini, FileLocateRule.Root);

            Dictionary<string, ConfigurationSection> content;
            try
            {
                content = ConfigurationManager.Instance.CreateInstance(fileLoc);// new IniConfiguration(fileLoc);
            }
            catch (FileNotFoundException)
            {
                GameConsole.Instance.Write(ResourceAssembly.Instance.CM_FileNotFonud(FileSystem.Ra2_Ini) + ResourceAssembly.Instance.CM_UseDefaultSet, ConsoleMessageType.Warning);
                content = new Dictionary<string, ConfigurationSection>();
                ////ini = new IniFile(fileLoc);
                //content.Add(AudioConfigName, new IniSection(AudioConfigName));
                //content.Add(VideoConfigName, new IniSection(VideoConfigName));
                //content.Add(SkirmishConfigName, new IniSection(SkirmishConfigName));
            }

            ra2IniPath = fileLoc.Path;
            configs = new Dictionary<string, BasicConfigBase>();

            foreach (KeyValuePair<string, BasicConfigFactory> e in configTypes)
            {
                ConfigurationSection sect;
                if (!content.TryGetValue(e.Key, out sect))
                {
                    sect = new IniSection(e.Key);
                }

                configs.Add(e.Key, e.Value.CreateInstance(sect));
            }

            //ConfigurationSection sect = content[AudioConfigName];
            //AudioConfigs audioCons = new AudioConfigs(sect);

            //sect = content[VideoConfigName];
            //VideoConfigs videoCons = new VideoConfigs(sect);


            //sect = content[SkirmishConfigName];
            //SkirmishConfigs skirCons = new SkirmishConfigs(sect);


            //configs = new Dictionary<string, BasicConfigBase>(CaseInsensitiveStringComparer.Instance);
            //configs.Add(AudioConfigName, audioCons);
            //configs.Add(VideoConfigName, videoCons);
            //configs.Add(SkirmishConfigName, skirCons);

        }
        public void Save()
        {
            //FileStream fstm = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            //fstm.SetLength(0);
            //StreamWriter sw = new StreamWriter(fstm, Encoding.Default);

            //sw.WriteLine("[Intro]");
            //sw.WriteLine("Play = no");
            //sw.WriteLine();

            //for (int i = 0; i < configs.Count; i++)
            //{
            //    configs[i].Write(sw);
            //}

            //sw.Close();
            IniConfiguration ra2ini = new IniConfiguration(string.Empty, configs.Count);

            foreach (KeyValuePair<string, BasicConfigBase> e in configs)
            {
                IniSection sect = new IniSection(e.Key);

                e.Value.Write(sect);
            }

            ra2ini.Save(ra2IniPath);
        }

        public BasicConfigBase this[string name]
        {
            get { return configs[name]; }
        }

    }
}
