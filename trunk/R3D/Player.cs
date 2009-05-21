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
using R3D.Core;
using R3D.Logic;

namespace R3D
{
    public enum PlayerType
    {
        /// <summary>
        /// 本客户端电脑玩家
        /// </summary>
        Computer,
        /// <summary>
        /// 本客户端人类玩家
        /// </summary>
        LocalHuman,
        /// <summary>
        /// 远程玩家（人类+电脑）
        /// </summary>
        Remote
    }
    public enum ComputerPlayerLevel
    {
        Easy,
        Medium,
        Hard
    }

    public struct PlayerInfo
    {
        int startX;
        int startY;

        PlayerType type;
        //int color;
        List<House> houses;
        //int team;

        string name;
        ComputerPlayerLevel computerLvl;


        public PlayerInfo(string name, int sx, int sy, PlayerType type,  List<House> houses)
        {
            this.name = name;
            this.startX = sx;
            this.startY = sy;

            this.type = type;
            this.houses = houses;
            computerLvl = ComputerPlayerLevel.Easy;

        }
        public PlayerInfo(string name, int sx, int sy, PlayerType type, List<House> houses, ComputerPlayerLevel lvl)
        {
            this.name = name;
            this.startX = sx;
            this.startY = sy;

            this.type = type;
            this.houses = houses;
            this.computerLvl = lvl;

        }

        public int StartX
        {
            get { return startX; }
            set { startX = value; }
        }
        public int StartY
        {
            get { return startY; }
            set { startY = value; }
        }
        public PlayerType Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public List<House> Houses
        {
            get { return houses; }
            set { houses = value; }
        }

    }


    public delegate void ControlledHousesChangedHandler();

    public class BattleStatsticInfo
    {
        string name;

        int bldLosses;
        int infLosses;
        int unitLosses;
        int airLosses;
        int othLosses;

        int bldKills;
        int infKills;
        int unitKills;
        int airKills;
        int othKills;

        int bldBlds;
        int infBlds;
        int unitBlds;
        int airBlds;
        int othBlds;


        int points;

        public BattleStatsticInfo(string name)
        {
            this.name = name;
            bldLosses = 0;
            infLosses = 0;
            unitLosses = 0;
            airLosses = 0;
            othLosses = 0;

            bldKills = 0;
            infKills = 0;
            unitKills = 0;
            airKills = 0;
            othKills = 0;


            bldBlds = 0;
            infBlds = 0;
            unitBlds = 0;
            airBlds = 0;
            othBlds = 0;

            points = 0;
        }

        public string PlayerName
        {
            get { return name; }
            private set { name = value; }
        }

        public int BuildingLosses
        {
            get { return bldLosses; }
            private set { bldLosses = value; }
        }
        public int InfantryLosses
        {
            get { return infLosses; }
            private set { infLosses = value; }
        }
        public int UnitLosses
        {
            get { return unitLosses; }
            private set { unitLosses = value; }
        }
        public int AircraftLosses
        {
            get { return airLosses; }
            private set { airLosses = value; }
        }
        public int OtherLosses
        {
            get { return othLosses; }
            private set { othLosses = value; }
        }

        public int Losses
        {
            get { return bldLosses + infLosses + unitLosses + airLosses + othLosses; }
        }

        public int BuildingKills
        {
            get { return bldKills; }
            private set { bldKills = value; }
        }
        public int InfantryKills
        {
            get { return infKills; }
            private set { infKills = value; }
        }
        public int UnitKills
        {
            get { return unitKills; }
            private set { unitKills = value; }
        }
        public int AircraftKills
        {
            get { return airKills; }
            private set { othKills = value; }
        }
        public int OtherKills
        {
            get { return othKills; }
            private set { othKills = value; }
        }

        public int Kills
        {
            get { return bldKills + infKills + unitKills + airKills + othKills; }
        }


        public int BuildingBuilds
        {
            get { return bldBlds; }
            private set { bldBlds = value; }
        }
        public int InfantryBuilds
        {
            get { return infBlds; }
            private set { infBlds = value; }
        }
        public int UnitBuilds
        {
            get { return unitBlds; }
            private set { unitBlds = value; }
        }
        public int AircraftBuilds
        {
            get { return airBlds; }
            private set { airBlds = value; }
        }
        public int OtherBuilds
        {
            get { return othBlds; }
            private set { othBlds = value; }
        }

        public int Builds
        {
            get { return bldBlds + infBlds + unitBlds + airBlds + othBlds; }
        }


        public int Points
        {
            get { return points; }
            private set { points = value > 0 ? value : 0; }
        }

        public void Built(Techno tech)
        {
            if (!tech.Type.DontScore)
            {
                switch (tech.Type.WhatAmI)
                {
                    case TechnoCategory.Aircraft:
                        AircraftBuilds++;
                        break;
                    case TechnoCategory.Building:
                        BuildingBuilds++;
                        break;
                    case TechnoCategory.Infantry:
                        InfantryBuilds++;
                        break;
                    case TechnoCategory.Unit:
                        UnitBuilds++;
                        break;
                    case TechnoCategory.Unknown:
                        OtherBuilds++;
                        break;
                }

                Points += tech.Type.Points / 3;
            }
        }

        public void Killed(Techno tech)
        {
            if (!tech.Type.DontScore)
            {
                switch (tech.Type.WhatAmI)
                {
                    case TechnoCategory.Aircraft:
                        AircraftKills++;
                        break;
                    case TechnoCategory.Building:
                        BuildingKills++;
                        break;
                    case TechnoCategory.Infantry:
                        InfantryKills++;
                        break;
                    case TechnoCategory.Unit:
                        UnitKills++;
                        break;
                    case TechnoCategory.Unknown:
                        OtherKills++;
                        break;
                }
                Points += tech.Type.Points;
            }
        }

        public void Lost(Building tech)
        {
            if (!tech.Type.DontScore)
            {
                switch (tech.Type.WhatAmI)
                {
                    case TechnoCategory.Aircraft:
                        AircraftLosses++;
                        break;
                    case TechnoCategory.Building:
                        BuildingLosses++;
                        break;
                    case TechnoCategory.Infantry:
                        InfantryLosses++;
                        break;
                    case TechnoCategory.Unit:
                        UnitLosses++;
                        break;
                    case TechnoCategory.Unknown:
                        OtherLosses++;
                        break;
                }
                Points -= tech.Type.Points / 2;
            }
        }

        public int  CalculateFinalScore(TimeSpan time, bool isWinning)
        {
            int points = isWinning ? (Points + Points / 2) : Points;            
            int minutes = time.Minutes / 10;

            if (minutes == 0)
            {
                minutes = 1;
            }

            return points / minutes;
        }

    }

    /// <summary>
    /// 代表真人或电脑玩家
    /// </summary>
    public abstract class Player
    {
        List<House> controledHouse;
        string name;

        int startX;
        int startY;

        protected Battle battle;

        protected Player(Battle battle, PlayerInfo info)
        {
            this.battle = battle;

            controledHouse = info.Houses;

            name = info.Name;

            startX = info.StartX;
            startY = info.StartY;
        }

        public abstract PlayerType Type
        {
            get;
        }

        public int StartX
        {
            get { return startX; }
        }
        public int StartY
        {
            get { return startY; }
        }
        public string Name
        {
            get { return name; }
        }

        //public List<House> ControlledHouses
        //{
        //    get { return controledHouse; }
        //}


        public House GetControlledHouse(int index)
        {
            return controledHouse[index];
        }

        public int ControlledHouseCount
        {
            get { return controledHouse.Count; }
        }

        public void AddControlledHouse(House house)
        {
            controledHouse.Add(house);
            if (ControlledHousesChanged != null)
            {
                ControlledHousesChanged();
            }
        }

        public void RemoveControlledHouse(House house)
        {
            controledHouse.Remove(house);
            if (ControlledHousesChanged != null)
            {
                ControlledHousesChanged();
            }
        }



        public event ControlledHousesChangedHandler ControlledHousesChanged;
        
    }

    public class ComputerPlayer : Player
    {
        public ComputerPlayer(Battle battle, PlayerInfo info)
            : base(battle, info)
        {
        }

        public override PlayerType Type
        {
            get { return PlayerType.Computer; }
        }
    }

    public class HumanPlayer : Player
    {

        public HumanPlayer(Battle battle, PlayerInfo info)
            : base(battle, info)
        {

        }


        public override PlayerType Type
        {
            get { return PlayerType.LocalHuman; }
        }

        public Camera Camera
        {
            get;
            set;
        }
    }
}
