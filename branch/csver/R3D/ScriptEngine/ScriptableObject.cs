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

using R3D.Base;
using System.Reflection.Emit;

namespace R3D.ScriptEngine
{
    /// <summary>
    /// 代表一个带有脚本描述的对象
    /// </summary>
    public abstract class ScriptableObject : IDisposable
    {
        static readonly string NoName = "NoName";

        Dictionary<string, ScriptProc> methods;
        Dictionary<uint, ScriptProc> hashedMethods;
        List<ScriptProc> listedMethods;


        protected LoadHandler objectLoad;

        protected bool IsLoaded
        {
            get;
            private set;
        }

        public bool Disposed
        {
            get;
            private set;
        }


        [ScriptEvent("objectLoad")]
        public event LoadHandler ObjectLoad
        {
            add { objectLoad += value; }
            remove { objectLoad -= value; }
        }

        protected virtual void OnLoad()
        {
            Bind(false);
            if (objectLoad != null)
            {
                objectLoad(this);
            }
            IsLoaded = true;
        }
        //bool scrLoaded;
        string scriptName;

        ///// <summary>
        ///// 脚本是否已经成功加载
        ///// </summary>
        //public bool IsScriptAvailable
        //{
        //    get { return scrLoaded; }
        //}

        /// <summary>
        /// 包含脚本程序的静态类的名称
        /// </summary>
        public string Name
        {
            get { return scriptName; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="em">脚本程序集</param>
        /// <param name="typeName">脚本程序集中的类型</param>
        protected ScriptableObject(string typeName)
        {
            scriptName = typeName;

            methods = new Dictionary<string, ScriptProc>(10);
            listedMethods = new List<ScriptProc>(10);

            if (typeName != null)
                PostLoad(typeName);
            else
                GameConsole.Instance.Write(ResourceAssembly.Instance.CM_ScriptNotUsed(this), ConsoleMessageType.Exclamation);
        }

        protected ScriptableObject(ScriptableObject so)
        {
            int cnt = so.methods.Count;
            listedMethods = new List<ScriptProc>(cnt);
            hashedMethods = new Dictionary<uint, ScriptProc>(cnt);
            methods = new Dictionary<string, ScriptProc>(cnt);

            listedMethods.AddRange(so.listedMethods);
            for (int i = 0; i < cnt; i++)
            {
                methods.Add(listedMethods[i].Name, listedMethods[i]);
            }
            scriptName = so.scriptName;

            //scrLoaded = true;

        }


        protected void PostLoad(string typeName)
        {
            Game g = Game.Instance;
            //if (!scrLoaded)
            //{
            if (g != null)
            {
                ScriptAssembly em = g.ScriptModule;
                // 如果有脚本程序集
                if (em != null)
                {
                    Type type = em[typeName];

                    if (type != null)
                    {
                        // 获得相应类型的方法
                        MethodInfo[] typeMths = type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);


                        for (int i = 0; i < typeMths.Length; i++)
                        {
                            ScriptProc item = new ScriptProc(typeMths[i]);

                            methods.Add(typeMths[i].Name, item);
                            listedMethods.Add(item);
                        }
                        //scrLoaded = true;

                        ScriptLoaded();
                    }
                }
            }
            //}
            //else
            //    throw new ObjectAlreadyLoadedException(this);

        }

        void ScriptLoaded()
        {
            GameConsole.Instance.Write(ResourceAssembly.Instance.CM_ScriptLoaded(this), ConsoleMessageType.Information);
        }

        /// <summary>
        /// 给对象后期绑定事件脚本
        /// </summary>
        /// <param name="ignoreExisting">忽略已设置的事件</param>
        void Bind(bool ignoreExisting)
        {


            Type thisType = this.GetType();
            EventInfo[] events = thisType.GetEvents(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < events.Length; i++)
            {
                object[] attribs = events[i].GetCustomAttributes(typeof(ScriptEventAttribute), false);

                if (attribs.Length > 0)
                {
                    ScriptEventAttribute seAtt = (ScriptEventAttribute)attribs[0];

                    if (!ignoreExisting)
                    {
                        FieldInfo fi = thisType.GetField(seAtt.EventDelegateName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase);

                        if (fi.GetValue(this) != null)
                            continue;
                    }

                    ScriptProc sp;
                    string mName = events[i].Name + "Impl";

                    if (methods.TryGetValue(mName, out sp) && sp.AutoBind)
                    {
                        ParameterInfo[] scrParam = sp.Method.GetParameters();
                        ParameterInfo[] eventParam = events[i].EventHandlerType.GetMethod("Invoke").GetParameters();

                        if (scrParam.Length != eventParam.Length)
                            throw new InvalidScriptException(
                                ResourceAssembly.Instance.Ex_ScriptSignMismatch(
                                sp.Sign, mName + ScriptProc.GetSign(eventParam)));

                        for (int j = 0; j < scrParam.Length; j++)
                        {
                            if (scrParam[j].ParameterType != eventParam[j].ParameterType)
                            {
                                throw new InvalidScriptException(
                                    ResourceAssembly.Instance.Ex_ScriptSignMismatch(
                                    sp.Sign, mName + ScriptProc.GetSign(eventParam)));
                            }
                        }

                        Delegate scriptDelegate = Delegate.CreateDelegate(events[i].EventHandlerType, sp.Method);
                        events[i].AddEventHandler(this, scriptDelegate);
                    }
                    else
                    {
                        MethodInfo iimp = thisType.GetMethod(mName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                                                
                        if (iimp != null)
                        {
                            Delegate scriptDelegate = Delegate.CreateDelegate(events[i].EventHandlerType, this, iimp);
                            events[i].AddEventHandler(this, scriptDelegate);
                        }
                    }



                } // If have event script settings
            }

        }

        public ScriptProc this[string name]
        {
            get
            {
                //if (!scrLoaded)
                //    throw new ObjectNotInitializedException(this);
                return methods[name];
            }
        }

        public ScriptProc GetHashedProcedure(uint hash)
        {
            //if (!scrLoaded)
            //    throw new ObjectNotInitializedException(this);

            ScriptProc res;
            if (hashedMethods.TryGetValue(hash, out res))
            {
                return res;
            }
            else if (hashedMethods.Count < methods.Count)
            {
                for (int i = 0; i < methods.Count; i++)
                {
                    if (listedMethods[i].NameHash == hash)
                    {
                        hashedMethods.Add(hash, listedMethods[i]);
                        return listedMethods[i];
                    }
                }
                throw new KeyNotFoundException();
            }
            throw new KeyNotFoundException();
        }

        public ScriptProc GetProcedure(string name)
        {
            //if (!scrLoaded)
            //    throw new ObjectNotInitializedException(this);

            return methods[name];
        }

        public ScriptProc GetProcedure(int i)
        {
            //if (!scrLoaded)
            //    throw new ObjectNotInitializedException(this);

            return listedMethods[i];
        }


        ///// <summary>
        ///// 给带内部实现的UI类中未设置的的事件附上内部实现
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <remarks>
        ///// 尝试绑定顺序：
        /////  用户直接定义
        /////  后期绑定脚本
        /////  内部实现
        ///// </remarks>
        //protected static void Bind(IInternalImplUI obj, bool autoLoad)
        //{
        //    //Bind(false);
        //    ((ScriptableObject)obj).Bind(false);

        //    if (obj.LoadHandler == null)
        //        obj.ObjectLoad += obj.OnLoadInternal;

        //    if (autoLoad)
        //        obj.LoadHandler(obj);

        //    if (obj.MouseDownHandler == null)
        //        obj.MouseDown += obj.OnMouseDownInternal;
        //    if (obj.MouseMoveHandler == null)
        //        obj.MouseMove += obj.OnMouseMoveInternal;
        //    if (obj.MouseUpHandler == null)
        //        obj.MouseUp += obj.OnMouseUpInternal;
        //    if (obj.PaintHandler == null)
        //        obj.Paint += obj.OnPaintInternal;


        //}
        //protected static void Bind(ILoadableInternalImpl obj)
        //{
        //    ((ScriptableObject)obj).Bind(false);

        //    if (obj.LoadHandler == null)
        //        obj.ObjectLoad += obj.OnLoadInternal;

        //}

        /// <summary>
        /// 给一般性脚本可控制对象 后期绑定脚本
        /// </summary>
        /// <remarks>
        /// 尝试绑定顺序：
        ///  用户直接定义
        ///  后期绑定脚本
        /// </remarks>
        protected void Bind()
        {
            Bind(false);
        }





        public virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                for (int i = 0; i < listedMethods.Count; i++)
                    listedMethods[i].Dispose();
            }
            objectLoad = null;
            listedMethods = null;
        }

        

        #region IDisposable 成员

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion

        ~ScriptableObject()
        {
            if (!Disposed)
            {
                Dispose();
            }
        }

        public override string ToString()
        {
            string res = scriptName;
            if (res == null)                 
                res = NoName;
            return res;
        }
    }


}
