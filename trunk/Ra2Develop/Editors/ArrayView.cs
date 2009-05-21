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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using R3D.UI;

namespace Ra2Develop.Editors
{
    public partial class ArrayView<T> : Form
    {
        static int[] res;
        static DialogResult dr;
        public ArrayView()
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(Program.StringTable, this);
        }

        public static DialogResult ShowDialog(string text, T[] array, out int[] result)
        {
            ArrayView<T> f = new ArrayView<T>();

            f.label1.Text = text;
            if (array != null)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    f.listBox1.Items.Add(i.ToString() + ' ' + array[i].ToString());
                }
            }

            f.ShowDialog();

            result = res;
            res = null;
            return dr;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dr = DialogResult.OK;
            res = new int[listBox1.CheckedItems.Count];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = listBox1.CheckedIndices[i];
            }
            Array.Sort<int>(res);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dr = DialogResult.Cancel;
            this.Close();
        }
    }
}
