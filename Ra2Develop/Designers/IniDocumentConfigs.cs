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
using System.Drawing;
using System.Text.RegularExpressions;
using System.Xml;

namespace Ra2Develop.Designers
{
    [Flags()]
    public enum IniKeyInfoCategory
    {
        None = 0,
        Description = 1,
        Value = 1 << 1,
        Remark = 1 << 2,
    }
    public enum IniValueType : int
    {
        Byte,
        Int16,
        Int32,
        Int64,
        UInt16,
        UInt32,
        UInt64,
        Single,
        Double,
        String,
        Enum,
        IniListItem,
        ByteArray,
        Int16Array,
        Int32Array,
        Int64Array,
        UInt16Array,
        UInt32Array,
        UInt64Array,
        SingleArray,
        DoubleArray,
        StringArray,
        EnumArray,
        IniListItemArray
    }

    public struct IniTagInfo
    {
        IniValueType valType;
        Type enumType;
        string[] iniLists;
        string platform;
        string description;
        string value;
        string remark;
        string keyword;


        public IniValueType ValType
        {
            get { return valType; }
            set { valType = value; }
        }

        public Type EnumType
        {
            get { return enumType; }
            set { enumType = value; }
        }

        public string[] IniLists
        {
            get { return iniLists; }
            set { iniLists = value; }
        }

        public string Platform
        {
            get { return platform; }
            set { platform = value; }
        }
        public string Description
        {
            get {return  description; }
            set { description = value; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public string Remark
        {
            get { return remark; }
            set { remark = value; }
        }
        public string Keyword
        {
            get { return keyword; }
            set { keyword = value; }
        }

        public IniTagInfo(string key, string pl, string desc, string value)
        {
            keyword = key;
            description = desc;
            this.value = value;
            remark = string.Empty;
            platform = pl;

            valType = IniValueType.Int32;
            enumType = null;
            iniLists = null;

        }
        public IniTagInfo(string key, string pl, string desc, string remark, string value)
        {
            keyword = key;
            description = desc;
            this.value = value;
            this.remark = remark;
            platform = pl;

            valType = IniValueType.Int32;
            enumType = null;
            iniLists = null;

        }

        public IniTagInfo(XmlTextReader xml)
        {
            keyword = xml.Name;
            platform = xml.GetAttribute("Platform");
            description = xml.GetAttribute("Description");
            remark = xml.GetAttribute("Remark");
            value = xml.GetAttribute("Value");

            valType = (IniValueType)Enum.Parse(typeof(IniValueType), xml.GetAttribute("ValueType"));
            
            
            iniLists = null;
            enumType = null;

            if (valType == IniValueType.Enum || valType == IniValueType.EnumArray)
            {
                enumType = Type.GetType(xml.GetAttribute("EnumType"));
            }
            else if (valType == IniValueType.IniListItem || valType == IniValueType.IniListItemArray)
            {
                iniLists = xml.GetAttribute("IniLists").Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public void WriteTo(XmlTextWriter xml)
        {
            
        }
    }
    public class IniDataBase : Dictionary<string, IniTagInfo>
    {
        string keyword;
        Regex regex;

        public string Keyword
        {
            get { return keyword; }
        }

        public IniDataBase(string name)
        {
            keyword = name;
        }

        public string ToString(IniKeyInfoCategory config)
        {
            string desc = Program.StringTable["Ini:Desc"];
            string platform = Program.StringTable["Ini:platform"];
            string val = Program.StringTable["Ini:value"];
            string remark = Program.StringTable["Ini:remark"];


            StringBuilder sb = new StringBuilder();

            sb.AppendLine(keyword);
            foreach (KeyValuePair<string, IniTagInfo> i in this)
            {
                sb.Append('[');
                sb.Append(platform);
                sb.Append(i.Key);
                sb.AppendLine("]");

                if ((int)(config & IniKeyInfoCategory.Description) != 0)
                {
                    sb.AppendLine(desc);
                    sb.AppendLine(i.Value.Description);
                }
                if ((int)(config & IniKeyInfoCategory.Remark) != 0)
                {
                    sb.AppendLine(remark);
                    sb.AppendLine(i.Value.Remark);
                }
                if ((int)(config & IniKeyInfoCategory.Value) != 0)
                {
                    sb.AppendLine(val);
                    sb.AppendLine(i.Value.Value);
                }

                sb.AppendLine();
            }

            return sb.ToString();

        }

        public override string ToString()
        {
            string platform = Program.StringTable["Ini:platform"];

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(keyword);
            foreach (KeyValuePair<string, IniTagInfo> i in this)
            {
                sb.Append('[');
                sb.Append(platform);

                sb.Append(i.Key);
                sb.AppendLine("]");

                sb.Append(i.Value.Description);

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public Regex Filter
        {
            get
            {
                return regex;// new Regex(regex, RegexOptions.Singleline);
            }
        }

        public void Read(BinaryReader br)
        {
            regex = new Regex(br.ReadString(), RegexOptions.Singleline);

            int psc = br.ReadInt32();
            for (int j = 0; j < psc; j++)
            {
                IniTagInfo psentry =
                    new IniTagInfo(
                        br.ReadString(),
                        br.ReadString(),
                        br.ReadString(),
                        br.ReadString(),
                        br.ReadString()
                        );

                //entry.Add(psentry.Platform, psentry);
                Add(psentry.Keyword, psentry);
            }
          
        }
        public void Read(XmlTextReader xml)
        {
            string format = xml.GetAttribute("FormatVersion").ToLower();

            switch (format)
            {
                case "1.0":
                    while (DevUtils.XmlMoveToNextElement(xml))
                    {
                        switch (xml.Name)
                        {
                            case "Filter":
                                regex = new Regex(xml.ReadString(), RegexOptions.Singleline);
                                break;
                            case "Entries":
                                int psc = int.Parse(xml.GetAttribute("Count"));
                                DevUtils.XmlMoveToNextElement(xml);

                                for (int i = 0; i < psc; i++)
                                {
                                    IniTagInfo psentry = new IniTagInfo(xml);
                                    //entry.Add(psentry.Keyword, psentry);
                                    Add(psentry.Keyword, psentry);

                                    DevUtils.XmlMoveToNextElement(xml);
                                }

                                break;
                        }
                    }

                    break;
            }

        }
        public void Write(BinaryWriter bw)
        {

        }

        public void Write(XmlTextWriter xml)
        {
            
        }
    }

    public class IniDocumentConfigs : DocumentConfigBase
    {
        readonly static string ConfigFile = Path.Combine("Configs", "inic.xml");

        static readonly string XmlConfigName = "IniConfig";
        //static readonly string XmlDBName = "IniDatabase";
        /*
         * 格式
         *   版本
         *   记录数:int32
         *   IniDataBase
         *     keyword
         *     Platform数:int32
         *     IniTagInfo
         *   
         * 
         */


        static IniDocumentConfigs singleton;

        List<IniDataBase> databases;
        List<string> dbPaths;
        private IniDocumentConfigs()
        {
            if (File.Exists(ConfigFile))
            {
                FileStream fs = new FileStream(ConfigFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                

                //BinaryReader br = new BinaryReader(fs, Encoding.Unicode);

                //br.ReadBytes(16);
                //int count = br.ReadInt32();

                //databases = new List<IniDataBase>(count);
                //for (int i = 0; i < count; i++)
                //{
                //    IniDataBase entry = new IniDataBase(br.ReadString());

                //    entry.Read(br);
                //    databases.Add(entry);
                //}

                //br.Close();
                 dbPaths = new List<string>();

                string basePath = Path.GetDirectoryName(ConfigFile);

                XmlTextReader xml = new XmlTextReader(fs);
                //xml.Encoding = Encoding.Unicode;
                xml.WhitespaceHandling = WhitespaceHandling.None;
                xml.MoveToContent();

                while (xml.Name != XmlConfigName)
                {
                    if (!DevUtils.XmlMoveToNextElement(xml))
                    {
                        throw new InvalidDataException(ConfigFile);
                    }
                }

                string version = xml.GetAttribute("FormatVersion").ToLower();

                switch (version)
                {
                    case "1.0":
                        while (DevUtils.XmlMoveToNextElement(xml))
                        {
                            switch (xml.Name)
                            {
                                case "Databases":
                                    int dbCount = int.Parse(xml.GetAttribute("Count"));
                                    dbPaths.Capacity = dbCount + 1;

                                    DevUtils.XmlMoveToNextElement(xml);
                                    for (int i = 0; i < dbCount; i++)
                                    {
                                        string path = xml.GetAttribute("Path");

                                        dbPaths.Add(path);

                                        DevUtils.XmlMoveToNextElement(xml);
                                    }
                                    break;
                            }

                        }
                        break;
                }

                xml.Close();

                // load databases
                for (int i = 0; i < dbPaths.Count; i++)
                {
                    fs = new FileStream(dbPaths[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                    xml = new XmlTextReader(fs);
                    //xml.Encoding = Encoding.Unicode;
                    xml.WhitespaceHandling = WhitespaceHandling.None;
                    xml.MoveToContent();

                    while (string.IsNullOrEmpty(xml.Name))
                    {
                        if (!DevUtils.XmlMoveToNextElement(xml))
                            throw new InvalidDataException(ConfigFile);
                    }

                    IniDataBase db = new IniDataBase(xml.Name);

                    db.Read(xml);

                    xml.Close();
                }
            }
            else
            {
                databases = new List<IniDataBase>();
            }

        }

        public static IniDocumentConfigs Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new IniDocumentConfigs();
                return singleton;
            }
        }



        protected override void Save()
        {
            FileStream fs = new FileStream(ConfigFile, FileMode.Open, FileAccess.Write, FileShare.Write);
            //BinaryWriter bw = new BinaryWriter(fs, Encoding.Unicode);
            XmlTextWriter xml = new XmlTextWriter(fs, Encoding.Unicode);

            xml.Formatting = Formatting.Indented;

            xml.WriteStartDocument();

            xml.WriteStartElement(XmlConfigName);

            xml.WriteAttributeString("FormatVersion", "1.0");
            xml.WriteEndElement();

            xml.WriteStartElement("Databases");
            xml.WriteAttributeString("Count", databases.Count.ToString());

            for (int i = 0; i < databases.Count; i++)
            {
                xml.WriteStartElement("DB" + i.ToString());
                xml.WriteAttributeString("Path", dbPaths[i]);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();

            xml.Close();
        }
    }
}
