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
    public class EVAVoicesFactory : ISoundsFactory
    {

        public int CurrentSideId
        {
            get;
            set;
        }

        //public int SideCount
        //{
        //    get;
        //    protected set;
        //}
        public FMOD.System SoundSystem
        {
            get;
            protected set;
        }
        public SoundManager SoundMgr
        {
            get;
            protected set;
        }

        public EVAVoicesFactory(SoundManager mgr, FMOD.System ss, int sideId)
        {
            SoundMgr = mgr;
            CurrentSideId = sideId;
            SoundSystem = ss;

        }

        #region ISoundsFactory 成员

        public Dictionary<string, SoundBase> CreateInstance()
        {
            Dictionary<string, SoundBase> res = new Dictionary<string, SoundBase>();


            Configuration ini = ConfigurationManager.Instance.CreateInstance(FileSystem.Instance.Locate(FileSystem.Eva_Ini, FileSystem.GameResLR));

            ConfigurationSection sect = ini["DialogList"];

            ConfigurationSection.ValueCollection sectValues = sect.Values;
            
            foreach (string s in sectValues)
            {
                if (!string.IsNullOrEmpty(s))
                {
                    ConfigurationSection voiceSect = ini[s];

                    EvaVoice ev = new EvaVoice(this, s);
                    ev.Parse(voiceSect);

                    ev.LocateSounds(null);

                    res.Add(s, ev);
                }
            }


            return res;
        }

        public string Type
        {
            get { return "EVA Voice"; }
        }

        #endregion
    }
}
