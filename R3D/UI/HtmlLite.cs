 using System;
 using System.Collections.Generic;
 using System.Text;
 using System.Windows.Forms;
 using System.Runtime.InteropServices;
 using System.Drawing;

namespace WindowsApplication1
{
    //
    //
    // A managed wrapper for htmllite.dll 
    // 
    // Important: htmllite.dll must be on the system PATH in order
    // for this control to work in design-time.   
    //
    // This code was built against htmllite.dll version 8.0.50727.42   
    // Nothing terribly interesting here, just one new property and   
    // one event.   
    // 
    public class HTMLLiteControl : System.Windows.Forms.TextBox
    {
        public HTMLLiteControl()
        {
            this.AutoSize = false;
            this.Multiline = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        [DllImport("htmllite.dll", CallingConvention = CallingConvention.StdCall,
            EntryPoint = "_FRegisterHtmlLiteClass@4")]
        public static extern void RegisterHtmlLite(int zero);
        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_NOTIFY = 0x004E;

        /// <summary>
        /// A link has been clicked either via the Mouse or Spacebar key
        /// </summary>
        private const int HTMLLITE_CODE_LEFTCLICK = 1000;
        /// <summary>
        /// A link has received focus due to the Tab Cycle, or Keyboard Arrow Keys
        /// </summary>
        private const int HTMLLITE_CODE_TABCYCLE = 1001;
        /// <summary>
        /// A link has been right-clicked, and already had focus
        /// </summary>
        private const int HTMLLITE_CODE_RIGHTCLICK = 1003;
        /// <summary>
        /// Mouse is over a link
        /// </summary>
        private const int HTMLLITE_CODE_MOUSEOVER = 1004;
        /// <summary>
        /// Mouse is hovering a link (~1 second = hover)
        /// </summary>
        private const int HTMLLITE_CODE_MOUSEHOVER = 1005;
        /// <summary>
        /// Mouse has left a link's rectangle
        /// </summary>
        private const int HTMLLITE_CODE_MOUSEEXIT = 1006;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NMHTMLLITE
        {
            public IntPtr hwndFrom;
            public IntPtr idFrom;
            public IntPtr code;
            public IntPtr linkid;
            public RECT linkrc;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            IntPtr hwndParent = GetParent(this.Handle);
            if (hwndParent != IntPtr.Zero)
            {
                if (parentHook != null && parentHook.Handle != IntPtr.Zero)
                    parentHook.ReleaseHandle();
                if (parentHook == null)
                {
                    parentHook = new ParentHook(this, hwndParent);
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (this.parentHook != null)
                this.parentHook.ReleaseHandle();
        }

        [DllImport("user32")]
        private static extern IntPtr GetParent(IntPtr hwnd);
        protected override CreateParams CreateParams
        {
            get
            {
                RegisterHtmlLite(0);
                CreateParams createParams = base.CreateParams;
                createParams.ClassName = "HTMLLITE";
                return createParams;
            }
        }

        protected void OnParentNotify(NMHTMLLITE hdr)
        {
            if (hdr.hwndFrom == this.Handle && hdr.code.ToInt32() == HTMLLITE_CODE_LEFTCLICK)
                OnLinkClicked(hdr.linkid.ToInt32());
        }

        protected virtual void OnLinkClicked(int linkId)
        {
            if (this.LinkClicked != null)
                this.LinkClicked(this, new LinkClickedEventArgs(linkId));
        }

        private class ParentHook : NativeWindow
        {
            public ParentHook(HTMLLiteControl owner, IntPtr hwndParent)
            {
                ownerControl = owner;
                AssignHandle(hwndParent);
            }
            private HTMLLiteControl ownerControl = null;
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_NOTIFY && m.LParam != IntPtr.Zero)
                {
                    NMHTMLLITE hdr = (NMHTMLLITE)m.GetLParam(typeof(NMHTMLLITE));
                    int code = hdr.code.ToInt32();
                    if (code >= HTMLLITE_CODE_LEFTCLICK && code <= HTMLLITE_CODE_MOUSEEXIT)
                        ownerControl.OnParentNotify(hdr);
                } base.WndProc(ref m);
            }
        }

        private ParentHook parentHook = null;
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ERASEBKGND:
                    using (Graphics screenGraphics = Graphics.FromHdc(m.WParam))
                    using (Brush b = new SolidBrush(this.BackColor))
                        screenGraphics.FillRectangle(b, this.ClientRectangle);
                    break;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Gets/sets the HTML source (read the Text property to get the formatted text).    
        /// </summary>
        public string HTML
        {
            get
            {
                return html;
            }
            set
            {
                base.Text = value;
                html = value;
            }
        }

        private string html = string.Empty;

        /// <summary>
        /// Fired when a link is clicked, with the linkid passed in the event arguments.
        /// </summary>
        public event EventHandler<LinkClickedEventArgs> LinkClicked = null;
    }

    public class LinkClickedEventArgs : System.EventArgs
    {
        public LinkClickedEventArgs(int linkId)
        {
            this.LinkId = linkId;
        }

        public int LinkId;
    }
}
