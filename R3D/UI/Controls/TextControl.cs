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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using SlimDX.Direct3D9;
using R3D.ConfigModel;

namespace R3D.UI.Controls
{
    public abstract class TextControl : Control
    {
        SlimDX.Direct3D9.Font d3dFont;
        System.Drawing.Font font;

        //protected string text;

        protected TextControl(GameUI gameUI, string scriptName)
            : base(gameUI, scriptName)
        {
            Text = string.Empty;
        }

        protected TextControl(GameUI gameUI)
            : this(gameUI, null)
        {
        }


        /// <summary>
        /// Gets or sets the font.
        /// </summary>
        /// <value>The font.</value>
        public System.Drawing.Font Font
        {
            get { return font; }
            set
            {
                if (font != value)
                {
                    // Dispose the old font
                    if (font != null)
                    {
                        font.Dispose();
                    }
                    if (d3dFont != null)
                    {
                        d3dFont.Dispose();
                        d3dFont = null;
                    }

                    font = value;

                    if (font != null)
                    {
                        d3dFont = new SlimDX.Direct3D9.Font(Device, font);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control should automatically size to its contents.
        /// </summary>
        /// <value><c>true</c> if the control should automatically size to its contents; otherwise, <c>false</c>.</value>
        public bool AutoSize
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }

        public ContentAlignment ContentAlign
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the preferred size of the control.
        /// </summary>
        /// <returns>The preferred size of the control.</returns>
        protected virtual Size GetPreferredSize()
        {            
            // return the size of the text             
            if (d3dFont != null)
            {                
                return d3dFont.MeasureString(Sprite, Text, GetDrawTextFormat(ContentAlign) | DrawTextFormat.SingleLine).Size;
            }
            else
            {
                return new Size(50, 50);
            }
        }

        public override void Update(float dt)
        {
            // call the base method
            base.Update(dt);

            if (AutoSize)
            {
                // check if we need to autosize
                Size size = GetPreferredSize();
                if (size != Size.Empty)
                {
                    // resize
                    Size = size;
                    //Width += Padding.Horizontal;
                    //Height += Padding.Vertical;
                }
            }
        }

        protected void DrawString(string str, int x, int y, int w, int h)
        {
            if (!string.IsNullOrEmpty(str))
            {
                if (d3dFont != null)
                {
                    Rectangle rect = new Rectangle(x, y, w, h);

                    rect.Height -= padding.Vertical;
                    rect.Width -= padding.Horizontal;
                    rect.X += padding.Left;
                    rect.Y += padding.Right;

                    d3dFont.DrawString(Sprite, str, rect, GetDrawTextFormat(ContentAlign) | DrawTextFormat.WordBreak, foreColor.ToArgb());
                }
            }
        }

        public override void PaintControl()
        {
            base.PaintControl();

            if (!string.IsNullOrEmpty(Text))
            {
                if (d3dFont != null)
                {
                    Rectangle rect = new Rectangle(0, 0, Width, Height);

                    rect.Height -= padding.Vertical;
                    rect.Width -= padding.Horizontal;
                    rect.X += padding.Left;
                    rect.Y += padding.Right;
                    if (AutoSize)
                    {
                        d3dFont.DrawString(Sprite, Text, rect, GetDrawTextFormat(ContentAlign) | DrawTextFormat.SingleLine, foreColor.ToArgb());
                    }
                    else
                    {
                        d3dFont.DrawString(Sprite, Text, rect, GetDrawTextFormat(ContentAlign) | DrawTextFormat.WordBreak, foreColor.ToArgb());
                    }
                }
            }
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (font != null)
                {
                    font.Dispose();
                    font = null;
                }
                if (d3dFont != null)
                {
                    d3dFont.Dispose();
                    d3dFont = null;
                }
            }
            else            
            {
                font = null;
                d3dFont = null;
            }
        }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);

            this.Text = sect.GetUIString("Text", string.Empty);

            float fSize = sect.GetFloat("FontSize", 10.5f);

            string v;
            if (sect.TryGetValue("Font", out v))
            {
                this.Font = new System.Drawing.Font(v.Trim(), fSize);
            }

            if (sect.TryGetValue("ContentAlignment", out v))
            {
                this.ContentAlign = (ContentAlignment)Enum.Parse(typeof(ContentAlignment), v, true);
            }
        }

        static protected DrawTextFormat GetDrawTextFormat(ContentAlignment ca)
        {
            DrawTextFormat res;
            switch (ca)
            {
                case ContentAlignment.BottomCenter:
                    res = DrawTextFormat.Bottom | DrawTextFormat.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    res = DrawTextFormat.Bottom | DrawTextFormat.Left;
                    break;
                case ContentAlignment.BottomRight:
                    res = DrawTextFormat.Bottom | DrawTextFormat.Right;
                    break;
                case ContentAlignment.MiddleCenter:
                    res = DrawTextFormat.VerticalCenter | DrawTextFormat.Center;
                    break;
                case ContentAlignment.MiddleLeft:
                    res = DrawTextFormat.VerticalCenter | DrawTextFormat.Left;
                    break;
                case ContentAlignment.MiddleRight:
                    res = DrawTextFormat.VerticalCenter | DrawTextFormat.Right;
                    break;
                case ContentAlignment.TopCenter:
                    res = DrawTextFormat.Top | DrawTextFormat.Center;
                    break;
                case ContentAlignment.TopLeft:
                    res = DrawTextFormat.Top | DrawTextFormat.Left;
                    break;
                case ContentAlignment.TopRight:
                    res = DrawTextFormat.Top | DrawTextFormat.Right;
                    break;
                default :
                    res = DrawTextFormat.Left | DrawTextFormat.Top;
                    break;
            }
            
            if (IsTextRight2Left)
            {
                res |= DrawTextFormat.RtlReading;
            }
            return res;
        }


        public Size MeasureString()
        {
            return GetPreferredSize();
        }

        //protected Size MeasureString(StringFormat format)
        //{
        //    Bitmap img = new Bitmap(1, 1);
        //    System.Drawing.Graphics mg = System.Drawing.Graphics.FromImage(img);
        //    SizeF ms = mg.MeasureString(text, font, new PointF(), format);
        //    mg.Dispose();
        //    img.Dispose();
        //    return new Size((int)ms.Width, (int)ms.Height);
        //}

        //protected Bitmap DrawString(string text, System.Drawing.Font font, StringFormat format, float width, float height)
        //{
        //    Bitmap tmp = new Bitmap((int)width, (int)height);
        //    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tmp);
        //    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        //    g.DrawString(text, font, new SolidBrush(foreColor), new RectangleF(0, 0, width, height),
        //        format);

        //    g.Dispose();
        //    return tmp;
        //}

        //protected Bitmap DrawString(string text, System.Drawing.Font font, TextFormatFlags format, int width, int height)
        //{
        //    Bitmap tmp = new Bitmap((int)width, (int)height);
        //    System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(tmp);
        //    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        //    TextRenderer.DrawText(g, text, font, new Rectangle(0, 0, width, height), foreColor, backColor, format);
        //    g.Dispose();
        //    return tmp;
        //}

        //protected TextFormatFlags CreateTextFormat(RightToLeft rtl,
        //    ContentAlignment textAlign, bool showEllipsis)
        //{
        //    TextFormatFlags res = TextFormatFlags.Default;

        //    switch (textAlign)
        //    {
        //        case ContentAlignment.BottomCenter:
        //            res |= TextFormatFlags.Bottom | TextFormatFlags.HorizontalCenter;
        //            break;
        //        case ContentAlignment.BottomLeft:
        //            res |= TextFormatFlags.Bottom | TextFormatFlags.Left;
        //            break;
        //        case ContentAlignment.BottomRight:
        //            res |= TextFormatFlags.Bottom | TextFormatFlags.Right;
        //            break;
        //        case ContentAlignment.MiddleCenter:
        //            res |= TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
        //            break;
        //        case ContentAlignment.MiddleLeft:
        //            res |= TextFormatFlags.VerticalCenter | TextFormatFlags.Left;
        //            break;
        //        case ContentAlignment.MiddleRight:
        //            res |= TextFormatFlags.VerticalCenter | TextFormatFlags.Right;
        //            break;
        //        case ContentAlignment.TopCenter:
        //            res |= TextFormatFlags.Top | TextFormatFlags.HorizontalCenter;
        //            break;
        //        case ContentAlignment.TopLeft:
        //            res |= TextFormatFlags.Top | TextFormatFlags.Left;
        //            break;
        //        case ContentAlignment.TopRight:
        //            res |= TextFormatFlags.Top | TextFormatFlags.Right;
        //            break;
        //    }

        //    if (rtl == RightToLeft.Yes)
        //        res |= TextFormatFlags.RightToLeft;

        //    if (showEllipsis)
        //        res |= TextFormatFlags.WordEllipsis | TextFormatFlags.PathEllipsis | TextFormatFlags.EndEllipsis;

        //    res |= TextFormatFlags.WordBreak;
        //    //res= TextFormatFlags.

        //    return res;
        //}

        //static readonly ContentAlignment anyRight = ContentAlignment.BottomRight | ContentAlignment.MiddleRight | ContentAlignment.TopRight;
        //static readonly ContentAlignment anyBottom = ContentAlignment.BottomRight | ContentAlignment.BottomCenter | ContentAlignment.BottomLeft;
        //static readonly ContentAlignment anyCenter = ContentAlignment.BottomCenter | ContentAlignment.MiddleCenter | ContentAlignment.TopCenter;
        //static readonly ContentAlignment anyMiddle = ContentAlignment.MiddleRight | ContentAlignment.MiddleCenter | ContentAlignment.MiddleLeft;

        //static StringAlignment TranslateLineAlignment(ContentAlignment align)
        //{
        //    if ((align & anyBottom) != ((ContentAlignment)0))
        //        return StringAlignment.Far;
        //    if ((align & anyMiddle) != ((ContentAlignment)0))
        //        return StringAlignment.Center;
        //    return StringAlignment.Near;
        //}

        //static StringAlignment TranslateAlignment(ContentAlignment align)
        //{
        //    if ((align & anyRight) != ((ContentAlignment)0))
        //        return StringAlignment.Far;
        //    if ((align & anyCenter) != ((ContentAlignment)0))
        //        return StringAlignment.Center;
        //    return StringAlignment.Near;
        //}

        //static StringFormat StringFormatForAlignment(ContentAlignment align)
        //{
        //    StringFormat format = new StringFormat();
        //    format.Alignment = TranslateAlignment(align);
        //    format.LineAlignment = TranslateLineAlignment(align);
        //    return format;
        //}

        ///// <summary>
        ///// 根据提供的格式信息为字符串建立StringFormat，通常用于字符串绘制。
        ///// </summary>
        //protected static StringFormat CreateStringFormat(bool autoSize, RightToLeft rtl,
        //    ContentAlignment textAlign, bool showEllipsis, bool useMnemonic)
        //{
        //    StringFormat format = StringFormatForAlignment(textAlign);
        //    if (rtl == RightToLeft.Yes)
        //        format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;

        //    if (showEllipsis)
        //    {
        //        format.Trimming = StringTrimming.EllipsisCharacter;
        //        format.FormatFlags |= StringFormatFlags.LineLimit;
        //    }
        //    if (!useMnemonic)
        //        format.HotkeyPrefix = HotkeyPrefix.None;
        //    else
        //        format.HotkeyPrefix = HotkeyPrefix.Hide;

        //    if (autoSize)
        //        format.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;

        //    return format;
        //}
    }
}
