using System;
using System.Collections.Generic;
using System.Text;
using Ra2Reload.IO;

namespace Ra2Reload.IsoMap
{
    public class Tile3DFactory : ITileFactory
    {

        #region IAbstractFactory<TileBase> 成员

        public TileBase CreateInstance(string file)
        {
            return Tile3D.FromFile(file);
        }

        public TileBase CreateInstance(ResourceLocation fl)
        {
            return Tile3D.FromFile(fl);
        }

        public string Type
        {
            get { return "3D Tiles"; }
        }

        #endregion
    }
}
