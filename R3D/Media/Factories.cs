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

namespace R3D.Media
{
    public interface IAnimFactory : IAbstractFactory<AnimBase, ResourceLocation>
    {
        string[] Filters { get; }
    }

    public interface IImageFactory : IAbstractFactory<ImageBase, ResourceLocation>
    {
        //ImageBase CreateInstance(string file);
        //ImageBase CreateInstance(FileLocation fl);

        //string Type { get; }

        ImageBase CreateInstanceUnmanaged(ResourceLocation fl);

        string[] Filters { get; }
    }


}
