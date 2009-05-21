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
using System.IO;
using SlimDX;
using R3D.MathLib;

namespace R3D.GraphicsEngine
{
    public unsafe class IndexStream16
    {
        int length;
        internal int position;

        internal ushort* dest;

        public IndexStream16(ushort* ptr, int length)
        {
            dest = ptr;
            this.length = length;
        }

        public int Length
        {
            get { return length; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public void Write(ushort index)
        {
            dest[position++] = index;
        }
    }
    public unsafe class IndexStream32
    {
        int length;
        int position;
        uint* dest;

        public IndexStream32(uint* ptr, int length)
        {
            dest = ptr;
            this.length = length;
        }

        public int Length
        {
            get { return length; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public void Write(uint index)
        {
            dest[position++] = index;
        }
    }

    /// <summary>    
    /// 地形树节点上的数据，对应一个cell。
    /// </summary>
    public class TerrainTreeNodeData
    {
        public const int NoIndex = -1;

        public int x;
        public int y;

        /// <summary>
        /// 指定节点的纹理包，渲染时规划用
        /// </summary>
        public int texturePack;

        //public int groupIdx;

        /// <summary>
        /// cell在treeData（储存所有TerrainTreeNodeData的数组）中的索引
        /// </summary>
        public int cellIndex;

        /// <summary>
        /// 主cell第一个顶点的索引
        /// </summary>
        public int vbIndex;

        /// <summary>
        /// 第二cell第一个顶点的索引
        /// </summary>
        public int vbSecIndex = NoIndex;

        public bool ignoreLod;

        public float lodFactor;
        public float lodHeight;

        public float lodFactor2;
        public float lodHeight2;
        public int varision;
        public GameModel model;

        public bool visible;
    }

    public delegate void GroupingCallBack(TerrainTreeNodeData[] group);

    public class TerrainTreeNode
    {
        public const int MinGroup = 32;
        const int IBGroup = 13107;

        int childrenCount;
        internal BoundingSphere bounding;
        TerrainTreeNode[] children;

        /// <summary>
        /// 节点以及子节点对应的Cells
        /// </summary>
        TerrainTreeNodeData[] data;

        public TerrainTreeNode(TerrainTreeNodeData[] remain, int left, int top, int width, int height, GroupingCallBack gpcb)
        {
            data = remain;

            // 检查能否再分
            if (data.Length > MinGroup)
            {
                int hw = width >> 1;
                int hh = height >> 1;

                int averCount = remain.Length >> 2;

                List<TerrainTreeNodeData> tl = new List<TerrainTreeNodeData>(averCount);
                List<TerrainTreeNodeData> tr = new List<TerrainTreeNodeData>(averCount);
                List<TerrainTreeNodeData> bl = new List<TerrainTreeNodeData>(averCount);
                List<TerrainTreeNodeData> br = new List<TerrainTreeNodeData>(averCount);

                for (int i = 0; i < remain.Length; i++)
                {
                    if (remain[i].x < hw + left && remain[i].y < hh + top)
                    {
                        tl.Add(remain[i]);
                    }
                    else if (remain[i].x < hw + left)
                    {
                        bl.Add(remain[i]);
                    }
                    else if (remain[i].y < hh + top)
                    {
                        tr.Add(remain[i]);
                    }
                    else
                    {
                        br.Add(remain[i]);
                    }
                }

                // 防止死递归：如果分不开就停止构建子节点
                if ((tl.Count > 0 ? 1 : 0) + (bl.Count > 0 ? 1 : 0) + (tr.Count > 0 ? 1 : 0) + (br.Count > 0 ? 1 : 0) > 1)
                {
                    children = new TerrainTreeNode[4];

                    if (tl.Count > 0)
                    {
                        TerrainTreeNodeData[] chdata = tl.ToArray();
                        if (gpcb != null && tl.Count < IBGroup && remain.Length >= IBGroup)
                        {
                            gpcb(chdata);
                        }
                        children[0] = new TerrainTreeNode(chdata, left, top, hw, hh, gpcb);
                        childrenCount++;
                    }
                    if (bl.Count > 0)
                    {
                        TerrainTreeNodeData[] chdata = bl.ToArray();
                        if (gpcb != null && bl.Count < IBGroup && remain.Length >= IBGroup)
                        {
                            gpcb(chdata);
                        }
                        children[1] = new TerrainTreeNode(chdata, left, top + hh, hw, hh, gpcb);
                        childrenCount++;
                    }
                    if (tr.Count > 0)
                    {
                        TerrainTreeNodeData[] chdata = tr.ToArray();
                        if (gpcb != null && tr.Count < IBGroup && remain.Length >= IBGroup)
                        {
                            gpcb(chdata);
                        }
                        children[2] = new TerrainTreeNode(chdata, left + hw, top, hw, hh, gpcb);
                        childrenCount++;
                    }
                    if (br.Count > 0)
                    {
                        TerrainTreeNodeData[] chdata = br.ToArray();
                        if (gpcb != null && br.Count < IBGroup && remain.Length >= IBGroup)
                        {
                            gpcb(chdata);
                        }
                        children[3] = new TerrainTreeNode(chdata, left + hw, top + hh, hw, hh, gpcb);
                        childrenCount++;
                    }
                }
            }
        }

        public TerrainTreeNode this[int index]
        {
            get { return children[index]; }
        }

        public TerrainTreeNodeData[] GetData()
        {
            return data;
        }

        public int ChildrenCount
        {
            get { return childrenCount; }
        }

        public BoundingSphere Bounding
        {
            get { return bounding; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vtxData">任何一个VB的就行</param>
        public unsafe void CalculateBoudingVolume(TerrainVertex*[] vtxData)
        {
            // 后序遍历
            if (childrenCount == 0)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    int group = data[i].texturePack + 1;
                    Vector3 cellCentre = vtxData[group][data[i].vbIndex + 1].pos;
                    Vector3.Add(ref cellCentre, ref vtxData[group][data[i].vbIndex + 2].pos, out cellCentre);
                    Vector3.Add(ref cellCentre, ref vtxData[group][data[i].vbIndex].pos, out cellCentre);
                    Vector3.Add(ref cellCentre, ref vtxData[group][data[i].vbIndex + 3].pos, out cellCentre);

                    cellCentre.X *= 0.25f;
                    cellCentre.Y *= 0.25f;
                    cellCentre.Z *= 0.25f;

                    bounding.Center.X += cellCentre.X;
                    bounding.Center.Y += cellCentre.Y;
                    bounding.Center.Z += cellCentre.Z;
                }

                float invl = 1f / data.Length;
                bounding.Center.X *= invl;
                bounding.Center.Y *= invl;
                bounding.Center.Z *= invl;

                bounding.Radius = float.MinValue;

                for (int i = 0; i < data.Length; i++)
                {
                    int group = data[i].texturePack + 1;
                    float distS = MathEx.DistanceSquared(ref bounding.Center, ref vtxData[group][data[i].vbIndex + 1].pos);
                    if (distS > bounding.Radius)
                        bounding.Radius = distS;
                    distS = MathEx.DistanceSquared(ref bounding.Center, ref vtxData[group][data[i].vbIndex + 2].pos);
                    if (distS > bounding.Radius)
                        bounding.Radius = distS;
                    distS = MathEx.DistanceSquared(ref bounding.Center, ref vtxData[group][data[i].vbIndex].pos);
                    if (distS > bounding.Radius)
                        bounding.Radius = distS;
                    distS = MathEx.DistanceSquared(ref bounding.Center, ref vtxData[group][data[i].vbIndex + 3].pos);
                    if (distS > bounding.Radius)
                        bounding.Radius = distS;
                }

                bounding.Radius = (float)Math.Sqrt(bounding.Radius);

            }
            else
            {

                for (int i = 0; i < 4; i++)
                {
                    if (children[i] != null)
                    {
                        children[i].CalculateBoudingVolume(vtxData);

                        Vector3.Add(ref bounding.Center, ref children[i].bounding.Center, out bounding.Center);
                    }
                }

                float invC = 1f / childrenCount;
                bounding.Center.X *= invC;
                bounding.Center.Y *= invC;
                bounding.Center.Z *= invC;

                bounding.Radius = float.MinValue;
                for (int i = 0; i < 4; i++)
                {
                    if (children[i] != null)
                    {
                        float distS = MathEx.Distance(ref bounding.Center, ref children[i].bounding.Center) + children[i].bounding.Radius;
                        if (distS > bounding.Radius)
                            bounding.Radius = distS;
                    }
                }

            }
        }

    }
}
