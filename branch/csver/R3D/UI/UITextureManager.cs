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
using R3D.Logic;
using R3D.Media;
using SlimDX.Direct3D9;

namespace R3D.UI
{
    public abstract class UITextureManagerBase
    {
        protected class InstanceEntry : IDisposable
        {
            string name;

            WeakReference target;

            public InstanceEntry(string name, Texture obj)
            {
                this.name = name;
                this.target = new WeakReference(obj);
            }

            public string Name
            {
                get { return name; }
            }

            public Texture Object
            {
                get { return (Texture)target.Target; }
            }
            public object RawObject
            {
                get { return target.Target; }
            }
            public bool IsAlive
            {
                get { return target.IsAlive; }
            }

            #region IDisposable 成员

            public void Dispose()
            {
                target = null;
            }

            #endregion

            //public override int GetHashCode()
            //{
            //    return hash;
            //}
        }
        protected Dictionary<string, InstanceEntry> instances = new Dictionary<string, InstanceEntry>();
        protected Dictionary<int, InstanceEntry> instancesByHash = new Dictionary<int, InstanceEntry>();

        public PaletteCache PaletteCache
        {
            get;
            set;
        }

    }

    public class CameoManager : UITextureManagerBase
    {
        static CameoManager singleton;


        public static void Initialize(Device dev)
        {
            if (singleton != null)
            {
                throw new InvalidOperationException();
            }
            singleton = new CameoManager(dev);
        }
        public static CameoManager Instance
        {
            get
            {
                return singleton;
            }
        }

        Device device;
        FileLocation missIconFl;

        private CameoManager(Device dev)
        {
            device = dev;
            PaletteCache = new PaletteCache(10);

            missIconFl = FileSystem.Instance.Locate(new string[] { FileSystem.Cameo_Mix + "xxicon.shp", FileSystem.CameoOld_Mix + "xxicon.shp" }, FileSystem.GameResLR);
        }

        ImageBase LoadCameoPal(FileLocation fl, CameoInfo info)
        {
            ShpAnim shp = AnimManager.Instance.CreateInstance(fl) as ShpAnim;

            if (shp == null)
            {
                shp.Dispose();
                throw new NotSupportedException();
            }

            ResourceLocation pall = FileSystem.Instance.Locate(info.FilePalettes, info.LocateRule);

            shp.Palette = PaletteCache.CreatePalette(pall);

            ImageBase res = shp.GetImage(0);
            if (info.HasTransparentColor)
            {
                res.MakeTransparent(shp.Palette.Data[info.PaletteTransparentIndex]);
            }
            return res;
        }

        ImageBase LoadCameo(FileLocation fl, CameoInfo info)
        {
            ImageBase res = ImageManager.Instance.CreateInstance(fl);
            if (info.HasTransparentColor)
                res.MakeTransparent(info.TransparentColor);
            return res;
        }

        Texture GetTexture(string name, FileLocation fl, CameoInfo info)
        {
            Texture texCam = null;

            InstanceEntry entry;
            if (instances.TryGetValue(name, out entry))
            {
                if (entry.IsAlive)
                {
                    texCam = entry.Object;
                }
                else
                {
                    instances.Remove(name);
                    entry.Dispose();
                }
            }

            if (texCam == null)
            {
                ImageBase imgCam;
                if (info.UsePalette)
                {
                    imgCam = LoadCameoPal(fl,info);
                }
                else
                {
                    imgCam = LoadCameo(fl, info);
                }
                texCam = imgCam.GetTexture(device, Usage.None, Pool.Managed);
                imgCam.Dispose();

                InstanceEntry inst = new InstanceEntry(name, texCam);
                instances.Add(name, inst);
                instancesByHash.Add(texCam.GetHashCode(), inst);
            }
            return texCam;
        }

        public Texture[] CreateInstance(CameoInfo info)
        {
            FileLocation fl;
            FileLocation afl;
            string name;
            string altName;

            if (info.UsePalette)
            {
                fl = FileSystem.Instance.TryLocate(info.FileNames, info.LocateRule);
                if (fl == null)
                    fl = missIconFl;
                name = fl.Name;

                afl = FileSystem.Instance.TryLocate(info.AltFileNames, info.LocateRule);
                if (afl == null)
                    afl = missIconFl;
                altName = afl.Name;
            }
            else
            {
                fl = FileSystem.Instance.TryLocate(info.FileNames, info.LocateRule);
                if (fl == null)
                    fl = missIconFl; 
                name = fl.Name;

                afl = FileSystem.Instance.TryLocate(info.AltFileNames, info.LocateRule);
                if (afl == null)
                    afl = missIconFl;
                altName = afl.Name;
            }


            Texture texCam = GetTexture(name, fl, info);
            Texture texCamAlt = GetTexture(altName, afl, info);


            return new Texture[2] { texCam, texCamAlt };
        }
        public void DestoryInstance(Texture obj)
        {
            InstanceEntry entry = null;

            int hash = obj.GetHashCode();
            if (instancesByHash.TryGetValue(hash, out entry))
            {
                instances.Remove(entry.Name);
                instancesByHash.Remove(hash);
                entry.Dispose();
            }

            obj.Dispose();
        }
    }

    public class UITextureManager : UITextureManagerBase
    {
        static UITextureManager singleton;

        public static void Initialize(Device dev)
        {
            if (singleton != null)
            {
                throw new InvalidOperationException();
            }
            singleton = new UITextureManager(dev);
        }
        public static UITextureManager Instance
        {
            get
            {
                return singleton;
            }
        }

        Device device;

        private UITextureManager(Device dev)
        {
            device = dev;

            CreationUsage = Usage.None;
            CreationPool = Pool.Managed;
        }

        public Usage CreationUsage
        {
            get;
            set;
        }
        public Pool CreationPool
        {
            get;
            set;
        }

        Texture LoadTexture(string name, FileLocation fl, UIImageInformation info)
        {
            ImageBase res;
            if (info.UsePalette)
            {
                fl = FileSystem.Instance.Locate(info.FileNames, info.LocateRule);
                ShpAnim shp = AnimManager.Instance.CreateInstance(fl) as ShpAnim; // ShpAnim.FromFile(fl);

                if (shp == null)
                {
                    shp.Dispose();
                    throw new NotSupportedException();
                }

                ResourceLocation pall = FileSystem.Instance.Locate(info.FilePalettes, info.LocateRule);

                if (PaletteCache == null)
                {
                    shp.Palette = Palette.FromFile(pall);
                }
                else
                {
                    shp.Palette = PaletteCache.CreatePalette(pall);
                }

                int frame = info.Frame;

                res = shp.GetImage(frame);
                if (info.HasTransparentColor)
                    res.MakeTransparent(info.TransparentColor);

                //hash = shp.GetHashCode();

                name = fl.Name + frame.ToString();
                //shp.Dispose();
            }
            else
            {
                fl = FileSystem.Instance.Locate(info.FileNames, info.LocateRule);
                res = ImageManager.Instance.CreateInstance(fl);
                if (info.HasTransparentColor)
                    res.MakeTransparent(info.TransparentColor);

                name = fl.Name;
                //hash = res.GetHashCode();
            }

            Texture tex = res.GetTexture(device, CreationUsage, CreationPool);
            res.Dispose();

            InstanceEntry inst = new InstanceEntry(name, tex);
            instances.Add(name, inst);
            instancesByHash.Add(tex.GetHashCode(), inst);
            return tex;
        }

        public Texture CreateInstance(UIImageInformation info)
        {
            FileLocation fl;

            string name;

            if (info.UsePalette)
            {
                fl = FileSystem.Instance.Locate(info.FileNames, info.LocateRule);
                int frame = info.Frame;
                name = fl.Name + frame.ToString();
            }
            else
            {
                fl = FileSystem.Instance.Locate(info.FileNames, info.LocateRule);
                
                name = fl.Name;
            }

            Texture result = null;
            InstanceEntry entry;
            if (instances.TryGetValue(name, out entry))
            {
                if (entry.IsAlive)
                {
                    result = entry.Object;
                }
                else
                {
                    instances.Remove(name);
                    entry.Dispose();
                }
            }
            if (result == null)
            {
                result = LoadTexture(name, fl, info);
            }

            return result;
        }
        public void DestoryInstance(Texture obj)
        {
            InstanceEntry entry = null;

            int hash = obj.GetHashCode();
            if (instancesByHash.TryGetValue(hash, out entry))
            {
                instances.Remove(entry.Name);
                instancesByHash.Remove(hash);
                entry.Dispose();
            }

            obj.Dispose();
        }
    }
}
