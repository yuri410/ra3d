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
using SlimDX.Direct3D9;

namespace R3D.GraphicsEngine
{
    public class TextureManager : ObjectTracker<Texture>
    {
        static TextureManager singleton;

        static void InvalidOpErr()
        {
            throw new InvalidOperationException();
        }

        public static void Initialize(Device dev)
        {
            singleton = new TextureManager(dev);
        }

        public static TextureManager Instance
        {
            get
            {
                if (singleton == null)
                    InvalidOpErr();
                return singleton;
            }
        }

        Device device;
        Usage usage;
        Pool pool;


        private TextureManager(Device dev)
        {
            device = dev;
            usage = Usage.None;
            pool = Pool.Managed;
        }

        public Usage CreationUsage
        {
            get { return usage; }
            set { usage = value; }
        }
        public Pool CreationPool
        {
            get { return pool; }
            set { pool = value; }
        }

        protected override Texture create(ResourceLocation rl)
        {
            return Texture.FromStream(device, rl.GetStream, usage, pool);//Texture.FromStream(device, rl.GetStream, 0, 0, 1, usage, Format.Unknown, pool, Filter.None, Filter.None, 0);
        }

        protected override void destroy(Texture obj)
        {
            if (obj != null && !obj.Disposed)
                obj.Dispose();
        }


    }
}
