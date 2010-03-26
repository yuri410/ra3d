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
using R3D.Base;
using R3D.IO;

namespace R3D.GraphicsEngine.Effects
{
    public class EffectManager
    {
        static EffectManager singleton;

        /// <summary>
        ///   Gets the singleton instance of this class.
        /// </summary>
        public static EffectManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new EffectManager();
                return singleton;
            }
        }

        private EffectManager()
        {
            Enabled = true;
        }

        Dictionary<string, ModelEffect> modelFX = new Dictionary<string, ModelEffect>(CaseInsensitiveStringComparer.Instance);
        
        Dictionary<string, PostEffect> postFX = new Dictionary<string, PostEffect>(CaseInsensitiveStringComparer.Instance);


        Dictionary<string, ModelEffectFactory> modelFXFac = new Dictionary<string, ModelEffectFactory>(CaseInsensitiveStringComparer.Instance);

        Dictionary<string, PostEffectFactory> postFXFac = new Dictionary<string, PostEffectFactory>(CaseInsensitiveStringComparer.Instance);


        public void RegisterModelEffectType(string name, ModelEffectFactory fac)
        {
            modelFXFac.Add(name, fac);
        }

        public void RegisterPostEffectType(string name, PostEffectFactory fac)
        {
            postFXFac.Add(name, fac);
        }

        public void UnregisterModelEffectType(string name)
        {
            modelFXFac.Remove(name);
        }

        public void UnregisterPostEffectType(string name)
        {
            postFXFac.Remove(name);
        }


        public ModelEffect GetModelEffect(string name)
        {
            return Enabled ? modelFX[name] : null;
        }

        public PostEffect GetPostEffect(string name)
        {
            return Enabled ? postFX[name] : null;
        }

        public int ModelEffectCount
        {
            get { return modelFX.Count; }
        }

        public int PostEffectCount
        {
            get { return postFX.Count; }
        }

        public bool HasModelEffect(string name)
        {
            return modelFX.ContainsKey(name);
        }

        public bool HasPostEffect(string name)
        {
            return postFX.ContainsKey(name);
        }

        public void LoadEffects()
        {
            foreach (KeyValuePair<string, ModelEffectFactory> e in modelFXFac)
            {
                modelFX.Add(e.Key, e.Value.CreateInstance());
            }
            foreach (KeyValuePair<string, PostEffectFactory> e in postFXFac)
            {
                postFX.Add(e.Key, e.Value.CreateInstance());
            }
        }

        public void UnloadEffects()
        {
            foreach (KeyValuePair<string, ModelEffect> e in modelFX)
            {
                modelFXFac[e.Key].DestroyInstance(e.Value);
            }
            foreach (KeyValuePair<string, PostEffect> e in postFX)
            {
                postFXFac[e.Key].DestroyInstance(e.Value);
            }
            modelFX.Clear();
            postFX.Clear();
        }

        public bool Enabled
        {
            get;
            set;
        }
    }
}
