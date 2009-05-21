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
using System.Threading;
using System.Windows.Forms;
using R3D;
using R3D.Base;
using R3D.IO;
using R3D.ScriptEngine;
using R3D.UI;
using R3D.UI.Controls;

namespace Expandmd
{
    public static class MainMenu
    {
        public static class Panel
        {
            //[ScriptBinding(ScriptBinding.Auto)]
            //public static void OnLoad(object sender)
            //{
            //    //MenuPanel panel = (MenuPanel)sender;


            //}

            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {
                MessageBox.Show("just a test");
            }
        }

        public static class Item1
        {
            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {
                R3D.UI.Menu mm = Game.Instance.GameUI.Menu;
                mm.UserChangeMenu("SinglePlayerMenu");//mm[1]);
            }
        }
    
        public static class Item9
        {
            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {    
                R3D.UI.Menu mm = Game.Instance.GameUI.Menu;

                mm.UserChangeMenu((IMenuPage)null);

                while (mm.SlideState != MenuSlideState.Empty)
                {
                    //Thread.Sleep(5);
                    Application.DoEvents();
                }



                Game g = Game.Instance;
                GameUI gui = g.GameUI;
                Dialog dlg = new Dialog(gui, gui.DialogSettings["DlgOkCancel"]);

                dlg.Update(0);


                if (dlg.ButtonA != null)
                {
                    dlg.ButtonA.MouseUp += delegate(object _sender, MouseEventArgs _e)
                    {
                        dlg.CloseDialog();
                        dlg.Dispose();
                        g.Exit();
                    };
                }
                if (dlg.ButtonB != null)
                {
                    dlg.ButtonB.MouseUp += delegate(object _sender, MouseEventArgs _e)
                    {
                        dlg.CloseDialog();
                        dlg.Dispose();
                        mm.SlideState = MenuSlideState.SlidingIn;
                    };
                }
                dlg.Text = StringTableManager.StringTable["GUI:ExitAreYouSure"];



                dlg.ShowDialog();

                while (dlg.IsShown)
                {
                    Thread.Sleep(5);
                    Application.DoEvents();
                }


            }
        }




    }

}
