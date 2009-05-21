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
using SlimDX.DirectInput;

namespace R3D.UI.Controls
{
    public class TextBoxFactory : ControlFactory
    {
        public TextBoxFactory()
            : base("TextBox")
        { }

        public override Control CreateInstance(GameUI gameUI, string name)
        {
            return new TextBox(gameUI, name);
        }
    }

    public class TextBox : TextControl
    {
        int currentPosition;

        

        public TextBox(GameUI gameUI, string name)
            : base(gameUI, name)
        {
            IsInputControl = true;
            Text = string.Empty;
        }

        void ProcessKey(Key k)
        {

            switch (k)
            {
                case Key.Backspace:
                    if (currentPosition > 0)
                    {
                        Text.Remove(currentPosition - 1, 1);
                    }
                    break;
                case Key.Delete:
                    if (currentPosition < Text.Length - 1)
                    {
                        Text.Remove(Text.Length - 1, 1);
                    }
                    break;
                case Key.Home:

                    break;
                case Key.End:

                    break;
                case Key.LeftArrow:
                    if (currentPosition > 0)
                        currentPosition--;
                    break;
                case Key.RightArrow:
                    if (currentPosition < Text.Length - 1)
                        currentPosition++;
                    break;
                default:

                    break;
            }
        }


        protected override void OnKeyPressed(System.Windows.Forms.KeyPressEventArgs e)
        {
            base.OnKeyPressed(e);

            
        }
    }
}
