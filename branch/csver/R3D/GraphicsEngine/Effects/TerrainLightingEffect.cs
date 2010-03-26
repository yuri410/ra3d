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
using R3D.Core;
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine.Effects
{    
    public class TerrainLightingEffectFactory : ModelEffectFactory
    {
        static readonly string typeName = "TerrainLighting";

        public static string Name
        {
            get { return typeName; }
        }

        Device dev;

        public TerrainLightingEffectFactory(Device dev)
        {
            this.dev = dev;
        }

        public override ModelEffect CreateInstance()
        {
            return new TerrainLightingEffect(dev);
        }

        public override void DestroyInstance(ModelEffect fx)
        {
            fx.Dispose();
        }
    }

    class TerrainLightingEffect : ModelEffect
    {
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
        EffectHandle tlParamTex0;
        EffectHandle tlParamTex1;
        EffectHandle tlParamMVP;

        EffectHandle shadowMapParam;
        EffectHandle shadowMapTransform;

        bool stateSetted;

        public TerrainLightingEffect(Device dev)
            : base(false, new PassType[1] { PassType.RenderObjects })
        {
            device = dev;

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CacheMix + "TerrainLighting.fx", FileSystem.GameResLR);
            ArchiveStreamReader sr = new ArchiveStreamReader(fl);

            string code = sr.ReadToEnd();
            string err;
            effect = Effect.FromString(dev, code, null, null, null, ShaderFlags.OptimizationLevel3, null, out err);
            sr.Close();

            effect.Technique = new EffectHandle("TerrainLighting");

            tlParamLa = new EffectHandle("I_a");
            tlParamLd = new EffectHandle("I_d");
            tlParamLs = new EffectHandle("I_s");
            tlParamKa = new EffectHandle("k_a");
            tlParamKd = new EffectHandle("k_d");
            tlParamKs = new EffectHandle("k_s");
            tlParamPwr = new EffectHandle("power");
            tlParamLdir = new EffectHandle("lightDir");

            tlParamTex0 = new EffectHandle("normalMap");
            tlParamTex1 = new EffectHandle("colorMap");

            tlParamVpos = new EffectHandle("cameraPos");
            tlParamMVP = new EffectHandle("mvp");
            //tlParamMV = new EffectHandle("modelview");

            shadowMapParam = new EffectHandle("shadowMap");
            shadowMapTransform = new EffectHandle("smTrans");
        }

        public void SetMaterialAmbient(ref Color4 v)
        {
            effect.SetValue(tlParamKa, v);
        }
        public void SetMaterialDiffuse(ref Color4 v)
        {
            effect.SetValue(tlParamKd, v);
        }
        public void SetMaterialSpecular(ref Color4 v)
        {
            effect.SetValue(tlParamKs, v);
        }
        public void SetMaterialPower(float v)
        {
            effect.SetValue(tlParamPwr, v);
        }
        public void SetLightAmbient(ref Color4 v)
        {
            effect.SetValue(tlParamLa, v);
        }
        public void SetLightDiffuse(ref Color4 v)
        {
            effect.SetValue(tlParamLd, v);
        }
        public void SetLightSpecular(ref Color4 v)
        {
            effect.SetValue(tlParamLs, v);
        }
        public void SetLightDirection(ref Vector3 v)
        {
            effect.SetValue(tlParamLdir, new float[] { v.X, v.Y, v.Z });
        }

        public void SetNormalMap(BaseTexture v)
        {
            effect.SetTexture(tlParamTex0, v);
        }
        public void SetTerrainTexture(BaseTexture v)
        {
            effect.SetTexture(tlParamTex1, v);
        }

        public void SetViewerPosition(ref Vector3 v)
        {
            effect.SetValue(tlParamVpos, new float[] { v.X, v.Y, v.Z });
        }

        public void SetMVP(ref Matrix v)
        {
            effect.SetValue(tlParamMVP, v);
        }

        protected override int begin()
        {
            //return 1;
            stateSetted = false;
            return effect.Begin(FX.DoNotSaveState | FX.DoNotSaveShaderState | FX.DoNotSaveSamplerState);
        }

        protected override void end()
        {
            //return;
            effect.End();
        }

        public override void BeginPass(int passId)
        {
            //return;
            effect.BeginPass(passId);
        }

        public override void EndPass()
        {
            //return;
            effect.EndPass();
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

                tlParamTex0.Dispose();
                tlParamTex1.Dispose();

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

            tlParamTex0 = null;
            tlParamTex1 = null;

            tlParamVpos = null;
            tlParamMVP = null;

            shadowMapParam = null;
            shadowMapTransform = null;
        }


        public override void Setup(SceneManagerBase sceMgr, MeshMaterial mat, ref RenderOperation op)
        {
            //device.SetRenderState<FillMode>(RenderState.FillMode, FillMode.Wireframe);
            //device.SetRenderState(RenderState.Lighting, false);
            //device.SetTexture(0, null);
            //device.SetTexture(1, null);

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
                effect.SetValue(tlParamMVP, sceMgr.CurrentCamera.Frustum.view * sceMgr.CurrentCamera.Frustum.proj);

                //effect.SetValue(tlParamTexTrans, sceMgr.ShadowMap.ShadowTransform);
                //effect.SetValue(tlParamShadowMap, sceMgr.ShadowMap.ShadowColorMap);

                effect.SetTexture(shadowMapParam, sceMgr.ShadowMap.ShadowColorMap);
                effect.SetValue(shadowMapTransform, sceMgr.ShadowMap.ViewProj);

                stateSetted = true;
            }
            effect.SetValue(tlParamKa, mat.mat.Ambient);
            effect.SetValue(tlParamKd, mat.mat.Diffuse);
            effect.SetValue(tlParamKs, mat.mat.Specular);
            effect.SetValue(tlParamPwr, mat.mat.Power);

            effect.SetTexture(tlParamTex0, mat.GetTexture(0));
            effect.SetTexture(tlParamTex1, mat.GetTexture(1));


            //SetMaterialAmbient(ref mat.mat.Ambient);
            //SetMaterialDiffuse(ref mat.mat.Diffuse);
            //SetMaterialPower(mat.mat.Power);
            //SetMaterialSpecular(ref mat.mat.Specular);

            //SetNormalMap(mat.GetTexture(0));
            //SetTerrainTexture(mat.GetTexture(1));

            effect.CommitChanges();
        }
        public override Camera GetPassCamera(int passId)
        {
            return null;
        }
    }
}
