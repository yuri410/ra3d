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
using R3D.AI;
using R3D.Collections;
using R3D.ConfigModel;
using R3D.Core;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Effects;
using R3D.IsoMap;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Logic
{
    /// <summary>
    /// 指定一个cell上的交通情况
    /// </summary>
    public enum CellTraffic : byte
    {
        Clear = 0,
        Jam,
        Blocked
    }

    public struct Waypoint
    {
        public int Index;
        public int X;
        public int Y;

        public Waypoint(int index, int x, int y)
        {
            Index = index;
            X = x;
            Y = y;
        }
    }


    public struct CellInfo
    {
        public CellHeight HeightInfo;

        public FastList<Techno> Units;

        public FastList<TerrainObject> TObjs;

        public bool Used;
    }

    public class PrerequisiteAliasInfo : IConfigurable
    {
        public TechnoType[] PrerequisitePower
        {
            get;
            protected set;
        }
        public TechnoType[] PrerequisiteFactory
        {
            get;
            protected set;
        }
        public TechnoType[] PrerequisiteBarracks
        {
            get;
            protected set;
        }
        public TechnoType[] PrerequisiteRadar
        {
            get;
            protected set;
        }
        public TechnoType[] PrerequisiteTech
        {
            get;
            protected set;
        }
        public TechnoType[] PrerequisiteProc
        {
            get;
            protected set;
        }
        public TechnoType[] PrerequisiteProcAlternate
        {
            get;
            protected set;
        }


        string GetPrereqGroupCode(TechnoType[] group)
        {
            StringBuilder code = new StringBuilder();
            code.Append(PrerequisiteExpressionCompiler.LBracket);

            TechnoType[] paras = group;

            int len2 = paras.Length;
            for (int j = 0; j < len2; j++)
            {
                code.Append(paras[j].TypeName);
                if (j < len2 - 1)
                {
                    code.Append(PrerequisiteExpressionCompiler.OpOr);
                }
            }

            code.Append(PrerequisiteExpressionCompiler.RBracket);

            return code.ToString();
        }


        public string PrerequisitePowerExp
        {
            get;
            protected set;
        }
        public string PrerequisiteFactoryExp
        {
            get;
            protected set;
        }
        public string PrerequisiteBarracksExp
        {
            get;
            protected set;
        }
        public string PrerequisiteRadarExp
        {
            get;
            protected set;
        }
        public string PrerequisiteTechExp
        {
            get;
            protected set;
        }
        public string PrerequisiteProcExp
        {
            get;
            protected set;
        }
        public string PrerequisiteProcAlternateExp
        {
            get;
            protected set;
        }


        string[] PrerequisitePowerName
        {
            get;
            set;
        }
        string[] PrerequisiteFactoryName
        {
            get;
            set;
        }
        string[] PrerequisiteBarracksName
        {
            get;
            set;
        }
        string[] PrerequisiteRadarName
        {
            get;
            set;
        }
        string[] PrerequisiteTechName
        {
            get;
            set;
        }
        string[] PrerequisiteProcName
        {
            get;
            set;
        }
        string[] PrerequisiteProcAlternateName
        {
            get;
            set;
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            PrerequisitePowerName = sect.GetStringArray("PrerequisitePower");

            PrerequisiteFactoryName = sect.GetStringArray("PrerequisiteFactory");
            PrerequisiteBarracksName = sect.GetStringArray("PrerequisiteBarracks");
            PrerequisiteRadarName = sect.GetStringArray("PrerequisiteRadar");
            PrerequisiteTechName = sect.GetStringArray("PrerequisiteTech");
            PrerequisiteProcName = sect.GetStringArray("PrerequisiteProc");

            PrerequisiteProcAlternateName = sect.GetStringArray("PrerequisiteProcAlternate");
        }

        #endregion

        TechnoType[] ParseTechnoTypes(Dictionary<string, TechnoType> technoTpye, string[] typeNames)
        {
            TechnoType[] res = new TechnoType[typeNames.Length];
            for (int i = 0; i < typeNames.Length; i++)
            {
                res[i] = technoTpye[typeNames[i]];
            }
            return res;
        }

        public void ParseTechnoTypes(Dictionary<string, TechnoType> technoType)
        {
            PrerequisitePower = ParseTechnoTypes(technoType, PrerequisitePowerName);
            PrerequisiteFactory = ParseTechnoTypes(technoType, PrerequisiteFactoryName);
            PrerequisiteBarracks = ParseTechnoTypes(technoType, PrerequisiteBarracksName);
            PrerequisiteRadar = ParseTechnoTypes(technoType, PrerequisiteRadarName);
            PrerequisiteTech = ParseTechnoTypes(technoType, PrerequisiteTechName);
            PrerequisiteProc = ParseTechnoTypes(technoType, PrerequisiteProcName);
            PrerequisiteProcAlternate = ParseTechnoTypes(technoType, PrerequisiteProcAlternateName);

            PrerequisitePowerExp = GetPrereqGroupCode(PrerequisitePower);
            PrerequisiteFactoryExp = GetPrereqGroupCode(PrerequisiteFactory);
            PrerequisiteBarracksExp = GetPrereqGroupCode(PrerequisiteBarracks);
            PrerequisiteRadarExp = GetPrereqGroupCode(PrerequisiteRadar);
            PrerequisiteTechExp = GetPrereqGroupCode(PrerequisiteTech);
            PrerequisiteProcExp = GetPrereqGroupCode(PrerequisiteProc);
            PrerequisiteProcAlternateExp = GetPrereqGroupCode(PrerequisiteProcAlternate);
        }

    }

    public class BuildableTable : IConfigurable
    {
        bool[] data = new bool[16];
        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            data[(int)TerrainType.Clear] = sect.GetBool("Clear", false);
            data[(int)TerrainType.Ice] = sect.GetBool("Ice", false);
            data[(int)TerrainType.Tunnel] = sect.GetBool("Tunnel", false);
            data[(int)TerrainType.Railroad] = sect.GetBool("Railroad", false);
            data[(int)TerrainType.Rock] = sect.GetBool("Rock", false);
            data[(int)TerrainType.Water] = sect.GetBool("Water", false);
            data[(int)TerrainType.Beach] = sect.GetBool("Beach", false);
            data[(int)TerrainType.Road] = sect.GetBool("Road", false);
            data[(int)TerrainType.Rough] = sect.GetBool("Rough", false);
            data[13] = true;

        }

        #endregion


        public bool IsBuildable(TerrainType ttype)
        {
            return data[(int)ttype];
        }
    }

    /// <summary>
    /// 记录，处理地图上的东西
    /// </summary>
    public class BattleField
    {
        Terrain terrain;
        CellTraffic[][][] passTable;

        Device dev;

        SceneManagerBase sceneMgr;

        public SceneManagerBase SceneManager
        {
            get { return sceneMgr; }
        }
        //Camera cam;

        public LocomotionManager LocomotionMgr
        {
            get;
            protected set;
        }
        public PathFinderManager PathFinder
        {
            get;
            protected set;
        }

        public TheaterBase Theater
        {
            get;
            protected set;
        }


        public BuildableTable BuildableTable
        {
            get;
            protected set;
        }

        public Dictionary<string, TechnoType> TechnoTypes
        {
            get;
            protected set;
        }
        public Dictionary<string, UnitType> UnitTypes
        {
            get;
            protected set;
        }
        public Dictionary<string, InfantryType> InfantryTypes
        {
            get;
            protected set;
        }
        public Dictionary<string, AircraftType> AircraftTypes
        {
            get;
            protected set;
        }

        public Dictionary<string, BuildingType> BuildingTypes
        {
            get;
            protected set;
        }
        public Dictionary<string, BuildingType> DefenceTypes
        {
            get;
            protected set;
        }
        /// <summary>
        /// base building types
        /// </summary>
        public Dictionary<string, BuildingType> ResourceTypes
        {
            get;
            protected set;
        }

        public Dictionary<string, TerrainObjectType> TerrainTypes
        {
            get;
            protected set;
        }

        public Dictionary<string, SpeedType> SpeedTypes
        {
            get;
            protected set;
        }

        public Dictionary<string, int> Colors
        {
            get;
            set;
        }

        //public List<HouseType> Countries
        //{
        //    get;
        //    protected set;
        //}

        public Dictionary<string, HouseType> Countries
        {
            get;
            protected set;
        }

        public Dictionary<string, SideType> Sides
        {
            get;
            protected set;
        }

        public Dictionary<string, int> ArmorTypes
        {
            get;
            protected set;
        }


        public List<Techno> UnitsOnMap
        {
            get;
            protected set;
        }

        public List<TerrainObject> TerrainObjects
        {
            get;
            protected set;
        }

        //List<TerrainObject > 


        //        void BuildPathFinder(MapBase map, CellData[] cells)
        //        {
        //            int len = map.Width + map.Height - 1;

        //            TerrainType[][] terr = new TerrainType[len][];
        //            passTable = new CellTraffic[len][][];
        //            for (int i = 0; i < len; i++)
        //            {
        //                terr[i] = new TerrainType[len];

        //                passTable[i] = new CellTraffic[len][];
        //                for (int j = 0; j < len; j++)
        //                {
        //                    passTable[i][j] = new CellTraffic[1];
        //                }
        //            }


        //            for (int i = 0; i < cells.Length; i++)
        //            {

        //                TileBase[] tileType = Theater[cells[i].tile];
        //#warning todo bridge

        //                if (tileType != null)
        //                {
        //                    terr[cells[i].x][cells[i].y] = tileType[0].GetTerrainType(cells[i].subTile);
        //                }
        //            }

        //            PathFinder = new PathFinderManager(passTable, terr);

        //        }
        void LoadInGameTypes(Battle battle, MapBase map)
        {
            // load in-game types
            ConfigModel.Configuration rules = GameConfigs.Instance.CurrentRules;
            ConfigModel.Configuration art = GameConfigs.Instance.Art;



            ConfigurationSection sect = rules["Colors"];
            Colors = new Dictionary<string, int>(sect.Count + 1);
            foreach (KeyValuePair<string, string> e in sect)
            {
                Colors.Add(e.Key, sect.GetColorRGBInt(e.Key));
            }


            LocomotionMgr = new LocomotionManager();
            LocomotionMgr.RegisterLocomotionType(new DriveLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new FlyLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new HoverLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new JumpjetLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new MechLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new RocketLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new ShipLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new TeleportLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new TunnelLocomotionFactory());
            LocomotionMgr.RegisterLocomotionType(new WalkLocomotionFactory());

            SpeedTypes = new Dictionary<string, SpeedType>(20, CaseInsensitiveStringComparer.Instance);

            sect = rules["SpeedTypes"];
            foreach (KeyValuePair<string, string> e in sect)
            {
                ConfigurationSection stSect;
                if (rules.TryGetValue(e.Value, out stSect) && !SpeedTypes.ContainsKey(e.Value))
                {
                    SpeedType st = new SpeedType(e.Value);
                    st.Parse(stSect);
                    SpeedTypes.Add(e.Value, st);
                }
                else
                {
                    GameConsole.Instance.Write("Speed Type " + e.Value + "'s defination is not found. Ignored.", ConsoleMessageType.Warning);
                }
            }

            sect = rules["BuildableTable"];
            BuildableTable = new BuildableTable();
            BuildableTable.Parse(sect);

            sect = rules["ArmorTypes"];
            ArmorTypes = new Dictionary<string, int>(10, CaseInsensitiveStringComparer.Instance);

            int index = 0;
            foreach (KeyValuePair<string, string> e in sect)
            {
                ArmorTypes.Add(e.Value, index++);
            }


            TechnoTypes = new Dictionary<string, TechnoType>(1000, CaseInsensitiveStringComparer.Instance);

            sect = rules["InfantryTypes"];
            InfantryTypes = new Dictionary<string, InfantryType>(sect.Count + 1);
            foreach (KeyValuePair<string, string> e in sect)
            {
                ConfigurationSection infSect;
                if (rules.TryGetValue(e.Value, out infSect) && !InfantryTypes.ContainsKey(e.Value))
                {
                    InfantryType inf = new InfantryType(this, e.Value);

                    inf.Parse(infSect);
                    inf.ParseArmor(ArmorTypes);

                    InfantryTypes.Add(e.Value, inf);

                    TechnoTypes.Add(e.Value, inf);
                }
                else
                {
                    GameConsole.Instance.Write("Infantry Type " + e.Value + "'s defination is not found. Ignored.", ConsoleMessageType.Warning);
                }
            }

            sect = rules["VehicleTypes"];
            UnitTypes = new Dictionary<string, UnitType>(sect.Count + 1, CaseInsensitiveStringComparer.Instance);
            foreach (KeyValuePair<string, string> e in sect)
            {
                ConfigurationSection unitSect;
                if (rules.TryGetValue(e.Value, out unitSect) && !UnitTypes.ContainsKey(e.Value))
                {
                    UnitType unit = new UnitType(this, e.Value);
                    unit.Parse(unitSect);
                    unit.ParseArmor(ArmorTypes);

                    UnitTypes.Add(e.Value, unit);

                    TechnoTypes.Add(e.Value, unit);
                }
                else
                {
                    GameConsole.Instance.Write("Vehicle Type " + e.Value + "'s defination is not found. Ignored.", ConsoleMessageType.Warning);
                }
            }

            DefenceTypes = new Dictionary<string, BuildingType>(CaseInsensitiveStringComparer.Instance);
            ResourceTypes = new Dictionary<string, BuildingType>(CaseInsensitiveStringComparer.Instance);

            sect = rules["BuildingTypes"];
            BuildingTypes = new Dictionary<string, BuildingType>(sect.Count + 1, CaseInsensitiveStringComparer.Instance);
            foreach (KeyValuePair<string, string> e in sect)
            {
                ConfigurationSection airSect;
                if (rules.TryGetValue(e.Value, out airSect) && !BuildingTypes.ContainsKey(e.Value))
                {
                    BuildingType bld = new BuildingType(this, e.Value);
                    bld.Parse(airSect);
                    bld.ParseArmor(ArmorTypes);

                    BuildingTypes.Add(e.Value, bld);

                    if (bld.IsBaseDefense)
                    {
                        DefenceTypes.Add(e.Key, bld);
                    }
                    else
                    {
                        BuildingTypes.Add(e.Key, bld);
                    }

                    TechnoTypes.Add(e.Value, bld);
                }
                else
                {
                    GameConsole.Instance.Write("Building Type " + e.Value + "'s defination is not found. Ignored.", ConsoleMessageType.Warning);
                }
            }

            sect = rules["Countries"];
            Countries = new Dictionary<string, HouseType>(CaseInsensitiveStringComparer.Instance);

            foreach (KeyValuePair<string, string> e in sect)
            {
                ConfigurationSection cntSect = rules[e.Value];

                HouseType country = new HouseType(e.Value, Colors, TechnoTypes);
                country.Parse(cntSect);

                Countries.Add(country.Name, country);
            }


            sect = rules["Sides"];
            Sides = new Dictionary<string, SideType>(CaseInsensitiveStringComparer.Instance);
            index = 0;
            foreach (KeyValuePair<string, string> e in sect)
            {
                SideType side = new SideType(e.Key, index++, Countries);

                string[] cntNames = sect.GetStringArray(e.Key);
                side.ParseCountries(cntNames);

                ConfigurationSection sideSect;
                if (rules.TryGetValue(e.Key, out sideSect))
                {
                    side.Parse(sideSect);
                }
                Sides.Add(side.Name, side);
            }


            sect = rules["TerrainTypes"];

            TerrainTypes = new Dictionary<string, TerrainObjectType>(CaseInsensitiveStringComparer.Instance);
            foreach (KeyValuePair<string, string> e in sect)
            {
                TerrainObjectType ttype = new TerrainObjectType(this, e.Value);

                ConfigurationSection ttSect;
                if (rules.TryGetValue(e.Value, out ttSect))
                {
                    ttype.Parse(ttSect);
                    ttype.LoadGraphics(art);
                    TerrainTypes.Add(e.Value, ttype);
                }
                else
                {
                    GameConsole.Instance.Write("TerrainObject Type " + e.Value + "'s defination is not found. Ignored.", ConsoleMessageType.Warning);
                }
            }


            //Countries = new List<HouseType>(sect.Count + 1);

            //foreach (KeyValuePair<string, string> e in sect)
            //{
            //    ConfigurationSection cntSect = rules[e.Value];

            //    HouseType country = new HouseType(e.Value, Colors, TechnoTypes);

            //    Countries.Add(country);
            //}



            //foreach (UnitType ut in UnitTypes.Values)
            //{
            //    ut.LoadGraphics(GameConfigs.Instance.Art);
            //}

            sect = rules[GameConfigs.GeneralSectionName];

            PrerequisiteAliasInfo prereqInfo = new PrerequisiteAliasInfo();
            prereqInfo.Parse(sect);
            prereqInfo.ParseTechnoTypes(TechnoTypes);

            PrerequisiteExpressionCompiler compiler = new PrerequisiteExpressionCompiler();

            foreach (KeyValuePair<string, TechnoType> e in TechnoTypes)
            {
                e.Value.LoadGraphics(GameConfigs.Instance.Art);
                e.Value.ParseTechnoTypes(compiler, prereqInfo, TechnoTypes);
                e.Value.ParseBuildingTypes(BuildingTypes);
                e.Value.ParseCountries(Countries);
            }
            
        }



        SkyBox CreateSkyBox(string name)
        {
            SkyBox sbx = new SkyBox(dev);
            sbx.Parse(GameConfigs.Instance.Art[name]);
            return sbx;
        }

        void SpawnEntities(MapBase map)
        {
            ConfigModel.Configuration rules = GameConfigs.Instance.CurrentRules;

            ConfigurationSection sect = rules[GameConfigs.GeneralSectionName];

            string[] v = sect.GetStringArray("BaseUnit");

            for (int i = 0; i < v.Length; i++)
            {
                UnitType mcv = UnitTypes[v[i]];

                List<HouseType> owners = mcv.Owners;
                for (int j = 0; j < owners.Count; j++)
                {
                    owners[j].MCVTypes.Add(mcv);
                }
            }

            foreach (KeyValuePair<string, TechnoType> e in TechnoTypes)
            {
                TechnoType tech = e.Value;

                if (tech.AllowedToStartInMultiplayer)
                {
                    List<HouseType> owners = tech.Owners;

                    for (int j = 0; j < owners.Count; j++)
                    {
                        owners[j].MultiplayerStartupUTs.Add(tech);
                    }
                }
            }

            TerrainObjectInfo[] terrObjectInfo = map.TerrainObjects;
            for (int i = 0; i < terrObjectInfo.Length; i++)
            {
                TerrainObjectType ttype;

                if (TerrainTypes.TryGetValue(terrObjectInfo[i].TypeName, out ttype))
                {
                    TerrainObject tobj = ttype.CreateInstance();
                    SpawnTerrainObject(tobj, terrObjectInfo[i].X, terrObjectInfo[i].Y);
                }
                else
                {
                    GameConsole.Instance.Write("Cannot find TerrainObjectType" + terrObjectInfo[i].TypeName + " when loading terrain objects.", ConsoleMessageType.Warning);
                }
                //terrObjects[i]
            }
        }

        public void SpawnUnit(Techno techno, int x, int y, bool autoBridge)
        {
            techno.SetPosition(x, y);
            //techno.Yaw = MathEx.PIf * 7f / 4f;

            if (autoBridge)
            {

            }


            sceneMgr.AddObjectToScene(techno);
            UnitsOnMap.Add(techno);

        }
        public void RemoveUnit(Techno techno) 
        {
            sceneMgr.RemoveObjectFromScene(techno);
            UnitsOnMap.Remove(techno );
        }

        public void SpawnTerrainObject(TerrainObject tobj, int x, int y)
        {
            tobj.SetPosition(x, y);

           
            sceneMgr.AddObjectToScene(tobj);
            TerrainObjects.Add(tobj);
        }


        public void SpawnPlayerMultiplayerStartupUnit(Player player)
        {
            const int Spacing = 3;


            House firstHouse = player.GetControlledHouse(0);

            List<UnitType> unit = firstHouse.Country.MCVTypes;

            int startX = player.StartX;
            int startY = player.StartY;

            for (int i = 0; i < unit.Count; i++)
            {
                Techno mcv = unit[i].CreateInstance(firstHouse);

                SpawnUnit(mcv, startX, startY, true);

                startX--;
                startY--;
            }
        }

        public CellInfo[][] CellInfo
        {
            get;
            private set;
        }
        //public CellHeight[][] CellHeights
        //{
        //    get;
        //    private set;
        //}

        public Terrain BattleTerrain
        {
            get { return terrain; }
        }

        public BattleField(Battle battle, MapBase map)
        {
            CellData[] cells = map.GetCellData();
            dev = battle.Device;

            Theater = map.TerrainTheater;

            //HeightMap heightMap = map.GetHeightMap();
            TheaterBase theater = map.TerrainTheater;

            //int width = heightMap.Width;
            //int height = heightMap.Height;
            //BuildPathFinder(map, cells);

            //InitializeEffect();

            terrain = new Terrain(battle, dev, map, Theater, cells);
            //CellHeights = terrain.CellHeights;


            CellHeight[][] cellHgt = terrain.GetCellHeight();

            this.CellInfo = new CellInfo[cellHgt.Length][];
            for (int i = 0; i < this.CellInfo.Length; i++)
            {
                this.CellInfo[i] = new CellInfo[cellHgt[i].Length];
                for (int j = 0; j < this.CellInfo[i].Length; j++)
                {
                    this.CellInfo[i][j].HeightInfo = cellHgt[i][j];
                    // this.CellInfo[i][j].Units = new FastList<Techno>();
                    // this.CellInfo[i][j].TObjs = new FastList<TerrainObject>();
                }
            }

            int len = map.Width + map.Height - 1;

            TerrainType[][] terr = new TerrainType[len][];
            passTable = new CellTraffic[len][][];
            for (int i = 0; i < len; i++)
            {
                terr[i] = new TerrainType[len];

                passTable[i] = new CellTraffic[len][];
                for (int j = 0; j < len; j++)
                {
                    passTable[i][j] = new CellTraffic[1];
                }
            }

            for (int i = 0; i < cells.Length; i++)
            {
                TileBase[] tileType = Theater[cells[i].tile];
#warning todo bridge

                if (tileType != null)
                {
                    int cx = cells[i].x;
                    int cy = cells[i].y;

                    terr[cx][cy] = tileType[0].GetTerrainType(cells[i].subTile);

                    this.CellInfo[cx][cy].TObjs = new FastList<TerrainObject>();
                    this.CellInfo[cx][cy].Units = new FastList<Techno>();
                    this.CellInfo[cx][cy].Used = true;
                }
            }

            PathFinder = new PathFinderManager(passTable, terr);






            theater.ReleaseTextures();

            //rnder = new Renderer(dev, this, terrain, map);
            //sceneMgr = new SceneManager(dev, new Atmosphere(dev, map.Atmosphere, CreateSkyBox));


            sceneMgr = new OctreeSceneManager(dev, new Atmosphere(dev, map.Atmosphere, CreateSkyBox), new OctreeBox(ref terrain.BoundingSphere), 10f);

            sceneMgr.RegisterCamera(battle.Camera);

            sceneMgr.AddObjectToScene(terrain);

            //Atmosphere atmos = new Atmosphere(dev, map.Atmosphere, CreateSkyBox);

            UnitsOnMap = new List<Techno>(1000);
            TerrainObjects = new List<TerrainObject>(500);

            LoadInGameTypes(battle, map);


            SpawnEntities(map);


            //DriverLocomotionDescription ldesc = new DriverLocomotionDescription(this, new DriveLocomotionFactory());

            //testUT = new UnitType(this, "TEST", ldesc);

            //testUT.Model = new GameModel(dev, 1);
            //testUT.Model.Entities[0] = new GameMesh(Mesh.CreateTeapot(dev));

            //testUnit = (Unit)testUT.CreateInstance(null);
            //testUnit.X = 25;
            //testUnit.Y = 50;
            //testUnit.Z = 0;
            //testUnit.Locomotor.Position = new Vector3(94 * Terrain.HorizontalUnit, 0, 1 * Terrain.HorizontalUnit);
            //, 45, 0);
            //testUnit.SetDestination(25, 55, 0);
            //testUnit.Update(0);
        }

        public void GetCellCenter(int x, int y, out Vector3 res)
        {
            res = new Vector3(x * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f, CellInfo[x][y].HeightInfo.centre, y * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f);
        }
        public Vector3 GetCellCenter(int x, int y)
        {
            return new Vector3(x * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f, CellInfo[x][y].HeightInfo.centre, y * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f);
        }


        public void Render()
        {
            sceneMgr.RenderScene();
        }

        /// <summary>
        /// 游戏逻辑
        /// </summary>
        public void Update(float dt)
        {
            for (int i = 0; i < UnitsOnMap.Count; i++)
            {
                if (!UnitsOnMap[i].IsSleeping)
                {
                    UnitsOnMap[i].Update(dt);
                }
            }


            sceneMgr.Update(dt);
            //cam.Update();
            //sceneMgr.Update();
            //testUnit.Update(0);
        }

    }
}
