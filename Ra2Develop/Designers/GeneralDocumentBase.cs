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
using System.IO;
using System.Text;
using R3D.IO;

namespace Ra2Develop.Designers
{
    public class GeneralDocumentBase : DocumentBase
    {
        DocumentAbstractFactory factory;

        ResourceLocation resLoc;

        bool saved;

        protected virtual string NewFileName
        {
            get { return Program.StringTable["GUI:NewFileName"]; }
        }


        protected void Init(DocumentAbstractFactory fac, ResourceLocation rl)
        {
            factory = fac;
            resLoc = rl;

            if (resLoc != null)
            {
                this.Text = resLoc.ToString();

                if (resLoc is FileLocation)
                    this.Text = Path.GetFileName(this.Text);

                if (resLoc.IsReadOnly)
                    this.Text += Program.StringTable["GUI:READONLY"];
            }
            else
            {
                this.Text = NewFileName;
            }

            this.TabText = this.Text;

        }

        public override bool IsReadOnly
        {
            get { return resLoc == null ? false : resLoc.IsReadOnly; }
        }

        public override int GetHashCode()
        {
            if (resLoc != null && resLoc.Name != null)
            {
                return resLoc.GetHashCode();
            }
            return 0;
        }
        public override string ToString()
        {
            if (resLoc != null)
                return resLoc.ToString();
            return Text;
        }
        public override ResourceLocation ResourceLocation
        {
            get { return resLoc; }
            set { resLoc = value; }
        }
        public override bool Saved
        {
            get { return saved; }
            protected set
            {
                if (value != saved)
                {
                    saved = value;
                    if (saveChanged != null)
                    {
                        saveChanged(this);
                    }
                }
            }
        }
        public override DocumentAbstractFactory Factory
        {
            get { return factory; }
        }
    }
}
