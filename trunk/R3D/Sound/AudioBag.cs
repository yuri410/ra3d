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
using System.IO;
using System.Runtime.InteropServices;
using R3D.IO;

namespace R3D.Sound
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AudioIdxHeader
    {
        public FileID id;
        public int two;
        public int soundCount;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public unsafe struct AudioIdxEntry
    {
        public fixed byte fname[16];
        //public string fname;
        public int offset;
        public int size;
        public int sampleRate;
        public int flags;
        public int chunkSize;
    }

    public class AudioBag
    {
        //AudioIdxEntry[] sounds;
        Dictionary<string, AudioIdxEntry> data;
        AudioIdxHeader header;



        private unsafe AudioBag(ResourceLocation fl)
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(fl);
            
            header.id = (FileID)br.ReadInt32();

            if (header.id == FileID.AudioIdx)
            {
                header.two = br.ReadInt32();
                header.soundCount = br.ReadInt32();

                data = new Dictionary<string, AudioIdxEntry>(header.soundCount);

                //byte[] buffer;
                for (int i = 0; i < header.soundCount; i++)
                {
                    //buffer = br.ReadBytes(sizeof(AudioIdxEntry));

                    AudioIdxEntry entry;

                    for (int j = 0; j < 16; j++)
                    {
                        entry.fname[j] = br.ReadByte();
                    }
                    entry.offset = br.ReadInt32();
                    entry.size = br.ReadInt32();
                    entry.sampleRate = br.ReadInt32();
                    entry.flags = br.ReadInt32();
                    entry.chunkSize = br.ReadInt32();

                    string name = Utils.GetName(entry.fname);

                    data.Add(name, entry);
                }
                //sounds = new AudioIdxEntry[header.soundCount];

                //byte[] buffer = br.ReadBytes(header.soundCount * sizeof(AudioIdxEntry));

                //Helper.MemCopy(Helper.ldelema<AudioIdxEntry>(sounds), Helper.ldelema<byte>(buffer), buffer.Length);             
            }
            else
            {
                br.Close();
                throw new DataFormatException(fl.ToString());
            }


            br.Close();
        }

        public int SoundCount
        {
            get { return header.soundCount; }
        }

        public static AudioBag FromFile(string file)
        {
            return FromFile(new FileLocation(file));
        }
        public static AudioBag FromFile(ResourceLocation fl)
        {
            return new AudioBag(fl);
        }

        public Dictionary<string, AudioIdxEntry> Data
        {
            get { return data; }
        }

        public AudioIdxEntry[] GetSounds()
        {
            Dictionary<string, AudioIdxEntry>.ValueCollection vals = data.Values;//.CopyTo();
            AudioIdxEntry[] d = new AudioIdxEntry[vals.Count];
            vals.CopyTo(d, 0);
            return d;
        }

    }
}
