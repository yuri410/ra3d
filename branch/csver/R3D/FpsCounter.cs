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
using System.Diagnostics;

namespace R3D
{
    public class FpsCounter
    {
        long iBegin, iEnd;
        float dFps = -1;

        long iStat;
        float dMultiplier = 1;

        int iStatMax = 100;

        Stopwatch sw = Stopwatch.StartNew();

        public int StepFrame
        {
            get { return iStatMax; }
            set { iStatMax = value; }
        }
        public float FPS
        {
            get { return dFps; }
        }
        public float Multiplier
        {
            get { return dMultiplier; }
            set { dMultiplier = value; }
        }


        public void SetEnd()
        {
            iStat++;

            if (iStat >= iStatMax)
            {
                iEnd = sw.ElapsedMilliseconds;// Environment.TickCount;

                dFps = 1000.0f * ((float)iStat / (float)(iEnd - iBegin));
                iStat = 0;
                SetBegin();
            }
        }
        public void SetBegin()
        {
            iBegin = sw.ElapsedMilliseconds;// Environment.TickCount;
        }

        public override string ToString()
        {
            return "{ FPS= " + System.Math.Round(dFps * dMultiplier, 2).ToString() + " }";
        }
    }
}
