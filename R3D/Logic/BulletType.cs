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
using R3D.ConfigModel;

namespace R3D.Logic
{
    /// <summary>
    /// 
    /// </summary>
    public class BulletType : EntityType
    {
        public BulletType(BattleField btfld, string typeName)
            : base(btfld, typeName)
        { }

        public override void Parse(ConfigurationSection sect)
        {
            throw new NotImplementedException();
        }



        public bool Airburst
        {
            get;
            protected set;
        }
        public bool Floater
        {
            get;
            protected set;
        }
        public bool SubjectToCliffs
        {
            get;
            protected set;
        }
        public bool SubjectToElevation
        {
            get;
            protected set;
        }
        public bool SubjectToWalls
        {
            get;
            protected set;
        }
        public bool VeryHigh
        {
            get;
            protected set;
        }
        public bool Shadow
        {
            get;
            protected set;
        }
        public bool Arcing
        {
            get;
            protected set;
        }
        public bool Dropping
        {
            get;
            protected set;
        }
        public bool Level
        {
            get;
            protected set;
        }
        public bool Inviso
        {
            get;
            protected set;
        }
        public bool Proximity
        {
            get;
            protected set;
        }
        public bool Ranged
        {
            get;
            protected set;
        }
        public bool Rotates
        {
            get;
            protected set;
        }
        public bool Inaccurate
        {
            get;
            protected set;
        }
        public bool FlakScatter
        {
            get;
            protected set;
        }
        public bool AA
        {
            get;
            protected set;
        }
        public bool AG
        {
            get;
            protected set;
        }
        public bool Degenerates
        {
            get;
            protected set;
        }
        public bool Bouncy
        {
            get;
            protected set;
        }
        public bool AnimPalette
        {
            get;
            protected set;
        }
        public bool FirersPalette
        {
            get;
            protected set;
        }

        public long Cluster
        {
            get;
            protected set;
        }
        public WeaponType AirburstWeapon
        {
            get;
            protected set;
        }
        public WeaponType ShrapnelWeapon
        {
            get;
            protected set;
        }
        public long ShrapnelCount
        {
            get;
            protected set;
        }
        public long DetonationAltitude
        {
            get;
            protected set;
        }
        public bool Vertical
        {
            get;
            protected set;
        }

        public double Elasticity
        {
            get;
            protected set;
        }
        public long Acceleration
        {
            get;
            protected set;
        }
        public int Color
        {
            get;
            protected set;
        }
        //public AnimType Trailer
        //{
        //    get;
        //    protected set;
        //}

        public long ROT
        {
            get;
            protected set;
        }
        public long CourseLockDuration
        {
            get;
            protected set;
        }
        public long SpawnDelay
        {
            get;
            protected set;
        }

        public bool Scalable
        {
            get;
            protected set;
        }

        public long Arm
        {
            get;
            protected set;
        }
        public byte AnimLow
        {
            get;
            protected set;
        }
        public byte AnimHigh
        {
            get;
            protected set;
        }
        public byte AnimRate
        {
            get;
            protected set;
        }

        public bool Flat
        {
            get;
            protected set;
        }
    }
}
