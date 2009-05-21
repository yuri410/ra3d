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
using System.Text;
using R3D.Collections;
using R3D.GraphicsEngine;
using R3D.GraphicsEngine.Effects;
using R3D.IO;
using R3D.MathLib;
using R3D.Media;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Core
{
    public unsafe abstract class SceneManagerBase : IDisposable
    {
        //struct PicInPicVtx
        //{
        //    public Vector2 pos;
        //    float dummyZ;
        //    float dummyW;

        //    public Vector2 tex1;

        //    public static VertexFormat Format
        //    {
        //        get { return VertexFormat.PositionRhw | VertexFormat.Texture1; }
        //    }
        //}

        //[TestOnly()]
        //struct TestVertex
        //{
        //    public Vector3 pos;
        //    public Vector3 n;

        //    public int diffuse;


        //    public static VertexFormat Format
        //    {
        //        get { return VertexFormat.Position | VertexFormat.Normal | VertexFormat.Diffuse; }
        //    }
        //}
        //[TestOnly()]
        //VertexBuffer[] testVxl;
        //[TestOnly()]
        //RenderOperation[] testOp;
        //[TestOnly()]
        //int testFrame;

        VertexBuffer axis;

        Device device;

        Atmosphere atmos;
        ShadowMap shadowMap;

        //PicInPicVtx[] testQuad;
        //VertexDeclaration testDecl;


        //List<ISceneObject> renderQueue;

        ///// <summary>
        ///// Dictionary(效果哈希代码，Dictionary(材质哈希代码，List(Pair(SceneObject,int))))
        ///// </summary>        
        //Dictionary<int, Dictionary<int, List<Pair<ISceneObject, int>>>> batchTable;

        Dictionary<int, FastList<RenderOperation>> batchTable;
        Dictionary<int, Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>> instanceTable;

        Dictionary<int, ModelEffect> effects;


        protected FastList<SceneObject> visiableObjects;
        //Dictionary<int, MeshMaterial> materials;

        //Queue<SceneObject> multiBatchBuffer;
        //Queue<SceneObject> multiBatchBuffer2;

        /// <summary>
        /// 摄像机列表，不包含effect的
        /// </summary>
        List<Camera> cameraList;


        RenderOperation axisOp;


        void BuildAxis()
        {
            axis = new VertexBuffer(device, sizeof(VertexPC) * 6, Usage.None, VertexPC.Format, Pool.Managed);
            VertexPC* dst = (VertexPC*)axis.Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            Vector3 centre = new Vector3();
            centre.Y += 15;

            float ext = 100;

            dst[0] = new VertexPC { pos = centre, diffuse = Color.Red.ToArgb() };
            dst[1] = new VertexPC { pos = new Vector3(ext + centre.X, centre.Y, centre.Z), diffuse = Color.Red.ToArgb() };
            dst[2] = new VertexPC { pos = centre, diffuse = Color.Green.ToArgb() };
            dst[3] = new VertexPC { pos = new Vector3(centre.X, centre.Y + ext, centre.Z), diffuse = Color.Green.ToArgb() };
            dst[4] = new VertexPC { pos = centre, diffuse = Color.Blue.ToArgb() };
            dst[5] = new VertexPC { pos = new Vector3(centre.X, centre.Y, centre.Z + ext), diffuse = Color.Blue.ToArgb() };

            axis.Unlock();


            axisOp.Geomentry = new GeomentryData(null);
            axisOp.Geomentry.Format = VertexPC.Format;
            axisOp.Geomentry.IndexBuffer = null;
            axisOp.Material = MeshMaterial.DefaultMaterial;
            axisOp.Geomentry.PrimCount = 3;
            axisOp.Geomentry.PrimitiveType = PrimitiveType.LineList;
            axisOp.Transformation = Matrix.Identity;
            axisOp.Geomentry.VertexBuffer = axis; ;
            axisOp.Geomentry.VertexCount = 6;
            axisOp.Geomentry.VertexDeclaration = new VertexDeclaration(device, D3DX.DeclaratorFromFVF(VertexPC.Format));
            axisOp.Geomentry.VertexSize = sizeof(VertexPC);


            //testVxl = new VertexBuffer[Game.testVxl.Section.Length];
            //testOp = new RenderOperation[Game.testVxl.Section.Length];

            //Palette pal = Game.testPal;

            //MeshMaterial vxlMat = new MeshMaterial(device);
            //vxlMat.CullMode = Cull.None;
            //vxlMat.mat.Ambient = new Color4(1f, 0.2f, 0.2f, 0.2f);
            //vxlMat.mat.Diffuse = new Color4(1f, 0.8f, 0.8f, 0.8f);
            //vxlMat.mat.Specular = new Color4(0f, 0.0f, 0.0f, 0.0f);

            //vxlMat.SetEffect(EffectManager.Instance.GetModelEffect(StandardEffectFactory.Name));

            //for (int i = 0; i < testVxl.Length; i++)
            //{
            //    VoxelSection vxls = Game.testVxl.Section[i];

            //    VoxelData[][][] sounds = vxls.Data;


            //    List<TestVertex> vertices = new List<TestVertex>(3000);
            //    for (int x = 0; x < sounds.Length; x++)
            //    {
            //        for (int y = 0; y < sounds[x].Length; y++)
            //        {
            //            for (int z = 0; z < sounds[x][y].Length; z++)
            //            {
            //                if (sounds[x][y][z].used)
            //                {
            //                    TestVertex vtx;
            //                    vtx.diffuse = (int)pal.ARGBData[sounds[x][y][z].colour];

            //                    if (sounds[x][y][z].normal > 243)
            //                        sounds[x][y][z].normal = 243;
            //                    vtx.n = Game.ra2Normals[sounds[x][y][z].normal];

            //                    //float temp = vtx.n.Z;
            //                    //vtx.n.Z = vtx.n.Y;
            //                    //vtx.n.Y = temp;


            //                    vtx.pos = new Vector3(x * 12, y * 12, z * 12);
            //                    vertices.Add(vtx);
            //                }
            //            }
            //        }
            //    }

            //    if (vertices.Count > 0)
            //    {
            //        testVxl[i] = new VertexBuffer(device, sizeof(TestVertex) * vertices.Count, Usage.WriteOnly, TestVertex.Format, Pool.Managed);

            //        void* dst2 = testVxl[i].Lock(0, 0, LockFlags.None).DataPointer.ToPointer();

            //        TestVertex[] vtxArr = vertices.ToArray();
            //        fixed (void* src = &vtxArr[0])
            //        {
            //            Memory.Copy(src, dst2, sizeof(TestVertex) * vertices.Count);
            //        }

            //        testVxl[i].Unlock();
            //    }

            //    testOp[i].Geomentry = new GeomentryData(null);
            //    testOp[i].Geomentry.Format = TestVertex.Format;
            //    testOp[i].Geomentry.IndexBuffer = null;
            //    testOp[i].Material = vxlMat;
            //    testOp[i].Geomentry.PrimCount = vertices.Count;
            //    testOp[i].Geomentry.PrimitiveType = PrimitiveType.PointList;
            //    testOp[i].Transformation = Matrix.Identity;
            //    testOp[i].Geomentry.VertexBuffer = testVxl[i];
            //    testOp[i].Geomentry.VertexCount = vertices.Count;
            //    testOp[i].Geomentry.VertexDeclaration = new VertexDeclaration(device, D3DX.DeclaratorFromFVF(TestVertex.Format));
            //    testOp[i].Geomentry.VertexSize = sizeof(TestVertex);


            //}


        }

        public bool Disposed
        {
            get;
            private set;
        }

        public Camera CurrentCamera
        {
            get;
            protected set;
        }

        public Atmosphere Atmosphere
        {
            get { return atmos; }
            set { atmos = value; }
        }

        /// <summary>
        /// Gets the number of primitives rendered.
        /// 获取渲染的图元数量
        /// </summary>
        public int PrimitiveCount
        {
            get;
            protected set;
        }

        /// <summary>
        /// 获取渲染的顶点数量
        /// </summary>
        public int VertexCount
        {
            get;
            protected set;
        }

        /// <summary>
        /// 获取渲染批次数量
        /// </summary>
        public int BatchCount
        {
            get;
            protected set;
        }

        public int RenderedObjectCount
        {
            get;
            protected set;
        }

        protected SceneManagerBase(Device dev, Atmosphere atmos)
        {
            device = dev;

            Atmosphere = atmos;
            cameraList = new List<Camera>();

            //multiBatchBuffer = new Queue<SceneObject>();
            //multiBatchBuffer2 = new Queue<SceneObject>();
            //postEffects = new List<PostEffect>();

            //renderQueue = new List<ISceneObject>();

            //batchTable = new Dictionary<int, Dictionary<int, List<Pair<ISceneObject, int>>>>();
            //effects = new Dictionary<int, EffectBase>();
            //materials = new Dictionary<int, MeshMaterial>();

            //effects.Add(0, null);

            effects = new Dictionary<int, ModelEffect>();
            batchTable = new Dictionary<int, FastList<RenderOperation>>();
            instanceTable = new Dictionary<int, Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>>();


            visiableObjects = new FastList<SceneObject>();

            shadowMap = new ShadowMap(dev);

            BuildAxis();
            BuildSceneManager();

            //testQuad = new PicInPicVtx[4];

            //testQuad[0].pos = new Vector2(0, 0);
            //testQuad[0].tex1 = new Vector2(0, 0);
            ////testQuad[0].dummy = 1;
            //testQuad[1].pos = new Vector2(256, 0);
            //testQuad[1].tex1 = new Vector2(1, 0);
            ////testQuad[1].dummy = 1;
            //testQuad[2].pos = new Vector2(0, 256);
            //testQuad[2].tex1 = new Vector2(0, 1);
            ////testQuad[2].dummy = 1;
            //testQuad[3].pos = new Vector2(256, 256);
            //testQuad[3].tex1 = new Vector2(1, 1);
            ////testQuad[3].dummy = 1;
            ////testDecl = new VertexDeclaration(device, D3DX.DeclaratorFromFVF(PicInPicVtx.Format));
        }

        protected abstract void BuildSceneManager();

        public void RegisterCamera(Camera cam)
        {
            cameraList.Add(cam);
        }

        public void UnregisterCamera(Camera cam)
        {
            cameraList.Remove(cam);
        }

        /// <summary>
        /// add object to the scene and attach it to a suitable node
        /// </summary>
        /// <param name="obj"></param>
        public abstract void AddObjectToScene(SceneObject obj);
        public abstract void RemoveObjectFromScene(SceneObject obj);

        //public virtual void AddChild(SceneNode node)
        //{
        //    sceneNodes.Add(node);
        //}

        //public virtual SceneNode FindNode(SceneObject obj)
        //{
        //    throw new NotImplementedException();
        //}

        public ShadowMap ShadowMap
        {
            get { return shadowMap; }
        }

        //[TestOnly()]
        //void AddOp(RenderOperation[] ops)
        //{
        //    for (int k = 0; k < ops.Length; k++)
        //    {
        //        if (ops[k].Geomentry != null)
        //        {
        //            //renderQueue.Enqueue(ops[k]);

        //            MeshMaterial mate = ops[k].Material;

        //            if (mate != null)
        //            {
        //                FastList<RenderOperation> opList;
        //                if (mate.Effect == null)
        //                {
        //                    ModelEffect effect;
        //                    if (!effects.TryGetValue(0, out effect))
        //                    {
        //                        effects.Add(0, null);
        //                    }

        //                    if (!batchTable.TryGetValue(0, out opList))
        //                    {
        //                        opList = new FastList<RenderOperation>();
        //                        batchTable.Add(0, opList);
        //                    }

        //                }
        //                else
        //                {
        //                    int hash = mate.Effect.GetHashCode();
        //                    ModelEffect effect;
        //                    if (!effects.TryGetValue(hash, out effect))
        //                    {
        //                        effects.Add(hash, mate.Effect);
        //                    }


        //                    if (!batchTable.TryGetValue(hash, out opList))
        //                    {
        //                        opList = new FastList<RenderOperation>();
        //                        batchTable.Add(hash, opList);
        //                    }
        //                }

        //                //Matrix.Multiply(ref ops[k].Transformation, ref obj.Transformation, out ops[k].Transformation);
        //                //ops[k].Transformation = obj.Transformation;
        //                opList.Add(ref ops[k]);
        //            }
        //        }
        //    }
        //}

        void AddAxisOperation()
        {
            ModelEffect effect;
            if (!effects.TryGetValue(0, out effect))
            {
                effects.Add(0, null);
            }

            FastList<RenderOperation> opList;
            if (!batchTable.TryGetValue(0, out opList))
            {
                opList = new FastList<RenderOperation>();
                batchTable.Add(0, opList);
            }
            opList.Add(ref axisOp);


            //device.SetRenderState(RenderState.PointSize, 2f);
            //device.SetRenderState(RenderState.NormalizeNormals, true);

            //Matrix vxlScale = Matrix.Scaling(0.0083333f, 0.0083333f, 0.0083333f);
            ////Matrix vxlRotate
            //Matrix vxlTrans = Matrix.Translation(99 * Terrain.HorizontalUnit, 5 * Terrain.VerticalUnit, 77 * Terrain.HorizontalUnit);//

            ////Matrix.Multiply (ref vxlScale,ref vxlTrans, out 

            ////Matrix cur = Matrix.Identity;

            //for (int i = 0; i < testOp.Length; i++)
            //{               
            //    Matrix hvatrans = Game.testHva.GetTransform(i)[testFrame];

            //    Game.testVxl.Section[i].GetTransform(out testOp[i].Transformation);

            //    Matrix.Multiply(ref testOp[i].Transformation,ref hvatrans,  out testOp[i].Transformation);
            //    Matrix.Multiply(ref testOp[i].Transformation, ref vxlScale, out testOp[i].Transformation);
            //    //testOp[i].Transformation = cur;

            //    Matrix.Multiply(ref testOp[i].Transformation, ref vxlTrans, out testOp[i].Transformation);
            //}

            //AddOp(testOp);

            //testFrame++;
            //if (testFrame >= Game.testHva.FrameCount)
            //{
            //    testFrame = 0;
            //}
        }

        public void RenderScene()
        {
            VertexCount = 0;
            BatchCount = 0;
            PrimitiveCount = 0;
            RenderedObjectCount = 0;

            AddAxisOperation();
            //device.SetTransform(TransformState.Projection, cam.Projection);
            //device.SetTransform(TransformState.View, cam.View);
            //device.SetTransform(TransformState.World, Matrix.Identity);

            //RenderVisibleObjects(cam);

            //先给每个camera渲染到他们的renderTarget
            //然后运行postEffect
            for (int i = 0; i < cameraList.Count; i++)
            {

                CurrentCamera = cameraList[i];

                PrepareVisibleObjects(CurrentCamera);

                device.PixelShader = null;
                device.VertexShader = null;
                shadowMap.Begin(atmos.LightDirection, CurrentCamera);

                device.SetTexture(0, null);
                device.SetRenderState(RenderState.AlphaTestEnable, false);
                device.SetRenderState<FogMode>(RenderState.FogTableMode, FogMode.None);


                ////device.SetRenderState(RenderState.ColorWriteEnable, 0);
                device.SetRenderState(RenderState.Lighting, false);
                device.SetRenderState(RenderState.AlphaBlendEnable, false);
                device.SetRenderState<Cull>(RenderState.CullMode, Cull.None);

                foreach (KeyValuePair<int, FastList<RenderOperation>> e1 in batchTable)
                {
                    FastList<RenderOperation> opList = e1.Value;
                    for (int j = 0; j < opList.Count; j++)
                    {
                        RenderOperation op = opList.Elements[j];

                        if (op.Geomentry.VertexCount == 0)
                            continue;

                        BatchCount++;
                        PrimitiveCount += op.Geomentry.PrimCount;
                        VertexCount += op.Geomentry.VertexCount;

                        MeshMaterial mate = op.Material;
                        if (mate == null)
                            mate = MeshMaterial.DefaultMaterial;


                        shadowMap.SetTransform(ref op.Transformation);
                        device.SetTransform(TransformState.World, op.Transformation);
                        device.SetStreamSource(0, op.Geomentry.VertexBuffer, 0, op.Geomentry.VertexSize);
                        device.VertexFormat = op.Geomentry.Format;
                        device.VertexDeclaration = op.Geomentry.VertexDeclaration;

                        if (op.Geomentry.UseIndices)
                        {
                            device.Indices = op.Geomentry.IndexBuffer;
                            device.DrawIndexedPrimitives(op.Geomentry.PrimitiveType, 0, 0, op.Geomentry.VertexCount, 0, op.Geomentry.PrimCount);
                        }
                        else
                        {
                            device.DrawPrimitives(op.Geomentry.PrimitiveType, 0, op.Geomentry.PrimCount);
                        }
                    }
                }

                shadowMap.End();


                device.SetRenderTarget(0, CurrentCamera.RenderTarget);

                device.SetTransform(TransformState.Projection, CurrentCamera.ProjectionMatrix);
                device.SetTransform(TransformState.World, Matrix.Identity);
                device.SetTransform(TransformState.View, CurrentCamera.ViewMatrix);


                atmos.Render();



                device.PixelShader = null;
                device.VertexShader = null;

                //device.SetTexture(0, shadowMap.ShadowColorMap);
                //device.VertexFormat = PicInPicVtx.Format;
                //device.DrawUserPrimitives<PicInPicVtx>(PrimitiveType.TriangleStrip, 0, 2, testQuad);
                //device.SetRenderState(RenderState.DepthBias, 0.0002f);
                //device.SetRenderState(RenderState.SlopeScaleDepthBias, 2f);

                device.SetRenderState(RenderState.AlphaBlendEnable, false);
                device.SetRenderState(RenderState.Lighting, false);
                device.SetTexture(0, null);

                foreach (KeyValuePair<int, FastList<RenderOperation>> e1 in batchTable)
                {
                    FastList<RenderOperation> opList = e1.Value;

                    if (opList.Count > 0)
                    {
                        device.PixelShader = null;
                        device.VertexShader = null;
                        ModelEffect effect = effects[e1.Key];

                        int passCount;
                        if (effect != null)
                            passCount = effect.Begin();
                        else
                            passCount = 1;

                        for (int p = 0; p < passCount; p++)
                        {
                            //PassType pt = effect.GetPassType(p);
                            if (effect != null)
                                effect.BeginPass(p);

                            //switch (pt)
                            //{
                            //case PassType.RenderObjects:

                            for (int j = 0; j < opList.Count; j++)
                            {
                                RenderOperation op = opList.Elements[j];
                                GeomentryData gm = op.Geomentry;

                                if (gm.VertexCount == 0)
                                    continue;

                                BatchCount++;
                                PrimitiveCount += gm.PrimCount;
                                VertexCount += gm.VertexCount;

                                MeshMaterial mate = op.Material;
                                if (mate == null)
                                    mate = MeshMaterial.DefaultMaterial;

                                //device.SetRenderState(RenderState.ZWriteEnable, !mate.IsTransparent);
                                device.SetRenderState(RenderState.AlphaTestEnable, mate.IsTransparent);
                                device.SetRenderState<Cull>(RenderState.CullMode, mate.CullMode);


                                if (effect != null)
                                    effect.Setup(this, op.Material, ref op);
                                else
                                {
                                    device.Material = mate.mat;
                                    for (int tl = 0; tl < MaterialBase.MaxTexLayers; tl++)
                                    {
                                        device.SetTexture(tl, mate.GetTexture(tl));
                                    }

                                    device.SetTransform(TransformState.World, op.Transformation);
                                }

                                device.SetStreamSource(0, gm.VertexBuffer, 0, gm.VertexSize);
                                device.VertexFormat = gm.Format;
                                device.VertexDeclaration = gm.VertexDeclaration;

                                if (gm.UseIndices)
                                {
                                    device.Indices = gm.IndexBuffer;
                                    device.DrawIndexedPrimitives(gm.PrimitiveType, 0, 0, gm.VertexCount, 0, gm.PrimCount);
                                }
                                else
                                {
                                    device.DrawPrimitives(gm.PrimitiveType, 0, gm.PrimCount);
                                }
                            } // for (int j = 0; j < opList.Count; j++)
                            if (effect != null)
                                effect.EndPass();
                        } //  for (int p = 0; p < passCount; p++)
                        if (effect != null)
                            effect.End();
                    } // if (opList.Count > 0)
                }

                //device.SetRenderState(RenderState.DepthBias, 0);
                //device.SetRenderState(RenderState.SlopeScaleDepthBias, 0f);

            }



            Dictionary<int, FastList<RenderOperation>>.ValueCollection vals = batchTable.Values;
            foreach (FastList<RenderOperation> opList in vals)
            {
                opList.FastClear();
            }
            Dictionary<int, Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>>.ValueCollection instTableVals = instanceTable.Values;
            foreach (Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>> matTbl in instTableVals)
            {
                Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>.ValueCollection matTableVals = matTbl.Values;
                foreach (Dictionary<GeomentryData, FastList<RenderOperation>> geoTable in matTableVals)
                {
                    Dictionary<GeomentryData, FastList<RenderOperation>>.ValueCollection geoTableVals = geoTable.Values;
                    foreach (FastList<RenderOperation> opList in geoTableVals)
                    {
                        opList.FastClear();
                    }
                }
            }
            effects.Clear();
        }

        public abstract void FindObjects(FastList<SceneObject> objects, Frustum frus);
        public abstract SceneObject FindObject(Ray ray);
        protected abstract void PrepareVisibleObjects(Camera camera);

        /// <summary>
        /// 用于渲染批次优化
        /// </summary>
        /// <param name="obj"></param>
        protected void AddObject(SceneObject obj)
        {
            RenderedObjectCount++;

            visiableObjects.Add(obj);

            RenderOperation[] ops = obj.GetRenderOperation();
            if (ops != null)
            {
                for (int k = 0; k < ops.Length; k++)
                {
                    if (ops[k].Geomentry != null)
                    {
                        //renderQueue.Enqueue(ops[k]);
                        Matrix.Multiply(ref ops[k].Transformation, ref obj.Transformation, out ops[k].Transformation);

                        MeshMaterial mate = ops[k].Material;
                        GeomentryData geoData = ops[k].Geomentry;

                        if (mate != null)
                        {
                            int hash;
                            bool supportsInst;
                            if (mate.Effect == null)
                            {
                                hash = 0;
                                // if effect is null, instancing is supported by defualt
                                supportsInst = true;
                            }
                            else
                            {
                                supportsInst = mate.Effect.SupportsInstancing;
                                hash = mate.Effect.GetHashCode();
                            }

                            //ModelEffect effect;
                            //if (!effects.TryGetValue(0, out effect))
                            //{
                            //    effects.Add(0, null);
                            //}


                            //if (!batchTable.TryGetValue(0, out opList))
                            //{
                            //    opList = new FastList<RenderOperation>();
                            //    batchTable.Add(0, opList);
                            //}

                            if (supportsInst)
                            {
                                ModelEffect effect;
                                if (!effects.TryGetValue(hash, out effect))
                                {
                                    effects.Add(hash, mate.Effect);
                                }

                                Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>> matTable;
                                if (!instanceTable.TryGetValue(hash, out matTable))
                                {
                                    matTable = new Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>();
                                    instanceTable.Add(hash, matTable);
                                }

                                Dictionary<GeomentryData, FastList<RenderOperation>> geoDataTbl;
                                if (!matTable.TryGetValue(mate, out geoDataTbl))
                                {
                                    geoDataTbl = new Dictionary<GeomentryData, FastList<RenderOperation>>();
                                    matTable.Add(mate, geoDataTbl);
                                }

                                FastList<RenderOperation> instOpList;
                                if (!geoDataTbl.TryGetValue(geoData, out instOpList))
                                {
                                    instOpList = new FastList<RenderOperation>();
                                    geoDataTbl.Add(geoData, instOpList);
                                }

                                instOpList.Add(ref ops[k]);
                            }
                            else
                            {
                                ModelEffect effect;
                                FastList<RenderOperation> opList;

                                if (!effects.TryGetValue(hash, out effect))
                                {
                                    effects.Add(hash, mate.Effect);
                                }

                                if (!batchTable.TryGetValue(hash, out opList))
                                {
                                    opList = new FastList<RenderOperation>();
                                    batchTable.Add(hash, opList);
                                }

                                //Matrix.Multiply(ref ops[k].Transformation, ref obj.Transformation, out ops[k].Transformation);
                                //ops[k].Transformation = obj.Transformation;
                                opList.Add(ref ops[k]);
                            }
                        }
                    }
                }
            }



            //renderQueue.Enqueue(obj);
            //MeshMaterial[] mats = obj.Materials;

            //// 预先获取无效果的材质表
            //Dictionary<int, List<ISceneObject>> matTableNE;
            //if (!batchTable.TryGetValue(0, out matTableNE))
            //{
            //    matTableNE = new Dictionary<int, List<Pair<ISceneObject, int>>>();
            //    batchTable.Add(0, matTableNE);
            //}


            //for (int i = 0; i < mats.Length; i++)
            //{
            //    int hash;
            //    Dictionary<int, List<SceneObject>> matTable;

            //    if (mats[i].Effect != null)
            //    {
            //        hash = mats[i].Effect.GetHashCode();

            //        if (!batchTable.TryGetValue(hash, out matTable))
            //        {
            //            matTable = new Dictionary<int, List<Pair<ISceneObject, int>>>();
            //            batchTable.Add(matTable);
            //        }
            //    }
            //    else
            //    {
            //        matTable = matTableNE;
            //    }


            //    hash = mats[i].GetHashCode();
            //    List<Pair<ISceneObject, int>> objs;
            //    if (!matTable.TryGetValue(hash, out objs))
            //    {
            //        objs = new List<Pair<ISceneObject, int>>();
            //        matTable.Add(objs);
            //    }

            //    objs.Add(new Pair<ISceneObject, int>(obj, i));
            //}
        }


        public virtual void Update(float dt)
        {
            for (int i = 0; i < cameraList.Count; i++)
            {
                cameraList[i].Update();
            }

            atmos.Update(dt);
        }



        #region IDisposable 成员

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                axis.Dispose();
                visiableObjects.Clear();

                Dictionary<int, FastList<RenderOperation>>.ValueCollection vals = batchTable.Values;
                foreach (FastList<RenderOperation> opList in vals)
                {
                    opList.Clear();
                }
                batchTable.Clear();

                Dictionary<int, Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>>.ValueCollection instTableVals = instanceTable.Values;
                foreach (Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>> matTbl in instTableVals)
                {
                    Dictionary<MeshMaterial, Dictionary<GeomentryData, FastList<RenderOperation>>>.ValueCollection matTableVals = matTbl.Values;
                    foreach (Dictionary<GeomentryData, FastList<RenderOperation>> geoTable in matTableVals)
                    {
                        Dictionary<GeomentryData, FastList<RenderOperation>>.ValueCollection geoTableVals = geoTable.Values;
                        foreach (FastList<RenderOperation> opList in geoTableVals)
                        {
                            opList.Clear();
                        }
                        geoTable.Clear();
                    }
                    matTbl.Clear();
                }
                effects.Clear();
            }
            batchTable = null;
            instanceTable = null;
            effects = null;
            visiableObjects = null;
            axis = null;
            cameraList = null;
        }

        public void Dispose()
        {
            if (!Disposed)
            {
                Dispose(true);
                Disposed = true;
            }
            else
            {
                throw new ObjectDisposedException(ToString());
            }
        }

        #endregion
    }

    public unsafe class SceneManager : SceneManagerBase
    {
        protected SceneNode rootNode;

        List<SceneNode> sceneNodes;


        public SceneManager(Device device, Atmosphere atmos)
            : base(device, atmos)
        {
        }

        protected override void BuildSceneManager()
        {
            sceneNodes = new List<SceneNode>();
            // the default sceMgr is simply made up of one node
            SceneNode node = new SceneNode(this, null);
            sceneNodes.Add(node);

        }

        public override void AddObjectToScene(SceneObject obj)
        {
            if (sceneNodes.Count > 0)
            {
                sceneNodes[0].AttchedObjects.Add(obj);
            }
        }
        public override void RemoveObjectFromScene(SceneObject obj)
        {
            if (sceneNodes.Count > 0)
            {
                sceneNodes[0].RemoveObject(obj); 
            }
        }

        public override void FindObjects(FastList<SceneObject> objects, Frustum frus)
        {
            for (int i = 0; i < sceneNodes.Count; i++)
            {
                SceneNode node = sceneNodes[i];
                for (int j = 0; j < node.AttchedObjects.Count; j++)
                {
                    SceneObject curObj =node.AttchedObjects.Elements[j];
                    if (frus.IsSphereIn(ref  curObj.BoundingSphere.Center, curObj.BoundingSphere.Radius))
                    {
                        objects.Add(curObj);
                    }
                }
            }
        }
        public override SceneObject FindObject(Ray ray)
        {
            SceneObject result = null;
            float nearest = float.MaxValue;
            for (int i = 0; i < sceneNodes.Count; i++)
            {
                SceneNode node = sceneNodes[i];
                for (int j = 0; j < node.AttchedObjects.Count; j++)
                {
                    SceneObject curObj = node.AttchedObjects.Elements[j];
                    if (MathEx.BoundingSphereIntersects(ref curObj.BoundingSphere, ref ray))
                    {
                        float dist = MathEx.DistanceSquared(ref curObj.BoundingSphere.Center, ref ray.Position);
                        if (dist < nearest)
                        {
                            nearest = dist;
                            result = curObj;
                        }
                    }
                }
            }
            return result;
        }

        protected override void PrepareVisibleObjects(Camera camera)
        {
            visiableObjects.FastClear();
            for (int i = 0; i < sceneNodes.Count; i++)
            {
                FastList<SceneObject> objs = sceneNodes[i].AttchedObjects;
                for (int j = 0; j < objs.Count; j++)
                {
                    if (objs.Elements[j].HasSubObjects)
                    {
                        objs.Elements[j].PrepareVisibleObjects(camera);
                    }
                    //if (objs[j].RequireMultiBatches)
                    //{
                    //    multiBatchBuffer.Enqueue(objs[j]);
                    //    multiBatchBuffer2.Enqueue(objs[j]);
                    //}

                    AddObject(objs.Elements[j]);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                sceneNodes.Clear();
            }
            sceneNodes = null;
        }
    }
}
