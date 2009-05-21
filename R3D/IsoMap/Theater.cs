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

using R3D.IO;
using R3D.Base;
using R3D.Media;
using R3D.ConfigModel;
using SlimDX.Direct3D9;

namespace R3D.IsoMap
{
    /// <summary>
    /// 代表一种地形。使用原版游戏的实现。
    /// 同时提供该类型地形的文件包。
    /// </summary>
    public class Theater : TheaterBase
    {
        protected string tileExtension;
        protected string[] archivePaths;
        protected Palette palette;
        //string tileTextureExt;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="name">ini name</param>
        public Theater(string name)
            : base(name)
        {

            string iniFile = name + Game.Suffix + FileSystem.dotIni;
            FileLocation fl = FileSystem.Instance.Locate(FileSystem.LocalMix + iniFile, FileSystem.GameResLR);

            Load(fl);
        }
        protected Theater(FileLocation fl)
            : base(Path.GetFileNameWithoutExtension(fl.Path))
        {
            Load(fl);
        }


        protected virtual void Load(ResourceLocation fl)
        {
            // 第一个tile，第二个形态
            //List<List<TileBase>> list = new List<List<TileBase>>();
            List<TileSet> tileSets = new List<TileSet>(1000);

            IniConfiguration ini = new IniConfiguration(fl);

            ConfigurationSection general = ini["General"];
            tileExtension = general["TileExt"];
            //tileTextureExt = general.GetString("TileTextureExt", ".png");
            archivePaths = general.GetPaths("Archive");
            string[] palettePath = general.GetPaths("Palette");

            FileLocation pal = FileSystem.Instance.Locate(palettePath, FileSystem.GameResLR);
            palette = Palette.FromFile(pal);

            for (int i = 0; i < int.MaxValue; i++)
            {
                string sectName = "TileSet";
                if (i < 10)
                    sectName += '0';
                if (i < 100)
                    sectName += '0';
                if (i < 1000)
                    sectName += '0';
                sectName += i.ToString();

                ConfigurationSection sect;
                try
                {
                    sect = ini[sectName];
                }
                catch (KeyNotFoundException)
                {
                    break;
                }


                TileSet tSet = new TileSet();
                tSet.Parse(sect);
                tSet.LoadTiles(this, null, tileExtension, archivePaths);

                tileSets.Add(tSet);

                //int count = sect.GetInt("TilesInSet");
                //string fileName = sect["FileName"];

                //for (int j = 0; j < count; j++)
                //{
                //    List<TileBase> subList = new List<TileBase>();

                //    string fn = fileName;
                //    if (j < 9)
                //        fn += '0';
                //    fn+=(j+1).ToString ();

                //    // 第一个形态
                //    FileLocation tileFl = FileSystem.Instance.TryLocate(fn +  tileExtension, archivePaths, FileSystem.GameResLR);
                //    if (tileFl == null)
                //    {
                //        GameConsole.Instance.Write(ResourceAssembly.Instance.CM_TileFileMissing(fn), ConsoleMessageType.Exclamation);

                //        // 该tileSet没有地块
                //        // 用null填充
                //        for (int k = 0; k < count - j; k++)
                //        {
                //            List<TileBase> subList2 = new List<TileBase>();
                //            subList2.Add(null);
                //            list.Add(subList2);
                //        }
                //        break;
                //    }
                //    subList.Add(Tile.FromFile(tileFl));

                //    for (char k = 'a'; k <= 'z'; k++) 
                //    {
                //        tileFl = FileSystem.Instance.TryLocate(fn + k + tileExtension, archivePaths, FileSystem.GameResLR);
                //        if (tileFl != null)
                //        {
                //            subList.Add(Tile.FromFile(tileFl));
                //        }
                //        else                        
                //        {
                //            break;
                //        }
                //    }

                //    list.Add(subList);
                //}
            }

            //base.tiles = new TileBase[list.Count][];
            //for (int i = 0; i < list.Count; i++)
            //{
            //    tiles[i] = list[i].ToArray();
            //}
            TileSets = tileSets.ToArray();
        }

        public override TileBase CreateTile(Device device, ResourceLocation resLoc)
        {
            return Tile.FromFile(resLoc);
        }

        public static Theater FromFile(FileLocation fl)
        {
            return new Theater(fl);
        }

        public override ImageBase[][] GetTileTexture(int index)
        {
            if (this[index] != null && this[index].Length > 0)
            {
                if (this[index][0] != null)
                {
                    int blockCount = this[index][0].BlockCount;
                    int formCount = this[index].Length;

                    ImageBase[][] res = new ImageBase[blockCount][];

                    for (int i = 0; i < blockCount; i++)
                    {
                        res[i] = new ImageBase[formCount];
                    }

                    // 形态
                    for (int i = 0; i < this[index].Length; i++)
                    {
                        // 单元块
                        ImageBase[] tr = this[index][i].GetImages(palette);
                        for (int j = 0; j < tr.Length; j++)
                        {
                            res[j][i] = tr[j];
                        }
                    }

                    return res;
                }
                return null;
            }
            return null;
        }
        public override ImageBase[] GetTileTexture(int index, int subTile)
        {
            if (this[index] != null && this[index].Length > 0)
            {
                ImageBase[] res = new ImageBase[this[index].Length];

                // 形态
                for (int i = 0; i < this[index].Length; i++)
                {
                    // 单元块
                    res[i] = this[index][i].GetImage(subTile, palette);
                }

                return res;
            }

            return null;
        }
        
        //public override ImageBase[][] GetTileTexture(int index)
        //{
        //    if (tiles[index] == null)
        //    {
        //        return null;
        //    }
        //    return tiles[index].GetImages(tileTextureExt);
        //}
    }
}
