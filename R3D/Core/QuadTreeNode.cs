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

namespace R3D.Core
{
    public enum CornerType : int
    {
        TopLeft = 0,
        TopRight,
        BottomLeft,
        BottomRight
    }
    public class QuadTreeNode
    {
        const int ChildrenCount = 4;

        static int minQuadLen = 1;

        QuadTreeNode[] children;

        /// <summary>
        ///  四叉树保存的第一个值
        /// </summary>
        protected int centre;

        /// <summary>
        ///  四叉树保存的第二个值
        ///    TopLeft(TL)      TopRight(TR)
        ///              0------1
        ///              |      |
        ///              |      |
        ///              2------3
        /// BottomLeft(BL)      BottomRight(BR)
        /// </summary>
        protected int[] corner;

        //bool SubDevide()
        //{
        //    int nTopEdgeCenter;
        //    int nBottomEdgeCenter;
        //    int nLeftEdgeCenter;
        //    int nRightEdgeCenter;
        //    int nCentralPoint;

        //    // 上边中间
        //    nTopEdgeCenter = (corner[(int)CornerType.TopLeft] + corner[(int)CornerType.TopRight]) / 2;
        //    // 下边中间 
        //    nBottomEdgeCenter = (corner[(int)CornerType.BottomLeft] + corner[(int)CornerType.BottomRight]) / 2;
        //    // 左边中间
        //    nLeftEdgeCenter = (corner[(int)CornerType.TopLeft] + corner[(int)CornerType.BottomLeft]) / 2;
        //    // 右边中间
        //    nRightEdgeCenter = (corner[(int)CornerType.TopRight] + corner[(int)CornerType.BottomRight]) / 2;
        //    // 正中央
        //    nCentralPoint = (corner[(int)CornerType.TopLeft] + corner[(int)CornerType.TopRight] +
        //                            corner[(int)CornerType.BottomLeft] + corner[(int)CornerType.BottomRight]) / 4;

        //    // 不能进行再分割了？那么SubDivide()结束
        //    if ((corner[(int)CornerType.TopRight] - corner[(int)CornerType.TopLeft]) <= 1)
        //        return false;

        //    // 添加4个子节点
        //    children = new QuadTreeNode[4];
        //    children[(int)CornerType.TopLeft] =
        //        new QuadTreeNode(corner[(int)CornerType.TopLeft], nTopEdgeCenter, nLeftEdgeCenter, nCentralPoint);// _AddChild();
        //    children[(int)CornerType.TopRight] =
        //        new QuadTreeNode(nTopEdgeCenter, corner[(int)CornerType.TopRight], nCentralPoint, nRightEdgeCenter);
        //    children[(int)CornerType.BottomLeft] =
        //        new QuadTreeNode(nLeftEdgeCenter, nCentralPoint, corner[(int)CornerType.BottomLeft], nBottomEdgeCenter);
        //    children[(int)CornerType.BottomRight] =
        //        new QuadTreeNode(nCentralPoint, nRightEdgeCenter, nBottomEdgeCenter, corner[(int)CornerType.BottomRight]);
        //    return true;
        //}
        //void Build()
        //{
        //    if (SubDevide())
        //    {
        //        children[(int)CornerType.TopLeft].Build();
        //        children[(int)CornerType.TopRight].Build();
        //        children[(int)CornerType.BottomLeft].Build();
        //        children[(int)CornerType.BottomRight].Build();
        //    }
        //}

        /// <summary>
        /// 子节点构造函数
        /// </summary>
        /// <param name="nCornerTL"></param>
        /// <param name="nCornerTR"></param>
        /// <param name="nCornerBL"></param>
        /// <param name="nCornerBR"></param>
        private QuadTreeNode(int nCornerTL, int nCornerTR, int nCornerBL, int nCornerBR)
        {
            corner = new int[4];

            corner[(int)CornerType.TopLeft] = nCornerTL;
            corner[(int)CornerType.TopRight] = nCornerTR;
            corner[(int)CornerType.BottomLeft] = nCornerBL;
            corner[(int)CornerType.BottomRight] = nCornerBR;
            centre = (corner[(int)CornerType.TopLeft] + corner[(int)CornerType.TopRight] +
                      corner[(int)CornerType.BottomLeft] + corner[(int)CornerType.BottomRight]) / 4;

            // 上边中间
            int nTopEdgeCenter = (corner[(int)CornerType.TopLeft] + corner[(int)CornerType.TopRight]) / 2;
            // 下边中间 
            int nBottomEdgeCenter = (corner[(int)CornerType.BottomLeft] + corner[(int)CornerType.BottomRight]) / 2;
            // 左边中间
            int nLeftEdgeCenter = (corner[(int)CornerType.TopLeft] + corner[(int)CornerType.BottomLeft]) / 2;
            // 右边中间
            int nRightEdgeCenter = (corner[(int)CornerType.TopRight] + corner[(int)CornerType.BottomRight]) / 2;
            // 正中央
            int nCentralPoint = (corner[(int)CornerType.TopLeft] + corner[(int)CornerType.TopRight] +
                                    corner[(int)CornerType.BottomLeft] + corner[(int)CornerType.BottomRight]) / 4;

            // 能进行再分割？
            if ((corner[(int)CornerType.TopRight] - corner[(int)CornerType.TopLeft]) > minQuadLen)
            {
                // 添加4个子节点
                children = new QuadTreeNode[4];
                children[(int)CornerType.TopLeft] =
                    new QuadTreeNode(corner[(int)CornerType.TopLeft], nTopEdgeCenter, nLeftEdgeCenter, nCentralPoint);// _AddChild();
                children[(int)CornerType.TopRight] =
                    new QuadTreeNode(nTopEdgeCenter, corner[(int)CornerType.TopRight], nCentralPoint, nRightEdgeCenter);
                children[(int)CornerType.BottomLeft] =
                    new QuadTreeNode(nLeftEdgeCenter, nCentralPoint, corner[(int)CornerType.BottomLeft], nBottomEdgeCenter);
                children[(int)CornerType.BottomRight] =
                    new QuadTreeNode(nCentralPoint, nRightEdgeCenter, nBottomEdgeCenter, corner[(int)CornerType.BottomRight]);
            }
        }
        /// <summary>
        /// 根节点构造函数
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        public QuadTreeNode(int cx, int cy)
            : this(Helper(1), cx - 1, cx * (cy - 1), cx * cy - 1)
        { }

        static int Helper(int ss)
        {
            minQuadLen = ss;
            return 0;
        }

        /// <summary>
        /// 根节点构造函数
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        public QuadTreeNode(int cx, int cy, int sqSize)
            : this(Helper(sqSize), cx - 1, cx * (cy - 1), cx * cy - 1)
        { }

        public bool HasChildren
        {
            get { return children != null; }
        }
        public QuadTreeNode this[CornerType cor]
        {
            get { return children[(int)cor]; }
        }
        public int GetData(CornerType cor)
        {
            return corner[(int)cor];
        }
    }
}
