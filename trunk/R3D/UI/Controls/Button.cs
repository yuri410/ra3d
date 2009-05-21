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
using System.Drawing.Text;
using System.Text;
using System.Windows.Forms;
using R3D.ConfigModel;
using R3D.Base;
using R3D.IO;
using R3D.ScriptEngine;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.UI.Controls
{
    public class ButtonFactory : ControlFactory
    {
        public ButtonFactory()
            : base("Button")
        {
        }

        public override Control CreateInstance(GameUI gameUI, string name)
        {
            return new Button(gameUI, name);
        }
    }

    public class Button : TextControl, IDisposable
    {

        public static Button FromConfigSection(GameUI gameUI, ConfigurationSection sect)
        {
            Button res = new Button(gameUI, sect.Name);
            res.Parse(sect);
            return res;
        }

        public Button(GameUI gameUI, string name)
            : base(gameUI, name)
        {
        }

        public Button(GameUI gameUI)
            : this(gameUI, null)
        { }


        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            this.IsValid = sect.GetBool("IsValid", false);
        }

        public bool IsValid
        {
            get;
            set;
        }

        public Texture ImageDisabled
        {
            get;
            set;
        }
        public Texture ImageInvalid
        {
            get;
            set;
        }
        public Texture ImageMouseDown
        {
            get;
            set;
        }
        public Texture ImageMouseOver
        {
            get;
            set;
        }
        public Texture Image
        {
            get;
            set;
        }


        public override void PaintControl()
        {
            //spr.Transform = Matrix.Translation((int)(bounds.X + (bounds.Width - lblWidth) * 0.5f), (int)(bounds.Y + (bounds.Height - lblHeight) * 0.5f), 0);
            Sprite.Transform = Matrix.Translation(X, Y, 0);
            if (IsValid)
            {
                if (Enabled)
                {
                    if (IsPressed)
                    {
                        if (ImageMouseDown != null)
                            Sprite.Draw(ImageMouseDown, modColor.ToArgb());
                    }
                    else if (IsMouseOver)
                    {
                        if (ImageMouseOver != null)
                            Sprite.Draw(ImageMouseOver, modColor.ToArgb());
                    }
                    else
                    {
                        if (Image != null)
                            Sprite.Draw(Image, modColor.ToArgb());
                    }
                }
                else
                {
                    if (ImageDisabled != null)
                        Sprite.Draw(ImageDisabled, modColor.ToArgb());
                }
            }
            else
            {
                if (ImageInvalid != null)
                    Sprite.Draw(ImageInvalid, modColor.ToArgb());
            }

            if (IsPressed)
            {
                Sprite.Transform = Matrix.Translation(X + 1, Y + 1, 0);
            }
            base.PaintControl();
        }


        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            //if (disposing)
            //{
                //if (Image != null)
                //    Image.Dispose();
                //if (ImageDisabled != null)
                //    ImageDisabled.Dispose();
                //if (ImageInvalid != null)
                //    ImageInvalid.Dispose();
                //if (ImageMouseDown != null)
                //    ImageMouseDown.Dispose();
                //if (ImageMouseOver != null)
                //    ImageMouseOver.Dispose();
            //}
            Image = null;
            ImageDisabled = null;
            ImageInvalid = null;
            ImageMouseDown = null;
            ImageMouseOver = null;
        }

    }
}
