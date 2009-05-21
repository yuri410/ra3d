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
using System.Xml.Serialization;

namespace Ra2Develop.Designers
{
    /// <summary>
    /// singleton
    /// </summary> 
    [Serializable()]
    public class CsfDocumentConfigs : DocumentConfigBase
    {
        static readonly string ConfigFile = Path.Combine("Configs", "csfc.xml");
        static CsfDocumentConfigs singleton;


        int spliterDistVert;
        int spliterDistHoz;

        private CsfDocumentConfigs()
        {
            spliterDistVert = 200;
            spliterDistHoz = 240;
        }

        public int SpliterDistVert
        {
            get { return spliterDistVert; }
            set { spliterDistVert = value; }
        }
        public int SpliterDistHoz
        {
            get { return spliterDistHoz; }
            set { spliterDistHoz = value; }
        }

        public static CsfDocumentConfigs Instance
        {
            get
            {
                if (singleton == null)
                    Initialize();
                return singleton;
            }
        }

        

        static void Initialize()
        {
            if (File.Exists(ConfigFile))
            {
                singleton = Serialization.XmlDeserialize<CsfDocumentConfigs>(ConfigFile);
            }
            else
            {
                singleton = new CsfDocumentConfigs();
            }
        }

        protected override void Save()
        {
            Serialization.XmlSerialize<CsfDocumentConfigs>(this, ConfigFile);
        }
    }
}
