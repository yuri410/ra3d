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
using SlimDX.Direct3D9;
using SlimDX;

namespace R3D.GraphicsEngine
{
    public struct RenderOperation
    {
        public Matrix Transformation;

        public MeshMaterial Material;

        public GeomentryData Geomentry;        
    }

    public class GeomentryData
    {
        //public delegate void RenderedHandler(RenderOperation op);

        IRenderable sender;
        //public Matrix Transformation;


        //RenderedHandler renderHandler;

        public IRenderable Sender
        {
            get { return sender; }
        }
        public VertexFormat Format
        {
            get;
            set;
        }
        public int VertexSize
        {
            get;
            set;
        }
        public VertexDeclaration VertexDeclaration
        {
            get;
            set;
        }



        public VertexBuffer VertexBuffer
        {
            get;
            set;
        }
        public IndexBuffer IndexBuffer
        {
            get;
            set;
        }
        public bool UseIndices
        {
            get { return IndexBuffer != null; }
        }
       
        public PrimitiveType PrimitiveType
        {
            get;
            set;
        }
        public int PrimCount
        {
            get;
            set;
        }
        public int VertexCount
        {
            get;
            set;
        }

        public GeomentryData(IRenderable obj)
        {
            sender = obj;
        }

        //public bool IsLastOperation
        //{
        //    get;
        //    set;
        //}

        //public event RenderedHandler Rendered
        //{
        //    add { renderHandler += value; }
        //    remove { renderHandler -= value; }
        //}

        //public void OnRendered()
        //{
        //    if (renderHandler != null)
        //        renderHandler(this);
        //}

        //~RenderOperation()
        //{
        //    renderHandler = null;
        //}
    }
}
