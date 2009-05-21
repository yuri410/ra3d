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
using System.IO;
using R3D.IO;
using R3D.IsoMap;
using R3D.GraphicsEngine;
using R3D.Media;
using Ra2Develop.Editors.EditableObjects;

namespace Ra2Develop.Templates
{
    public class CsfTemplate : FileTemplateBase
    {

        public override DocumentBase CreateInstance(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return DesignerManager.Instance.CreateDocument(null, CsfDocument.Extension);
            }
            else
            {
                DevFileLocation fl = new DevFileLocation(fileName);

                BinaryWriter bw = new BinaryWriter(fl.GetStream, Encoding.Default);
                bw.Write((int)FileID.Csf);
                bw.Write((int)3);

                bw.Write((int)0);
                bw.Write((int)0);
                bw.Write((int)0);
                bw.Close();

                return DesignerManager.Instance.CreateDocument(fl, CsfDocument.Extension);
            }
        }
        public override string DefaultFileName
        {
            get { return "CsfFile.csf"; }
        }
        public override string Description
        {
            get { return Program.StringTable["MSG:CSFDESC"]; }
        }

        public override int Platform
        {
            get { return PresetedPlatform.RedAlert2 | PresetedPlatform.YurisRevenge | PresetedPlatform.Ra2Reload; }
        }

        public override string Name
        {
            get { return Program.StringTable["DOCS:CSFDESC"]; }
        }


        public override string Filter
        {
            get
            {
                return DesignerManager.Instance.FindFactory(CsfDocument.Extension).Filter;
            }
        }
    }

    public class Tile3DTemplate : FileTemplateBase
    {

        public override DocumentBase CreateInstance(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return DesignerManager.Instance.CreateDocument(null, Tile3DDocument.Extension);
            }
            else
            {
                DevFileLocation fl = new DevFileLocation(fileName);

                EditableTile3D data = new EditableTile3D();


                EditableBlock[] blocks = new EditableBlock[1];
                blocks[0].mat1 = new EditableBlockMaterial();
                blocks[0].mat1.Texture = null;
                blocks[0].bits = BlockBits.None;

                data.Blocks = blocks;

                EditableTile3D.ToFile(data, fileName);

                return DesignerManager.Instance.CreateDocument(fl, CsfDocument.Extension);
            }
        }

        public override string Filter
        {
            get { return DesignerManager.Instance.FindFactory(Tile3DDocument.Extension).Filter; }
        }

        public override string Description
        {
            get { return Program.StringTable["MSG:TILE3DDesc"]; }
        }

        public override int Platform
        {
            get { return PresetedPlatform.Ra2Reload; }
        }

        public override string Name
        {
            get { return Program.StringTable["DOCS:TILE3DDesc"]; }
        }
    }
}
