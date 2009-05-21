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
using R3D.IsoMap;
using R3D.IO;
using R3D;
using R3D.Media;
using System.Drawing;
using System.IO;

namespace Ra2Develop.Editors.EditableObjects
{
    public class EditableTile3D : Tile3DBase<EditableBlock, EditableBlockMaterial, EditableModel>, IDisposable
    {
        protected override EditableBlock CreateNewBlock()
        {
            return new EditableBlock(null);
        }

        protected override EditableBlockMaterial LoadMaterial(BinaryDataReader data)
        {
            return EditableBlockMaterial.FromBinary(data);
        }
        protected override EditableModel LoadModel(BinaryDataReader data)
        {
            return EditableModel.FromBinary(data);
        }

        public EditableTile3D()
            : base(null)
        { }

        private EditableTile3D(ResourceLocation rl)
            : base(null, rl.GetStream, rl.Name)
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Initialize();//.X = blocks[i].x;
                //blocks[i].Height = blocks[i].Height;
                //blocks[i].SecHeight = blocks[i].SecHeight;
            }
        }

        public EditableBlock[] Blocks
        {
            get { return blocks; }
            set
            {
                blocks = value;
                blockCount = blocks.Length;
            }
        }
        [Obsolete()]
        public void AddBlock(Block blk)
        {
            //blocks.Add(blk);
        }
        [Obsolete()]
        public void RemoveBlock(Block blk)
        {
            //blocks.RemoveAt(blk);
        }


        public override ImageBase[] GetImages(params object[] paras)
        {
            throw new NotSupportedException();
        }

        public override ImageBase GetImage(int subTile, params object[] paras)
        {
            throw new NotSupportedException();
        }

        public static EditableTile3D FromFile(string file)
        {
            return new EditableTile3D(new DevFileLocation(file));
        }
        public static EditableTile3D FromFile(ResourceLocation rl)
        {
            return new EditableTile3D(rl);
        }
        public static void ToFile(EditableTile3D tile, string file)
        {
            FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            fs.SetLength(0);

            tile.WrtieData(fs);
        }
        public static void ToResLoc(EditableTile3D tile, ResourceLocation rl)
        {
            tile.WrtieData(rl.GetStream);
        }

        protected override BinaryDataWriter SaveMaterial(EditableBlockMaterial mat)
        {
            return EditableBlockMaterial.ToBinary(mat);
        }

        protected override BinaryDataWriter SaveModel(EditableModel mdl)
        {
            return EditableModel.ToBinary(mdl);
        }

        #region IDisposable 成员

        public override void Dispose()
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].Dispose();
            }
        }

        #endregion
    }
}
