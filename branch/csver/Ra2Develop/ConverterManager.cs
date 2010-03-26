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
using Ra2Develop.Converters;
using System.Collections;
using R3D;

namespace Ra2Develop
{
    public class ConverterManager
    {
        static ConverterManager singleton;
        public static ConverterManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ConverterManager();
                return singleton; 
            }
        }


        List<ConverterBase> converters;

        private ConverterManager()
        {
            converters = new List<ConverterBase>();
        }
        public void Register(ConverterBase fac)
        {
            converters.Add(fac);
        }
        public void Unregister(ConverterBase fac) 
        {
            converters.Remove(fac);
        }

        public ConverterBase[] GetAllConverters()
        {
            return converters.ToArray();
        }
        public ConverterBase[] GetConvertersDest(string dstExt)
        {
            List<ConverterBase> res = new List<ConverterBase>(converters.Count);

            for (int i = 0; i < converters.Count; i++)
            {
                string[] dest = converters[i].DestExt;
                for (int j = 0; j < dest.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(dstExt, dest[j]))
                    {
                        res.Add(converters[i]);
                        break;
                    }
                }
            }

            return res.ToArray();
        }
        public ConverterBase[] GetConvertersSrc(string srcExt)
        {
            List<ConverterBase> res = new List<ConverterBase>(converters.Count);

            for (int i = 0; i < converters.Count; i++)
            {
                string[] source = converters[i].SourceExt;
                for (int j = 0; j < source.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(srcExt, source[j]))
                    {
                        res.Add(converters[i]);
                        break;
                    }
                }
            }

            return res.ToArray();
        }
        public ConverterBase[] GetConverters(string srcExt, string dstExt)
        {
            List<ConverterBase> res = new List<ConverterBase>(converters.Count);

            for (int i = 0; i < converters.Count; i++)
            {
                string[] source = converters[i].SourceExt;
                string[] dest = converters[i].DestExt;
                bool p1 = false;
                bool p2 = false;
                for (int j = 0; j < source.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(srcExt, source[j]))
                    {
                        p1 = true;
                        break;
                    }
                }
                
                for (int j = 0; j < dest.Length; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(dstExt, dest[j]))
                    {
                        p2 = true;
                        break;
                    }
                }

                if (p1 & p2)
                {
                    res.Add(converters[i]);
                }
            }

            return res.ToArray();
        }
    }
}
