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
using R3D.Collections;
using R3D.GraphicsEngine;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Core
{
    public class OctreeSceneManager : SceneManagerBase
    {
        OctreeSceneNode octRootNode;

        Queue<OctreeSceneNode> queue;

        internal OctreeBox range;

        public OctreeSceneManager(Device dev, Atmosphere atmos, OctreeBox range, float minBVSize)
            : base(dev, atmos)
        {
            this.range = range;
            MinimumBVSize = minBVSize;

            queue = new Queue<OctreeSceneNode>();
            octRootNode.BoundingVolume = range;
            octRootNode.BoundingVolume.GetBoundingSphere(out octRootNode.BoundingSphere);
        }

        public float MinimumBVSize
        {
            get;
            protected set;
        }

        public override void AddObjectToScene(SceneObject obj)
        {
            octRootNode.AddObject(obj);
            //base.AddObjectToScene(obj);
        }
        public override void RemoveObjectFromScene(SceneObject obj)
        {
            octRootNode.RemoveObject(obj);
        }

        protected override void BuildSceneManager()
        {        
            octRootNode = new OctreeSceneNode(this, null);
        }

        //public override SceneNode FindNode(SceneObject obj)
        //{
        //    return base.FindNode(obj);
        //}
        public override void FindObjects(FastList<SceneObject> objects, Frustum frus)
        {
            if (queue.Count == 0)
            {
                queue.Enqueue(octRootNode);
                while (queue.Count > 0)
                {
                    OctreeSceneNode node = queue.Dequeue();

                    // if the node does't intersect the frustum we don't give a damn
                    if (frus.IsSphereIn(ref node.BoundingSphere.Center, node.BoundingSphere.Radius))
                    {
                        for (int i = 0; i < 2; i++)
                            for (int j = 0; j < 2; j++)
                                for (int k = 0; k < 2; k++)
                                {
                                    if (node[i, j, k] != null)
                                    {
                                        queue.Enqueue(node[i, j, k]);
                                    }
                                }

                        for (int i = 0; i < node.AttchedObjects.Count; i++)
                        {
                            //if (frus.IsSphereIn(ref node.BoundingSphere.Center, node.BoundingSphere.Radius))
                            SceneObject curObj = node.AttchedObjects.Elements[i];

                            if (frus.IsSphereIn(ref curObj.BoundingSphere.Center, curObj.BoundingSphere.Radius))
                            {
                                objects.Add(curObj);
                            }
                        }
                    }

                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override SceneObject FindObject(Ray ray)
        {
            SceneObject result = null;
            float nearest = float.MaxValue;
            if (queue.Count == 0)
            {
                queue.Enqueue(octRootNode);
                while (queue.Count > 0)
                {
                    OctreeSceneNode node = queue.Dequeue();

                    // if the node does't intersect the frustum we don't give a damn
                    if (MathEx.BoundingSphereIntersects(ref  node.BoundingSphere, ref ray))
                    {
                        for (int i = 0; i < 2; i++)
                            for (int j = 0; j < 2; j++)
                                for (int k = 0; k < 2; k++)
                                {
                                    if (node[i, j, k] != null)
                                    {
                                        queue.Enqueue(node[i, j, k]);
                                    }
                                }

                        for (int i = 0; i < node.AttchedObjects.Count; i++)
                        {
                            SceneObject curObj = node.AttchedObjects.Elements[i];

                            if (curObj.IntersectsSelectionRay(ref ray))// MathEx.BoundingSphereIntersects(ref curObj.BoundingSphere, ref ray))
                            {
                                float dist = MathEx.DistanceSquared(ref  curObj.BoundingSphere.Center, ref ray.Position);
                                if (dist < nearest)
                                {
                                    nearest = dist;
                                    result = curObj;
                                }
                            }
                        }
                    }

                }
            }
            else
            {
                throw new InvalidOperationException();
            }
            return result;
        }
        protected override void PrepareVisibleObjects(Camera camera)
        {
            visiableObjects.FastClear();

            Frustum frus = camera.Frustum;

            // do a BFS pass here

            queue.Enqueue(octRootNode);

            while (queue.Count > 0)
            {
                OctreeSceneNode node = queue.Dequeue();

                // if the node does't intersect the frustum we don't give a damn
                if (frus.IsSphereIn(ref node.BoundingSphere.Center, node.BoundingSphere.Radius))
                {
                    for (int i = 0; i < 2; i++)
                        for (int j = 0; j < 2; j++)
                            for (int k = 0; k < 2; k++)
                            {
                                if (node[i, j, k] != null)
                                {
                                    queue.Enqueue(node[i, j, k]);
                                }
                            }
                    FastList<SceneObject> objs = node.AttchedObjects;
                    for (int i = 0; i < objs.Count; i++)
                    {
                        if (objs.Elements[i].HasSubObjects)
                        {
                            objs.Elements[i].PrepareVisibleObjects(camera);
                        }
                        AddObject(objs.Elements[i]);
                    }
                }

            }

            //base.PrepareVisibleObjects(camera);
        }
        
        public override void Update(float dt)
        {
            base.Update(dt);

            octRootNode.Update();
        }
    }
}
