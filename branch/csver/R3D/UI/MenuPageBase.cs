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
using R3D.UI;
using R3D.ScriptEngine;
using R3D.ConfigModel;
using R3D.UI.Controls;
using SlimDX.DirectInput;

namespace R3D.UI
{
    public interface IMenuPage : IUserInterface, IDisposable
    {
        MenuType Type { get; set; }
        Button this[int i] { get; }
        string Name { get; }
        string UIName { get; }

        event CloseHandler Close;


        void CloseMenuPage();

        void InvokeMouseMove(System.Windows.Forms.MouseEventArgs e);

        void InvokeMouseUp(System.Windows.Forms.MouseEventArgs e);

        void InvokeMouseDown(System.Windows.Forms.MouseEventArgs e);
        void InvokeMouseWheel(System.Windows.Forms.MouseEventArgs e);
        void InvokeKeyStateChanged(KeyCollection pressed);

        void InvokeKeyPressed(System.Windows.Forms.KeyPressEventArgs e);
    }

    public interface IMenuPageFactory
    {
        IMenuPage CreateInstance(GameUI gui, string name);
    }

    public abstract class MenuPageBase : Control
    {
        protected Button[] buttons;
        /// <summary>
        /// 左边主面板
        /// </summary>
        protected MenuPanel mainPanel;
        protected Menu menu;

        protected MenuType menuType;

        protected string uiName;
        protected CloseHandler close;

        protected bool isShown;

        bool isResReleased = true;


        public MenuPageBase(GameUI gameUI, string name)
            : base(gameUI, name)
        {
            buttons = new Button[9];
            mainPanel = new MenuPanel(gameUI, name + System.Type.Delimiter + "Panel");
            menu = gameUI.Menu;

            AddControl(mainPanel);
        }

        [ScriptEvent("close")]
        public event CloseHandler Close
        {
            add { close += value; }
            remove { close -= value; }
        }

        protected virtual void OnClose()
        {
            if (close != null)
                close(this);
        }

        protected virtual void LoadResource()
        {
            if (mainPanel != null && !mainPanel.Disposed)
            {
                mainPanel.LoadResource();
            }
        }
        protected virtual void UnloadResource()
        {
            if (mainPanel != null && !mainPanel.Disposed)
            {
                mainPanel.UnloadResource();
            }
        }

        public void CloseMenuPage()
        {
            OnClose();
            if (!isResReleased)
            {
                UnloadResource();
                isResReleased = true;
            }
        }

        public override void Update(float dt)
        {

            base.Update(dt);
            if (isResReleased)
            {
                LoadResource();
                isResReleased = false;
            }
            //for (int i = 0; i < 9; i++)
            //{
            //    buttons[i].Update(dt);
            //}
            //mainPanel.Update(dt);
        }
        public override void PaintControl()
        {
            if (isResReleased)
            {
                LoadResource();
                isResReleased = false;
            }
            base.PaintControl();


            //for (int i = 0; i < 9; i++)
            //    buttons[i].PaintControl();
            //mainPanel.PaintControl();
        }

        //protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        //{
        //    base.OnMouseDown(e);

        //    for (int i = 0; i < 9; i++)
        //        buttons[i].InvokeMouseDown(e);
        //    mainPanel.InvokeMouseDown(e);
        //}
        //protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        //{
        //    base.OnMouseUp(e);

        //    for (int i = 0; i < 9; i++)
        //        buttons[i].InvokeMouseUp(e);
        //    mainPanel.InvokeMouseUp(e);
        //}
        //protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        //{
        //    base.OnMouseMove(e);
        //    menu.HintText = string.Empty;
        //    for (int i = 0; i < 9; i++)
        //        buttons[i].InvokeMouseMove(e);
        //    mainPanel.InvokeMouseMove(e);
        //}
        //public override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        //{
        //    base.OnMouseWheel(e);
        //    for (int i = 0; i < 9; i++)
        //        buttons[i].InvokeMouseWheel(e);
        //    mainPanel.InvokeMouseWheel(e);
        //}

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);
            menu.HintText = string.Empty;
        }

        //public override void Update(float dt)
        //{
        //    base.Update(dt);

        //    if (buttons != null)
        //    {
        //        for (int i = 0; i < buttons.Length; i++)
        //        {
        //            buttons[i].Update(dt);
        //        }
        //    }
        //    if (mainPanel != null)
        //    {
        //        mainPanel.Update(dt);
        //    }
        //}

        //public override void OnLoad()
        //{
        //    Bind();
        //    objectLoad();
        //}
        //public MenuPanel Panel
        //{
        //    get { return mainPanel; }
        //}
        public MenuType Type
        {
            get
            {
                if (!IsLoaded)
                {
                    OnLoad();
                }
                return menuType;
            }
            set { menuType = value; }
        }
        public Button this[int i]
        {
            get
            {
                if (!IsLoaded)
                {
                    OnLoad();
                }
                return buttons[i];
            }
        }

        /// <summary>
        /// 获得菜单页的在用户界面上显示的名称。
        /// </summary>
        public string UIName
        {
            get
            {
                if (!IsLoaded)
                {
                    OnLoad();
                }
                return uiName;
            }
        }

        //public abstract void Load;
        //public abstract void Unload;
        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (buttons[i] != null && !buttons[i].Disposed)
                    {
                        buttons[i].Dispose();
                        buttons[i] = null;
                    }
                }
                if (mainPanel != null && !mainPanel.Disposed)
                {
                    mainPanel.Dispose();
                    mainPanel = null;
                }
            }
        }
    }
}
