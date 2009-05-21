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
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ra2Develop.Designers;
using Ra2Develop.Editors.EditableObjects;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.IsoMap;
using R3D.Media;
using SlimDX.Direct3D9;

namespace Ra2Develop.Converters
{
    public class Tmp2Tile3DConverter : ConverterBase
    {
        public class MaterialParams
        {
            public BlockMaterialEffect Effect
            {
                get;
                set;
            }

            public void SetupMaterial(EditableBlockMaterial mate)
            {
                mate.Effect = Effect;
            }
            //BlockModelType
        }
        const string CsfKey = "GUI:Tmp2Tile3D";

        public MaterialParams MaterialParameters
        {
            get;
            set;
        }
        public Palette Palette
        {
            get;
            set;
        }

        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show(Program.StringTable[CsfKey], GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                TmpConvDlg dlg = new TmpConvDlg();

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Palette = Palette.FromFile(dlg.PaletteFile);

                    MaterialParameters = dlg.MaterialParameters;

                    ProgressDlg pd = new ProgressDlg(Program.StringTable["GUI:Converting"]);

                    pd.MinVal = 0;
                    pd.Value = 0;
                    pd.MaxVal = files.Length;

                    pd.Show();

                    for (int i = 0; i < files.Length; i++)
                    {
                        string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + dlg.FileExtension);

                        Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));
                        pd.Value = i;

                    }
                    pd.Close();
                    pd.Dispose();
                }
                dlg.Dispose();
            }
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            if (Palette == null)
                throw new InvalidOperationException();
            if (MaterialParameters == null)
                throw new InvalidOperationException();

            Tile tile = Tile.FromFile(source);

            EditableTile3D tile3d = new EditableTile3D();

            List<EditableBlock> blocks = new List<EditableBlock>(tile.BlockCount);// new EditableBlock[tile.BlockCount];

            for (int i = 0; i < tile.BlockCount; i++)
            {
                if (tile.IsVaild(i))
                {
                    EditableBlock blk = new EditableBlock(null);
                    //blocks[i] = new EditableBlock(null);
                    blk.bits = BlockBits.None;
                    blk.height = (short)tile.GetHeight(i);

                    //Color left;
                    //Color right;

                    //tile.GetRadarColor(i, out left, out right);
                    //blk.indexHelper = i;

                    blk.radar_red_left = tile.Blocks[i].radar_red_left;// left.R;
                    blk.radar_green_left = tile.Blocks[i].radar_green_left;
                    blk.radar_blue_left = tile.Blocks[i].radar_blue_left;

                    blk.radar_red_right = tile.Blocks[i].radar_red_right;
                    blk.radar_green_right = tile.Blocks[i].radar_green_right;
                    blk.radar_blue_right = tile.Blocks[i].radar_blue_right;

                    blk.ramp_type = (byte)tile.Blocks[i].ramp_type; //.GetRampType(i);
                    blk.terrain_type = tile.Blocks[i].terrain_type;// tile.GetTerrainType(i);

                    int x = tile.Blocks[i].x;
                    int y = tile.Blocks[i].y;

                    blk.x = (int)(y / 30.0f + x / 60.0f);
                    blk.y = (int)(y / 30.0f - x / 60.0f);

                    blk.mat1 = new EditableBlockMaterial();
                    MaterialParameters.SetupMaterial(blk.mat1);
                    //blk.mat1.mat = material.mat;

                    ImageBase img = tile.GetImageNoFS(i, Palette);
                    blk.mat1.Texture = img.GetTexture(GraphicsDevice.Instance.Device, Usage.None, Pool.Managed);
                    img.Dispose();

                    blocks.Add(blk);
                }
            }

            tile3d.Blocks = blocks.ToArray();

            EditableTile3D.ToResLoc(tile3d, dest);
            tile3d.Dispose();
            
        }

        public override string Name
        {
            get { return Program.StringTable[CsfKey]; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".tem", ".sno", ".lun", ".des", ".urb", ".ubn" }; }
        }
        public override string[] DestExt
        {
            get { return new string[] { ".tem", ".sno", ".lun", ".des", ".urb", ".ubn", ".tmp3" }; }
        }

        public override string SourceDesc
        {
            get { return Program.StringTable["DOCS:TileDesc"]; }
        }

        public override string DestDesc
        {
            get { return Program.StringTable["DOCS:TILE3DDesc"]; }
        }
    }
}
