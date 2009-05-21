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

namespace Ra2Develop
{
    public static class Serialization
    {
        public static T XmlDeserialize<T>(string file) where T : class
        {
            StreamReader sr = new StreamReader(file, Encoding.Unicode);

            T obj = (T)(new XmlSerializer(typeof(T)).Deserialize(sr));

            sr.Close();
            return obj;
        }

        public static void XmlSerialize<T>(T obj, string file) where T : class
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            fs.SetLength(0);
            StreamWriter sw = new StreamWriter(fs, Encoding.Unicode);

            new XmlSerializer(typeof(T)).Serialize(sw, obj);

            sw.Close();
        }
    }
}
