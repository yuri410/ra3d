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
using R3D.Logic;
using R3D.Core;

namespace R3D.Collections
{


    public class SelectedUnitCollection : CollectionBase<Techno>
    {
        public SelectedUnitCollection()
            : base(60)
        {
        }

        public void SelectUnit(Techno unit)
        {
            unit.IsSelected = true;
            base.Add(unit);
        }

        public void DeselectUnit(Techno unit)
        {
            unit.IsSelected = false;
            base.Remove(unit);
        }

        public bool IsSelected(Techno unit)
        {
            return Contains(unit);
        }

        public void DeselectAll()
        {
            for (int i = 0; i < Count; i++)
            {
                Elements[i].IsSelected = false;
            }
            FastClear();
        }
    }
}
