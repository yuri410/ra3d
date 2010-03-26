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
using System.Drawing;
using System.IO;
using System.Text;
using R3D.Base;
using R3D.IO;
using R3D.Logic;
using R3D.Media;
using R3D.UI;

namespace R3D.ConfigModel
{
    public abstract class ConfigurationSection : Dictionary<string, string>
    {
        static readonly string none = "none";

        public static bool IsNone(string str)
        {
            return CaseInsensitiveStringComparer.Compare(str, none);
        }
        public static string CheckNone(string str)
        {
            if (str == null)
                return null;
            if (CaseInsensitiveStringComparer.Compare(str, none))
            {
                return string.Empty;
            }
            return str;
        }
        public static string CheckNoneNull(string str)
        {
            if (str == null)
                return null;
            if (CaseInsensitiveStringComparer.Compare(str, none))
            {
                return null;
            }
            return str;
        }
        public static string[] CheckNone(string[] arr)
        {
            if (arr.Length == 1)
            {
                if (CaseInsensitiveStringComparer.Compare(arr[0], none))
                {
                    return Utils.EmptyStringArray;
                }
                return arr;
            }
            return arr;
        }

        public ConfigurationSection(string name)
        {
            Name = name;
        }
        public ConfigurationSection(string name, IDictionary<string, string> dictionary)
            : base(dictionary)
        {
            Name = name;
        }

        public ConfigurationSection(string name, IEqualityComparer<string> comparer)
            : base(comparer)
        {
            Name = name;
        }

        public ConfigurationSection(string name, int capacity)
            : base(capacity)
        {
            Name = name;
        }

        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>SHP是唯一允许的Anim</remarks>
        /// <param name="rule"></param>
        /// <returns></returns>
        public abstract CameoInfo GetCameo(FileLocateRule rule);
        public abstract UIImageInformation GetImage(FileLocateRule rule);

        public abstract UIImageInformation GetImage(string key, FileLocateRule rule);



        public abstract bool TryGetPaths(string key, out string[] res);

        public abstract string[] GetPaths(string key);
        public abstract void GetRectangle(string key, out Rectangle rect);

        public abstract bool TryGetColorRGBA(string key, out Color clr);
        public abstract Color GetColorRGBA(string key, Color def);
        public abstract Color GetColorRGBA(string key);
        public abstract int GetColorRGBInt(string key);
        public abstract int GetColorRGBInt(string key, int def);

        //public abstract int GetColorRGB

        public abstract bool TryGetBool(string key, out bool res);
        public abstract bool GetBool(string key);
        public abstract bool GetBool(string key, bool def);

        public abstract float GetFloat(string key);
        public abstract float GetFloat(string key, float def);

        public abstract float[] GetFloatArray(string key);
        public abstract float[] GetFloatArray(string key, float[] def);

        public abstract string GetUIString(string key);
        public abstract string GetUIString(string key, string def);

        public abstract string GetString(string key, string def);

        public abstract string[] GetStringArray(string key);
        public abstract string[] GetStringArray(string key, string[] def);

        public abstract int GetInt(string key);
        public abstract int GetInt(string key, int def);

        public abstract int[] GetIntArray(string key);
        public abstract int[] GetIntArray(string key, int[] def);

        public abstract Size GetSize(string key);
        public abstract Size GetSize(string key, Size def);

        public abstract Point GetPoint(string key);
        public abstract Point GetPoint(string key, Point def);


        public abstract float GetPercentage(string key);
        public abstract float GetPercentage(string key, float def);

        public abstract float[] GetPercetageArray(string key);        
    }
}
