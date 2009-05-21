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

namespace Ra2Develop.Converters
{
    public partial class ProgressDlg : Form
    {

        public ProgressDlg(string message)
        {
            InitializeComponent();
            Message = message;
        }

        public string Message
        {
            get { return label1.Text; }
            set
            {
                label1.Text = value;
                Application.DoEvents();
            }
        }

        public int Value
        {
            get { return progressBar1.Value; }
            set
            {
                progressBar1.Value = value;
                Application.DoEvents();
            }
        }
        public int MaxVal
        {
            get { return progressBar1.Maximum; }
            set { progressBar1.Maximum = value; }
        }
        public int MinVal
        {
            get { return progressBar1.Minimum; }
            set { progressBar1.Minimum = value; }
        }
    }
}
