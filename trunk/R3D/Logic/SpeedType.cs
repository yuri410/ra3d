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
using R3D.IsoMap;

namespace R3D.Logic
{
    public unsafe class SpeedType : IConfigurable
    {
        float[] terrainWeight = new float[16];
        bool[] passableTable = new bool[16];

        public string TypeName
        {
            get;
            private set;
        }

        public SpeedType(string typeName)
        {
            TypeName = typeName;
        }

        public float[] TerrainWeight
        {
            get { return terrainWeight; }
        }
        public bool[] PassableTable
        {
            get { return passableTable; }
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            terrainWeight[(int)TerrainType.Clear] = sect.GetPercentage("Clear", 1);
            terrainWeight[(int)TerrainType.Ice] = sect.GetPercentage("Ice", 1);
            terrainWeight[(int)TerrainType.Tunnel] = sect.GetPercentage("Tunnel", 1);
            terrainWeight[(int)TerrainType.Railroad] = sect.GetPercentage("Railroad", 1);
            terrainWeight[(int)TerrainType.Rock] = sect.GetPercentage("Rock", 1);
            terrainWeight[(int)TerrainType.Water] = sect.GetPercentage("Water", 1);
            terrainWeight[(int)TerrainType.Beach] = sect.GetPercentage("Beach", 1);
            terrainWeight[(int)TerrainType.Road] = sect.GetPercentage("Road", 1);
            terrainWeight[(int)TerrainType.Rough] = sect.GetPercentage("Rough", 1);


            terrainWeight[13] = terrainWeight[(int)TerrainType.Clear];

            for (int i = 0; i < 16; i++)
            {
                passableTable[i] = terrainWeight[i] > float.Epsilon;
            }
        }

        #endregion
    }
}
