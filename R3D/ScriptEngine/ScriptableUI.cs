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
using SlimDX.Direct3D9;
using R3D.ConfigModel;

namespace R3D.ScriptEngine
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class ScriptableUI : ScriptableObject
    {
 
        protected PaintHandler2D paint;
        protected MouseHandler mouseMove;
        protected MouseHandler mouseUp;
        protected MouseHandler mouseDown;
        protected MouseHandler mouseClick;
        protected MouseHandler mouseWheel;

        //protected LoadHandler objectLoad;
        protected EventHandler mouseEnter;
        protected EventHandler mouseLeave;

        protected UpdateHandler updating;
        protected ParseHander configParsing;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName">The name of the ScriptableUI object</param>
        protected ScriptableUI(string typeName)
            : base(typeName)
        { }

        //[ScriptEvent("objectLoad")]
        //public event LoadHandler ObjectLoad
        //{
        //    add { objectLoad += value; }
        //    remove { objectLoad -= value; }
        //}

        [ScriptEvent("paint")]
        public event PaintHandler2D Paint
        {
            add { paint += value; }
            remove { paint -= value; }
        }

        /// <summary>
        /// Occurs when a mouse button is pressed.
        /// </summary>
        [ScriptEvent("mouseClick")]
        public event MouseHandler MouseClick
        {
            add { mouseClick += value; }
            remove { mouseClick -= value; }
        }

        /// <summary>
        /// Occurs when a mouse button is pressed.
        /// </summary>
        [ScriptEvent("mouseDown")]
        public event MouseHandler MouseDown
        {
            add { mouseDown += value; }
            remove { mouseDown -= value; }
        }

        /// <summary>
        /// Occurs when the mouse moves over the control.
        /// </summary>
        [ScriptEvent("mouseMove")]
        public event MouseHandler MouseMove
        {
            add { mouseMove += value; }
            remove { mouseMove -= value; }
        }

        /// <summary>
        /// Occurs when a mouse button is released.
        /// </summary>
        [ScriptEvent("mouseUp")]
        public event MouseHandler MouseUp
        {
            add { mouseUp += value; }
            remove { mouseUp -= value; }
        }

        /// <summary>
        /// Occurs when the mouse enters the dialog.
        /// </summary>
        [ScriptEvent("mouseEnter")]
        public event EventHandler MouseEnter
        {
            add { mouseEnter += value; }
            remove { mouseEnter -= value; }
        }

        /// <summary>
        /// Occurs when the mouse leaves the dialog.
        /// </summary>
        [ScriptEvent("mouseLeave")]
        public event EventHandler MouseLeave
        {
            add { mouseLeave += value; }
            remove { mouseLeave -= value; }
        }

        /// <summary>
        /// Occurs when the mouse wheel is spun.
        /// </summary>
        [ScriptEvent("mouseWheel")]
        public event MouseHandler MouseWheel
        {
            add { mouseWheel += value; }
            remove { mouseWheel -= value; }
        }

        [ScriptEvent("updating")]
        public event UpdateHandler Updating 
        {
            add { updating += value; }
            remove { updating -= value; }
        }

        [ScriptEvent("configParsing")]
        public event ParseHander ConfigParsing
        {
            add { configParsing += value; }
            remove { configParsing -= value; }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            if (mouseClick != null)
            {
                mouseClick(this, e);
            }
        }

        

        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            if (mouseUp != null)
            {
                mouseUp(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            if (mouseDown != null)
            {
                mouseDown(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:Paint"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnPaint(Sprite spr)
        {
            if (paint != null)
            {
                paint(spr);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseMove"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            if (mouseMove != null)
            {
                mouseMove(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseWheel"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event sounds.</param>
        public virtual void OnMouseWheel(MouseEventArgs e)
        {
            if (mouseWheel != null)
            {
                mouseWheel(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseLeave"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseLeave(EventArgs e)
        {
            if (mouseLeave != null)
            {
                mouseLeave(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:MouseEnter"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event sounds.</param>
        protected virtual void OnMouseEnter(EventArgs e)
        {
            if (mouseEnter != null)
            {
                mouseEnter(this, e);
            }
        }

        protected virtual void OnUpdate(float dt)
        {
            if (updating != null)
            {
                updating(this, dt);
            }
        }

        protected virtual void OnConfigParsing(ConfigurationSection sect)
        {
            if (configParsing != null)
            {
                configParsing(this, sect);
            }
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            paint = null;

            mouseMove = null;
            mouseUp = null;
            mouseDown = null;
            mouseClick = null;
            mouseWheel = null;

            mouseEnter = null;
            mouseLeave = null;
            updating = null;
            configParsing = null;
        }

    }
}
