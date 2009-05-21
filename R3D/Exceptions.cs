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

using R3D.IO;

namespace R3D
{
    [Serializable]
    public class ScriptException : Exception
    {
        public ScriptException() : base(ResourceAssembly.Instance.Ex_ScriptError) { }
        public ScriptException(string message) : base(message) { }
    }

    [Serializable]
    public class InvalidScriptException : ScriptException
    {
        public InvalidScriptException() { }
        public InvalidScriptException(string message) : base(message) { }
    }

    public enum ScriptSecurityType
    {
        InvaildSecurityAttribute,
        ReflectionPermission,
        RegistryPermission,
        FileIOPermission,
        SecurityPermission,
        PinvokeImpl,
        Unmanaged,
        Native,
        InternalCall,
        ReferenceAssembly

    }

    [Serializable]
    public class ScriptSecurityException : ScriptException
    {

        public ScriptSecurityException(ScriptSecurityType sst, params object[] param) 
        {
            switch (sst)
            {
                case ScriptSecurityType.FileIOPermission:
                    break;
                case ScriptSecurityType.ReflectionPermission:
                    break;
                case ScriptSecurityType.RegistryPermission:
                    break;
                case ScriptSecurityType.SecurityPermission:
                    break;
                case ScriptSecurityType.InvaildSecurityAttribute:
                    break;
                case ScriptSecurityType.InternalCall:
                    break;
                case ScriptSecurityType.Native:
                    break;
                case ScriptSecurityType.PinvokeImpl:
                    break;
                case ScriptSecurityType.Unmanaged:
                    break;
                case ScriptSecurityType.ReferenceAssembly:
                    break;
            }
        }
    }
    [Serializable]
    public class ScriptArgumentException : Exception
    {

        public ScriptArgumentException() { }
        public ScriptArgumentException(string message) : base(message) { }
        public ScriptArgumentException(string message, Exception inner) : base(message, inner) { }
        protected ScriptArgumentException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ObjectAlreadyLoadedException : Exception
    {
        public ObjectAlreadyLoadedException(object obj) : base(ResourceAssembly.Instance.Ex_ObjectAlreadyLoaded(obj)) { }       
    }

    [Serializable]
    public sealed class ObjectNotInitializedException : Exception
    {
        public ObjectNotInitializedException(object obj) : base(ResourceAssembly.Instance.Ex_ObjectNotInit(obj)) { }
    }

  

    [Serializable]
    public class DataFormatException : Exception 
    {

        public DataFormatException(string desc) : base(ResourceAssembly.Instance.Ex_InvaildFileFormat(desc)) { }
        public DataFormatException(ResourceLocation rl)
            : this(rl.ToString()) { }
    }



}
