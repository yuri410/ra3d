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
using System.Text;
using R3D.Core;
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine.Effects
{
    public class TreeEffectFactory : ModelEffectFactory
    {
        static readonly string typeName = "Tree";


        public static string Name
        {
            get { return typeName; }
        }

        Device device;

        public TreeEffectFactory(Device dev)
        {
            device = dev;
        }

        public override ModelEffect CreateInstance()
        {
            return new TreeEffect(device);
        }

        public override void DestroyInstance(ModelEffect fx)
        {
            fx.Dispose();
        }
    }

    class TreeEffect : ModelEffect
    {
        bool stateSetted;

        Device device;

        Effect effect;
        EffectHandle tlParamLa;
        EffectHandle tlParamLd;
        EffectHandle tlParamLs;
        EffectHandle tlParamKa;
        EffectHandle tlParamKd;
        EffectHandle tlParamKs;
        EffectHandle tlParamPwr;
        EffectHandle tlParamLdir;
        EffectHandle tlParamVpos;
        EffectHandle tlParamClrMap;
        EffectHandle tlParamMVP;

        EffectHandle tlParamWorldT;
        EffectHandle shadowMapParam;
        EffectHandle shadowMapTransform;

        Texture noTexture;

        public unsafe TreeEffect(Device dev)
            : base(false, new PassType[1] { PassType.RenderObjects })
        {
            device = dev;

            noTexture = new Texture(dev, 1, 1, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);
            *((int*)noTexture.LockRectangle(0, LockFlags.None).Data.DataPointer.ToPointer()) = Color.Gray.ToArgb();
            noTexture.UnlockRectangle(0);

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CacheMix + "Tree.fx", FileSystem.GameResLR);
            ArchiveStreamReader sr = new ArchiveStreamReader(fl);

            string code = sr.ReadToEnd();
            string err;
            effect = Effect.FromString(dev, code, null, null, null, ShaderFlags.None, null, out err);
            sr.Close();

            effect.Technique = new EffectHandle("Tree");

            tlParamLa = new EffectHandle("I_a");
            tlParamLd = new EffectHandle("I_d");
            tlParamLs = new EffectHandle("I_s");
            tlParamKa = new EffectHandle("k_a");
            tlParamKd = new EffectHandle("k_d");
            tlParamKs = new EffectHandle("k_s");
            tlParamPwr = new EffectHandle("power");
            tlParamLdir = new EffectHandle("lightDir");

            tlParamClrMap = new EffectHandle("clrMap");

            tlParamVpos = new EffectHandle("cameraPos");
            tlParamMVP = new EffectHandle("mvp");


            tlParamWorldT = new EffectHandle("worldTrans");
            shadowMapParam = new EffectHandle("shadowMap");
            shadowMapTransform = new EffectHandle("smTrans");
        }

        public override Camera GetPassCamera(int passId)
        {
            return null;
        }

        protected override int begin()
        {
            stateSetted = false;
            return effect.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);
        }

        protected override void end()
        {
            effect.End();
        }

        public override void BeginPass(int passId)
        {
            effect.BeginPass(passId);
        }

        public override void EndPass()
        {
            effect.EndPass();
        }

        public override void Setup(SceneManagerBase sceMgr, MeshMaterial mat, ref RenderOperation op)
        {
            //device.SetRenderState<FillMode>(RenderState.FillMode, FillMode.Wireframe);
            //device.SetRenderState(RenderState.Lighting, false);
            //device.SetTexture(0, null);
            //device.SetTexture(1, null);
            device.SetRenderState(RenderState.AlphaTestEnable, true);
            device.SetRenderState<Compare>(RenderState.AlphaFunc, Compare.Greater);
            device.SetRenderState(RenderState.AlphaRef, 128);
            //return;
            if (!stateSetted)
            {
                Light light = sceMgr.Atmosphere.Light;
                Vector3 lightDir = light.Direction;
                effect.SetValue(tlParamLa, light.Ambient);
                effect.SetValue(tlParamLd, light.Diffuse);
                effect.SetValue(tlParamLs, light.Specular);
                effect.SetValue(tlParamLdir, new float[3] { lightDir.X, lightDir.Y, lightDir.Z });

                Vector3 pos = sceMgr.CurrentCamera.Position;
                effect.SetValue(tlParamVpos, new float[3] { pos.X, pos.Y, pos.Z });

                effect.SetTexture(shadowMapParam, sceMgr.ShadowMap.ShadowColorMap);

                stateSetted = true;
            }
            effect.SetValue(tlParamKa, mat.mat.Ambient);
            effect.SetValue(tlParamKd, mat.mat.Diffuse);
            effect.SetValue(tlParamKs, mat.mat.Specular);
            effect.SetValue(tlParamPwr, mat.mat.Power);

            Texture clrTex = mat.GetTexture(0);
            if (clrTex == null)
                clrTex = noTexture;
            effect.SetTexture(tlParamClrMap, clrTex);

            effect.SetValue(tlParamMVP, op.Transformation * sceMgr.CurrentCamera.Frustum.view * sceMgr.CurrentCamera.Frustum.proj);

            Matrix lightPrjTrans;
            Matrix.Multiply(ref op.Transformation, ref sceMgr.ShadowMap.ViewProj, out lightPrjTrans);

            effect.SetValue(shadowMapTransform, lightPrjTrans);
            effect.SetValue(tlParamWorldT, op.Transformation);

            effect.CommitChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                effect.Dispose();

                tlParamLa.Dispose();
                tlParamLd.Dispose();
                tlParamLs.Dispose();
                tlParamKa.Dispose();
                tlParamKd.Dispose();
                tlParamKs.Dispose();
                tlParamPwr.Dispose();
                tlParamLdir.Dispose();

                tlParamClrMap.Dispose();
                tlParamWorldT.Dispose();

                tlParamVpos.Dispose();
                tlParamMVP.Dispose();

                shadowMapParam.Dispose();
                shadowMapTransform.Dispose();
            }
            effect = null;
            tlParamLa = null;
            tlParamLd = null;
            tlParamLs = null;
            tlParamKa = null;
            tlParamKd = null;
            tlParamKs = null;
            tlParamPwr = null;
            tlParamLdir = null;

            tlParamClrMap = null;
            tlParamWorldT = null;

            tlParamVpos = null;
            tlParamMVP = null;

            shadowMapParam = null;
            shadowMapTransform = null;
        }
    }
}
