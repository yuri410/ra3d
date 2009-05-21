using System;
using System.Collections.Generic;
using System.Text;
using Ra2Reload.IO;

namespace Ra2Reload.IsoMap
{
    public sealed class TileFactory : ITileFactory
    {
        #region ITileFactory 成员

        public TileBase CreateInstance(string file)
        {
            return Tile.FromFile(file);
        }

        public TileBase CreateInstance(ResourceLocation fl)
        {
            return Tile.FromFile(fl);
        }

        public string Type
        {
            get { return "Ra2 Tile(Old)"; }
        }

        #endregion

    }

    public interface ITileFactory : IAbstractFactory<TileBase, ResourceLocation>
    {
        //TileBase CreateInstance(string file);
        //TileBase CreateInstance(FileLocation fl);

        //string Type { get; }
    }
}
