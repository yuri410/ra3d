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
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Reflection;

using R3D.UI;
using R3D.IO;
using R3D.Base;
using R3D.Sound;

namespace R3D.ScriptEngine
{
    /// <summary>
    /// 代表一段脚本程序（含参数处理）
    /// </summary>
    public class ScriptProc : IDisposable
    {
        const char ParamSeperator = ',';

        bool disposed;

        MethodInfo method;
        //string name;
        //ArrayList argument;
        bool autoBind;
        //bool isEventProc;

        public MethodInfo Method
        {
            get { return method; }
        }

        public ScriptProc(MethodInfo mi)
        {
            method = mi;
            //argument = arg;

            object[] attrs = mi.GetCustomAttributes(typeof(ScriptBindingAttribute), false);
            if (attrs.Length > 0)
            {
                //isEventProc = true;
                if (((ScriptBindingAttribute)attrs[0]).BindMode == ScriptBinding.Auto)
                {
                    autoBind = true;
                }
            }
        }

        //public ScriptProc(MethodInfo mi)
        //    : this(mi, new ArrayList())
        //{
            //if (!isEventProc)
            //{
                //Game game = Game.Instance;
                //ParameterInfo[] paramInfo = method.GetParameters();

                //for (int i = 0; i < paramInfo.Length; i++)// (ParameterInfo pi in paramInfo)
                //{
                //    Type paramType = paramInfo[i].ParameterType;

                //    object toAdd = null;
                //    if (paramType == typeof(Game))
                //        toAdd = game;
                //    else if (paramType == typeof(GameUI))
                //        toAdd = game.GameUI;
                //    else if (paramType == typeof(Menu))
                //        toAdd = game.GameUI.Menu;
                //    else if (paramType == typeof(Themes))
                //        toAdd = game.Themes;
                //    else
                //        throw new ScriptArgumentException();

                //    if (Game.IsInDebugMode && toAdd == null)
                //    {
                //        GameConsole.Instance.Write(ResourceAssembly.Instance.CM_ObjNotInit(paramType.Name), ConsoleMessageType.Warning);
                //    }
                //    argument.Add(toAdd);
                //}
            //}
        //}
        //public ScriptProc(MethodInfo mi, IList arg)
        //    : this(mi, new ArrayList(arg))
        //{ }

        /// <summary>
        /// 指示ScriptProc对象是否是用于
        /// </summary>
        public bool AutoBind
        {
            get { return autoBind; }
        }
        public int NameHash
        {
            get { return method.Name.GetHashCode(); }
        }
        public string Name
        {
            get { return method.Name; }
        }

        public void Invoke()
        {
            method.Invoke(null, null);//argument.Count == 0 ? null : argument.ToArray());
        }
        //public void Invoke(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    method.Invoke(null, argument.Count == 0 ? null : argument.ToArray());
        //}
      

        #region IDisposable Members

        public void Dispose()
        {
            if (!disposed)
            {
                //argument.Clear();
                //argument = null;
                method = null;
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        #endregion

        public static string GetSign(ParameterInfo[] pm)
        {
            string sep = ", ";
            int l = pm.Length;
            StringBuilder sb = new StringBuilder(l * 2);
            
            for (int i = 0; i < l; i++)
            {
                sb.Append(pm[i].ToString());
                if (i < l - 1)
                    sb.Append(sep);
            }

            return '(' + sb.ToString() + ')';
        }
        public string Sign
        {
            get { return method.Name + GetSign(method.GetParameters()); }
        }
        public override string ToString()
        {
            return "{ Name= " + method.Name + GetSign(method.GetParameters()) + '}';
        }
    }
}
