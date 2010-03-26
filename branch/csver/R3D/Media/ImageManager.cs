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
    public class ImageManager : UniqueObjectManager<ResourceLocation, ImageBase>
    {
        static ImageManager singleton;

        public static ImageManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ImageManager();
                return singleton;
            }
        }

        Dictionary<string, IImageFactory> factories;

        private ImageManager()
        {
            factories = new Dictionary<string, IImageFactory>(CaseInsensitiveStringComparer.Instance);
        }

        public void RegisterImageFormat(IImageFactory fac)
        {
            string[] exts = fac.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                factories.Add(exts[i], fac);
            }
        }
        public void UnregisterImageFormat(IImageFactory fac)
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

        public ImageBase CreateInstaceUnmanaged(FileLocation fl)
        {
            string ext = Path.GetExtension(fl.Path);

            IImageFactory fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstanceUnmanaged(fl);
            }
            else
                throw new NotSupportedException(ext);
        }
        public bool Supprts(ResourceLocation rl)
        {
            string ext = Path.GetExtension(rl.Name);
            return factories.ContainsKey(ext);
        }
        protected override ImageBase create(ResourceLocation rl)
        {
            string ext = Path.GetExtension(rl.Name);

            IImageFactory fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(rl);
            }
            else
                throw new NotSupportedException(ext);
        }
        protected override void destroy(ImageBase obj)
        {
            obj.Dispose();
        }

        public ImageBase CreateInstance(string file)
        {
            return CreateInstance(new FileLocation(file));
        }
        public ImageBase CreateInstance(int width, int height, ImagePixelFormat format)
        {
            return new RawImage(width, height, format);
        }

        public string GetFilter()
        {
            Dictionary<string, IImageFactory>.ValueCollection val = factories.Values;

            //List<Pair<string, string>> fmts = new List<Pair<string, string>>();
            StringBuilder flt = new StringBuilder(val.Count * 4 + 4);

            IImageFactory lastFac = null;
            foreach (IImageFactory fac in val)
            {
                if (fac != lastFac)
                {
                    string[] flts = fac.Filters;

                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < flts.Length; j++)
                    {
                        sb.Append('*');
                        sb.Append(flts[j]);
                        if (j < flts.Length - 1)
                            sb.Append(';');
                    }
                    flt.Append(fac.Type);
                    flt.Append("(" + sb.ToString() + ")|");
                    flt.Append(sb.ToString());
                    flt.Append("|");
                    //fmts.Add(new Pair<string, string>(fac.Type, sb.ToString()));

                    lastFac = fac;
                }
            }
            flt.Remove(flt.Length - 1, 1);
            return flt.ToString();
            
        }
    }
}
