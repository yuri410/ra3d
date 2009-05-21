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

namespace R3D.IsoMap
{
    public class MapManager
    {
        static MapManager singleton;

        public static MapManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new MapManager();
                return singleton;
            }
        }

        Dictionary<string, IMapFactory> factories = new Dictionary<string, IMapFactory>(CaseInsensitiveStringComparer.Instance);

        public void RegisterMapFormat(IMapFactory fac)
        {
            string[] ext = fac.Filter;
            for (int i = 0; i < ext.Length; i++)
            {
                factories.Add(ext[i], fac);
            }
        }
        public void UnregisterMapFormat(IMapFactory fac)
        {
            string[] ext = fac.Filter;
            for (int i = 0; i < ext.Length; i++)
            {
                factories.Remove(ext[i]);
            }
        }

        public MapBase CreateInstance(string file)
        {
            string ext = Path.GetExtension(file);

            IMapFactory fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(file);
            }
            else
                throw new NotSupportedException(ext);

        }
        public MapBase CreateInstance(ResourceLocation fl, string ext)
        {
            //string ext = Path.GetExtension(fl.Path).ToUpper();           
            IMapFactory fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(fl);
            }
            else
                throw new NotSupportedException(ext);

        }

        public MapBase CreateInstance(MapLocationInfo map)
        {
            IMapFactory fac;
            if (factories.TryGetValue(map.Ext, out fac))
            {
                return fac.CreateInstance(map.ResLocation);
            }
            else
                throw new NotSupportedException(map.Ext);
        }


        public MapLocationInfo[] GetMaps()
        {
            Dictionary<string, IMapFactory>.ValueCollection val = factories.Values;

            List<MapLocationInfo[]> resLocSets = new List<MapLocationInfo[]>(5);
            int mapCount = 0;

            foreach (IMapFactory fac in val)
            {
                MapLocationInfo[] resLocs = fac.GetMaps();
                mapCount += resLocs.Length;
                resLocSets.Add(resLocs);
            }

            MapLocationInfo[] res = new MapLocationInfo[mapCount];
            int curOfs = 0;
            for (int i = 0; i < resLocSets.Count; i++)
            {
                Array.Copy(resLocSets[i], 0, res, curOfs, resLocSets[i].Length);
                curOfs += resLocSets[i].Length;
            }

            return res;
        }

    }
}
