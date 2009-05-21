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

namespace R3D.Logic
{
    public class Unit : Techno
    {
        UnitType techType;

        public UnitType UnitType        
        {
            get { return techType; }
        }

        public Unit(BattleField btfld, House owner, UnitType type)
            : base(btfld, owner, type)
        {
            techType = type;

            OwnerHouse.AddUnit(this);
        }

                

        public override void Update(float dt)
        {
            base.Update(dt);
        }


        public override void DeployAction()
        {
            if (techType.DeploysInto != null)
            {
                if (CanDeploy)
                {
                    Delete();

                    battleField.SpawnUnit(techType.DeploysInto.CreateInstance(OwnerHouse), X, Y, false);
                }
            }
            else
            {
                base.DeployAction();
            }
        }

        public override void Delete()
        {
            base.Delete();

            OwnerHouse.RemoveUnit(this);
        }

        

        public override bool CanDeploy
        {
            get
            {
                if (!IsDeployable)
                    return false;

                if (techType.DeploysInto != null)
                {
                    return techType.DeploysInto.IsBuildable(X, Y);
                }

                return true;
            }
        }

        public override bool IsDeployable
        {
            get
            {
                return this.techType.DeploysInto != null;
            }
        }
    }
}
