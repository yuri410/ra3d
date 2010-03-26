using System;
using System.Collections.Generic;
using System.Text;
using DXLib.Direct3D9;

namespace Ra2Reload.UI
{
    public class UIPictureBox : UIControl, IDisposable
    {
        public UIPictureBox(Device dev)
            : base(dev)
        { }

        protected override void Apply()
        {
            throw new NotImplementedException();
        }

        #region IDisposable 成员

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion

        public override void OnPaint(Sprite spr)
        {
            throw new NotImplementedException();
        }
    }
}
