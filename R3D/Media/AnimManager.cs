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

namespace R3D.Media
{
    public class AnimManager : UniqueObjectManager<ResourceLocation, AnimBase>
    {
        static AnimManager singleton;

        public static AnimManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new AnimManager();
                return singleton;
            }
        }
        Dictionary<string, IAnimFactory> factories;

        private AnimManager()
        {
            factories = new Dictionary<string, IAnimFactory>(CaseInsensitiveStringComparer.Instance);
        }

        public void RegisterImageFormat(IAnimFactory fac)
        {
            string[] exts = fac.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                factories.Add(exts[i], fac);
            }
        }
        public void UnregisterImageFormat(IAnimFactory fac)
        {
            string[] exts = fac.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                factories.Remove(exts[i]);
            }
        }
        public bool UnregisterImageFormat(string type)
        {
            return factories.Remove(type);
        }

        public bool Supprts(ResourceLocation rl)
        {
            string ext = Path.GetExtension(rl.Name);
            return factories.ContainsKey(ext);
        }
        protected override AnimBase create(ResourceLocation rl)
        {
            string ext = Path.GetExtension(rl.Name);

            IAnimFactory fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(rl);
            }
            else
                throw new NotSupportedException(ext);
        }
        protected override void destroy(AnimBase obj)
        {
            obj.Dispose();
        }
        public AnimBase CreateInstance(string file)
        {
            return CreateInstance(new FileLocation(file));
        }
    }
}
