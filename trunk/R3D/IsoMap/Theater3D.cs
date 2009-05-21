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
using System.IO;
using System.Text;
using R3D.ConfigModel;
using R3D.IO;
using R3D.Media;
using SlimDX.Direct3D9;

namespace R3D.IsoMap
{
    public class Theater3D : TheaterBase
    {
        string tileExtension;
        string[] archivePaths;
        Device device;

        public Theater3D(Device dev, string name)
            : base(name)
        {
            string iniFile = name + Game.Suffix + FileSystem.dotIni;
            FileLocation fl = FileSystem.Instance.Locate(FileSystem.LocalMix + iniFile, FileSystem.GameResLR);

            device = dev;
            Load(fl);
        }
        private Theater3D(Device dev, FileLocation fl)
            : base(Path.GetFileNameWithoutExtension(fl.Path))
        {
            device = dev;
            Load(fl);
        }

        void Load(ResourceLocation rl)
        {
            List<TileSet> tileSets = new List<TileSet>(1000);

            Configuration ini = ConfigurationManager.Instance.CreateInstance(rl);// new IniConfiguration(rl);

            ConfigurationSection general = ini["General"];
            tileExtension = general["TileExt"];
            archivePaths = general.GetPaths("Archive");


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
                tSet.LoadTiles(this, device, tileExtension, archivePaths);

                tileSets.Add(tSet);
            }

            TileSets = tileSets.ToArray();
        }

        public override TileBase CreateTile(Device device, ResourceLocation resLoc)
        {
            return Tile3D.FromFile(device, resLoc);
        }
        public static Theater3D FromFile(Device dev, FileLocation fl)
        {
            return new Theater3D(dev, fl);
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
                    res[i] = this[index][i].GetImage(subTile);
                }

                return res;
            }

            return null;
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
                        ImageBase[] tr = this[index][i].GetImages();
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

        public override void ReleaseTextures()
        {
            for (int i = 0; i < TileCount; i++)
            {
                if (this[i] != null)
                {
                    for (int j = 0; j < this[i].Length; j++)
                    {
                        this[i][j].ReleaseTextures();
                    }
                }
            }
        }
    }
}
