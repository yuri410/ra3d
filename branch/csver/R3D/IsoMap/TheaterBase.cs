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

using R3D.Media;
using R3D.Base;
using R3D.ConfigModel;
using R3D.IO;
using SlimDX.Direct3D9;
namespace R3D.IsoMap
{
    /// <summary>
    /// for world builder use
    /// </summary>
    public class ExtendedTileSet : TileSet
    {
        //bool morphable;
        //bool shadowCaster;

        public bool Morphable
        {
            get;
            protected set;
        }
        //public bool ShadowCaster
        //{
        //    get;
        //    protected set;
        //}
        public bool AllowToPlace
        {
            get;
            protected set;
        }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            AllowToPlace = sect.GetBool("AllowToPlace", true);
            Morphable = sect.GetBool("Morphable", true);

        }
    }
    public class TileSet : IConfigurable
    {
        TileBase[][] tiles;

        int tileCount;

        int highRadarColor;
        int lowRadarColor;

        string fileName;

        public TileBase[][] Tiles
        {
            get { return tiles; }
        }
        public TileBase[] this[int index]
        {
            get { return tiles[index]; }
        }
        public string Name
        {
            get;
            protected set;
        }
        public int TileCount
        {
            get { return tileCount; }
        }
        public int HighRadarColor
        {
            get { return highRadarColor; }
        }
        public int LowRadarColor
        {
            get { return lowRadarColor; }
        }
        public string FileName
        {
            get { return fileName; }
        }

        public bool AllowTiberium
        {
            get;
            protected set;
        }
        public bool AllowBurrowing
        {
            get;
            protected set;
        }

        public void LoadTiles( FileSystem fs,TheaterBase theater, Device device, string tileExtension, string[] archivePaths)
        {
            tiles = new TileBase[tileCount][];
            for (int i = 0; i < tileCount; i++)
            {
                List<TileBase> subList = new List<TileBase>();

                string fn = fileName;
                if (i < 9)
                    fn += '0';
                fn += (i + 1).ToString();

                // 第一个形态
                FileLocation tileFl = fs.TryLocate(fn + tileExtension, archivePaths, FileSystem.GameResLR);
                if (tileFl == null)
                {
                    GameConsole.Instance.Write(ResourceAssembly.Instance.CM_TileFileMissing(fn), ConsoleMessageType.Exclamation);
                    // 该tileSet没有地块
                    break;
                }
                subList.Add(theater.CreateTile(device, tileFl)); //Tile3D.FromFile(device, tileFl);

                for (char k = 'a'; k <= 'z'; k++)
                {
                    tileFl = fs.TryLocate(fn + k + tileExtension, archivePaths, FileSystem.GameResLR);
                    if (tileFl != null)
                    {
                        subList.Add(theater.CreateTile(device, tileFl)); // Tile3D.FromFile(device, tileFl);
                    }
                    else
                    {
                        break;
                    }
                }

                tiles[i] = subList.ToArray();
            }
            //tiles = new TileBase[tileCount][];
            //for (int i = 0; i < tileCount; i++)
            //{
            //    List<TileBase> subList = new List<TileBase>();

            //    string fn = fileName;
            //    if (i < 9)
            //        fn += '0';
            //    fn += (i + 1).ToString();


            //    ArchiveFileEntry ent;
            //    string tmpName = fn + tileExtension;
            //    // 第一个形态
            //    if (arc.Find(tmpName, out ent))
            //        subList.Add(theater.CreateTile(device, new FileLocation(arc, FileSystem.CombinePath(arc.FilePath, tmpName), ent)));
            //    else
            //        break;

            //    for (char k = 'a'; k <= 'z'; k++)
            //    {
            //        tmpName = fn + k + tileExtension;
            //        if (arc.Find(tmpName, out ent))
            //            subList.Add(theater.CreateTile(device, new FileLocation(arc, FileSystem.CombinePath(arc.FilePath, tmpName), ent)));
            //        else
            //            break;                    
            //    }

            //    tiles[i] = subList.ToArray();
            //}
        }
        public void LoadTiles(TheaterBase theater, Device device, string tileExtension, string[] archivePaths)
        {
            tiles = new TileBase[tileCount][];
            for (int i = 0; i < tileCount; i++)
            {
                List<TileBase> subList = new List<TileBase>();

                string fn = fileName;
                if (i < 9)
                    fn += '0';
                fn += (i + 1).ToString();

                // 第一个形态
                FileLocation tileFl = FileSystem.Instance.TryLocate(fn + tileExtension, archivePaths, FileSystem.GameResLR);
                if (tileFl == null)
                {
                    GameConsole.Instance.Write(ResourceAssembly.Instance.CM_TileFileMissing(fn), ConsoleMessageType.Exclamation);
                    // 该tileSet没有地块
                    break;
                }
                subList.Add(theater.CreateTile(device, tileFl)); //Tile3D.FromFile(device, tileFl);
                //subList.Add(Tile3D.FromFile(device, tileFl));

                for (char k = 'a'; k <= 'z'; k++)
                {
                    tileFl = FileSystem.Instance.TryLocate(fn + k + tileExtension, archivePaths, FileSystem.GameResLR);
                    if (tileFl != null)
                    {
                        subList.Add(theater.CreateTile(device, tileFl)); // Tile3D.FromFile(device, tileFl);
                        //subList.Add(Tile3D.FromFile(device, tileFl));
                    }
                    else
                    {
                        break;
                    }
                }

                tiles[i] = subList.ToArray();
            }
        }

        #region IConfigurable 成员

        public virtual void Parse(ConfigurationSection sect)
        {
            Name = sect["SetName"];
            tileCount = sect.GetInt("TilesInSet", 0);

            highRadarColor = sect.GetColorRGBInt("HighRadarColor", 0);
            lowRadarColor = sect.GetColorRGBInt("LowRadarColor", 0);

            AllowTiberium = sect.GetBool("AllowTiberium", false);
            AllowBurrowing = sect.GetBool("AllowBurrowing", false);

            fileName = sect["FileName"];
        }

        #endregion
    }


    public abstract class TheaterBase : IDisposable
    {
        bool disposed;

        /// <summary>
        /// 第一个tile编号，第二个形态
        /// </summary>
        TileBase[][] tiles;
        string name;

        TileSet[] tileSet;

        /// <summary>
        /// table for fast tileSet look up
        /// tsTable[i][j] means tile i is in tileSet j
        /// </summary>
        Dictionary<int, int> tsTable;


        public TheaterBase(string name)
        {
            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public TileBase[][] Tiles
        {
            get { return tiles; }
        }
        public TileBase[] this[int idx]
        {
            get
            {
                return tiles[idx];// tiles[idx];
            }
        }

        public int TileCount
        {
            get { return tiles.Length; }
        }
        public int TileSetCount
        {
            get { return tileSet.Length; }
        }

        /// <summary>
        /// 获得地块的纹理
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public abstract ImageBase[] GetTileTexture(int index, int subTile);

        public abstract ImageBase[][] GetTileTexture(int index);

        public abstract TileBase CreateTile(Device device, ResourceLocation resLoc);

        public TileSet[] TileSets
        {
            get { return tileSet; }
            protected set
            {
                tileSet = value;

                tsTable = new Dictionary<int, int>(1000);
                //tiles = new TileBase[tileSet.Length];

                List<TileBase[]> tileList = new List<TileBase[]>(1000);

                for (int i = 0; i < tileSet.Length; i++)
                {
                    TileBase[][] setTiles = tileSet[i].Tiles;

                    for (int j = 0; j < setTiles.Length; j++)
                    {
                        if (setTiles[j] != null)
                            tsTable.Add(tileList.Count, i);
                        tileList.Add(setTiles[j]);
                    }
                }

                tiles = tileList.ToArray();
            }
        }
        public int FindTileSet(int idx)
        {
            if (tsTable != null)
            {
                int index;
                if (tsTable.TryGetValue(idx, out index))
                {
                    return index;
                }
            }
            return -1;
        }


        public virtual void ReleaseTextures()
        {
        }
        
        

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                tileSet = null;
                for (int i = 0; i < TileCount; i++)
                {
                    if (tiles[i] != null)
                    {
                        for (int j = 0; j < tiles[i].Length; j++)
                        {
                            if (tiles[i][j] != null)
                            {
                                tiles[i][j].Dispose();
                            }
                        }
                    }
                }
                tiles = null;
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion
    }
}
