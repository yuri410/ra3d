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
using System.Drawing;
using System.Windows.Forms;

using SlimDX;
using SlimDX.Direct3D9;
using R3D.Base;
using R3D.IO;
using R3D.ScriptEngine;
using R3D.UI.Controls;
using R3D.ConfigModel;

namespace R3D.UI
{
    /// <summary>
    /// 代表一个菜单页的主面板。
    /// </summary>
    public class MenuPanel : R3D.UI.Controls.Control
    {
        //CloseHandler close;

        Texture backImage;
        UIImageInformation bgInfo;

        LoadHandler loadingResource;
        LoadHandler unloadingResource;
        LoadHandler disposingHandler;

        public MenuPanel(GameUI gameUI, string name)
            : base(gameUI, name)
        {            
            Width = 632;
            Height = 568;
        }

        //public override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //}

        [ScriptEvent("loadingResource")]
        public event LoadHandler LoadingResource
        {
            add { loadingResource += value; }
            remove { loadingResource -= value; }
        }
        [ScriptEvent("unloadingResource")]
        public event LoadHandler UnloadingResource
        {
            add { unloadingResource += value; }
            remove { unloadingResource -= value; }
        }
        [ScriptEvent("disposingHandler")]
        public event LoadHandler Disposing
        {
            add { disposingHandler += value; }
            remove { disposingHandler -= value; }
        }

        public void LoadResource()
        {
            if (bgInfo != null)
            {
                backImage = UITextureManager.Instance.CreateInstance(bgInfo);
            }

            if (loadingResource != null)
            {
                loadingResource(this);
            }
        }
        public void UnloadResource()
        {
            if (backImage != null)
            {
                UITextureManager.Instance.DestoryInstance(backImage);
            }
            if (unloadingResource != null)
            {
                unloadingResource(this);
            }

        }
        
        

        public override void Parse(ConfigurationSection sect)
        {
            bgInfo = sect.GetImage(FileSystem.GameResLR);
        }

        protected void AutoParseConfigsForControls(R3D.ConfigModel.Configuration config)
        {
            for (int i = 0; i < ControlCount; i++)
            {
                string controlName = this.Name + Type.Delimiter + GetControl(i).Name;

                ConfigurationSection sect;
                if (config.TryGetValue(controlName, out sect))
                {
                    GetControl(i).Parse(sect);
                }
            }
        }


        //[ScriptEvent("close")]
        //public event CloseHandler Close
        //{
        //    add { close += value; }
        //    remove { close -= value; }
        //}

        public override void Update(float dt)
        {
            base.Update(dt);
        }
        public override void PaintControl()
        {
            Sprite.Transform = Matrix.Translation(X, Y, 0);
            if (backImage != null)
            {
                Sprite.Draw(backImage, modColor.ToArgb());
            }
            base.PaintControl();
        }

        //public void OnClose(object sender)
        //{
        //    if (close != null)
        //        close(this);
        //}
        //public override void OnMouseDown(object sender, MouseEventArgs e)
        //{
        //    isMouseDown = e.Button == MouseButtons.Left && IsInBounds(e.X, e.Y);
        //    if (isMouseDown && mouseDown != null)
        //        mouseDown(this, e);
        //}
        //public override void OnMouseMove(object sender, MouseEventArgs e)
        //{
        //    isMouseMove = IsInBounds(e.X, e.Y);
        //    if (isMouseMove && mouseMove != null)
        //        mouseMove(this, e);
        //}
        //public override void OnMouseUp(object sender, MouseEventArgs e)
        //{
        //    isMouseMove = IsInBounds(e.X, e.Y);
        //    if (isMouseDown && isMouseMove && base.mouseUp != null)
        //        mouseUp(this, e);

        //    isMouseDown = false;
        //    isMouseMove = false;
        //}

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (disposingHandler != null)
                {
                    disposingHandler(this);
                }
            }
        }
    }
}
