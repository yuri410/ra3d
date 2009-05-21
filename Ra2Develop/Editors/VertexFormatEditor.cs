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
using System.Drawing.Design;
using System.ComponentModel;
using SlimDX.Direct3D9;
using R3D.GraphicsEngine;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.Design;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Globalization;

namespace Ra2Develop.Editors
{
    public class VertexFormatEditor : UITypeEditor
    {
        VertexFormatEditorControl ui;

        //public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        //{
        //    return true;
        //}
        //public override void PaintValue(PaintValueEventArgs e)
        //{
            //e.Graphics.DrawString(e.Value.ToString(), Control.DefaultFont, SystemBrushes.ControlText, new PointF());
        //}        

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (ui == null)
                ui = new VertexFormatEditorControl();

            //m_ui.SetStates((DockAreas)value);
            ui.Value = (VertexFormat)value;
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            ui.Service = edSvc;
            edSvc.DropDownControl(ui);

            ui.Service = null;
            return ui.Value;
        }


        public override bool IsDropDownResizable
        {
            get { return true; }
        }
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
    }
    public class VertexFormatConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return ((destinationType == typeof(string))
                //|| (destinationType == typeof(InstanceDescriptor)))
                || base.CanConvertTo(context, destinationType));
        }
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException("destinationType");
            }
            if (culture == null)
            {
                culture = CultureInfo.CurrentCulture;
            }

            if ((destinationType == typeof(string)) && (value is VertexFormat))
            {
                string separator = culture.TextInfo.ListSeparator + " ";
                VertexFormat fmt = (VertexFormat)value;


                StringBuilder sb = new StringBuilder(20);

                if ((fmt & VertexFormat.Position) == VertexFormat.Position)
                {
                    sb.Append("Position");
                    sb.Append(separator);
                }
                if ((fmt & VertexFormat.Normal) == VertexFormat.Normal)
                {
                    sb.Append("Normal");
                    sb.Append(separator);
                }
                if ((fmt & VertexFormat.Texture1) == VertexFormat.Texture1)
                {
                    sb.Append("Texture1");
                    sb.Append(separator);
                }
                else if ((fmt & VertexFormat.Texture2) == VertexFormat.Texture2)
                {
                    sb.Append("Texture2");
                    sb.Append(separator);
                }
                else if ((fmt & VertexFormat.Texture3) == VertexFormat.Texture3)
                {
                    sb.Append("Texture3");
                    sb.Append(separator);
                }
                else if ((fmt & VertexFormat.Texture4) == VertexFormat.Texture4)
                {
                    sb.Append("Texture4");
                    sb.Append(separator);
                }

                if ((fmt & VertexFormat.Diffuse) == VertexFormat.Diffuse)
                {
                    sb.Append("Colour");
                    sb.Append(separator);
                }

                string res = sb.ToString();
                res = res.Substring(0, res.Length - separator.Length);
                return res;
            }
            //if ((destinationType == typeof(InstanceDescriptor)) && (value is VertexFormat))
            //{
            //    //$S9 = new ValType[] { typeof(float), typeof(float) };
            //    VertexFormat fmt = (VertexFormat)value;
            //    info = typeof(VertexFormat).GetConstructor(new ValType[] { typeof(int), typeof(int), typeof(int), typeof(int) });
            //    if (info != null)
            //    {
            //        return new InstanceDescriptor(info, new object[] { faceData.IndexA, faceData.IndexB, faceData.IndexC, faceData.MaterialIndex });
            //    }
            //}
            return base.ConvertTo(context, culture, value, destinationType);
        }
        //public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        //{
        //    return false;// base.GetCreateInstanceSupported(context);
        //}
    }
}
