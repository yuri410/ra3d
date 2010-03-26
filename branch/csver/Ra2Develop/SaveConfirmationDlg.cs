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
using Ra2Develop.Designers;
using R3D;

namespace Ra2Develop
{
    public partial class SaveConfirmationDlg : Form
    {
        public SaveConfirmationDlg()
        {
            InitializeComponent();
            R3D.UI.LanguageParser.ParseLanguage(Program.StringTable, this);
            dr = DialogResult.Cancel;
        }


        static DialogResult dr;
        static DocumentBase[] savingDocs;

        static Dictionary<string, DocumentBase> docs;

        public static Pair<DialogResult, DocumentBase[]> Show(IWin32Window parent, DocumentBase[] allDocs)
        {
            docs = new Dictionary<string, DocumentBase>(allDocs.Length);
            SaveConfirmationDlg f = new SaveConfirmationDlg();
            for (int i = 0; i < allDocs.Length; i++)
            {
                docs.Add(allDocs[i].ToString(), allDocs[i]);
                f.listBox1.Items.Add(allDocs[i].ToString());
            }

            f.ShowDialog(parent);

            Pair<DialogResult, DocumentBase[]> res;
            res.a = dr;
            res.b = savingDocs;

            dr = DialogResult.Cancel;
            savingDocs = null;
            docs = null;
            return res;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            List<DocumentBase> res = new List<DocumentBase>(listBox1.Items.Count);
            for (int i = 0; i < listBox1.Items.Count; i++)
            {
                res.Add(docs[(string)listBox1.Items[i]]);
            }
            savingDocs = res.ToArray();
            dr = DialogResult.Yes;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dr = DialogResult.No;
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
