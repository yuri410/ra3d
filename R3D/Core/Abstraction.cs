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
using System.Reflection;

namespace R3D.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class FactoryAttribute : Attribute
    {
        public string Name
        {
            get;
            private set;
        }
        public Type ManagerType
        {
            get;
            private set;
        }
        public Type ProductType
        {
            get;
            private set;
        }

        // This is a positional argument
        public FactoryAttribute(string name, Type managerType, Type productType)
        {
            this.ProductType = productType;
            this.ManagerType = managerType;
            this.Name = name;
        }

    }

    public abstract class AbstractFactoryManager
    {
        protected delegate void FactoryFoundCallback<FType>(FType fac) where FType : class;

        protected void SearchForFactories<FType>(Assembly assm, FactoryFoundCallback<FType> cbk)
            where FType : class
        {

            Type[] types = assm.GetTypes();

            Type myType = this.GetType();

            for (int i = 0; i < types.Length; i++)
            {
                object[] atts = types[i].GetCustomAttributes(typeof(FactoryAttribute), false);

                if (atts.Length > 0)
                {
                    FactoryAttribute fatt = (FactoryAttribute)atts[0];
                    if (fatt.ManagerType == myType)
                    {
                        ConstructorInfo cinfo = types[i].GetConstructor(Type.EmptyTypes);
                        if (cinfo != null)
                        {
                            cbk((FType)cinfo.Invoke(null));
                        }                      

                        //Activator.CreateInstance(types[i], null);
                    }
                }
            }
        }
    }
}
