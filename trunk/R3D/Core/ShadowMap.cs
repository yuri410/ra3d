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
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Core
{
    public class ShadowMap
    {
        Effect effect;
        EffectHandle mvpParam;

        Texture shadowDepthMap;
        Texture shadowRt;

        Surface shadowRtSurface;
        Surface shadowDepSurface;

        public const int ShadowMapLength = 1024;

        Viewport smVp;
        Device device;
        //Texture shadowMap;
        //Camera smCamera;
        
        //Matrix TexTransform;
        //public Matrix ShadowTransform;

        EffectHandle genSMTec;
        //EffectHandle drawSceneTec;


        public ShadowMap(Device dev)
        {
            device = dev;

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.CacheMix + "HardwareShadowMap.fx", FileSystem.GameResLR);
            ArchiveStreamReader sr = new ArchiveStreamReader(fl);
            string code = sr.ReadToEnd();

            string err;
            effect = Effect.FromString(device, code, null, null, null, ShaderFlags.None, null, out err);
            sr.Close();

            mvpParam = new EffectHandle("mvp");

            genSMTec = new EffectHandle("GenerateShadowMap");
            effect.Technique = genSMTec;
            //drawSceneTec = new EffectHandle("RenderScene");


            shadowDepthMap = new Texture(device, ShadowMapLength, ShadowMapLength, 1, Usage.DepthStencil, Format.D24X8, Pool.Default);
            shadowRt = new Texture(device, ShadowMapLength, ShadowMapLength, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);

            smVp.MinZ = 0;
            smVp.MaxZ = 1;
            smVp.Height = ShadowMapLength;
            smVp.Width = ShadowMapLength;
            smVp.X = 0;
            smVp.Y = 0;


            shadowRtSurface = shadowRt.GetSurfaceLevel(0);
            shadowDepSurface = shadowDepthMap.GetSurfaceLevel(0);
        }


        Surface stdDepth;
        Surface stdRenderTarget;
        Viewport stdVp;

        public Matrix LightProjection;
        public Matrix ViewTransform;
        public Matrix ViewProj;

        public void End()
        {
            effect.EndPass();
            effect.End();

            device.DepthStencilSurface = stdDepth;
            stdDepth = null;
            device.SetRenderTarget(0, stdRenderTarget);
            stdRenderTarget = null;
            device.Viewport = stdVp;
        }
        public void Begin(Vector3 lightDir, Camera cam)
        {
            stdVp = device.Viewport;

            device.Viewport = smVp;

            stdDepth = device.DepthStencilSurface;
            stdRenderTarget = device.GetRenderTarget(0);


            device.SetRenderTarget(0, shadowRtSurface);
            device.DepthStencilSurface = shadowDepSurface;

            device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0, 1, 0);


            effect.Begin(FX.DoNotSaveSamplerState | FX.DoNotSaveShaderState | FX.DoNotSaveState);
            effect.BeginPass(0);
            float zFar = cam.FarPlane;

            Matrix.OrthoRH((float)ShadowMapLength / 4f, (float)ShadowMapLength / 4f, cam.NearPlane,  zFar, out LightProjection);

            Vector3 camPos = cam.Position;
            Vector3 up = cam.Front;


            Vector3 lightTarget = camPos;
            Vector3 offset = up;
            Vector3.Multiply(ref offset, 0.5f * zFar, out offset);

            Vector3.Add(ref lightTarget, ref offset, out lightTarget);


            Vector3 lightPos = lightTarget;
            offset = lightDir;
            Vector3.Multiply(ref offset, 0.5f * zFar, out offset);
            Vector3.Subtract(ref lightPos, ref offset, out lightPos);


            //Vector3 v1;
            //Vector3.Subtract(ref camPos, ref lPos, out v1);

            
            //Vector3.Cross(ref lightDir, ref v1, out up);
            //Vector3.Cross(ref lightDir, ref up, out up);
            up.Y = 0;
            if (up.LengthSquared() == 0)
            {
                up = new Vector3(0.707f, 0, 0.707f);
            }
            Matrix.LookAtRH(ref lightPos, ref lightTarget, ref up, out ViewTransform);




            ViewProj = ViewTransform * LightProjection;

            //Matrix proj = cam.Frustum.proj;
            //Matrix view = cam.Frustum.view;

            //device.SetTransform(TransformState.Projection, proj);
            //device.SetTransform(TransformState.View, view);


            //ShadowTransform = view * proj * TexTransform;
        }


        public void SetTransform(ref Matrix world)
        {
            effect.SetValue(mvpParam, world * ViewProj);
            effect.CommitChanges();
        }


        public Texture ShadowColorMap
        {
            get { return shadowRt; }
        }

        //public Texture ShadowTexture
        //{
        //    get { return shadowDepthMap; }
        //}
    }
}
