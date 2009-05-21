using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Ra2Reload.UI;

using SlimDX.Direct3D9;

namespace Ra2Reload.ScriptEngine
{
    /* 内部实现的事件
     *  事件引发时，如果没有用户邦定的事件处理程序，则调用自带的处理程序
     *  
     * 内部实现不能调用后者，否则就不是内部实现。
     * 
     */
    public interface ILoadInternalImpl
    {
        void OnLoadInternal(object sender);
    }

    public interface IMouseInternalImpl
    {
        void OnMouseMoveInternal(object sender, MouseEventArgs e);
        void OnMouseDownInternal(object sender, MouseEventArgs e);
        void OnMouseUpInternal(object sender, MouseEventArgs e);
    }

    public interface IPaintInternalImpl
    {
        void OnPaintInternal(Sprite spr);
    }
    public interface ICloseInernalImpl
    {
        void OnCloseInternal(object sender);
    }

    public interface IInternalImplUI
    {
        //void UseDefOnLoad();
        //void UseDefOnMouseMove();
        //void UseDefOnMouseDown();
        //void UseDefOnMouseUp();
        //void UseDefOnPaint();

        void OnPaintInternal(Sprite spr);
        void OnMouseMoveInternal(object sender, MouseEventArgs e);
        void OnMouseDownInternal(object sender, MouseEventArgs e);
        void OnMouseUpInternal(object sender, MouseEventArgs e);
        void OnLoadInternal(object sender);

        //bool HasObjectLoad { get; }
        //bool HasMouseMove { get; }
        //bool HasMouseDown { get; }
        //bool HasMouseUp { get; }
        //bool HasPaint { get; }
        //MouseHandler MouseUpHandler { get; }
        //MouseHandler MouseDownHandler { get; }
        //MouseHandler MouseMoveHandler { get; }

        //PaintHandler2D PaintHandler { get; }
        //LoadHandler LoadHandler { get; }

    }

}
