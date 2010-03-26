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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.Base;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.MathLib;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.IsoMap
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TmpHeader
    {
        public int blockCountX;
        public int blockCountY;

        public int width;
        public int height;
    }

    [Flags()]
    public enum TmpImageBits : uint
    {
        HasExtraData = 1 << 0,
        HasZData = 1 << 1,
        HasDamagedData = 1 << 2
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct TmpImageHeader
    {
        public int x;
        public int y;

        public int extra_ofs;
        public int z_ofs;
        public int extra_z_ofs;
        public int x_extra;
        public int y_extra;
        public int cx_extra;
        public int cy_extra;

        public TmpImageBits bits;
        //uint has_extra_data: 1;
        //uint has_z_data: 1;
        //uint has_damaged_data: 1;
        public sbyte height;
        public TerrainType terrain_type;
        public sbyte ramp_type;
        public byte radar_red_left;
        public byte radar_green_left;
        public byte radar_blue_left;
        public byte radar_red_right;
        public byte radar_green_right;
        public byte radar_blue_right;
        public fixed sbyte pad[3];
    }

    public enum TerrainType : byte
    {
        Clear = 0,
        Ice = 0x01,
        Tunnel = 0x05,
        Railroad = 0x06,
        Rock = 0x07,
        Water = 0x09,
        Beach = 0x0A,
        Road = 0x0B,
        Rough = 0x0E,

        BirdgeSE = byte.MaxValue
    }


    public class Tile : TileBase
    {
        TmpHeader header;
        int[] tileOffsets;
        TmpImageHeader[] blocks;

        bool[] isVaild;

        int[] remapOffset;

        static Matrix transMat;

        static Point[] castTable;
        static Point[] sampledPixels;

        const int SrcWidth = 60;
        const int SrcHeight = 30;

        const int HSWidth = SrcWidth / 2;
        const int HSHeight = SrcHeight / 2;


        const int ConWidth = 28;
        const int ConHeight = 30;

       

        unsafe static Tile()
        {
            transMat = Matrix.Translation(-HSWidth, -HSHeight, 0) * Matrix.RotationZ(-(float)Math.PI * 64f / 180f) * Matrix.Translation(HSWidth, HSHeight, 0);

            Matrix t2 = Matrix.Identity;

            float sin = (float)Math.Sin(Math.PI * 50.0 / 180.0);
            t2.M12 = sin;
            t2.M42 = -sin * SrcWidth * (float)Math.Cos(Math.PI * 64f / 180f);

            transMat *= t2;
            transMat *= Matrix.Translation(-SrcHeight * (float)Math.Cos(Math.PI * 64f / 180f) - 1, -1, 0);

            int[] data = new int[900];
            for (int i = 0; i < 900; i++)
            {
                data[i] = i;
            }

            int[] imageData = new int[SrcWidth * SrcHeight];
            fixed (int* dst = &imageData[0])
            {
                Helper.MemSet(dst, -1, SrcWidth * SrcHeight * 4);
            }

            int pos = 0;
            for (int line = 0; line < HSHeight; line++)
            {
                int pixCount = line * 4 + 4;
                int xpos = SrcHeight - line * 2 - 2;
                for (int i = 0; i < pixCount; i++)
                {
                    imageData[line * SrcWidth + xpos + i] = data[pos + i];
                }
                pos += pixCount;

            }
            for (int line = HSHeight; line < SrcHeight - 1; line++)
            {
                int pixCount = SrcWidth - (line - HSHeight) * 4 - 4;
                int xpos = (line - HSHeight) * 2 + 2;
                for (int i = 0; i < pixCount; i++)
                {
                    imageData[line * SrcWidth + xpos + i] = data[pos + i];
                }
                pos += pixCount;

            }

            List<Point> sp = new List<Point>();

            int[] transformed = new int[ConWidth * ConHeight];
            fixed (int* dst = &transformed[0])
            {
                Helper.MemSet(dst, -1, ConWidth * ConHeight * sizeof(int));
            }
            for (int i = 0; i < SrcHeight; i++)
            {
                for (int j = 0; j < SrcWidth; j++)
                {
                    // 只变换有颜色的
                    if (imageData[i * SrcWidth + j] != -1)
                    {
                        Vector3 res = MathEx.MatrixTransformPoint(ref transMat, new Vector3(j, i, 0));

                        int x = (int)res.X;
                        int y = (int)res.Y;
                        if (x >= 0 && x < ConWidth && y >= 0 && y < ConHeight)
                        {
                            if (transformed[y * ConWidth + x] == -1)
                            {
                                transformed[y * ConWidth + x] = imageData[i * SrcWidth + j];
                            }
                            else
                            {
                                sp.Add(new Point(j, i));
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < sp.Count; i++)
            {
                Vector3 res = MathEx.MatrixTransformPoint(ref transMat, new Vector3(sp[i].X, sp[i].Y, 0));
                int x = (int)res.X;
                int y = (int)res.Y;


                if (res.X > x && res.Y > y)  // br
                {
                    if (y < ConHeight - 1)
                    {
                        if (x < ConWidth - 1 && transformed[(y + 1) * ConWidth + x + 1] == -1)
                        {
                            transformed[(y + 1) * ConWidth + x + 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                        else if (transformed[(y + 1) * ConWidth + x] == -1)
                        {
                            transformed[(y + 1) * ConWidth + x] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                    }
                    else if (x < ConWidth - 1 && transformed[y * ConWidth + x + 1] == -1)
                    {
                        transformed[y * ConWidth + x + 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                    }
                }
                else if (res.X > x)  // tr
                {
                    if (y > 0)
                    {
                        if (x < ConWidth - 1 && transformed[(y - 1) * ConWidth + x + 1] == -1)
                        {
                            transformed[(y - 1) * ConWidth + x + 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                        else if (transformed[(y - 1) * ConWidth + x] == -1)
                        {
                            transformed[(y - 1) * ConWidth + x] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                    }
                    else if (x < ConWidth - 1 && transformed[y * ConWidth + x + 1] == -1)
                    {
                        transformed[y * ConWidth + x + 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                    }

                }
                else if (res.Y > y) // bl
                {
                    if (y < ConHeight - 1)
                    {
                        if (x > 0 && transformed[(y + 1) * ConWidth + x - 1] == -1)
                        {
                            transformed[(y + 1) * ConWidth + x - 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                        else if (transformed[(y + 1) * ConWidth + x] == -1)
                        {
                            transformed[(y + 1) * ConWidth + x] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                    }
                    else if (x > 0 && transformed[y * ConWidth + x - 1] == -1)
                    {
                        transformed[y * ConWidth + x - 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                    }
                }
                else
                {
                    if (y > 0)
                    {
                        if (x > 0 && transformed[(y - 1) * ConWidth + x - 1] == -1)
                        {
                            transformed[(y - 1) * ConWidth + x - 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                        else if (transformed[(y - 1) * ConWidth + x] == -1)
                        {
                            transformed[(y - 1) * ConWidth + x] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                        }
                    }
                    else if (x > 0 && transformed[y * ConWidth + x - 1] == -1)
                    {
                        transformed[y * ConWidth + x - 1] = imageData[sp[i].Y * SrcWidth + sp[i].X];
                    }
                }
            }



            castTable = new Point[900];
            sp.Clear();

            for (int i = 0; i < 900; i++)
            {
                castTable[i] = new Point(-1, -1);
            }

            for (int i = 0; i < ConHeight; i++)
            {
                for (int j = 0; j < ConWidth; j++)
                {
                    if (transformed[i * ConWidth + j] != -1)
                    {
                        castTable[transformed[i * ConWidth + j]] = new Point(j, i);
                    }
                    else
                    {
                        sp.Add(new Point(j, i));
                    }
                }
            }

            sampledPixels = sp.ToArray();
        }


        uint[] ReadImage(ArchiveBinaryReader br, Palette pal)
        {
            byte[] indexedImage = br.ReadBytes(900);
            uint[] palColors = pal.ARGBData;


            uint[] texture = new uint[ConWidth * ConHeight];

            for (int i = 0; i < 900; i++)
            {
                Point pt = castTable[i];

                if (pt.X != -1 && pt.Y != -1)
                {
                    texture[pt.Y * ConWidth + pt.X] = palColors[indexedImage[i]];
                }
            }

            for (int i = 0; i < sampledPixels.Length; i++)
            {
                Point pt = sampledPixels[i];
                int address = pt.Y * ConWidth + pt.X;
                if ((texture[address] & 0xff000000) == 0 || ((texture[address] << 8) == 0))
                {
                    uint r = 0; //= dstp[address] & 0x00ff0000;
                    uint g = 0; //= dstp[address] & 0x0000ff00;
                    uint b = 0; //= dstp[address] & 0x000000ff;

                    int count = 0;

                    int add = (pt.Y - 1) * ConWidth + pt.X;
                    if (pt.Y > 0 && (texture[add] << 8) != 0)
                    {
                        r += (texture[add] & 0x00ff0000) >> 16;
                        g += (texture[add] & 0x0000ff00) >> 8;
                        b += texture[add] & 0x000000ff;

                        count++;
                    }
                    add = pt.Y * ConWidth + pt.X - 1;
                    if (pt.X > 0 && (texture[add] << 8) != 0)
                    {

                        r += (texture[add] & 0x00ff0000) >> 16;
                        g += (texture[add] & 0x0000ff00) >> 8;
                        b += texture[add] & 0x000000ff;

                        count++;
                    }

                    add = (pt.Y + 1) * ConWidth + pt.X;
                    if (pt.Y < ConHeight - 1 && (texture[add] << 8) != 0)
                    {

                        r += (texture[add] & 0x00ff0000) >> 16;
                        g += (texture[add] & 0x0000ff00) >> 8;
                        b += texture[add] & 0x000000ff;

                        count++;
                    }
                    add = pt.Y * ConWidth + pt.X + 1;
                    if (pt.X < ConHeight - 1 && (texture[add] << 8) != 0)
                    {

                        r += (texture[add] & 0x00ff0000) >> 16;
                        g += (texture[add] & 0x0000ff00) >> 8;
                        b += texture[add] & 0x000000ff;

                        count++;
                    }
                    if (count > 0)
                    {
                        texture[address] = (uint)(((byte)((r / count) & 0xff) << 16) | ((byte)((g / count) & 0xff) << 8) | (byte)((b / count) & 0xff));
                        texture[address] |= 0xff000000;
                    }
                }

            }
            return texture;
        }

        ///// <summary>
        ///// 获得这个地块的纹理，多张不同形态。在特定位置
        ///// </summary>
        ///// <returns></returns>
        //public unsafe override ImageBase[] GetImages(int subTile, params object[] paras)
        //{
        //    Palette pal = (Palette)paras[0];

        //    ImageBase[][] res = new ImageBase[blockCount][];

        //    List<FileLocation> allForms = new List<FileLocation>(27);
        //    string fn = Path.GetFileNameWithoutExtension(Name);
        //    string ext = Path.GetExtension(Name);
        //    string loc = FileSystem.Instance.GetPath(Name);

        //    allForms.Add(FileSystem.Instance.Locate(loc + fn + ext, FileSystem.GameResLR));
        //    for (char j = 'a'; j <= 'z'; j++)
        //    {
        //        string file = loc + fn + j + ext;

        //        FileLocation fl = FileSystem.Instance.TryLocate(file, FileSystem.GameResLR);
        //        if (fl != null)
        //        {
        //            allForms.Add(fl);
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }

        //    List<ImageBase> imgs = new List<ImageBase>(allForms.Count);

        //    // 第一个特殊，就是自己
        //    ArchiveBinaryReader br = new ArchiveBinaryReader(allForms[0]);
        //    // 转到相应图片位置
        //    long oldPos = br.BaseStream.Position;
        //    br.BaseStream.Position += tileOffsets[subTile] + sizeof(TmpImageHeader);

        //    uint[] sounds = ReadImage(br, pal);
        //    br.BaseStream.Position = oldPos;

        //    RawImage image = new RawImage(ConWidth, ConHeight, ImagePixelFormat.A8R8G8B8);
        //    fixed (uint* src = &sounds[0])
        //    {
        //        Helper.MemCopy(image.GetData(), src, sounds.Length * sizeof(uint));
        //    }
        //    imgs.Add(image);
        //    br.Close();

        //    for (int j = 1; j < allForms.Count; j++)
        //    {
        //        br = new ArchiveBinaryReader(allForms[j]);

        //        oldPos = br.BaseStream.Position;

        //        // 跳过
        //        br.BaseStream.Position += sizeof(TmpHeader) + subTile * sizeof(int);
        //        int offset = br.ReadInt32();
        //        br.BaseStream.Position = oldPos + offset + sizeof(TmpImageHeader);

        //        sounds = ReadImage(br, pal);

        //        br.BaseStream.Position = oldPos;

        //        image = new RawImage(ConWidth, ConHeight, ImagePixelFormat.A8R8G8B8);

        //        fixed (uint* src = &sounds[0])
        //        {
        //            Helper.MemCopy(image.GetData(), src, sounds.Length * sizeof(uint));
        //        }

        //        imgs.Add(image);
        //        br.Close();

        //    }

        //    return imgs.ToArray();

        //}

        //public unsafe override ImageBase[][] GetImages(params object[] paras)
        //{
        //    Palette pal = (Palette)paras[0];

        //    ImageBase[][] res = new ImageBase[blockCount][];

        //    List<FileLocation> allForms = new List<FileLocation>(27);

        //    string fn = Path.GetFileNameWithoutExtension(Name);
        //    string ext = Path.GetExtension(Name);
        //    string loc = FileSystem.Instance.GetPath(Name);

        //    allForms.Add(FileSystem.Instance.Locate(loc + fn + ext, FileSystem.GameResLR));


        //    for (char j = 'a'; j <= 'z'; j++)
        //    {
        //        string file = loc + fn + j + ext;

        //        FileLocation fl = FileSystem.Instance.TryLocate(file, FileSystem.GameResLR);
        //        if (fl != null)
        //        {
        //            allForms.Add(fl);
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }

        //    for (int i = 0; i < blockCount; i++)
        //    {
        //        List<ImageBase> imgs = new List<ImageBase>(allForms.Count);

        //        // 第一个特殊，就是自己
        //        ArchiveBinaryReader br = new ArchiveBinaryReader(allForms[0]);
        //        // 转到相应图片位置
        //        long oldPos = br.BaseStream.Position;
        //        br.BaseStream.Position += tileOffsets[i] + sizeof(TmpImageHeader);

        //        uint[] sounds = ReadImage(br, pal);
        //        br.BaseStream.Position = oldPos;

        //        RawImage image = new RawImage(ConWidth, ConHeight, ImagePixelFormat.A8R8G8B8);
        //        fixed (uint* src = &sounds[0])
        //        {
        //            Helper.MemCopy(image.GetData(), src, sounds.Length * sizeof(uint));
        //        }
        //        imgs.Add(image);
        //        br.Close();

        //        for (int j = 1; j < allForms.Count; j++)
        //        {
        //            br = new ArchiveBinaryReader(allForms[j]);

        //            oldPos = br.BaseStream.Position;

        //            // 跳过
        //            br.BaseStream.Position += sizeof(TmpHeader) + i * sizeof(int);
        //            int offset = br.ReadInt32();
        //            br.BaseStream.Position = oldPos + offset + sizeof(TmpImageHeader);

        //            sounds = ReadImage(br, pal);

        //            br.BaseStream.Position = oldPos;

        //            image = new RawImage(ConWidth, ConHeight, ImagePixelFormat.A8R8G8B8);

        //            fixed (uint* src = &sounds[0])
        //            {
        //                Helper.MemCopy(image.GetData(), src, sounds.Length * sizeof(uint));
        //            }

        //            imgs.Add(image);
        //            br.Close();

        //        }

        //        res[i] = imgs.ToArray();
        //    }


        //    //Texture.ToFile(res[0][0].GetTexture(Game.Instance.Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\0.bmp", ImageFileFormat.Bmp);
        //    //Texture.ToFile(res[0][1].GetTexture(Game.Instance.Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\1.bmp", ImageFileFormat.Bmp);
        //    //Texture.ToFile(res[0][2].GetTexture(Game.Instance.Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\2.bmp", ImageFileFormat.Bmp);
        //    //Texture.ToFile(res[0][3].GetTexture(Game.Instance.Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\3.bmp", ImageFileFormat.Bmp);

        //    return res;
        //}
        public unsafe override ImageBase GetImage(int subTile, params object[] paras)
        {
            Palette pal = (Palette)paras[0];

            ImageBase[] res = new ImageBase[blockCount];

            string fn = Path.GetFileNameWithoutExtension(Name);
            string ext = Path.GetExtension(Name);
            string loc = FileSystem.Instance.GetPath(Name);

            FileLocation fl = FileSystem.Instance.Locate(loc + fn + ext, FileSystem.GameResLR);

            ArchiveBinaryReader br = new ArchiveBinaryReader(fl);
            // 转到相应图片位置
            br.BaseStream.Position = tileOffsets[subTile] + sizeof(TmpImageHeader);

            uint[] data = ReadImage(br, pal);

            RawImage image = new RawImage(ConWidth, ConHeight, ImagePixelFormat.A8R8G8B8);
            fixed (uint* src = &data[0])
            {
                Memory.Copy(src, image.GetData(), data.Length * sizeof(uint));
            }

            br.Close();

            return image;
        }

        public unsafe ImageBase GetImageNoFS(int subTile, params object[] paras)
        {
            Palette pal = (Palette)paras[0];

            ImageBase[] res = new ImageBase[blockCount];


            FileLocation fl = new FileLocation(Name);

            ArchiveBinaryReader br = new ArchiveBinaryReader(fl);
            // 转到相应图片位置
            br.BaseStream.Position = tileOffsets[subTile] + sizeof(TmpImageHeader);

            uint[] data = ReadImage(br, pal);

            RawImage image = new RawImage(ConWidth, ConHeight, ImagePixelFormat.A8R8G8B8);
            fixed (uint* src = &data[0])
            {
                Memory.Copy(src, image.GetData(), data.Length * sizeof(uint));
            }

            br.Close();

            return image;
        }
        public unsafe override ImageBase[] GetImages(params object[] paras)
        {
            Palette pal = (Palette)paras[0];

            ImageBase[] res = new ImageBase[blockCount];

            string fn = Path.GetFileNameWithoutExtension(Name);
            string ext = Path.GetExtension(Name);
            string loc = FileSystem.Instance.GetPath(Name);

            ResourceLocation fl = FileSystem.Instance.Locate(loc + fn + ext, FileSystem.GameResLR);

            for (int i = 0; i < blockCount; i++)
            {
                ArchiveBinaryReader br = new ArchiveBinaryReader(fl);
                // 转到相应图片位置
                br.BaseStream.Position = tileOffsets[i] + sizeof(TmpImageHeader);

                uint[] data = ReadImage(br, pal);

                RawImage image = new RawImage(ConWidth, ConHeight, ImagePixelFormat.A8R8G8B8);
                fixed (uint* src = &data[0])
                {
                    Memory.Copy(src, image.GetData(), data.Length * sizeof(uint));
                }

                br.Close();

                res[i] = image;
            }

            return res;
        }


        //public override ImageBase[] GetImages(string ext, int subTile)
        //{
        //    List<ImageBase> imgs = new List<ImageBase>();

        //    string fn = Path.GetFileNameWithoutExtension(Name);
        //    string loc = FileSystem.Instance.GetPath(Name);


        //    string file = loc + fn + subTile.ToString() + ext;

        //    FileLocation fl = FileSystem.Instance.TryLocate(file, FileSystem.GameResLR);

        //    if (fl != null)
        //    {
        //        imgs.Add(ImageManager.Instance.CreateImageUnmanaged(fl));
        //    }
        //    else
        //    {
        //        GameConsole.Instance.Write(ResourceAssembly.Instance.CM_MissingTileImage(file), ConsoleMessageType.Warning);
        //        imgs.Add(ImageManager.Instance.CreateImageUnmanaged(FileSystem.Instance.SpecialFile.MissingTileTexture));
        //    }

        //    for (char i = 'a'; i <= 'z'; i++)
        //    {
        //        file = loc + fn + i + subTile.ToString() + ext;

        //        fl = FileSystem.Instance.TryLocate(file, FileSystem.GameResLR);
        //        if (fl != null)
        //            imgs.Add(ImageManager.Instance.CreateImageUnmanaged(fl));
        //        else
        //            break;
        //    }

        //    return imgs.ToArray();
        //}
        //public override ImageBase[][] GetImages(string ext)
        //{
        //    ImageBase[][] res = new ImageBase[blockCount][];
        //    for (int i = 0; i < blockCount; i++)
        //    {
        //        List<ImageBase> imgs = new List<ImageBase>();

        //        string fn = Path.GetFileNameWithoutExtension(Name);
        //        string loc = FileSystem.Instance.GetPath(Name);


        //        string file = loc + fn + i.ToString() + ext;

        //        FileLocation fl = FileSystem.Instance.TryLocate(file, FileSystem.GameResLR);

        //        if (fl != null)
        //        {
        //            imgs.Add(ImageManager.Instance.CreateImageUnmanaged(fl));
        //        }
        //        else
        //        {
        //            GameConsole.Instance.Write(ResourceAssembly.Instance.CM_MissingTileImage(file), ConsoleMessageType.Warning);

        //            for (int j = i; j < blockCount; j++)
        //            {
        //                res[j] = new ImageBase[] { ImageManager.Instance.CreateImageUnmanaged(FileSystem.Instance.SpecialFile.MissingTileTexture) };
        //            }
        //            return res;
        //            //continue;
        //        }

        //        for (char j = 'a'; j <= 'z'; j++)
        //        {
        //            file = loc + fn + j + i.ToString() + ext;

        //            fl = FileSystem.Instance.TryLocate(file, FileSystem.GameResLR);
        //            if (fl != null)
        //                imgs.Add(ImageManager.Instance.CreateImageUnmanaged(fl));
        //            else
        //                break;
        //        }

        //        res[i] = imgs.ToArray();

        //    }
        //    //Texture.ToFile(res[0][0].GetTexture(Game.Instance .Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\0.bmp", ImageFileFormat.Bmp);
        //    //Texture.ToFile(res[0][1].GetTexture(Game.Instance.Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\1.bmp", ImageFileFormat.Bmp);
        //    //Texture.ToFile(res[0][2].GetTexture(Game.Instance.Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\2.bmp", ImageFileFormat.Bmp);
        //    //Texture.ToFile(res[0][3].GetTexture(Game.Instance.Device, Usage.None, Pool.Managed), @"C:\Documents and Settings\Yuri\桌面\3.bmp", ImageFileFormat.Bmp);

        //    return res;
        //}

      
        public override int GetHeight(int i)
        {
            if (i >= blockCount) i = blockCount - 1;
            return blocks[i].height;
        }

        public override TerrainType GetTerrainType(int i)
        {
            if (i >= blockCount) i = blockCount - 1;
            return blocks[i].terrain_type;
        }
        public override void GetRadarColor(int index, out Color left, out Color right)
        {
            if (index >= blockCount) index = blockCount - 1;
            left = Color.FromArgb(255, blocks[index].radar_red_left, blocks[index].radar_green_left, blocks[index].radar_blue_left);
            right = Color.FromArgb(255, blocks[index].radar_red_right, blocks[index].radar_green_right, blocks[index].radar_blue_right);
        }


        public TmpImageHeader[] Blocks
        {
            get { return blocks; }
        }


        /// <summary>
        /// 具体情形参见Tmp Studio
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public override int GetRampType(int i)
        {
            if (i >= blockCount) i = blockCount - 1;
            return blocks[i].ramp_type;
        }
        public override int GetSecHeight(int i)
        {
            throw new NotSupportedException();
        }
        public override int GetSecRampType(int i)
        {
            throw new NotSupportedException();
        }
        public override TerrainType GetSecTerrainType(int i)
        {
            throw new NotSupportedException();
        }

        private Tile(string file)
            : this(new FileLocation(file)) { }
        private unsafe Tile(ResourceLocation fl)
            : base(fl.Name)
        {
            Stream src = fl.GetStream;

            ArchiveBinaryReader br = new ArchiveBinaryReader(src, Encoding.Default);

            header.blockCountX = br.ReadInt32();
            header.blockCountY = br.ReadInt32();
            header.width = br.ReadInt32();
            header.height = br.ReadInt32();

            blockCount = header.blockCountX * header.blockCountY;
            tileOffsets = new int[blockCount];

            for (int i = 0; i < blockCount; i++)
            {
                tileOffsets[i] = br.ReadInt32();
            }

            blocks = new TmpImageHeader[blockCount];
            isVaild = new bool[blockCount];

            for (int i = 0; i < blockCount; i++)
            {
                if (tileOffsets[i] != 0)
                {
                    isVaild[i] = true;
                    src.Position = tileOffsets[i];

                    blocks[i].x = br.ReadInt32();
                    blocks[i].y = br.ReadInt32();

                    blocks[i].extra_ofs = br.ReadInt32();
                    blocks[i].z_ofs = br.ReadInt32();
                    blocks[i].extra_z_ofs = br.ReadInt32();

                    blocks[i].x_extra = br.ReadInt32();
                    blocks[i].y_extra = br.ReadInt32();
                    blocks[i].cx_extra = br.ReadInt32();
                    blocks[i].cy_extra = br.ReadInt32();

                    blocks[i].bits = (TmpImageBits)(br.ReadUInt32() & 7);
                    blocks[i].height = br.ReadSByte();
                    blocks[i].terrain_type = (TerrainType)br.ReadByte();
                    blocks[i].ramp_type = br.ReadSByte();

                    blocks[i].radar_red_left = br.ReadByte();
                    blocks[i].radar_green_left = br.ReadByte();
                    blocks[i].radar_blue_left = br.ReadByte();
                    blocks[i].radar_red_right = br.ReadByte();
                    blocks[i].radar_green_right = br.ReadByte();
                    blocks[i].radar_blue_right = br.ReadByte();
                    //blocks[i].pad[0] = br.ReadSByte();
                    //blocks[i].pad[1] = br.ReadSByte();
                    //blocks[i].pad[2] = br.ReadSByte();

                    // skip all images
                }
            }
            
            br.Close();
        }

        void BuildRemapOffset()
        {
            int shift = 0;
            remapOffset = new int[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                if (!isVaild[i])
                    shift--;

                remapOffset[i] = shift;
            }
        }
        public override void RemapSubtileIndex(ref int p)
        {
            if (remapOffset == null)
            {
                BuildRemapOffset();
            }
            p = (byte)(p + remapOffset[p]);
        }
        public bool IsVaild(int index)
        {
            return isVaild[index];
        }

        public static Tile FromFile(ResourceLocation fl)
        {
            return new Tile(fl);
        }
        public static Tile FromFile(string file)
        {
            return new Tile(file);
        }

        public override void Dispose()
        { }

        //public static 
        //public TmpFile(string file)
        //    : this(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read),
        //           file, (int)new FileInfo(file).Length, true)
        //{ }

        //private unsafe TmpFile(Stream src, string file, int size, bool autoCloseStream)
        //    : base(file, size, !autoCloseStream)
        //{
        //}

        //public TmpFile(FileLocation file)
        //    : this(file.GetStream, file.Path, file.Size, !file.IsInMix)
        //{ }

        //void ReadImage(MixBinaryReader br)
        //{
        //    int w = header.width;
        //    int h = header.height;

        //    int[] imagedata = new int[w * h];
        //    int[] zdata = new int[w * h];
        //    int idata = 0;
        //    int j = 0;
        //    for (int emptyPlace = w / 2 - 2; emptyPlace >= 0; emptyPlace -= 2)
        //    {
        //        for (j = 0; j < emptyPlace; j++)
        //            imagedata[idata++] = 0;
        //        for (j = 0; j < w - emptyPlace * 2; j++)
        //            imagedata[idata++] = br.ReadInt32();
        //        for (j = 0; j < emptyPlace; j++)
        //            imagedata[idata++] = 0;
        //    }
        //    for (int emptyPlace = 2; emptyPlace <= w / 2 - 1; emptyPlace += 2)
        //    {
        //        for (j = 0; j < emptyPlace; j++)
        //        {
        //            imagedata[idata] = 0;
        //            zdata[idata++] = 0;
        //        }
        //        for (j = 0; j < w - emptyPlace * 2; j++)
        //            imagedata[idata++] = br.ReadInt32();
        //        for (j = 0; j < emptyPlace; j++)
        //            imagedata[idata++] = 0;
        //    }
        //}
        //void ReadImage() { }
    }
}
