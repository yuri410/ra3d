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
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SlimDX;
using SlimDX.Direct3D9;
using System.ComponentModel;
using R3D.MathLib;
using R3D.GraphicsEngine;

namespace Ra2Develop.Designers
{
    public delegate void ViewChangedHandler();

    public class View3D : IDisposable
    {
        ToolStrip viewControls;

        ToolStripButton rotateLeft;

        ToolStripButton rotateRight;

        ToolStripButton rotateUp;

        ToolStripButton rotateDown;

        ToolStripTextBox eyeDistanceTBox;

        ViewChangedHandler viewChanged;

        float eyeAngleX;

        float eyeAngleY;

        float eyeDist;

        float fovy;


        VertexBuffer axis;

        Vector3 eye;

        Matrix projection;
        Matrix view;

        Vector3 target;

        public View3D(float ex, float ey, float edist, float fovy)
        {
            eyeAngleX = ex;
            eyeAngleY = ey;
            eyeDist = edist;
            this.fovy = fovy;

            rotateLeft = new ToolStripButton(Program.StringTable["GUI:3DRotateL"]);
            rotateLeft.Click += rotateLeftTool_Click;
            rotateLeft.DisplayStyle = ToolStripItemDisplayStyle.Image;

            rotateRight = new ToolStripButton(Program.StringTable["GUI:3DRotateR"]);
            rotateRight.Click += rotateRightTool_Click;
            rotateRight.DisplayStyle = ToolStripItemDisplayStyle.Image;

            rotateUp = new ToolStripButton(Program.StringTable["GUI:3DRotateU"]);
            rotateUp.Click += rotateUpTool_Click;
            rotateUp.DisplayStyle = ToolStripItemDisplayStyle.Image;

            rotateDown = new ToolStripButton(Program.StringTable["GUI:3DRotateD"]);
            rotateDown.Click += rotateDownTool_Click;
            rotateDown.DisplayStyle = ToolStripItemDisplayStyle.Image;

            eyeDistanceTBox = new ToolStripTextBox();
            eyeDistanceTBox.Validating += eyeDistanceTextBox_Validating;

            EyeDistance = eyeDist;
            viewControls = new ToolStrip(rotateLeft, rotateRight, rotateUp, rotateDown, eyeDistanceTBox);




        }

        private void rotateLeftTool_Click(object sender, EventArgs e)
        {
            EyeAngleX -= 0.1f;
        }

        private void rotateRightTool_Click(object sender, EventArgs e)
        {
            EyeAngleX += 0.1f;
        }

        private void rotateUpTool_Click(object sender, EventArgs e)
        {
            EyeAngleY += 0.1f;
        }

        private void rotateDownTool_Click(object sender, EventArgs e)
        {
            EyeAngleY -= 0.1f;
        }

        private void eyeDistanceTextBox_Validating(object sender, CancelEventArgs e)
        {
            float res;
            if (float.TryParse(eyeDistanceTBox.Text, out res))
            {
                if (res > 0)
                {
                    eyeDist = res;
                }
                else
                {
                    MessageBox.Show(Program.StringTable["MSG:TooClose"], Program.StringTable["GUI:Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                    e.Cancel = true;
                }
            }
            else
            {
                MessageBox.Show(Program.StringTable["MSG:InvaildFloat"], Program.StringTable["GUI:Error"], MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }

        public ToolStrip ToolBar
        {
            get { return viewControls; }
        }

        public Vector3 EyePosition
        {
            get { return eye; }
        }

        public Vector3 Target
        {
            get { return target; }
        }

        public Matrix Projection
        {
            get { return projection; }
        }

        public Matrix View
        {
            get { return view; }
        }

        public float EyeAngleX
        {
            get { return eyeAngleX; }
            set
            {
                eyeAngleX = value;
                if (viewChanged != null)
                    viewChanged();
            }
        }

        public float EyeAngleY
        {
            get { return eyeAngleY; }
            set
            {
                eyeAngleY = value;
                if (eyeAngleY < -MathEx.PIf * 0.5f)
                    eyeAngleY = -MathEx.PIf * 0.5f;
                if (eyeAngleY > MathEx.PIf * 0.5f)
                    eyeAngleY = MathEx.PIf * 0.5f;

                if (viewChanged != null)
                    viewChanged();
            }
        }

        public float EyeDistance
        {
            get { return eyeDist; }
            set
            {
                eyeDist = value;
                if (eyeDist < 10)
                    eyeDist = 10;
                eyeDistanceTBox.Text = value.ToString();
                if (viewChanged != null)
                    viewChanged();
            }
        }

        public float Fovy
        {
            get { return fovy; }
        }

        public event ViewChangedHandler ViewChanged
        {
            add { viewChanged += value; }
            remove { viewChanged -= value; }
        }

        public unsafe void SetTransform(Device dev, Control ctl)
        {
            projection = Matrix.PerspectiveFovRH(fovy, (float)ctl.ClientSize.Width / (float)ctl.ClientSize.Height, 1, 1000);
            dev.SetTransform(TransformState.Projection, projection);

            eye.Y = (float)Math.Sin(eyeAngleY) * eyeDist;

            float r = (float)Math.Cos(eyeAngleY) * eyeDist;

            if (r < 0)
            {
                r = 0;
                eye.X = 0;
                eye.Z = 0;

                int sign = Math.Sign(eye.Y);

                Vector3 up = new Vector3(sign * (float)Math.Cos(eyeAngleX),
                    0,
                    sign * (float)Math.Sin(eyeAngleX));

                view = Matrix.LookAtRH(eye, target, up);
                dev.SetTransform(TransformState.View, view);
            }
            else if (r - eyeDist < 0.001f)
            {
                Vector3 up = new Vector3(0, 1, 0);

                eye.X = -(float)Math.Cos(eyeAngleX) * r;
                eye.Z = -(float)Math.Sin(eyeAngleX) * r;

                view = Matrix.LookAtRH(eye, target, up);
                dev.SetTransform(TransformState.View, view);
            }
            else
            {
                eye.X = (float)Math.Cos(eyeAngleX) * r;
                eye.Z = (float)Math.Sin(eyeAngleX) * r;

                Vector3 target = new Vector3(-eye.X, -eye.Y, -eye.Z);
                Vector3 tmp = new Vector3(eye.X, 0, eye.Z);

                Vector3 up;
                Vector3.Cross(ref target, ref tmp, out up);
                Vector3.Cross(ref target, ref up, out up);

                up.Normalize();
                view = Matrix.LookAtRH(eye, target, up);
                dev.SetTransform(TransformState.View, view);
            }

            if (axis == null)
            {

                axis = new VertexBuffer(dev, sizeof(VertexPC) * 6, Usage.None, VertexPC.Format, Pool.Managed);
                VertexPC* dst = (VertexPC*)axis.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

                float ext = 10;

                dst[0] = new VertexPC { pos = new Vector3(), diffuse = Color.Red.ToArgb() };
                dst[1] = new VertexPC { pos = new Vector3(ext, 0, 0), diffuse = Color.Red.ToArgb() };
                dst[2] = new VertexPC { pos = new Vector3(), diffuse = Color.Green.ToArgb() };
                dst[3] = new VertexPC { pos = new Vector3(0, ext, 0), diffuse = Color.Green.ToArgb() };
                dst[4] = new VertexPC { pos = new Vector3(), diffuse = Color.Blue.ToArgb() };
                dst[5] = new VertexPC { pos = new Vector3(0, 0, ext), diffuse = Color.Blue.ToArgb() };

                axis.Unlock();
            }
            dev.SetTransform(TransformState.World, Matrix.Identity);

            dev.SetStreamSource(0, axis, 0, sizeof(VertexPC));
            dev.VertexFormat = VertexPC.Format;
            dev.DrawPrimitives(PrimitiveType.LineList, 0, 3);
        }



        #region IDisposable 成员

        public void Dispose()
        {
            if (axis != null)
            {
                axis.Dispose();
                axis = null;
            }
        }

        #endregion
    }
    public class GraphicsDevice
    {
        Mesh ball;

        Panel target;

        Device dev;

        Sprite sprite;

        Dictionary<Control, SwapChain> renderTargets;



        SlimDX.Direct3D9.Font font;

        Direct3D direct3D;

        SwapChain currentSc;

        static GraphicsDevice singleton;

        public static GraphicsDevice Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new GraphicsDevice();
                return singleton; 
            }
        }

        void CreateBallMesh()
        {
            ball = Mesh.CreateSphere(dev, 1, 10, 10);
        }

        public Mesh BallMesh
        {
            get
            {
                if (ball == null)
                    CreateBallMesh();
                return ball;
            }
        }

        private GraphicsDevice()
        {
            direct3D = new Direct3D();

            target = new Panel();
            target.Dock = DockStyle.Fill;
            target.BackColor = Color.Black;
            target.Show();

            PresentParameters pm = new PresentParameters();
            pm.AutoDepthStencilFormat = Format.D16;
            pm.EnableAutoDepthStencil = true;
            pm.Windowed = true;
            pm.SwapEffect = SwapEffect.Discard;

            dev = new Device(direct3D, 0, DeviceType.Hardware, target.Handle, CreateFlags.HardwareVertexProcessing | CreateFlags.FpuPreserve, pm);

            //target.Resize += delegate(object sender, EventArgs e)
            //{
            //    pm.BackBufferWidth = target.ClientSize.Width;
            //    pm.BackBufferHeight = target.ClientSize.Height;
            //    if (target.ClientSize.Width == 0)
            //        pm.BackBufferWidth = 1;
            //    if (target.ClientSize.Height == 0)
            //        pm.BackBufferWidth = 1;

            //    dev.Reset(pm);
            //};

            renderTargets = new Dictionary<Control, SwapChain>();

        }

        //public event MouseEventHandler MouseMove
        //{
        //    add { target.MouseMove += value; }
        //    remove { target.MouseMove -= value; }
        //}
        //public event MouseEventHandler MouseDown
        //{
        //    add { target.MouseDown += value; }
        //    remove { target.MouseDown -= value; }
        //}
        //public event MouseEventHandler MouseClick
        //{
        //    add { target.MouseClick += value; }
        //    remove { target.MouseClick -= value; }
        //}
        //public event MouseEventHandler MouseUp
        //{
        //    add { target.MouseUp += value; }
        //    remove { target.MouseUp -= value; }
        //}

        //public void Bind(Control frm)
        //{
        //    //if (!isBinded)
        //    //{
        //    target.Parent = null;
        //    frm.Controls.Add(target);
        //    //isBinded = true;
        //    //}
        //    //else
        //    //    throw new InvalidOperationException();
        //}
        //public void Unbind(Control frm)
        //{
        //    //if (isBinded)
        //    //{
        //    frm.Controls.RemoveAt(target);
        //    target.Parent = null;
        //    //isBinded = false;
        //    //}
        //    //else
        //    //    throw new InvalidOperationException();
        //}


        public Sprite GetSprite
        {
            get
            {
                if (sprite == null)
                    sprite = new Sprite(dev);
                return sprite;
            }
        }

        public SlimDX.Direct3D9.Font DefaultFont
        {
            get
            {
                if (font == null)
                    font = new SlimDX.Direct3D9.Font(GraphicsDevice.Instance.Device, Control.DefaultFont);
                return font;
            }
        }

        public void BeginScene(Control ctl)
        {
            Size size = ctl.ClientSize;
            if (!renderTargets.TryGetValue(ctl, out currentSc))
            {
                PresentParameters pm = new PresentParameters();
                pm.AutoDepthStencilFormat = Format.D16;
                pm.DeviceWindowHandle = ctl.Handle;
                pm.EnableAutoDepthStencil = true;
                pm.Windowed = true;
                pm.SwapEffect = SwapEffect.Discard;


                pm.BackBufferHeight = size.Height;
                pm.BackBufferWidth = size.Width;

                //ctl.Resize += resetDev;
                currentSc = new SwapChain(dev, pm);
                renderTargets.Add(ctl, currentSc);
            }

            Surface back = currentSc.GetBackBuffer(0);

            SurfaceDescription desc = dev.DepthStencilSurface.Description;
            if (desc.Height != size.Height || desc.Width != size.Width)
            {
                dev.DepthStencilSurface.Dispose();
                dev.DepthStencilSurface = Surface.CreateDepthStencil(dev, size.Width, size.Height, Format.D16, MultisampleType.None, 0, true);            
            }

            desc = back.Description;
            if (desc.Height != size.Height || desc.Width != size.Width)
            {
                PresentParameters pm = new PresentParameters();
                pm.AutoDepthStencilFormat = Format.D16;
                pm.DeviceWindowHandle = ctl.Handle;
                pm.EnableAutoDepthStencil = true;
                pm.Windowed = true;
                pm.SwapEffect = SwapEffect.Discard;

                if (size.Width <= 0)
                    size.Width = 1;
                if (size.Height <= 0)
                    size.Height = 1;

                pm.BackBufferHeight = size.Height;
                pm.BackBufferWidth = size.Width;

                renderTargets[ctl].Dispose();
                renderTargets.Remove(ctl);
                currentSc = new SwapChain(dev, pm);
                renderTargets.Add(ctl, currentSc);
            }

            back = currentSc.GetBackBuffer(0);
            dev.SetRenderTarget(0, back);
            //dev.DepthStencilSurface = currentSc.
            //back.Dispose();            
        
        }

        //void resetDev(object sender, EventArgs e)
        //{
        //    Control ctl = (Control)sender;

        //    PresentParameters pm = new PresentParameters();
        //    pm.AutoDepthStencilFormat = Format.D16;
        //    pm.DeviceWindowHandle = ctl.Handle;
        //    pm.EnableAutoDepthStencil = true;
        //    pm.Windowed = true;
        //    pm.SwapEffect = SwapEffect.Discard;

        //    Size size = ctl.ClientSize;

        //    if (size.Width <= 0)
        //        size.Width = 1;
        //    if (size.Height <= 0)
        //        size.Height = 1;

        //    pm.BackBufferHeight = size.Height;
        //    pm.BackBufferWidth = size.Width;


        //    renderTargets[ctl].Dispose();
        //    renderTargets.RemoveAt(ctl);
        //    renderTargets.Add(ctl, new SwapChain(dev, pm));

        //    pm.DeviceWindowHandle = target.Handle;
        //}

        public void EndScene()
        {
            if (currentSc == null)
            {
                throw new InvalidOperationException();
            }           
            currentSc.Present(Present.DoNotWait);            
            currentSc = null;
        }

        public void Free(Control ctl)
        {
            SwapChain sc;
            if (renderTargets.TryGetValue(ctl, out sc))
            {
                sc.Dispose();
                renderTargets.Remove(ctl);               
            }
        }

        public Device Device
        {
            get { return dev; }
        }
    }
}
