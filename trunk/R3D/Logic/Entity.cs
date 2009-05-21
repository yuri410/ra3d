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
using R3D.Core;
using R3D.GraphicsEngine;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;

namespace R3D.Logic
{
    [Flags()]
    public enum EntityCategory
    {
        Unknown = 0,
        Techno = 1,
        Infantry = 1 << 1,
        Unit = 1 + Infantry,
        Building = 2 + Infantry,
        Aircraft = 3 + Infantry,
        TerrainObject = 4 + Infantry,
    }

    public abstract class Entity : SceneObject
    {
        EntityType entType;
        protected BattleField battleField;
        //protected string name;
        protected bool sleeping;


        public Vector3 Position;
        protected Vector3 velocity;
        protected Vector3 angularVelocity;
        public Quaternion Orientation;


        protected Entity(BattleField btfld, EntityType type)
            : base(false)
        {
            battleField = btfld;
            entType = type;
            Orientation = Quaternion.Identity;

            TechnoType tech = type as TechnoType;
            if (tech != null)
            {
                switch (tech.WhatAmI)
                {
                    case TechnoCategory.Unit:
                        Category = EntityCategory.Unit | EntityCategory.Techno;
                        break;
                    case TechnoCategory.Infantry:
                        Category = EntityCategory.Infantry | EntityCategory.Techno;
                        break;
                    case TechnoCategory.Building:
                        Category = EntityCategory.Building | EntityCategory.Techno;
                        break;
                    case TechnoCategory.Aircraft:
                        Category = EntityCategory.Aircraft | EntityCategory.Techno;
                        break;
                    case TechnoCategory.Unknown:
                        Category = EntityCategory.Techno;
                        break;
                }
            }
            else
            {
                if (type is TerrainObjectType)
                {
                    Category = EntityCategory.TerrainObject;
                }
                else
                {
                    Category = EntityCategory.Unknown;
                }
            }
        }

        public virtual void SetPosition(int x, int y)
        {

        }

        //#region IRenderable 成员

        //public void Render()
        //{
        //    if (entType != null)
        //    {
        //        Device dev = entType.Model.Device;
        //        dev.SetTransform(TransformState.World, transformation);

        //        entType.Model.Render();
        //    }
        //}

        //#endregion

        /// <summary>
        /// 睡觉时不处理
        /// </summary>
        public bool IsSleeping
        {
            get { return sleeping; }
            set { sleeping = value; }
        }
        //public string Name
        //{
        //    get { return name; }
        //    set { name = value; }
        //}

        public bool IsPhysicalSimulated
        {
            get;
            set;
        }

        public virtual void Update(float dt)
        {
            if (IsPhysicalSimulated)
            {
                Quaternion add;
                MathEx.QuaternionMultiplyVector(ref Orientation, ref angularVelocity, out add);

                float hdt = 0.5f * dt;
                add.W *= hdt;
                add.X *= hdt;
                add.Y *= hdt;
                add.Z *= hdt;

                Quaternion.Add(ref Orientation, ref add, out Orientation);
                Orientation.Normalize();

                Vector3 offset = velocity;
                offset.X *= dt;
                offset.Y *= dt;
                offset.Z *= dt;

                Vector3.Add(ref Position, ref offset, out Position);

                Matrix rotation;
                Matrix.RotationQuaternion(ref Orientation, out rotation);

                Matrix.Translation(ref Position, out Transformation);
                Matrix.Multiply(ref Transformation, ref rotation, out Transformation);

                BoundingSphere.Center = Position;

                if ((offset.X == 0f) && (offset.Y == 0f) && (offset.Z == 0f))
                {
                    RequiresUpdate = true;
                }
            }
            else
            {
                Matrix rotation;
                Matrix.RotationQuaternion(ref Orientation, out rotation);

                Matrix.Multiply(ref Transformation, ref rotation, out Transformation);

                BoundingSphere.Center = Position;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override RenderOperation[] GetRenderOperation()
        {
            if (entType != null)
            {
                return entType.Model.GetRenderOperation();
            }
            return null;
        }

        public EntityCategory Category
        {
            get;
            protected set;
        }
    }
}
