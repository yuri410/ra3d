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
using R3D;
using R3D.IO;
using R3D.Logic;
using R3D.ScriptEngine;
using R3D.UI;

namespace Expandmd
{
    public static class SinglePlayerMenu
    {
        //public static class Item1
        //{
        //    [ScriptBinding(ScriptBinding.Auto)]
        //    public static void OnMouseUp(object sender, MouseEventArgs e)
        //    {
        //        R3D.UI.Menu mm = Game.Instance.GameUI.Menu;
        //        mm.UserChangeMenu(mm[1]);
        //    }
        //}

        public static class Item3
        {
            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {
                R3D.UI.Menu mm = Game.Instance.GameUI.Menu;
                mm.UserChangeMenu("SkirmishMenu");
                //if (Runtime.MessageBox("start a test?", "DlgOkCancel") == MsgBoxResult.ButtonA)
                //{
                //    BattleSettings sets = new BattleSettings();

                //    sets.MapFile = FileSystem.Instance.Locate("multimd.mix\\xgrinder.map3", FileSystem.GameResLR);
                //    sets.MapExt = ".map3";
                //    sets.LoadScreen = new BattleLoadScreen(Game.Instance.Device);

                //    sets.PlayerInfo = new PlayerInfo[0];

                //    Battle bat = new Battle(Game.Instance.Device, sets);

                //    Game.Instance.CurrentBattle = bat;

                //    bat.Load();
                //}
            }
        }


        public static class Item9
        {
            [ScriptBinding(ScriptBinding.Auto)]
            public static void MouseClickImpl(object sender, MouseEventArgs e)
            {
                R3D.UI.Menu mm = Game.Instance.GameUI.Menu;
                mm.UserChangeMenu("MainMenu");
            }
        }
    }
}
