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
using System.Drawing.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using SlimDX;
using SlimDX.Direct3D9;

using R3D.IO;
using R3D.Base;
using R3D.ConfigModel;

namespace R3D.UI.Controls
{
    public class LabelFactory : ControlFactory
    {
        public LabelFactory()
            : base("Label")
        {
        }

        public override Control CreateInstance(GameUI gameUI, string name)
        {
            return new Label(gameUI, name);
        }
    }
    public class Label : TextControl
    {
        public Label(GameUI gameUI)
            : base(gameUI)
        {
        }

        public Label(GameUI gameUI, string name)
            : base(gameUI, name)
        {
        }

        public static Label FromConfigSection(GameUI gameUI, ConfigurationSection sect)
        {
            Label res = new Label(gameUI, sect.Name);
            res.Parse(sect);
            return res;
        }

        public override string ToString()
        {
            return "{ Text= " + Text + " }";
        }

        public override void PaintControl()
        {
            Sprite.Transform = Matrix.Translation(X, Y, 0);
            base.PaintControl();
        }
    }

    
}
