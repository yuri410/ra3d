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
using System.Drawing.Design;
using System.IO;
using System.Text;
using Ra2Develop.Designers;
using R3D;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.IsoMap;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Develop.Editors.EditableObjects
{
    [TypeConverter(typeof(BlockMaterialConverter))]
    public class EditableBlockMaterial : BlockMaterialBase<Texture>, IDisposable
    {
        ImageBase detailedTexture;

        EditObjChangedHandler changed;
        void Changed()
        {
            if (changed != null)
                changed();
        }
        [Browsable(false)]
        public event EditObjChangedHandler StateChanged
        {
            add { changed += value; }
            remove { changed -= value; }
        }


        [Editor(typeof(TextureEditor), typeof(UITypeEditor))]
        [LocalizedDescription("PROP:MaterialTexture")]
        public override Texture Texture
        {
            get
            {
                return hasTexture ? texture : null;
            }
            set
            {
                if (value == null)
                {
                    texture.Dispose();
                    texture = null;

                    hasTexture = false;
                    Changed();
                }
                else
                {
                    if (texture != value && texture != null)
                    {
                        texture.Dispose();
                        Changed();
                    }
                    texture = value;
                    hasTexture = true;
                }                 
            }
        }

        [Editor(typeof(ImageEditor), typeof(UITypeEditor))]
        [LocalizedDescription("PROP:MaterialTexture")]
        public ImageBase DetailedTexture
        {
            get { return detailedTexture; }
            set
            {
                if (value == null)
                {
                    detailedTexture.Dispose();
                    detailedTexture = null;

                    hasDetailedTex = false;
                    Changed();
                }
                else
                {
                    if (detailedTexture != value && detailedTexture != null)
                    {
                        detailedTexture.Dispose();
                        Changed();
                    }
                    detailedTexture = value;
                    hasDetailedTex = true;
                }
            }
        }


        //[LocalizedDescription("PROP:MaterialPower")]
        //public float Power
        //{
        //    get { return mat.Power; }
        //    set
        //    {
        //        mat.Power = value;
        //        Changed();
        //    }
        //}

        //[Editor(typeof(Color4Editor), typeof(UITypeEditor))]
        //[LocalizedDescription("PROP:MaterialAmbient")]
        //public Color4 Ambient
        //{
        //    get { return mat.Ambient; }
        //    set
        //    {
        //        mat.Ambient = value;
        //        Changed();
        //    }
        //}

        //[Editor(typeof(Color4Editor), typeof(UITypeEditor))]
        //[LocalizedDescription("PROP:MaterialDiffuse")]
        //public Color4 Diffuse
        //{
        //    get { return mat.Diffuse; }
        //    set
        //    {
        //        mat.Diffuse = value;
        //        Changed();
        //    }
        //}

        //[Editor(typeof(Color4Editor), typeof(UITypeEditor))]
        //[LocalizedDescription("PROP:MaterialSpecular")]
        //public Color4 Specular
        //{
        //    get { return mat.Specular; }
        //    set
        //    {
        //        mat.Specular = value; 
        //        Changed();
        //    }
        //}

        //[Editor(typeof(Color4Editor), typeof(UITypeEditor))]
        //[LocalizedDescription("PROP:MaterialEmissive")]
        //public Color4 Emissive
        //{
        //    get { return mat.Emissive; }
        //    set
        //    {
        //        mat.Emissive = value;
        //        Changed();
        //    }
        //}

        public override string ToString()
        {
            return Program.StringTable["PROP:Material"];
        }

        protected override Texture LoadTexture(ArchiveBinaryReader br)
        {
            ImageBase img = br.ReadEmbededImage();
            Texture tex = img.GetTexture(GraphicsDevice.Instance.Device, Usage.None, Pool.Managed);

            br.Close();
            return tex;
        }
        protected override void SaveTexture(Texture tex, ArchiveBinaryWriter bw)
        {
            ImageBase img = RawImage.FromTexture(tex);
            bw.Write(img);
            bw.Close();
        }

        
        public static EditableBlockMaterial FromBinary(BinaryDataReader data)
        {
            EditableBlockMaterial res = new EditableBlockMaterial();

            res.ReadData(data);
            if (res.hasDetailedTex)
            {
                ArchiveBinaryReader br = data.GetData(DetailedTextureTag);

                res.detailedTexture = br.ReadEmbededImage();

                br.Close();
            }

            return res;
        }
        public static BinaryDataWriter ToBinary(EditableBlockMaterial mat)
        {
            BinaryDataWriter data = new BinaryDataWriter();

            mat.hasDetailedTex = mat.detailedTexture != null;

            mat.WriteData(data);
            if (mat.hasDetailedTex)
            {
                ArchiveBinaryWriter bw = data.AddEntry(DetailedTextureTag); // br = sounds.GetData(DetailedTextureTag);

                bw.Write(mat.detailedTexture);
                bw.Close();
            }

            return data;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (detailedTexture != null)
                detailedTexture.Dispose();
            if (texture != null)
                texture.Dispose();
        }

        #endregion
    }
}
