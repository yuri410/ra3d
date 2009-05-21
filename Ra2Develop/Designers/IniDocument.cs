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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using R3D.IO;
using R3D.UI;
using WeifenLuo.WinFormsUI.Docking;

namespace Ra2Develop.Designers
{
    public partial class IniDocument : GeneralDocumentBase
    {
        public const string Extension = ".ini";

        IniCodeEditorControl codeEdit;
        public IniDocument(DocumentAbstractFactory fac, ResourceLocation rl)
        {
            InitializeComponent();

            LanguageParser.ParseLanguage(Program.StringTable, this);

            codeEdit = new IniCodeEditorControl();

            codeEdit.Dock = DockStyle.Fill;

            Controls.Add(codeEdit);

            Init(fac, rl);
            Saved = true;
        }


        public override bool LoadRes()
        {
            if (ResourceLocation != null)
            {
                ArchiveStreamReader sr = new ArchiveStreamReader(ResourceLocation);

                codeEdit.Code.Source = sr.ReadToEnd();

                sr.Close();

                Saved = true;
                return true;
            }
            Saved = true;
            return true;
        }

        public override bool SaveRes()
        {
            Saved = true;
            return true;
        }

        public override ToolStrip[] ToolStrips
        {
            get { return new ToolStrip[0]; }
        }
    }
}
