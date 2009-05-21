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
using System.IO;
using System.Text;
using R3D.Base;
using R3D.IO;

//using TempSection = System.Collections.Generic.KeyValuePair<
//                string, System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>>;

namespace R3D.ConfigModel
{
    public class IniConfiguration : Configuration
    {
        static readonly string SegmentL = "[";
        static readonly string SegmentR = "]";
        const char CommetChar = ';';
        //const string ScriptL = "{";
        //const string ScriptR = "}";
        const char Equal = '=';
        //const string ScriptType = "ScriptName";
        //const string Space = " ";

        static readonly string InheritKeyword = "InheritsFrom";

        static readonly char[] EqualArray = new char[] { Equal };

        public void Save(ResourceLocation dest)
        {
            ArchiveStreamWriter sw = new ArchiveStreamWriter(dest);

            string keyValue = " = ";
            string sl = SegmentL;
            string sr = SegmentR;

            foreach (KeyValuePair<string, ConfigurationSection> sect in this)
            {
                sw.WriteLine(sl + sect.Key + sr);
                foreach (KeyValuePair<string, string> kw in sect.Value)
                {
                    sw.WriteLine(kw.Key + keyValue + kw.Value);
                }
                sw.WriteLine();
            }

            sw.Close();
        }
        /// <summary>
        /// 仅支持ra2.ini
        /// </summary>
        /// <param name="fileName"></param>
        public void Save(string fileName)
        {
            FileStream fstm = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            fstm.SetLength(0);
            StreamWriter sw = new StreamWriter(fstm, Encoding.Default);

            string keyValue = " = ";
            string sl = SegmentL;
            string sr = SegmentR;

            foreach (KeyValuePair<string, ConfigurationSection> sect in this)
            {
                sw.WriteLine(sl + sect.Key + sr);
                foreach (KeyValuePair<string, string> kw in sect.Value)
                {
                    sw.WriteLine(kw.Key + keyValue + kw.Value);
                }
                sw.WriteLine();
            }

            sw.Close();
        }


        public IniConfiguration(string file)
            : this(new FileLocation(file))
        { }

        public IniConfiguration(ResourceLocation file) :
            base(file.Name, CaseInsensitiveStringComparer.Instance)
        {
            ArchiveStreamReader sr = new ArchiveStreamReader(file.GetStream, Encoding.Default);
            ConfigurationSection curSect = null;
            string curSectName = string.Empty;

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();

                // 去除注释
                int cpos = line.IndexOf(CommetChar);
                if (cpos != -1)
                    line = line.Substring(0, cpos);

                line = line.Trim();

                if (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith(SegmentL) & line.EndsWith(SegmentR))
                    {
                        curSectName = line.Substring(1, line.Length - 2).Trim();
                        curSect = new IniSection(curSectName);
                        
                        try
                        {
                            Add(curSectName, curSect);
                        }
                        catch (ArgumentException)
                        {
                            GameConsole.Instance.Write(ResourceAssembly.Instance.CM_IniSegmentRep(Name, curSectName), ConsoleMessageType.Error);
                            curSect = this[curSectName];
                        }
                    }
                    else if (curSect != null)
                    {
                        string[] arg = line.Split(EqualArray);

                        if (arg.Length > 1)
                        {
                            string keyword = arg[0].TrimEnd();
                            string value = arg[1].TrimStart();
                            try
                            {
                                curSect.Add(keyword, value);
                            }
                            catch (ArgumentException)
                            {
                                GameConsole.Instance.Write(ResourceAssembly.Instance.CM_IniKeywordRep(Name, curSectName, keyword), ConsoleMessageType.Error);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    curSect.Remove(keyword);
                                    curSect.Add(keyword, value);
                                }
                            }
                        }
                    }
                }
            }

            sr.Close();
            ProcessSectionHierarchy();

        }

        public IniConfiguration(string name, int cap)
            : base(name, cap, CaseInsensitiveStringComparer.Instance)
        { }


        /// <summary>
        ///   Unfold the inherited section
        /// </summary>
        void ProcessSectionHierarchy()
        {
            foreach (KeyValuePair<string, ConfigurationSection> e1 in this)
            {
                ConfigurationSection sect = e1.Value;

                ConfigurationSection curSect = sect;

                string parent;
                while (curSect.TryGetValue(InheritKeyword, out parent))
                {
                    ConfigurationSection parentSect;
                    if (this.TryGetValue(parent, out parentSect))
                    {
                        foreach (KeyValuePair<string, string> e2 in parentSect)
                        {
                            if (!sect.ContainsKey(e2.Key))
                            {
                                sect.Add(e2.Key, e2.Value);
                            }
                        }

                        curSect = parentSect;
                    }
                    else
                    {
                        GameConsole.Instance.Write(ResourceAssembly.Instance.CM_IniParentMissing(sect.Name, parent), ConsoleMessageType.Warning);
                        break;
                    }
                }
            }
        }
        
        public override void Merge(Configuration config)
        {
            Configuration copy = config.Clone();

            foreach (KeyValuePair<string, ConfigurationSection> e1 in copy)
            {
                ConfigurationSection sect;
                if (!TryGetValue(e1.Key, out sect))
                {
                    Add(e1.Key, e1.Value);
                }
                else
                {                    
                    foreach (KeyValuePair<string, string> e2 in e1.Value)
                    {
                        if (sect.ContainsKey(e2.Key))
                        {
                            sect.Remove(e2.Key);
                        }

                        sect.Add(e2.Key, e2.Value);

                    }
                }
            }
        }
      
        public override Configuration Clone()
        {
            IniConfiguration ini = new IniConfiguration(base.Name, this.Count);

            foreach (KeyValuePair<string, ConfigurationSection> e1 in this)
            {
                //Dictionary<string, string> newSectData = new Dictionary<string, string>(e1.Value.Count);
                IniSection newSect = new IniSection(e1.Key, e1.Value.Count);

                foreach (KeyValuePair<string, string> e2 in e1.Value)
                {
                    newSect.Add(e2.Key, e2.Value);
                }

                ini.Add(e1.Key, newSect);
            }

            return ini;
        }
    }
    public class ConfigurationIniFormat:ConfigurationFormat 
    {
        public override string[] Filters
        {
            get { return new string[] { ".ini" }; }
        }

        public override Configuration Load(ResourceLocation rl)
        {
            return new IniConfiguration(rl);
        }
    }
}
