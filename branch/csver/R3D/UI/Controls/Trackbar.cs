﻿/*
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
using R3D.ConfigModel;
using R3D.Base;
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.UI.Controls
{
    public class TrackbarFactory : ControlFactory
    {
        public TrackbarFactory()
            : base("TrackBar")
        {
        }

        public override Control CreateInstance(GameUI gameUI, string name)
        {
            return new Trackbar(gameUI, name);
        }
    }
    public class Trackbar : Control, IDisposable
    {
        int value;


        //int sLeft;
        //int sRight;

        Label lbl;

        Size sliderSize;
        //Size textBgSize;

        //void ComputeSlideRange()
        //{
        //    sLeft = sliderSize.Width / 2;
        //    sRight = Width - sLeft;

        //}

        //public Texture TextBackGround
        //{
        //    get;
        //    set;
        //}
        public Texture BackgroundImage
        {
            get;
            set;
        }
        public Texture SliderImage
        {
            get;
            set;
        }

        //public Size TextBackSize
        //{
        //    get { return textBgSize; }
        //    set { textBgSize = value; }
        //}

        public Size SliderSize
        {
            get { return sliderSize; }
            set { sliderSize = value; }
        }


        public Trackbar(GameUI gameUI)
            : this(gameUI,null )
        {
        }

        public Trackbar(GameUI gameUI, string name)
            : base(gameUI, name)
        {
            lbl = new Label(gameUI);
            lbl.AutoSize = false;

              Rectangle rect = Bounds;
                rect.Height -= padding.Vertical;
                rect.Width -= padding.Horizontal;
                rect.X += padding.Left;
                rect.Y += padding.Right;
        }

        public static Trackbar FromConfigSection(GameUI gameUI, ConfigurationSection sect)
        {
            Trackbar res = new Trackbar(gameUI, sect.Name);

            res.Parse(sect);

            return res;
        }


        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            this.Location = sect.GetPoint("Position", Point.Empty);

            this.Maximum = sect.GetInt("Maximum");
            this.Minimum = sect.GetInt("Minimum");
            this.Step = sect.GetInt("Step", 1);
            this.Value = sect.GetInt("Value", Minimum);

            lbl.Height = this.Height;
            lbl.Width = this.Width / 3;

            lbl.Text = Minimum.ToString();
        }

        public int Value
        {
            get { return value; }
            set
            {
                lbl.Text = value.ToString();
                this.value = value;
            }
        }
        public int Step
        {
            get;
            set;
        }
        public int Maximum
        {
            get;
            set;
        }
        public int Minimum
        {
            get;
            set;
        }

        public override void PaintControl()
        {
            int modColorInt = modColor.ToArgb();
            Sprite.Transform = Matrix.Translation(X, Y, 0);
            Sprite.Draw(BackgroundImage, modColorInt);
            base.PaintControl();



            int sliderX = X + padding.Left + sliderSize.Width / 2 + (int)((2 * Width / 3 - padding.Right - sliderSize.Width) * ((double)value / (double)Maximum));
            Sprite.Transform = Matrix.Translation(
                sliderX,
                Y, 0);
            Sprite.Draw(SliderImage, modColorInt);

            //Sprite.Transform = Matrix.Translation(X + Width / 3, Y, 0);

            lbl.PaintControl();
        }
        public override void Update(float dt)
        {
            base.Update(dt);
            lbl.X = X + 2 * Width / 3;
            lbl.Y = Y;
            lbl.ModulateColor = this.modColor;
        }


        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (BackgroundImage != null)
                {
                    BackgroundImage.Dispose();
                }
                if (SliderImage != null)
                {
                    SliderImage.Dispose();
                }
            }
            BackgroundImage = null;
            SliderImage = null;

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (IsPressed)
            {

            }
        }



        //protected override void Apply()
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        if (lbl != null)
        //        {
        //            lbl.Dispose();
        //        }

        //        lbl = new Texture(device, 1, 1, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

        //        DataRectangle rect = lbl.LockRectangle(0, LockFlags.None);
        //        rect.Data.Write<uint>(0xff000000);

        //        lbl.UnlockRectangle(0);
        //    }
        //    else
        //        if (useGDIp)
        //        {
        //            TextFormatFlags fmt = CreateTextFormat(RightToLeft.No, contentAlignment, false);
        //            Bitmap tmp = DrawString(text, font, fmt, textBgSize.Width, textBgSize.Height);

        //            if (lbl != null)
        //                lbl.Dispose();

        //            lbl = Utils.Bitmap2Texture(device, tmp, Usage.None, Pool.Managed);// Texture.FromBitmap(d3d, tmp, Usage.None, Pool.Managed);

        //            tmp.Dispose();
        //        }
        //        else
        //        {
        //            StringFormat format = CreateStringFormat(false, RightToLeft.No, contentAlignment, false, useMnemonic);

        //            //SizeF ms = MeasureString(format);
        //            int lblWidth = textBgSize.Width;// ms.Width;
        //            int lblHeight = textBgSize.Height;// ms.Height;

        //            Bitmap tmp = DrawString(text, font, format, lblWidth, lblHeight); // new Bitmap((int)width, (int)height);

        //            if (lbl != null)
        //                lbl.Dispose();

        //            lbl = Utils.Bitmap2Texture(device, tmp, Usage.None, Pool.Managed);// Texture.FromBitmap(d3d, tmp, Usage.None, Pool.Managed);
        //            tmp.Dispose();
        //        }

        //    requireUpdate = false;
        //}

        //public override void OnPaint(Sprite spr)
        //{
        //    if (requireUpdate)
        //        Apply();


        //    spr.Transform = Matrix.Translation(
        //        bounds.X + sLeft + (int)((bounds.Width - textBgSize.Width - sliderSize.Width) * ((double)value / (double)max)),
        //        bounds.Y, 0);

        //    spr.Draw(slider, -1);


        //    spr.Transform = Matrix.Translation(bounds.Width - textBgSize.Width, bounds.Y, 0);
        //    spr.Draw(textBackGround, -1);
        //    spr.Draw(lbl, -1);

            


        //}


        //#region IDisposable Members

        //public void Dispose()
        //{
        //    if (!disposed)
        //    {

        //        disposed = true;
        //    }
        //    else
        //        throw new ObjectDisposedException(ToString());
        //}

        //#endregion
    }
}
