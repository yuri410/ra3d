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
using SlimDX.Direct3D9;
using R3D.ConfigModel;
using R3D.IO;
using SlimDX;
using R3D.MathLib;

namespace R3D.GraphicsEngine
{
    public class SkyBox : IConfigurable, IDisposable
    {
        struct SkyVertex
        {
            public Vector3 pos;
            public Vector3 texCoord;

            public static VertexFormat Format
            {
                get { return (VertexFormat)((int)VertexFormat.Position | (int)VertexFormat.Texture1 | Utils.GetTexCoordSize3Format(0)); }
            }
        }

        bool disposed;

        CubeTexture dayTex;
        CubeTexture nightTex;

        VertexBuffer box;
        IndexBuffer indexBuffer;

        VertexDeclaration vtxDecl;

        Device device;

        Effect effect;
        EffectHandle nightAlpha;
        EffectHandle day;
        EffectHandle night;

        public unsafe SkyBox(Device dev)
        {
            device = dev;

            // sqrt(3)/3
            const float l = 1f / MathEx.Root3;

            vtxDecl = new VertexDeclaration(device, D3DX.DeclaratorFromFVF(SkyVertex.Format));
            box = new VertexBuffer(dev, sizeof(SkyVertex) * 8, Usage.WriteOnly, VertexPT1.Format, Pool.Managed);

            SkyVertex* dst = (SkyVertex*)box.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            //dst[0] = new SkyVertex { pos = new Vector3(-0.5f, -0.5f, -0.5f), texCoord = new Vector3(-l, -l, -l) };
            //dst[1] = new SkyVertex { pos = new Vector3(0.5f, -0.5f, -0.5f), texCoord = new Vector3(l, -l, -l) };
            //dst[2] = new SkyVertex { pos = new Vector3(-0.5f, -0.5f, 0.5f), texCoord = new Vector3(-l, -l, l) };
            //dst[3] = new SkyVertex { pos = new Vector3(0.5f, -0.5f, 0.5f), texCoord = new Vector3(l, -l, l) };
            //dst[4] = new SkyVertex { pos = new Vector3(-0.5f, 0.5f, -0.5f), texCoord = new Vector3(-l, l, -l) };
            //dst[5] = new SkyVertex { pos = new Vector3(0.5f, 0.5f, -0.5f), texCoord = new Vector3(l, l, -l) };
            //dst[6] = new SkyVertex { pos = new Vector3(-0.5f, 0.5f, 0.5f), texCoord = new Vector3(-l, l, l) };
            //dst[7] = new SkyVertex { pos = new Vector3(0.5f, 0.5f, 0.5f), texCoord = new Vector3(l, l, l) };
            dst[0] = new SkyVertex { pos = new Vector3(-50f, -50f, -50f), texCoord = new Vector3(-l, -l, -l) };
            dst[1] = new SkyVertex { pos = new Vector3(50f, -50f, -50f), texCoord = new Vector3(l, -l, -l) };
            dst[2] = new SkyVertex { pos = new Vector3(-50f, -50f, 50f), texCoord = new Vector3(-l, -l, l) };
            dst[3] = new SkyVertex { pos = new Vector3(50f, -50f, 50f), texCoord = new Vector3(l, -l, l) };
            dst[4] = new SkyVertex { pos = new Vector3(-50f, 50f, -50f), texCoord = new Vector3(-l, l, -l) };
            dst[5] = new SkyVertex { pos = new Vector3(50f, 50f, -50f), texCoord = new Vector3(l, l, -l) };
            dst[6] = new SkyVertex { pos = new Vector3(-50f, 50f, 5f), texCoord = new Vector3(-l, l, l) };
            dst[7] = new SkyVertex { pos = new Vector3(50f, 50, 50f), texCoord = new Vector3(l, l, l) };

            box.Unlock();

            indexBuffer = new IndexBuffer(dev, sizeof(ushort) * 36, Usage.WriteOnly, Pool.Managed, true);

            ushort* ibDst = (ushort*)indexBuffer.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            ibDst[0] = 0;
            ibDst[1] = 1;
            ibDst[2] = 3;

            ibDst[3] = 0;
            ibDst[4] = 2;
            ibDst[5] = 3;


            ibDst[6] = 4;
            ibDst[7] = 5;
            ibDst[8] = 7;

            ibDst[9] = 4;
            ibDst[10] = 6;
            ibDst[11] = 7;



            ibDst[12] = 0;
            ibDst[13] = 1;
            ibDst[14] = 4;

            ibDst[15] = 1;
            ibDst[16] = 4;
            ibDst[17] = 5;


            ibDst[18] = 0;
            ibDst[19] = 4;
            ibDst[20] = 2;

            ibDst[21] = 4;
            ibDst[22] = 6;
            ibDst[23] = 2;


            ibDst[24] = 1;
            ibDst[25] = 3;
            ibDst[26] = 5;

            ibDst[27] = 5;
            ibDst[28] = 7;
            ibDst[29] = 3;


            ibDst[30] = 2;
            ibDst[31] = 3;
            ibDst[32] = 6;

            ibDst[33] = 2;
            ibDst[34] = 7;
            ibDst[35] = 3;

            indexBuffer.Unlock();


            
            


            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CacheMix + "DayNight.fx", FileSystem.GameResLR);
            ArchiveStreamReader sr = new ArchiveStreamReader(fl);
            string code = sr.ReadToEnd();
            string err;
            effect = Effect.FromString(device, code, null, null, null, ShaderFlags.None, null, out err);

            effect.Technique = new EffectHandle("DayNight");

            nightAlpha = new EffectHandle("nightAlpha");

            day = new EffectHandle("Day");
            night = new EffectHandle("Night");

        }

        public float DayNightLerpParam
        {
            get;
            set;
        }

        public unsafe void Render()
        {
            if (dayTex != null)
            {
                Matrix view = device.GetTransform(TransformState.View);
                Matrix oldView = view;
                view.M41 = 0;
                view.M42 = 0;
                view.M43 = 0;

                device.SetTransform(TransformState.View, view);

                int passCount = effect.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);


                effect.SetTexture(day, dayTex);
                effect.SetTexture(night, nightTex);
                effect.SetValue(nightAlpha, DayNightLerpParam);
                effect.CommitChanges();
      
                for (int i = 0; i < passCount; i++)
                {
                    effect.BeginPass(i);

                    device.SetRenderState(RenderState.ZEnable, false);
                    device.SetRenderState(RenderState.ZWriteEnable, false);
                    device.SetRenderState<Cull>(RenderState.CullMode, Cull.None);

                    device.SetStreamSource(0, box, 0, sizeof(SkyVertex));
                    device.VertexFormat = SkyVertex.Format;
                    device.VertexDeclaration = vtxDecl;

                    device.Indices = indexBuffer;
                    device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);

                    device.SetRenderState(RenderState.ZEnable, true);
                    device.SetRenderState(RenderState.ZWriteEnable, true);

                    device.SetTransform(TransformState.View, oldView);

                    effect.EndPass();
                }

                effect.End();
            }
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            string dayTexFile = sect.GetString("DayTexture", null);
            if (dayTexFile != null)
            {
                FileLocation fl = FileSystem.Instance.Locate(dayTexFile, FileSystem.GameCurrentResLR);

                dayTex = CubeTexture.FromStream(device, fl.GetStream, Usage.None, Pool.Managed);
            }

            string nightTexFile = sect.GetString("NightTexture", null);
            if (nightTexFile != null)
            {
                FileLocation fl = FileSystem.Instance.Locate(nightTexFile, FileSystem.GameCurrentResLR);

                nightTex = CubeTexture.FromStream(device, fl.GetStream, Usage.None, Pool.Managed);
            }
        }

        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            if (disposed)
            {
                if (dayTex != null)
                {
                    dayTex.Dispose();
                    dayTex = null;
                }
                if (nightTex != null)
                {
                    nightTex.Dispose();
                    nightTex = null;
                }
                indexBuffer.Dispose();
                box.Dispose();

                disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }
}
