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
//using R3D.Graphics.Effects;
using R3D.IO;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;
using R3D.GraphicsEngine.Effects;

namespace R3D.GraphicsEngine
{
    [Flags()]
    public enum MaterialFlags
    {
        None = 0,
        RemapColor = 1
    }

    public abstract class MaterialBase
    {
        public const int MaxTexLayers = 4;

        static readonly string IsTransparentTag = "IsTransparent";
        static readonly string CullModeTag = "CullMode";

        bool isTransparent;
        Cull cullMode = Cull.None;
        
        //protected Device device;
        //EffectBase effect;


        public Cull CullMode
        {
            get { return cullMode; }
            set { cullMode = value; }
        }
        public bool IsTransparent
        {
            get { return isTransparent; }
            set { isTransparent = value; }
        }

        //public EffectBase Effect
        //{
        //    get { return effect; }
        //    set { effect = value; }
        //}
        protected virtual void ReadData(BinaryDataReader data)
        {
            cullMode = (Cull)data.GetDataInt32(CullModeTag, 0);
            isTransparent = data.GetDataBool(IsTransparentTag, false);
        }
        protected virtual void WriteData(BinaryDataWriter data)
        {
            data.AddEntry(CullModeTag, (int)cullMode);
            data.AddEntry(IsTransparentTag, isTransparent);
        }

    }

    public abstract class MeshMaterialBase<TexType> : MaterialBase, IDisposable where TexType : class
    {
        static readonly string MaterialColorTag = "MaterialColor";

        static readonly string IsTexEmbededTag = "IsEmbeded";
        static readonly string MaterialFlagTag = "Flags";
        static readonly string HasTextureTag = "HasTexture";
        static readonly string TextureTag = "Texture";
        static readonly string EffectTag = "Effect";


        public Material mat;
        protected string[] textureFiles = new string[MaxTexLayers];

        protected bool[] texEmbeded = new bool[MaxTexLayers];

        protected TexType[] textures = new TexType[MaxTexLayers];

        MaterialFlags flags;
        string effectName;
        bool disposed;

        public ModelEffect Effect
        {
            get; 
            protected set; 
        }

        //public string GetTextureFile(int idx)
        //{
        //    return textureFiles[idx];
        //}
        //public bool GetTextureEmbedded(int idx)
        //{
        //    return texEmbeded[idx];
        //}
        public TexType GetTexture(int idx)
        {
            return textures[idx];
        }
        public void SetTexture(int idx, TexType tex)
        {
            textures[idx] = tex;
        }
        public void SetEffect(ModelEffect eff)
        {
            Effect = eff;
        }
 
        public MaterialFlags Flags
        {
            get { return flags; }
            set { flags = value; }
        }

        protected abstract TexType LoadTexture(ArchiveBinaryReader br, bool isEmbeded, int index);
        protected abstract void DestroyTexture(TexType tex);

        protected abstract void SaveTexture(ArchiveBinaryWriter bw, TexType tex, bool isEmbeded, int index);
        protected abstract ModelEffect LoadEffect(string name);

        protected override void ReadData(BinaryDataReader data)
        {
            base.ReadData(data);

            ArchiveBinaryReader br;

            Flags = (MaterialFlags)data.GetDataInt32(MaterialFlagTag);

            br = data.GetData(MaterialColorTag);
            br.ReadMaterial(out mat);
            br.Close();


            br = data.GetData(EffectTag);
            effectName = br.ReadStringUnicode();
            if (effectName.Length == 0)
                effectName = StandardEffectFactory.Name;
            Effect = LoadEffect(effectName);
            
            br.Close();

            br = data.GetData(IsTexEmbededTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                texEmbeded[i] = br.ReadBoolean();
            }
            br.Close();

            bool[] hasTexture = new bool[4];
            br = data.GetData(HasTextureTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                hasTexture[i] = br.ReadBoolean();
            }
            br.Close();


            for (int i = 0; i < MaxTexLayers; i++)
            {
                if (hasTexture[i])
                {
                    br = data.GetData(TextureTag + i.ToString());
                    textures[i] = LoadTexture(br, texEmbeded[i], i);
                    br.Close();
                }
            }


            //if (textureCount > 0)
            //{
            //    br = sounds.GetData("Texture1");
            //    Texture1 = LoadTexture(br, Texture1Embeded, 1);
            //    br.Close();
            //    textureCount--;
            //}
            //if (textureCount > 0)
            //{
            //    br = sounds.GetData("Texture2");
            //    Texture2 = LoadTexture(br, Texture2Embeded, 2);
            //    br.Close();
            //    textureCount--;
            //}
            //if (textureCount > 0)
            //{
            //    br = sounds.GetData("Texture3");
            //    Texture3 = LoadTexture(br, Texture3Embeded, 3);
            //    br.Close();
            //    textureCount--;
            //}
            //if (textureCount > 0)
            //{
            //    br = sounds.GetData("Texture4");
            //    Texture4 = LoadTexture(br, Texture4Embeded, 4);
            //    br.Close();
            //    textureCount--;
            //}

        }
        protected override void WriteData(BinaryDataWriter data)
        {
            base.WriteData(data);
            

            data.AddEntry(MaterialFlagTag, (int)Flags);

            ArchiveBinaryWriter bw;

            bw = data.AddEntry(EffectTag);
            //if (Effect == null)
            //{
            bw.WriteStringUnicode(effectName);
            //}
            //else
            //{
            //    bw.WriteStringUnicode(Effect.Name);
            //}
            bw.Close();

            bw = data.AddEntry(MaterialColorTag);
            bw.Write(ref mat);
            bw.Close();


            bw = data.AddEntry(IsTexEmbededTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                bw.Write(texEmbeded[i]);
            }
            bw.Close();

            bw = data.AddEntry(HasTextureTag);
            for (int i = 0; i < MaxTexLayers; i++)
            {
                bw.Write(textures[i] != null || !string.IsNullOrEmpty(textureFiles[i]));
            }
            bw.Close();

            for (int i = 0; i < MaxTexLayers; i++)
            {
                if (textures[i] != null || !string.IsNullOrEmpty(textureFiles[i]))
                {
                    bw = data.AddEntry(TextureTag + i.ToString());
                    SaveTexture(bw, textures[i], texEmbeded[i], i);
                    bw.Close();
                }
            }
        }


        #region IDisposable 成员

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (Effect != null)
                {
                    Effect.Dispose();
                    Effect = null;
                }
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }
        public void Dispose()
        {
            if (!disposed)
            {
                if (Effect != null)
                {
                    Effect.Dispose();
                    Effect = null;
                }
                if (textures != null)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (textures[i] != null)
                        {
                            DestroyTexture(textures[i]);
                            //TextureManager.Instance.DestoryInstance(textures[i]);
                            textures[i] = null;
                        }
                    }
                    textures = null;
                }
                disposed = true;
            }
            else
                throw new ObjectDisposedException(ToString());
        }

        #endregion
    }

    public class MeshMaterial : MeshMaterialBase<Texture>, IComparable<MeshMaterial>
    {
        
        public readonly static Material DefaultMatColor;

        public static MeshMaterial DefaultMaterial
        {
            get;
            private set;
        }





        static MeshMaterial()
        {
            Color4 clr;
            clr.Alpha = 1;
            clr.Blue = 1;
            clr.Green = 1;
            clr.Red = 1;

            DefaultMatColor.Ambient = clr;
            DefaultMatColor.Diffuse = clr;
            DefaultMatColor.Power = 0;
            clr.Alpha = 0;
            clr.Red = 0;
            clr.Green = 0;
            clr.Blue = 0;
            DefaultMatColor.Emissive = clr;
            DefaultMatColor.Specular = clr;

            DefaultMaterial = new MeshMaterial(null);
            DefaultMaterial.CullMode = Cull.None;
            DefaultMaterial.mat = DefaultMatColor;            
        }

        protected Device device;
        public MeshMaterial(Device dev)
        {
            device = dev;
        }

        #region IComparable<MeshMaterial> 成员

        public int CompareTo(MeshMaterial other)
        {
            return this.GetHashCode().CompareTo(other.GetHashCode());
        }

        #endregion

        /// <summary>
        ///  重写以适应不同环境下的使用
        /// </summary>
        Texture LoadTexture(string fileName)
        {
            fileName = fileName.Trim();
            if (!string.IsNullOrEmpty(fileName))
            {
                FileLocation fl = FileSystem.Instance.TryLocate(fileName, FileSystem.GameCurrentResLR);
                if (fl != null)
                {
                    return TextureManager.Instance.CreateInstance(fl);// Texture.FromStream(device, fl.GetStream, Usage.None, Pool.Managed);
                }
                else
                {
                    GameConsole.Instance.Write("Texture: " + fileName + "is not found.", ConsoleMessageType.Warning);
                    return null;
                }
            }
            return null;
        }
        protected override Texture LoadTexture(ArchiveBinaryReader br, bool isEmbeded, int index)
        {
            if (isEmbeded)
            {
                return Texture.FromStream(device, br.BaseStream, Usage.None, Pool.Managed);
            }
            else
            {
                return LoadTexture(br.ReadStringUnicode());
            }
        }

        protected override void SaveTexture(ArchiveBinaryWriter bw, Texture tex, bool isEmbeded, int index)
        {
            throw new NotSupportedException();
        }

        protected override ModelEffect LoadEffect(string name)
        {
            return EffectManager.Instance.GetModelEffect(name);
        }
        public static MeshMaterial FromBinary(Device dev, BinaryDataReader data)
        {
            MeshMaterial res = new MeshMaterial(dev);

            res.ReadData(data);

            return res;
        }
        public static BinaryDataWriter ToBinary(MeshMaterial mat)
        {
            BinaryDataWriter data = new BinaryDataWriter();
            mat.WriteData(data);

            return data;
        }

        protected override void DestroyTexture(Texture tex)
        {
            TextureManager.Instance.DestoryInstance(tex);
        }
    }

}
