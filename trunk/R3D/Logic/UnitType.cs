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
    public class UnitType : TechnoType
    {
        public UnitType(BattleField btfld, string typeName)
            : base(btfld, typeName)
        { }
        public UnitType(BattleField btfld, string typeName, LocomotionDescription desc)
            : base(btfld, typeName)
        {
            LocomotorDesc = desc;
        }
        public override Techno CreateInstance(House owner)
        {
            return new Unit(Field, owner, this);
        }

        public BuildingType DeploysInto
        {
            get;
            protected set;
        }

        string DeploysIntoName
        {
            get;
            set;
        }

        public override void ParseBuildingTypes(Dictionary<string, BuildingType> blds)
        {
            base.ParseBuildingTypes(blds);

            if (!string.IsNullOrEmpty(DeploysIntoName))
            {
                DeploysInto = blds[DeploysIntoName];
            }
        }

        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            ImmuneToPsionics = sect.GetBool("ImmuneToPsionics", false);
            DeploysIntoName = sect.GetString("DeploysInto", null);
        }

        public override void ParseTechnoTypes(PrerequisiteExpressionCompiler compiler, PrerequisiteAliasInfo pinfo, Dictionary<string, TechnoType> tech)
        {
            base.ParseTechnoTypes(compiler, pinfo, tech);
        }

        public override TechnoCategory WhatAmI
        {
            get
            {
                return TechnoCategory.Unit;
            }
        }

    }
}
