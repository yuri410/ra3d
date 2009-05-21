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
using SlimDX.Direct3D9;
using SlimDX;
using R3D.ConfigModel;
using R3D.IO;
using R3D.Media;

namespace R3D.UI.Controls
{
    public class ImageControlFactory : ControlFactory 
    {

        public ImageControlFactory()
            : base("ImageControl")
        { 
        }

        public override Control CreateInstance(GameUI gameUI, string name)
        {
            return new ImageControl(gameUI, name);
        }
    }

    public class ImageControl : Control
    {
        Texture image;

        public static ImageControl FromConfigSection(GameUI gameUI, ConfigurationSection sect)
        {
            ImageControl res = new ImageControl(gameUI, sect.Name);
            res.Parse(sect);
            return res;
        }

        public ImageControl(GameUI gameUI, string name)
            : base(gameUI, name)
        {
        }

        public ImageControl(GameUI gameUI)
            : this(gameUI, null)
        {
        }

        public Texture Image
        {
            get { return image; }
            set { image = value; }
        }

        public override void Update(float dt)
        {                     
            base.Update(dt);
        }

        public override void PaintControl()
        {
            Sprite.Transform = Matrix.Translation(X, Y, 0);
            base.PaintControl();

            if (image != null)
            {
                Sprite.Draw(image, modColor.ToArgb());
            }
        }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            //ImageBase img = sect.GetImage(FileSystem.GameResLR);
            //image = img.GetTexture(device, Usage.None, Pool.Managed);
            //img.Dispose();

            UIImageInformation imgInfo = sect.GetImage(FileSystem.GameResLR);
            image = UITextureManager.Instance.CreateInstance(imgInfo);

        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                UITextureManager.Instance.DestoryInstance(image);
                image = null;
            }
            else
            {
                image = null;
            }
        }

    }
}
