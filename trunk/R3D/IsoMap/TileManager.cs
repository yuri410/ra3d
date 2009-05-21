using System;
using System.Collections.Generic;
using System.Text;
using Ra2Reload.IO;
using Ra2Reload.GameResource;

namespace Ra2Reload.IsoMap
{
    public sealed class TileManager : UniqueObjectManager<TileBase>
    {
        //ITileFactory factory;
        static TileManager singleton;

        public static TileManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new TileManager();
                return singleton;
            }
        }
        private TileManager() { }

        public ITileFactory Creator
        {
            get { return factory; }
            set { factory = value; }
        }

        public TileBase CreateInstance(string file)
        {
            TileBase res = Exists(ComputeHash(file));
            if (res != null)
            {
                res.RefCount++;
                return res;
            }
            else
            {
                return factory.CreateInstance(file);
            }
        }
        public TileBase CreateInstance(ResourceLocation fl)
        {
            TileBase res = Exists(ComputeHash(fl.Name));
            if (res != null)
            {
                res.RefCount++;
                return res;
            }
            else
            {
                return factory.CreateInstance(fl);
            }
        }
    }
}
