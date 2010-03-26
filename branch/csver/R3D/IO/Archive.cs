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
using System.IO;
namespace R3D.IO
{
    public abstract class Archive : Ra2ReloadFile
    {
        protected Archive(string file, int size, bool isinArchive)
            : base(file, size, isinArchive)
        { }

        public abstract Dictionary<uint, ArchiveFileEntry> Files
        {
            get;
        }
        public abstract int FileCount
        {
            get;
        }
        public abstract bool Find(string file, out ArchiveFileEntry entry);

        public static uint ComputeID(string name)
        {
            name = name.ToUpper();

            int l = name.Length;
            int a = l >> 2;

            if ((l & 3) != 0)
            {
                name += (char)(l - (a << 2));
                int i = 3 - (l & 3);
                while (i-- != 0)
                    name += name[a << 2];
            }

            return Crypto.Crc.ComputeCrc(name.ToCharArray(), name.Length);
        }
        public abstract Stream ArchiveStream
        {
            get;
        }

        public abstract void Dispose();
    }
    public interface IArchiveFactory : IAbstractFactory<Archive, FileLocation>
    { }
}
