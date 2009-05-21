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

namespace R3D.Sound
{
    public class EVAVocManager : SoundManager
    {
        List<EvaVoice> queued = new List<EvaVoice>();

        public EVAVocManager(AudioConfigs ac)
            : base(ac)
        {
        }


        public bool IsPlaying
        {
            get;
            protected set;
        }

        public void PlayEvaVoice(string name)
        {
            EvaVoice snd = (EvaVoice)this[name];
            lock (queued)
            {
                bool contains = false;

                int prioId = -1;

                for (int i = 0; i < queued.Count; i++)
                {
                    if (prioId == -1)
                    {
                        if (snd.Priority > queued[i].Priority)                        
                            prioId = i;
                    }
                    else if (!contains)
                    {
                        if (queued[i] == snd)
                        {
                            contains = true;                            
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (!contains)
                {
                    snd.SoundStopped += this.GoOnNextVoc;

                    if (prioId == -1)
                    {
                        queued.Add((EvaVoice)snd);
                    }
                    else
                    {
                        queued.Insert(prioId, (EvaVoice)snd);
                    }
                }

                //if (!queued.Contains(snd))
                //{
                //    snd.SoundStopped += this.GoOnNextVoc;

                //    bool added = false;
                //    for (int i = 0; i < queued.Count; i++)
                //    {
                //        if (snd.Priority > queued[i].Priority)
                //        {
                //            queued.Insert(i, (EvaVoice)snd);
                //            added = true;
                //            break;
                //        }
                //    }

                //    if (!added)
                //    {
                //        queued.Add((EvaVoice)snd);
                //    }
                //}
            }
        }

        void GoOnNextVoc(SoundBase snd)
        {
            snd.SoundStopped -= this.GoOnNextVoc;

            
                if (queued.Count > 0)
                {
                    queued.RemoveAt(0);
                }
            
            IsPlaying = false;
        }

        public void Update()
        {
            lock (queued)
            {
                if (queued.Count > 0)
                {
                    queued[0].Play();
                    IsPlaying = true;
                }
            }
        }

        public void Clear()
        {
            lock (queued)
            {
                queued.Clear();
            }
        }
    }
}
