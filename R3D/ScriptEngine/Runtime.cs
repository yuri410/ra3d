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
using System.Threading;

using R3D.UI;
using R3D.Base;
using R3D.UI.Controls;


namespace R3D.ScriptEngine
{
    public enum MsgBoxResult
    {
        None,
        ButtonA,
        ButtonB
    }
 

    /// <summary>
    /// 运行时库。提供一组方法，以方便开发人员使用脚本。
    /// </summary>
    public static class SRuntime
    {
        public delegate void DialogCloseCallback(MsgBoxResult res);

        public static void MessageBoxNST(string text, string style, DialogCloseCallback dccbk)
        {
            Game g = Game.Instance;
            GameUI gui = g.GameUI;
            Dialog dlg = new Dialog(Game.Instance.GameUI, gui.DialogSettings[style]);


            dlg.Update(0);

            if (dlg.ButtonA != null)
            {
                dlg.ButtonA.MouseUp += delegate(object _sender, MouseEventArgs _e)
                {
                    //result = MsgBoxResult.ButtonA;
                    dlg.CloseDialog();
                    if (dccbk != null)
                    {
                        dccbk(MsgBoxResult.ButtonA);
                    }

                    dlg.Dispose();
                };
            }
            if (dlg.ButtonB != null)
            {
                dlg.ButtonB.MouseUp += delegate(object _sender, MouseEventArgs _e)
                {
                    //result = MsgBoxResult.ButtonB;
                    dlg.CloseDialog();
                    if (dccbk != null)
                    {
                        dccbk(MsgBoxResult.ButtonB);
                    }
                    dlg.Dispose();
                };
            }
            dlg.Text = text;


            dlg.ShowDialog();
        }

        public static void MessageBox(string csfEntry, string style, DialogCloseCallback dccbk)
        {
            Game g = Game.Instance;
            GameUI gui = g.GameUI;
            Dialog dlg = new Dialog(Game.Instance.GameUI, gui.DialogSettings[style]);


            dlg.Update(0);

            if (dlg.ButtonA != null)
            {
                dlg.ButtonA.MouseUp += delegate(object _sender, MouseEventArgs _e)
                {
                    //result = MsgBoxResult.ButtonA;
                    dlg.CloseDialog();
                    if (dccbk != null)
                    {
                        dccbk(MsgBoxResult.ButtonA);
                    }

                    dlg.Dispose();
                };
            }
            if (dlg.ButtonB != null)
            {
                dlg.ButtonB.MouseUp += delegate(object _sender, MouseEventArgs _e)
                {
                    //result = MsgBoxResult.ButtonB;
                    dlg.CloseDialog();
                    if (dccbk != null)
                    {
                        dccbk(MsgBoxResult.ButtonB);
                    }
                    dlg.Dispose();
                };
            }
            dlg.Text = StringTableManager.StringTable[csfEntry];


            dlg.ShowDialog();

            //while (dlg.IsShown)
            //{
            //    Thread.Sleep(1);
            //    Application.DoEvents();
            //}

            //return result;
        }



        public static T IIf<T>(bool expression, T truePart, T falsePart)
        {
            return expression ? truePart : falsePart;
        }

        public static int ARBG(int red, int green, int blue)
        {
            if (red > 255)
                red = 255;
            if (green > 255)
                green = 255;
            if (blue > 255)
                blue = 255;
            return (0xff << 24) | (red << 16) | (green << 8) | blue;
        }
        public static int ARBG(int alpha, int red, int green, int blue)
        {
            if (alpha > 255)
                alpha = 255;
            if (red > 255)
                red = 255;
            if (green > 255)
                green = 255;
            if (blue > 255)
                blue = 255;
            return (alpha << 24) | (red << 16) | (green << 8) | blue;
        }

        public static int AscW(char @char)
        {
            return @char;
        }

        public static int AscW(string @string)
        {
            if (string.IsNullOrEmpty(@string))
            {
                throw new ArgumentException("string");
            }
            return @string[0];
        }


        public unsafe static IntPtr GetArrayPointer(int[] array, int index)
            //where T : struct
        {
            fixed (void* ptr = &array[index])
            {
                return new IntPtr(ptr);
            }
        }

    }
}
