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
using Ra2Develop.Tools;

namespace Ra2Develop
{
    public class ToolManager
    {
        static ToolManager singleton;

        public static ToolManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ToolManager();
                return singleton;
            }
        }

        Dictionary<Type, IToolAbstractFactory> factories;
        Dictionary<string, IToolAbstractFactory> factories2;

        

        private ToolManager()
        {
            factories = new Dictionary<Type, IToolAbstractFactory>();
            factories2 = new Dictionary<string, IToolAbstractFactory>();
        }

        public ITool CreateTool(string typeName)
        {
            IToolAbstractFactory fac;
            if (factories2.TryGetValue(typeName, out fac))
            {
                return fac.CreateInstance();
            }
            else
                throw new NotSupportedException();
        }
        public ITool CreateTool(Type type)
        {
            IToolAbstractFactory fac;
            if (factories.TryGetValue(type, out fac))
            {
                return fac.CreateInstance();
            }
            else
                throw new NotSupportedException();
        }

        public void RegisterToolType(IToolAbstractFactory fac)
        {
            factories.Add(fac.CreationType, fac);
            factories2.Add(fac.CreationType.ToString(), fac);
        }
        public void UnregisterToolType(IToolAbstractFactory fac)
        {
            factories.Remove(fac.CreationType);
            factories2.Remove(fac.CreationType.ToString());

        }
    }
}
