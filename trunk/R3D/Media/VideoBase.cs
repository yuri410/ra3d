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

using SlimDX.Direct3D9;

using R3D.IO;

using AviFile;

namespace R3D.Media
{
    public abstract class VideoBase
    {
        protected int currentFrame;
        protected int totalFrame;

        protected int width;
        protected int height;

        protected float frameLength;

        public int FrameCount
        {
            get { return totalFrame; }
        }
        public int Width
        {
            get { return width; }
        }
        public int Height
        {
            get { return height; }
        }

        public abstract void OnPaint(Sprite spr);

        public abstract void Update();
    }

    public class AviVideoFactory:IVideoFactory 
    {
        public AviVideoFactory()
        {
        }

        #region IAbstractFactory<VideoBase> 成员

        public VideoBase CreateInstance(string file)
        {
            return AviVideo.FromFile(file);
        }

        public VideoBase CreateInstance(ResourceLocation fl)
        {
            return AviVideo.FromFile(fl);
        }

        public string Type
        {
            get { return ".AVI"; }
        }

        #endregion
    }

    public class AviVideo : VideoBase
    {
        private AviVideo(ResourceLocation fl)
        {
            
        }

        public static AviVideo FromFile(string file)
        {
            return FromFile(new FileLocation(file));
        }
        public static AviVideo FromFile(ResourceLocation fl)
        {
            return new AviVideo(fl);
        }

        public override void OnPaint(Sprite spr)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }
    }
}
