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

namespace R3D.Core
{
    public abstract class SceneNodeBase
    {
        protected SceneNodeBase()
        {
            AttchedObjects = new FastList<SceneObject>(8);
        }

        //public abstract void AddChild(SceneNode ch);
        public abstract void AddObject(SceneObject obj);
        public abstract void RemoveObject(SceneObject obj);
        
        public FastList<SceneObject> AttchedObjects
        {
            get;
            protected set;
        }

        public virtual void Update() { }
        protected abstract void Eject(SceneObject sceneObj);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="NodeType"></typeparam>
    /// <remarks>
    /// 几何意义上的节点
    /// </remarks>
    public class SceneNode : SceneNodeBase
    {
        public SceneManager Manager
        {
            get;
            private set;
        }

        protected List<SceneNode> children;
        protected SceneNode parent;

        //bool requiresUpdate;


        public SceneNode(SceneManager mgr, SceneNode parent)
        {
            children = new List<SceneNode>(8);
           
            Manager = mgr;
            this.parent = parent;
        }

        //public override void AddChild(SceneNode ch)
        //{
        //    children.Add(ch);
        //}

        public int ChildrenCount
        {
            get { return children.Count; }
        }
       

        public override void AddObject(SceneObject obj)
        {
            // the default scene node simply add the object to itself.
            AttchedObjects.Add(obj);
        }

        public override void RemoveObject(SceneObject obj)
        {
            AttchedObjects.Remove(obj);
        }
       
        protected override void Eject(SceneObject sceneObj)
        {
            throw new NotSupportedException();
        }

    }
}
