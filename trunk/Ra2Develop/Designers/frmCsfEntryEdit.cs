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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using R3D.Base;

namespace Ra2Develop.Designers
{
    public partial class frmCsfEntryEdit : Form
    {
        static char[] WhitespaceChars = new char[] { 
            '\t', '\n', '\v', '\f', '\r', ' ', '\x0085', '\x00a0', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', 
            ' ', ' ', ' ', ' ', '​', '\u2028', '\u2029', '　', '﻿'
            };

        public bool isCanceled;

        public string content;
        public string name;
        public string extraData;
        StringTable translateData;

        public frmCsfEntryEdit(StringTable st)
        {
            InitializeComponent();

            translateData = st;
            R3D.UI.LanguageParser.ParseLanguage(st, this);
        }

        private void frmCsfInsert_Load(object sender, EventArgs e)
        {
            textBox1.Text = name;
            textBox3.Text = content;
            textBox2.Text = extraData;
            isCanceled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            isCanceled = false;

            name = textBox1.Text;
            extraData = textBox2.Text;
            content = textBox3.Text;
            this.Close();
        }

        private void textBox1_Validating(object sender, CancelEventArgs e)
        {
            string text = textBox1.Text;

            if (text.StartsWith(" ") | text.EndsWith(" "))
            {
                e.Cancel = true;
                MessageBox.Show(this, translateData["MSG:CSFNameSpaceSE"], translateData["GUI:Error"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (text.StartsWith(CsfDocument.NoCategory))
            {
                e.Cancel = true;
                MessageBox.Show(this, translateData["MSG:CSFStartNC"], translateData["GUI:Error"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            for (int i = 0; i < text.Length; i++)
                if (text[i] > 255 || text[i] < 0)
                {
                    e.Cancel = true;
                    MessageBox.Show(this, translateData["MSG:CSFExtraDataLimit"], translateData["GUI:Error"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
        }

        private void textBox2_Validating(object sender, CancelEventArgs e)
        {
            string text = textBox2.Text;
            for (int i = 0; i < text.Length; i++)           
                if (text[i] > 255 || text[i] < 0)
                {
                    e.Cancel = true;
                    MessageBox.Show(this, translateData["MSG:CSFExtraDataLimit"], translateData["GUI:Error"], MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            
        }

        private void frmCsfEntryEdit_Shown(object sender, EventArgs e)
        {
            isCanceled = true;
        }


    }
}
