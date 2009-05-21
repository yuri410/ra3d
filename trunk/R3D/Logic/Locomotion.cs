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
using System.Runtime.InteropServices;
using System.Text;
using R3D.AI;
using R3D.ConfigModel;
using R3D.GraphicsEngine;
using R3D.MathLib;
using R3D.UI;
using SlimDX;

namespace R3D.Logic
{
    /// <summary>
    /// 物理运动控制
    /// </summary>
    public abstract class Locomotion
    {
        static readonly string DefaultMoveMA = "MoveMouseAct";
        static readonly string DefaultMoveInvMA = "MoveMouseInvAct";

        protected Techno unit;
        protected BattleField field;

        //public Vector3 Position;

        //public Quaternion orient;

        public float[] TerrainWeight
        {
            get;
            set;
        }

        //public Matrix transformation;

        public bool IsMoving
        {
            get;
            protected set;
        }

        //public virtual PathFinderResult CurrentPath
        //{
        //    get;
        //    set;
        //}

        //public void GetTransformation(out Matrix mat)
        //{
        //    mat = transformation;
        //}

        protected Locomotion(BattleField fld, Techno u)
        {
            field = fld;
            unit = u;
            //pathFinderMgr = fld.PathFinder;
        }


        public virtual string LocomotionMAName
        {
            get { return DefaultMoveMA; }
        }
        public virtual string LocomotionMAInvName
        {
            get { return DefaultMoveInvMA; }
        }

        /// <returns>仍然需要更新true，否则false</returns>
        protected abstract bool updateRotate(float targetYaw, float dt);

        /// <returns>仍然需要更新true，否则false</returns>
        protected abstract bool updateMove(int x, int y, int z, float dt);

        /// <returns>仍然需要更新true，否则false</returns>
        public bool UpdateRotate(float targetYaw, float dt)
        {
            bool res = updateRotate(targetYaw, dt);
            //Update(dt);
            return res;
        }

        /// <returns>仍然需要更新true，否则false</returns>
        public bool UpdateMove(int x, int y, int z, float dt)
        {
            bool res = updateMove(x, y, z, dt);
            //Update(dt);
            return res;
        }

        public abstract void Update(float dt);
    }


    public class DriveLocomotion : Locomotion
    {


        //bool justInCell;

        //Vector3 angularVelocity;

        float cellLerp;
        //float rotationLerp;

        bool isRotating;
        

        float rotationDirection;
        float rotationRadDiff;

        //float curCellYaw;

        public bool JustInCell
        {
            get;
            protected set;
        }


        public DriveLocomotion(BattleField btfld, Techno unit, DriverLocomotionDescription desc)
            : base(btfld, unit)
        {
            //pathFinder = btfld.PathFinder.CreatePathFinder();
            //Quaternion.RotationYawPitchRoll(0, 0, 0, out orient);
            Description = desc;
            rotationDirection = 1;
            JustInCell = true;

        }
        public override void Update(float dt)
        {
            //if (CurrentPath != null)
            //{
            //    if (nextNode > currentNode)
            //    {
            //        unit.RequiresUpdate = true;

            //        int dx = CurrentPath[nextNode].X - CurrentPath[currentNode].X;
            //        int dy = CurrentPath[nextNode].Y - CurrentPath[currentNode].Y;


            //        if (nextNode < CurrentPath.NodeCount - 1)
            //        {
            //            int oriDesc = dx * 3 + dy;
            //            float targetYaw = 0;
            //            switch (oriDesc)
            //            {
            //                case -4:// -1 , -1
            //                    targetYaw = MathEx.Angle2Radian(135);
            //                    break;
            //                case -1://  0, -1
            //                    targetYaw = MathEx.Angle2Radian(90);
            //                    break;
            //                case -3:// -1, 0
            //                    targetYaw = MathEx.Angle2Radian(180);
            //                    break;
            //                case 2:// 1, -1
            //                    targetYaw = MathEx.Angle2Radian(45);
            //                    break;
            //                case -2:// -1, 1
            //                    targetYaw = MathEx.Angle2Radian(225);
            //                    break;
            //                case 3:// 1, 0
            //                    targetYaw = MathEx.Angle2Radian(0);
            //                    break;
            //                case 1: //0, 1
            //                    targetYaw = MathEx.Angle2Radian(270);
            //                    break;
            //                case 4: //1, 1
            //                    targetYaw = MathEx.Angle2Radian(315);
            //                    break;
            //            }

            //            if (targetYaw != curCellYaw)
            //            {
            //                if (!isRotating)
            //                {
            //                    isRotating = true;
            //                    rotationRadDiff = MathEx.RadianDifference(targetYaw, curCellYaw);
            //                    rotationDirection = rotationRadDiff > 0 ? 1 : -1;
            //                }
            //            }
            //            else
            //            {
            //                isRotating = false;
            //            }

            //            IsMoving = true;

            //            if (!isRotating)
            //            {
            //                float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            //                cellLerp += (unit.Type.Speed * dt * 10) / (dist * Terrain.HorizontalUnit);

            //                if (cellLerp >= 1)
            //                {
            //                    cellLerp = 0;
            //                    currentNode = nextNode;
            //                    nextNode++;
            //                }

            //                int cx = CurrentPath[currentNode].X;
            //                int cy = CurrentPath[currentNode].Y;
            //                int nx = CurrentPath[nextNode].X;
            //                int ny = CurrentPath[nextNode].Y;


            //                Vector3 curCellPos = new Vector3(cx * Terrain.HorizontalUnit, field.CellInfo[cx][cy].HeightInfo.centre, cy * Terrain.HorizontalUnit);
            //                Vector3 nxtCellPos = new Vector3(nx * Terrain.HorizontalUnit, field.CellInfo[nx][ny].HeightInfo.centre, ny * Terrain.HorizontalUnit);

            //                Vector3.Lerp(ref curCellPos, ref nxtCellPos, cellLerp, out unit.Position);
            //            }
            //            else
            //            {
            //                //rotationLerp += (unit.Type.ROT * dt * (360f / 255f)) / Math.Abs(targetYaw - curCellYaw);

            //                float incr = unit.Type.ROT * dt * (360f / 256f);
            //                unit.Yaw += rotationDirection * incr;
            //                unit.Yaw %= MathEx.PIf * 2;


            //                if (Math.Abs(unit.Yaw - targetYaw) < incr)
            //                {
            //                    curCellYaw = targetYaw;
            //                    unit.Yaw = targetYaw;
            //                    isRotating = false;
            //                }

            //                //if (rotationLerp >= 1)
            //                //{
            //                //    rotationLerp = 0;
            //                //    curCellYaw = targetYaw;
            //                //    unit.Yaw = targetYaw; 
            //                //    isRotating = false;
            //                //}
            //                //else
            //                //{
            //                //    unit.Yaw = curCellYaw * (1 - rotationLerp) + targetYaw * rotationLerp;
            //                //}
            //            }
            //        }
            //        else
            //        {
            //            currentNode = nextNode;
            //            IsMoving = false;
            //        }
            //    }
            //    //
            //}



            //Vector3 pos = Position;

            //pos.Y += field.CellInfo[unit.X][unit.Y].HeightInfo.centre;

            //unit.BoundingSphere.Center = Position;



            //Matrix rotation;
            Quaternion yawQuat = Quaternion.RotationAxis(new Vector3(0, 1, 0), unit.Yaw);

            Quaternion untOrientaion;
            Quaternion.Multiply(ref yawQuat, ref unit.Orientation, out untOrientaion);

            Matrix.RotationQuaternion(ref untOrientaion, out unit.Transformation);

            //Matrix.Multiply(ref unit.Transformation, ref rotation, out unit.Transformation);
            Matrix trans;
            Matrix.Translation(ref unit.Position, out trans);
            Matrix.Multiply(ref unit.Transformation, ref trans, out unit.Transformation);
            //Matrix.Translation(ref Position, out transformation);
            //Quaternion.RotationAxis(ref battleField.CellHeights[x][y].Normal, 0, out base.orient);
        }

        //public override PathFinderResult CurrentPath
        //{
        //    get
        //    {
        //        return base.CurrentPath;
        //    }
        //    set
        //    {
        //        base.CurrentPath = value;
        //        if (value != null)
        //        {
        //            currentNode = 0;
        //            if (value.NodeCount > 0)
        //                nextNode = 1;
        //            IsMoving = true;
        //            //justInCell = true;
        //        }
        //    }
        //}

        public DriverLocomotionDescription Description
        {
            get;
            protected set;
        }
        /// <returns>仍然需要更新true，否则false</returns>
        protected override bool updateRotate(float targetYaw, float dt)
        {
            if (targetYaw != unit.CellYaw)
            {
                if (!isRotating)
                {
                    isRotating = true;
                    rotationRadDiff = MathEx.RadianDifference(targetYaw, unit.CellYaw);
                    rotationDirection = rotationRadDiff > 0 ? 1 : -1;
                }
            }
            else
            {
                isRotating = false;
            }

            float incr = unit.Type.ROT * dt * (360f / 256f);
            unit.Yaw += rotationDirection * incr;
            unit.Yaw %= MathEx.PIf * 2;


            if (Math.Abs(unit.Yaw - targetYaw) < incr)
            {
                unit.CellYaw = targetYaw;
                unit.Yaw = targetYaw;
                isRotating = false;
            }

            //Update(dt);
            return isRotating;
        }
        /// <returns>仍然需要更新true，否则false</returns>
        protected override bool updateMove(int nx, int ny, int nz, float dt)
        {
            int cx = unit.X; // CurrentPath[currentNode].X;
            int cy = unit.Y; // CurrentPath[currentNode].Y;

            int dx = nx - cx;
            int dy = ny - cy;

            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            cellLerp += (unit.Type.Speed * dt * 10) / (dist * Terrain.HorizontalUnit);

            if (cellLerp >= 1)
            {
                cellLerp = 0;
                //currentNode = nextNode;
                //nextNode++;
                //unit.X = nx;
                //unit.Y = ny;
                unit.SetPosition(nx, ny);
                JustInCell = true;
                return false;
            }


            //Vector3 curCellPos = new Vector3(cx * Terrain.HorizontalUnit, field.CellInfo[cx][cy].HeightInfo.centre, cy * Terrain.HorizontalUnit);
            //Vector3 nxtCellPos = new Vector3(nx * Terrain.HorizontalUnit, field.CellInfo[nx][ny].HeightInfo.centre, ny * Terrain.HorizontalUnit);
            Vector3 curCellPos;
            Vector3 nxtCellPos;
            field.GetCellCenter(cx, cy, out curCellPos);
            field.GetCellCenter(nx, ny, out nxtCellPos);

            Vector3.Lerp(ref curCellPos, ref nxtCellPos, cellLerp, out unit.Position);
            JustInCell = false;

            //Update(dt);
            return true;
        }
    }

    public class FlyLocomotion : Locomotion
    {
        public FlyLocomotion(BattleField btfld, Techno unit, FlyLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class HoverLocomotion : Locomotion
    {
        public HoverLocomotion(BattleField btfld, Techno unit, HoverLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class JumpjetLocomotion : Locomotion
    {
        public JumpjetLocomotion(BattleField btfld, Techno unit, JumpjetLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class MechLocomotion : Locomotion
    {
        public MechLocomotion(BattleField btfld, Techno unit, MechLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class RocketLocomotion : Locomotion
    {
        public RocketLocomotion(BattleField btfld, Techno unit, RocketLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class ShipLocomotion : Locomotion
    {
        public ShipLocomotion(BattleField btfld, Techno unit, ShipLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class TeleportLocomotion : Locomotion
    {
        public TeleportLocomotion(BattleField btfld, Techno unit, TeleportLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class WalkLocomotion : Locomotion
    {
        public WalkLocomotion(BattleField btfld, Techno unit, WalkLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }

    public class TunnelLocomotion : Locomotion
    {
        public TunnelLocomotion(BattleField btfld, Techno unit, TunnelLocomotionDescription desc)
            : base(btfld, unit)
        {

        }
        public override void Update(float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateRotate(float targetYaw, float dt)
        {
            throw new NotImplementedException();
        }

        protected override bool updateMove(int x, int y, int z, float dt)
        {
            throw new NotImplementedException();
        }
    }
}
