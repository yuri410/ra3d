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
using System.Drawing;

namespace R3D.Core
{
    public enum NodeUseState : byte
    {
        Unused,
        PartlyUsed,
        FullUsed
    }

    public class ImageQuadTreeNode
    {
        NodeUseState isUsed;

        public NodeUseState IsUsed
        {
            get { return isUsed; }
            set { isUsed = value; }
        }
        const int ChildrenCount = 4;

        static int minQuadLen = 1;

        ImageQuadTreeNode[] children;
        ImageQuadTreeNode parent;

        ushort left;
        ushort top;
        ushort right;
        ushort bottom;

        void UpdateParent()
        {
            bool and = true;
            bool or = false;

            for (int i = 0; i < 4; i++)
            {
                and &= (children[i].IsUsed == NodeUseState.FullUsed);
                or |= (children[i].IsUsed == NodeUseState.FullUsed);
            }

            if (and)
            {
                isUsed = NodeUseState.FullUsed;
            }
            else if (or)
            {
                isUsed = NodeUseState.PartlyUsed;
            }
            else
            {
                isUsed = NodeUseState.Unused;
            }


            if (parent != null)
            {
                parent.UpdateParent();
            }
        }

        void FullUse()
        {
            isUsed = NodeUseState.FullUsed;
            if (HasChildren)
            {
                children[0].FullUse();
                children[1].FullUse();
                children[2].FullUse();
                children[3].FullUse();
            }
        }

        public void Reset()
        {
            isUsed = NodeUseState.Unused;
            if (HasChildren)
            {
                children[0].Reset();
                children[1].Reset();
                children[2].Reset();
                children[3].Reset();
            }
        }

        public bool Find(int w, int h, out Point pos)
        {                        
            if (isUsed == NodeUseState.Unused &&
                ((((Width >> 1) < w || (Height >> 1) < h) && (w < Width && h < Height)) || !HasChildren))
            {
                FullUse();
                if (parent != null)
                    parent.UpdateParent();

                pos = new Point(Left, Top);
                return true;
                //found = true;
            }
            if (HasChildren)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (children[i].isUsed != NodeUseState.FullUsed &&
                        children[i].Width >= w &&
                        children[i].Height >= h)
                    {
                        if (children[i].Find(w, h, out pos))
                            return true;
                    }
                }
                pos = Point.Empty;
                return false;
            }
            // not found

            pos = Point.Empty;
            return false;


        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="w"></param>
        ///// <param name="h"></param>
        ///// <returns>节点是否能放下w*h</returns>
        //public bool Find(int w, int h, out Point pos)
        //{
        //    if (isUsed == NodeUseState.FullUsed || this.Width < w || this.Height < h)
        //    {
        //        pos = Point.Empty;
        //        return false;
        //    }

        //    // 能否使用更深的节点（更小的图块）
        //    bool deeper = false;
        //    if (HasChildren)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (!deeper)
        //            {
        //                deeper |= children[0].Find(w, h, out pos);
        //            }
        //        }

        //    }
        //    else
        //        deeper = false;

        //    if (!deeper)
        //    {
        //        pos = new Point(Left,Top );
        //        FullUse();
        //        if (parent != null)
        //            parent.UpdateParent();
        //        return true;
        //    }
        //    pos = Point.Empty;
        //    return false;
        //}

        /// <summary>
        /// 子节点构造函数
        /// </summary>
        private ImageQuadTreeNode(ImageQuadTreeNode par, ushort left, ushort top, ushort right, ushort bottom)
        {
            this.parent = par;
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;

            ushort centreX = (ushort)((left + right) / 2);
            ushort centreY = (ushort)((top + bottom) / 2);

            // 能进行再分割？
            if ((right - left) > minQuadLen && (bottom - top) > minQuadLen)
            {
                // 添加4个子节点
                children = new ImageQuadTreeNode[4];
                children[(int)CornerType.TopLeft] =
                    new ImageQuadTreeNode(this, left, top, centreX, centreY);// _AddChild();
                children[(int)CornerType.TopRight] =
                    new ImageQuadTreeNode(this, centreX, top, right, centreY);
                children[(int)CornerType.BottomLeft] =
                    new ImageQuadTreeNode(this, left, centreY, centreX, bottom);
                children[(int)CornerType.BottomRight] =
                    new ImageQuadTreeNode(this, centreX, centreY, right, bottom);
            }
        }


        static ushort Helper(int ss)
        {
            minQuadLen = ss;
            return 0;
        }

        /// <summary>
        /// 根节点构造函数
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        public ImageQuadTreeNode(ushort cx, ushort cy, int sqSize)
            : this(null, Helper(sqSize), 0, cx, cy)
        { }

        public int Width
        {
            get { return right - left; }
        }
        public int Height
        {
            get { return bottom - top; }
        }

        public int Top
        {
            get { return top; }
        }
        public int Left
        {
            get { return left; }
        }

        public bool HasChildren
        {
            get { return children != null; }
        }
        public ImageQuadTreeNode this[CornerType cor]
        {
            get { return children[(int)cor]; }
        }
    }
}
