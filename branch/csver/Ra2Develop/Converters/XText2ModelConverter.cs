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
using System.Windows.Forms;
using Ra2Develop.Designers;
using Ra2Develop.Editors.EditableObjects;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Animating;
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Develop.Converters
{
    public class XText2ModelConverter : ConverterBase
    {
        const string CsfKey = "GUI:X2Mesh";

        public override void ShowDialog(object sender, EventArgs e)
        {
            string[] files;
            string path;
            if (ConvDlg.Show(Program.StringTable[CsfKey], GetOpenFilter(), out files, out path) == DialogResult.OK)
            {
                ProgressDlg pd = new ProgressDlg(Program.StringTable["GUI:Converting"]);

                pd.MinVal = 0;
                pd.Value = 0;
                pd.MaxVal = files.Length;

                pd.Show();
                for (int i = 0; i < files.Length; i++)
                {
                    string dest = Path.Combine(path, Path.GetFileNameWithoutExtension(files[i]) + ".mesh");

                    Convert(new DevFileLocation(files[i]), new DevFileLocation(dest));
                    pd.Value = i;
                }
                pd.Close();
                pd.Dispose();
            }
        }

        public override void Convert(ResourceLocation source, ResourceLocation dest)
        {
            Mesh mesh = Mesh.FromStream(GraphicsDevice.Instance.Device, source.GetStream, MeshFlags.Managed);

            ExtendedMaterial[] mats = mesh.GetMaterials();
            EditableMeshMaterial[] outMats = new EditableMeshMaterial[mats.Length];
            for (int i = 0; i < mats.Length; i++)
            {
                outMats[i] = new EditableMeshMaterial();
                outMats[i].mat = mats[i].MaterialD3D;
                outMats[i].TextureFile1 = mats[i].TextureFileName;
            }


            string name = string.Empty;
            FileLocation fl = source as FileLocation;
            if (fl != null)
            {
                name = Path.GetFileNameWithoutExtension(fl.Path);
            }
            EditableMesh outMesh = new EditableMesh(name, mesh, outMats);


            EditableModel outMdl = new EditableModel();


           
            mesh.Dispose();

            outMdl.Entities = new EditableMesh[] { outMesh };
            outMdl.ModelAnimation = new NoAnimation(GraphicsDevice.Instance.Device, new Matrix[] { Matrix.Identity });

            EditableModel.ToFile(outMdl, dest);


            outMdl.Dispose();
        }

        public override string Name
        {
            get { return Program.StringTable[CsfKey]; }
        }

        public override string[] SourceExt
        {
            get { return new string[] { ".x" }; }
        }
        public override string[] DestExt
        {
            get { return new string[] { ".mesh" }; }
        }

        public override string SourceDesc
        {
            get { return Program.StringTable["DOCS:MeshDesc"]; }
        }

        public override string DestDesc
        {
            get { return Program.StringTable["Docs:XTextDesc"]; }
        }
    }
}
