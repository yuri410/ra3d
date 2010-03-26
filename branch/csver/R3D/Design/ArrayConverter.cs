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
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace R3D.Design
{
    public class ArrayConverter<ItemEditorType, ItemType> : TypeConverter
        where ItemEditorType : UITypeEditor
    {
        class ArrayPropertyDescriptor : TypeConverter.SimplePropertyDescriptor
        {
            // Fields
            private int index;

            // Methods
            public ArrayPropertyDescriptor(Type arrayType, Type elementType, int index)
                : base(arrayType, "[" + index + "]", elementType, new Attribute[] { new EditorAttribute(typeof(ItemEditorType), typeof(UITypeEditor)) })
            {
                this.index = index;
            }

            public override object GetValue(object instance)
            {
                ItemType[] list = instance as ItemType[];
                if (instance != null)
                {
                    if (list.Length > this.index)
                    {
                        return list[this.index];
                    }
                }
                return null;
            }

            public override void SetValue(object instance, object value)
            {
                ItemType[] list = instance as ItemType[];

                if (instance != null)
                {
                    if (list.Length > this.index)
                    {
                        list[this.index] = (ItemType)value;
                    }
                    this.OnValueChanged(instance, EventArgs.Empty);
                }
            }
        }
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            PropertyDescriptor[] properties = null;
            ItemType[] list = value as ItemType[];
            if (list != null)
            {
                int length = list.Length;
                properties = new PropertyDescriptor[length];
                for (int i = 0; i < length; i++)
                {
                    properties[i] = new ArrayPropertyDescriptor(typeof(ItemType[]), typeof(ItemType), i);
                }
            }
            return new PropertyDescriptorCollection(properties);
        }
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
