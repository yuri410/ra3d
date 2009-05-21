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
using System.Windows.Forms;

using R3D.Base;

namespace R3D.UI
{
    public static class LanguageParser
    {
        static StringTable strTable;

        public static void ParseLanguage(StringTable data, Control ctl)
        {
            strTable = data;
            ParseLanguage(ctl);
            strTable = null;
        }
        public static void ParseLanguage(StringTable data, ToolStrip ctl)
        {
            strTable = data;
            for (int i = 0; i < ctl.Items.Count; i++)
                ParseLanguage(ctl.Items[i]);
            strTable = null;
        }
        public static void ParseLanguage(StringTable data, ListView ctl)
        {
            strTable = data;
            ParseLanguage(ctl);
            strTable = null;
        }
        static void ParseLanguage(Control ctl)
        {
            if (!string.IsNullOrEmpty(ctl.Text))
            {
                //KeyValuePair<string, string> entry;
                //if (strTable.TryGetValue(ctl.Text, out entry))
                    ctl.Text = strTable[ctl.Text];// entry.Value;
            }

            Control.ControlCollection col = ctl.Controls;
            for (int i = 0; i < col.Count; i++)
                ParseLanguage(col[i]);

        }

        static void ParseLanguage(ToolStripItem ctl)
        {
            if (!string.IsNullOrEmpty(ctl.Text))
            {
                ctl.Text = strTable[ctl.Text];
                //KeyValuePair<string, string> entry;
                //if (strTable.TryGetValue(ctl.Text, out entry))
                //    ctl.Text = entry.Value;
            }

            ToolStripDropDownItem itm = ctl as ToolStripDropDownItem;
            if (itm != null)
                for (int i = 0; i < itm.DropDownItems.Count; i++)
                    ParseLanguage(itm.DropDownItems[i]);

        }
        static void ParseLanguage(ListView lv)
        {
            for (int i = 0; i < lv.Columns.Count; i++)
            {
                lv.Columns[i].Text = strTable[lv.Columns[i].Text];
                //KeyValuePair<string, string> entry;
                //if (strTable.TryGetValue(lv.Columns[i].Text, out entry))
                //    lv.Columns[i].Text = entry.Value;
            }
        }
    }
}