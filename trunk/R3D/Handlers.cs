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
using R3D.ConfigModel;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace R3D
{

    public delegate void LoadHandler(object sender);

    public delegate void CloseHandler(object sender);
    public delegate void UpdateHandler(object sender, float dt);
    //public delegate void PaintHandler();
    public delegate void PaintHandler2D(Sprite spr);

    public delegate void MouseHandler(object sender, MouseEventArgs e);

    public delegate void KeyHandler(object sender, KeyCollection pressed);

    public delegate void ParseHander(object sender, ConfigurationSection sect);
    
}
