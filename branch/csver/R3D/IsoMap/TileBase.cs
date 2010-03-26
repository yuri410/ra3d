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
using System.Drawing;

using R3D.IO;
using R3D.Media;
using R3D.GraphicsEngine;
using R3D.Base;
using SlimDX;

namespace R3D.IsoMap
{
    public abstract class TileBase : IDisposable 
    {
        //protected TheaterBase theater;
        protected int blockCount;

        string name;

        protected string Name
        {
            get { return name; }
        }

        protected TileBase(string name)
        {
            this.name = name;
            //allowDynLoad = false;
            //resState = ResourceState.Loaded;
        }
        //protected TileBase(string name, TheaterBase theater)
        //    : base(TileManager.Instance, name)
        //{
        //    this.theater = theater;
        //}

        public int BlockCount
        {
            get { return blockCount; }
        }

        //public abstract ImageBase[] GetImages(string ext, int subTile);
        //public abstract ImageBase[][] GetImages(string ext);
        //public abstract ImageBase[][] GetImages(params object[] paras);
        //public abstract ImageBase[] GetImages(int subTile, params object[] paras);
        public abstract ImageBase[] GetImages(params object[] paras);
        public abstract ImageBase GetImage(int subTile, params object[] paras);

        //public abstract bool SupportsMatrial { get; }
        //public abstract bool SupportsDoubleLayer { get; }

        public virtual BlockBits GetBlockBits(int index)        
        {
            return BlockBits.None;
        }
        
        

        //protected TmpFileBase(string file, int size, bool isinMix)
        //    : base(file, size, isinMix) { }
        //public TheaterBase Theater
        //{
        //    get { return theater; }
        //    internal set { theater = value; }
        //}

        public abstract int GetHeight(int i);
        public abstract TerrainType GetTerrainType(int i);
        public abstract int GetRampType(int i);
        public abstract int GetSecRampType(int i);
        public abstract TerrainType GetSecTerrainType(int i);
        public abstract int GetSecHeight(int i);

        public abstract void GetRadarColor(int index, out Color left, out Color right);


        public virtual GameModel GetTileModel(int index)
        {
            return null;
        }

        public virtual void ReleaseTextures()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rampType"></param>
        /// <param name="v1">top</param>
        /// <param name="v2">left</param>
        /// <param name="v3">right</param>
        /// <param name="v4">bottom</param>
        /// <param name="vc">centre</param>
        public static void SetRampVertex(byte rampType, ref Vector3 v1, ref Vector3 v2, ref Vector3 v3, ref Vector3 v4, ref Vector3 vc)
        {
            SetRampVertexHeight(rampType, ref v1.Y, ref v2.Y, ref v3.Y, ref v4.Y, ref vc.Y);
        }

        public static void SetRampVertexHeight(byte rampType, ref float v1, ref float v2, ref float v3, ref float v4, ref float vc)
        {
            switch (rampType)
            {
                case 0:
                    break;
                case 1:
                    v3 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit * 0.5f;
                    break;
                case 2:
                    v2 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit * 0.5f;
                    break;
                case 3:
                    v1 += Terrain.VerticalUnit;
                    v2 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit * 0.5f;
                    break;
                case 4:
                    v1 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit * 0.5f;
                    break;

                case 5:
                    v4 += Terrain.VerticalUnit;
                    break;
                case 6:
                    v2 += Terrain.VerticalUnit;
                    break;
                case 7:
                    v1 += Terrain.VerticalUnit;
                    break;
                case 8:
                    v3 += Terrain.VerticalUnit;
                    break;

                case 9:
                    v2 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;
                case 10:
                    v1 += Terrain.VerticalUnit;
                    v2 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;
                case 11:
                    v1 += Terrain.VerticalUnit;
                    v2 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;
                case 12:
                    v1 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;

                case 13:
                    v2 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit * 2;
                    vc += Terrain.VerticalUnit;
                    break;
                case 14:
                    v1 += Terrain.VerticalUnit;
                    v2 += Terrain.VerticalUnit * 2;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;
                case 15:
                    v1 += Terrain.VerticalUnit * 2;
                    v2 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;
                case 16:
                    v1 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit * 2;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;

                case 17:
                    v2 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    break;
                case 18:
                    v1 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;
                case 19:
                    v2 += Terrain.VerticalUnit;
                    v3 += Terrain.VerticalUnit;
                    vc += Terrain.VerticalUnit;
                    break;
                case 20:
                    v1 += Terrain.VerticalUnit;
                    v4 += Terrain.VerticalUnit;
                    break;
            }

        }

        #region IDisposable 成员

        public abstract void Dispose();

        #endregion


        public virtual void RemapSubtileIndex(ref int p)
        {
            
        }
    }
}
