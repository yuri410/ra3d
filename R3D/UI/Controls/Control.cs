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
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using R3D.ConfigModel;
using R3D.Base;
using R3D.InputEngine;
using R3D.IO;
using R3D.ScriptEngine;
using SlimDX;
using SlimDX.Direct3D9;
using SlimDX.DirectInput;

namespace R3D.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Control : ScriptableUI, IConfigurable
    {
        public static bool IsTextRight2Left
        {
            get;
            private set;
        }

        static Control()
        {
            IsTextRight2Left = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft;
        }

        protected Color backColor;
        protected Color foreColor = Color.White;
        protected Color modColor = Color.White;

      



        protected object tag;
        protected Padding padding;

        protected KeyHandler hotkeyInvoked;
        protected KeyEventHandler keyPressed;

        bool hasFocus;

        protected Device Device
        {
            get;
            private set;
        }

        protected GameUI GameUI
        {
            get;
            private set;
        }

        List<Control> controls;

        /// <summary>
        /// Occurs when the control's hotkey is invoked.
        /// </summary>
        [ScriptEvent("hotkeyInvoked")]
        public event KeyHandler HotkeyInvoked
        {
            add { hotkeyInvoked += value; }
            remove { hotkeyInvoked -= value; }
        }

        [ScriptEvent("keyPressed")]
        public event KeyEventHandler KeyPressed
        {
            add { keyPressed += value; }
            remove { keyPressed -= value; }
        }

        protected Control(GameUI gameUI)
            : this(gameUI, null)
        {
            HotKey = new List<Key>();
        }

        protected Control(GameUI gameUI, string scriptName)
            : base(scriptName)
        {
            Device = gameUI.Device;
            Sprite = gameUI.GetSprite;
            GameUI = gameUI;

            //txtHint = TextRenderingHint.ClearTypeGridFit;
            modColor = Color.White;

            controls = new List<R3D.UI.Controls.Control>();

            //InputEngine = gameUI.InputEngine;

            //InputEngine.KeyStateChanged += this._OnKeyStateChanged;
            //InputEngine.MouseDown += this._OnMouseDown;
            //InputEngine.MouseMove += this._OnMouseMove;
            //InputEngine.MouseUp += this._OnMouseUp;
            //InputEngine.MouseWheel += this._OnMouseWheel;
        }

        public void AddControl(Control ctl)
        {
            ctl.Parent = this;
            controls.Add(ctl);
        }

        public void RemoveControl(Control ctl)
        {
            ctl.Parent = null;
            controls.Remove(ctl);
        }
        public Control GetControl(int index)
        {
            return controls[index];
        }
        public int ControlCount
        {
            get { return controls.Count; }
        }
        //public Input InputEngine
        //{
        //    get;
        //    private set;
        //}
        public bool HasHotKey
        {
            get;
            set;
        }
        public bool IsInputControl
        {
            get;
            set;
        }
        public List<Key> HotKey
        {
            get;
            private set;
        }
        public Sprite Sprite
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the parent dialog.
        /// </summary>
        /// <value>The parent dialog.</value>
        public Control Parent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets  or sets the focused control.
        /// </summary>
        /// <value>The focused control.</value>
        public Control FocusControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has focus.
        /// </summary>
        /// <value><c>true</c> if this instance has focus; otherwise, <c>false</c>.</value>
        public bool HasFocus
        {
            get { return hasFocus; }
            set
            {
                // avoid unecessary changes
                if (hasFocus == value)
                    return;

                // update the value
                hasFocus = value;

                // check if we now have the focus
                if (hasFocus && Parent != null && Parent.FocusControl != this)
                {
                    // update the state
                    Parent.ClearFocus();
                    Parent.FocusControl = this;
                }
            }
        }

        /// <summary>
        /// Clears the focus from any controls.
        /// </summary>
        public void ClearFocus()
        {
            // check if we have a focus control
            if (FocusControl != null)
            {
                //// clear the focus
                if (FocusControl.HasFocus)
                    FocusControl.HasFocus = false;
                FocusControl = null;
            }
        }

        #region Properities


        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        public bool Enabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the mouse is over the control.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the mouse is over the control; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseOver
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is pressed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is pressed; otherwise, <c>false</c>.
        /// </value>
        public bool IsPressed
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the X position.
        /// </summary>
        /// <value>The X position.</value>
        public int X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y position.
        /// </summary>
        /// <value>The Y position.</value>
        public int Y
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public int Width
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public int Height
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public Size Size
        {
            get { return new Size(Width, Height); }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get { return new Point(X, Y); }
            set { X = value.X; Y = value.Y; }
        }

        /// <summary>
        /// Gets or sets the bounds.
        /// </summary>
        /// <value>The bounds.</value>
        public Rectangle Bounds
        {
            get { return new Rectangle(X, Y, Width, Height); }
            set
            {
                X = value.X; Y = value.Y; Width = value.Width; Height = value.Height;
            }
        }

        public Color BackColor
        {
            get { return backColor; }
            set { backColor = value; }
        }

        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }

        #endregion

        public Color ModulateColor
        {
            get { return modColor; }
            set { modColor = value; }
        }

        public object Tag
        {
            get;
            set;
        }

        /// <summary>
        ///   Updates the control.
        /// </summary>
        /// <param name="dt">The elapsed time.</param>
        public virtual void Update(float dt)
        {
            if (!IsLoaded)
            {
                OnLoad();
            }
            OnUpdate(dt);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].Update(dt);
            }
        }

        /// <summary>
        ///   Draws the control.
        /// </summary>
        public virtual void PaintControl()
        {
            OnPaint(Sprite);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].PaintControl();
            }
        }

        public virtual void Parse(ConfigurationSection sect)
        {
            OnConfigParsing(sect);
            this.Enabled = sect.GetBool("Enabled", true);

            this.BackColor = sect.GetColorRGBA("BackColor", Color.Transparent);
            this.ForeColor = sect.GetColorRGBA("ForeColor", Color.White);

            this.Location = sect.GetPoint("Position", Point.Empty);

            modColor = sect.GetColorRGBA("ModulateColor", Color.White);

            this.Size = sect.GetSize("Size", new Size(100, 100));

            this.HasHotKey = sect.GetBool("HasHotKey", false);

            int[] pd = sect.GetIntArray("Padding", null);
            if (pd != null)
            {
                if (pd.Length == 4)
                {
                    padding.Left = pd[0];
                    padding.Top = pd[1];
                    padding.Right = pd[2];
                    padding.Bottom = pd[3];
                }
                else
                {
                    padding.All = pd[0];
                }
            }
        }

        protected bool IsInBounds(int x, int y)
        {
            return (x >= X) && (y >= Y) && (x <= Width + X) && (y <= Height + Y);
        }
        public static bool IsInBounds(int x, int y, ref Rectangle rect)
        {
            return (x >= rect.X) && (y >= rect.Y) && (x <= rect.Width + rect.X) && (y <= rect.Height + rect.Y);
        }

        protected virtual void OnKeyPressed(KeyPressEventArgs e) { }
      

        protected override void OnMouseMove(MouseEventArgs e)
        {
            bool inBounds = IsInBounds(e.X, e.Y);


            if (!IsMouseOver && inBounds)
            {
                OnMouseEnter(e);
            }
            else if (IsMouseOver && !inBounds)
            {
                OnMouseLeave(e);
            }

            if (inBounds)
            {
                base.OnMouseMove(e);
            }
            
        }

        /// <summary>
        ///   Raises the <see cref="E:MouseEnter"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event sounds.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            // mouse is over
            IsMouseOver = true;
        }

        /// <summary>
        ///   Raises the <see cref="E:MouseLeave"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event sounds.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            // mouse is not over
            IsMouseOver = false;
        }

        /// <summary>
        ///   Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (IsMouseOver)
            {
                HasFocus = true;
                base.OnMouseDown(e);
                // the mouse is now down
                if (e.Button == MouseButtons.Left)
                    IsPressed = true;

            }
        }

        /// <summary>
        ///   Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            // if the mouse was down, we just got a click
            if (IsPressed && e.Button == MouseButtons.Left)
            {
                base.OnMouseUp(e);


                if (Enabled && IsInBounds(e.X, e.Y))
                {
                    // raise the event
                    OnMouseClick(e);
                }
                IsPressed = false;
                IsMouseOver = false;
            }
        }

        public override void OnMouseWheel(MouseEventArgs e)
        {
            if (IsMouseOver)
            {
                base.OnMouseWheel(e);
            }
        }

        /// <summary>
        ///   Raises the <see cref="E:HotkeyInvoked"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnHotkeyInvoked(KeyCollection pressed)
        {
            // raise the event
            if (hotkeyInvoked != null)
                hotkeyInvoked(this, pressed);
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].Parent = null;
                }
                controls.Clear();
            }
        }

        public void InvokeMouseMove(MouseEventArgs e)
        {
            OnMouseMove(e);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].InvokeMouseMove(e);
            }
        }
        public void InvokeMouseUp(MouseEventArgs e)
        {
            OnMouseUp(e);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].InvokeMouseUp(e);
            }
        }
        public void InvokeMouseDown(MouseEventArgs e)
        {
            OnMouseDown(e);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].InvokeMouseDown(e);
            }
        }
        public void InvokeMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].InvokeMouseWheel(e);
            }
        }

        public void InvokeKeyPressed(KeyPressEventArgs e)
        {
            if (FocusControl != null)
            {
                FocusControl.InvokeKeyPressed(e);
            }
            else
            {
                this.OnKeyPressed(e);
            }
            
            
        }
        public void InvokeKeyStateChanged(KeyCollection pressed)
        {
            if (HasHotKey)
            {
                bool passed = true;
                for (int i = 0; i < HotKey.Count; i++)
                {
                    if (!pressed.Contains(HotKey[i]))
                    {
                        passed = false;
                        break;
                    }
                }

                if (passed)
                {
                    OnHotkeyInvoked(pressed);
                }
            }
            else if (IsInputControl)
            {
                OnHotkeyInvoked(pressed);
            }
            for (int i = 0; i < controls.Count; i++)
            {
                controls[i].InvokeKeyStateChanged(pressed);
            }
        }

    }
}
