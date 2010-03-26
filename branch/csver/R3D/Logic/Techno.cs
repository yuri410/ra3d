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
using R3D.AI;
using R3D.ConfigModel;
using R3D.Core;
using R3D.GraphicsEngine;
using R3D.IO;
using R3D.MathLib;
using SlimDX;
using SlimDX.Direct3D9;
using System.Drawing;

namespace R3D.Logic
{
    public enum UnitState
    {
        /// <summary>
        ///  Guard the general area where the unit starts at.
        /// </summary>
        AreaGuard,
        /// <summary>
        ///  Special attack mission used by team logic.
        /// </summary>
        Attack,
        /// <summary>
        ///  Engineer entry logic.
        /// </summary>
        Capture,
        /// <summary>
        ///  Buildings use this when building up after initial placement.
        /// </summary>
        Construction,
        /// <summary>
        ///  Enter building or transport for loading purposes.
        /// </summary>
        Enter,
        /// <summary>
        ///  Sit around and engage any enemy that wanders within weapon range.
        /// </summary>
        Guard,
        /// <summary>
        ///  Unit doesn't fire and is not considered a threat.
        /// </summary>
        Harmless,
        /// <summary>
        ///  Handle harvest ore - dump at refinery loop.
        /// </summary>
        Harvest,
        /// <summary>
        ///  Scan for and attack any enemies whereever they may be.
        /// </summary>
        Hunt,
        /// <summary>
        ///  Missile silo special launch missile mission.
        /// </summary>
        Missile,
        /// <summary>
        ///  Move to destination.
        /// </summary>
        Move,
        /// <summary>
        ///  While opening or closing a gate to allow passage.
        /// </summary>
        Open,
        /// <summary>
        ///  Patrol a series of waypoints
        /// </summary>
        Patrol,
        /// <summary>
        ///  Special move to destination after all other queued moves occur.
        /// </summary>
        QMove,
        /// <summary>
        ///  Service depot uses this mission to repair attached object.
        /// </summary>
        Repair,
        /// <summary>
        ///  Special team override mission.
        /// </summary>
        Rescue,
        /// <summary>
        ///  Run away (possibly leave the map).
        /// </summary>
        Retreat,
        /// <summary>
        ///  Tanya running to place bomb in building.
        /// </summary>
        Sabotage,
        /// <summary>
        ///  Buildings use this when deconstruction after being sold.
        /// </summary>
        Selling,
        /// <summary>
        ///  Unit sits still and plays dead.
        /// </summary>
        Sleep,
        /// <summary>
        ///  Just like guard mode, but cannot move.
        /// </summary>
        Sticky,
        /// <summary>
        ///  Stop moving and firing at the first available opportunity.
        /// </summary>
        Stop,
        /// <summary>
        ///  While dropping off cargo (e.g., APC unloading passengers).
        /// </summary>
        Unload
    }
    public class UnitStateParams : IConfigurable
    {
        /// <summary>
        /// Is its weapons disabled and thus ignored as a potential target until fired upon (def=no)?
        /// </summary>
        public bool NoThreat
        {
            get;
            set;
        }

        /// <summary>
        /// Is forced to sit there like a zombie and never recovers (def=no)?
        /// </summary>
        public bool Zombie
        {
            get;
            set;
        }

        /// <summary>
        /// Can it be recruited into a team or base defense (def=yes)?
        /// </summary>
        public bool Recruitable
        {
            get;
            set;
        }

        /// <summary>
        /// Is the object frozen in place but can still fire and function (def=no)?
        /// </summary>
        public bool Paralyzed
        {
            get;
            set;
        }

        /// <summary>
        /// Is allowed to retaliate while on this mission (def=yes)?
        /// </summary>
        public bool Retaliate
        {
            get;
            set;
        }

        /// <summary>
        /// Is allowed to scatter from threats (def=yes)?
        /// </summary>
        public bool Scatter
        {
            get;
            set;
        }

        /// <summary>
        /// delay between normal processing (larger = faster game, less responsiveness)
        /// </summary>
        public float Rate
        {
            get;
            set;
        }

        /// <summary>
        /// anti-aircraft delay rate (if not specifed it uses regular rate).
        /// </summary>
        public float AARate
        {
            get;
            set;
        }

        #region IConfigurable 成员

        public void Parse(ConfigurationSection sect)
        {
            NoThreat = sect.GetBool("NoThreat", false);
            Zombie = sect.GetBool("Zombie", false);
            Recruitable = sect.GetBool("Recruitable", true);
            Paralyzed = sect.GetBool("Paralyzed", false);
            Retaliate = sect.GetBool("Retaliate", true);
            Scatter = sect.GetBool("Scatter", true);
            Rate = sect.GetFloat("Rate", .016f);
            AARate = sect.GetFloat("AARate", Rate);
        }

        #endregion
    }

    public enum UnitKilledReason
    {
        Unknown,
        Killed,
        Accident
    }

    public class UnitKilledEventArgs : EventArgs
    {
        public bool Cancel
        {
            get;
            set;
        }

        public UnitKilledReason Reason
        {
            get;
            private set;
        }

        public Techno Killer
        {
            get;
            private set;
        }

        public UnitKilledEventArgs(Techno killer, UnitKilledReason reason)
        {
            Killer = killer;
            Reason = reason;
        }
    }

    public delegate void UnitKilledHandler(Techno sender, UnitKilledEventArgs e);
    public delegate void UnitPromotedHandler(Techno sender);

    

    public abstract class Techno : Entity
    {
        public static readonly string DefNoDeployMA = "NoDeployMouseAct";
        public static readonly string DefDeployMA = "DeployMouseAct";

        public const int PrimaryWeaponId = 0;
        public const int SecondaryWeaponId = 1;

        House owner;

        Locomotion locomotor;
        PathFinderManager pathFinderMgr;
        PathFinder pathFinder;

        protected float reloadCoolDown;

        UnitState status;

        TechnoCommandInterpreter cmdInterpreter = new TechnoCommandInterpreter();


        //int[] reloadCoolDown;

        //Vector2[] selLineVtx;

        public float CellYaw
        {
            get;
            set;
        }

        /// <summary>
        ///  
        /// </summary>
        /// <remarks> In radians</remarks>
        public float Yaw
        {
            get;
            set;
        }

        public PathFinder PathFinder
        {
            get { return pathFinder; }
        }

        public virtual void ChangeOwner(House house)
        {
            switch (Type.WhatAmI)
            {
                case TechnoCategory.Aircraft:
                    Aircraft air = (Aircraft)this;
                    owner.RemoveAircraft(air);
                    owner = house;
                    owner.AddAircraft(air);

                    break;
                case TechnoCategory.Building:
                    Building bld = (Building)this;
                    if (bld.BuildingType.IsBaseDefense)
                    {
                        owner.RemoveDefense(bld);
                        owner = house;
                        owner.AddDefense(bld);
                    }
                    else
                    {
                        owner.RemoveBuilding(bld);
                        owner = house;
                        owner.AddBuilding(bld);
                    }

                    break;
                case TechnoCategory.Infantry:
                    Infantry inf = (Infantry)this;
                    owner.RemoveInfantry(inf);
                    owner = house;
                    owner.AddInfantry(inf);
                    break;
                case TechnoCategory.Unit:
                    Unit unit = (Unit)this;
                    owner.RemoveUnit(unit);
                    owner = house;
                    owner.AddUnit(unit);
                    break;
            }
        }

        public House OwnerHouse
        {
            get { return owner; }
        }

        public bool IsSelected
        {
            get;
            set;
        }

        public Locomotion Locomotor
        {
            get { return locomotor; }
            set { locomotor = value; }
        }

        public int Health
        {
            get;
            set;
        }

        public TechnoType Type
        {
            get;
            protected set;
        }

        public int Veteran
        {
            get;
            protected set;
        }

        public void Promote()
        {
            Veteran++;
            if (UnitPromoted != null)
            {
                UnitPromoted(this);
            }
        }

        public Techno(BattleField btfld, House owner, TechnoType tech)
            : base(btfld, tech)
        {
            X = -1;
            Y = -1;

            Type = tech;
            Health = tech.Strength;

            this.owner = owner;

            if (tech.FactoryPlant)
            {
                if (tech.InfantryCostBonus != 1)
                {
                    owner.InfantryCostMOD.Add(this);
                }
                if (tech.UnitsCostBonus != 1)
                {
                    owner.VehicleCostMOD.Add(this);
                }
                if (tech.AircraftCostBonus != 1)
                {
                    owner.AircraftMoneyMOD.Add(this);
                }
                if (tech.BuildingsCostBonus != 1) 
                {
                    owner.BuildingMoneyMOD.Add(this);
                }
                if (tech.DefensesCostBonus != 1)
                {
                    owner.DefensesMoneyMOD.Add(this);
                }
            }

            pathFinderMgr = btfld.PathFinder;
            if (tech.LocomotorDesc != null)
            {
                locomotor = tech.LocomotorDesc.Factory.CreateInstance(btfld, this, tech.LocomotorDesc);                

                pathFinder = pathFinderMgr.CreatePathFinder();
                pathFinder.TerrainPassTable = tech.LocomotorDesc.SpeedType.PassableTable;
            }

           
            base.BoundingSphere.Radius = Terrain.HorizontalUnit * 0.5f;


            status = UnitState.Guard;

            //Texture testTex = TextureManager.Instance.CreateInstance(FileSystem.Instance.Locate("pathtest.dds", FileLocateRule.Root));

            //testMat = new MeshMaterial(Type.Model.Device);
            //testMat.SetEffect(null);
            //testMat.mat = MeshMaterial.DefaultMatColor;
            //testMat.SetTexture(0, testTex);


            //testData = new GeomentryData(this);
            //testData.Format = VertexPT1.Format;
            //testData.PrimCount = 2;
            //testData.VertexCount = 4;
            //testData.VertexDeclaration = new VertexDeclaration(Type.Model.Device, D3DX.DeclaratorFromFVF(VertexPT1.Format));
            //unsafe
            //{
            //    testData.VertexSize = sizeof(VertexPT1);

            //    testData.VertexBuffer = new VertexBuffer(Type.Model.Device, testData.VertexSize * testData.VertexCount, Usage.None, VertexPT1.Format, Pool.Managed);

            //    VertexPT1* dst = (VertexPT1*)testData.VertexBuffer.Lock(0, testData.VertexSize * testData.VertexCount, LockFlags.None).DataPointer.ToPointer();

            //    dst[0].pos = new Vector3(0, 0, 0);
            //    dst[1].pos = new Vector3(0, Terrain.HorizontalUnit, 0);
            //    dst[2].pos = new Vector3(Terrain.HorizontalUnit, 0, Terrain.HorizontalUnit);
            //    dst[3].pos = new Vector3(Terrain.HorizontalUnit, 0, 0);

            //    dst[0].u1 = 0; dst[0].v1 = 0;
            //    dst[1].u1 = 0; dst[1].v1 = 1;
            //    dst[2].u1 = 1; dst[2].v1 = 1;
            //    dst[3].u1 = 1; dst[3].v1 = 0;

            //    testData.VertexBuffer.Unlock();



            //    testData.IndexBuffer = new IndexBuffer(Type.Model.Device, sizeof(int) * 6, Usage.None, Pool.Managed, false);
            //    int* idst = (int*)testData.IndexBuffer.Lock(0, sizeof(int) * 6, LockFlags.None).DataPointer.ToPointer();

            //    idst[0] = 0;
            //    idst[1] = 1;
            //    idst[2] = 2;
            //    idst[3] = 0;
            //    idst[4] = 2;
            //    idst[5] = 3;

            //    testData.IndexBuffer.Unlock();
            //}
        }

        public UnitState State
        {
            get { return status; }
            //set
            //{
            //    if (value != status)
            //    {
                   

            //        status = value;
            //    }
            //}
        }

        /// <summary>
        /// X coordinate in 3D API
        /// </summary>
        public int X
        {
            get;
            protected set;
        }
        /// <summary>
        /// Z coordinate in 3D API
        /// </summary>
        public int Y
        {
            get;
            protected set;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Z
        {
            get;
            set;
        }

        

        public override void SetPosition(int x, int y)
        {
            if (X != x || Y != y)
            {
                RequiresUpdate = true;
            }


            if (X != -1 && Y != -1 && battleField.CellInfo[X][Y].Used)
            {
                battleField.CellInfo[X][Y].Units.Remove(this);
            }

            X = x;
            Y = y;

            if (battleField.CellInfo[x][y].Used)
            {
                battleField.CellInfo[x][y].Units.Add(this);
            }

            //SetDestination(X, Y, 0);
            battleField.GetCellCenter(x, y, out Position);
            //Position = battleField.GetCellCenter(); //new Vector3(X * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f, battleField.CellInfo[X][Y].HeightInfo.centre, Y * Terrain.HorizontalUnit + Terrain.HorizontalUnit * 0.5f);
            //Matrix.Translation(ref Position, out base.Transformation);

            //Quaternion.RotationAxis(ref battleField.CellInfo[x][y].HeightInfo.Normal, Yaw, out Orientation);
            //Orientation *= Quaternion.RotationAxis(new Vector3(0, 1, 0), MathEx.Angle2Radian(90));

            base.BoundingSphere.Center = Position;
        }

        public bool IsOutOfControl
        {
            get;
            set;
        }

        public override void Update(float dt)
        {
            //base.Update(dt);
            
            if (locomotor != null && !IsOutOfControl)
            {
                cmdInterpreter.Update(dt);

                locomotor.Update(dt);
                BoundingSphere.Center = Position;
            }
            else
            {
                base.Update(dt);
            }
        }

        public bool IsMoving
        {
            get
            {
                if (locomotor != null)
                    return locomotor.IsMoving;
                return false;
            }
        }


     


        /// <summary>
        /// 立即执行部署动作
        /// </summary>
        public virtual void DeployAction()
        {

        }

        public virtual void AttackAction()
        {

        }

        public void Command(TechnoScriptSet script)
        {
            TechnoCommandSet cmdSet = new TechnoCommandSet();

            cmdInterpreter.CommandSet = cmdSet;
        }

        public void Command(string name, string[] param)
        {

            //MoveCommand mcmd = new MoveCommand(this, x, y, z);

            TechnoCommandSet cmdSet = new TechnoCommandSet();
            cmdSet.AddCommand(TechnoCommandManager.Instance.GetCommand(name, this, param));

            cmdInterpreter.CommandSet = cmdSet;
        }

        public virtual void Deploy()
        {
            DeployCommand cmd = new DeployCommand(this);
            TechnoCommandSet cmdSet = new TechnoCommandSet();
            cmdSet.AddCommand(cmd);

            cmdInterpreter.CommandSet = cmdSet;
        }

        public virtual void Move(int x, int y, int z)
        {
            //pathFinder.Reset();
            if (locomotor != null)
            {
                //PathFinderResult path = pathFinder.FindPath(X, Y, Z, x, y, z);

                //if (path != null)
                //{

                MoveCommand mcmd = new MoveCommand(this, x, y, z);

                TechnoCommandSet cmdSet = new TechnoCommandSet();
                cmdSet.AddCommand(mcmd);

                cmdInterpreter.CommandSet = cmdSet;
                //}
            }
        }

        public virtual void Stop()
        {
            cmdInterpreter.Stop();
        }
        


        public virtual void RenderSelectionBox(Line uiLine, ref Vector3 posProj, float projRadius)
        {
            Vector2[] vtx = new Vector2[3];
            vtx[0].X = posProj.X - projRadius;
            vtx[0].Y = posProj.Y + projRadius - 6;

            vtx[1].X = posProj.X - projRadius;
            vtx[1].Y = posProj.Y + projRadius;

            vtx[2].X = posProj.X - projRadius + 6;
            vtx[2].Y = posProj.Y + projRadius;

            uiLine.Draw(vtx, Color.White);

            vtx[0].X = posProj.X + projRadius;
            vtx[0].Y = posProj.Y + projRadius - 6;

            vtx[1].X = posProj.X + projRadius;
            vtx[1].Y = posProj.Y + projRadius;

            vtx[2].X = posProj.X + projRadius - 6;
            vtx[2].Y = posProj.Y + projRadius;
            uiLine.Draw(vtx, Color.White);

            vtx = new Vector2[5];
            vtx[0].X = posProj.X - projRadius;
            vtx[0].Y = posProj.Y - projRadius;

            vtx[1].X = posProj.X + projRadius;
            vtx[1].Y = posProj.Y - projRadius;

            vtx[2].X = posProj.X + projRadius;
            vtx[2].Y = posProj.Y - projRadius + 3;

            vtx[3].X = posProj.X - projRadius;
            vtx[3].Y = posProj.Y - projRadius + 3;

            vtx[4].X = posProj.X - projRadius;
            vtx[4].Y = posProj.Y - projRadius;
            uiLine.Draw(vtx, Color.White);
        }

        public static int CalculateOriDesc(int dx, int dy)
        {
            return dx * 3 + dy;
        }
        public static float CalculateYaw(int dx, int dy)
        {
            return CalculateYaw(dx * 3 + dy);
        }

        public static float CalculateYaw(int oriDesc)
        {
            switch (oriDesc)
            {
                case -4:// -1 , -1
                    return MathEx.Angle2Radian(135);
                case -1://  0, -1
                    return MathEx.Angle2Radian(90);
                case -3:// -1, 0
                    return MathEx.Angle2Radian(180);
                case 2:// 1, -1
                    return MathEx.Angle2Radian(45);
                case -2:// -1, 1
                    return MathEx.Angle2Radian(225);
                case 3:// 1, 0
                    return MathEx.Angle2Radian(0);
                case 1: //0, 1
                    return MathEx.Angle2Radian(270);
                case 4: //1, 1
                    return MathEx.Angle2Radian(315);
            }
            return 0;
        }

        public void Kill(Techno killer)
        {
            if (UnitKilled != null)
            {
                UnitKilledReason reason = (killer == null) ? UnitKilledReason.Accident : UnitKilledReason.Killed;

                UnitKilled(killer, new UnitKilledEventArgs(killer, reason));
            }

            Delete();
        }
        public virtual void Delete()
        {
            //battleField.UnitsOnMap.Remove(this);
            battleField.RemoveUnit(this);
            if (X != -1 && Y != -1)
            {
                battleField.CellInfo[X][Y].Units.Remove(this);
            }
        }


        protected virtual int SelectWeapon(Entity target)
        {
            if (target == null)
            {
                return PrimaryWeaponId;
            }



            return PrimaryWeaponId;
        }

        public WeaponType GetWeapon(int index)
        {
            return Type.Weapons[index];
        }


        public virtual bool IsLegalTarget(Entity ent)
        {
            if ((ent.Category & EntityCategory.Techno) == EntityCategory.Techno)
            {
                Techno tech = (Techno)ent;
                return tech.Type.LegalTarget && OwnerHouse.IsAllyOrEnemy(tech.OwnerHouse);
            }
            //if ((ent.Category & EntityCategory.TerrainObject) == EntityCategory.TerrainObject)
            //{
            //    return false;
            //}


            return false;
        }
        public virtual string GetUIName(House getter)
        {
            
            return Type.UIName;
        }

        public virtual bool CanAttack()
        {
            return Type.WeaponCount > 0;
        }

        protected void FireWeapon(int index)
        {
            
        }

        public event UnitKilledHandler UnitKilled;
        public event UnitPromotedHandler UnitPromoted;

       
        //GeomentryData testData;

        //MeshMaterial testMat;

        //public override RenderOperation[] GetRenderOperation()
        //{
        //    if (locomotor == null || locomotor.CurrentPath == null)
        //    {
        //        return base.GetRenderOperation();
        //    }
        //    else
        //    {
        //        RenderOperation[] baseOps = base.GetRenderOperation();
        //        RenderOperation[] ops = new RenderOperation[locomotor.CurrentPath.NodeCount + baseOps.Length];  // locomotor.CurrentPath;

        //        for (int i = 0; i < baseOps.Length; i++)
        //        {
        //            ops[i].Geomentry = testData;
        //            ops[i].Material = testMat;


        //            //Position.X = cx * Terrain.HorizontalUnit;
        //            //Position.Y = field.CellInfo[cx][cy].HeightInfo.centre;
        //            //Position.Z = cy * Terrain.HorizontalUnit;
        //            int cx = locomotor.CurrentPath[i].X;
        //            int cy = locomotor.CurrentPath[i].Y;


        //            ops[i].Transformation = Matrix.Translation(cx, Type.Field.CellInfo[cx][cy].HeightInfo.centre + 1, cy);
        //        }

        //        return ops;
        //    }
        //}
        //public override RenderOperation[] GetRenderOperation()
        //{
            //if (IsSelected)
            //{
                //return base.GetRenderOperation();
            //}
            //else
            //{
            //    RenderOperation[] mdlOp = base.GetRenderOperation();

            //    int mdlLen = mdlOp.Length;

            //    RenderOperation[] ops = new RenderOperation[mdlLen + 1];
            //    Array.Copy(mdlOp, ops, mdlLen);

            //    ops[mdlLen].Material = MeshMaterial.DefaultMaterial;
            //    ops[mdlLen].Geomentry
            //    ops[mdlLen].Transformation = base.Transformation;

            //    return ops;
            //}
        //}

        /// <summary>
        /// 指示该部队可否在一定情况下部署
        /// </summary>
        public virtual bool IsDeployable
        {
            get { return false; }
        }

        /// <summary>
        /// 指示该部队在当前情况下能否部署
        /// </summary>
        public virtual bool CanDeploy
        {
            get
            {
                return !IsPrimaryFactory;
            }
        }

        public bool IsPrimaryFactory
        {
            get;
            protected internal set;
        }

        public void GotPrimaryFactory()
        {
            IsPrimaryFactory = true;
        }
    }

    
}
