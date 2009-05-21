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

namespace Ra2Develop.Designers
{
    //public delegate void CompilerOutputDelegate(string outputLine);
    public delegate void SourceCodeChangedHandler(string newSource, string oldSource);

    public class IniCode
    {
        string source = string.Empty;

        private bool sourceChanged = false;


        public event SourceCodeChangedHandler ScriptSourceChanged;


        public bool SourceChanged
        {
            get { return sourceChanged; }
            set { sourceChanged = value; }
        }



        public string Source
        {
            get { return source; }
            set
            {
                sourceChanged = source != value;

                if (sourceChanged)
                {
                    if (ScriptSourceChanged != null)
                    {
                        ScriptSourceChanged(value, source);
                    }
                }

                source = value;
            }
        }
    }

}
