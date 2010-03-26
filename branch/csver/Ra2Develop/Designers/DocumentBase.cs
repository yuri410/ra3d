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
using System.Text;
using System.Windows.Forms;
using R3D.IO;
using WeifenLuo.WinFormsUI.Docking;
using System.ComponentModel;

namespace Ra2Develop.Designers
{
    public delegate void PropertyUpdateHandler(object sender, object[] allObjects);
    public delegate void SaveStateChangedHandler(object sender);

    public class DocumentBase : DockContent, IDocument
    {
        protected PropertyUpdateHandler propertyUpdated;
        protected SaveStateChangedHandler saveChanged;

        bool activated;

        /// <summary>
        /// 更新属性窗格
        /// </summary>
        [Browsable(false)]
        [ReadOnly(true)]
        public event PropertyUpdateHandler PropertyUpdate
        {
            add { propertyUpdated += value; }
            remove { propertyUpdated -= value; }
        }
        [Browsable(false)]
        [ReadOnly(true)]
        public event SaveStateChangedHandler SavedStateChanged
        {
            add { saveChanged += value; }
            remove { saveChanged -= value; }
        }

        public virtual Icon GetIcon()
        {
            return Program.DefaultIcon;
        }
        
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual DocumentAbstractFactory Factory
        {
            get { throw new NotImplementedException(); }
        }
        public virtual bool LoadRes()
        {
            throw new NotImplementedException();
        }
        public virtual bool SaveRes()
        {
            throw new NotImplementedException();
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual ToolStrip[] ToolStrips
        {
            get { throw new NotImplementedException(); }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual ResourceLocation ResourceLocation
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        public virtual bool Saved
        {
            get { throw new NotImplementedException(); }
            protected set
            {
                throw new NotImplementedException();
            }
        }

        protected virtual void active() { }
        protected virtual void deactive() { }

        void IDocument.DocActivate()
        {
            if (!activated)
            {
                active();
                activated = true;
            }
        }

        void IDocument.DocDeactivate()
        {
            if (activated)
            {
                deactive();
                activated = false;
            }
        }
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [ReadOnly(true)]
        bool IDocument.IsActivated
        {
            get { return activated; }
        }
    }
}
