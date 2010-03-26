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
using R3D.Base;

namespace R3D.IO
{
    public abstract class ResourceLocation
    {
        string name;
        protected int size;


        public ResourceLocation(string name, int s)
        {
            this.name = name;
            size = s;
        }

        public string Name
        {
            get { return name; }
        }

        public abstract bool IsReadOnly { get; }
        public abstract Stream GetStream { get; }

        public int Size
        {
            get { return size; }
        }

        public override string ToString()
        {
            return name;
        }
        public override int GetHashCode()
        {
            return Resource.GetHashCode(name);
        }
    }

    /// <summary>
    /// 记录一个文件的位置信息，FileSystem查询结果。
    /// </summary>
    public class FileLocation : ResourceLocation
    {
        Archive parent;
        string path;

        uint id;
        int offset;


        public FileLocation(FileLocation fl)
            : base(fl.Name, fl.size)
        {
            this.parent = fl.parent;
            this.path = fl.path;
            this.id = fl.id;
            this.offset = fl.offset;
        }

        public FileLocation(string filePath)
            : this(filePath, (int)new FileInfo(filePath).Length)
        { }

        protected FileLocation(string filePath, int size)
            : base(filePath, size)
        {
            path = filePath;

            // 必须是绝对路径
            if (!System.IO.Path.IsPathRooted(filePath))
                throw new ArgumentException();
        }

        public FileLocation(Archive pack, string filePath, ArchiveFileEntry fileInfo)
            : base(filePath, fileInfo.size)
        {
            parent = pack;
            path = filePath;
            id = fileInfo.id;
            offset = fileInfo.offset;
            size = fileInfo.size;
        }


        /// <summary>
        /// 获得文件流。对于文件包中的文件，以VirtualStream提供
        /// </summary>
        /// <remarks>mix中的用mix的流</remarks>
        public override Stream GetStream
        {
            get
            {
                if (parent != null)
                {
                    Stream s = parent.ArchiveStream;
                    s.Position += offset;
                    //s.SetLength((long)size);
                    return new VirtualStream(s, s.Position, size);
                }
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
        }
        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// 局部offset
        /// </summary>
        public int Offset
        {
            get { return offset; }
        }
        public bool IsInArchive
        {
            get { return parent != null; }
        }
        /// <summary>
        /// 文件完整路径（包括资源包）。
        /// </summary>
        public string Path
        {
            get { return path; }
        }
    }

    public class StreamedLocation : ResourceLocation
    {
        Stream stream;

        public StreamedLocation(Stream stm)
            : base("Stream: " + stm.ToString(), (int)stm.Length)
        {
            stream = stm;
        }

        public override bool IsReadOnly
        {
            get { return !stream.CanWrite; }
        }

        public override Stream GetStream
        {
            get { return stream; }
        }

    }
}
