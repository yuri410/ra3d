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
using System.Resources;
using System.IO;

using R3D.IO;

namespace R3D
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>单例</remarks>
    public sealed class ResourceAssembly : ExternalModule, IDisposable
    {
        const string ResourceDllName = "LangResource";
        const string ResourceDll = "LangResource" + FileSystem.dotDll;

        const string ResourceBaseName = "LangResource.Resources";

        static ResourceAssembly singleton;

        public static ResourceAssembly Instance
        {
            get
            {
                if (singleton == null)
                    CreateInstance();
                return singleton;
            }
        }
        private static void CreateInstance() 
        {
            singleton = new ResourceAssembly();
        }

        ResourceManager resMgr;

        private ResourceAssembly()
            : base(FileSystem.Instance.Locate(ResourceDll, FileLocateRule.Root), false)
        { }

        private ResourceAssembly(R3D.IO.ResourceLocation fl)
            : base(fl, false) { }

        protected override void Load()
        {
            resMgr = new ResourceManager(ResourceBaseName, assembly);
            resMgr.IgnoreCase = true;
            
        }
        public string GetString(string name)
        {
            return resMgr.GetString(name);
        }

        #region IDisposable 成员

        public void Dispose()
        {
            resMgr.ReleaseAllResources();
            resMgr = null;
        }

        #endregion

        /// <summary>
        ///   查找类似 类数：{0} 的本地化字符串。
        /// </summary>
        public string CM_ClassCount(int count)
        {
            return string.Format(resMgr.GetString("CM_ClassCount"), count.ToString());
        }
        /// <summary>
        ///   查找类似 文件未找到：{0}。 的本地化字符串。
        /// </summary>
        public string CM_FileNotFonud(string file)
        {
            return string.Format(resMgr.GetString("CM_FileNotFonud"), file);
        }
        /// <summary>
        ///   查找类似 [FMOD错误]  的本地化字符串。
        /// </summary>
        public string CM_FMODErr(string desc)
        {
            return resMgr.GetString("CM_FMODErr") + desc;
        }
        /// <summary>
        ///   查找类似 [{0}] INI中段落存在多个相同的关键字{1}，将使用关键字最后一个非空的值。 的本地化字符串。
        /// </summary>
        public string CM_IniKeywordRep(string ini, string seg, string keyword)
        {
            return string.Format(resMgr.GetString("CM_IniKeywordRep"), Path.GetFileName(ini), seg, keyword);
        }
        /// <summary>
        ///   查找类似 [{0}] INI中名称相同的段落尝试合并时发生错误：{1} 的本地化字符串。
        /// </summary>
        public string CM_IniSegmentConcatErr(string ini, string seg, string err)
        {
            return string.Format(resMgr.GetString("CM_IniSegmentConcatErr"), Path.GetFileName(ini), seg, err);
        }
        /// <summary>
        ///   查找类似 [{0}] INI中名称相同的段落尝试合并时有名称将同的关键字{1}，忽略。 的本地化字符串。
        /// </summary>
        public string CM_IniSegmentConcatKeyRep(string ini, string seg, string key)
        {
            return string.Format(resMgr.GetString("CM_IniSegmentConcatKeyRep"), Path.GetFileName(ini), seg, key);
        }
        /// <summary>
        ///   查找类似 [{0}] INI中存在多个名称相同的段落{1}，名称相同的段落将尝试合并。 的本地化字符串。
        /// </summary>
        public string CM_IniSegmentRep(string ini, string segment)
        {
            return string.Format(resMgr.GetString("CM_IniSegmentRep"), Path.GetFileName(ini), segment);
        }
        /// <summary>
        ///   查找类似 ra2{0}.ini中屏幕分辨率设置无效。恢复至800x600。 的本地化字符串。
        /// </summary>
        public string CM_InvaildResol(string suffix)
        {
            return string.Format(resMgr.GetString("CM_InvaildResol"), suffix);
        }
        /// <summary>
        ///   查找类似 方法数：{0} 的本地化字符串。
        /// </summary>
        public string CM_MethodCount(int count)
        {
            return string.Format(resMgr.GetString("CM_MethodCount"), count.ToString());
        }
        /// <summary>
        ///   查找类似 对象{0}未实例化(ScriptProc) 的本地化字符串。
        /// </summary>
        public string CM_ObjNotInit(string obj)
        {
            return string.Format(resMgr.GetString("CM_ObjNotInit"), obj);
        }
        /// <summary>
        ///   查找类似 脚本程序集已读取：{0} 的本地化字符串。
        /// </summary>
        public string CM_ScriptAssemblyLoaded(string fullname)
        {
            return string.Format(resMgr.GetString("CM_ScriptAssemblyLoaded"), fullname);
        }
        /// <summary>
        ///   查找类似 未找到脚本程序集 {0} 的本地化字符串。
        /// </summary>
        public string CM_ScriptAssemblyNotFound(string file)
        {
            return string.Format(resMgr.GetString("CM_ScriptAssemblyNotFound"), file);
        }
        /// <summary>
        ///   查找类似 Timer已启动：{0} 的本地化字符串。
        /// </summary>
        public string CM_TimerStarted(string time)
        {
            return string.Format(resMgr.GetString("CM_TimerStarted"), time);
        }
        /// <summary>
        ///   查找类似 类型数：{0} 的本地化字符串。
        /// </summary>
        public string CM_TypeCount(int count)
        {
            return string.Format(resMgr.GetString("CM_TypeCount"), count.ToString());
        }
        /// <summary>
        ///   查找类似 使用默认设置。 的本地化字符串。
        /// </summary>
        public string CM_UseDefaultSet
        {
            get
            {
                return resMgr.GetString("CM_UseDefaultSet");
            }
        }

        /// <summary>
        ///   查找类似 控制台 的本地化字符串。
        /// </summary>
        public string Console
        {
            get
            {
                return resMgr.GetString("Console");
            }
        }
        /// <summary>
        ///   查找类似 提交 的本地化字符串。
        /// </summary>
        public string Console_Submit
        {
            get
            {
                return resMgr.GetString("Console_Submit");
            }
        }
        /// <summary>
        ///   查找类似 控制台： 的本地化字符串。
        /// </summary>
        public string ExPrint_Console
        {
            get
            {
                return resMgr.GetString("ExPrint_Console");
            }
        }
        /// <summary>
        ///   查找类似 附加数据： 的本地化字符串。
        /// </summary>
        public string ExPrint_ExtraData
        {
            get
            {
                return resMgr.GetString("ExPrint_ExtraData");
            }
        }
        /// <summary>
        ///   查找类似 局部变量： 的本地化字符串。
        /// </summary>
        public string ExPrint_LocalVars
        {
            get
            {
                return resMgr.GetString("ExPrint_LocalVars");
            }
        }
        /// <summary>
        ///   查找类似 方法： 的本地化字符串。
        /// </summary>
        public string ExPrint_Method
        {
            get
            {
                return resMgr.GetString("ExPrint_Method");
            }
        }
        /// <summary>
        ///   查找类似 脚本错误： 的本地化字符串。
        /// </summary>
        public string ExPrint_ScriptError
        {
            get
            {
                return resMgr.GetString("ExPrint_ScriptError");
            }
        }
        /// <summary>
        ///   查找类似 源： 的本地化字符串。
        /// </summary>
        public string ExPrint_Source
        {
            get
            {
                return resMgr.GetString("ExPrint_Source");
            }
        }
        /// <summary>
        ///   查找类似 时间： 的本地化字符串。
        /// </summary>
        public string ExPrint_Time
        {
            get
            {
                return resMgr.GetString("ExPrint_Time");
            }
        }
        /// <summary>
        ///   查找类似 版本： 的本地化字符串。
        /// </summary>
        public string ExPrint_Version
        {
            get
            {
                return resMgr.GetString("ExPrint_Version");
            }
        }

        /// <summary>
        ///   查找类似 注意： 的本地化字符串。
        /// </summary>
        public string MsgLvl_Attention
        {
            get
            {
                return resMgr.GetString("MsgLvl_Attention");
            }
        }
        /// <summary>
        ///   查找类似 错误： 的本地化字符串。
        /// </summary>
        public string MsgLvl_Error
        {
            get
            {
                return resMgr.GetString("MsgLvl_Error");
            }
        }
        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
        public string MsgLvl_Information
        {
            get
            {
                return resMgr.GetString("MsgLvl_Information");
            }
        }
        /// <summary>
        ///   查找类似  的本地化字符串。
        /// </summary>
        public string MsgLvl_Normal
        {
            get
            {
                return resMgr.GetString("MsgLvl_Normal");
            }
        }
        /// <summary>
        ///   查找类似 警告： 的本地化字符串。
        /// </summary>
        public string MsgLvl_Warning
        {
            get
            {
                return resMgr.GetString("MsgLvl_Warning");
            }
        }

        /// <summary>
        ///   查找类似 [缺少记录] 的本地化字符串。
        /// </summary>
        public string StrTable_Missing
        {
            get
            {
                return resMgr.GetString("StrTable_Missing");
            }
        }
        /// <summary>
        ///   查找类似 程序集己加密：{0} 的本地化字符串。
        /// </summary>
        public string CM_AssemblyEncrypted(string file)
        {
            return string.Format(resMgr.GetString("CM_AssemblyEncrypted"), file);
        }       
        /// <summary>
        ///   查找类似 对象{0}未初始化。 的本地化字符串。
        /// </summary>
        public string Ex_ObjectNotInit(object obj)
        {
            return string.Format(resMgr.GetString("Ex_ObjectNotInit"), obj.ToString());
        }
        /// <summary>
        ///   查找类似 对象{0}已经加载。 的本地化字符串。
        /// </summary>
        public string Ex_ObjectAlreadyLoaded(object obj)
        {
            return string.Format(resMgr.GetString("Ex_ObjectAlreadyLoaded"), obj.ToString());
        }
        /// <summary>
        ///   查找类似 文件{0}的格式无效 的本地化字符串。
        /// </summary>
        public string Ex_InvaildFileFormat(string file)
        {
            return string.Format(resMgr.GetString("Ex_InvaildFileFormat"), file);
        }
        /// <summary>
        ///   查找类似 脚本错误 的本地化字符串。
        /// </summary>
        public string Ex_ScriptError
        {
            get
            {
                return resMgr.GetString("Ex_ScriptError");
            }
        }
        /// <summary>
        ///   查找类似 脚本程序的签名不符合要求。脚本程序{0}签名应为{1}。 的本地化字符串。
        /// </summary>
        public string Ex_ScriptSignMismatch(string scr, string required)
        {
            return string.Format(resMgr.GetString("Ex_ScriptSignMismatch"), scr, required);
        }
        /// <summary>
        ///   查找类似 未给对象{0}加载脚本。 的本地化字符串。
        /// </summary>
        public  string CM_ScriptNotLoaded(object obj)
        {
            return string.Format(resMgr.GetString("CM_ScriptNotLoaded"), obj.GetType().Name + ':' + obj.ToString());
        }
        /// <summary>
        ///   查找类似 未给对象{0}加载脚本。因为名称为null。 的本地化字符串。
        /// </summary>
        public string CM_ScriptNotUsed(object obj)
        {
            return string.Format(resMgr.GetString("CM_ScriptNotUsed"), obj.GetType().Name + ':' + obj.ToString());
        }

        /// <summary>
        ///   查找类似 找不到地块文件{0} 的本地化字符串。
        /// </summary>
        public string CM_TileFileMissing(string file)
        {
            return string.Format(resMgr.GetString("CM_TileFileMissing"), file);
        }

        public string CM_MissingTileImage(string p)
        {
            return string.Format(resMgr.GetString("CM_MissingTileImage"), p);
        }

        public string CM_SoundNotFound(string p)
        {
            return string.Format(resMgr.GetString("CM_SoundNotFound"), p);            
        }

        /// <summary>
        ///   查找类似 线程： 的本地化字符串。
        /// </summary>
        public string ExPrint_Thread
        {
            get
            {
                return resMgr.GetString("ExPrint_Thread");
            }
        }

        public string CM_ScriptLoaded(object obj)
        {
            return string.Format(resMgr.GetString("CM_ScriptLoaded"), obj.GetType().Name + ':' + obj.ToString());
        }

        public string CM_IniParentMissing(string sect, string parent)
        {
            return string.Format(resMgr.GetString("CM_IniParentMissing"), sect, parent);
        }
    }
}
