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
using SlimDX;

namespace R3D.InputEngine
{
    public class DIKeyboard : IKeyboard
    {
        const int BufferSize = 20;

        bool disposed;

        Device<KeyboardState> keyboard;

        Control parent;

        KeyboardState lastState;

        public bool IsImmediate
        {
            get;
            private set;
        }

        public DIKeyboard(Control form)
            : this(form, false, true, false, true)        
        {
        }

        public DIKeyboard(Control form, bool exclusive, bool foreground, bool disableWindowsKeys, bool isImmediate)
        {
            IsImmediate = isImmediate;
            parent = form;


            CooperativeLevel cooperativeLevel;//= CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground;

            if (exclusive)
                cooperativeLevel = CooperativeLevel.Exclusive;
            else
                cooperativeLevel = CooperativeLevel.Nonexclusive;

            if (foreground)
                cooperativeLevel |= CooperativeLevel.Foreground;
            else
                cooperativeLevel |= CooperativeLevel.Background;

            if (disableWindowsKeys && !exclusive && foreground)
                cooperativeLevel |= CooperativeLevel.NoWinKey;



            keyboard = new Device<KeyboardState>(SystemGuid.Keyboard);
            keyboard.SetCooperativeLevel(form, cooperativeLevel);

            if (!IsImmediate)
            {
                // IMPORTANT STEP TO USE BUFFERED DEVICE DATA!
                //
                // DirectInput uses unbuffered I/O (buffer size = 0) by default.
                // If you want to read buffered sounds, you need to set a nonzero
                // buffer size.
                //
                // The buffer size is an int property associated with the device.

                try
                {
                    keyboard.Properties.BufferSize = BufferSize;
                }
                catch (Exception)
                {
                    return;
                }
            }


            keyboard.Acquire();


            //keyDelay = delay;
            //delays = new float[((int)Key.MediaSelect) + 1];
            //for (int i = (int)Key.Escape; i < (int)Key.MediaSelect; i++)
            //{
            //    delays[i] = 0.0f;
            //}

        }

        public event KeyHandler KeyStateChanged;

        #region IKeyboard 成员

        public void Update(float dt)
        {
            // be sure that the device is still acquired
            if (keyboard.Acquire().IsFailure)
                return;

            // poll for more input
            if (keyboard.Poll().IsFailure)
                return;

            // get the current state of the keyboard
            KeyboardState state = new KeyboardState();// keyboard.GetCurrentState();
            keyboard.GetCurrentState(ref state);

            if (Result.Last.IsFailure)
                return;


            if (lastState != null)
            {
                KeyCollection pressed = state.PressedKeys;
                KeyCollection lastPressed = lastState.PressedKeys;

                if (pressed.Count != lastPressed.Count)
                {
                    if (KeyStateChanged != null)
                    {
                        KeyStateChanged(this, pressed);
                    }
                }
                else
                {
                    bool equals = true;
                    foreach (Key key in pressed)
                    {
                        if (!lastPressed.Contains(key))
                        {
                            equals = false;
                            break;
                        }
                    }
                    if (equals && KeyStateChanged != null)
                    {
                        KeyStateChanged(this, pressed);
                    }
                }

            }
            lastState = state;       
        }

        #endregion

        public bool IsPressed(Key key)
        {
            if (IsImmediate)
            {
                // be sure that the device is still acquired
                if (keyboard.Acquire().IsFailure)
                    return false;

                // poll for more input
                if (keyboard.Poll().IsFailure)
                    return false;

                KeyboardState state = keyboard.GetCurrentState();

                if (Result.Last.IsFailure)
                    return false;

                return state.IsPressed((Key)key);
            }
            return false;
        }


        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                keyboard.Unacquire();
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

    }
}
