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
    public class Building : Techno
    {
        BuildingType techType;

        public BuildingType BuildingType
        {
            get { return techType; }
        }

        public Building(BattleField btfld, House owner, BuildingType type)
            : base(btfld, owner, type)
        {
            techType = type;

            if (techType.IsBaseDefense)
            {
                OwnerHouse.AddDefense(this);
            }
            else
            {
                OwnerHouse.AddBuilding(this);
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);
        }

        public override void Delete()
        {
            if (techType.IsBaseDefense)
            {
                OwnerHouse.RemoveDefense(this);
            }
            else
            {
                OwnerHouse.RemoveBuilding(this);
            }


            battleField.UnitsOnMap.Remove(this);

            if (X != -1 && Y != -1)
            {
                for (int i = 0; i < techType.Foundation.Width; i++)
                {
                    for (int j = 0; j < techType.Foundation.Height; j++)
                    {
                        if (battleField.CellInfo[X + i][Y + j].Used)
                            battleField.CellInfo[X + i][Y + j].Units.Remove(this);
                    }
                }
            }
        }

        public override void SetPosition(int x, int y)
        {
            if (X != -1 && Y != -1)
            {
                for (int i = 0; i < techType.Foundation.Width; i++)
                {
                    for (int j = 0; j < techType.Foundation.Height; j++)
                    {
                        if (battleField.CellInfo[X + i][Y + j].Used)
                            battleField.CellInfo[X + i][Y + j].Units.Remove(this);
                    }
                }
            }

            X = x;
            Y = y;

            for (int i = 0; i < techType.Foundation.Width; i++)
            {
                for (int j = 0; j < techType.Foundation.Height; j++)
                {
                    if (battleField.CellInfo[x + i][y + j].Used)
                        battleField.CellInfo[x + i][y + j].Units.Remove(this);
                }
            }

            battleField.GetCellCenter(x, y, out Position);

            base.BoundingSphere.Center = Position;
        }

    }
}
