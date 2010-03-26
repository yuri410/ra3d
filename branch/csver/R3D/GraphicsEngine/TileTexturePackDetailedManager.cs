using System;
using System.Collections.Generic;
using System.Text;
using Ra2Reload.GameResource;

namespace Ra2Reload.Graphics
{
    public class TileTexturePackDetailedManager : CachedObjectManager<TileTexturePackDetailed>
    {
        static TileTexturePackDetailedManager singleton;

        private TileTexturePackDetailedManager()
        { }

        public static TileTexturePackDetailedManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new TileTexturePackDetailedManager();
                return singleton;
            }
        }
    }
}
