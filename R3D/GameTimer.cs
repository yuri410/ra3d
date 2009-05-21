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
using System.Runtime.InteropServices;

namespace R3D
{
    public static class GameTimer
    {
        class TimeAdjuster
        {
            public TimeAdjuster()
            {
                timeBeginPeriod(tCaps.min);
            }
            ~TimeAdjuster()
            {
                timeEndPeriod(tCaps.min);
            }
        }

        struct TimeCaps
        {
            public int min, max;
        }

        static TimeAdjuster tAdj;
        static TimeCaps tCaps;

        static int loopPassed;
        static uint startTime;
        static long curTime;

        

        static GameTimer()
        {
            timeGetDevCaps(out tCaps, 8);
            tAdj = new TimeAdjuster();

            startTime = GetTime();
            Update();
        }

        public static int MinDelay
        {
            get { return tCaps.min; }
        }
        public static int MaxDelay
        {
            get { return tCaps.max; }
        }

        [DllImport("winmm.dll")]
        static extern int timeGetDevCaps(out TimeCaps cap, int cbdc);
        [DllImport("winmm.dll")]
        static extern int timeBeginPeriod(int per);
        [DllImport("winmm.dll")]
        static extern int timeEndPeriod(int per);

        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        public static extern uint GetTime();

        public static long GetInterval()
        {
            long old = curTime;
            Update();
            return curTime - old;
        }

        public static void Update()
        {
            long t = GetTime() + uint.MaxValue * loopPassed;
            if (t < curTime)
            {
                loopPassed++;
                curTime = t + uint.MaxValue;
            }
            else
                curTime = t;
        }


        public static long Time
        {
            get
            {
                return curTime - startTime;
            }
        }

        public static TimeSpan TimeSpan
        {
            get { return TimeSpan.FromMilliseconds(Time); }
        }
    }
}
