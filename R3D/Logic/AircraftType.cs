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
    public class AircraftType : TechnoType
    {
        public AircraftType(BattleField btfld, string typeName)
            : base(btfld, typeName)
        { }

        public override Techno CreateInstance(House owner)
        {
            return new Aircraft(Field, owner, this);
        }
        public override void Parse(ConfigurationSection sect)
        {
            base.Parse(sect);
            ImmuneToPsionics = sect.GetBool("ImmuneToPsionics", false);
        }


        public override TechnoCategory WhatAmI
        {
            get
            {
                return TechnoCategory.Aircraft;
            }
        }
    }
}
