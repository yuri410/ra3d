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

namespace R3D.Sound
{
    public class DummySound : SoundBase
    {
        static SoundBase singleton;
        private DummySound()
            : base(null, "Dummy")
        { }

        public static SoundBase Instance
        {
            get
            {
                if (singleton == null)
                    singleton = new DummySound();
                return singleton;
            }
        }

        public override void LocateSounds(string[] sounds, params object[] para)
        {
            throw new NotSupportedException();
        }

        public override void Play()
        {
            // do nothing
        }

        public override int GeiSize()
        {
            return 0;
        }

        protected override void load()
        {
            throw new NotSupportedException();
        }

        protected override void unload()
        {
            throw new NotSupportedException();
        }
    }
}
