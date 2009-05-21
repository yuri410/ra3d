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
using R3D.UI.Controls;

namespace R3D.UI
{
    public abstract class ControlFactory
    {

        protected ControlFactory(string typeName)
        {
            TypeName = typeName;
        }

        public abstract Control CreateInstance(GameUI gameUI, string name);

        public string TypeName
        {
            get;
            private set;
        }
    }
   
    public class ControlManager
    {
        static ControlManager singleton;

        public static ControlManager Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new ControlManager();
                return singleton;
            }
        }

        Dictionary<string, ControlFactory> factories;

        private ControlManager()
        {
            factories = new Dictionary<string, ControlFactory>(CaseInsensitiveStringComparer.Instance);
        }

        public void RegisterControlType(string typeName, ControlFactory fac)
        {
            factories.Add(typeName, fac);
        }

        public void UnregisterControlType(string typeName)
        {
            factories.Remove(typeName);
        }

        public void UnregisterControlType(ControlFactory fac)
        {
            factories.Remove(fac.TypeName);
        }


        public Control CreateControl(GameUI gameUI, string type, string name)
        {
            ControlFactory fac;
            if (factories.TryGetValue(type, out fac))
            {
                return fac.CreateInstance(gameUI, name);
            }
            else
            {
                throw new NotSupportedException(type);
            }
        }
    }
}
