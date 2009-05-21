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
    public class WeaponType : ObjectType
    {
        public static readonly string DefaultAttackInMA = "AttackInMouseAct";
        public static readonly string DefaultAttackOutMA = "AttackOutMouseAct";
        public static readonly string DefaultAttackInvMA = "AttackInvMouseAct";

        protected string warheadName;
        protected string projectileName;



        public WeaponType(string typeName)
            : base(typeName)
        { }


        public override void Parse(ConfigurationSection sect)
        {
            projectileName = sect["Projectile"];
            warheadName = sect["Warhead"];

            Damage = sect.GetFloat("Damage", 0);
            ROF = sect.GetFloat("ROF", 0);
            Range = sect.GetFloat("Range", 0);

            MouseActionInrange = sect.GetString("MouseActionInrange", DefaultAttackInMA);
            MouseActionOutofRange = sect.GetString("MouseActionOutofRange", DefaultAttackOutMA);
            MouseActionInvalid = sect.GetString("MouseActionInvalid", DefaultAttackInvMA);

            MouseAction2Inrange = sect.GetString("MouseAction2Inrange", MouseActionInrange);
            MouseAction2OutofRange = sect.GetString("MouseAction2OutofRange", MouseActionOutofRange);
            MouseAction2Invalid = sect.GetString("MouseAction2Invalid", MouseActionInvalid);



        }

        public void ParseWarhead(Dictionary<string, WarheadType> warheads)
        {
            Warhead = warheads[warheadName];
        }
        public void ParseProjectile(Dictionary<string, BulletType> bulletTypes)
        {
            Projectile = bulletTypes[projectileName];
        }

        public WarheadType Warhead
        {
            get;
            protected set;
        }

        public BulletType Projectile
        {
            get;
            protected set;
        }

        public float Damage
        {
            get;
            protected set;
        }

        public float Range
        {
            get;
            protected set;
        }

        public float Speed
        {
            get;
            set;
        }

        public float ROF
        {
            get;
            protected set;
        }


        public string MouseActionInrange
        {
            get;
            protected set;
        }

        public string MouseActionOutofRange
        {
            get;
            protected set;
        }

        public string MouseActionInvalid
        {
            get;
            protected set;
        }



        public string MouseAction2Inrange
        {
            get;
            protected set;
        }
        public string MouseAction2OutofRange
        {
            get;
            protected set;
        }
        public string MouseAction2Invalid
        {
            get;
            protected set;
        }
    }
}
