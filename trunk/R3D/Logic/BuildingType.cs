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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using R3D.ConfigModel;
using R3D.IsoMap;

namespace R3D.Logic
{
    public class FoundationInfo : IConfigurable
    {   
        static char[] FoundationSpilter = new char[2] { 'x', 'X' };

        /// <summary>
        ///  
        /// </summary>
        /// <remarks>
        /// 索引顺序X,Y
        ///  X: 左上至右下  
        /// </remarks>
        bool[][] usage;        
        bool[][] buffer;

        /// <summary>
        ///
        /// </summary>
        /// <remarks>X</remarks>
        public int Width
        {
            get;
            protected set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Y</remarks>
        public int Height
        {
            get;
            protected set;
        }

        public FoundationInfo()
        { }
        public FoundationInfo(bool createDef)
        {
            if (createDef)
            {
                usage = new bool[1][];
                buffer = new bool[1][];
                usage[0] = new bool[1];
                buffer[0] = new bool[1];
                usage[0][0] = true;
            }
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            string fSize = sect["Foundation"];

            string[] v = fSize.Split(FoundationSpilter, StringSplitOptions.RemoveEmptyEntries);

            Width = int.Parse(v[0]);
            Height = int.Parse(v[1]);

            usage = new bool[Width][];
            buffer = new bool[Width][];
            for (int i = 0; i < Width; i++)
            {
                usage[i] = new bool[Height];
                buffer[i] = new bool[Height];
                for (int j = 0; j < Height; j++)
                {
                    usage[i][j] = true;
                }
            }

            for (int i = 0; i < Height; i++)
            {
                if (sect.TryGetValue("FoundationLine" + i.ToString(), out fSize))
                {
                    for (int j = 0; j < Width; j++)
                    {
                        usage[j][i] = fSize[j] == '1';
                    }
                }
                else
                {
                    break;
                }
            }

        }

        #endregion


        public void SetUsage(int x, int y, bool value)
        {
            usage[x][y] = value;
        }
        public bool GetUsage(int x,int y)
        {
            return usage[x][y];
        }

        public bool Check(int px, int py, BattleField field)
        {
            TerrainType[][] terrType = field.PathFinder.TerrainTable;
            BuildableTable bdTable = field.BuildableTable;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (usage[i][j])
                    {
                        if (!bdTable.IsBuildable(terrType[i + px][j + py]))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool[][] CheckCell(int px, int py, BattleField field)
        {
            TerrainType[][] terrType = field.PathFinder.TerrainTable;
            BuildableTable bdTable = field.BuildableTable;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    buffer[i][j] = usage[i][j] ? bdTable.IsBuildable(terrType[i + px][j + py]) : false;
                }
            }
            return buffer;
        }

        public float GetBoundingBallRandius()
        {
            return (float)Math.Sqrt(Width * Width + Height * Height) * 0.5f;
        }
        public float GetBoundingBallX()
        {
            return Width * 0.5f;
        }

        public float GetBoundingBallZ()
        {
            return Height * 0.5f;
        }
    }

    public class BuildingType : TechnoType
    {

        public BuildingType(BattleField btfld, string typeName)
            : base(btfld, typeName)
        { }

        public override Techno CreateInstance(House owner)
        {
            return new Building(Field, owner, this);
        }

        public FoundationInfo Foundation
        {
            get;
            set;
        }


        public int Adjacent
        {
            get;
            protected set;
        }
        public bool IsBaseDefense
        {
            get;
            protected set;
        }

        public bool Capturable 
        {
            get;
            protected set;
        }


        public bool BaseNormal
        {
            get;
            protected set;
        }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            ImmuneToPsionics = sect.GetBool("ImmuneToPsionics", true);
            Adjacent = sect.GetInt("Adjacent", 1);
            IsBaseDefense = sect.GetBool("IsBaseDefense", false);

            Capturable = sect.GetBool("Capturable", false);

            BaseNormal = sect.GetBool("BaseNormal", false);
        }

        public override void LoadGraphics(Configuration config)
        {
            base.LoadGraphics(config);

            ConfigurationSection sect;

            if (config.TryGetValue(base.TypeName, out sect))
            {
                Foundation = new FoundationInfo();
                Foundation.Parse(sect);
            }
            else
            {
                Foundation = new FoundationInfo(true);
            }
        }

        public bool IsBuildable(int x, int y)
        {
            return Foundation.Check(x, y, Field);
            //int maxX = x;
        }

        /// <summary>
        /// Gets the ingame cost of the building.
        /// </summary>
        /// <returns></returns>
        public override int GetCost(House owner)
        {
            if (owner != null)
            {
                float factoryRatio = owner.GetBuildingCostMult();
                float countryRatio = owner.Country.CostBuildingsRatio;

                return (int)(Cost * factoryRatio * countryRatio);
            }
            return base.GetCost(owner);
        }

        public override TechnoCategory WhatAmI
        {
            get
            {
                return TechnoCategory.Building;
            }
        }
    }
}
