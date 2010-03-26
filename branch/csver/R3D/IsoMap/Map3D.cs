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
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Text;
using R3D.ConfigModel;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.Logic;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.IsoMap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IsoMapPackEntry3D
    {
        public ushort x;
        public ushort y;
        public short tile;
        public byte subTile;

        public float top;
        public float left;
        public float right;
        public float bottom;
        public float centre;

        public float secTop;
        public float secLeft;
        public float secRight;
        public float secBottom;
        public float secCentre;

        

        public override string ToString()
        {
            return "Tile= " + tile.ToString() + ", Pos=(" + x.ToString() + ", " + y.ToString() + ")";
        }
    }

    /// <summary>
    /// 地图，包括地图上的东西，逻辑
    /// </summary>
    public class Map3D : MapBase
    {
        IsoMapPackEntry3D[] cells;

        HeightMap heightMap;

        TheaterBase theater;
        Waypoint[] waypoints;
        TerrainObjectInfo[] terrObjs;
        MapAtmosphereInfo atmos;

        Waypoint[] startPoints;

        ConfigModel.Configuration config;
        string theaterName;

        public override string TheaterName
        {
            get { return theaterName; }
        }


        unsafe byte[] DecodeData(ConfigurationSection sect)
        {
            StringBuilder sb = new StringBuilder(sect.Count + 1);

            foreach (string line in sect.Values)
            {
                sb.Append(line);
            }
            int len = sb.Length;
            string data = sb.ToString();

            int pos = data.IndexOf('=');
            if (pos != -1)
            {
                data = data.Substring(0, pos);
                len = data.Length;
            }


            StringBuilder sb2 = new StringBuilder();
            while (len % 4 != 0)
            {
                sb2.Append('=');
                len++;
            }


            byte[] bytes = Convert.FromBase64String(data + sb2.ToString());

            System.IO.MemoryStream src = new System.IO.MemoryStream(bytes);
            GZipStream dec = new GZipStream(src, CompressionMode.Decompress);


            System.IO.MemoryStream dst = new System.IO.MemoryStream(bytes.Length * 3);

            BinaryReader br = new BinaryReader(dec,Encoding.Default );


            byte[] buffer;
            do
            {
                buffer = br.ReadBytes(1024);

                dst.Write(buffer, 0, buffer.Length);
            }
            while (buffer.Length == 1024);

            br.Close ();

            return dst.ToArray();
        }

        public override void Load()
        {
            R3D.ConfigModel.Configuration ini = config;
            ConfigurationSection sect = ini["Map"];

            Rectangle rect;
            sect.GetRectangle("Size", out rect);
            width = rect.Width;
            height = rect.Height;

            theaterName = sect["Theater"];
            theater = TheaterManager.Instance.CreateInstance(theaterName);


            byte[] bytes = DecodeData(ini["IsoMapPack5"]);

            cellCount = width * height + (width - 1) * height;

            //if (cellCount != bytes.Length / sizeof(IsoMapPackEntry3D))
            //{
            //    throw new DataFormatException(ini.Name);
            //}

            cells = new IsoMapPackEntry3D[cellCount];

            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            ArchiveBinaryReader br = new ArchiveBinaryReader(ms, Encoding.Default);

            for (int i = 0; i < cellCount; i++)
            {
                cells[i].x = br.ReadUInt16();
                cells[i].y = br.ReadUInt16();

                short tile = br.ReadInt16();
                byte subTile = br.ReadByte();
                cells[i].tile = tile;// br.ReadInt16();
                cells[i].subTile = subTile;// br.ReadByte();

                if (theater[tile][0] != null && theater[tile][0].BlockCount <= subTile)
                {
                    cells[i].subTile = (byte)(theater[tile][0].BlockCount - 1);
                }

                cells[i].top = br.ReadSingle();
                cells[i].left = br.ReadSingle();
                cells[i].right = br.ReadSingle();
                cells[i].bottom = br.ReadSingle();
                cells[i].centre = br.ReadSingle();

                cells[i].secTop = br.ReadSingle();
                cells[i].secLeft = br.ReadSingle();
                cells[i].secRight = br.ReadSingle();
                cells[i].secBottom = br.ReadSingle();
                cells[i].secCentre = br.ReadSingle();

            }
            br.Close();

            int len = width + height - 1;
            heightMap = new HeightMap(len + 1, len + 1);


            sect = ini["Waypoints"];
            List<Waypoint> wps = new List<Waypoint>(200);
            foreach (KeyValuePair<string, string> e in sect)
            {
                Waypoint wp;
                wp.Index = int.Parse(e.Key);

                int wpv = int.Parse(e.Value);

                wp.Y = wpv / 1000;
                wp.X = wpv % 1000;

                wps.Add(wp);
            }
            waypoints = wps.ToArray();

            if (ini.TryGetValue("Terrain", out sect))
            {
                //sect = ini["Terrain"];
                List<TerrainObjectInfo> tos = new List<TerrainObjectInfo>(300);
                foreach (KeyValuePair<string, string> e in sect)
                {
                    TerrainObjectInfo obj;
                    obj.TypeName = e.Value;

                    int wpv = int.Parse(e.Key);
                    obj.Y = wpv / 1000;
                    obj.X = wpv % 1000;

                    tos.Add(obj);
                }
                terrObjs = tos.ToArray();
            }
            else
                terrObjs = new TerrainObjectInfo[0];

            sect = ini["Atmosphere"];
            atmos.Parse(sect);


        }



        private unsafe Map3D(ResourceLocation fl)
        {
            ConfigModel.Configuration ini = new IniConfiguration(fl);
            
            config = ini;

            ConfigurationSection sect;
            if (ini.TryGetValue("Header", out sect))
            {
                int spCount = sect.GetInt("NumberStartingPoints");
                startPoints = new Waypoint[spCount];

                for (int i = 0; i < spCount; i++)
                {
                    startPoints[i].Index = i;

                    Point pt = sect.GetPoint("Waypoint" + (i + 1).ToString());

                    startPoints[i].X = pt.X;
                    startPoints[i].Y = pt.Y;
                }
            }
            else
            {
                List<Waypoint> spList = new List<Waypoint>(9);
                for (int i = 0; i < waypoints.Length && spList.Count < 9; i++)
                {
                    if (waypoints[i].Index < 8 && waypoints[i].Index >= 0)
                    {
                        spList.Add(waypoints[i]);
                    }
                }

                startPoints = spList.ToArray();
            }
        }


        public static Map3D FromFile(string file)
        {
            return FromFile(new FileLocation(file));
        }
        public static Map3D FromFile(ResourceLocation fl)
        {
            return new Map3D(fl);
        }

        public override Waypoint[] StartPoints
        {
            get { return startPoints; }
        }
        public override TerrainObjectInfo[] TerrainObjects
        {
            get { return terrObjs; }
        }
        public override Waypoint[] Waypoints
        {
            get { return waypoints; }
        }
        public override TheaterBase TerrainTheater
        {
            get { return theater; }
        }
        public override MapAtmosphereInfo Atmosphere
        {
            get { return atmos; }
        }

        public override HeightMap GetHeightMap()
        {
            return heightMap;
        }
        public unsafe override void SetCellData(int cellIndex, bool isSecLayer, TerrainVertex* top, TerrainVertex* left, TerrainVertex* right, TerrainVertex* bottom, TerrainVertex* centre)
        {
            if (!isSecLayer)
            {
                top->pos.Y = cells[cellIndex].top;
                left->pos.Y = cells[cellIndex].left;
                right->pos.Y = cells[cellIndex].right;
                bottom->pos.Y = cells[cellIndex].bottom;
                centre->pos.Y = cells[cellIndex].centre;
            }
            else
            {
                top->pos.Y = cells[cellIndex].secTop;
                left->pos.Y = cells[cellIndex].secLeft;
                right->pos.Y = cells[cellIndex].secRight;
                bottom->pos.Y = cells[cellIndex].secBottom;
                centre->pos.Y = cells[cellIndex].secCentre;
 
            }
        }

        public override CellData[] GetCellData()
        {
            int len = cells.Length;
            CellData[] res = new CellData[len];

            for (int i = 0; i < len; i++)
            {
                res[i].x = cells[i].x;
                res[i].y = cells[i].y;
                res[i].tile = (ushort)cells[i].tile;
                res[i].z = (short)(cells[i].centre / Terrain.VerticalUnit);
                res[i].subTile = cells[i].subTile;
            }
            return res;
        }

        public override ConfigModel.Configuration Config
        {
            get { return config; }
        }
    }
}
