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
using System.Text;
using System.Threading;
using R3D.ConfigModel;
using R3D.Core;
using R3D.Base;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Effects;
using R3D.IO;
using R3D.IsoMap;
using R3D.MathLib;
using R3D.UI;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Logic
{
    //public delegate void ProgressCallBack(int value);

    public class BattleSettings
    {
        ResourceLocation mapFile;
        //ProgressCallBack progressUpdater;
        IBattleLoadScreen loadScreen;
        PlayerInfo[] playerInfo;

       

        string mapExt;

        public SideType[] Sides
        {
            get;
            set;
        }
        public HouseType[] Countries
        {
            get;
            set;
        }

        public IBattleLoadScreen LoadScreen
        {
            get { return loadScreen; }
            set { loadScreen = value; }
        }

        public ResourceLocation MapFile
        {
            get { return mapFile; }
            set { mapFile = value; }
        }
        public string MapExt 
        {
            get { return mapExt; }
            set { mapExt = value; }
        }

        public PlayerInfo[] PlayerInfo
        {
            get { return playerInfo; }
            set { playerInfo = value; }
        }
        //public ProgressCallBack ProgressCallBack
        //{
        //    get { return progressUpdater; }
        //    set { progressUpdater = value; }
        //}


    }

    public class CameraSetting : IConfigurable
    {
        public bool CanRotate
        {
            get;
            protected set;
        }
        public bool CanZoom
        {
            get;
            protected set;
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            CanRotate = sect.GetBool("CameraCanRotate", false);
            //FieldOfView = sect.GetFloat("FieldOfView", 45f);
            CanZoom = sect.GetBool("CameraCanZoom", false);            
        }

        #endregion
    }

    public unsafe class Battle
    {
        Device device;
        Game game;

        BattleSettings settings;
        BattleField btfld;

        Thread loadThread;
        bool isLoaded;

        RtsCamera camera;


        HumanPlayer localPlayer;

        Player[] players;

        BattleUI battleUI;

        MapBase map;

        public Battle(Game game, BattleSettings sets)
        {
            this.game = game;
            device = game.Device;
            settings = sets;

            CameraSettings = new CameraSetting();

            Size cs = game.ClientSize;
            camera = new RtsCamera(60, (float)cs.Width / (float)cs.Height);
            camera.RenderTarget = device.GetRenderTarget(0);
        }

        public CameraSetting CameraSettings
        {
            get;
            protected set;
        }

        public HumanPlayer LocalPlayer
        {
            get { return localPlayer; }
        }

        public Device Device
        {
            get { return device; }
        }

        public Camera Camera
        {
            get { return camera; }
        }

        /// <summary>
        ///  Loads the map specialized rule
        ///  载入地图微观ini设置
        /// </summary>
        void LoadRules(MapBase map)
        {
            ConfigModel.Configuration rules = GameConfigs.Instance.Rules.Clone();
            rules.Merge(map.Config);
            GameConfigs.Instance.CurrentRules = rules;
        }

        public static void MergeRules(MapBase map)
        {
            ConfigModel.Configuration rules = GameConfigs.Instance.Rules.Clone();
            rules.Merge(map.Config);
            GameConfigs.Instance.CurrentRules = rules;
        }




        void load()
        {
            map.Load();

            // load the ingame ui
            battleUI = new BattleUI(this, game.GameUI, game.SoundSystem);

            ConfigModel.Configuration rules = GameConfigs.Instance.CurrentRules;
            ConfigurationSection sect = rules["MouseActionList"];
            foreach (KeyValuePair<string, string> e in sect)
            {
                MouseAction ma = new MouseAction();
                ma.Parse(rules[e.Value]);

                battleUI.RegisterMouseAction(e.Value, ma);
            }


            localPlayer.GetControlledHouse(0).ParentUI = battleUI;
            //localPlayer.Camera = camera;

            battleUI.Load();



            btfld = new BattleField(this, map);
            //settings.LoadScreen.Progress = 1;



            // change house's country to ingame country
            for (int i = 0; i < localPlayer.ControlledHouseCount; i++)
            {
                HouseType newCnt = btfld.Countries[localPlayer.GetControlledHouse(i).Country.Name];
                localPlayer.GetControlledHouse(i).Country = newCnt;

                localPlayer.GetControlledHouse(i).Load(btfld);
            }



            battleUI.InitializeBattleControl(btfld);



            camera.Position = new Vector3(Terrain.HorizontalUnit * localPlayer.StartX, 0, Terrain.HorizontalUnit * localPlayer.StartY);
            btfld.SpawnPlayerMultiplayerStartupUnit(localPlayer);


            // set it again
            Size cs = game.ClientSize;
            camera.AspectRatio = (float)cs.Width / (float)cs.Height;

            ConfigurationSection camSect = rules[GameConfigs.GeneralSectionName];
            CameraSettings.Parse(camSect);

            camera.Parse(camSect);
            camera.ResetView();

            Thread.Sleep(1000);
            isLoaded = true;
        }

        public static void GetSidesAndCountries(out Dictionary<string, HouseType> cnts, out Dictionary<string, SideType> sides)
        {
            cnts = GetCountries();
            sides = GetSides(cnts);

            Dictionary<string, HouseType>.ValueCollection vals = cnts.Values;
            foreach (HouseType e in vals)
            {
                e.ParseSide(sides);
            }
        }

        public static Dictionary<string, SideType> GetSides(Dictionary<string, HouseType> countries)
        {
            ConfigModel.Configuration rules = GameConfigs.Instance.CurrentRules;

            ConfigurationSection sect = rules["Sides"];

            Dictionary<string, SideType> sides = new Dictionary<string, SideType>();
            int index = 0;
            foreach (KeyValuePair<string, string> e in sect)
            {
                SideType side = new SideType(e.Key, index++, countries);

                string[] cntNames = sect.GetStringArray(e.Key);
                side.ParseCountries(cntNames);

                ConfigurationSection sideSect;
                if (rules.TryGetValue(e.Key, out sideSect))
                {
                    side.Parse(sideSect);
                }
                sides.Add(side.Name, side);
            }
            return sides;
        }

        /// <summary>
        /// multiplayer colors are loaded  when a new map is choosen or  a battle is loading.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, int> GetMultiplayerColors()
        {
            ConfigurationSection sect = GameConfigs.Instance.CurrentRules["Colors"];
            Dictionary<string, int> colors = new Dictionary<string, int>();
            foreach (KeyValuePair<string, string> e in sect)
            {
                colors.Add(e.Key, sect.GetColorRGBInt(e.Key));
            }
            return colors;
        }

        /// <summary>
        /// multiplayer countries are loaded when a new map is choosen or  a battle is loading.
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, HouseType> GetCountries()
        {
            ConfigModel.Configuration rules = GameConfigs.Instance.CurrentRules;

            ConfigurationSection sect = rules["Countries"];

            Dictionary<string, HouseType> availableCnts = new Dictionary<string, HouseType>();

            foreach (KeyValuePair<string, string> e in sect)
            {
                ConfigurationSection cntSect = rules[e.Value];

                HouseType country = new HouseType(e.Value, null, null);
                country.Parse(cntSect);

                availableCnts.Add(country.Name, country);
                
            }
            return availableCnts;
        }
        

        public void Load()
        {
            PlayerInfo[] playerInfo = settings.PlayerInfo;
            players = new Player[playerInfo.Length];

            for (int i = 0; i < playerInfo.Length; i++)
            {

                switch (playerInfo[i].Type)
                {
                    case PlayerType.Computer:
                        players[i] = new ComputerPlayer(this, playerInfo[i]);
                        break;
                    case PlayerType.LocalHuman:
                        localPlayer = new HumanPlayer(this, playerInfo[i]);
                        players[i] = localPlayer;
                        localPlayer.Camera = camera;

                        break;
                    case PlayerType.Remote:
                        break;
                }
            }

            
            map = MapManager.Instance.CreateInstance(settings.MapFile, settings.MapExt);
            if (GameConfigs.Instance.CurrentRules == null)
            {
                LoadRules(map);
            }

            if (settings.LoadScreen == null)
            {
                if (localPlayer.ControlledHouseCount > 0)
                {
                    settings.LoadScreen = new SkirmishLoadScreen(game.GameUI, localPlayer.GetControlledHouse(0).Country.LoadScreenName);
                }
            }
            game.GameUI.BattleLoadScrn = settings.LoadScreen;


            loadThread = new Thread(load);
            loadThread.Name = "Game Loader";
            loadThread.SetApartmentState(ApartmentState.MTA);
            loadThread.Start();
        }

        public bool IsLoaded
        {
            get { return isLoaded; }
        }
        public BattleSettings Settings
        {
            get { return settings; }
        }
        public void OnPaint()
        {
            if (isLoaded)
            {
                btfld.Render();
            }
        }

        public void Update(float dt)
        {
            if (isLoaded && loadThread != null)
            {
                loadThread.Abort();
                loadThread = null;

                game.GameUI.BattleLoadScrn = null;

                settings.LoadScreen.Dispose();


                game.CurrentBattle = this;
                game.GameUI.BattleUI = battleUI;

                GameConsole.Instance.Write("Game Loaded");
            }

            if (isLoaded)
            {               
                btfld.Update(dt);
            }
        }
    }
}
