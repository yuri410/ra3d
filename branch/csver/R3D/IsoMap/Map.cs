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
using System.IO;
using System.Drawing;
using System.Runtime.InteropServices;

using SlimDX;

using R3D.Base;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.ConfigModel;
using R3D.Logic;

namespace R3D.IsoMap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IsoMapPackEntry
    {
        public ushort x;
        public ushort y;

        public short tile;
        public ushort zero1;
        //public ushort zero2;
        //public byte unk1;
        public byte subTile;
        public ushort z;
        //public ushort zero3;

        public override string ToString()
        {
            return "{ Tile= " + tile.ToString() + ", Pos=(" + x.ToString() + ", " + y.ToString() + ")}";
        }
    }


    /// <summary>
    /// 原版游戏地图
    /// </summary>
    public class Map : MapBase 
    {
        public delegate TheaterBase TheaterCreationCallback(string name);

        IsoMapPackEntry[] cells;

        HeightMap heightMap;

        TheaterBase theater;
        ConfigModel.Configuration config;

        string theaterName;

        public override string TheaterName
        {
            get { return theaterName; }
        }
        void BuildHeightMap()
        {
            int len = width + height - 1;
            heightMap = new HeightMap(len + 1, len + 1);

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i].x--;
                cells[i].y--;

                int x = cells[i].x;
                int y = cells[i].y;
                if (cells[i].tile == -1)
                    cells[i].tile = 0;

                if (theater[cells[i].tile] == null)
                {
                    cells[i].tile = 0;
                }
                TileBase tile = theater[cells[i].tile][0];

                if (cells[i].subTile >= tile.BlockCount)
                    cells[i].subTile = (byte)(tile.BlockCount - 1);

                switch (tile.GetRampType(cells[i].subTile))
                {
                    case 0:
                        heightMap[y, x] = (byte)(cells[i].z);     // 上
                        heightMap[y, x + 1] = (byte)(cells[i].z);     // 左
                        heightMap[y + 1, x] = (byte)(cells[i].z);     // 右
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);     // 下
                        break;
                    case 1:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 2:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 3:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 4:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 5:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 6:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 7:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 8:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 9:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 10:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 11:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 12:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 13:
                        heightMap[y, x] = (byte)(cells[i].z);     // 上
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);     // 左
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);     // 右
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 2);     // 下
                        break;
                    case 14:
                        heightMap[y, x] = (byte)(cells[i].z + 1);     // 上
                        heightMap[y, x + 1] = (byte)(cells[i].z + 2);     // 左
                        heightMap[y + 1, x] = (byte)(cells[i].z);     // 右
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);     // 下
                        break;
                    case 15:
                        heightMap[y, x] = (byte)(cells[i].z + 2);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 16:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 2);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 17:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 18:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    case 19:
                        heightMap[y, x] = (byte)(cells[i].z);
                        heightMap[y, x + 1] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x] = (byte)(cells[i].z + 1);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z);
                        break;
                    case 20:
                        heightMap[y, x] = (byte)(cells[i].z + 1);
                        heightMap[y, x + 1] = (byte)(cells[i].z);
                        heightMap[y + 1, x] = (byte)(cells[i].z);
                        heightMap[y + 1, x + 1] = (byte)(cells[i].z + 1);
                        break;
                    default:
                        throw new InvalidDataException();
                }

            }

            for (int i = 0; i < heightMap.Height; i++)
                for (int j = 0; j < heightMap.Width; j++)
                    heightMap[i, j] *= 16;
        }

        public override void Load()
        {           
        }

        public unsafe Map(IniConfiguration ini)
        {
            config = ini;

            ConfigurationSection sect = ini["Map"];
            Rectangle rect;
            sect.GetRectangle("Size", out rect);
            width = rect.Width;
            height = rect.Height;

            string theaterName = sect["Theater"];
            theater = TheaterManager.Instance.CreateInstance(theaterName);


            byte[] bytes = DecodeData(ini["IsoMapPack5"]);

            cellCount = width * height + (width - 1) * height;

            if (cellCount != bytes.Length / sizeof(IsoMapPackEntry))
            {
                throw new DataFormatException(ini.Name);
            }

            cells = new IsoMapPackEntry[cellCount];

            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            ArchiveBinaryReader br = new ArchiveBinaryReader(ms, Encoding.Default);

            for (int i = 0; i < cellCount; i++)
            {
                cells[i].x = br.ReadUInt16();
                cells[i].y = br.ReadUInt16();

                cells[i].tile = br.ReadInt16();
                cells[i].zero1 = br.ReadUInt16();
                cells[i].subTile = br.ReadByte();
                cells[i].z = br.ReadUInt16();
            }
            br.Close();


            BuildHeightMap();
        }
        public unsafe Map(IniConfiguration ini, TheaterCreationCallback tccb)
        {
            config = ini;

            ConfigurationSection sect = ini["Map"];
            Rectangle rect;
            sect.GetRectangle("Size", out rect);
            width = rect.Width;
            height = rect.Height;

            theaterName = sect["Theater"];
            theater = tccb(theaterName);// TheaterManager.Instance.CreateInstance(theaterName);


            byte[] bytes = DecodeData(ini["IsoMapPack5"]);

            cellCount = width * height + (width - 1) * height;

            if (cellCount != bytes.Length / sizeof(IsoMapPackEntry))
            {
                throw new DataFormatException(ini.Name);
            }

            cells = new IsoMapPackEntry[cellCount];

            System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes);
            ArchiveBinaryReader br = new ArchiveBinaryReader(ms, Encoding.Default);

            for (int i = 0; i < cellCount; i++)
            {
                cells[i].x = br.ReadUInt16();
                cells[i].y = br.ReadUInt16();

                cells[i].tile = br.ReadInt16();
                cells[i].zero1 = br.ReadUInt16();
                cells[i].subTile = br.ReadByte();
                cells[i].z = br.ReadUInt16();
            }
            br.Close();
            BuildHeightMap();
        }
        byte[] DecodeData(ConfigurationSection sect)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < int.MaxValue; i++)
            {
                string line;
                if (sect.TryGetValue(i.ToString(), out line))
                {
                    sb.Append(line);
                }
                else
                    break;
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


            byte[] bytes = Convert.FromBase64String(data + sb2.ToString()); // Crypto.Decoder.Decode64(src);
            return Crypto.Decoder.Decode5(bytes);
        }

        public static Map FromFile(string file)
        {
            return FromFile(new FileLocation(file));
        }
        public static Map FromFile(ResourceLocation fl)
        {
            return new Map(new IniConfiguration(fl));
        }

        public IsoMapPackEntry[] Cells
        {
            get { return cells; }
        }
        public override HeightMap GetHeightMap()
        {
            return heightMap;
        }

        public override Waypoint[] StartPoints
        {
            get { throw new NotSupportedException(); }
        }
        public override Waypoint[] Waypoints
        {
            get { throw new NotSupportedException(); }
        }
        public override TerrainObjectInfo[] TerrainObjects
        {
            get { throw new NotSupportedException(); }
        }
        public override MapAtmosphereInfo Atmosphere
        {
            get { throw new NotSupportedException(); }
        }
        public override TheaterBase TerrainTheater
        {
            get { return theater; }
        }
        public override unsafe void SetCellData(int cellIndex, bool isSecLayer, TerrainVertex* top, TerrainVertex* left, TerrainVertex* right, TerrainVertex* bottom, TerrainVertex* centre)
        {
            byte rampType = (byte)theater[cells[cellIndex].tile][0].GetRampType(cells[cellIndex].subTile);

            TileBase.SetRampVertexHeight(rampType, ref top->pos.Y, ref left->pos.Y, ref right->pos.Y, ref bottom->pos.Y, ref centre->pos.Y);
        }
        public override CellData[] GetCellData()
        {
            int len = cells.Length;
            CellData[] res = new CellData[len];

            for (int i = 0; i < len; i++)
            {
                TileBase tile = theater[cells[i].tile][0];
                
                res[i].x = cells[i].x;
                res[i].y = cells[i].y;
                res[i].tile = (ushort)cells[i].tile;
                res[i].z = (short)(cells[i].z);
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
