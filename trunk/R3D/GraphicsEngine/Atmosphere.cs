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
using R3D.IsoMap;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine
{
    public class Atmosphere
    {
        public delegate SkyBox SkyBoxLoadCallback(string name);

        SkyBox skyBox;
        MapAtmosphereInfo info;
        Device device;

        Light light;
        Light currentLight;

        FogMode fogMode;
        Color fogColor;
        int currentFogColor;

        float fogStart;
        float fogEnd;
        float fogDensity;

        float sunAngle;


        public float SunAngle
        {
            get { return sunAngle; }
            set { sunAngle = value; }
        }

        public Atmosphere(Device dev, MapAtmosphereInfo info,SkyBoxLoadCallback sblcbk)
        {
            device = dev;
            this.info = info;

            //shadowMap = new ShadowMap(dev);


            light.Ambient = info.ambientColor;
            light.Diffuse = info.diffuseColor;
            light.Specular = info.specularColor;
            light.Type = LightType.Directional;
            currentLight.Ambient = light.Ambient;
            currentLight.Diffuse = light.Diffuse;
            currentLight.Specular = light.Specular;
            currentLight.Type = LightType.Directional;


            sunAngle = 3 * MathEx.PIf / 4;
            light.Direction = new Vector3(-(float)Math.Cos(sunAngle), -(float)Math.Sin(sunAngle), 0f);
            currentLight.Direction = light.Direction;

            fogMode = info.fogMode;
            fogStart = info.fogStart;
            fogEnd = info.fogEnd;
            fogDensity = info.fogDensity;
            fogColor = Color.FromArgb(info.fogColor);
            currentFogColor = info.fogColor;

            if (info.HasSky)
            {
                skyBox = sblcbk(info.skyName);
            }
        }

        public float FogDensity
        {
            get { return fogDensity; }
            set { fogDensity = value; }
        }
        public float FogEnd
        {
            get { return fogEnd; }
            set { fogEnd = value; }
        }
        public float FogStart
        {
            get { return fogStart; }
            set { fogStart = value; }
        }
        public FogMode FogMode
        {
            get { return fogMode; }
            set { fogMode = value; }
        }
        public bool FogEnabled
        {
            get { return fogMode != FogMode.None; }
        }
        public int FogColor
        {
            get { return currentFogColor; }
            set { currentFogColor = value; }
        }

        public void Render()
        {
            if (skyBox != null)
            {
                skyBox.Render();
            }
            if (FogEnabled)
            {
                device.SetRenderState(RenderState.FogEnable, true);
                device.SetRenderState<FogMode>(RenderState.FogTableMode, fogMode);
                device.SetRenderState<FogMode>(RenderState.FogVertexMode, fogMode);

                device.SetRenderState(RenderState.FogStart, fogStart);
                device.SetRenderState(RenderState.FogEnd, fogEnd);
                device.SetRenderState(RenderState.FogColor, currentFogColor);
                device.SetRenderState(RenderState.FogDensity, fogDensity);
            }
            else
            {
                device.SetRenderState(RenderState.FogEnable, false);
            }
        }

        int MultiplyColor(ref Color clr, float bgn)
        {
            return (0xff << 24) | ((int)(clr.R * bgn) << 16) | ((int)(clr.G * bgn) << 8) | (int)(clr.B * bgn);
        }

        public void Update(float dt)
        {
            float angle = MathEx.Radian2Angle(sunAngle) % 360f;
            float sin = (float)Math.Sin(sunAngle);


            const float fadeRange = 25f;
            const float fadeRange2 = 10f;

            const float totalRange = fadeRange + fadeRange2;

            const float lowestBrightness = 0f;
            const float invLowestBrightness = 1 - lowestBrightness;
            const float lowestBrightness2 = 0.2f;

            if (info.HasDayNight)
            {
                float step = (dt / info.dayLength) * (MathEx.PIf * 2);

                sunAngle += step;

                if (angle > 180 + fadeRange2 && angle < 360 - fadeRange2)
                {
                    currentLight.Direction = new Vector3((float)Math.Cos(sunAngle), sin, 0.5f);
                }
                else
                {
                    currentLight.Direction = new Vector3(-(float)Math.Cos(sunAngle), -sin, -0.5f);
                }
                currentLight.Direction.Normalize();
            }

            if (sunAngle > 450)
            {
                sunAngle = 90;
            }

            //Color currFogClr = fogColor;


            if (skyBox != null)
            {
                if (angle >= 360 - fadeRange2 || angle <= fadeRange)
                {
                    if (angle > 90)
                    {
                        skyBox.DayNightLerpParam = (360 + fadeRange - angle) / totalRange;

                        float brightness = invLowestBrightness * (1f - (360 + fadeRange - angle) / totalRange) + lowestBrightness;
                        currentLight.Diffuse = light.Diffuse * brightness;
                        currentFogColor = MultiplyColor(ref fogColor, brightness);
                    }
                    else
                    {
                        skyBox.DayNightLerpParam = (fadeRange - angle) / totalRange;

                        float brightness = invLowestBrightness * (1f - (fadeRange - angle) / totalRange) + lowestBrightness;
                        currentLight.Diffuse = light.Diffuse * brightness;
                        currentFogColor = MultiplyColor(ref fogColor, brightness);
                    }
                }
                else if (angle >= 180 - fadeRange && angle <= fadeRange2 + 180)
                {
                    skyBox.DayNightLerpParam = 1f - ((180f + fadeRange2 - angle) / totalRange);

                    float brightness = invLowestBrightness * ((180f + fadeRange2 - angle) / totalRange) + lowestBrightness;
                    currentLight.Diffuse = light.Diffuse * brightness;
                    currentFogColor = MultiplyColor(ref fogColor, brightness);
                }
                else if (angle > fadeRange && angle < 180 - fadeRange)
                {
                    currentLight.Diffuse = light.Diffuse;
                    currentFogColor = fogColor.ToArgb();

                    skyBox.DayNightLerpParam = 0;
                }
                else if (angle > 180 + fadeRange2 && angle < 360 - fadeRange2)
                {
                    float extraDiffuse = (2 * lowestBrightness2) * Math.Abs(Math.Max(0f, ((angle - (180 + fadeRange2)) / (180 - fadeRange2 * 2) * 0.5f - 1f)));
                    float brightness = lowestBrightness + extraDiffuse;

                    currentLight.Diffuse = light.Diffuse * brightness;
                    currentFogColor = MultiplyColor(ref fogColor, brightness);
                    skyBox.DayNightLerpParam = 1;
                }


            }

            //currentFogColor = currFogClr.ToArgb();
        }

        public Light Light
        {
            get { return currentLight; }
        }
        public Vector3 LightDirection
        {
            get { return currentLight.Direction; }
        }
        //public ShadowMap ShadowMap
        //{
        //    get { return shadowMap; }
        //}

    }
}
