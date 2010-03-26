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
using R3D.IO;

namespace R3D.ScriptEngine
{
    public class ScriptManager
    {
        const string ScriptNamespace = "Expand" + Game.Suffix;
        const string ScriptDll = "expand" + Game.Suffix + "*" + FileSystem.dotDll;

        static ScriptManager singleton;

        public static ScriptManager Instance
        {
            get { return singleton; }
        }

        public static void Initialize()
        {
            singleton = new ScriptManager();
        }


        bool disposed;
        Dictionary<string, Type> types;

        List<ScriptAssembly> modules;

        public ScriptManager()
        {
            types = new Dictionary<string, Type>();


            string[] dlls = FileSystem.Instance.SearchFile(ScriptDll);
            modules = new List<ScriptAssembly>(dlls.Length + 5);

            for (int i = 0; i < dlls.Length; i++)
            {
                modules.Add(new ScriptAssembly(new FileLocation(dlls[i])));
            }

            for (int i = 0; i < modules.Count; i++)
            {
                Type[] asTypes = modules[i].Types;
                for (int j = 0; j < asTypes.Length; j++)
                {
                    types.Add(asTypes[i].FullName, asTypes[i]);
                }
            }
        }

        public Type this[string typeName]
        {
            get
            {
                Type res;
                types.TryGetValue(ScriptNamespace + Type.Delimiter + typeName, out res);
                return res;
            }
        }

        public bool GetProcedure(string name, out ScriptProc proc)
        {
            //name = ScriptNamespace + name;
            int pos = name.LastIndexOf(Type.Delimiter);
            Type type = this[name.Substring(0, pos)];
            if (type != null)
            {
                MethodInfo mi = type.GetMethod(name.Substring(pos), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                if (mi != null)
                {
                    proc = new ScriptProc(mi);
                    return true;
                }
                proc = null;
                return false;
            }
            proc = null;
            return false;
        }
        public ScriptProc GetProcedure(string name)
        {
            //name = ScriptNamespace + name;
            int pos = name.LastIndexOf(Type.Delimiter);
            Type type = this[name.Substring(0, pos)]; // Type.GetType(name.Substring(0, pos), true);

            ScriptProc proc = new ScriptProc(
                type.GetMethod(name.Substring(pos), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));
            return proc;
        }
        ~ScriptManager()
        {
            if (!disposed)
                Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!disposed)
            {
                types.Clear();
                for (int i = 0; i < modules.Count; i++)
                {
                    modules[i].Dispose();
                }
                modules.Clear();
                disposed = true;
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        #endregion
    }
}
