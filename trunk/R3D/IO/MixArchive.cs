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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.Crypto;

namespace R3D.IO
{
    public class MixArchive : Archive
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct MixHeader
        {
            public Int16 fileCount;
            public int size;
        }
        const int mix_checksum = 0x00010000;
        const int mix_encrypted = 0x00020000;
        const int cb_mix_key = 56;
        const int cb_mix_key_source = 80;
        const int cb_mix_checksum = 20;

        bool hasCheckSum;
        bool isEncrypted;
        //bool isInMix;

        int fileCount;
        int contentSize;

        //List<ArchiveFileEntry> index;
        Dictionary<uint, ArchiveFileEntry> index;
        //WSCodeDecrypt decrypt;
        Stream stm;

        //string filePath;

        //int fileSize;
        int baseOffset;

        //public string FilePath
        //{
        //    get { return filePath; }
        //}

        public override bool Find(string file, out ArchiveFileEntry entry)
        {
            uint id = ComputeID(file);

            //entry = new ArchiveFileEntry();
            //for (int i = 0; i < index.Count; i++)
            //    if (index[i].id == id)
            //    {
            //        entry = index[i];
            //        return true;
            //    }
            return index.TryGetValue(id, out entry);
        }


        public override Dictionary<uint, ArchiveFileEntry> Files
        {
            get { return index; }
        }
        public bool IsEncrypted
        {
            get { return isEncrypted; }
        }
        public override int FileCount
        {
            get { return fileCount; }
        }

        /// <summary>
        /// mix文件流
        /// </summary>
        public override Stream ArchiveStream
        {
            get
            {
                stm.Position = baseOffset;
                return stm;
            }
        }

        unsafe void Read(Stream src)
        {
            stm = src;

            ArchiveBinaryReader br = new ArchiveBinaryReader(src, Encoding.Default);

            long oldp = src.Position;

            fileCount = br.ReadUInt16();

            if (fileCount > 0)
            {
                contentSize = br.ReadInt32();
                int cbIndex = fileCount * sizeof(ArchiveFileEntry);
                if ((fileCount >> 12 | fileSize) != (6 + cbIndex + contentSize))
                    throw new DataFormatException(filePath);
                index = new Dictionary<uint, ArchiveFileEntry>(fileCount);
                for (int i = 0; i < fileCount; i++)
                {
                    ArchiveFileEntry entry;
                    entry.id = br.ReadUInt32();
                    entry.offset = br.ReadInt32() + 6 + cbIndex;
                    entry.size = br.ReadInt32();
                    index.Add(entry.id, entry);
                }

            }
            else
            {
                src.Position = oldp;

                int flag = br.ReadInt32();

                hasCheckSum = ((flag & mix_checksum) != 0);
                isEncrypted = ((flag & mix_encrypted) != 0);

                if (isEncrypted)
                {
                    WSCodeDecrypt decrypt = new WSCodeDecrypt(br.ReadBytes(cb_mix_key_source));

                    byte[] dec = new byte[8];
                    decrypt.Decipher(br.ReadBytes(8), dec);

                    fileCount = dec[0] | dec[1] << 8;
                    contentSize = dec[2] | dec[3] << 8 | dec[4] << 16 | dec[5] << 24;

                    index = new Dictionary<uint, ArchiveFileEntry>(fileCount);

                    int cbIndex = fileCount * sizeof(ArchiveFileEntry);
                    int cbF = cbIndex + 5 & ~7;

                    if (fileSize != 92 + cbF + contentSize + (hasCheckSum ? 20 : 0))
                        throw new DataFormatException(filePath);

                    if (fileCount > 0)
                    {
                        byte[] buffer = new byte[cbF];
                        decrypt.Decipher(br.ReadBytes(cbF), buffer);

                        byte[] buffer2 = new byte[cbF];
                        buffer2[0] = dec[6];
                        buffer2[1] = dec[7];
                        Array.Copy(buffer, 0, buffer2, 2, cbF - 2);

                        for (int i = 0; i < fileCount; i++)
                        {
                            int pos = i * sizeof(ArchiveFileEntry);

                            ArchiveFileEntry entry;
                            entry.id = (uint)(buffer2[pos] | (buffer2[pos + 1] << 8) | (buffer2[pos + 2] << 16) | (buffer2[pos + 3] << 24));
                            entry.offset = (buffer2[pos + 4] | (buffer2[pos + 5] << 8) | (buffer2[pos + 6] << 16) | (buffer2[pos + 7] << 24)) + 92 + cbF;
                            entry.size = buffer2[pos + 8] | (buffer2[pos + 9] << 8) | (buffer2[pos + 10] << 16) | (buffer2[pos + 11] << 24);
                            index.Add(entry.id, entry);
                        }
                    }
                    decrypt.Dispose();
                }
                else
                {
                    fileCount = br.ReadUInt16();
                    contentSize = br.ReadInt32();

                    index = new Dictionary<uint, ArchiveFileEntry>(fileCount);

                    int cbIndex = fileCount * sizeof(ArchiveFileEntry);
                    if (fileSize != 4 + cbIndex + sizeof(MixHeader) + contentSize + (hasCheckSum ? 20 : 0))
                        throw new DataFormatException(filePath);

                    for (int i = 0; i < fileCount; i++)
                    {
                        ArchiveFileEntry entry;
                        entry.id = br.ReadUInt32();
                        entry.offset = br.ReadInt32() + 4 + cbIndex + sizeof(MixHeader);
                        entry.size = br.ReadInt32();
                        index.Add(entry.id, entry);
                    }
                }
            }

            br.Close(false);

        }

        public MixArchive(string file)
            : this(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), file, (int)new FileInfo(file).Length,false )
        { }


        private MixArchive(Stream src, string file, int size, bool isInArc)
            : base(file, size, isInArc)
        {
            //isInMix = !closeStream;
            //fileSize = size;
            baseOffset = (int)src.Position;
            //filePath = file;

            Read(src);
        }

        public MixArchive(FileLocation file)
            : this(file.GetStream, file.Path, file.Size, file.IsInArchive)
        { }

        public override void Dispose()
        {
            stm.Close();

            stm = null;
        }
    }
}
