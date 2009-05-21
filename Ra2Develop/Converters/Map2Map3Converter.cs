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
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;
using R3D;
using R3D.ConfigModel;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.IsoMap;
using SlimDX;
using Ra2Develop.Designers;
using R3D.Media;
using R3D.GraphicsEngine.Effects;
using R3D.Base;

namespace Ra2Develop.Converters
{
    public class Map2Map3Converter : ConverterBase
    {
        class OldTheater : Theater        
        {
            static string theaterTypeName;
            static FileLocation GetFileLoc(string name)
            {
                theaterTypeName = name;
                switch (name)
                {
                    case "TEMPERATE":
                        return Ra2FileSystem.Instance.Locate(FileSystem.LocalMix + "temperatmd.ini", FileSystem.GameResLR);
                }
                throw new NotSupportedException(name);
            }

            public OldTheater(string name)
                : base(GetFileLoc(name))
            {
            }
            protected override void Load(ResourceLocation fl)
            {
                // 第一个tile，第二个形态
                //List<List<TileBase>> list = new List<List<TileBase>>();
                List<TileSet> tileSets = new List<TileSet>(1000);

                IniConfiguration ini = new IniConfiguration(fl);

                //ConfigurationSection general = ini["General"];
                //tileExtension = general["TileExt"];
                ////tileTextureExt = general.GetString("TileTextureExt", ".png");
                //archivePaths = general.GetPaths("Archive");
                string palettePath;// = general.GetPaths("Palette");

                string pathSep = new string(System.IO.Path.DirectorySeparatorChar, 1);

                switch (theaterTypeName.ToUpper())
                {
                    case "TEMPERATE":
                        archivePaths = new string[1] { "ra2.mix" + pathSep + "isotemp.mix" + pathSep };
                        palettePath = "ra2.mix" + pathSep + "cache.mix" + pathSep + "isotem.pal";
                        tileExtension = ".tem";
                        break;
                    default:
                        throw new NotSupportedException();
                }

                FileLocation pal = Ra2FileSystem.Instance.Locate(palettePath, FileSystem.GameResLR);
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
                    //tSet.LoadTiles(this, null, tileExtension, archivePaths);
                    tSet.LoadTiles(Ra2FileSystem.Instance, this, GraphicsDevice.Instance.Device, tileExtension, archivePaths);
                    tileSets.Add(tSet);

                }

                TileSets = tileSets.ToArray();
            }
        }

        const string CsfKey = "GUI:MAP2MAP3";

        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show(CsfKey, GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(Program.StringTable["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;               

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".map3");

                    Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));
                    pd.Value = i;
                }
                pd.Close();
                pd.Dispose();
            }

        }

        TheaterBase createTheater(string name)
        {
            return new OldTheater(name);
        }

        public unsafe override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            bool oldState = EffectManager.Instance.Enabled;
            EffectManager.Instance.Enabled = false;

            IniConfiguration ini = new IniConfiguration(source);// Map.FromFile(source);

            Map map = new Map(ini, createTheater);
            IsoMapPackEntry[] cells = map.Cells;
            IsoMapPackEntry3D[] newCells = new IsoMapPackEntry3D[cells.Length];

            TheaterBase theater = map.TerrainTheater;

            Theater3D newTheater = new Theater3D(GraphicsDevice.Instance.Device, map.TheaterName);

            for (int i = 0; i < cells.Length; i++)
            {
                newCells[i].x = cells[i].x;
                newCells[i].y = cells[i].y;
                newCells[i].tile = cells[i].tile;

                int subTile = cells[i].subTile;
                newCells[i].left = cells[i].z * Terrain.VerticalUnit;
                newCells[i].right = newCells[i].left;
                newCells[i].top = newCells[i].left;
                newCells[i].bottom = newCells[i].left;
                newCells[i].centre = newCells[i].left;

                TileBase tile = theater[cells[i].tile][0];
                tile.RemapSubtileIndex(ref subTile);

                TileBase newTile = newTheater[cells[i].tile][0];

                TileBase.SetRampVertexHeight((byte)newTile.GetRampType(subTile),
                    ref newCells[i].top,
                    ref newCells[i].left,
                    ref newCells[i].right,
                    ref newCells[i].bottom,
                    ref newCells[i].centre);

                BlockBits bit = newTile.GetBlockBits(subTile);
                if ((bit & BlockBits.HasDoubleLayer) == BlockBits.HasDoubleLayer)
                {
                    newCells[i].secLeft = (cells[i].z + newTile.GetSecHeight(subTile) - newTile.GetHeight(subTile)) * Terrain.VerticalUnit;
                    newCells[i].secRight = newCells[i].secLeft;
                    newCells[i].secTop = newCells[i].secLeft;
                    newCells[i].secBottom = newCells[i].secLeft;
                    newCells[i].secCentre = newCells[i].secLeft;

                    TileBase.SetRampVertexHeight((byte)newTile.GetRampType(subTile),
                        ref newCells[i].secTop,
                        ref newCells[i].secLeft,
                        ref newCells[i].secRight,
                        ref newCells[i].secBottom,
                        ref newCells[i].secCentre);
                }


                //if (!tile.UseCollapsedIndex)
                //tile.RemapSubtileIndex(ref subTile);


                newCells[i].subTile = (byte)subTile;

            }

            //string tempFile = Path.GetTempFileName();
            byte[] buffer = new byte[newCells.Length * sizeof(IsoMapPackEntry3D)];
            fixed (void* src = &newCells[0], dst = &buffer[0])
            {
                Memory.Copy(src, dst, buffer.Length);
            }

            System.IO.MemoryStream ms = new System.IO.MemoryStream(buffer.Length);
            GZipStream cmp = new GZipStream(ms, CompressionMode.Compress);
            BinaryWriter bw = new BinaryWriter(cmp);
            bw.Write(buffer);
            bw.Flush();
            bw.Close();

            buffer = ms.ToArray();

            string[] data = System.Convert.ToBase64String(buffer, Base64FormattingOptions.InsertLineBreaks).Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

            ConfigurationSection sect = ini["IsoMapPack5"];
            sect.Clear();
            //int line = 1;
            for (int i = 0; i < data.Length; i++)
            {
                sect.Add(i.ToString(), data[i]);
            }

            IniSection atSect = new IniSection("Atmosphere");
            atSect.Add("DayLength", "60");
            ini.Add("Atmosphere", atSect);

            ini.Save(dest);
            theater.Dispose();
            newTheater.Dispose();

            EffectManager.Instance.Enabled = oldState;
        }

        public override string Name
        {
            get { return Program.StringTable[CsfKey]; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".map", ".yrm" }; }
        }
        public override string[] DestExt
        {
            get { return new string[] { ".map", ".yrm", ".map3" }; }
        }

        public override string SourceDesc
        {
            get { return Program.StringTable["DOCS:MapDesc"]; }
        }

        public override string DestDesc
        {
            get { return Program.StringTable["DOCS:Map3Desc"]; }
        }
    }
}
