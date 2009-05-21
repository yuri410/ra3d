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
using R3D.ConfigModel;
using R3D.Base;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.Logic;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.IsoMap
{
    public struct TerrainObjectInfo
    {
        public string TypeName;
        public int X;
        public int Y;
    }

    public struct MapAtmosphereInfo : IConfigurable
    {
        public Color4 ambientColor;
        public Color4 diffuseColor;
        public Color4 specularColor;

        public string skyName;

        public WeatherType weather;
        public float dayLength;

        public bool startWithRealTime;


        public FogMode fogMode;
        public float fogStart;
        public float fogEnd;
        public float fogDensity;
        public int fogColor;

        public bool FogEnabled
        {
            get { return fogMode != FogMode.None; }
        }
        public bool HasSky
        {
            get { return !string.IsNullOrEmpty(skyName); }
        }
        public bool HasDayNight
        {
            get { return dayLength > 10; }
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            dayLength = sect.GetFloat("DayLength", 0);
            startWithRealTime = sect.GetBool("StartWithRealTime", false);
            weather = (WeatherType)Enum.Parse(typeof(WeatherType), sect.GetString("WeatherType", "None"), true);

            skyName = sect.GetString("Sky", "DefaultSkyBox");

            ambientColor = new Color4(
                    sect.GetFloat("AmbientRed", 0.3f),
                    sect.GetFloat("AmbientGreen", 0.3f),
                    sect.GetFloat("AmbientBlue", 0.3f));
            diffuseColor = new Color4(
                    sect.GetFloat("DiffuseRed", 0.6f),
                    sect.GetFloat("DiffuseGreen", 0.6f),
                    sect.GetFloat("DiffuseBlue", 0.6f));
            specularColor = new Color4(
                    sect.GetFloat("SpecularRed", 0.0f),
                    sect.GetFloat("SpecularGreen", 0.0f),
                    sect.GetFloat("SpecularBlue", 0.0f));


            fogMode = (FogMode)Enum.Parse(typeof(FogMode), sect.GetString("FogMode", "None"), true);
            fogDensity = sect.GetFloat("FogDensity", 0.002f);
            fogStart = sect.GetFloat("FogStart", 150f);
            fogEnd = sect.GetFloat("FogEnd", 200);
            fogColor = sect.GetColorRGBA("FogColor", Color.DarkGray).ToArgb();

        }

        #endregion
    }


    public class MapInfo : MapInfoBase
    {
        public MapInfo(ResourceLocation fl)
        {
            IniConfiguration ini = new IniConfiguration(fl);

            ConfigurationSection sect = ini["Map"];

            Rectangle rect;
            sect.GetRectangle("Size", out rect);

            width = rect.Width;
            height = rect.Height;
            theater = sect["Theater"];

            sect = ini["Preview"];
            sect.GetRectangle("Size", out rect);
            prevWidth = rect.Width;
            prevHeight = rect.Height;


        }

    }



    /// <summary>
    /// 用来在不完全加载地图的情况下获取地图信息
    /// </summary>
    /// <remarks>抽象的，和相应Map类的工厂对应</remarks>
    public abstract class MapInfoBase
    {
        protected int width;
        protected int height;

        protected int prevWidth;
        protected int prevHeight;

        protected ResourceLocation mapFile;
        protected int maxPlayers;
        protected string name;
        protected string theater;

        

        public int PreviewWidth
        {
            get { return prevWidth; }
        }
        public int PreviewHeight
        {
            get { return prevHeight; }
        }
        public ImageBase GetPreview()
        {
            throw new NotImplementedException();
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }
        public int MaxPlayers
        {
            get { return maxPlayers; }
        }
    }

    /// <summary>
    /// 地图，包括地图上的东西，逻辑
    /// </summary>
    /// <remarks>
    /// 所有种类的地图都应实现：
    ///   提供HeightMap
    ///   提供基本信息
    ///   提供VertexData（纹理坐标）
    /// </remarks>
    public abstract class MapBase
    {
        protected int width;
        protected int height;
        protected int cellCount;


        public abstract Waypoint[] StartPoints
        {
            get;
        }

        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public abstract string TheaterName
        {
            get;
        }
        public abstract TheaterBase TerrainTheater
        {
            get;
        }

        public int CellCount
        {
            get { return cellCount; }
        }

        //protected MapFileBase(string file, int size, bool isinMix)
        //    : base(file, size, isinMix) { }
        public abstract Waypoint[] Waypoints
        {
            get;
        }
        public abstract TerrainObjectInfo[] TerrainObjects
        {
            get;
        }
        public abstract MapAtmosphereInfo Atmosphere
        {
            get;
        }

        public abstract HeightMap GetHeightMap();

        public unsafe abstract void SetCellData(int cellIndex, bool isSecLayer, TerrainVertex* top, TerrainVertex* left, TerrainVertex* right, TerrainVertex* bottom, TerrainVertex* centre);

        public abstract CellData[] GetCellData();

        public abstract ConfigModel.Configuration Config
        {
            get;
        }


        /// <summary>
        /// load the resources used by the map
        /// </summary>
        public abstract void Load();

    }

    public struct CellData
    {
        public ushort x;
        public ushort y;
        public short z;

        public ushort tile;
        public ushort subTile;

        public override string ToString()
        {
            return "Tile= " + tile.ToString() + ", Pos=(" + x.ToString() + ", " + y.ToString() + ")";
        }
    }
}
