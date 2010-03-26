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
using System.Windows.Forms;

namespace R3D.InputEngine
{
    // TODO: implementation independent

    public class Input : IDisposable
    {
        DIMouse mouse;
        DIKeyboard keyboard;

        bool disposed;

        public event MouseHandler MouseWheel
        {
            add
            {
                if (mouse != null)
                {
                    mouse.MouseWheel += value;
                }
            }
            remove
            {
                if (mouse != null)
                {
                    mouse.MouseWheel -= value;
                }
            }
        }
        public event MouseHandler MouseMove
        {
            add
            {
                if (mouse != null)
                {
                    mouse.MouseMove += value;
                }
            }
            remove
            {
                if (mouse != null)
                {
                    mouse.MouseMove += value;
                }
            }
        }
        public event MouseHandler MouseDown
        {
            add
            {
                if (mouse != null)
                {
                    mouse.MouseDown += value;
                }
            }
            remove
            {
                if (mouse != null)
                {
                    mouse.MouseDown += value;
                }
            }
        }
        public event MouseHandler MouseUp
        {
            add
            {
                if (mouse != null)
                {
                    mouse.MouseUp += value;
                }
            }
            remove
            {
                if (mouse != null)
                {
                    mouse.MouseUp += value;
                }
            }
        }
        //public event MouseHandler MouseClick
        //{
        //    add
        //    {
        //        if (mouse != null)
        //        {
        //            mouse.MouseClick += value;
        //        }
        //    }
        //    remove
        //    {
        //        if (mouse != null)
        //        {
        //            mouse.MouseClick += value;
        //        }
        //    }
        //}

        public event KeyHandler KeyStateChanged
        {
            add
            {
                if (keyboard != null)
                {
                    keyboard.KeyStateChanged += value;
                }
            }
            remove
            {
                if (keyboard != null)
                {
                    keyboard.KeyStateChanged -= value;
                }
            }
        }

        public Input(Control form)
        {
            DirectInput.Initialize();

            keyboard = new DIKeyboard(form);
            mouse = new DIMouse(form);
        }

        public void Update(float dt)
        {
            if (mouse != null)
            {
                mouse.Update(dt);
            }
            if (keyboard != null)
            {
                keyboard.Update(dt);
            }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                mouse.Dispose();
                mouse = null;
                keyboard.Dispose();
                keyboard = null;
                disposed = true;
            }
            else
            {                 
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        ~Input()
        {
            if (!disposed)
                Dispose();
        }
    }


    public interface IKeyboard : IDisposable
    {
        void Update(float dt);

        event KeyHandler KeyStateChanged;
    }
    public interface IMouse : IDisposable
    {
        void Update(float dt);


        event MouseHandler MouseWheel;
        event MouseHandler MouseMove;
        event MouseHandler MouseDown;
        event MouseHandler MouseUp;
        //event MouseHandler MouseClick;

    }
}
