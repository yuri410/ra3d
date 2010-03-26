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
    /// Contains paramters to build a locomotor.
    /// A LocomotionDescription object is created with a <seealso cref="R3D.Logic.TechnoType">TechnoType</seealso> object with parameters from the it.
    /// </summary>
    public abstract class LocomotionDescription : IConfigurable
    {
        SpeedType speedType;


        protected LocomotionDescription(BattleField btfld, LocomotionFactory fac)
        {
            Factory = fac;
            Field = btfld;
        }

        public SpeedType SpeedType
        {
            get { return speedType; }
            protected set { speedType = value; }
        }
        public BattleField Field
        {
            get;
            protected set;
        }
        public LocomotionFactory Factory
        {
            get;
            protected set;
        }

        #region IConfigurable 成员

        public virtual void Parse(ConfigurationSection sect)
        {
            string spdType = sect.GetString("SpeedType", null);
            if (spdType != null)
            {
                speedType = Field.SpeedTypes[spdType];
            }

        }

        #endregion
    }

    public class FlyLocomotionDescription : LocomotionDescription
    {
        public FlyLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }

    public class HoverLocomotionDescription : LocomotionDescription
    {
        public HoverLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }
    public class JumpjetLocomotionDescription : LocomotionDescription
    {
        public JumpjetLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }
    public class MechLocomotionDescription : LocomotionDescription
    {
        public MechLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }

    public class RocketLocomotionDescription : LocomotionDescription
    {
        public RocketLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }

    public class ShipLocomotionDescription : LocomotionDescription
    {
        public ShipLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }

    public class TeleportLocomotionDescription : LocomotionDescription
    {
        public TeleportLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }

    public class WalkLocomotionDescription : LocomotionDescription
    {
        public WalkLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }

    public class TunnelLocomotionDescription : LocomotionDescription
    {
        public TunnelLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            throw new NotImplementedException();
        }
    }
    public class DriverLocomotionDescription : LocomotionDescription
    {
        public DriverLocomotionDescription(BattleField btfld, LocomotionFactory fac)
            : base(btfld, fac)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sect">
        /// A <seealso cref="R3D.Config.ConfigurationSection">ConfigurationSection</seealso> object which
        /// contains configuration for the <seealso cref="R3D.Logic.TechnoType">TechnoType</seealso> this locomotion belongs to.
        /// </param>
        public override void Parse(ConfigurationSection sect)
        {
            //base.Parse(sect);
            string spdType = sect.GetString("SpeedType", "Wheel");
            SpeedType = Field.SpeedTypes[spdType];

            Speed = sect.GetFloat("Speed", 1);
            Rot = sect.GetFloat("ROT", 1);
            AccelerationFactor = sect.GetFloat("AccelerationFactor", 0.03f);
            Accelerates = sect.GetBool("Accelerates", false);
            DeaccelerationFactor = sect.GetFloat("DeaccelerationFactor", 0.002f);

        }


        public bool Accelerates
        {
            get;
            protected set;
        }
        public float DeaccelerationFactor
        {
            get;
            protected set;
        }
        public float AccelerationFactor
        {
            get;
            protected set;
        }
        public float Speed
        {
            get;
            protected set;
        }
        public float Rot
        {
            get;
            protected set;
        }
    }
}
