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
    public class NormalMappingEffectFactory : ModelEffectFactory
    {
        static readonly string typeName = "NormalMapping";

        public static string Name
        {
            get { return typeName; }
        }

        Device device;

        public NormalMappingEffectFactory(Device dev)        
        {
            device = dev;
        }

        public override ModelEffect CreateInstance()
        {
            return new NormalMappingEffect(device);
        }

        public override void DestroyInstance(ModelEffect fx)
        {
            fx.Dispose();
        }
    }

    /// <summary>
    /// Represents a normal mapping effect    
    /// </summary>
    class NormalMappingEffect : ModelEffect
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
        EffectHandle tlParamNrmMap;
        EffectHandle tlParamClrMap;
        EffectHandle tlParamMVP;
        EffectHandle tlParamMV;

        bool stateSetted;


        public NormalMappingEffect(Device dev)
            : base(false, new PassType[1] { PassType.RenderObjects })
        {
            device = dev;

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CacheMix + "NormalMapping.fx", FileSystem.GameResLR);
            ArchiveStreamReader sr = new ArchiveStreamReader(fl);

            string code = sr.ReadToEnd();
            effect = Effect.FromString(dev, code, null, null, null, ShaderFlags.None, null);
            sr.Close();


            tlParamLa = new EffectHandle("I_a");
            tlParamLd = new EffectHandle("I_d");
            tlParamLs = new EffectHandle("I_s");
            tlParamKa = new EffectHandle("k_a");
            tlParamKd = new EffectHandle("k_d");
            tlParamKs = new EffectHandle("k_s");
            tlParamPwr = new EffectHandle("power");
            tlParamLdir = new EffectHandle("lightDirection");

            tlParamNrmMap = new EffectHandle("nmap");
            tlParamClrMap = new EffectHandle("cmap");

            tlParamVpos = new EffectHandle("viewerPos");
            tlParamMVP = new EffectHandle("mvp");
            tlParamMV = new EffectHandle("modelview");

            effect.Technique = new EffectHandle("NormalMapping");
        }

        public override Camera GetPassCamera(int passId)
        {
            return null;
        }

        protected override int begin()
        {
            //return 1;
            return effect.Begin(FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState);
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
                effect.SetValue(tlParamLa, light.Ambient);
                effect.SetValue(tlParamLd, light.Diffuse);
                effect.SetValue(tlParamLs, light.Specular);
                effect.SetValue(tlParamLdir, new float[3] { light.Direction.X, light.Direction.Y, light.Direction.Z });

                Vector3 pos = sceMgr.CurrentCamera.Position;
                effect.SetValue(tlParamVpos, new float[3] { pos.X, pos.Y, pos.Z });

                stateSetted = true;
            }

            Matrix world = device.GetTransform(TransformState.World);

            Matrix mv;
            Matrix.Multiply(ref world, ref sceMgr.CurrentCamera.Frustum.view, out mv);

            effect.SetValue(tlParamMVP, mv * sceMgr.CurrentCamera.Frustum.proj);
            effect.SetValue(tlParamMV, mv);



            effect.SetValue(tlParamKa, mat.mat.Ambient);
            effect.SetValue(tlParamKd, mat.mat.Diffuse);
            effect.SetValue(tlParamKs, mat.mat.Specular);
            effect.SetValue(tlParamPwr, mat.mat.Power);

            effect.SetTexture(tlParamNrmMap, mat.GetTexture(1));
            effect.SetTexture(tlParamClrMap, mat.GetTexture(0));
            
            effect.CommitChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                effect.Dispose();
            }
            effect = null;
        }
    }
}
