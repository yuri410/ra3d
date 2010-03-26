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
using R3D.IO;
using System.IO;

namespace R3D.IsoMap
{
    public interface IMapFactory : IAbstractFactory<MapBase, ResourceLocation>
    {
        string[] Filter { get; }
        MapLocationInfo[] GetMaps();
        MapInfoBase[] GetMapInfo();
    }
    public class MapLocationInfo
    {
        public ResourceLocation ResLocation
        {
            get;
            protected set;
        }

        public string Ext
        {
            get;
            protected set;
        }

        public MapLocationInfo(ResourceLocation resLoc,string ext)
        {
            ResLocation = resLoc;
            Ext = ext;
        }

    }
    public class Map3DFactory : IMapFactory
    {
        static readonly string FirstExt = ".Map3";

        public MapBase CreateInstance(string file)
        {
            return Map3D.FromFile(file);
        }

        public MapBase CreateInstance(ResourceLocation fl)
        {
            return Map3D.FromFile(fl);
        }

        public string Type
        {
            get { return "3D Map"; }
        }

        public string[] Filter
        {
            get { return new string[] { FirstExt }; }
        }

        public MapLocationInfo[] GetMaps()
        {
            Archive multi = FileSystem.Instance.Locate(FileSystem.Multi_Mix.Remove(FileSystem.Multi_Mix.Length - 1));
            List<MapLocationInfo> files = new List<MapLocationInfo>();
            Dictionary<uint, ArchiveFileEntry>.ValueCollection af = multi.Files.Values;
            foreach (ArchiveFileEntry e in af)
            {
                MapLocationInfo mapInfo = new MapLocationInfo(new FileLocation(multi, FileSystem.CombinePath(multi.FilePath, Convert.ToString(e.id, 16)), e), FirstExt);
                files.Add(mapInfo);
            }

            string[] filters = Filter;
            int ftlen = filters.Length;
            for (int i = 0; i < ftlen; i++)
            {
                filters[i] = "*" + filters[i];
            }

            DirectoryInfo[] dirs = FileSystem.Instance.GetWorkingDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                for (int j = 0; j < ftlen; j++)
                {
                    FileInfo[] fls = dirs[i].GetFiles(filters[j]);

                    for (int k = 0; k < fls.Length; k++)
                    {
                        MapLocationInfo mapInfo = new MapLocationInfo(new FileLocation(fls[k].FullName), FirstExt);

                        files.Add(mapInfo);
                    }
                }
            }
            return files.ToArray();
        }
        public MapInfoBase[] GetMapInfo()
        {
            MapLocationInfo[] files = GetMaps();
            MapInfoBase[] mapis = new MapInfoBase[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                mapis[i] = new MapInfo(files[i].ResLocation);
            }
            return mapis;
        }

    }
}
