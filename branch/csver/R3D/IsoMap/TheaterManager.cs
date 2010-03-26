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

namespace R3D.IsoMap
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 不是抽象层
    /// </remarks>
    public class TheaterManager
    {
        private TheaterManager()
        {
            factories = new Dictionary<string, ITheaterFactory>(CaseInsensitiveStringComparer.Instance);
        }
        static TheaterManager singleton;
        public static TheaterManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new TheaterManager();
                return singleton;
            }
        }

        Dictionary<string, ITheaterFactory> factories;

        public void RegisterTheaterType(ITheaterFactory fac)
        {
            factories.Add(fac.Type, fac);
        }
        public void UnregisterTheaterType(ITheaterFactory fac)
        {
            factories.Remove(fac.Type);
        }

        string FindTypeToken(FileLocation fl)
        {
            ConfigModel.Configuration ini = ConfigurationManager.Instance.CreateInstance(fl);// new IniConfiguration(fl);

            ConfigurationSection general = ini["General"];
            return general["TheaterType"];
        }
        string FindTypeToken(string name)
        {
            string iniFile = name + Game.Suffix + FileSystem.dotIni;
            FileLocation fl = FileSystem.Instance.Locate(FileSystem.LocalMix + iniFile, FileSystem.GameResLR);
            return FindTypeToken(fl);
        }

        public TheaterBase CreateInstance(FileLocation fl)
        {
            ITheaterFactory fac;
            string token = FindTypeToken(fl);
            if (factories.TryGetValue(token, out fac))
            {
                return fac.CreateInstance(fl);
            }
            else
            {
                throw new NotSupportedException(fl.Path);
            }
        }
        public TheaterBase CreateInstance(string name)
        {
            ITheaterFactory fac;
            string token = FindTypeToken(name);
            if (factories.TryGetValue(token, out fac))
            {
                return fac.CreateInstance(name);
            }
            else
            {
                throw new NotSupportedException(token);
            }
        }
    }
}
