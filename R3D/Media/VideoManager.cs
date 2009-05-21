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

namespace R3D.Media
{
    public interface IVideoFactory : IAbstractFactory<VideoBase, ResourceLocation>
    {

    }

    public class VideoManager
    {
        static VideoManager singleton;
        public static VideoManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new VideoManager();
                }
                return singleton; 
            }
        }

        Dictionary<string, IVideoFactory> factories;

        public VideoManager()
        {
            factories = new Dictionary<string, IVideoFactory>(CaseInsensitiveStringComparer.Instance);

        }

        public void RegisterVideoFormat(IVideoFactory fac)
        {
            factories.Add(fac.Type, fac);
        }
        public bool UnregisterVideoFormat(IVideoFactory fac)
        {
            return factories.Remove(fac.Type);
        }
        public bool UnregisterVideoFormat(string type)
        {
            return factories.Remove(type);
        }

        public VideoBase CreateVideo(string file)
        {
            string ext = Path.GetExtension(file);

            IVideoFactory fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(file);
            }
            else
                throw new NotSupportedException();
        }
        public VideoBase CreateVideo(FileLocation fl)
        {
            string ext = Path.GetExtension(fl.Path);

            IVideoFactory fac;
            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(fl);
            }
            else
                throw new NotSupportedException();
        }
    }
}
