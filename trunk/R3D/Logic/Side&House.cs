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

using R3D.ConfigModel;
using R3D.UI;

namespace R3D.Logic
{
    public class SideType : NamedObjectType
    {
        Dictionary<string, HouseType> countries;

        public List<HouseType> SideCountries
        {
            get;
            set;
        }

        public SideType(string name, int index, Dictionary<string, HouseType> cnts)
            : base(name)
        {
            countries = cnts;
            SideUIName = string.Empty;
            SideCountries = new List<HouseType>();
            Index = index;
        }

        public int Index
        {
            get;
            protected set;
        }

        public void ParseCountries(string[] valueList)
        {
            for (int i = 0; i < valueList.Length; i++)
            {
                SideCountries.Add(countries[valueList[i]]);
            }
        }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            SideUIName = sect["SideUIName"];
            //throw new NotImplementedException();
        }


        public string SideUIName
        {
            get;
            protected set;
        }
    }

    /// <summary>
    ///  country
    /// </summary>
    public class HouseType : NamedObjectType
    {
        List<InfantryType> veteranInf;
        List<UnitType> veteranUnits;
        List<AircraftType> veteranAir;
        //List<BuildingType> veteranBlds;

        public string UIName
        {
            get;
            protected set;
        }
        public Dictionary<string, int> Colors
        {
            get;
            protected set;
        }

        /// <summary>
        ///  Gets the reference to the units(all) list.
        /// </summary>
        public Dictionary<string, TechnoType> UnitTypes
        {
            get;
            protected set;
        }




        public int ColorScheme
        {
            get;
            protected set;
        }

        public List<InfantryType> VeteranInfantry
        {
            get { return veteranInf; }
        }
        public List<UnitType> VeteranUnits
        {
            get { return veteranUnits; }
        }
        public List<AircraftType> VeteranAircraft
        {
            get { return veteranAir; }
        }

        /// <summary>
        ///  Get the MCV unit types this country has
        /// </summary>
        public List<UnitType> MCVTypes
        {
            get;
            protected set;
        }

        /// <summary>
        ///  Get the multiplayer startup unit types.
        /// </summary>
        public List<TechnoType> MultiplayerStartupUTs
        {
            get;
            protected set;
        }


        //public List<BuildingType> VeteranBuildings
        //{
        //    get { return veteranBlds; }
        //}

        public void ParseSide(List<SideType> sides)
        {
            for (int i = 0; i < sides.Count; i++)
            {
                if (CaseInsensitiveStringComparer.Compare(sides[i].Name, SideName))
                {
                    Side = sides[i];
                    break;
                }
            }
        }
        public void ParseSide(Dictionary<string, SideType> sides)
        {
            SideType side;
            if (sides.TryGetValue(SideName, out side))
            {
                Side = side;
            }
        }


        string SideName
        {
            get;
            set;
        }

        public SideType Side
        {
            get;
            protected set;
        }

        public float ROFRatio
        {
            get;
            protected set;
        }
        public float ArmorInfantryRatio
        {
            get;
            protected set;
        }
        public float ArmorUnitsRatio
        {
            get;
            protected set;
        }
        public float ArmorAircraftRatio
        {
            get;
            protected set;
        }
        public float ArmorBuildingsRatio
        {
            get;
            protected set;
        }
        public float ArmorDefensesRatio
        {
            get;
            protected set;
        }
        public float CostInfantryRatio
        {
            get;
            protected set;
        }
        public float CostUnitsRatio
        {
            get;
            protected set;
        }
        public float CostAircraftRatio
        {
            get;
            protected set;
        }
        public float CostBuildingsRatio
        {
            get;
            protected set;
        }
        public float CostDefensesRatio
        {
            get;
            protected set;
        }

        public float SpeedInfantryRatio
        {
            get;
            protected set;
        }
        public float SpeedUnitsRatio
        {
            get;
            protected set;
        }
        public float SpeedAircraftRatio
        {
            get;
            protected set;
        }

        public float BuildTimeInfantryRatio
        {
            get;
            protected set;
        }
        public float BuildTimeUnitsRatio
        {
            get;
            protected set;
        }
        public float BuildTimeAircraftRatio
        {
            get;
            protected set;
        }
        public float BuildTimeBuildingsRatio
        {
            get;
            protected set;
        }
        public float BuildTimeDefensesRatio
        {
            get;
            protected set;
        }

        public float IncomeRatio
        {
            get;
            protected set;
        }

        public bool Multiplay
        {
            get;
            protected set;
        }

        public bool MultiplayPassive
        {
            get;
            protected set;
        }

        public string LoadScreenName
        {
            get;
            protected set;
        }
        public string FlagName
        {
            get;
            protected set;
        }




        public HouseType(string typeName,
            Dictionary<string, int> colors,
            Dictionary<string, TechnoType> uts)
            : base(typeName)
        {
            Colors = colors;
            UnitTypes = uts;

            MCVTypes = new List<UnitType>();
            MultiplayerStartupUTs = new List<TechnoType>();
        }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            UIName = ParseUIName(sect);

            if (Colors != null)
            {
                string color = sect["Color"];
                ColorScheme = Colors[color];
            }

            Multiplay = sect.GetBool("Multiplay", true);
            MultiplayPassive = sect.GetBool("MultiplayPassive", false);

            ArmorInfantryRatio = sect.GetFloat("ArmorInfantryMult", 1);
            ArmorUnitsRatio = sect.GetFloat("ArmorUnitsMult", 1);
            ArmorAircraftRatio = sect.GetFloat("ArmorAircraftMult", 1);
            ArmorBuildingsRatio = sect.GetFloat("ArmorBuildingsMult", 1);
            ArmorDefensesRatio = sect.GetFloat("ArmorDefensesMult", 1);

            ROFRatio = sect.GetFloat("ROF", 1);

            CostInfantryRatio = sect.GetFloat("CostInfantryMult", 1);
            CostUnitsRatio = sect.GetFloat("CostUnitsMult", 1);
            CostAircraftRatio = sect.GetFloat("CostAircraftMult", 1);
            CostBuildingsRatio = sect.GetFloat("CostBuildingsMult", 1);
            CostDefensesRatio = sect.GetFloat("CostDefensesMult", 1);

            SpeedInfantryRatio = sect.GetFloat("SpeedInfantryMult", 1);
            SpeedUnitsRatio = sect.GetFloat("SpeedUnitsMult", 1);
            SpeedAircraftRatio = sect.GetFloat("SpeedAircraftMult", 1);

            BuildTimeInfantryRatio = sect.GetFloat("BuildTimeInfantryMult", 1);
            BuildTimeUnitsRatio = sect.GetFloat("BuildTimeUnitsMult", 1);
            BuildTimeAircraftRatio = sect.GetFloat("BuildTimeAircraftMult", 1);
            BuildTimeBuildingsRatio = sect.GetFloat("BuildTimeBuildingsMult", 1);
            BuildTimeDefensesRatio = sect.GetFloat("BuildTimeDefensesMult", 1);

            IncomeRatio = sect.GetFloat("IncomeMult", 1);

            LoadScreenName = sect.GetString("LoadScreen", string.Empty);

            FlagName = sect.GetString("Flag", string.Empty);

            SideName = sect["Side"];

            veteranInf = new List<InfantryType>();
            veteranAir = new List<AircraftType>();
            veteranUnits = new List<UnitType>();

            if (UnitTypes != null)
            {
                string[] temp = sect.GetStringArray("VeteranInfantry", Utils.EmptyStringArray);
                for (int i = 0; i < temp.Length; i++)
                {
                    veteranInf.Add((InfantryType)UnitTypes[temp[i]]);
                }

                temp = sect.GetStringArray("VeteranUnits", Utils.EmptyStringArray);
                for (int i = 0; i < temp.Length; i++)
                {
                    veteranUnits.Add((UnitType)UnitTypes[temp[i]]);
                }


                temp = sect.GetStringArray("VeteranAircraft", Utils.EmptyStringArray);
                for (int i = 0; i < temp.Length; i++)
                {
                    veteranAir.Add((AircraftType)UnitTypes[temp[i]]);
                }
            }
        }

        public House CreateInstance(BattleField btfld)
        {
            return new House(btfld, this);
        }
        public House CreateInstance(BattleField btfld, ConfigurationSection sect)
        {
            House res = new House(btfld, this);
            res.Parse(sect);
            return res;
        }

    }

    public delegate void InfantryEventHandler(Infantry obj);
    public delegate void UnitEventHandler(Unit obj);
    public delegate void AircraftEventHandler(Aircraft obj);
    public delegate void DefBuildingEventHandler(Building obj);
    public delegate void ResBuildingEventHandler(Building obj);


    /// <summary>
    ///  player
    /// </summary>
    public class House : IConfigurable, IDisposable
    {
        Dictionary<House, bool> allyOrEnemy;

        #region Factory Props
        public List<Techno> BarrackList
        {
            get;
            protected set;
        }
        public List<Techno> WarFactoryList
        {
            get;
            protected set;
        }
        public List<Techno> ShipYardList
        {
            get;
            protected set;
        }
        public List<Techno> ConstructionYardList
        {
            get;
            protected set;
        }
        public List<Techno> AircraftPadList
        {
            get;
            protected set;
        }
        public List<Techno> RadarList
        {
            get;
            protected set;
        }

        public bool HasRadar
        {
            get { return RadarList.Count > 0; }
        }
        public bool HasBarracks
        {
            get { return BarrackList.Count > 0; }
        }
        public bool HasWarFactory
        {
            get { return WarFactoryList.Count > 0; }
        }
        public bool HasShipYard
        {
            get { return ShipYardList.Count > 0; }
        }
        public bool HasConYard
        {
            get { return ConstructionYardList.Count > 0; }
        }
        public bool HasAirPad
        {
            get { return AircraftPadList.Count > 0; }
        }
        #endregion

        #region TechnoLists
        public List<Techno> TechnoList
        {
            get;
            protected set;
        }

        public List<Building> ResBuildingList
        {
            get;
            protected set;
        }

        public List<Unit> UnitList
        {
            get;
            protected set;
        }

        public List<Aircraft> AircraftList
        {
            get;
            protected set;
        }

        public List<Infantry> InfantryList
        {
            get;
            protected set;
        }

        public List<Building> DefBuildingList
        {
            get;
            protected set;
        }


        public void AddUnit(Unit unit)
        {
            TechnoList.Add(unit);
            UnitList.Add(unit);

            OnUnitCreated(unit);
        }
        public void AddInfantry(Infantry inf)
        {
            TechnoList.Add(inf);
            InfantryList.Add(inf);

            OnInfantryCreated(inf);
        }
        public void AddBuilding(Building bld)
        {
            TechnoList.Add(bld);
            ResBuildingList.Add(bld);

            OnResBuildingCreated(bld);
        }
        public void AddDefense(Building bld)
        {
            TechnoList.Add(bld);
            DefBuildingList.Add(bld);

            OnDefBuildingCreated(bld);
        }
        public void AddAircraft(Aircraft air)
        {
            TechnoList.Add(air);
            AircraftList.Add(air);

            OnAircraftCreated(air);
        }

        public void RemoveUnit(Unit unit)
        {
            TechnoList.Remove(unit);
            UnitList.Remove(unit);

            OnUnitDeleted(unit);
        }
        public void RemoveInfantry(Infantry inf)
        {
            TechnoList.Remove(inf);
            InfantryList.Remove(inf);

            OnInfantryDeleted(inf);
        }
        public void RemoveBuilding(Building bld)
        {
            TechnoList.Remove(bld);
            ResBuildingList.Remove(bld);

            OnResBuildingDeleted(bld);
        }
        public void RemoveDefense(Building bld)
        {
            TechnoList.Remove(bld);
            DefBuildingList.Remove(bld);

            OnDefBuildingDeleted(bld);
        }
        public void RemoveAircraft(Aircraft air)
        {
            TechnoList.Remove(air);
            AircraftList.Remove(air);

            OnAircraftDeleted(air);
        }

        #endregion

        public BattleUI ParentUI
        {
            get;
            set;
        }

        public HouseType Country
        {
            get;
            set;
        }


        public TechTree TechnoTree
        {
            get;
            protected set;
        }

        public House(BattleField btfld, HouseType country)
        {
            Country = country;

            TechnoList = new List<Techno>(200);
            InfantryList = new List<Infantry>(100);
            UnitList = new List<Unit>(100);
            ResBuildingList = new List<Building>(100);
            DefBuildingList = new List<Building>(100);
            AircraftList = new List<Aircraft>(100);

            InfantryCostMOD = new List<Techno>();
            VehicleCostMOD = new List<Techno>();
            DefensesMoneyMOD = new List<Techno>();
            BuildingMoneyMOD = new List<Techno>();
            AircraftMoneyMOD = new List<Techno>();

            allyOrEnemy = new Dictionary<House, bool>();

            BarrackList = new List<Techno>();
            WarFactoryList = new List<Techno>();
            ShipYardList = new List<Techno>();
            ConstructionYardList = new List<Techno>();
            AircraftPadList = new List<Techno>();
            RadarList = new List<Techno>();

            if (btfld != null)
            {
                TechnoTree = new TechTree(btfld, this);
            }
        }

        public void Load(BattleField btfld)
        {
            TechnoTree = new TechTree(btfld, this);
        }

        // 先处理自己，然后才是TechTree

        #region unit events
        public event InfantryEventHandler InfantryCreated;
        public event UnitEventHandler UnitCreated;
        public event AircraftEventHandler AircraftCreated;
        public event DefBuildingEventHandler DefBuildingCreated;
        public event ResBuildingEventHandler ResBuildingCreated;

        public event InfantryEventHandler InfantryDeleted;
        public event UnitEventHandler UnitDeleted;
        public event AircraftEventHandler AircraftDeleted;
        public event DefBuildingEventHandler DefBuildingDeleted;
        public event ResBuildingEventHandler ResBuildingDeleted;


        void TechnoCreated(Techno obj)
        {
            if (obj.Type.Power > 0)
            {
                this.Power += obj.Type.Power;
            }
            else if (obj.Type.Power < 0)
            {
                this.PowerDrain -= obj.Type.Power;
            }

            switch (obj.Type.Factory)
            {
                case FactoryType.None:
                    break;
                case FactoryType.InfantryType:
                    BarrackList.Add(obj);
                    break;
                case FactoryType.BuildingType:
                    ConstructionYardList.Add(obj);
                    break;
                case FactoryType.UnitType:
                    if (obj.Type.Naval)
                    {
                        ShipYardList.Add(obj);
                    }
                    else
                    {
                        WarFactoryList.Add(obj);
                    }
                    break;
                case FactoryType.AircraftType:
                    AircraftPadList.Add(obj);
                    break;
                default:
                    throw new NotSupportedException(obj.Type.Factory.ToString());
            }

        }
        void TechnoDeleted(Techno obj)
        {
            if (obj.Type.Power > 0)
            {
                this.Power -= obj.Type.Power;
            }
            else if (obj.Type.Power < 0)
            {
                this.PowerDrain += obj.Type.Power;
            }

            switch (obj.Type.Factory)
            {
                case FactoryType.None:
                    break;
                case FactoryType.InfantryType:
                    BarrackList.Remove(obj);
                    break;
                case FactoryType.BuildingType:
                    ConstructionYardList.Remove(obj);
                    break;
                case FactoryType.UnitType:
                    if (obj.Type.Naval)
                    {
                        ShipYardList.Remove(obj);
                    }
                    else
                    {
                        WarFactoryList.Remove(obj);
                    }
                    break;
                case FactoryType.AircraftType:
                    AircraftPadList.Remove(obj);
                    break;
                default:
                    throw new NotSupportedException(obj.Type.Factory.ToString());
            }
        }

        protected void OnInfantryCreated(Infantry inf)
        {
            if (TechnoTree != null)
            { 
                TechnoCreated(inf);

                TechnoTree.InfantryCreated(inf);
                if (InfantryCreated != null)
                {
                    InfantryCreated(inf);
                }
            }
        }
        protected void OnUnitCreated(Unit unit)
        {
            if (TechnoTree != null)
            {  
                TechnoCreated(unit);

                TechnoTree.UnitCreated(unit);
                if (UnitCreated != null)
                {
                    UnitCreated(unit);
                }
            }
        }
        protected void OnAircraftCreated(Aircraft air)
        {
            if (TechnoTree != null)
            {
                TechnoCreated(air);

                TechnoTree.AirCreated(air);
                if (AircraftCreated != null)
                {
                    AircraftCreated(air);
                }
            }
        }
        protected void OnDefBuildingCreated(Building bld)
        {
            if (TechnoTree != null)
            {
                TechnoCreated(bld);

                TechnoTree.DefBuildingCreated(bld);
                if (DefBuildingCreated != null)
                {
                    DefBuildingCreated(bld);
                }
            }
        }
        protected void OnResBuildingCreated(Building bld)
        {
            if (TechnoTree != null)
            {  
                TechnoCreated(bld);

                TechnoTree.ResBuildingCreated(bld);
                if (ResBuildingCreated != null)
                {
                    ResBuildingCreated(bld);
                }
            }
        }


        protected void OnInfantryDeleted(Infantry inf)
        {
            if (TechnoTree != null)
            { 
                TechnoDeleted(inf);

                TechnoTree.InfantryDeleted(inf);
                if (InfantryDeleted != null)
                {
                    InfantryDeleted(inf);
                }
            }
        }
        protected void OnUnitDeleted(Unit unit)
        {
            if (TechnoTree != null)
            {  
                TechnoDeleted(unit);

                TechnoTree.UnitDeleted(unit);
                if (UnitDeleted != null)
                {
                    UnitDeleted(unit);
                }
            }
        }
        protected void OnAircraftDeleted(Aircraft air)
        {
            if (TechnoTree != null)
            {  
                TechnoDeleted(air);

                TechnoTree.AircraftDeleted(air);
                if (AircraftDeleted != null)
                {
                    AircraftDeleted(air);
                }
            }
        }
        protected void OnDefBuildingDeleted(Building bld)
        {
            if (TechnoTree != null)
            { 
                TechnoDeleted(bld);

                TechnoTree.DefBuildingDeleted(bld);
                if (DefBuildingDeleted != null)
                {
                    DefBuildingDeleted(bld);
                }
            }
        }
        protected void OnResBuildingDeleted(Building bld)
        {
            if (TechnoTree != null)
            { 
                TechnoDeleted(bld);

                TechnoTree.ResBuildingDeleted(bld);
                if (ResBuildingDeleted != null)
                {
                    ResBuildingDeleted(bld);
                }
            }
        }
        #endregion

        public event ConstructionOptionChangedHandler NewConstructionOptions
        {
            add
            {
                if (TechnoTree != null)
                    TechnoTree.NewConstructionOptions += value;
            }
            remove
            {
                if (TechnoTree != null)
                    TechnoTree.NewConstructionOptions -= value;
            }
        }
        public event ConstructionOptionChangedHandler LostConstructionOptions
        {
            add
            {
                if (TechnoTree != null)
                    TechnoTree.LostConstructionOptions += value;
            }
            remove
            {
                if (TechnoTree != null)
                    TechnoTree.LostConstructionOptions -= value;
            }
        }
        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            PlayerControl = sect.GetBool("PlayerControl", false);
            IQ = sect.GetInt("IQ", 0);
            TechLevel = sect.GetInt("TechLevel", 1);
            ColorName = sect["Color"];
            Credits = sect.GetInt("Credits", 0);

            AllyNames = sect.GetStringArray("Allies", null);

        }

        #endregion

        public void ParseColor(Dictionary<string, int> clrTable)
        {
            Color = clrTable[ColorName];
        }

        public void ParseOtherHouses(Dictionary<string, House> houses)
        {
            if (AllyNames != null)
            {
                for (int i = 0; i < AllyNames.Length; i++)
                {
                    allyOrEnemy.Add(houses[AllyNames[i]], true);
                }
            }
        }


        public float GetInfantryCostMult()
        {
            float res = 1;
            for (int i = 0; i < InfantryCostMOD.Count; i++)
            {
                res *= InfantryCostMOD[i].Type.InfantryCostBonus;
            }
            return res;
        }

        public float GetVehicleCostMult()
        {
            float res = 1;
            for (int i = 0; i < InfantryCostMOD.Count; i++)
            {
                res *= VehicleCostMOD[i].Type.UnitsCostBonus;
            }
            return res;
        }

        public float GetDefenseCostMult()
        {
            float res = 1;
            for (int i = 0; i < InfantryCostMOD.Count; i++)
            {
                res *= DefensesMoneyMOD[i].Type.DefensesCostBonus;
            }
            return res;
        }

        public float GetBuildingCostMult()
        {
            float res = 1;
            for (int i = 0; i < InfantryCostMOD.Count; i++)
            {
                res *= BuildingMoneyMOD[i].Type.BuildingsCostBonus;
            }
            return res;
        }

        public float GetAircraftCostMult()
        {
            float res = 1;
            for (int i = 0; i < InfantryCostMOD.Count; i++)
            {
                res *= AircraftMoneyMOD[i].Type.AircraftCostBonus;
            }
            return res;
        }

        public List<Techno> InfantryCostMOD
        {
            get;
            protected set;
        }

        public List<Techno> VehicleCostMOD
        {
            get;
            protected set;
        }

        /// <summary>
        ///  cost mult on defence buildings
        /// </summary>        
        public List<Techno> DefensesMoneyMOD
        {
            get;
            protected set;
        }

        /// <summary>
        /// cost mult on resource buildings
        /// </summary>
        public List<Techno> BuildingMoneyMOD
        {
            get;
            protected set;
        }

        /// <summary>
        /// cost mult on resource buildings
        /// </summary>
        public List<Techno> AircraftMoneyMOD
        {
            get;
            protected set;
        }

        /// <summary>
        ///  power produced by the power plants of this house
        /// </summary>
        public int Power
        {
            get;
            set;
        }

        public int PowerDrain
        {
            get;
            set;
        }

        public int Credits
        {
            get;
            set;
        }









        string[] AllyNames
        {
            get;
            set;
        }

        public bool PlayerControl
        {
            get;
            protected set;
        }

        public int IQ
        {
            get;
            protected set;
        }

        public int TechLevel
        {
            get;
            protected set;
        }

        string ColorName
        {
            get;
            set;
        }


        public int Color
        {
            get;
            set;
        }

        public bool IsAllyOrEnemy(House house)
        {
            bool res;
            if (allyOrEnemy.TryGetValue(house, out res))
            {
                return res;
            }
            return false;
        }


        public void SetPrimaryFactory(Techno techno)
        {
            switch (techno.Type.Factory)
            {
                case FactoryType.None:
                    break;
                case FactoryType.InfantryType:
                    for (int i = 0; i < BarrackList.Count; i++)
                    {
                        if (BarrackList[i] != techno)
                        {
                            techno.IsPrimaryFactory = false;
                        }
                    }
                    break;
                case FactoryType.BuildingType:
                    for (int i = 0; i < ConstructionYardList.Count; i++)
                    {
                        if (ConstructionYardList[i] != techno)
                        {
                            techno.IsPrimaryFactory = false;
                        }
                    } 
                    break;
                case FactoryType.AircraftType:
                    for (int i = 0; i < AircraftPadList.Count; i++)
                    {
                        if (AircraftPadList[i] != techno)
                        {
                            techno.IsPrimaryFactory = false;
                        }
                    }
                    break;
                case FactoryType.UnitType:
                    for (int i = 0; i < WarFactoryList.Count; i++)
                    {
                        if (WarFactoryList[i] != techno)
                        {
                            techno.IsPrimaryFactory = false;
                        }
                    }
                    break;
            }
        }

        public bool Disposed
        {
            get;
            protected set;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!Disposed)
            {
                InfantryCreated = null;
                UnitCreated = null;
                AircraftCreated = null;
                DefBuildingCreated = null;
                ResBuildingCreated = null;

                InfantryDeleted = null;
                UnitDeleted = null;
                AircraftDeleted = null;
                DefBuildingDeleted = null;
                ResBuildingDeleted = null;

                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }

}
