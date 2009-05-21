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
using System.ComponentModel;
using Ra2Develop.Editors.EditableObjects;

namespace Ra2Develop.Editors
{
    public class MeshMaterialConverter : ExpandableObjectConverter
    {
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            string[] props = new string[]
            {
                "Texture1", "Texture1Embeded", "TextureFile1",
                "Texture2", "Texture2Embeded", "TextureFile2",
                "Texture3", "Texture3Embeded", "TextureFile3",
                "Texture4", "Texture4Embeded", "TextureFile4",
                "Flags", "IsTwoSided", "IsTransparent",
                "Power", "Ambient", "Diffuse", "Specular", "Emissive" 
            };

            return TypeDescriptor.GetProperties(typeof(EditableMeshMaterial), attributes).Sort(props);
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
