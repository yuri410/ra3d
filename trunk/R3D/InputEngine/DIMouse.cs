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

using SlimDX;
using SlimDX.DirectInput;
using System.Drawing;

namespace R3D.InputEngine
{
    public class DIMouse : IMouse
    {
        bool disposed;

        Device<MouseState> mouse;
        Control parent;

        MouseState lastState;        
        //MouseButtons lastMouseBtn;

        Point currentPosition;

        public DIMouse(Control form)
            : this(form, false, true, false)
        {
        }

        public bool Immediate
        {
            get;
            protected set;
        }

        public DIMouse(Control form, bool exclusive, bool foreground, bool immediate)
        {
            parent = form;
            // make sure that DirectInput has been initialized
            // build up cooperative flags
            CooperativeLevel cooperativeLevel = CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground;

            mouse = new Device<MouseState>(SystemGuid.Mouse);
            mouse.SetCooperativeLevel(form, cooperativeLevel);

            Immediate = immediate;
            if (!immediate)
            {
                mouse.Properties.BufferSize = 8;
            }
            mouse.Acquire();
        }

        #region IMouse 成员

        void Process(MouseState state)
        {

            if (lastState != null)
            {
                bool[] buttons = state.GetButtons();
                bool[] lastButtons = lastState.GetButtons();

                MouseButtons mb = MouseButtons.None;
                if (buttons[0])
                    mb |= MouseButtons.Left;

                if (buttons[1])
                    mb |= MouseButtons.Right;

                if (buttons[2])
                    mb |= MouseButtons.Middle;

                MouseEventArgs arg = null;

                //currentPosition.X += state.X;
                //currentPosition.Y += state.Y;

                currentPosition = parent.PointToClient(Form.MousePosition);

                if (!(state.X == 0 && state.Y == 0))
                {
                    if (MouseMove != null)
                    {
                        if (arg == null)
                        {
                            arg = new MouseEventArgs(mb, 1, currentPosition.X, currentPosition.Y, state.Z);
                        }
                        MouseMove(this, arg);
                    }
                }

                if (state.Z != 0)
                {
                    if (MouseWheel != null)
                    {
                        if (arg == null)
                        {
                            arg = new MouseEventArgs(mb, 1, currentPosition.X, currentPosition.Y, state.Z);
                        }
                        MouseWheel(this, arg);
                    }
                }

                if ((buttons[0] && !lastButtons[0]) || (buttons[1] && !lastButtons[1]) || (buttons[2] && !lastButtons[2]))
                {
                    if (MouseDown != null)
                    {
                        if (arg == null)
                        {
                            arg = new MouseEventArgs(mb, 1, currentPosition.X, currentPosition.Y, state.Z);
                        }
                        MouseDown(this, arg);
                    }
                }
                if ((!buttons[0] && lastButtons[0]) || (!buttons[1] && lastButtons[1]) || (!buttons[2] && lastButtons[2]))
                {
                    if (MouseUp != null)
                    {
                        mb = MouseButtons.None;
                        if (lastButtons[0])
                            mb |= MouseButtons.Left;

                        if (lastButtons[1])
                            mb |= MouseButtons.Right;

                        if (lastButtons[2])
                            mb |= MouseButtons.Middle;


                        if (arg == null)
                        {
                            arg = new MouseEventArgs(mb, 1, currentPosition.X, currentPosition.Y, state.Z);
                        }
                        MouseUp(this, arg);
                    }
                }
            }
            lastState = state;
        }

        public void Update(float dt)
        {
            // be sure that the device is still acquired
            if (mouse.Acquire().IsFailure)
                return;

            // poll for more input
            if (mouse.Poll().IsFailure)
                return;

            if (Immediate)
            {
                // get the list of buffered sounds events
                BufferedDataCollection<MouseState> bufferedData = mouse.GetBufferedData();
                if (Result.Last.IsFailure || bufferedData.Count == 0)
                    return;

                foreach (BufferedData<MouseState> packet in bufferedData)
                {
                    Process(packet.Data);
                }
            }
            else
            {
                try
                {
                    // get the current state of the mouse
                    MouseState state = new MouseState();// mouse.GetCurrentState();
                    mouse.GetCurrentState(ref state);

                    if (Result.Last.IsFailure)
                        return;
                    Process(state);
                }
                catch                
                {
                    return;
                }
            }
        }

        public event MouseHandler MouseWheel;
        public event MouseHandler MouseMove;
        public event MouseHandler MouseDown;
        public event MouseHandler MouseUp;


        #endregion

        //public void Update()
        //{
        //    // be sure that the device is still acquired
        //    if (mouse.Acquire().IsFailure)
        //        return;

        //    // poll for more input
        //    if (mouse.Poll().IsFailure)
        //        return;


        //    // get the list of buffered sounds events
        //    BufferedDataCollection<MouseState> bufferedData = mouse.GetBufferedData();
        //    if (Result.Last.IsFailure || bufferedData.Count == 0)
        //        return;

        //    MouseState result = new MouseState();

        //    for (int i = 0; i < bufferedData.Count; i++)
        //    {
        //        result.X += packet.Data.X;
        //        result.Y += packet.Data.Y;
        //        result.Z += packet.Data.Z;
        //    }
        //}

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                if (mouse != null)
                {
                    mouse.Unacquire();
                    mouse.Dispose();
                }
                mouse = null;

                disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        ~DIMouse()
        {
            if (!disposed)
            {
                Dispose();
            }
        }

        //void CreateDevice()
        //{

        //    // create the device
        //    try
        //    {
        //        mouse = new Device<MouseState>(SystemGuid.Mouse);
        //        mouse.SetCooperativeLevel(this, CooperativeLevel.Nonexclusive | CooperativeLevel.Foreground);
        //    }
        //    catch (DirectInputException e)
        //    {
        //        MessageBox.Show(e.Message);
        //        return;
        //    }

        //    //if (!immediateRadio.Checked)
        //    //{
        //    //    // since we want to use buffered sounds, we need to tell DirectInput
        //    //    // to set up a buffer for the sounds
        //    //    mouse.Properties.BufferSize = 8;
        //    //}

        //    // acquire the device
        //    mouse.Acquire();

        //    // set the timer to go off 12 times a second to read input
        //    // NOTE: Normally applications would read this much faster.
        //    // This rate is for demonstration purposes only.
        //    timer.Interval = 1000 / 12;
        //    timer.Start();
        //}

        //void ReadImmediateData()
        //{
        //    // be sure that the device is still acquired
        //    if (mouse.Acquire().IsFailure)
        //        return;

        //    // poll for more input
        //    if (mouse.Poll().IsFailure)
        //        return;

        //    // get the current state of the mouse
        //    MouseState state = mouse.GetCurrentState();
        //    if (Result.Last.IsFailure)
        //        return;

        //    StringBuilder sounds = new StringBuilder();
        //    sounds.AppendFormat(CultureInfo.CurrentCulture, "(X={0} Y={1} Z={2})", state.X, state.Y, state.Z);
        //    for (int i = 0; i < 8; i++)
        //    {
        //        sounds.Append(" B");
        //        sounds.Append(i);
        //        sounds.Append("=");
        //        if (state.IsPressed(i))
        //            sounds.Append("1");
        //        else
        //            sounds.Append("0");
        //    }

        //    dataBox.Text = sounds.ToString();
        //}

        //void ReadBufferedData()
        //{


        //    StringBuilder sounds = new StringBuilder();

        //    MouseState result = new MouseState();
        //    foreach (BufferedData<MouseState> packet in bufferedData)
        //    {
        //        result.X += packet.Data.X;
        //        result.Y += packet.Data.Y;
        //        result.Z += packet.Data.Z;
        //    }

        //    sounds.AppendFormat(CultureInfo.CurrentCulture, "(X={0} Y={1} Z={2})", result.X, result.Y, result.Z);
        //    for (int i = 0; i < 8; i++)
        //    {
        //        sounds.Append(" B");
        //        sounds.Append(i);
        //        sounds.Append("=");
        //        if (bufferedData[bufferedData.Count - 1].Data.IsPressed(i))
        //            sounds.Append("1");
        //        else
        //            sounds.Append("0");
        //    }

        //    dataBox.Text = sounds.ToString();
        //}


    }
}
