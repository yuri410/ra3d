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
    public abstract class Ra2ReloadFile
    {
        protected bool isInArchive;
        protected int fileSize;
        protected string fileName;
        protected string filePath;

        protected Ra2ReloadFile(string file, int size, bool isinMix)
        {
            isInArchive = isinMix;
            fileName = Path.GetFileName(file);
            filePath = file;
            fileSize = size;
        }

        public bool IsInArchive
        {
            get { return isInArchive; }
        }
        public int FileSize
        {
            get { return fileSize; }
        }
        public string FileName
        {
            get { return fileName; }
        }
        public string FilePath
        {
            get { return filePath; }
        }
    }
}
