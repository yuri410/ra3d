﻿/*
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using R3D.IO;
using WeifenLuo.WinFormsUI.Docking;

namespace Ra2Develop.Designers
{
    
    public interface IDocument
    {
        //int GetHashCode();
        //Icon GetIcon();
        //string ToString();
        void DocActivate();
        void DocDeactivate();
        bool IsActivated { get; }
    }
    public abstract class DocumentAbstractFactory
    {
        public abstract DocumentBase CreateInstance(ResourceLocation res);

        public abstract Type CreationType { get; }

        public abstract string Description { get; }

        public virtual string Filter
        {
            get { return DevUtils.GetFilter(Description, Filters); }
        }

        public abstract string[] Filters { get; }
    }
}
