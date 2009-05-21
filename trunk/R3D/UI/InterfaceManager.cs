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
using R3D.UI.Controls;
using SlimDX.DirectInput;
using SlimDX.Direct3D9;
using System.Windows.Forms;

namespace R3D.UI
{
    public abstract class InterfaceManager : IDisposable
    {
        StateBlock stateBlock;

        protected Device Device
        {
            get;
            private set;
        }

        public GameUI GameUI
        {
            get;
            private set;
        }



        protected InterfaceManager(GameUI gameUI)
        {
            GameUI = gameUI;
            Device = gameUI.Device;

            stateBlock = new StateBlock(Device, StateBlockType.All);
        }

        public abstract void InvokeKeyPressed(KeyPressEventArgs e);

        public abstract void InvokeKeyStateChanged(KeyCollection pressed);

        public abstract void InvokeMouseDown(MouseEventArgs e);

        public abstract void InvokeMouseMove(MouseEventArgs e);

        public abstract void InvokeMouseUp(MouseEventArgs e);

        public abstract void InvokeMouseWheel(MouseEventArgs e);

        public abstract void Load();

        protected abstract void paintUI(Sprite spr);

        /// <summary>
        /// Paints the interface.
        /// </summary>
        public void PaintUI(Sprite spr)
        {
   
            // store the current state
            stateBlock.Capture();

            // clear shaders
            Device.VertexShader = null;
            Device.PixelShader = null;


            // set common render states
            Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);
            Device.SetRenderState(RenderState.AlphaTestEnable, false);
            Device.SetRenderState(RenderState.SeparateAlphaBlendEnable, false);
            Device.SetRenderState(RenderState.BlendOperation, BlendOperation.Add);
            Device.SetRenderState(RenderState.ColorWriteEnable, ColorWriteEnable.All);
            Device.SetRenderState(RenderState.ShadeMode, ShadeMode.Gouraud);
            Device.SetRenderState(RenderState.FogEnable, false);
            Device.SetRenderState(RenderState.ZWriteEnable, false);
            Device.SetRenderState(RenderState.FillMode, FillMode.Solid);
            Device.SetRenderState(RenderState.CullMode, Cull.Counterclockwise);
            Device.SetRenderState(RenderState.ZEnable, false);

            // set common texture stage states
            Device.SetTextureStageState(0, TextureStage.ResultArg, TextureArgument.Current);
            Device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Disable);
            Device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Disable);

            // set common sampler states
            Device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Point);
            Device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Point);

            paintUI(spr);

            stateBlock.Apply();

        }

        public abstract void Unload();

        /// <summary>
        /// Updates the interface.
        /// </summary>
        /// <param name="dt">The elapsed time.</param>
        public abstract void Update(float dt);


        public bool Disposed
        {
            get;
            private set;
        }

        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                stateBlock.Dispose();
            }
            stateBlock = null;
            Device = null;
            GameUI = null;
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        
        ~InterfaceManager()
        {
            if (!Disposed)
                Dispose();
        }
    }
}
