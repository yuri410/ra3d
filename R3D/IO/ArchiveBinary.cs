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
using System.Text;
using R3D.GraphicsEngine;
using R3D.IsoMap;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.IO
{
    public class ArchiveBinaryReader : BinaryReader
    {
        //bool closeStream = true;

        public ArchiveBinaryReader(ResourceLocation fl)
            : this(fl.GetStream, Encoding.Default)
        { }

        public ArchiveBinaryReader(Stream src)
            : base(src)
        { }

        public ArchiveBinaryReader(Stream src, Encoding enc)
            : base(src, enc)
        { }

        //public bool AutoCloseStream
        //{
        //    get { return closeStream; }
        //    set { closeStream = value; }
        //}

        //public override void Close()
        //{
        //    base.Close();
        //}

        public void ReadMatrix(out Matrix mat)
        {
            mat.M11 = ReadSingle();
            mat.M12 = ReadSingle();
            mat.M13 = ReadSingle();
            mat.M14 = ReadSingle();
            mat.M21 = ReadSingle();
            mat.M22 = ReadSingle();
            mat.M23 = ReadSingle();
            mat.M24 = ReadSingle();
            mat.M31 = ReadSingle();
            mat.M32 = ReadSingle();
            mat.M33 = ReadSingle();
            mat.M34 = ReadSingle();
            mat.M41 = ReadSingle();
            mat.M42 = ReadSingle();
            mat.M43 = ReadSingle();
            mat.M44 = ReadSingle();
        }

        public string ReadStringUnicode()
        {
            int len = ReadInt32();
            char[] chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                chars[i] = (char)ReadUInt16();
            }
            //char[] chars = ReadChars(len);
            return new string(chars);
        }

        public Matrix ReadMatrix()
        {
            Matrix mat;
            mat.M11 = ReadSingle();
            mat.M12 = ReadSingle();
            mat.M13 = ReadSingle();
            mat.M14 = ReadSingle();
            mat.M21 = ReadSingle();
            mat.M22 = ReadSingle();
            mat.M23 = ReadSingle();
            mat.M24 = ReadSingle();
            mat.M31 = ReadSingle();
            mat.M32 = ReadSingle();
            mat.M33 = ReadSingle();
            mat.M34 = ReadSingle();
            mat.M41 = ReadSingle();
            mat.M42 = ReadSingle();
            mat.M43 = ReadSingle();
            mat.M44 = ReadSingle();
            return mat;
        }

        public void ReadMaterial(out Material mat)
        {
            mat.Ambient.Alpha = ReadSingle();
            mat.Ambient.Red = ReadSingle();
            mat.Ambient.Green = ReadSingle();
            mat.Ambient.Blue = ReadSingle();
            mat.Diffuse.Alpha = ReadSingle();
            mat.Diffuse.Red = ReadSingle();
            mat.Diffuse.Green = ReadSingle();
            mat.Diffuse.Blue = ReadSingle();
            mat.Specular.Alpha = ReadSingle();
            mat.Specular.Red = ReadSingle();
            mat.Specular.Green = ReadSingle();
            mat.Specular.Blue = ReadSingle();
            mat.Emissive.Alpha = ReadSingle();
            mat.Emissive.Red = ReadSingle();
            mat.Emissive.Green = ReadSingle();
            mat.Emissive.Blue = ReadSingle();
            mat.Power = ReadSingle();            
        }

        //public MeshMaterial.Data ReadMaterial()
        //{
        //    MeshMaterial.Data mat = new MeshMaterial.Data();

        //    if (ReadInt32() == (int)FileID.Material)
        //    {
        //        mat.Flags = (MaterialFlags)ReadInt32();
        //        ReadMaterial(out mat.material);

        //        int len = ReadInt32();
        //        if (len > 0)
        //        {
        //            char[] chars = ReadChars(len);

        //            mat.TextureName = new string(chars);
        //        }
        //        return mat;
        //    }
        //    else
        //    {
        //        throw new InvalidDataException();
        //    }
        //}
        //public MeshMaterial.DataTextureEmbeded ReadMaterialEx()
        //{
        //    MeshMaterial.DataTextureEmbeded mat = new MeshMaterial.DataTextureEmbeded();

        //    if (ReadInt32() == (int)FileID.Material)
        //    {
        //        mat.Flags = (MaterialFlags)ReadInt32();
        //        ReadMaterial(out mat.material);

        //        bool hasTexture = ReadBoolean();
        //        if (hasTexture)
        //        {
        //            mat.Texture = LoadImage();
        //        }
        //        return mat;
        //    }
        //    else
        //    {
        //        throw new InvalidDataException();
        //    }
        //}
        //public BlockMaterial ReadBlockMaterial()
        //{
        //    BlockMaterial mat = new BlockMaterial();

        //    if (ReadInt32() == (int)FileID.Material)
        //    {
        //        mat.Flags = (MaterialFlags)ReadInt32();
        //        ReadMaterial(out mat.material);

        //        bool hasTexture = ReadBoolean();
        //        if (hasTexture)
        //        {
        //            mat.Texture = LoadImage();
        //        }
        //        return mat;
        //    }
        //    else
        //    {
        //        throw new InvalidDataException();
        //    }
        //}
        public unsafe ImageBase ReadEmbededImage()
        {
            int imgWidth = ReadUInt16();
            int imgHeight = ReadUInt16();

            int bytesPerPixel = ReadByte();

            RawImage image = new RawImage(imgWidth, imgHeight, bytesPerPixel == 4 ? ImagePixelFormat.A8R8G8B8 : ImagePixelFormat.R8G8B8);

            byte[] buffer = ReadBytes(imgHeight * imgWidth * bytesPerPixel);

            fixed (byte* src = &buffer[0])
            {
                Helper.MemCopy(image.GetData(), src, buffer.Length);
            }

            return image;
        }

        public BinaryDataReader ReadBinaryData()
        {
            int size = ReadInt32();
            
            VirtualStream vs = new VirtualStream(BaseStream, BaseStream.Position, size);
            return new BinaryDataReader(vs);
        }

        public void Close(bool closeBaseStream)
        {
            base.Dispose(closeBaseStream);
        }
    }

    public class ArchiveBinaryWriter : BinaryWriter
    {
        //bool closeStream = true;

        public ArchiveBinaryWriter(ResourceLocation rl) :
            this(rl.GetStream, Encoding.Default)
        { }
        public ArchiveBinaryWriter(Stream output) : base(output) { }

        public ArchiveBinaryWriter(Stream output, Encoding encoding) : base(output, encoding) { }

        //public bool AutoCloseStream
        //{
        //    get { return closeStream; }
        //    set { closeStream = value; }
        //}
        //public override void Close()
        //{
        //    base.Close();
        //}

        public void WriteStringUnicode(string str)
        {
            if (str == null)
                str = string.Empty;
            int len = str.Length;
            Write(len);

            for (int i = 0; i < len; i++)
            {
                Write((ushort)str[i]);
            }
        }

        public void Write(ref Matrix mat)
        {
            Write(mat.M11);
            Write(mat.M12);
            Write(mat.M13);
            Write(mat.M14);
            Write(mat.M21);
            Write(mat.M22);
            Write(mat.M23);
            Write(mat.M24);
            Write(mat.M31);
            Write(mat.M32);
            Write(mat.M33);
            Write(mat.M34);
            Write(mat.M41);
            Write(mat.M42);
            Write(mat.M43);
            Write(mat.M44);
        }
        public void Write(Matrix mat)
        {
            Write(mat.M11);
            Write(mat.M12);
            Write(mat.M13);
            Write(mat.M14);
            Write(mat.M21);
            Write(mat.M22);
            Write(mat.M23);
            Write(mat.M24);
            Write(mat.M31);
            Write(mat.M32);
            Write(mat.M33);
            Write(mat.M34);
            Write(mat.M41);
            Write(mat.M42);
            Write(mat.M43);
            Write(mat.M44);
        }

        public void Write(ref Material mat)
        {
            Write(mat.Ambient.Alpha);
            Write(mat.Ambient.Red);
            Write(mat.Ambient.Green);
            Write(mat.Ambient.Blue);

            Write(mat.Diffuse.Alpha);
            Write(mat.Diffuse.Red);
            Write(mat.Diffuse.Green);
            Write(mat.Diffuse.Blue);

            Write(mat.Specular.Alpha);
            Write(mat.Specular.Red);
            Write(mat.Specular.Green);
            Write(mat.Specular.Blue);

            Write(mat.Emissive.Alpha);
            Write(mat.Emissive.Red);
            Write(mat.Emissive.Green);
            Write(mat.Emissive.Blue);

            Write(mat.Power);
        }

        //public void Write(MeshMaterial.Data mat)
        //{
        //    Write((int)FileID.Material);
        //    Write((int)mat.Flags);
        //    Write(ref mat.material);
        //    if (string.IsNullOrEmpty(mat.TextureName))
        //    {
        //        Write((int)0);
        //    }
        //    else
        //    {
        //        string name = mat.TextureName;
        //        Write(name.Length);
        //        for (int i = 0; i < name.Length; i++)
        //        {
        //            Write(name[i]);
        //        }
        //    }
        //}
        //public void Write(MeshMaterial.DataTextureEmbeded mat)
        //{
        //    Write((int)FileID.Material);
        //    Write((int)mat.Flags);
        //    Write(ref mat.material);
        //    if (mat.Texture == null)
        //    {
        //        Write(false);
        //    }
        //    else
        //    {
        //        Write(true);
        //        SaveImage(mat.Texture);
        //    }
        //}

        public unsafe void Write(ImageBase image)
        {
            Write((UInt16)image.Width);
            Write((UInt16)image.Height);

            Write((byte)image.BytesPerPixel);

            //bw.Write(image.InternalData);

            byte* src = (byte*)image.GetData();
            for (int i = 0; i < image.ContentSize; i++)
            {
                Write(src[i]);
            }
        }
        public void Write(BinaryDataWriter data)
        {
            Write(0); //占个位置
            Flush();

            long start = BaseStream.Position;

            data.Save(new VirtualStream(BaseStream, BaseStream.Position));
            data.Dispose();

            long end = BaseStream.Position;
            int size = (int)(end - start);

            BaseStream.Position = start - 4;
            Write(size);
            BaseStream.Position = end;
        }
    }

    public class VirtualStream : Stream
    {
        Stream stream;

        long length;
        long baseOffset;

        bool isOutput;

        public VirtualStream(Stream stream)
        {
            this.stream = stream;
            this.length = stream.Length;
            this.baseOffset = 0;
            stream.Position = 0;
        }
        public VirtualStream(Stream stream, long baseOffset)
        {
            isOutput = true;
            this.stream = stream;
            this.baseOffset = 0;
            stream.Position = baseOffset;
        }
        public VirtualStream(Stream stream, long baseOffset, long length)
        {
            this.stream = stream;
            this.length = length;
            this.baseOffset = baseOffset;
            stream.Position = baseOffset;
        }


        public bool IsOutput
        {
            get { return isOutput; }
        }
        public long BaseOffset
        {
            get { return baseOffset; }
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
        {
            stream.Flush();
        }

        public override long Length
        {
            get { return isOutput ? stream.Length : length; }
        }

        public long AbsolutePosition
        {
            get { return stream.Position; }
        }
        public override long Position
        {
            get
            {
                return stream.Position - baseOffset;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                if (value > Length)
                    throw new EndOfStreamException();
                stream.Position = value + baseOffset;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position + count > length)
            {
                count = (int)(length - Position);
            }
            if (count > 0)
            {
                return stream.Read(buffer, offset, count);
            }
            return 0;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    if (offset > length)
                    {
                        offset = length;
                    }
                    if (offset < 0)
                    {
                        offset = 0;
                    }
                    break;
                case SeekOrigin.Current:
                    if (stream.Position + offset > baseOffset + length)
                    {
                        offset = baseOffset + length - stream.Position;
                    }
                    if (stream.Position + offset < baseOffset)
                    {
                        offset = baseOffset - stream.Position;
                    }
                    break;
                case SeekOrigin.End:
                    if (offset > 0)
                    {
                        offset = 0;
                    }
                    if (offset < -length)
                    {
                        offset = -length;
                    }
                    break;
            }
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
            if (isOutput)
                length += count;
        }
        public override void WriteByte(byte value)
        {
            stream.WriteByte(value);
            if (isOutput)
                length++;
        }

        public override void Close() { }
    }
}
