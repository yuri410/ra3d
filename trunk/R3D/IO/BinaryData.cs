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
    // key - value 组成的二进制数据，用于版本兼容并代替DataSrcBase
    public unsafe class BinaryDataReader
    {
        struct Entry
        {
            public string name;
            public int offset;
            public int size;

            public Entry(string name, int offset, int size)
            {
                this.name = name;
                this.offset = offset;
                this.size = size;
            }
        }

        int sectCount;
        Dictionary<string, Entry> positions;
        Stream stream;

        byte[] buffer;

        public BinaryDataReader(Stream stm)
        {
            stream = stm;
            buffer = new byte[sizeof(decimal)];

            ArchiveBinaryReader br = new ArchiveBinaryReader(stm, Encoding.Default);

            // 读出所有块
            sectCount = br.ReadInt32();
            positions = new Dictionary<string, Entry>(sectCount);

            for (int i = 0; i < sectCount; i++)
            {
                string name = br.ReadStringUnicode();
                int size = br.ReadInt32();

                positions.Add(name, new Entry(name, (int)br.BaseStream.Position, size));

                br.BaseStream.Position += size;
            }
            br.Close();
        }

        public ArchiveBinaryReader GetData(string name)
        {
            Entry ent = positions[name];
            return new ArchiveBinaryReader(new VirtualStream(stream, ent.offset, ent.size));
        }
        public Stream GetDataStream(string name)
        {
            Entry ent = positions[name];
            return new VirtualStream(stream, ent.offset, ent.size);
        }
        public int GetDataInt32(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(int));

            return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);

        }
        public uint GetDataUInt32(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(uint));

            return (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
        }

        public short GetDataInt16(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(short));

            return (short)(buffer[0] | (buffer[1] << 8));
        }
        public ushort GetDataUInt16(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(ushort));

            return (ushort)(buffer[0] | (buffer[1] << 8));
        }

        public long GetDataInt64(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(long));

            uint num = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
            uint num2 = (uint)(buffer[4] | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24));
            return (long)((num2 << 32) | num);
        }
        public ulong GetDataUInt64(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(ulong));

            uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
            uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
            return ((num2 << 32) | num);
        }

        public bool GetDataBool(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(bool));

            return (buffer[0] != 0);
        }

        public float GetDataSingle(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(float));

            uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
            return *(((float*)&num));
        }
        public double GetDataDouble(string name)
        {
            Entry ent = positions[name];

            stream.Position = ent.offset;
            stream.Read(buffer, 0, sizeof(double));

            uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
            uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
            ulong num3 = (num2 << 32) | num;
            return *(((double*)&num3));

        }
        



        public int GetDataInt32(string name, int def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {//= positions[name];

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(int));

                return buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24);
            }
            return def;
        }
        public uint GetDataUInt32(string name,uint def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {
                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(uint));

                return (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
            }
            return def;
        }

        public short GetDataInt16(string name, short def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {
                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(short));

                return (short)(buffer[0] | (buffer[1] << 8));
            }
            return def;
        }
        public ushort GetDataUInt16(string name, ushort def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {

                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(ushort));

                return (ushort)(buffer[0] | (buffer[1] << 8));
            }
            return def;
        }

        public long GetDataInt64(string name, long def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {
                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(long));

                uint num = (uint)(buffer[0] | (buffer[1] << 8) | (buffer[2] << 16) | (buffer[3] << 24));
                uint num2 = (uint)(buffer[4] | (buffer[5] << 8) | (buffer[6] << 16) | (buffer[7] << 24));
                return (long)((num2 << 32) | num);
            }
            return def;
        }
        public ulong GetDataUInt64(string name, ulong def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {
                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(ulong));

                uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
                return ((num2 << 32) | num);
            }
            return def;
        }

        public bool GetDataBool(string name,bool def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {
                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(bool));

                return (buffer[0] != 0);
            }
            return def;
        }

        public float GetDataSingle(string name,float def)
        {
             Entry ent;
             if (positions.TryGetValue(name, out ent))
             {
                 stream.Position = ent.offset;
                 stream.Read(buffer, 0, sizeof(float));

                 uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                 return *(((float*)&num));
             }
             return def;
        }
        public double GetDataDouble(string name, double def)
        {
            Entry ent;
            if (positions.TryGetValue(name, out ent))
            {
                stream.Position = ent.offset;
                stream.Read(buffer, 0, sizeof(double));

                uint num = (uint)(((buffer[0] | (buffer[1] << 8)) | (buffer[2] << 16)) | (buffer[3] << 24));
                uint num2 = (uint)(((buffer[4] | (buffer[5] << 8)) | (buffer[6] << 16)) | (buffer[7] << 24));
                ulong num3 = (num2 << 32) | num;
                return *(((double*)&num3));
            }
            return def;
        }



        public void Close()
        {
            stream.Close();
        }


        public int GetChunkOffset(string name)
        {
            Entry ent = positions[name];
            return ent.offset;
        }
        public Stream BaseStream
        {
            get { return stream; }
        }
    }
    public unsafe class BinaryDataWriter : IDisposable
    {
        class Entry
        {
            public string name;
            public System.IO.MemoryStream buffer;

            public Entry(string name)
            {
                this.name = name;
                buffer = new System.IO.MemoryStream();
            }

        }

        bool disposed;
        Dictionary<string, Entry> positions = new Dictionary<string, Entry>();
        byte[] buffer = new byte[sizeof(decimal)];

        public ArchiveBinaryWriter AddEntry(string name)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);
            return new ArchiveBinaryWriter(new VirtualStream(ent.buffer, 0));
        }
        public Stream AddEntryS(string name)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);
            return new VirtualStream(ent.buffer, 0);
        }

        //public int GetSize()
        //{
        //    int size = 0;
        //    Dictionary<string, Entry>.ValueCollection vals = positions.Values;
        //    foreach (Entry e in vals)
        //    {
        //        size += (int)e.buffer.Length;
        //    }
        //    return size;
        //}

        public void AddEntry(string name, int value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);

            ent.buffer.Write(buffer, 0, sizeof(int));
        }
        public void AddEntry(string name, uint value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);

            ent.buffer.Write(buffer, 0, sizeof(uint));
        }
        public void AddEntry(string name, short value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);

            ent.buffer.Write(buffer, 0, sizeof(short));
        }
        public void AddEntry(string name, ushort value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);

            ent.buffer.Write(buffer, 0, sizeof(ushort));
        }
        public void AddEntry(string name, long value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            buffer[7] = (byte)(value >> 56);

            ent.buffer.Write(buffer, 0, sizeof(long));
        }
        public void AddEntry(string name, ulong value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            buffer[7] = (byte)(value >> 56);

            ent.buffer.Write(buffer, 0, sizeof(ulong));
        }
        public void AddEntry(string name, float value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            uint num = *((uint*)&value);
            buffer[0] = (byte)num;
            buffer[1] = (byte)(num >> 8);
            buffer[2] = (byte)(num >> 16);
            buffer[3] = (byte)(num >> 24);

            ent.buffer.Write(buffer, 0, sizeof(float));
        }
        public void AddEntry(string name, double value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            ulong num = *((ulong*)&value);
            buffer[0] = (byte)num;
            buffer[1] = (byte)(num >> 8);
            buffer[2] = (byte)(num >> 16);
            buffer[3] = (byte)(num >> 24);
            buffer[4] = (byte)(num >> 32);
            buffer[5] = (byte)(num >> 40);
            buffer[6] = (byte)(num >> 48);
            buffer[7] = (byte)(num >> 56);

            ent.buffer.Write(buffer, 0, sizeof(double));
        }
        public void AddEntry(string name, bool value)
        {
            Entry ent = new Entry(name);
            positions.Add(name, ent);

            buffer[0] = value ? ((byte)1) : ((byte)0);

            ent.buffer.Write(buffer, 0, sizeof(bool));
        }

        public ArchiveBinaryWriter GetData(string name)
        {
            Entry ent = positions[name];
            return new ArchiveBinaryWriter(new VirtualStream(ent.buffer, 0));
        }

        public void SetData(string name, int value)
        {
            Entry ent = positions[name];

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(int));
        }
        public void SetData(string name, uint value)
        {
            Entry ent = positions[name];

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(uint));
        }
        public void SetData(string name, short value)
        {
            Entry ent = positions[name];

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(short));
        }
        public void SetData(string name, ushort value)
        {
            Entry ent = positions[name];

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(ushort));
        }
        public void SetData(string name, long value)
        {
            Entry ent = positions[name];

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            buffer[7] = (byte)(value >> 56);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(long));
        }
        public void SetData(string name, ulong value)
        {
            Entry ent = positions[name];

            buffer[0] = (byte)value;
            buffer[1] = (byte)(value >> 8);
            buffer[2] = (byte)(value >> 16);
            buffer[3] = (byte)(value >> 24);
            buffer[4] = (byte)(value >> 32);
            buffer[5] = (byte)(value >> 40);
            buffer[6] = (byte)(value >> 48);
            buffer[7] = (byte)(value >> 56);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(ulong));
        }
        public void SetData(string name, float value)
        {
            Entry ent = positions[name];

            uint num = *((uint*)&value);
            buffer[0] = (byte)num;
            buffer[1] = (byte)(num >> 8);
            buffer[2] = (byte)(num >> 16);
            buffer[3] = (byte)(num >> 24);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(float));
        }
        public void SetData(string name, double value)
        {
            Entry ent = positions[name];

            ulong num = *((ulong*)&value);
            buffer[0] = (byte)num;
            buffer[1] = (byte)(num >> 8);
            buffer[2] = (byte)(num >> 16);
            buffer[3] = (byte)(num >> 24);
            buffer[4] = (byte)(num >> 32);
            buffer[5] = (byte)(num >> 40);
            buffer[6] = (byte)(num >> 48);
            buffer[7] = (byte)(num >> 56);


            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(double));
        }
        public void SetData(string name, bool value)
        {
            Entry ent = positions[name];

            buffer[0] = value ? ((byte)1) : ((byte)0);

            ent.buffer.Position = 0;
            ent.buffer.Write(buffer, 0, sizeof(bool));
        }


        public void Save(Stream stm)
        {
            ArchiveBinaryWriter bw = new ArchiveBinaryWriter(stm, Encoding.Default);

            bw.Write(positions.Count);

            foreach (KeyValuePair<string, Entry> e in positions)
            {
                bw.WriteStringUnicode(e.Key);
                bw.Write((int)e.Value.buffer.Length);
                bw.Flush();
                e.Value.buffer.WriteTo(stm);
            }
            bw.Close();
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                foreach (KeyValuePair<string, Entry> e in positions)
                {
                    e.Value.buffer.Dispose();
                }
                positions.Clear();
                disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(this.ToString());
            }
        }

        #endregion

        ~BinaryDataWriter()
        {
            if (!disposed)
                Dispose();
        }
    }
}
