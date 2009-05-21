using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using SlimDX.Direct3D9;

using Ra2Reload.Media;
using Ra2Reload.GameResource;

namespace Ra2Reload.Graphics
{
    /// <summary>
    /// 一张纹理，打包了一些地块的精细纹理
    /// </summary>
    public class TileTexturePackDetailed : CachedObject, ITileTexturePack
    {
        public TileTexturePackDetailed()             
            : base (TileTexturePackDetailedManager.Instance)
        {

        }
        protected override void load()
        {
            throw new NotImplementedException();
        }

        protected override void unload()
        {
            throw new NotImplementedException();
        }

        public override int GeiSize()
        {
            throw new NotImplementedException();
        }

        #region ITileTexturePack 成员

        public Rectangle[] Append(int index, ImageBase[] tileImage)
        {
            throw new NotImplementedException();
        }

        public void Lock()
        {
            throw new NotImplementedException();
        }

        public void Unlock()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public int Width
        {
            get { throw new NotImplementedException(); }
        }

        public int Height
        {
            get { throw new NotImplementedException(); }
        }


        public Texture Texture
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}