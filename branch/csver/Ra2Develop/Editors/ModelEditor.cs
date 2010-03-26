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
using System.Text;
using System.Windows.Forms.Design;
using Ra2Develop.Editors.EditableObjects;
using R3D.GraphicsEngine;
using R3D.IsoMap;

namespace Ra2Develop.Editors
{
    /// <summary>
    /// 为模型添加实体，导入 等功能
    /// </summary>
    public class ModelEditor : UITypeEditor
    {
        ModelEditControl ui;
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (ui == null)
                ui = new ModelEditControl();

            //m_ui.SetStates((DockAreas)value);
            ui.Value = (EditableModel)value;
            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            ui.Service = edSvc;
            edSvc.DropDownControl(ui);

            ui.Service = null;
            EditableModel result = ui.Value;
            ui.Value = null;
            return result;
            //return m_ui.DockAreas;
        }
    }

    //public class EditableBlockModel
    //{
    //    GameModel sounds;
    //    public EditableBlockModel(GameModel d)
    //    {
    //        if (d == null)
    //        {
    //            throw new ArgumentNullException();
    //        }
    //        sounds = d;
    //    }

    //    [Browsable(false)]
    //    public GameModel Data
    //    {
    //        get { return sounds; }
    //        set { sounds = value; }
    //    }

    //    [TypeConverter(typeof(BlockMeshesTypeConverter))]
    //    [Editor(typeof(BlockModelArrayEditor), typeof(UITypeEditor))]
    //    public GameMesh[] Entities
    //    {
    //        get { return sounds.Entities; }
    //        set { sounds.Entities = value; }
    //    }
    //}

}
