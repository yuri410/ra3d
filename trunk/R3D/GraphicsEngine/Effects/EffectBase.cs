using System;
using System.Collections.Generic;
using System.Text;
using Ra2Reload.GameResource;
using Ra2Reload.Graphics;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Reload.Graphics.Effects
{
    //[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    //public sealed class EffectNameAttribute : Attribute
    //{
    //    string name;

    //    // This is a positional argument
    //    public EffectNameAttribute(string name)
    //    {
    //        this.name = name;
    //    }

    //    public string Name
    //    {
    //        get { return name; }
    //        protected set
    //        {
    //            name = value;
    //        }
    //    }
    //}
    public delegate void RenderCallBack();

    /// <summary>
    /// 为所有渲染效果提供抽象标准
    /// </summary>
    public abstract class EffectBase : UniqueObject
    {
        //IRenderable target;

        //public IRenderable Target
        //{
        //    get { return target; }
        //    set { target = value; }
        //}

        protected EffectBase(string name)
            : base(name) { }

        public abstract void Render(RenderCallBack rcb);

    }

    public abstract class ShaderEffectBase : EffectBase, IDisposable
    {
        protected Dictionary<string, EffectHandle> effHandles;
        protected Effect effect;

        bool disposed;

        protected ShaderEffectBase(string name)
            : base(name )
        {
            effHandles = new Dictionary<string, EffectHandle>();
        }

        /// <summary>
        /// 一个典型的Render
        /// </summary>
        public override void Render(RenderCallBack rcb)
        {
            if (effect != null && rcb != null)
            {
                int passCount = effect.Begin(FX.DoNotSaveState);
                for (int i = 0; i < passCount; i++)
                {
                    effect.BeginPass(i);
                    //Target.Render();
                    rcb();
                    effect.EndPass();
                }
                effect.End();
            }
        }

        EffectHandle CreateEffectHandle(string name)
        {
            EffectHandle handle;
            handle = new EffectHandle(name);
            effHandles.Add(name, handle);
            return handle;
        }
        public void SetValue(string parameter, BaseTexture value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, bool value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, bool[] values)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, values);
        }
        public void SetValue(string parameter, Color4 value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, Color4[] values)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, values);
        }
        public void SetValue(string parameter, float value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, float[] values)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, values);
        }
        public void SetValue(string parameter, int value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, int[] values)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, values);
        }
        public void SetValue(string parameter, Matrix value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, Matrix[] values)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, values);
        }
        public void SetValue(string parameter, string value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, Vector4 value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, value);
        }
        public void SetValue(string parameter, Vector4[] values)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValue(handle, values);
        }
        public void SetValueTranspose(string parameter, Matrix value)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValueTranspose(handle, value);
        }
        public void SetValueTranspose(string parameter, Matrix[] values)
        {
            EffectHandle handle;
            if (!effHandles.TryGetValue(parameter, out handle))
            {
                handle = CreateEffectHandle(parameter);
            }
            effect.SetValueTranspose(handle, values);
        }


        protected override void dispose()
        {
            if (!disposed)
            {
                Dictionary<string, EffectHandle>.ValueCollection vals = effHandles.Values;
                foreach (EffectHandle e in vals)
                {
                    e.Dispose();
                }
                effHandles.Clear();

                effect.Dispose();
                disposed = true;
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }
    }
}
