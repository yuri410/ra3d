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

namespace R3D.ConfigModel
{
    /// <summary>
    /// Singleton class contains the game rule configs
    /// </summary>
    public class GameConfigs
    {
        static GameConfigs singleton;

        public static readonly string GeneralSectionName = "General";



        public static void Initialize()
        {
            singleton = new GameConfigs();
        }
        public static GameConfigs Instance
        {
            get
            {
                return singleton;
            }
        }


        public Configuration Rules
        {
            get;
            protected set;
        }
        public Configuration Art
        {
            get;
            protected set;
        }


        public Configuration CurrentRules
        {
            get;
            set;
        }

        private GameConfigs()
        {
            Rules = ConfigurationManager.Instance.CreateInstance(FileSystem.Instance.Locate(FileSystem.Rule_Ini, FileSystem.GameResLR));
            Art = ConfigurationManager.Instance.CreateInstance(FileSystem.Instance.Locate(FileSystem.Art_Ini, FileSystem.GameResLR));
        }
    }
}
