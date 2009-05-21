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
using System.Windows.Forms;
using R3D.Base;
using R3D.IO;

namespace Ra2Develop
{
    class DevenvStringTable : StringTable
    {
        public DevenvStringTable(string file)
        {
            StringTableCsfFormat fmt = new StringTableCsfFormat();
            fmt.Read(this, File.OpenRead(file));
        }

        public override string this[string a]
        {
            get
            {
                KeyValuePair<string, string> value;
                return base.TryGetValue(a, out value) ? value.Value : "[MISSING]" + a;
            }
        }
    }

    static class Program
    {
        static Icon defaultIcon;
        static StringTable strTable;
        static MainForm form;

        public static MainForm MainForm
        {
            get { return form; }
        }
        public static Icon DefaultIcon
        {
            get { return defaultIcon; }
        }
        public static StringTable StringTable
        {
            get
            {
                if (strTable == null)
                    LoadStringTable();
                return strTable; 
            }
        }

        static void LoadStringTable()
        {
            strTable = new DevenvStringTable((Path.Combine(Application.StartupPath, "Ra2Develop.csf")));

        }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (strTable == null)
                LoadStringTable();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            defaultIcon = new Icon(typeof(Form), "wfc.ico");
            form = new MainForm();
            Application.Run(form);
        }


    }
}
