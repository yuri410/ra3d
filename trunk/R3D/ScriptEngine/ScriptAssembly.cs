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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Policy;
using System.Security.Permissions;

using R3D.IO;

namespace R3D.ScriptEngine
{
    /// <summary>
    /// 代表脚本程序集
    /// </summary>
    public sealed class ScriptAssembly : ExternalModule, IDisposable
    {
        public const string ScriptDll = "expand" + Game.Suffix + FileSystem.dotDll;
        public const string ScriptNamespace = "Expand" + Game.Suffix;


        Dictionary<string, Type> types;
        bool disposed;
        Type[] typeArr;

        public static bool HasExtensionDLL()
        {
            try
            {
                FileSystem.Instance.Locate(ScriptDll, FileLocateRule.Root);
                return true;
            }
            catch (System.IO.FileNotFoundException)
            {
                GameConsole.Instance.Write(ResourceAssembly.Instance.CM_ScriptAssemblyNotFound(ScriptDll), ConsoleMessageType.Warning);
                return false;
            }
        }

        void CheckAttributes(object[] attribs)
        {
            for (int i = 0; i < attribs.Length; i++)
            {
                ReflectionPermissionAttribute pa1 = attribs[i] as ReflectionPermissionAttribute;
                if (pa1 != null)
                    throw new ScriptSecurityException(ScriptSecurityType.InvaildSecurityAttribute);

                RegistryPermissionAttribute pa2 = attribs[i] as RegistryPermissionAttribute;
                if (pa2 != null)
                    throw new ScriptSecurityException(ScriptSecurityType.InvaildSecurityAttribute);

                FileIOPermissionAttribute pa3 = attribs[i] as FileIOPermissionAttribute;
                if (pa3 != null)
                    throw new ScriptSecurityException(ScriptSecurityType.InvaildSecurityAttribute);

                SecurityPermissionAttribute pa4 = attribs[i] as SecurityPermissionAttribute;
                if (pa4 != null)
                    throw new ScriptSecurityException(ScriptSecurityType.InvaildSecurityAttribute);

            }
        }

        void CheckReferences()
        {
            AssemblyName[] allowedRefName = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
            int l1 = allowedRefName.Length;
            Assembly[] allowedRefs = new Assembly[l1 + 1];
            for (int i = 0; i < l1; i++)
                allowedRefs[i] = Assembly.Load(allowedRefName[i]);
            allowedRefs[l1] = Assembly.GetExecutingAssembly();
            l1++;

            AssemblyName[] refName = assembly.GetReferencedAssemblies();
            int l2 = refName.Length;
            Assembly[] refs = new Assembly[l2];
            for (int i = 0; i < l2; i++)
                refs[i] = Assembly.Load(refName[i]);

            for (int i = 0; i < l2; i++)
            {
                bool passed = false;
                for (int j = 0; j < l1; j++)
                {
                    if (CaseInsensitiveStringComparer.Compare(refs[i].CodeBase, allowedRefs[j].CodeBase))
                    {
                        passed = true;
                        break;
                    }
                }

                if (!passed)
                    throw new ScriptSecurityException(ScriptSecurityType.ReferenceAssembly);
            }
        }

        protected override void Load()
        {
            CheckReferences();

            object[] attribs = assembly.GetCustomAttributes(false);

            for (int i = 0; i < attribs.Length; i++)
            {
                ReflectionPermissionAttribute pa1 = attribs[i] as ReflectionPermissionAttribute;
                if (pa1 != null)
                {
                    if (pa1.Action != SecurityAction.RequestRefuse ||
                        !pa1.Unrestricted)
                        throw new ScriptSecurityException(ScriptSecurityType.ReflectionPermission);
                    continue;
                }

                RegistryPermissionAttribute pa2 = attribs[i] as RegistryPermissionAttribute;
                if (pa2 != null)
                {
                    if (pa2.Action != SecurityAction.RequestRefuse ||
                        !pa2.Unrestricted)
                        throw new ScriptSecurityException(ScriptSecurityType.RegistryPermission);

                    continue;
                }

                FileIOPermissionAttribute pa3 = attribs[i] as FileIOPermissionAttribute;
                if (pa3 != null)
                {
                    if (pa3.Action != SecurityAction.RequestRefuse)
                        throw new ScriptSecurityException(ScriptSecurityType.FileIOPermission);

                    if (!pa3.Unrestricted && (
                        ((int)(pa3.AllFiles & FileIOPermissionAccess.Append) == 0) ||
                        ((int)(pa3.AllFiles & FileIOPermissionAccess.Write) == 0) ||
                        ((int)(pa3.AllFiles & FileIOPermissionAccess.PathDiscovery) == 0))
                        )
                        throw new ScriptSecurityException(ScriptSecurityType.FileIOPermission);
                    continue;
                }

                SecurityPermissionAttribute pa4 = attribs[i] as SecurityPermissionAttribute;
                if (pa4 != null)
                {
                    if (
                        !(pa4.Action == SecurityAction.RequestRefuse &&
                          pa4.ControlAppDomain &&
                          pa4.ControlDomainPolicy &&
                          pa4.ControlEvidence &&
                          pa4.ControlPolicy &&
                          pa4.ControlPrincipal &&
                          pa4.ControlThread)
                        && !(pa4.Action == SecurityAction.RequestMinimum && pa4.Flags == SecurityPermissionFlag.SkipVerification))
                        throw new ScriptSecurityException(ScriptSecurityType.SecurityPermission);
                    continue;
                }
            }


            Type[] tps = assembly.GetTypes();
            typeArr = tps;

            types = new Dictionary<string, Type>(tps.Length);

            int cCount = 0;

            for (int i = 0; i < tps.Length; i++)
            {
                if (tps[i].IsClass)
                    cCount++;

                CheckAttributes(tps[i].GetCustomAttributes(true));

                MethodInfo[] methods = tps[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                for (int j = 0; j < methods.Length; j++)
                {
                    CheckAttributes(methods[j].GetCustomAttributes(true));

                    MethodImplAttributes mia = methods[j].GetMethodImplementationFlags();

                    if ((methods[j].Attributes & MethodAttributes.PinvokeImpl) != 0)
                        throw new ScriptSecurityException(ScriptSecurityType.PinvokeImpl);

                    if ((mia & MethodImplAttributes.Unmanaged) != 0)
                        throw new ScriptSecurityException(ScriptSecurityType.Unmanaged);

                    if ((mia & MethodImplAttributes.Native) != 0)
                        throw new ScriptSecurityException(ScriptSecurityType.Native);

                    if ((mia & MethodImplAttributes.InternalCall) != 0)
                        throw new ScriptSecurityException(ScriptSecurityType.InternalCall);

                    methods[j] = null;
                }
                methods = null;

                types.Add(tps[i].FullName.Replace('+', Type.Delimiter), tps[i]);
            }

            GameConsole.Instance.Write(ResourceAssembly.Instance.CM_ScriptAssemblyLoaded(assembly.FullName), ConsoleMessageType.Information);
            GameConsole.Instance.Write(ResourceAssembly.Instance.CM_ClassCount(cCount), ConsoleMessageType.Normal);
            GameConsole.Instance.Write(ResourceAssembly.Instance.CM_TypeCount(types.Count), ConsoleMessageType.Normal);
        }

        public ScriptAssembly()
            : base(FileSystem.Instance.Locate(ScriptDll, FileLocateRule.Root), true)
        { }
        public ScriptAssembly(R3D.IO.ResourceLocation fl)
            : base(fl, true) { }

        public Type this[string name]
        {
            get
            {
                Type res;
                types.TryGetValue(ScriptNamespace + Type.Delimiter + name, out res);
                return res;
            }
        }
        public Type[] Types
        {
            get { return typeArr; }
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

        ~ScriptAssembly()
        {
            if (!disposed)
                Dispose();
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (!disposed)
            {
                assembly = null;
                types.Clear();
                types = null;
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        #endregion
    }
}
