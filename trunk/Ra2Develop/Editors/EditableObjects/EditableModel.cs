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
using System.Drawing.Design;
using System.IO;
using System.Text;
using Ra2Develop.Converters;
using Ra2Develop.Designers;
using R3D.Design;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Animating;
using R3D.IO;
using SlimDX;
using SlimDX.Direct3D9;

namespace Ra2Develop.Editors.EditableObjects
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [Editor(typeof(ModelEditor), typeof(UITypeEditor))]
    //[Editor(typeof(GameModelArrayEditor), typeof (UITypeEditor))]
    public class EditableModel : GameModelBase<EditableMesh>, IDisposable
    {

        //public event EditObjChangedHandler StateChanged;
                

        public EditableModel()
            : base(GraphicsDevice.Instance.Device)
        { }
        public EditableModel(Animation anim)
            : base(GraphicsDevice.Instance.Device)
        {
            animation = anim;
        }

        [Editor(typeof(MeshArrayEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(ArrayConverter<EmbededMeshEditor, EditableMesh>))]
        public new EditableMesh[] Entities
        {
            get { return entities; }
            set
            {
                entities = value;
                NoAnimation noAnim = animation as NoAnimation;
                if (noAnim != null && noAnim.Transforms.Length != entities.Length)
                {
                    // 更新

                    Matrix[] oldTrans = noAnim.Transforms;
                    noAnim.Transforms = new Matrix[entities.Length];

                    if (oldTrans.Length > entities.Length)
                    {
                        //缩小
                        Array.Copy(oldTrans, noAnim.Transforms, entities.Length);
                    }
                    else
                    {
                        //增添
                        Array.Copy(oldTrans, noAnim.Transforms, noAnim.Transforms.Length);
                        for (int i = noAnim.Transforms.Length; i < entities.Length; i++)
                        {
                            noAnim.Transforms[i] = Matrix.Identity;
                        }
                    }
                }
            }
        }


        public static unsafe EditableModel FromFile(ResourceLocation rl)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(rl);
            if (br.ReadInt32() == (int)FileID.Model)
            {
                BinaryDataReader data = br.ReadBinaryData();
                EditableModel mdl = FromBinary(data);
                br.Close();
                return mdl;
            }
            br.Close();
            throw new InvalidDataException();
        }
        public static void ToFile(EditableModel mdl, ResourceLocation dest)
        {
            ArchiveBinaryWriter bw = new ArchiveBinaryWriter(dest);

            bw.Write((int)FileID.Model);

            bw.Write(ToBinary(mdl));
            bw.Close();
        }

        public static EditableModel FromBinary(BinaryDataReader data)
        {
            EditableModel mdl = new EditableModel();

            mdl.ReadData(data);

            return mdl;
        }
        public static unsafe BinaryDataWriter ToBinary(EditableModel mdl)
        {
            BinaryDataWriter data = new BinaryDataWriter();
            mdl.WriteData(data);
            return data;
        }


        //public GameMesh.Data[] Entities
        //{
        //    get { }
        //}

        //void UpdateAnimation()
        //{

        //}

        public void Render()
        {
#warning anim
            if (entities != null)
            {
                if (animation == null)
                {
                    NoAnimation noAnim = new NoAnimation(dev, entities.Length);
                    for (int i = 0; i < entities.Length; i++)
                    {
                        noAnim.Transforms[i] = Matrix.Identity;
                    }
                    animation = noAnim;
                }

                Matrix baseTrans = dev.GetTransform(TransformState.World);

                //animation.Begin();
                for (int i = 0; i < entities.Length; i++)
                {
                    dev.SetTransform(TransformState.World, baseTrans);
                    if (animation.IsAvailable(i))
                    {
                        //animation.SetAnimation(i);
                        Matrix trans;
                        animation.GetTransform(i, out trans);
                        dev.MultiplyTransform(TransformState.World, trans);
                    }

                    entities[i].Render();
                    //DevUtils.DrawEditMesh(GraphicsDevice.Instance.Device, entities[i]);
                }
                //animation.End();
            }
        }

        protected override EditableMesh LoadMesh(BinaryDataReader data)
        {
            EditableMesh mesh = new EditableMesh();
            mesh.Load(data);
            return mesh;
        }
        protected override BinaryDataWriter SaveMesh(EditableMesh mesh)
        {
            return mesh.Save();
        }

        public void ImportEntityFromXml(string file)
        {
            Xml2ModelConverter conv = new Xml2ModelConverter();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(65536);
            conv.Convert(new FileLocation(file), new StreamedLocation(new VirtualStream(ms, 0)));

            ms.Position = 0;

            EditableModel data = EditableModel.FromFile(new StreamedLocation(ms));

            if (animation is NoAnimation && data.animation is NoAnimation)
            {
                EditableMesh[] newEnt = new EditableMesh[data.entities.Length + entities.Length];
                EditableMesh[] addEnt = data.entities;
                Array.Copy(entities, newEnt, entities.Length);
                Array.Copy(addEnt, 0, newEnt, entities.Length, addEnt.Length);



                NoAnimation na = (NoAnimation)animation;

                Matrix[] newTrans = new Matrix[newEnt.Length];
                Matrix[] addTrans = ((NoAnimation)data.animation).Transforms;

                Array.Copy(na.Transforms, newTrans, na.Transforms.Length);
                Array.Copy(addTrans, 0, newTrans, na.Transforms.Length, addTrans.Length);

                entities = newEnt;
                na.Transforms = newTrans;
            }
        }


        #region IDisposable 成员

        public void Dispose()
        {
            if (entities != null)
            {
                for (int i = 0; i < entities.Length; i++)
                {
                    entities[i].Dispose();
                }
                entities = null;
            }
            animation = null;
            //StateChanged = null;
        }

        #endregion
    }
}
