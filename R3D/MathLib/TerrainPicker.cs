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
using R3D.Logic;
using SlimDX;

namespace R3D.MathLib
{
    public class TerrainPicker
    {
        FastList<TerrainTreeNodeData> fastTestBuffer;

        Ray pickRay;

        public TerrainPicker(Terrain terr, CellInfo[][] cellInfo)
        {
            ParentTerrain = terr;
            CellInfo = cellInfo;

            fastTestBuffer = new FastList<TerrainTreeNodeData>(TerrainTreeNode.MinGroup);
        }

        public CellInfo[][] CellInfo
        {
            get;
            protected set;
        }
        public Terrain ParentTerrain
        {
            get;
            protected set;
        }


        public void GetResultCell(out int x, out int y)
        {
            float nearest = float.MaxValue;
            TerrainTreeNodeData bestObj = null;

            for (int i = 0; i < fastTestBuffer.Count; i++)
            {
                if (fastTestBuffer.Elements[i].lodHeight < nearest)
                {
                    bestObj = fastTestBuffer.Elements[i];
                    nearest = fastTestBuffer.Elements[i].lodHeight;
                }
            }

            if (bestObj != null)
            {
                x = bestObj.x;
                y = bestObj.y;
            }
            else
            {
                x = -1;
                y = -1;
            }
        }


        public void SetPickRay(ref Ray ray)
        {
            pickRay = ray;
        }
        public void GetPickRay(out Ray ray)
        {
            ray = pickRay;
        }

        public Ray PickRay
        {
            get { return pickRay; }
            set { pickRay = value; }
        }

        public void Reset()
        {
            fastTestBuffer.FastClear();
        }


        public void Intersect(TerrainTreeNode node)
        {
            TerrainTreeNodeData[] data = node.GetData();

            //Vector3 center = new Vector3(node.bounding.Center.X - pickRay.Position.X, node.bounding.Center.Y - pickRay.Position.Y, node.bounding.Center.Z - pickRay.Position.Z);


            float cx = node.bounding.Center.X - pickRay.Position.X;
            float cy = node.bounding.Center.Y - pickRay.Position.Y;
            float cz = node.bounding.Center.Z - pickRay.Position.Z;

            Vector3 n = pickRay.Direction;
            float dl1 = n.X * cx + n.Y * cy + n.Z * cz;

            n.X = cx - n.X * dl1;
            n.Y = cy - n.Y * dl1;
            n.Z = cz - n.Z * dl1;

            n.Normalize();

            dl1 = Math.Abs(-(n.X * cx + n.Y * cy + n.Z * cz));

            if (dl1 <= node.bounding.Radius)
            //if (MathEx.IntersectBoundingBall(ref center, node.bounding.Radius, ref pickRay))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    int x = data[i].x;
                    int y = data[i].y;

                    //center = new Vector3(x * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f, CellInfo[x][y].HeightInfo.centre, y * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f);
                    cx = x * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f;
                    cz = y * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f;
                    cy = CellInfo[x][y].HeightInfo.centre;

                    cx -= pickRay.Position.X;
                    cy -= pickRay.Position.Y;
                    cz -= pickRay.Position.Z;

                    n = pickRay.Direction;
                    dl1 = n.X * cx + n.Y * cy + n.Z * cz;

                    n.X = cx - n.X * dl1;
                    n.Y = cy - n.Y * dl1;
                    n.Z = cz - n.Z * dl1;
                    n.Normalize();
                    dl1 = Math.Abs(-(n.X * cx + n.Y * cy + n.Z * cz));

                    if (dl1 <= Terrain.HorizontalUnit * (MathEx.Root2 * 0.5f))
                    //if (MathEx.IntersectBoundingBall(ref center, Terrain.HorizontalUnit * (MathEx.Root2 * 0.5f), ref pickRay))
                    {
                        //CellInfo[x][y].HeightInfo.Normal;
                        if (CheckCell(x, y))
                        {
                            fastTestBuffer.Add(data[i]);
                        }
                    }
                }
            }
        }

        bool CheckCell(int x, int y)
        {

            Vector3 pN = CellInfo[x][y].HeightInfo.Normal;
            pN.X = -pN.X;
            pN.Y = -pN.Y;
            pN.Z = -pN.Z;

            //Vector3 va;
            //va.X = x * Terrain.HorizontalUnit - pickRay.Position.X;
            //va.Z = y * Terrain.HorizontalUnit - pickRay.Position.Z;
            //va.Y = CellInfo[x][y].HeightInfo.top - pickRay.Position.Y;

            float top_x = x * Terrain.HorizontalUnit;
            float top_z = y * Terrain.HorizontalUnit;

            float bottom_x = (x + 1) * Terrain.HorizontalUnit;
            float bottom_z = (y + 1) * Terrain.HorizontalUnit;


            float nDist =
                (top_x - pickRay.Position.X) * pN.X +
                (CellInfo[x][y].HeightInfo.top - pickRay.Position.Y) * pN.Y +
                (y * Terrain.HorizontalUnit - pickRay.Position.Z) * pN.Z;

            float cosine = pN.X * pickRay.Direction.X + pN.Y * pickRay.Direction.Y + pN.Z * pickRay.Direction.Z;

            float len = nDist / cosine;

            //Vector3 insectPt = pickRay.Position;
            //insectPt.X += len * pickRay.Direction.X;
            //insectPt.Y += len * pickRay.Direction.Y;
            //insectPt.Z += len * pickRay.Direction.Z;
            float insectX = pickRay.Position.X + len * pickRay.Direction.X;
            float insectZ = pickRay.Position.Z + len * pickRay.Direction.Z;

            return insectX >= top_x && insectX <= bottom_x && insectZ >= top_z && insectZ <= bottom_z;
        }
    }
}
