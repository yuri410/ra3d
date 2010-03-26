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
using System.Text;
using R3D.ConfigModel;
using R3D.Base;
using R3D.IO;
using R3D.ScriptEngine;
using R3D.UI.Controls;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.UI
{
    /// <summary>
    /// 代表一个菜单页。菜单页由主面板（左）按钮列表（右）组成。    
    /// </summary>
    /// <remarks>自动释放D3D资源，其他数据不释放</remarks>
    public sealed class MenuPage : MenuPageBase, IMenuPage
    {
        ConfigModel.Configuration uiIni;

        
        

        void ShowHintHelper(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                string msg = (string)((Control)sender).Tag;
                if (menu.HintText != msg)
                    menu.HintText = msg;
            }
            //menu.isHintShown = true;
        }

        //void CloseHintHelper(object sender, EventArgs e)
        //{
        //    menu.HintText = string.Empty;
        //}
        
        void PlayClickSound(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.IsValid && btn.Enabled)
            {
                menu.MenuSounds.GuiMainButtonSound.Play();
            }
        }

        void MenuButtonLoadImpl(object sender)
        {
            Button btn = (Button)sender;

            btn.MouseMove += ShowHintHelper;
            //btn.MouseLeave += CloseHintHelper;
            btn.MouseDown += PlayClickSound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gui"></param>
        /// <param name="name">对象名称。将被用于邦定脚本，包含脚本的静态类名称和这个参数一样（区分大小写）。</param>
        public MenuPage(GameUI gui, string name)
            : base(gui, name)
        {
            uiIni = gui.UIConfig;
        }


        //protected override void LoadResource()
        //{
            //isResReleased = false;
        //}
        //protected override void UnloadResource()
        //{
            //isResReleased = true;
        //}

        void ObjectLoadImpl(object sender)
        {
            //if (isResReleased)
            //{
            //Game game = Game.Instance;
            ConfigurationSection sect = uiIni[Name];
            menuType = (MenuType)Enum.Parse(typeof(MenuType), sect["MenuType"], true);

            uiName = sect.GetUIString("Name");

            for (int i = 0; i < 9; i++)
            {
                string itemName = Name + ".Item" + (i + 1).ToString();
                sect = uiIni[itemName];

                buttons[i] = Button.FromConfigSection(GameUI, sect);

                buttons[i].X = 644;
                buttons[i].Y = 199 + i * menu.ButtonHeight;
                buttons[i].Size = new Size(menu.ButtonWidth, menu.ButtonHeight);

                buttons[i].ContentAlign = ContentAlignment.MiddleCenter;
                //buttons[i].TextOffset = mm.textOffset;

                buttons[i].Image = menu.ButtonStdImg;
                buttons[i].ImageMouseDown = menu.ButtonMouseDownImg;
                buttons[i].ImageMouseOver = menu.ButtonMouseOverImg;
                buttons[i].ImageInvalid = menu.ButtonInvImg;
                buttons[i].ImageDisabled = menu.ButtonDisImg;

                buttons[i].Tag = sect.GetUIString("Tip", "GUI:Blank");


                buttons[i].ObjectLoad += MenuButtonLoadImpl;
                // 邦定脚本，如果有事件在这之前邦定上了脚本，那个事件就不再邦定了
                //buttons[i].OnLoad();

                //buttons[i].MouseMove += ShowHintHelper;
                //buttons[i].MouseDown += PlayClickSound;

                AddControl(buttons[i]);                
            }

            if (menuType == MenuType.Preview)
            {
                buttons[0].Image = menu.MapBoard;

                buttons[0].Size = menu.MapBoardSize;
                buttons[0].X = 644;
                buttons[0].Y = 199 - 42;
                buttons[0].Enabled = false;
                buttons[0].ImageDisabled = menu.MapBoard;

            }

            //string panSectName = Name + ".Panel";

            if (uiIni.TryGetValue(mainPanel.Name, out sect))
            {
                mainPanel.Parse(sect);
            }

            //isResReleased = false;
            isShown = true;
            //}
        }

        //void MouseDownImpl(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    for (int i = 0; i < 9; i++)
        //        buttons[i].OnMouseDown(this, e);
        //    mainPanel.OnMouseDown(this, e);
        //}
        //void MouseMoveImpl(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    menu.HintText = string.Empty;
            //for (int i = 0; i < 9; i++)
            //    buttons[i].OnMouseMove(this, e);
            //mainPanel.OnMouseMove(this, e);
        //}
        //void MouseUpImpl(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    for (int i = 0; i < 9; i++)
        //        buttons[i].OnMouseUp(this, e);
        //    mainPanel.OnMouseUp(this, e);
        //}
        //void PaintImpl(Sprite spr)
        //{

        //}
        //void CloseImpl(object sender)
        //{
        //    //mainPanel.OnClose(this);
        //    //base.Dispose(true);


        //    UnloadResource();
        //    isShown = false;
        //}


        //void UpdatingImpl(object sender, float dt)
        //{
            
        //    for (int i = 0; i < 9; i++)
        //    {
        //        buttons[i].Update(dt);
        //    }
        //    mainPanel.Update(dt);
        //}

        //#region IDisposable 成员

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //}
        //public void Dispose()
        //{
        //    if (!disposed || isShown)
        //    {
        //        //if (isShown)
        //        OnClose(this);
        //        //base.Dispose(true);
        //        disposed = true;
        //    }
        //    else
        //        throw new ObjectDisposedException(this.ToString());
        //}

        //#endregion

        //~MenuPage()
        //{
        //    if (!disposed || isShown)
        //        Dispose();
        //}

    }


    public class MenuPageFactory : IMenuPageFactory
    {

        #region IMenuPageFactory 成员

        public IMenuPage CreateInstance(GameUI gui, string name)
        {
            return new MenuPage(gui, name);
        }

        #endregion
    }
}
