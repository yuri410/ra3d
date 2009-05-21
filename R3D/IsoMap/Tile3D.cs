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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.MathLib;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.IsoMap
{
    [Flags()]
    public enum BlockBits : int
    {
        None = 0,
        HasDoubleLayer = 1 << 0,
        HasTileModel = 1 << 1,
        HasDaformationData = 1 << 2,
        Invisible = 1 << 3
    }
    [Flags()]
    public enum BlockMaterialEffect
    {
        None = 0,
        Water = 1
    }
    [Obsolete()]
    public enum BlockModelType
    {
        Single = 0,
        AutoMergeGaps
    }


    //public class BlockMesh : GameMesh
    //{
    //    //public new class Data : GameMesh.Data
    //    //{
    //    //    BlockModelType type;
    //    //    VertexFlags[] flags;

    //    //    public BlockModelType ModelType
    //    //    {
    //    //        get { return type; }
    //    //        set { type = value; }
    //    //    }

    //    //    public override void Load()
    //    //    {
    //    //        ArchiveBinaryReader br = new ArchiveBinaryReader(resLocation);

    //    //        int id = br.ReadInt32();

    //    //        if (id == (int)FileID.Mesh)
    //    //        {
    //    //            type = (BlockModelType)br.ReadInt32();
    //    //            ReadContent(br);
    //    //            int count = br.ReadInt32();
    //    //            flags = new VertexFlags[count];
    //    //            for (int i = 0; i < count; i++)
    //    //            {
    //    //                flags[i] = (VertexFlags)br.ReadInt32();
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            throw new DataFormatException(resLocation.ToString());
    //    //        }

    //    //        br.Close();
    //    //    }
    //    //    public override void Save()
    //    //    {
    //    //        ArchiveBinaryWriter bw = new ArchiveBinaryWriter(resLocation);

    //    //        bw.Write((int)FileID.Mesh);
    //    //        bw.Write((int)type);
    //    //        WriteContent(bw);
    //    //        if (flags != null)
    //    //        {
    //    //            bw.Write(flags.Length);
    //    //            for (int i = 0; i < flags.Length; i++)
    //    //            {
    //    //                bw.Write((int)flags[i]);
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            bw.Write((int)0);
    //    //        }
    //    //        bw.Close();
    //    //    }
    //    //}

    //    BlockModelType type;
    //    public BlockModelType ModelType
    //    {
    //        get { return type; }
    //        set { type = value; }
    //    }

    //}
    //public class BlockModel
    //{
    //    //public class Data : DataSrcBase
    //    //{
    //    //    List<BlockMesh.Data> entities = new List<BlockMesh.Data>();

    //    //    public List<BlockMesh.Data> Entities
    //    //    {
    //    //        get { return entities; }
    //    //        set { entities = value; }
    //    //    }


    //    //    public override void Load()
    //    //    {
    //    //        if (resLocation == null)
    //    //            throw new InvalidOperationException();

    //    //        ArchiveBinaryReader br = new ArchiveBinaryReader(resLocation);

    //    //        if (br.ReadInt32() != (int)FileID.Model)
    //    //        {
    //    //            br.ReadInt32();
    //    //            int count = br.ReadInt32();
    //    //            entities.Clear();
    //    //            entities.Capacity = count;

    //    //            for (int i = 0; i < count; i++)
    //    //            {
    //    //                BlockMesh.Data sounds = new BlockMesh.Data();
    //    //                sounds.ResourceLocation = new StreamedLocation(br.BaseStream, false);
    //    //                sounds.Load();
    //    //                entities.Add(sounds);
    //    //            }
    //    //            br.Close();
    //    //        }
    //    //        else
    //    //        {
    //    //            throw new DataFormatException(resLocation);
    //    //        }
    //    //    }

    //    //    public override void Save()
    //    //    {
    //    //        if (resLocation == null)
    //    //            throw new InvalidOperationException();
    //    //        ArchiveBinaryWriter bw = new ArchiveBinaryWriter(resLocation);
    //    //        //bw.BaseStream.SetLength(0);
    //    //        bw.Write((int)FileID.Model);
    //    //        bw.Write((int)0);
    //    //        bw.Write(entities.Count);
    //    //        bw.Flush();
    //    //        for (int i = 0; i < entities.Count; i++)
    //    //        {
    //    //            entities[i].ResourceLocation = new StreamedLocation(bw.BaseStream, false);
    //    //            entities[i].Save();
    //    //        }

    //    //        bw.Close();
    //    //    }


    //    //}
    //    BlockMesh[] entities;

    //}


    /// <summary>
    /// 提供抽象数据格式支持
    /// </summary>
    public abstract class BlockMaterialBase<TexType>
        where TexType : class
    {
        protected static readonly string EffectTag = "Effect";

        protected static readonly string HasTextureTag = "HasTexture";
        protected static readonly string HasDetailedTextureTag = "HasDetailedTexture";


        protected static readonly string TextureTag = "Texture";
        protected static readonly string DetailedTextureTag = "DetailedTexture";

        protected BlockMaterialEffect effect;

        protected bool hasTexture;
        protected TexType texture;

        protected bool hasDetailedTex;

        protected string detTexFile;
       

        [Browsable(false)]
        public bool HasTexture
        {
            get { return hasTexture; }
            set { hasTexture = value; }
        }
        [Browsable(false)]
        public bool HasDetailedTexture
        {
            get { return hasDetailedTex; }
            set { hasDetailedTex = value; }
        }


        public BlockMaterialEffect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        public virtual TexType Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public string DetailedTextureFile
        {
            get { return detTexFile; }
            set { detTexFile = value; }
        }

        protected abstract TexType LoadTexture(ArchiveBinaryReader br);
        protected abstract void SaveTexture(TexType tex, ArchiveBinaryWriter bw);
        
        

        protected virtual void ReadData(BinaryDataReader data)
        {
            //base.ReadData(sounds);

            effect = (BlockMaterialEffect)data.GetDataInt32(EffectTag, 0);

            //ArchiveBinaryReader br = sounds.GetData(MaterialTag);
            //br.ReadMaterial(out mat);
            //br.Close();

            hasTexture = data.GetDataBool(HasTextureTag);

            
            if (hasTexture)
            {
                //isTexEmbedded = sounds.GetDataBool(IsTexEmbeddedTag);

                ArchiveBinaryReader br = data.GetData(TextureTag);
                //Stream stm = sounds.GetDataStream(TextureTag);
                texture = LoadTexture(br);

                br.Close();
            }

            hasDetailedTex = data.GetDataBool(HasDetailedTextureTag);

            if (hasDetailedTex)
            {
                ArchiveBinaryReader br = data.GetData(DetailedTextureTag);
                detTexFile = br.ReadStringUnicode();
                br.Close();
            }
        }

        protected virtual void WriteData(BinaryDataWriter data)
        {
            //base.WriteData(sounds);

            data.AddEntry(EffectTag, (int)effect);

            //ArchiveBinaryWriter bw = sounds.AddEntry(MaterialTag);
            //bw.Write(ref mat);
            //bw.Close();

            data.AddEntry(HasTextureTag, texture != null);

            if (texture != null)
            {
                //sounds.AddEntry(IsTexEmbeddedTag, isTexEmbedded);

                ArchiveBinaryWriter bw = data.AddEntry(TextureTag);
                SaveTexture(texture, bw);
                bw.Close();
                //bw.Write(texture); //.WriteStringUnicode(mat.textureFile);
            }

            data.AddEntry(HasDetailedTextureTag, hasDetailedTex);
            if (hasDetailedTex)
            {
                ArchiveBinaryWriter bw = data.AddEntry(DetailedTextureTag);
                bw.WriteStringUnicode(detTexFile);
                bw.Close();

            }
        }

        //protected abstract TexType LoadTexture(Stream stm);
        //protected abstract void SaveTexture(TexType tex, Stream stm);

    }

    /// <summary>
    /// 游戏中使用
    /// </summary>
    public class BlockMaterial : BlockMaterialBase<ImageBase>, IDisposable
    {
        public static Material DefBlockColor;

        static BlockMaterial()
        {
            DefBlockColor.Ambient = new Color4(0.35f, 0.35f, 0.35f);
            DefBlockColor.Diffuse = new Color4(0.9f, 0.9f, 0.9f);
            DefBlockColor.Specular = new Color4(0, 0.0f, 0.0f, 0.0f);
            DefBlockColor.Power = 16;
            //terrainLighting.SetValue(tlParamKa, new float[] { 0.3f, 0.3f, 0.3f, 1 });
            //terrainLighting.SetValue(tlParamKd, new float[] { 0.75f, 0.75f, 0.75f, 1 });
            //terrainLighting.SetValue(tlParamKs, new float[] { 0f, 0f, 0f, 1 });
            //terrainLighting.SetValue(tlParamPwr, 100f);
        }

        bool disposed;
        //Texture detailed;
        //int detailedTextureOffset;
        //ResourceLocation detailedTextureRes;
        //int detTexOffset;

        protected override ImageBase LoadTexture(ArchiveBinaryReader br)
        {
            return br.ReadEmbededImage();
        }
        protected override void SaveTexture(ImageBase tex, ArchiveBinaryWriter bw)
        {
            bw.Write(tex);
        }

        public static BlockMaterial FromBinary(BinaryDataReader data)
        {
            BlockMaterial res = new BlockMaterial();

            res.ReadData(data);
            
            return res;
        }
        public static BinaryDataWriter ToBinary(BlockMaterial mat)
        {
            BinaryDataWriter data = new BinaryDataWriter();

            mat.WriteData(data);

            return data;
        }


        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                if (texture != null)
                    texture.Dispose();
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion

        ~BlockMaterial()
        {
            if (!disposed)
                Dispose();
        }
    }


    /// <summary>
    /// 提供抽象数据格式支持
    /// </summary>
    public abstract class BlockBase<MType, MdlType>
        where MType : class
        where MdlType : class
    {
        //public int indexHelper = -1;

        /// <summary>
        /// 以单元为单位
        /// </summary>
        public int x;
        public int y;
        public BlockBits bits;

        public byte radar_red_left;
        public byte radar_green_left;
        public byte radar_blue_left;
        public byte radar_red_right;
        public byte radar_green_right;
        public byte radar_blue_right;

        public short height;
        public TerrainType terrain_type;
        public byte ramp_type;

        public short secHeight;
        public TerrainType secTerrType;
        public byte secRampType;

        public MType mat1;
        public MType mat2;
        protected MdlType model;

        [Browsable(false)]
        public virtual MdlType Model
        {
            get { return model; }
            set { model = value; }
        }
        [Browsable(false)]
        public virtual bool HasDoubleLayers
        {
            get { return (bits & BlockBits.HasDoubleLayer) != 0; }
            set { throw new NotSupportedException(); }
        }
        [Browsable(false)]
        public virtual bool Has3DModel
        {
            get { return (bits & BlockBits.HasTileModel) != 0; }
            set { throw new NotSupportedException(); }
        }
        [Browsable(false)]
        public virtual bool HasDeformationData
        {
            get { return (bits & BlockBits.HasDaformationData) != 0; }
            set { throw new NotSupportedException(); }
        }
    }

    /// <summary>
    /// 游戏中使用
    /// </summary>
    public class Block : BlockBase<BlockMaterial, GameModel>, IDisposable
    {
        bool disposed;

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                mat1.Dispose();
                if (mat2 != null)
                    mat2.Dispose();
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion

        ~Block()
        {
            if (!disposed)
                Dispose();
        }
    }



    /// <summary>
    /// 提供抽象数据格式支持
    /// </summary>
    public abstract class Tile3DBase<BlockType, MaterialType, ModelType> : TileBase
        where BlockType : BlockBase<MaterialType, ModelType>
        where MaterialType : class
        where ModelType : class
    {
        protected BlockType[] blocks;

        protected Device dev;

        //protected ResourceLocation resLoc;

        protected Tile3DBase(Device dev, Stream stm, string name)
            : base(name)
        {
            this.dev = dev;
            ReadData(stm);
        }
        protected Tile3DBase(string name)
            : base(name)
        { }


        protected abstract BlockType CreateNewBlock();
        protected abstract MaterialType LoadMaterial(BinaryDataReader data);
        protected abstract ModelType LoadModel(BinaryDataReader data);

        protected abstract BinaryDataWriter SaveMaterial(MaterialType mat);
        protected abstract BinaryDataWriter SaveModel(ModelType mdl);

        protected void ReadData(Stream stm)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(stm, Encoding.Default);

            int id = br.ReadInt32();

            if (id == (int)FileID.Tmp3D)
            {
                blockCount = br.ReadInt32();

                int[] blockOffsets = new int[blockCount];
                blocks = new BlockType[blockCount];

                for (int i = 0; i < blockCount; i++)
                {
                    blockOffsets[i] = br.ReadInt32();
                }

                for (int i = 0; i < blockCount; i++)
                {
                    br.BaseStream.Position = blockOffsets[i];

                    BlockType block = CreateNewBlock();

                    BlockBase<MaterialType, ModelType> baseBlock = block;

                    //baseBlock.indexHelper = br.ReadInt32();
                    baseBlock.x = br.ReadInt32();
                    baseBlock.y = br.ReadInt32();
                    baseBlock.bits = (BlockBits)br.ReadUInt32();
                    baseBlock.radar_red_left = br.ReadByte();
                    baseBlock.radar_green_left = br.ReadByte();
                    baseBlock.radar_blue_left = br.ReadByte();
                    baseBlock.radar_red_right = br.ReadByte();
                    baseBlock.radar_green_right = br.ReadByte();
                    baseBlock.radar_blue_right = br.ReadByte();

                    baseBlock.height = br.ReadInt16();
                    baseBlock.terrain_type = (TerrainType)br.ReadByte();
                    baseBlock.ramp_type = br.ReadByte();

                    BinaryDataReader matData = br.ReadBinaryData();
                    baseBlock.mat1 = LoadMaterial(matData); // BlockMaterial.FromBinary(br.ReadBinaryData());
                    matData.Close();

                    if (baseBlock.HasDoubleLayers)
                    {
                        baseBlock.secHeight = br.ReadInt16();
                        baseBlock.secTerrType = (TerrainType)br.ReadByte();
                        baseBlock.secRampType = br.ReadByte();

                        matData = br.ReadBinaryData();
                        baseBlock.mat2 = LoadMaterial(matData);// BlockMaterial.FromBinary(br.ReadBinaryData());
                        matData.Close();
                    }
                    if (baseBlock.Has3DModel)
                    {
                        //loadModel(i);
                        BinaryDataReader mdlData = br.ReadBinaryData();
                        baseBlock.Model = LoadModel(mdlData);
                        mdlData.Close();
                    }
                    if (baseBlock.HasDeformationData)
                    {

                    }

                    blocks[i] = block;
                }
            }
            else
            {
                throw new DataFormatException(Name);
            }

            br.Close();

        }

        protected void WrtieData(Stream stm)
        {
            ArchiveBinaryWriter bw = new ArchiveBinaryWriter(stm, Encoding.Default);

            bw.Write((int)FileID.Tmp3D);

            bw.Write(blockCount);

            long offsetPos = bw.BaseStream.Position;

            long[] blockOffsets = new long[blockCount];
            bw.BaseStream.Position += blockCount * sizeof(int);

            for (int i = 0; i < blockCount; i++)
            {
                blockOffsets[i] = bw.BaseStream.Position;

                //BlockType block = CreateNewBlock();
                BlockBase<MaterialType, ModelType> baseBlock = blocks[i];

                //bw.Write(baseBlock.indexHelper);
                bw.Write(baseBlock.x);
                bw.Write(baseBlock.y);
                bw.Write((uint)baseBlock.bits);

                bw.Write(baseBlock.radar_red_left);
                bw.Write(baseBlock.radar_green_left);
                bw.Write(baseBlock.radar_blue_left);
                bw.Write(baseBlock.radar_red_right);
                bw.Write(baseBlock.radar_green_right);
                bw.Write(baseBlock.radar_blue_right);

                bw.Write(baseBlock.height);
                bw.Write((byte)baseBlock.terrain_type);
                bw.Write(baseBlock.ramp_type);

                bw.Write(SaveMaterial(baseBlock.mat1));

                if (baseBlock.HasDoubleLayers)
                {
                    bw.Write(baseBlock.secHeight);
                    bw.Write((byte)baseBlock.secTerrType);
                    bw.Write(baseBlock.secRampType);

                    bw.Write(SaveMaterial(baseBlock.mat2));
                }
                if (baseBlock.Has3DModel)
                {
                    bw.Write(SaveModel(baseBlock.Model));
                }
                if (baseBlock.HasDeformationData)
                {

                }
            }

            bw.BaseStream.Position = offsetPos;
            for (int i = 0; i < blockCount; i++)
            {
                bw.Write((int)blockOffsets[i]);
            }

            bw.Close();
        }

        public override int GetHeight(int i)
        {
            //if (i >= blockCount)
            //{
            //    i = blockCount - 1;
            //}
            return blocks[i].height;
        }
        public override void GetRadarColor(int index, out Color left, out Color right)
        {
            BlockBase<MaterialType, ModelType> blk = blocks[index];
            left = Color.FromArgb(255, blk.radar_red_left, blk.radar_green_left, blk.radar_blue_left);
            right = Color.FromArgb(255, blk.radar_red_right, blk.radar_green_right, blk.radar_blue_right);
        }

        public override TerrainType GetTerrainType(int i)
        {
            //if (i >= blockCount)
            //{
            //    i = blockCount - 1;
            //}
            return blocks[i].terrain_type;
        }

        public override int GetRampType(int i)
        {
            return blocks[i].ramp_type;
        }
        public override int GetSecHeight(int i)
        {
            return blocks[i].secHeight;
        }
        public override int GetSecRampType(int i)
        {
            return blocks[i].secRampType;
        }
        public override TerrainType GetSecTerrainType(int i)
        {
            return blocks[i].secTerrType;
        }

        //public override bool SupportsMatrial
        //{
        //    get { return true; }
        //}
        //public override bool SupportsDoubleLayer
        //{
        //    get { return true; }
        //}

    }

    /// <summary>
    /// 新的3D地块
    /// </summary>
    /// <remarks>游戏中使用</remarks>
    public unsafe class Tile3D : Tile3DBase<Block, BlockMaterial, GameModel>
    {
        bool disposed;


        private Tile3D(Device dev, ResourceLocation fl)
            : base(dev, fl.GetStream, fl.Name)
        { }

        protected override Block CreateNewBlock()
        {
            return new Block();
        }
        protected override BlockMaterial LoadMaterial(BinaryDataReader data)
        {
            return BlockMaterial.FromBinary(data);
        }
        protected override GameModel LoadModel(BinaryDataReader data)
        {
            return GameModel.FromBinary(dev, data);
        }

        public static void ToStream(Tile3D tile, Stream stm)
        {
            tile.WrtieData(stm);
            //bw.Write((int)FileID.Tmp3D);

            //int blockCount = tile.blockCount;

            //bw.Write(blockCount);

            //long blockOffsetsPos = bw.BaseStream.Position;

            //int[] offsets = new int[blockCount];

            //// 注意：这里先占个位置
            //bw.BaseStream.Position += sizeof(int) * blockCount;

            //for (int i = 0; i < blockCount; i++)
            //{
            //    offsets[i] = (int)bw.BaseStream.Position;

            //    Block block = tile.blocks[i]; // new BlockData();

            //    bw.Write(block.x);
            //    bw.Write(block.y);
            //    bw.Write((uint)block.bits);

            //    bw.Write(block.radar_red_left);
            //    bw.Write(block.radar_green_left);
            //    bw.Write(block.radar_blue_left);
            //    bw.Write(block.radar_red_right);
            //    bw.Write(block.radar_green_right);
            //    bw.Write(block.radar_blue_right);

            //    bw.Write(block.height);
            //    bw.Write((sbyte)block.terrain_type);
            //    bw.Write(block.ramp_type);

            //    bw.Write(BlockMaterial.ToBinary(block.mat1));

            //    if (block.HasDoubleLayers)
            //    {
            //        bw.Write(block.secHeight);
            //        bw.Write((sbyte)block.secTerrType);
            //        bw.Write(block.secRampType);

            //        bw.Write(BlockMaterial.ToBinary(block.mat2));
            //    }
            //    if (block.Has3DModel)
            //    {
            //        GameModel mdl = block.Model;

            //        bw.Write(GameModel.ToBinary(mdl));
            //    }
            //    if (block.HasDeformationData)
            //    {

            //    }
            //}

            //bw.BaseStream.Position = blockOffsetsPos;
            //for (int i = 0; i < blockCount; i++)
            //{
            //    bw.Write(offsets[i]);
            //}

            //bw.Close();
        }
        public static Tile3D FromFile(Device dev, ResourceLocation fl)
        {
            return new Tile3D(dev, fl);
        }
        public static Tile3D FromFile(Device dev, string file)
        {
            return new Tile3D(dev, new FileLocation(file));
        }


        public override GameModel GetTileModel(int index)
        {
            return blocks[index].Model;
        }
        public override int GetHeight(int i)
        {
            //if (i >= blockCount)
            //{
            //    i = blockCount - 1;
            //}
            return blocks[i].height;
        }
        public override void GetRadarColor(int index, out Color left, out Color right)
        {
            Block blk = blocks[index];
            left = Color.FromArgb(255, blk.radar_red_left, blk.radar_green_left, blk.radar_blue_left);
            right = Color.FromArgb(255, blk.radar_red_right, blk.radar_green_right, blk.radar_blue_right);
        }

        public override TerrainType GetTerrainType(int i)
        {
            //if (i >= blockCount)
            //{
            //    i = blockCount - 1;
            //}
            return blocks[i].terrain_type;
        }

        public override int GetRampType(int i)
        {
            return blocks[i].ramp_type;
        }

        public override int GetSecHeight(int i)
        {
            return blocks[i].secHeight;            
        }
        public override int GetSecRampType(int i)
        {
            return blocks[i].secRampType;
        }
        public override TerrainType GetSecTerrainType(int i)
        {
            return blocks[i].secTerrType;
        }
#warning layer 2
        public override ImageBase[] GetImages(params object[] paras)
        {
            ImageBase[] res = new ImageBase[blocks.Length];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = blocks[i].mat1.Texture;
            }
            return res;
        }
        public override ImageBase GetImage(int subTile, params object[] paras)
        {
            return blocks[subTile].mat1.Texture;
        }
        public override BlockBits GetBlockBits(int index)
        {
            return blocks[index].bits;
        }
        protected override BinaryDataWriter SaveMaterial(BlockMaterial mat)
        {
            return BlockMaterial.ToBinary(mat);
        }

        protected override BinaryDataWriter SaveModel(GameModel mdl)
        {
            return GameModel.ToBinary(mdl);
        }

        public override void Dispose()
        {
            if (!disposed)
            {
                for (int i = 0; i < blockCount; i++)
                {
                    blocks[i].Dispose();
                }
                blocks = null;
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());

        }
        ~Tile3D()
        {
            if (!disposed)
                Dispose();
        }

        public override void ReleaseTextures()
        {
            for (int i = 0; i < blockCount; i++)
            {
                if (blocks[i].mat1.Texture != null)
                    blocks[i].mat1.Texture.Dispose();
                if (blocks[i].mat2 != null && blocks[i].mat2.Texture != null)
                    blocks[i].mat2.Texture.Dispose();
            }
        }

    }
}
