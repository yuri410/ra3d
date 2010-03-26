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

namespace R3D.Media
{
    public class PaletteCache
    {
        class Entry
        {
            public ResourceLocation ResLoc
            {
                get;
                set;
            }

            public Palette Palette
            {
                get;
                set;
            }

            public int Hash
            {
                get;
                set;
            }
        }
        const int DefaultCapacity = 10;



        Dictionary<int, Entry> bufferred;
        Queue<Entry> queue;

        int maxCapacity;

        public PaletteCache()
            : this(DefaultCapacity)
        { }

        public PaletteCache(int maxCapacity)
        {
            //bufferedSort = new List<Entry>();
            this.maxCapacity = maxCapacity;
            queue = new Queue<Entry>(maxCapacity + 1);
            bufferred = new Dictionary<int, Entry>(maxCapacity + 1);
        }

        public Palette CreatePalette(ResourceLocation rl)
        {
            Entry ent;
            if (bufferred.TryGetValue(rl.GetHashCode(), out ent))
            {
                return ent.Palette;
            }
            else
            {
                ent = new Entry();
                ent.Palette = Palette.FromFile(rl);
                ent.ResLoc = rl;
                ent.Hash = rl.GetHashCode();

                if (bufferred.Count > maxCapacity)
                {
                    Entry toDel = queue.Dequeue();
                    bufferred.Remove(toDel.Hash);
                }

                queue.Enqueue(ent);
                bufferred.Add(ent.Hash, ent);
                return ent.Palette;
            }
        }
    }
}
