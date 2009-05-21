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

namespace Ra2Reload.Config
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigurableAttribute : Attribute
    {
        bool configurable;

        public ConfigurableAttribute(bool configurable)
        {
            this.configurable = configurable;
        }

        public bool IsConfigurable
        {
            get { return configurable; }
            set { configurable = value; }
        }
    }

    /// <summary>
    /// 给对象的属性绑定配制文件(ini)关键字
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class ConfigKeywordAttribute : Attribute
    {

        string configKeyword;

        public ConfigKeywordAttribute(string configKey)
        {
            this.configKeyword = configKey;
        }

        public string ConfigKeyword
        {
            get { return configKeyword; }
            set{configKeyword = value;}
        }

    }
}
