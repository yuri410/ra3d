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
using Ra2Develop.Templates;
using Ra2Develop.Designers;

namespace Ra2Develop
{
    public class TemplateManager
    {
        static TemplateManager singleton;

        public static TemplateManager Instance
        {
            get
            {
                if (singleton == null)
                {
                    singleton = new TemplateManager();
                }
                return singleton; 
            }
        }

        List<FileTemplateBase> fileTmps;

        private TemplateManager()
        {
            fileTmps = new List<FileTemplateBase>(256);
        }

        public void RegisterTemplate(FileTemplateBase tem)
        {
            if (fileTmps.Contains(tem))
            {
                throw new InvalidOperationException();
            }
            fileTmps.Add(tem);
        }

        public void UnregisterTemplate(FileTemplateBase tem)
        {
            fileTmps.Remove(tem);
        }

        public FileTemplateBase[] GetFileTemplates()
        {
            return fileTmps.ToArray();
        }

        public FileTemplateBase[] GetFileTemplates(int platform)
        {
            List<FileTemplateBase> res = new List<FileTemplateBase>(fileTmps.Count);

            for (int i = 0; i < res.Count; i++)
            {
                if ((res[i].Platform & platform) != 0)
                {
                    res.Add(res[i]);
                }
            }
            return res.ToArray();
        }
    }
}
