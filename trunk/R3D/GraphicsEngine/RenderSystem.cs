using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.Direct3D9;
using Ra2Reload.Graphics.Effects;

namespace Ra2Reload.Graphics
{
    public class RenderSystem : Device
    {
        Dictionary<int, List<MeshMaterial>> effectTable;


        Queue<RenderOperation> renderQueue;

        int comingOps;

        public RenderSystem(int adapter, IntPtr handle, CreateFlags flags, PresentParameters pm)
            : base(adapter, DeviceType.Hardware, handle, flags, pm)
        {
            effectTable = new Dictionary<string, List<MeshMaterial>>();
            renderQueue = new Queue<RenderOperation>();
        }



        public void Render(RenderOperation op)
        {
            renderQueue.Enqueue(op);

            EffectBase eff = op.Material.Effect;
            if (eff != null)
            {
                
                
            }
        }

        
    }
}
