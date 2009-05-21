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

namespace R3D.Base
{
    public class StringTableManager
    {
        static StringTable strTbl;

        public static StringTable StringTable
        {
            get { return strTbl; }
        }

        Dictionary<string, StringTableFormat> formats = new Dictionary<string, StringTableFormat>(CaseInsensitiveStringComparer.Instance);

        public void Register(StringTableFormat fmt)
        {
            string[] exts = fmt.Filers;
            for (int i = 0; i < exts.Length; i++)
            {
                formats.Add(exts[i], fmt);
            }
        }

        public void LoadStringTable()
        {
            if (strTbl == null)
            {
                string ext = Path.GetExtension(FileSystem.Ra2_Csf);

                StringTableFormat fmt;
                if (formats.TryGetValue(ext, out fmt))
                {
                    strTbl = fmt.Load(FileSystem.Instance.Locate(FileSystem.Ra2_Csf, FileSystem.GameLangLR));
                }
                else
                    throw new NotSupportedException(ext);
            }
        }



    }
}
