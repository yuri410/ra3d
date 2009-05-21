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
using System.IO;
using System.Text;
using R3D.GraphicsEngine.Animating;
using R3D.IO;
using SlimDX.Direct3D9;
using SlimDX;

namespace R3D.GraphicsEngine
{
    public abstract class GameModelBase<MeshType> where MeshType : class
    {
        protected readonly string EntityCountTag = "EntityCount";

        protected readonly string EntityPrefix = "Ent";
        protected readonly string AnimationTag = "Animation";


        protected MeshType[] entities;
        protected Animation animation;
        protected Device dev;

        protected GameModelBase(Device device)
        {
            dev = device;
        }

        [Browsable(false)]
        public Device Device
        {
            get { return dev; }
        }

        public Animation ModelAnimation
        {
            get { return animation; }
            set { animation = value; }
        }

        //[Browsable(false)]
        public MeshType[] Entities
        {
            get { return entities; }
            set { entities = value; }
        }

        protected abstract MeshType LoadMesh(BinaryDataReader data);
        protected abstract BinaryDataWriter SaveMesh(MeshType mesh);

        protected void ReadData(BinaryDataReader data)
        {
            int entCount = data.GetDataInt32(EntityCountTag);
            entities = new MeshType[entCount];

            ArchiveBinaryReader br;
            for (int i = 0; i < entCount; i++)
            {
                br = data.GetData(EntityPrefix + i.ToString());
                BinaryDataReader meshData = br.ReadBinaryData();
                entities[i] = LoadMesh(meshData);
                meshData.Close();
                br.Close();
            }

            br = data.GetData(AnimationTag);
            string animType = br.ReadStringUnicode();

            switch (animType)
            {
                case NoAnimation.TypeName:
                    animation = new NoAnimation(dev, entCount);
                    break;
                default:
                    throw new NotSupportedException(animType);
            }

            BinaryDataReader animData = br.ReadBinaryData();
            animation.ReadData(animData);
            animData.Close();
            br.Close();
        }
        protected void WriteData(BinaryDataWriter data)
        {
            data.AddEntry(EntityCountTag, entities.Length);

            ArchiveBinaryWriter bw;
            for (int i = 0; i < entities.Length; i++)
            {
                bw = data.AddEntry(EntityPrefix + i.ToString());

                BinaryDataWriter meshData = SaveMesh(entities[i]);
                bw.Write(meshData);
                bw.Close();
            }

            bw = data.AddEntry(AnimationTag);
            bw.WriteStringUnicode(animation.AnimationType);

            BinaryDataWriter animData = new BinaryDataWriter();
            animation.WriteData(animData);
            bw.Write(animData);

            bw.Close();

        }
    }
    public class GameModel : GameModelBase<GameMesh>, IRenderable, IDisposable
    {
        //class Data : DataSrcBase
        //{
        //    List<GameMesh> entities;

        //    public Data()
        //    {
        //        entities = new List<GameMesh.Data>();
        //    }



        //    public override void Load()
        //    {
        //        if (resLocation == null)
        //            throw new InvalidOperationException();

        //        ArchiveBinaryReader br = new ArchiveBinaryReader(resLocation);

        //        if (br.ReadInt32() == (int)FileID.Model)
        //        {
        //            br.ReadInt32();
        //            int count = br.ReadInt32();
        //            entities.Clear();
        //            entities.Capacity = count;

        //            for (int i = 0; i < count; i++)
        //            {
        //                GameMesh.Data sounds = new GameMesh.Data();
        //                sounds.ResourceLocation = new StreamedLocation(br.BaseStream, false);
        //                sounds.Load();
        //                entities.Add(sounds);
        //            }
        //            br.Close();
        //        }
        //        else
        //        {
        //            throw new DataFormatException(resLocation);
        //        }

        //    }

        //    public override void Save()
        //    {
        //        if (resLocation == null)
        //            throw new InvalidOperationException();
        //        ArchiveBinaryWriter bw = new ArchiveBinaryWriter(resLocation);
        //        //bw.BaseStream.SetLength(0);
        //        bw.Write((int)FileID.Model);
        //        bw.Write((int)0);
        //        bw.Write(entities.Count);
        //        bw.Flush();

        //        for (int i = 0; i < entities.Count; i++)
        //        {
        //            entities[i].ResourceLocation = new StreamedLocation(bw.BaseStream, false);
        //            entities[i].Save();
        //        }
        //        bw.Flush();
        //        bw.Close();
        //    }
        //}

        //GeomentryData[] gmBuffer;
        RenderOperation[] opBuffer;
        int[] renderOpEntId;


        public GameModel(Device dev, Animation anim, GameMesh[] meshes)
            : base(dev)
        {
            this.animation = animation;
            this.entities = meshes;

        }

        public GameModel(Device dev, int entityCount)
            : base(dev)
        {
            entities = new GameMesh[entityCount];
            animation = new NoAnimation(dev, entityCount);
        }

        private GameModel(Device dev)
            : base(dev) { }

        protected override GameMesh LoadMesh(BinaryDataReader data)
        {
            GameMesh.Data md = new GameMesh.Data(dev);
            md.Load(data);
            return new GameMesh(dev, md);
        }
        protected override BinaryDataWriter SaveMesh(GameMesh mesh)
        {
            GameMesh.Data md = new GameMesh.Data(mesh);
            return md.Save();
        }

        public static GameModel FromFile(Device dev, ResourceLocation rl)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(rl);
            if (br.ReadInt32() == (int)FileID.Model)
            {
                BinaryDataReader data = br.ReadBinaryData();
                GameModel mdl = FromBinary(dev, data);
                br.Close();
                return mdl;
            }
            br.Close();
            throw new InvalidDataException();

        }
        public static void ToStream(GameModel mdl, Stream stm)
        {
            ArchiveBinaryWriter bw = new ArchiveBinaryWriter(stm, Encoding.Default);

            bw.Write((int)FileID.Model);

            bw.Write(ToBinary(mdl));

            bw.Close();
        }
        public static GameModel FromBinary(Device dev, BinaryDataReader data)
        {
            GameModel mdl = new GameModel(dev);

            mdl.ReadData(data);

            return mdl;
        }
        public static BinaryDataWriter ToBinary(GameModel mdl)
        {
            BinaryDataWriter data = new BinaryDataWriter();
            mdl.WriteData(data);
            return data;
        }

        //        public void Render()
        //        {
        //#warning anim
        //            if (entities != null)
        //            {
        //                if (animation == null)
        //                {
        //                    NoAnimation noAnim = new NoAnimation(dev, entities.Length);
        //                    for (int i = 0; i < entities.Length; i++)
        //                    {
        //                        noAnim.Transforms[i] = Matrix.Identity;
        //                    }
        //                    animation = noAnim;
        //                }

        //                animation.Begin();
        //                for (int i = 0; i < entities.Length; i++)
        //                {
        //                    if (animation.IsAvailable(i))
        //                    {
        //                        animation.SetAnimation(i);
        //                    }

        //                    entities[i].Render();
        //                    //DevUtils.DrawEditMesh(GraphicsDevice.Instance.Device, entities[i]);
        //                }
        //                animation.End();
        //            }
        //        }

        #region IRenderable 成员

        public RenderOperation[] GetRenderOperation()
        {
            if (opBuffer == null)
            {
                RenderOperation[][] entOps = new RenderOperation[entities.Length][];

                int opCount = 0;
                for (int i = 0; i < entities.Length; i++)
                {
                    entOps[i] = entities[i].GetRenderOperation();
                    //for (int j = 0; j < entOps[i].Length; j++)
                    //{
                    //    animation.GetTransform(i, out entOps[i][j].Transformation);
                    //}
                    opCount += entOps[i].Length;
                }

                int dstIdx = 0;
                //gmBuffer = new GeomentryData[opCount];
                opBuffer = new RenderOperation[opCount];
                renderOpEntId = new int[opCount];

                for (int i = 0; i < entities.Length; i++)
                {
                    Array.Copy(entOps[i], 0, opBuffer, dstIdx, entOps[i].Length);

                    for (int j = 0; j < entOps[i].Length; j++)
                    {
                        renderOpEntId[dstIdx + j] = i;
                        //opBuffer[dstIdx + j].Geomentry = entOps[i][j];
                        animation.GetTransform(i, out  opBuffer[dstIdx + j].Transformation);
                    }

                    dstIdx += entOps[i].Length;
                }
            }
            else
            {
                for (int i = 0; i < opBuffer.Length; i++)
                {
                    animation.GetTransform(renderOpEntId[i], out opBuffer[i].Transformation);
                }
                //animation.Animate();
            }
            return opBuffer;
        }


        #endregion

        #region IDisposable 成员

        public bool Disposed
        {
            get;
            protected set;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                animation.Dispose();
                for (int i = 0; i < entities.Length; i++)
                {
                    entities[i].Dispose();
                }

                Disposed = true;
            }
            else 
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }
}
