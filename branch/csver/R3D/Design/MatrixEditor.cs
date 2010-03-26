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
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text;
using System.Windows.Forms.Design;
using R3D.Base;
using SlimDX;

namespace R3D.Design
{
    public class MatrixEditor : UITypeEditor
    {
        MatrixEditControl ui;

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (ui == null)
            {
                Assembly asm = Assembly.GetEntryAssembly();
                Type tpe = asm.GetType("Ra2Develop.Program", true);
                PropertyInfo stProp = tpe.GetProperty("StringTable", BindingFlags.Public | BindingFlags.Static);

                ui = new MatrixEditControl((StringTable)stProp.GetValue(null, null));                
            }

            //m_ui.SetStates((DockAreas)value);
            ui.Value = (Matrix)value;
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            ui.Service = edSvc;
            edSvc.DropDownControl(ui);
            
            //ui.Value = null;
            Matrix mat = ui.Value;
            ui.Reset();
            return mat;
            //return m_ui.DockAreas;
        }
        public override bool IsDropDownResizable
        {
            get { return true; }
        }
    }
}
