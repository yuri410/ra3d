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
using System.IO;
using System.Text;
using System.Windows.Forms;
using R3D;
using R3D.ConfigModel;
using R3D.IO;
using R3D.IsoMap;
using R3D.Logic;
using R3D.ScriptEngine;
using R3D.UI;

namespace Expandmd
{
    /// <summary>
    ///  this should be loaded
    /// </summary>
    public class MapList : List<MapLocationInfo>
    {
        static MapList singleton;

        public static MapList Instance
        {
            get { return singleton; }
        }

        public static bool Initialized
        {
            get { return singleton != null; }
        }

        private MapList()
        {
            MapLocationInfo[] maps = MapManager.Instance.GetMaps();

            Array.Sort<MapLocationInfo>(maps, Comparison);

            for (int i = 0; i < maps.Length; i++)
            {
                Add(maps[i]);
            }
        }

        static int Comparison(MapLocationInfo l, MapLocationInfo r)
        {
            return l.ResLocation.Name.CompareTo(r.ResLocation.Name);
        }

        public static void Initialize()
        {
            singleton = new MapList();
        }

        public bool IsIndexValid(int idx)
        {
            return idx >= 0 && idx < Count;
        }
    }

    public static class SkirmishMenu
    {
        public static int MinMoney
        {
            get;
            private set;
        }
        public static int Money
        {
            get;
            private set;
        }
        public static int MaxMoney
        {
            get;
            private set;
        }
        public static int MoneyIncrement
        {
            get;
            private set;
        }

        public static int MinUnitCount
        {
            get;
            private set;
        }

        public static int UnitCount
        {
            get;
            private set;
        }
        public static int MaxUnitCount
        {
            get;
            private set;
        }

        public static int TechLevel
        {
            get;
            private set;
        }

        public static int GameSpeed
        {
            get;
            private set;
        }

        public static bool AllyChangeAllowed
        {
            get;
            private set;
        }

        /// <summary>
        ///  Gets or sets the defualt value for MCVRedeploys
        /// </summary>
        public static bool MCVRedeploys
        {
            get;
            private set;
        }
        public static bool ShortGame
        {
            get;
            private set;
        }
        public static bool MultiEngineer
        {
            get;
            private set;
        }

        public static bool Crates
        {
            get;
            private set;
        }

        public static Dictionary<string, HouseType> Countries
        {
            get;
            set;
        }
        public static Dictionary<string, int> Colors
        {
            get;
            set;
        }
        public static Dictionary<string, SideType> Sides
        {
            get;
            set;
        }

        static MapBase selectedMap;

        public static class Panel
        {
            [ScriptBinding(ScriptBinding.Auto)]
            public static void ObjectLoadImpl(object sender)
            {
                MenuPanel panel = (MenuPanel)sender;

                if (!MapList.Initialized)
                {
                    MapList.Initialize();
                }
            }

            static void ParseMultiplaySettings()
            {
                ConfigurationSection mdsect = GameConfigs.Instance.CurrentRules["MultiplayerDialogSettings"];

                MinMoney = mdsect.GetInt("MinMoney", 2500);
                Money = mdsect.GetInt("Money", 10000);
                MaxMoney = mdsect.GetInt("MaxMoney", 10000);
                MoneyIncrement = mdsect.GetInt("MoneyIncrement", 100);
                MinUnitCount = mdsect.GetInt("MinUnitCount", 1);
                UnitCount = mdsect.GetInt("UnitCount", 10);
                MaxUnitCount = mdsect.GetInt("MaxUnitCount", 20);
                TechLevel = mdsect.GetInt("TechLevel", 10);
                GameSpeed = mdsect.GetInt("GameSpeed", 10);

                AllyChangeAllowed = mdsect.GetBool("AllyChangeAllowed", false);
                MultiEngineer = mdsect.GetBool("MultiEngineer", false);
                ShortGame = mdsect.GetBool("ShortGame ", true); 
                MCVRedeploys = mdsect.GetBool("MCVRedeploys ", true);
                Crates = mdsect.GetBool("MCVRedeploys ", true);
            }
            static void LoadCountryAndSides()            
            {
                Dictionary<string, HouseType> countries;
                Dictionary<string, SideType> sides;
                Battle.GetSidesAndCountries(out countries, out sides);

                Countries = countries;
                Sides = sides;

                Colors = Battle.GetMultiplayerColors();
            }

           

            [ScriptBinding(ScriptBinding.Auto)]
            public static void LoadingResourceImpl(object sender)
            {
                SkirmishConfigs skcon = (SkirmishConfigs)BasicConfigs.Instance[SkirmishConfigFactory.SkirmishConfigName];

                int mapIdx = skcon.ScenIndex;

                if (!MapList.Instance.IsIndexValid(mapIdx))
                {
                    mapIdx = 0;
                }

                // GameConfigs.CurrentConfig should update when a new map is set
                // at the same time houses ,colors and side should load for the ui
                // [MultiplayerDialogSettings] should be parsed here

                if (MapList.Instance.Count > 0)
                {
                    MapLocationInfo currentMapInfo = MapList.Instance[mapIdx];
                    selectedMap = MapManager.Instance.CreateInstance(currentMapInfo);

                    Battle.MergeRules(selectedMap);
                }
                else
                {
                    SRuntime.MessageBox("No maps available", "DlgOkCancel", null);
                    return;
                }

                LoadCountryAndSides();

                ParseMultiplaySettings();
            }



            [ScriptBinding(ScriptBinding.Auto)]
            public static void UnloadingResourceImpl(object sender)
            {
                //MessageBox.Show("UnloadingResourceImpl");
                //Runtime.MessageBox("UnloadingResourceImpl", "DlgOkCancel", null);
            }

            [ScriptBinding(ScriptBinding.Auto)]
            public static void DisposingImpl(object sender)
            {
                MenuPanel panel = (MenuPanel)sender;

            }


        }

        public static class Item2
        {
            static void messageHandler(MsgBoxResult res)
            {
                if (res == MsgBoxResult.ButtonA)
                {
                    BattleSettings sets = new BattleSettings();

                    sets.MapFile = FileSystem.Instance.Locate("multimd.mix\\meatloaf.map3", FileSystem.GameResLR);
                    sets.MapExt = ".map3";

                    sets.PlayerInfo = new PlayerInfo[1];

                    sets.Countries = new HouseType[Countries.Count];

                    Waypoint[] stPts = selectedMap.StartPoints;

                    int index=0;
                    foreach (HouseType cnt in Countries.Values)
                    {
                        //sets.PlayerInfo[0] = new PlayerInfo("Test Only", stPts[0].X, stPts[0].Y, PlayerType.LocalHuman, new List<House>());
                        sets.PlayerInfo[0] = new PlayerInfo("Test Only", 97, 52, PlayerType.LocalHuman, new List<House>());
                        sets.PlayerInfo[0].Houses.Add(new House(null, cnt));
                        sets.LoadScreen = new SkirmishLoadScreen(Game.Instance.GameUI, cnt.LoadScreenName);

                        sets.Countries[index++] = cnt;
                        break;
                    }

                    Dictionary<string, SideType>.ValueCollection sides = Sides.Values;
                    sets.Sides = new SideType[sides.Count];
                    index = 0;
                    foreach (SideType sde in sides)
                    {
                        sets.Sides[index++] = sde;
                    }

                    Battle bat = new Battle(Game.Instance, sets);

                    Game.Instance.CurrentBattle = bat;

                    bat.Load();
                }
            }

            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {
                SRuntime.MessageBoxNST("start a test?", "DlgOkCancel", messageHandler);
            }
        }

        public static class Item3
        {
            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {
                string msg =
                    "This message is from a plug-in DLL.\nYou can code your own plug-ins.\n There are 2 ways to add customized features:\n\n" +
                    "(1) Event handlers. You can write some functions in the DLL. The functions will be used to handle the events taken place in the game. One event can just be a button being pressed or a super weapon activated.\n" +
                    "(2) New implementations. You can add/replace functions in the engine. ";

                SRuntime.MessageBoxNST(msg, "DlgOkCancel", null);
            }
        }

        public static class Item9
        {
            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {
                R3D.UI.Menu mm = Game.Instance.GameUI.Menu;
                mm.UserChangeMenu("SinglePlayerMenu");//mm[1]);
            }
        }
    }
}
