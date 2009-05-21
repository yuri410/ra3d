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
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace R3D
{
    public partial class GameConsole : Form
    {
        static Color[] colorTable =
            new Color[] { Color.DarkGray, Color.Green, Color.Black, Color.Orange, Color.Red };

        string[] msgPrefixTable = new string[] 
        {
            ResourceAssembly.Instance.MsgLvl_Normal,
            ResourceAssembly.Instance.MsgLvl_Information,
            ResourceAssembly.Instance.MsgLvl_Attention,
            ResourceAssembly.Instance.MsgLvl_Warning,
            ResourceAssembly.Instance.MsgLvl_Error
        };

        delegate void WriteHelper(string msg, ConsoleMessageType type);
        delegate string GetTextHelper();
 
        static GameConsole singleton;
        
        public static GameConsole Instance        
        {
            get
            {
                if (singleton == null)
                    singleton = new GameConsole();
                return singleton;
            }
        }


        ConsoleMessageType msgLvl;
        bool isShown;
        StringBuilder context;
        private GameConsole()
        {
            InitializeComponent();
            context = new StringBuilder();

            this.Text = ResourceAssembly.Instance.Console;// "控制台";
            button1.Text = ResourceAssembly.Instance.Console_Submit;// "提交";
            
        }
        public ConsoleMessageType MessageLevel        
        {
            get { return msgLvl; }
            set { msgLvl = value; }
        }


        void write(string msg, ConsoleMessageType msgType)
        {
            if (msgType >= msgLvl)
            {
                richTextBox1.SelectionStart = richTextBox1.TextLength;
                richTextBox1.SelectionLength = 0;

                msg = DateTime.UtcNow.TimeOfDay.ToString() + msgPrefixTable[(int)msgType] + msg + "\r\n";

                richTextBox1.SelectionColor = colorTable[(int)msgType];
                richTextBox1.SelectedText = msg;

                richTextBox1.ScrollToCaret();
            }
        }
        string getText() { return richTextBox1.Text; }

        public void Write(string msg)
        {
            Write(msg, ConsoleMessageType.Normal);
        }
        public void Write(string msg, ConsoleMessageType msgType)
        {
            if (isShown)
            {
                if (richTextBox1.InvokeRequired)
                {
                    this.Invoke((WriteHelper)this.write, msg, (object)msgType);
                }
                else
                {
                    if (msgType >= msgLvl)
                    {
                        richTextBox1.SelectionStart = richTextBox1.TextLength;
                        richTextBox1.SelectionLength = 0;

                        msg = DateTime.UtcNow.TimeOfDay.ToString() + msgPrefixTable[(int)msgType] + msg + "\r\n";

                        richTextBox1.SelectionColor = colorTable[(int)msgType];
                        richTextBox1.SelectedText = msg;

                        richTextBox1.ScrollToCaret();
                    }
                }
            }
            else
            {
                if (msgType >= msgLvl)
                {
                    context.AppendLine(DateTime.UtcNow.TimeOfDay.ToString() + msgPrefixTable[(int)msgType] + msg);
                }
            }
            //
        }

        public string ConsoleText
        {
            get
            {
                if (!richTextBox1.InvokeRequired)
                {
                    return isShown ? richTextBox1.Text : context.ToString();
                }
                else
                {
                    return isShown ? (string)richTextBox1.Invoke((GetTextHelper)getText) : context.ToString();
                }
            }
        }

        private void Console_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Write(comboBox1.Text);
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            copyMenuItem.Enabled = richTextBox1.SelectionLength != 0;
        }

        private void clearMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void GameConsole_Load(object sender, EventArgs e)
        {
            isShown = true;
        }


    

    }
}
