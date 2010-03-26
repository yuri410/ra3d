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
using System.IO;

using R3D;
using R3D.Base;

namespace R3D.IO
{
    public unsafe class MemoryStream : Stream
    {
        int length;
        byte* data;
        int position;

        public MemoryStream(void* data, int len)
        {
            this.data = (byte*)data;
            length = len;

        }

        public override bool CanRead
        {
            get { return true; }
        }
        public override bool CanSeek
        {
            get { return true; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        { }

        public override long Length
        {
            get { return length; }
        }

        public override long Position
        {
            get { return position; }
            set { position = (int)value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (offset + count > length)
            {
                count = length - offset;
            }

            fixed (byte* dst = &buffer[0])
            {
                Memory.Copy(data + position, dst, count);
            }
            position += count;
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position =(int) offset;
                    break;
                case SeekOrigin.Current:
                    position += (int)offset;
                    break;
                case SeekOrigin.End:
                    position = length + (int)offset;
                    break;
            }
            if (position < 0)
                position = 0;
            if (position > length)
                position = length;
            return position;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="System.NotSupportedException"></exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset + count > length)
            {
                throw new EndOfStreamException();
            }
            fixed (byte* src = &buffer[0])
            {
                Memory.Copy(src, data + position, count);
            }
            position += count;
        }
    }

    public unsafe class MemoryLocation : ResourceLocation
    {
        void* data;
        public MemoryLocation(void* pos, int size)
            : base("Addr: " + ((int)pos).ToString(), size)
        {
            data = pos;
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Stream GetStream
        {
            get
            {
                return new MemoryStream(data, size);
            }
        }

        //public override bool AutoClose
        //{
        //    get { return true; }
        //}
    }
    
}