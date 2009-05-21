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

namespace Ra2Reload.Config
{
    public static class ConfigHelper
    {
        public static void Configurate(ConfigurationSection sect, IConfigurable obj)
        {
            Type type = obj.GetType();
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < props.Length; i++)
            {
                if (props[i].CanWrite)
                {
                    bool p1 = false;
                    bool p2 = false;

                    string conName = props[i].Name;
                    bool conable = false;
                    object[] attributes = props[i].GetCustomAttributes(true);
                  
                    for (int j = 0; j < attributes.Length; j++)
                    {
                        ConfigurableAttribute a1 = attributes[i] as ConfigurableAttribute;


                        if (a1 != null)
                        {
                            p1 = true;
                            conable = a1.IsConfigurable;
                        }
                        else
                        {
                            ConfigKeywordAttribute a2 = attributes[i] as ConfigKeywordAttribute;
                            if (a2 != null)
                            {
                                p2 = true;
                                conName = a2.ConfigKeyword;
                            }
                        }
                        if (p1 & p2)
                        {
                            break;
                        }
                    }

                    if (conable)
                    {
                        
                    }
                }
            }
            FieldInfo[] flds = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            for (int i = 0; i < flds.Length; i++)
            {
                if (!flds[i].IsInitOnly)
                {

                }                
            }
        }
    }
}
