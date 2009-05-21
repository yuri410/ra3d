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
    public class ArchiveStreamReader : StreamReader
    {
        //bool closeStream = true;

        //public override void Close()
        //{
        //    base.Close();
        //    //base.Dispose(closeStream);
        //}


        public ArchiveStreamReader(ResourceLocation rl)
            : this(rl.GetStream, Encoding.Default)
        {
            //closeStream = rl.AutoClose;
        }

        public ArchiveStreamReader(Stream stream)
            : base(stream, true)
        { }

        public ArchiveStreamReader(string path)
            : base(path, true)
        { }

        public ArchiveStreamReader(Stream stream, bool detectEncodingFromByteOrderMarks)
            : base(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks, 1024)
        { }

        public ArchiveStreamReader(Stream stream, Encoding encoding)
            : base(stream, encoding, true, 1024)
        { }

        public ArchiveStreamReader(string path, bool detectEncodingFromByteOrderMarks)
            : base(path, Encoding.UTF8, detectEncodingFromByteOrderMarks, 1024)
        { }

        public ArchiveStreamReader(string path, Encoding encoding)
            : base(path, encoding, true, 1024)
        { }

        public ArchiveStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks)
            : base(stream, encoding, detectEncodingFromByteOrderMarks, 1024)
        { }

        public ArchiveStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks)
            : base(path, encoding, detectEncodingFromByteOrderMarks, 1024)
        { }

        public ArchiveStreamReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
            : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        { }

        public ArchiveStreamReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize)
            : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        { }
    }

    public class ArchiveStreamWriter : StreamWriter
    {
        //bool closeStream = true;

        //public override void Close()
        //{
        //    base.Dispose(closeStream);
        //}

        //public bool AutoCloseStream
        //{
        //    get { return closeStream; }
        //    set { closeStream = value; }
        //}

        public ArchiveStreamWriter(ResourceLocation rl)
            : this(rl.GetStream, Encoding.Default)
        {
            //closeStream = rl.AutoClose;
        }

        public ArchiveStreamWriter(Stream stream)
            : base(stream)
        {
        }

        public ArchiveStreamWriter(string path)
            : base(path)
        {
        }

        public ArchiveStreamWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
        }

        public ArchiveStreamWriter(string path, bool append)
            : base(path, append)
        {
        }

        public ArchiveStreamWriter(Stream stream, Encoding encoding, int bufferSize)
            : base(stream, encoding, bufferSize)
        {
        }

        public ArchiveStreamWriter(string path, bool append, Encoding encoding)
            : base(path, append, encoding)
        {
        }


        public ArchiveStreamWriter(string path, bool append, Encoding encoding, int bufferSize)
            : base(path, append, encoding, bufferSize)
        {
        }


    }
}
