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

namespace Ra2Develop.Designers
{
    public abstract class DocumentConfigBase
    {
        static List<DocumentConfigBase> existing;
        public static void SaveAll()
        {
            Environment.CurrentDirectory = Application.StartupPath;
            if (existing != null)
            {
                for (int i = 0; i < existing.Count; i++)
                    existing[i].Save();
            }
        }

        //protected bool isSaved;

        //public void Save()
        //{
        //    //if (!isSaved)
        //    //{
        //        //SaveImpl();
        //        //isSaved = true;
        //    //}
        //}

        protected abstract void Save();

        protected DocumentConfigBase()
        {
            if (existing == null)
                existing = new List<DocumentConfigBase>();
            existing.Add(this);
        }
    }
}

