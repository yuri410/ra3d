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
using R3D.IO;
using SlimDX.Direct3D9;
namespace R3D.IsoMap
{
    public interface ITheaterFactory : IAbstractFactory<TheaterBase, FileLocation>
    {
        string Description { get; }
    }

    public class TheaterFactory : ITheaterFactory
    {

        #region IAbstractFactory<TheaterBase> 成员

        public TheaterBase CreateInstance(string name)
        {
            return new Theater( name);
        }

        public TheaterBase CreateInstance(FileLocation fl)
        {
            return Theater.FromFile(fl);
        }

        public string Description
        {
            get { return "Ra2 Theater(Old)"; }
        }
        public string Type
        {
            get { return "RA2THEATER"; }
        }
        #endregion
    }
    public class Theater3DFactory : ITheaterFactory
    {
        Device dev;
        public Theater3DFactory(Device dev)
        {
            this.dev = dev;
        }

        #region IAbstractFactory<TheaterBase,FileLocation> 成员

        public TheaterBase CreateInstance(string file)
        {
            return new Theater3D(dev, file);
        }

        public TheaterBase CreateInstance(FileLocation fl)
        {
            return Theater3D.FromFile(dev, fl);
        }

        public string Description
        {
            get { return "R3D 3D Theater"; }
        }
        public string Type
        {
            get { return "3DTHEATER"; }
        }
        #endregion

    }
}
