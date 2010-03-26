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
    public class ModelManager : ObjectTracker<GameModel>
    {
        static ModelManager singleton;

        public static ModelManager Instance
        {
            get
            {
                return singleton;
            }
        }
        public static bool IsInitialized
        {
            get { return singleton != null; }
        }
        public static void Initialize(Device dev)
        {
            singleton = new ModelManager(dev);
        }

        Device dev;

        public GameModel MissingModel
        {
            get;
            private set;
        }

        private ModelManager(Device dev)
        {
            this.dev = dev;

            FileLocation fl = FileSystem.Instance.Locate(FileSystem.LocalMix + "xxModel.mesh", FileSystem.GameResLR);

            MissingModel = GameModel.FromFile(dev, fl);
        }

        protected override GameModel create(ResourceLocation rl)
        {
            return GameModel.FromFile(dev, rl);
        }

        protected override void destroy(GameModel obj)
        {
            if (obj != null && !obj.Disposed)
            {
                obj.Dispose();
            }
        }
    }
}
