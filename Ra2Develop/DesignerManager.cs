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
using Ra2Develop.Designers;
using R3D.IO;
using R3D;

namespace Ra2Develop
{
    public class DesignerManager
    {
        static DesignerManager singleton;

        public static DesignerManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new DesignerManager();
                }
                return singleton;
            }
        }

        Dictionary<string, DocumentAbstractFactory> factories;

        private DesignerManager()
        {
            factories = new Dictionary<string, DocumentAbstractFactory>(CaseInsensitiveStringComparer.Instance);
        }

        public bool RegisterDesigner(DocumentAbstractFactory fac)
        {
            bool passed = true;
            string[] exts = fac.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                if (!factories.ContainsKey(exts[i]))
                {
                    factories.Add(exts[i], fac);
                }
                else
                {
                    passed = false;
                }
            }
            return passed;
        }

        public void UnregisterDesigner(DocumentAbstractFactory dfac)
        {
            string[] exts = dfac.Filters;
            for (int i = 0; i < exts.Length; i++)
            {
                DocumentAbstractFactory fac;
                if (factories.TryGetValue(exts[i], out fac))
                {
                    if (fac == dfac)
                    {
                        factories.Remove(exts[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rl"></param>
        /// <param name="extension">用于识别内容类型的扩展名</param>
        public DocumentBase CreateDocument(ResourceLocation rl, string extension)
        {
            DocumentAbstractFactory fac;
            if (factories.TryGetValue(extension, out fac))
            {
                return fac.CreateInstance(rl);
            }
            else
                throw new NotSupportedException();
        }

        /// <param name="extension">用于识别类型的扩展名</param>
        public DocumentAbstractFactory FindFactory(string ext)
        {
            DocumentAbstractFactory res;
            factories.TryGetValue(ext, out res);
            return res;
        }

        public DocumentAbstractFactory FindFactory(Type type)
        {
            Dictionary<string, DocumentAbstractFactory>.ValueCollection vals = factories.Values;
            foreach (DocumentAbstractFactory fac in vals)
            {
                if (fac.GetType() == type)
                    return fac;
            }
            return null;
        }

        /// <summary>
        /// 获得所有格式的过滤器
        /// </summary>
        /// <returns></returns>
        public Pair<string, string>[] GetAllFormats()
        {
            Dictionary<string, DocumentAbstractFactory>.ValueCollection val = factories.Values;

            List<Pair<string, string>> fmts = new List<Pair<string, string>>();

            DocumentAbstractFactory lastFac = null;
            foreach (DocumentAbstractFactory fac in val)
            {
                if (fac != lastFac)
                {
                    string[] flts = fac.Filters;

                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < flts.Length; j++)
                    {
                        sb.Append('*');
                        sb.Append(flts[j]);
                        if (j < flts.Length - 1)
                            sb.Append(';');
                    }

                    fmts.Add(new Pair<string, string>(fac.Description, sb.ToString()));
                    lastFac = fac;
                }
            }

            return fmts.ToArray();
        }

        public string GetFilter()
        {
            Dictionary<string, DocumentAbstractFactory>.ValueCollection val = factories.Values;

            //List<Pair<string, string>> fmts = new List<Pair<string, string>>();
            StringBuilder flt = new StringBuilder(val.Count * 4 + 4);

            DocumentAbstractFactory lastFac = null;
            foreach (DocumentAbstractFactory fac in val)
            {
                if (fac != lastFac)
                {
                    //tring[] flts = fac.Filters;
                    flt.Append(DevUtils.GetFilter(fac.Description, fac.Filters));
                    //StringBuilder sb = new StringBuilder();
                    //for (int j = 0; j < flts.Length; j++)
                    //{
                    //    sb.Append('*');
                    //    sb.Append(flts[j]);
                    //    if (j < flts.Length - 1)
                    //        sb.Append(';');
                    //}
                    //flt.Append(fac.Description);
                    //flt.Append("(" + sb.ToString() + ")|");
                    //flt.Append(sb.ToString());
                    flt.Append('|');
                    //fmts.Add(new Pair<string, string>(fac.ValType, sb.ToString()));

                    lastFac = fac;
                }
            }
            flt.Remove(flt.Length - 1, 1);
            return flt.ToString();

        }
    }
}
