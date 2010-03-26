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
using R3D.ConfigModel;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Core
{
    /// <summary>
    /// Represents a view onto a 3D scene.
    /// </summary>
    public class Camera
    {
        protected Vector3 vPosition;

        protected Quaternion qOri;

        protected Vector3 vFront;

        protected Vector3 vTop;

        protected Vector3 vRight;

        float dFovy;
        float dNear;
        float dFar;
        float dAspect;

        protected Frustum fFrustum = new Frustum();

        protected bool isProjDirty;

        public Surface RenderTarget
        {
            get;
            set;
        }
        public Frustum Frustum
        {
            get { return fFrustum; }
        }

        /// <summary>
        /// Gets the view target direction(AKA z axis in camera space)
        /// 获取摄像机的朝向（摄像机空间Z轴）
        /// </summary>
        public Vector3 Front
        {
            get { return vFront; }
        }

        /// <summary>
        /// 摄像机Y
        /// </summary>
        public Vector3 Top
        {
            get { return vTop; }
        }

        /// <summary>
        /// 摄像机X
        /// </summary>
        public Vector3 Right
        {
            get { return vRight; }
        }

        /// <summary>
        /// Gets or sets the position of the camera eye point.
        /// </summary>
        /// <value>The position of the camera eye point.</value>
        public Vector3 Position
        {
            get { return vPosition; }
            set { vPosition = value; }
        }

        public Quaternion Orientation
        {
            get { return qOri; }
            set { qOri = value; }
        }

        public float FieldOfView
        {
            get { return MathEx.Radian2Angle(dFovy); }
            set
            {
                dFovy = MathEx.Angle2Radian(value);
                isProjDirty = true;
            }
        }

        public float NearPlane
        {
            get { return dNear; }
            set
            {
                dNear = value;
                isProjDirty = true;
            }
        }

        public float FarPlane
        {
            get { return dFar; }
            set
            {
                dFar = value;
                isProjDirty = true;
            }
        }

        public float AspectRatio
        {
            get { return dAspect; }
            set
            {
                dAspect = value;
                isProjDirty = true;
            }
        }

        public Camera(float aspect)
        {
            FarPlane = 200;
            FieldOfView = 45;
            NearPlane = 1;
            AspectRatio = aspect;
            qOri = Quaternion.Identity;
            Update();
        }

        public Matrix ViewMatrix
        {
            get { return fFrustum.view; }
        }
        public Matrix ProjectionMatrix
        {
            get { return fFrustum.proj; }
        }

        public float NearPlaneWidth
        {
            get;
            protected set;
        }
        public float NearPlaneHeight
        {
            get;
            protected set;
        }

        public virtual void GetSubareaProjection(ref RectangleF rect, out Matrix mat)
        {            
            //Matrix.PerspectiveRH(rect.Width, rect.Height, NearPlane, FarPlane, out mat);           
            Matrix.PerspectiveOffCenterRH(rect.Left * NearPlaneWidth, rect.Right * NearPlaneWidth, rect.Bottom * NearPlaneHeight, rect.Top * NearPlaneHeight, NearPlane, FarPlane, out mat);
        }

        public virtual void UpdateProjection()
        {
            //fFrustum.proj = Matrix.PerspectiveFovRH(MathEx.Angle2Radian(dFovy), dAspect, dNear, dFar);

            //float radFov = MathEx.Radian2Angle(dFovy);


            NearPlaneHeight = (float)(Math.Tan(dFovy * 0.5f)) * NearPlane * 2;
            NearPlaneWidth = NearPlaneHeight * AspectRatio;


            fFrustum.proj = Matrix.PerspectiveRH(NearPlaneWidth, NearPlaneHeight, dNear, dFar);
             
            isProjDirty = false;
        }

        public virtual void Update()
        {
            //Vector3 vMiPos = -vPosition;
            //mViewMatrix=//.LoadTranslateMatrix(ref vMiPos);
            //mViewMatrix *= qOri.ToMatrix4(new Vector3());

            //mProjMatrix.LoadProjectionMatrix(dFovy, dAspect, dNear, dFar);
            if (isProjDirty)
            {
                UpdateProjection();
            }

            MathEx.QuaternionToMatrix(ref qOri, out fFrustum.view);

            fFrustum.view = Matrix.Translation(-vPosition) * fFrustum.view;
            fFrustum.Update();//ref mViewMatrix, ref mProjMatrix);

            vFront = MathEx.QuaternionRotate(qOri, new Vector3(0, 0, -1));
            vTop = MathEx.QuaternionRotate(qOri, new Vector3(0, 1, 0));
            vRight = MathEx.QuaternionRotate(qOri, new Vector3(1, 0, 0));
        }


        public virtual void ResetView() { }
    }


    public class RtsCamera : Camera, IConfigurable
    {
        bool isPerspective;
        float height = 60;
        float orthoZoom;
        
        protected float dMoveSpeed = 2.5f;
        protected float dTurnSpeed = MathEx.PIf / 45;
        

        public RtsCamera(float fovy, float aspect)
            : base(aspect)
        {
            OrthoZoom = 65;
            isPerspective = fovy < 175 && fovy > 5;
            FieldOfView = fovy;
            ResetView();

            Update();
        }


        [TestOnly()]
        public void SetProjection(Matrix proj)
        {
            fFrustum.proj = proj;
        }



        public override void ResetView()
        {
            isPerspective = FieldOfView < 175 && FieldOfView > 5;
            if (isPerspective)
            {
                qOri = Quaternion.RotationAxis(new Vector3(0, 1, 0), -MathEx.PIf / 4);
                qOri *= Quaternion.RotationAxis(new Vector3(1, 0, 0), MathEx.PIf / 4);
            }
            else
            {
                qOri = Quaternion.RotationAxis(new Vector3(0, 1, 0), -MathEx.PIf / 4);
                qOri *= Quaternion.RotationAxis(new Vector3(1, 0, 0), MathEx.PIf / 6);
            }
        }

        public override void GetSubareaProjection(ref RectangleF rect, out Matrix mat)
        {
            if (isPerspective)
            {
                //mat = Matrix.Translation(rect.X * NearPlaneWidth, rect.Y * NearPlaneHeight, 0);

                //mat *= Matrix.PerspectiveRH(rect.Width * NearPlaneWidth, rect.Height * NearPlaneHeight, NearPlane, FarPlane);
                //Matrix.PerspectiveOffCenterRH(-NearPlaneWidth * 0.5f, NearPlaneWidth * 0.5f, NearPlaneHeight * 0.5f, -NearPlaneHeight * 0.5f, NearPlane, FarPlane, out mat);
                //MathEx.GetProjectionMatrixFrustum 
                //mat = MathEx.GetProjectionMatrixFrustum(NearPlaneWidth * rect.Left, NearPlaneWidth * rect.Right, NearPlaneHeight * rect.Top, NearPlaneHeight * rect.Bottom, NearPlane, FarPlane);

                //Matrix.PerspectiveOffCenterRH(NearPlaneWidth * -0.5f, NearPlaneWidth * 0.5f, NearPlaneHeight * 0.5f, NearPlaneHeight * -0.5f, NearPlane, FarPlane, out mat);
                Matrix.PerspectiveOffCenterRH(rect.Left * NearPlaneWidth, rect.Right * NearPlaneWidth, rect.Top * NearPlaneHeight, rect.Bottom * NearPlaneHeight, NearPlane, FarPlane, out mat);
            }
            else
            {
                //mat = MathEx.GetOrthoProjectionMatrix(rect.Left * NearPlaneWidth, rect.Right * NearPlaneWidth, rect.Top * NearPlaneHeight, rect.Bottom * NearPlaneHeight, NearPlane, FarPlane);
                Matrix.OrthoOffCenterRH(rect.Left * NearPlaneWidth, rect.Right * NearPlaneWidth, rect.Top * NearPlaneHeight, rect.Bottom * NearPlaneHeight, NearPlane, FarPlane, out mat);
            }
        }

        public override void UpdateProjection()
        {
            isPerspective = FieldOfView < 175 && FieldOfView > 5;

            if (isPerspective)
            {
                //fFrustum.proj = Matrix.PerspectiveFovRH(MathEx.Angle2Radian(FieldOfView), AspectRatio, NearPlane, FarPlane);
                base.UpdateProjection();
            }
            else
            {
                NearPlaneWidth = AspectRatio * OrthoZoom;
                NearPlaneHeight = OrthoZoom;
                fFrustum.proj = Matrix.OrthoRH(NearPlaneWidth, NearPlaneHeight, NearPlane, FarPlane);
                
                isProjDirty = false;
            }
        }

        public override void Update()
        {        
            vPosition.Y = height;
            if (isProjDirty)
            {
                UpdateProjection();
            }
            
            MathEx.QuaternionToMatrix(ref qOri, out fFrustum.view);

            fFrustum.view = Matrix.Translation(-vPosition) * fFrustum.view;
            fFrustum.Update();//ref mViewMatrix, ref mProjMatrix);

            vFront = MathEx.QuaternionRotate(qOri, new Vector3(0, 0, -1));
            vTop = MathEx.QuaternionRotate(qOri, new Vector3(0, 1, 0));
            vRight = MathEx.QuaternionRotate(qOri, new Vector3(1, 0, 0));

        }

        public float OrthoZoom
        {
            get { return orthoZoom; }
            set
            {
                orthoZoom = value;
                isProjDirty = true;
            }
        }

        public float Height
        {
            get
            {
                return height;
            }
            set
            {
                if (value >= 30 && value < 105)
                {
                    qOri *= Quaternion.RotationAxis(new Vector3(1, 0, 0), MathEx.Angle2Radian(value - height));

                    height = value;
                }
            }
        }
        public bool IsPerspective
        {
            get { return isPerspective; }
        }

        public void MoveFront()
        {
            Vector3 f = vFront;
            f.Y = 0;
            f.Normalize();
            vPosition += f * dMoveSpeed; 
        }
        public void MoveBack()
        {
            Vector3 f = vFront;
            f.Y = 0;
            f.Normalize();
            vPosition -= f * dMoveSpeed; 
        }

        public void TurnLeft()
        {
            Quaternion iq = qOri;
            iq.Conjugate();
            Vector3 top = MathEx.QuaternionRotate(iq, new Vector3(0, 1, 0));

            qOri *= Quaternion.RotationAxis(top, -dTurnSpeed);
            //qOri *= Quaternion.FromAngleAxis(dTurnSpeed, new Vector(0, -1, 0));
            qOri.Normalize();
        }
        public void TurnRight()
        {
            Quaternion iq = qOri;
            iq.Conjugate();

            Vector3 top = MathEx.QuaternionRotate(iq, new Vector3(0, 1, 0));// (~qOri).Rotate(new Vector(0, 1, 0));

            qOri *= Quaternion.RotationAxis(top, dTurnSpeed);
            qOri.Normalize();
        }
        public void MoveLeft()
        { vPosition -= vRight * dMoveSpeed; }
        public void MoveRight()
        { vPosition += vRight * dMoveSpeed; }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            FieldOfView = sect.GetFloat("CameraFieldOfView", 45f);
            OrthoZoom = sect.GetFloat("CameraOrthoViewZoom", 65f);
            Height = sect.GetFloat("CameraHeight", 60f);
        }

        #endregion
    }

    public class FpsCamera : Camera
    {
        public FpsCamera(float aspect)
            : base(aspect)
        { }
        protected float dMoveSpeed = 2.5f;
        protected float dTurnSpeed = MathEx.PIf / 45;

        public float MoveSpeed
        {
            get { return dMoveSpeed; }
            set { dMoveSpeed = value; }
        }
        public float TurnSpeed
        {
            get { return dTurnSpeed; }
            set { dTurnSpeed = value; }
        }
        public void MoveAbs(Vector3 mov)
        {
            vPosition += MathEx.QuaternionRotate(qOri, mov);
        }

        public void MoveFront()
        { vPosition += vFront * dMoveSpeed; }
        public void MoveBack()
        { vPosition -= vFront * dMoveSpeed; }
        public void MoveLeft()
        { vPosition -= vRight * dMoveSpeed; }
        public void MoveRight()
        { vPosition += vRight * dMoveSpeed; }
        public void MoveUp()
        {
            vPosition += vTop * dMoveSpeed;
        }
        public void MoveDown()
        {
            vPosition -= vTop * dMoveSpeed;
        }

        public void TurnLeft()
        {
            Quaternion iq = qOri;
            iq.Conjugate();
            Vector3 top = MathEx.QuaternionRotate(iq, new Vector3(0, 1, 0));

            qOri *= Quaternion.RotationAxis(top, -dTurnSpeed);
            //qOri *= Quaternion.FromAngleAxis(dTurnSpeed, new Vector(0, -1, 0));
            qOri.Normalize();
        }
        public void TurnRight()
        {
            Quaternion iq = qOri;
            iq.Conjugate();

            Vector3 top = MathEx.QuaternionRotate(iq, new Vector3(0, 1, 0));// (~qOri).Rotate(new Vector(0, 1, 0));

            qOri *= Quaternion.RotationAxis(top, dTurnSpeed);
            qOri.Normalize();
        }
        public void TurnUp()
        {
            qOri *= Quaternion.RotationAxis(new Vector3(1, 0, 0), -dTurnSpeed);
            qOri.Normalize();
        }
        public void TurnDown()
        {
            qOri *= Quaternion.RotationAxis(new Vector3(1, 0, 0), dTurnSpeed);
            qOri.Normalize();
        }
        public void RollLeft()
        {
            qOri *= Quaternion.RotationAxis(new Vector3(0, 0, 1), dTurnSpeed);
            qOri.Normalize();
        }
        public void RollRight()
        {
            qOri *= Quaternion.RotationAxis(new Vector3(0, 0, -1), dTurnSpeed);
            qOri.Normalize();
        }
    }
}
