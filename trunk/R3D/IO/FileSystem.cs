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
using System.IO;

namespace R3D.IO
{
    /// <summary>
    /// 文件定位规则
    /// </summary>
    [Flags()]
    public enum FileLocateRule : int
    {
        Root = 1 << 0,
        Standard = 1 << 1,
        Expand = 1 << 2,
        ExTheme = 1 << 3,
        ExLang = 1 << 4,
        CurrentArchiveSet = 1 << 5
    }


    /// <summary>
    /// 定位，检索文件
    /// 管理 包文件
    /// </summary>
    /// <remarks>
    /// 每次访问时检索，对于复合mix分层检测。
    /// </remarks> 
    public class FileSystem : IDisposable
    {
        public const string dotIni = ".ini";
        public const string dotCsf = ".csf";
        public const string dotPng = ".png";
        public const string dotPal = ".pal";
        public const string dotMix = ".mix";
        public const string dotShp = ".shp";
        public const string dotWav = ".wav";
        public const string dotDll = ".dll";

        public const string dotTxt = ".txt";
        public const string dotBag = ".bag";
        public const string dotIdx = ".idx";

        

        public static readonly string Ra2_Csf;

        public static readonly string Ra2_Ini;
        public static readonly string Keyboard_Ini;

        public static readonly string LangMix;
        public static readonly string Ra2_Mix;
        public static readonly string Theme_Mix;

        public static readonly string Multi_Mix;
        //public static readonly string Multi_MixArc;

        public static readonly string Expand_Mix;
        public static readonly string Ecache_Mix;
        public static readonly string ExTheme_Mix;
        public static readonly string ExLang_Mix;


        public static readonly string Audio_Mix;
        public static readonly string Cameo_Mix;
        public static readonly string CameoOld_Mix;

        public static readonly string LocalMix;
        public static readonly string CacheMix;
        public static readonly string CacheOldMix;
        public static readonly string ConquerMix;
        public static readonly string NeutralMix;
        public static readonly string LoadMix;
        

        public static readonly string Audio_Idx;
        public static readonly string Audio_Bag;

        public static readonly string Theme_Ini;
        public static readonly string Sound_Ini;
        public static readonly string UI_Ini;
        public static readonly string Rule_Ini;
        public static readonly string Art_Ini;

        public static readonly string Eva_Ini;

        static FileSystem singleton;

        static readonly char[] DirSepCharArray = new char[] { Path.DirectorySeparatorChar };

        static FileSystem()
        {
            Ra2_Ini = "ra2" + Game.Suffix + dotIni;
            Keyboard_Ini = "keyboard" + Game.Suffix + dotIni;

            // resources
            LangMix = "lang" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            Ra2_Mix = "ra2" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            Theme_Mix = "theme" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            Multi_Mix = "multi" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            //Multi_MixArc = "multi" + Game.Suffix + dotMix;

            Expand_Mix = "expand" + Game.Suffix + "*" + dotMix; // +Path.DirectorySeparatorChar;
            Ecache_Mix = "ecache" + Game.Suffix + "*" + dotMix;
            ExTheme_Mix = "extheme" + Game.Suffix + "*" + dotMix;
            ExLang_Mix = "exlang" + Game.Suffix + "*" + dotMix;

            Audio_Mix = LangMix + "audio" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            Cameo_Mix = LangMix + "cameo" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            CameoOld_Mix = "language.mix" + Path.DirectorySeparatorChar + "cameo" + dotMix + Path.DirectorySeparatorChar;

            LocalMix = Ra2_Mix + "local" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            NeutralMix = Ra2_Mix + "ntrl" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            CacheMix = Ra2_Mix + "cache" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            CacheOldMix = "ra2.mix" + Path.DirectorySeparatorChar + "cache" + dotMix + Path.DirectorySeparatorChar;
            LoadMix = Ra2_Mix + "load" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            ConquerMix = Ra2_Mix + "conq" + Game.Suffix + dotMix + Path.DirectorySeparatorChar;
            // ==================================================================================

            Audio_Bag = Audio_Mix + "audio" + dotBag;
            Audio_Idx = Audio_Mix + "audio" + dotIdx;
            // ui


            Ra2_Csf = LangMix + "ra2" + Game.Suffix + dotCsf;

            // ini
            Sound_Ini = LocalMix + "sound" + Game.Suffix + dotIni;
            UI_Ini = LocalMix + "ui" + Game.Suffix + dotIni;
            Theme_Ini = LocalMix + "theme" + Game.Suffix + dotIni;
            Rule_Ini = LocalMix + "rules" + Game.Suffix + dotIni;
            Art_Ini = LocalMix + "art" + Game.Suffix + dotIni;
            Eva_Ini = LocalMix + "eva" + Game.Suffix + dotIni;
        }

        public static FileSystem Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new FileSystem();
                return singleton;
            }
        }

        //List<Archive> stdPack;

        Dictionary<string, Archive> stdPack;

        List<Archive> expPack;
        List<string> workingDirs;
        List<Archive> exLang;
        List<Archive> exTheme;


        List<string> expDirs;
        List<string> exLangDirs;
        List<string> exThemeDirs;


        Dictionary<string, IArchiveFactory> factories;

        SpecialFiles specialFiles;

        bool disposed;

        protected FileSystem()
        {
            factories = new Dictionary<string, IArchiveFactory>(CaseInsensitiveStringComparer.Instance);

            //stdPack = new List<Archive>();
            stdPack = new Dictionary<string, Archive>(50, CaseInsensitiveStringComparer.Instance);

            workingDirs = new List<string>();
            expPack = new List<Archive>();
            exTheme = new List<Archive>();
            exLang = new List<Archive>();

            CurrentArchiveSet = new List<Archive>();

            expDirs = new List<string>();
            exLangDirs = new List<string>();
            exThemeDirs = new List<string>();

            RegisterArchiveType(new MixArchiveFactory());
        }

        public void AddWorkingDir(string path)
        {
            path = Path.GetFullPath(path);
            if (!path.EndsWith(new string(Path.DirectorySeparatorChar, 1)))
                path += Path.DirectorySeparatorChar;
            workingDirs.Add(path);
            LoadWorkingDir(path);
        }

        void LoadWorkingDir(string path)
        {
            string dirSeparatorChar = new string(Path.DirectorySeparatorChar, 1);

            string[] exs = Directory.GetFiles(path, Expand_Mix);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
                expPack.Add(new MixArchive(exs[j]));

            exs = Directory.GetDirectories(path, Expand_Mix, SearchOption.TopDirectoryOnly);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
            {
                if (!exs[j].EndsWith(dirSeparatorChar))
                    exs[j] += Path.DirectorySeparatorChar;
                expDirs.Add(exs[j]);
            }


            exs = Directory.GetFiles(path, Ecache_Mix);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
                expPack.Add(new MixArchive(exs[j]));

            exs = Directory.GetDirectories(path, Ecache_Mix, SearchOption.TopDirectoryOnly);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
            {
                if (!exs[j].EndsWith(dirSeparatorChar))
                    exs[j] += Path.DirectorySeparatorChar;
                expDirs.Add(exs[j]);
            }


            exs = Directory.GetFiles(path, ExLang_Mix);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
                exLang.Add(new MixArchive(exs[j]));

            exs = Directory.GetDirectories(path, ExLang_Mix, SearchOption.TopDirectoryOnly);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
            {
                if (!exs[j].EndsWith(dirSeparatorChar))
                    exs[j] += Path.DirectorySeparatorChar;
                exLangDirs.Add(exs[j]);
            }

            exs = Directory.GetFiles(path, ExTheme_Mix);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
                exTheme.Add(new MixArchive(exs[j]));
            exs = Directory.GetDirectories(path, ExTheme_Mix, SearchOption.TopDirectoryOnly);
            Array.Sort<string>(exs);
            for (int j = exs.Length - 1; j >= 0; j--)
            {
                if (!exs[j].EndsWith(dirSeparatorChar))
                    exs[j] += Path.DirectorySeparatorChar;
                exThemeDirs.Add(exs[j]);
            }
        }

        public string[] SearchFile(string path)
        {
            List<string[]> matches = new List<string[]>(workingDirs.Count);
            int count = 0;
            for (int i = 0; i < workingDirs.Count; i++)
            {
                string[] sm = Directory.GetFiles(workingDirs[i], path, SearchOption.TopDirectoryOnly);
                count += sm.Length;
                matches.Add(sm);
            }

            string[] allMatches = new string[count];

            int idx = 0;
            for (int i = 0; i < matches.Count; i++)
            {
                Array.Copy(matches[i], 0, allMatches, idx, matches[i].Length);
                idx += matches[i].Length;
            }
            return allMatches;
        }

        public SpecialFiles SpecialFile
        {
            get
            {
                if (specialFiles == null)
                    specialFiles = new SpecialFiles();
                return specialFiles;
            }
        }

        public void RegisterArchiveType(IArchiveFactory fac)
        {
            factories.Add(fac.Type, fac);
        }
        public bool UnregisterArchiveType(IArchiveFactory fac)
        {
            return factories.Remove(fac.Type);
        }
        public bool UnregisterArchiveType(string type)
        {
            return factories.Remove(type);
        }
        public DirectoryInfo[] GetWorkingDirectories()
        {
            DirectoryInfo[] res = new DirectoryInfo[workingDirs.Count];
            for (int i = 0; i < workingDirs.Count; i++)
            {
                res[i] = new DirectoryInfo(workingDirs[i]);
            }
            return res;
        }

        public static FileLocateRule AllLR
        {
            get { return FileLocateRule.Standard | FileLocateRule.Root | FileLocateRule.ExTheme | FileLocateRule.Expand | FileLocateRule.ExLang; }
        }
        public static FileLocateRule GameResLR
        {
            get { return FileLocateRule.Expand | FileLocateRule.Root | FileLocateRule.Standard; }
        }
        public static FileLocateRule GameCurrentResLR
        {
            get { return FileLocateRule.Expand | FileLocateRule.Root | FileLocateRule.Standard; }
        }
        public static FileLocateRule GameLangLR
        {
            get { return FileLocateRule.Standard | FileLocateRule.Root | FileLocateRule.ExLang; }
        }
        public static FileLocateRule GeneralLR
        {
            get { return FileLocateRule.Root | FileLocateRule.Standard; }
        }
        public static FileLocateRule GameThemeLR
        {
            get { return FileLocateRule.Root | FileLocateRule.Standard | FileLocateRule.ExTheme; }
        }
        public static string CombinePath(string path1, string path2)
        {
            if ((path1 == null) || (path2 == null))
            {
                throw new ArgumentNullException((path1 == null) ? "path1" : "path2");
            }
            if (path2.Length == 0)
            {
                return path1;
            }
            if (path1.Length == 0)
            {
                return path2;
            }            
            char ch = path1[path1.Length - 1];
            if (((ch != Path.DirectorySeparatorChar) && (ch != Path.AltDirectorySeparatorChar)) && (ch != Path.VolumeSeparatorChar))
            {
                return (path1 + Path.DirectorySeparatorChar + path2);
            }
            return (path1 + path2);
        }


        public static string GetArchivePath(string path)
        {
            path = path.ToUpper();

            string root = Path.GetPathRoot(path).ToUpper();
            while (root != path)
            {
                path = Path.GetDirectoryName(path);
                if (File.Exists(path))
                {
                    return path;
                }
            }
            return path;
        }


        public List<Archive> CurrentArchiveSet
        {
            get;
            private set;
        }


        public string GetPath(string fullPath)
        {
            fullPath = Path.GetDirectoryName(fullPath).ToUpper() + Path.DirectorySeparatorChar;

            for (int i = 0; i < workingDirs.Count; i++)
            {
                int pos = fullPath.IndexOf(workingDirs[i].ToUpper());
                if (pos != -1)
                {
                    return fullPath.Substring(pos + workingDirs[i].Length);
                }
            }
            return fullPath;
        }
        Archive CreateArchive(FileLocation fl)
        {
            string ext = Path.GetExtension(fl.Path);
            IArchiveFactory fac;

            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(fl);
            }
            else
                throw new NotSupportedException();
        }
        Archive CreateArchive(string file)
        {
            string ext = Path.GetExtension(file);
            IArchiveFactory fac;

            if (factories.TryGetValue(ext, out fac))
            {
                return fac.CreateInstance(file);
            }
            else
                throw new NotSupportedException();
        }
        public Archive Locate(string filePath)
        {
            Archive res;
            if (IsOpened(filePath, out res))
            {
                return res;
            }
            FileLocation fl = Locate(filePath, FileLocateRule.Standard | FileLocateRule.Root);
            res = CreateArchive(fl);
            stdPack.Add(res.FilePath, res);
            return res;
        }

        public FileLocation Locate(string filePath, FileLocateRule rule)
        {
            FileLocation res = TryLocate(filePath, rule);
            if (res == null)
                throw new FileNotFoundException(filePath);
            return res;
        }

        public FileLocation Locate(string[] filePath, string defPath, FileLocateRule rule)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < filePath.Length; i++)
            {
                FileLocation res = TryLocate(CombinePath(defPath, filePath[i]), rule);
                if (res != null)
                    return res;
                sb.AppendLine(filePath[i]);
            }
            throw new FileNotFoundException(sb.ToString());
        }
        public FileLocation Locate(string[] filePath, FileLocateRule rule)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < filePath.Length; i++)
            {
                FileLocation res = TryLocate(filePath[i], rule);
                if (res != null)
                    return res;

                sb.AppendLine(filePath[i]);
            }
            throw new FileNotFoundException(sb.ToString());
        }

        public FileLocation TryLocate(string[] filePath, FileLocateRule rule)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < filePath.Length; i++)
            {
                FileLocation res = TryLocate(filePath[i], rule);
                if (res != null)
                    return res;

                sb.AppendLine(filePath[i]);
            }
            return null;
        }

        public FileLocation Locate(string filePath, string[] paths, FileLocateRule rule)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                FileLocation res = TryLocate(CombinePath(paths[i], filePath), rule);
                if (res != null)
                    return res;
            }
            throw new FileNotFoundException(filePath);
        }

        public FileLocation TryLocate(string filePath, string[] paths, FileLocateRule rule)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                FileLocation res = TryLocate(CombinePath(paths[i], filePath), rule);
                if (res != null)
                    return res;
            }
            return null;
        }

        public FileLocation TryLocate(string filePath, FileLocateRule rule)
        {
            string fileName = Path.GetFileName(filePath);

            // 不在资源包中
            if ((rule & FileLocateRule.Root) != 0)
                for (int i = 0; i < workingDirs.Count; i++)
                {
                    // 根目录寻找
                    if (File.Exists(workingDirs[i] + fileName))
                        return new FileLocation(workingDirs[i] + fileName);

                    // 子目录寻找
                    if (File.Exists(workingDirs[i] + filePath))
                        return new FileLocation(workingDirs[i] + filePath);
                }

            // 在expand和ecache中
            if ((rule & FileLocateRule.Expand) != 0)
            {
                for (int i = 0; i < expPack.Count; i++)
                {
                    ArchiveFileEntry ent;
                    if (expPack[i].Find(fileName, out ent))
                        return new FileLocation(expPack[i], expPack[i].FilePath + Path.DirectorySeparatorChar + fileName, ent);
                }
                for (int i = 0; i < expDirs.Count; i++)
                {
                    string path = expDirs[i] + fileName;
                    if (File.Exists(path))
                        return new FileLocation(path);
                }
            }

            if ((rule & FileLocateRule.ExLang) != 0)
            {
                for (int i = 0; i < exLang.Count; i++)
                {
                    ArchiveFileEntry ent;
                    if (exLang[i].Find(fileName, out ent))
                        return new FileLocation(exLang[i], exLang[i].FilePath + Path.DirectorySeparatorChar + fileName, ent);
                }
                for (int i = 0; i < exLangDirs.Count; i++)
                {
                    string path = exLangDirs[i] + fileName;
                    if (File.Exists(path))
                        return new FileLocation(path);
                }
            }

            if ((rule & FileLocateRule.ExTheme) != 0)
            {
                for (int i = 0; i < exTheme.Count; i++)
                {
                    ArchiveFileEntry ent;
                    if (exTheme[i].Find(fileName, out ent))
                        return new FileLocation(exTheme[i], exTheme[i].FilePath + Path.DirectorySeparatorChar + fileName, ent);
                }
                for (int i = 0; i < exThemeDirs.Count; i++)
                {
                    string path = exThemeDirs[i] + fileName;
                    if (File.Exists(path))
                        return new FileLocation(path);
                }
            }

            if ((rule & FileLocateRule.CurrentArchiveSet) != 0)            
            {
                for (int i = 0; i < CurrentArchiveSet.Count; i++)
                {
                    ArchiveFileEntry ent;
                    if (CurrentArchiveSet[i].Find(fileName, out ent))
                        return new FileLocation(CurrentArchiveSet[i], CurrentArchiveSet[i].FilePath + Path.DirectorySeparatorChar + fileName, ent);
                }
            }

            if ((rule & FileLocateRule.Standard) != 0)
            {
                for (int k = 0; k < workingDirs.Count; k++)
                {
                    try
                    {
                        string[] locs = filePath.Split(DirSepCharArray, StringSplitOptions.RemoveEmptyEntries);
                        StringBuilder sb = new StringBuilder();

                        Archive entry = null;
                        Archive last;

                        bool found = true;


                        for (int i = 0; i < locs.Length - 1; i++)
                        {
                            if (i > 0)
                                sb.Append(Path.DirectorySeparatorChar + locs[i]);
                            else
                                sb.Append(locs[i]);

                            last = entry;

                            // 如果当前资源路径下的这个资源包未打开过
                            if (!IsOpened(workingDirs[k] + sb.ToString(), out entry))
                            {
                                // 如果在资源包中
                                if (last != null)
                                {
                                    ArchiveFileEntry ent;
                                    if (last.Find(locs[i], out ent))
                                    {
                                        entry = CreateArchive(new FileLocation(last, workingDirs[k] + sb.ToString(), ent));
                                        stdPack.Add(entry.FilePath, entry);
                                    }
                                    else
                                    {
                                        found = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    string arc = workingDirs[k] + sb.ToString();
                                    if (File.Exists(arc))
                                    {
                                        entry = CreateArchive(arc);
                                        stdPack.Add(entry.FilePath, entry);
                                    }
                                    else
                                    {
                                        found = false;
                                        break;
                                    }
                                }
                            }
                        }

                        if (found && entry != null)
                        {
                            ArchiveFileEntry res;
                            if (entry.Find(locs[locs.Length - 1], out res))
                                return new FileLocation(entry, workingDirs[k] + filePath, res);
                        }
                    }
                    catch (DataFormatException)
                    {
                        GameConsole.Instance.Write(ResourceAssembly.Instance.Ex_InvaildFileFormat(filePath), ConsoleMessageType.Warning);
                    }
                    catch (Exception e)
                    {
                        GameConsole.Instance.Write("FileSystem:" + e.Message, ConsoleMessageType.Warning);
                    }

                } // for working dir
            }
            return null;
        }

        ///// <summary>
        ///// 直接从ini值定位
        ///// </summary>
        ///// <param name="filePath"></param>
        ///// <param name="rule"></param>
        ///// <returns></returns>
        //public FileLocation Locate(string filePath, string defPath, string defSuffix, FileLocateRule rule)
        //{
        //    string[] v = filePath.Split(IniSection.PathSeprater);

        //    for (int i = 0; i < v.Length; i++)
        //    {
        //        v[i] = v[i].Trim();
        //        try
        //        {
        //            if (v[i].Contains(new string(new char[] { Path.DirectorySeparatorChar })))
        //                return Locate(v[i].Trim(), rule);
        //            else
        //                return Locate(defPath + v[i].Trim() + defSuffix, rule);
        //        }
        //        catch { }
        //    }
        //    throw new FileNotFoundException(filePath);
        //}



        bool IsOpened(string filePath, out Archive entry)
        {
            return stdPack.TryGetValue(filePath, out entry);
            //for (int i = 0; i < stdPack.Count; i++)
            //{
            //    if (CaseInsensitiveStringComparer.Compare(stdPack[i].FilePath, filePath))
            //    {
            //        entry = stdPack[i];
            //        return true;
            //    }
            //}
            //entry = null;
            //return false;
        }


        #region IDisposable 成员

        public void Dispose()
        {
            if (!disposed)
            {
                //for (int i = 0; i < stdPack.Count; i++)
                //    stdPack[i].Dispose();
                Dictionary<string, Archive>.ValueCollection vals = stdPack.Values;
                foreach (Archive arc in vals)
                {
                    arc.Dispose();
                }

                for (int i = 0; i < expPack.Count; i++)
                    expPack[i].Dispose();
                disposed = true;
                //GC.SuppressFinalize(this);
            }
            else
                throw new ObjectDisposedException(this.ToString());
        }

        #endregion

        ~FileSystem()
        {
            if (!disposed)
                Dispose();
        }
    }
}
