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
using System.Drawing;

using R3D.IO;
using R3D.Base;
using R3D.Media;

using SlimDX;
using SlimDX.Direct3D9;
using R3D.ConfigModel;

namespace R3D.UI
{
    /// <summary>
    /// parser
    /// </summary>
    public class UITexAnim : IDisposable
    {
        public enum PlayDirection
        {
            Forward,
            Backward
        }

        List<Texture> data;

        int start;
        int end;

        int currentFrame;
        int step;

        int width, height;
        PlayDirection direct;

        bool disposed;

        public UITexAnim(List<Texture> tex)
        {
            step = 1;
            data = tex;
        }


        public UITexAnim(ConfigurationSection sect, string defPath, string defSuffix)
        {
            Game game = Game.Instance;
            string[] pic = sect.GetPaths("Image");
            string[] pal = sect.GetPaths("ImagePalette");

            Device dev = game.Device;

            // shp
            //FileLocation fl = FileSystem.Instance.Locate(pic,
            //    defPath, defSuffix + FileSystem.dotShp,
            //    FileSystem.GameResLR);
            FileLocation fl = FileSystem.Instance.Locate(pic, FileSystem.GameResLR);

            ShpAnim shp = (ShpAnim)AnimManager.Instance.CreateInstance(fl);// ShpAnim.FromFile(fl);

            //Palette palette = Palette.FromFile(
            //    FileSystem.Instance.Locate(pal, defPath, defSuffix + FileSystem.dotPal, FileSystem.GameResLR));
            Palette palette = Palette.FromFile(
                FileSystem.Instance.Locate(pal, FileSystem.GameResLR));
            shp.Palette = palette;

            step = 1;

            data = new List<Texture>(shp.FrameCount);

            width = shp.Width;
            height = shp.Height;

            start = 0;
            end = shp.FrameCount - 1;


            Color tranClr;
            bool trans = sect.TryGetColorRGBA("Transparent", out tranClr);

            for (int i = 0; i < shp.FrameCount; i++)
            {
                ImageBase bmp = shp.GetImage(i);

                if (trans)
                    bmp.MakeTransparent(tranClr);

                data.Add(bmp.GetTexture(dev, Usage.None, Pool.Managed));

                bmp.Dispose();
            }
            shp.Dispose();
        }

        public int CurrentFrame
        {
            get { return currentFrame; }
            set { currentFrame = value; }
        }

        public int Width
        {
            get { return width; }
            set { width = value; }
        }
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public int Count
        {
            get { return data.Count; }
        }
        /// <summary>
        /// 动画开始索引
        /// </summary>
        public int StartFrame
        {
            get { return start; }
            set
            {
                start = value;
                step = end > start ? 1 : -1;
            }
        }
        public int EndFrame
        {
            get { return end; }
            set
            {
                end = value;
                step = end > start ? 1 : -1;
            }
        }

        public int Step
        {
            get { return step; }
        }

        public bool StartReached
        {
            get { return (currentFrame == start); }
        }
        public bool EndReached
        {
            get { return currentFrame == end; }
        }
        public bool SEReached        
        {
            get { return (currentFrame == end) | (currentFrame == start); }
        }

        public PlayDirection Direction
        {
            get { return direct; }
            set { direct = value; }
        }

        public void OnPaint(Sprite spr)
        {
            int s = step * ((direct == PlayDirection.Forward) ? 1 : -1);

            currentFrame += s;
            if (currentFrame < start)
                currentFrame = start;
            if (currentFrame > end)
                currentFrame = end;
            spr.Draw(data[currentFrame], -1);
        }

        public void Draw(Sprite spr, int index)
        {
            spr.Draw(data[index], -1);
        }

        public List<Texture> Data
        {
            get { return data; }
        }

        public Texture this[int i]
        {
            get { return data[i]; }
        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    data[i].Dispose();
                }

                data.Clear();
                data = null;
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        #endregion

        ~UITexAnim()
        {
            if (!disposed)
                Dispose();
        }
    }
}
