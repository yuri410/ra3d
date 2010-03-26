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
using R3D.IO;

namespace R3D.ConfigModel
{
    public abstract class ConfigurationFormat
    {
        public abstract string[] Filters { get; }

        public abstract Configuration Load(ResourceLocation rl);

        public Configuration Load(string file)
        {
            return Load(new FileLocation(file));
        }
    }

    public class ConfigurationManager
    {
        static ConfigurationManager singleton;

        public static ConfigurationManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ConfigurationManager();
                return singleton;
            }
        }

        Dictionary<string, ConfigurationFormat> formats = new Dictionary<string, ConfigurationFormat>(CaseInsensitiveStringComparer.Instance);

        public void Register(ConfigurationFormat fmt)
        {
            string[] exts = fmt.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                formats.Add(exts[i], fmt);
            }
        }

        public Configuration CreateInstance(string file)
        {
            string ext = Path.GetExtension(file);

            ConfigurationFormat fmt;
            if (formats.TryGetValue(ext, out fmt))
            {
                return fmt.Load(file);
            }
            throw new NotSupportedException(ext);
        }
        public Configuration CreateInstance(ResourceLocation rl)
        {            
            string ext = Path.GetExtension(rl.Name);

            ConfigurationFormat fmt;
            if (formats.TryGetValue(ext, out fmt))
            {
                return fmt.Load(rl);
            }
            throw new NotSupportedException(ext);
        }
    }
}
