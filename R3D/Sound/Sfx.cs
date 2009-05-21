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
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using R3D.Base;
using R3D.ConfigModel;
using R3D.IO;

namespace R3D.Sound
{

    public class Sfx : SoundBase
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct WavChunkHeader
        {
            public WavId id;
            public int size;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct WavFormatChunk
        {
            public WavChunkHeader header;
            public short formattag;
            public short channelCount;
            public int samplerate;
            public int byterate;
            public short blockalign;
            public short cbits_sample;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct WavHeader
        {
            public WavChunkHeader fileHeader;
            public WavId formType;
            public WavFormatChunk formatChunk;
            public WavChunkHeader dataChunkHeader;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct WavImaAdpcmFormatChunk
        {
            public WavChunkHeader header;
            public short formattag;
            public short channelCount;
            public int samplerate;
            public int byterate;
            public short blockalign;
            public short cbits_sample;
            public ushort extraSize;
            public ushort blockSize;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct WavFactChunk
        {
            public WavChunkHeader header;
            public int sampleCount;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct WavImaAdpcmHeader
        {
            public WavChunkHeader fileHeader;
            public WavId formType;
            public WavImaAdpcmFormatChunk formatChunk;
            public WavFactChunk factChunk;
            public WavChunkHeader dataChunkHeader;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        protected struct ImaAdpcmChunkHeader
        {
            public short sample;
            public byte index;
            public byte reserved;
        }
        protected enum WavId : int
        {
            WavFile = ((byte)'F' << 24) | ((byte)'F' << 16) | ((byte)'I' << 8) | ((byte)'R'),
            WavForm = ((byte)'E' << 24) | ((byte)'V' << 16) | ((byte)'A' << 8) | ((byte)'W'),
            WavFormat = ((byte)' ' << 24) | ((byte)'t' << 16) | ((byte)'m' << 8) | ((byte)'f'),
            WavFact = ((byte)'t' << 24) | ((byte)'c' << 16) | ((byte)'a' << 8) | ((byte)'f'),
            WavData = ((byte)'a' << 24) | ((byte)'t' << 16) | ((byte)'a' << 8) | ((byte)'d')
        }

        protected int minVol;
        protected int maxVol;
        protected int volume;
        protected int range;
        protected int loop;
        protected int limit;
        protected int minDelay;
        protected int maxDelay;


        protected SoundControl control;
        protected SoundType type;



        FMOD.System soundSystem;
        AudioIdxEntry[] sounds;

        ResourceLocation bagFile;

        int size;
        bool isUnloadable;
        SoundManager sndManager;



        public SoundType Type
        {
            get { return type; }
            set { type = value; }
        }
        public SoundControl Control
        {
            get { return control; }
            set { control = value; }
        }

        public int MaxDelay
        {
            get { return maxDelay; }
            set { maxDelay = value; }
        }
        public int MinDelay
        {
            get { return minDelay; }
            set { minDelay = value; }
        }
        public int Limit
        {
            get { return limit; }
            set { limit = value; }
        }
        public int Volume
        {
            get { return volume; }
            set { volume = value; }
        }
        public int MinVolume
        {
            get { return minVol; }
            set
            {
                minVol = value;
                if (maxVol < minVol)
                    maxVol = minVol;
            }
        }
        public int MaxVolume
        {
            get { return maxVol; }
            set
            {
                maxVol = value;
                if (minVol > maxVol)
                    minVol = maxVol;
            }
        }
        public int Loop
        {
            get { return loop; }
            set { loop = value; }
        }
        public int Range
        {
            get { return range; }
            set { range = value; }
        }

        public Sfx(SoundManager mgr, FMOD.System sndSys, string name)
            : base(mgr, name)
        {
            sndManager = mgr;
            soundSystem = sndSys;
        }

        FMOD.Sound[] fmodSounds;


        /// <summary>
        /// 防止传给非托管代码的delegate被GC和谐
        /// </summary>
        FMOD.CHANNEL_CALLBACK stopHandler;

        //static Dictionary<int, FMOD.CHANNEL_CALLBACK> tracker = new Dictionary<int, FMOD.CHANNEL_CALLBACK>();

        

        FMOD.RESULT SoundEnd(IntPtr channelraw, FMOD.CHANNEL_CALLBACKTYPE type, int command, uint commanddata1, uint commanddata2)
        {
            isUnloadable = true;
            FMOD.Channel ch = new FMOD.Channel();
            ch.setRaw(channelraw);
            FMOD.RESULT res = ch.setCallback(FMOD.CHANNEL_CALLBACKTYPE.END, null, 0);
           
            //tracker.RemoveAt();
            //stopHandler = null;
            return res;
        }

        public override void Play()
        {
            if (sounds != null)
            {
                isUnloadable = false;
                Use();
                FMOD.Channel ch = null;
                soundSystem.playSound(FMOD.CHANNELINDEX.FREE, fmodSounds[0], false, ref ch);

                stopHandler = SoundEnd;
                ch.setCallback(FMOD.CHANNEL_CALLBACKTYPE.END, stopHandler, 0);
                //tracker.Add(stopHandler.GetHashCode(), stopHandler);
                ch.setVolume(((float)volume / 100) * sndManager.SoundVolume);

            }
        }

        static unsafe WavHeader BuildWavHeader(int sampleCount, short samplerate, short sampleSize, short channelCount)
        {
            WavHeader header;
            header.fileHeader.id = WavId.WavFile;
            header.fileHeader.size = sizeof(WavHeader) - sizeof(WavChunkHeader) + sampleSize * sampleCount * channelCount;
            header.formType = WavId.WavForm;
            header.formatChunk.header.id = WavId.WavFormat;
            header.formatChunk.header.size = sizeof(WavFormatChunk) - sizeof(WavChunkHeader);
            header.formatChunk.formattag = 1;
            header.formatChunk.channelCount = channelCount;
            header.formatChunk.samplerate = samplerate;
            header.formatChunk.byterate = sampleSize * channelCount * samplerate;
            header.formatChunk.blockalign = (short)(sampleSize * channelCount);
            header.formatChunk.cbits_sample = (short)(sampleSize << 3);
            header.dataChunkHeader.id = WavId.WavData;
            header.dataChunkHeader.size = sampleSize * sampleCount * channelCount;
            return header;
        }
        static unsafe WavImaAdpcmHeader BuildWavImaAdpcmHeader(int audioSize, int sampleCount, int samplerate, int channelCount)
        {
            WavImaAdpcmHeader header;
            header.fileHeader.id = WavId.WavFile;// wav_file_id;
            header.fileHeader.size = sizeof(WavImaAdpcmHeader) - sizeof(WavChunkHeader) + audioSize;
            header.formType = WavId.WavForm; // wav_form_id;
            header.formatChunk.header.id = WavId.WavFormat;
            header.formatChunk.header.size = sizeof(WavImaAdpcmFormatChunk) - sizeof(WavChunkHeader);
            header.formatChunk.formattag = 0x11;
            header.formatChunk.channelCount = (short)channelCount;
            header.formatChunk.samplerate = samplerate;
            header.formatChunk.byterate = 11100 * channelCount * samplerate / 22050;
            header.formatChunk.blockalign = (short)(512 * channelCount);
            header.formatChunk.cbits_sample = 4;
            header.formatChunk.extraSize = 2;
            header.formatChunk.blockSize = 1017;
            header.factChunk.header.id = WavId.WavFact;
            header.factChunk.header.size = sizeof(WavFactChunk) - sizeof(WavChunkHeader);
            header.factChunk.sampleCount = sampleCount;
            header.dataChunkHeader.id = WavId.WavData;
            header.dataChunkHeader.size = audioSize;
            return header;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sounds"></param>
        /// <param name="para">AudioIdx, AudioBag</param>
        public unsafe override void LocateSounds(string[] sounds, params object[] para)
        {
            AudioBag bag = (AudioBag)para[0];
            //FileLocation fl = (FileLocation)para[1];
            bagFile = (ResourceLocation)para[1];
            //ArchiveBinaryReader br = new ArchiveBinaryReader(fl);

            //this.sounds = new FMOD.Sound[sounds.Length];
            this.sounds = new AudioIdxEntry[sounds.Length];
            //long oldp = br.BaseStream.Position;

            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].StartsWith("$"))
                    sounds[i] = sounds[i].Remove(0, 1);
                if (!string.IsNullOrEmpty(sounds[i]))
                {
                    try
                    {
                        this.sounds[i] = bag.Data[sounds[i]];
                        size += this.sounds[i].size;
                    }
                    catch (KeyNotFoundException)
                    {
                        GameConsole.Instance.Write(ResourceAssembly.Instance.CM_SoundNotFound(sounds[i]), ConsoleMessageType.Warning);
                        continue;
                    }
                }
            }  // for

        }

        [Obsolete()]
        public void Test(string name, byte[] bytes)
        {
            FileStream fs = new FileStream(@"C:\Documents and Settings\Yuri\桌面\" + name + "2.wav", FileMode.OpenOrCreate);

            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(bytes);

            bw.Close();
        }

        public override int GeiSize()
        {
            return size;
        }

        protected unsafe override void load()
        {
            ArchiveBinaryReader br = new ArchiveBinaryReader(bagFile);
            long oldp = br.BaseStream.Position;

            fmodSounds = new FMOD.Sound[sounds.Length];

            for (int i = 0; i < sounds.Length; i++)
            {
                if (sounds[i].size != 0)
                {
                    //AudioIdxEntry entry;
                    //try
                    //{
                    //    entry = bag.Data[sounds[i]];
                    //}
                    //catch (KeyNotFoundException)
                    //{
                    //    GameConsole.Instance.Write(ResourceAssembly.Instance.CM_SoundNotFound(sounds[i]), ConsoleMessageType.Warning);
                    //    continue;
                    //}

                    br.BaseStream.Position = oldp + sounds[i].offset;
                    byte[] buffer = br.ReadBytes(sounds[i].size);

                    int chanelCount = (sounds[i].flags & 1) != 0 ? 2 : 1;

                    if ((sounds[i].flags & 2) != 0)
                    {
                        int sampleCount = sounds[i].size / chanelCount >> 1;

                        WavHeader h = BuildWavHeader(sampleCount, (short)sounds[i].sampleRate, 2, (short)chanelCount);
                        WavHeader* hptr = &h;

                        int dataSize = sounds[i].size + sizeof(WavHeader);
                        byte[] data = new byte[dataSize];

                        fixed (byte* dst = &data[0])
                        {
                            Memory.Copy(hptr, dst, sizeof(WavHeader));
                        }

                        Buffer.BlockCopy(buffer, 0, data, sizeof(WavHeader), buffer.Length);


                        FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
                        exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
                        exinfo.length = (uint)dataSize;

                        soundSystem.createSound(data, FMOD.MODE.CREATESAMPLE | FMOD.MODE.LOWMEM | FMOD.MODE._2D | FMOD.MODE.DEFAULT | FMOD.MODE.SOFTWARE | FMOD.MODE.OPENMEMORY, ref exinfo, ref this.fmodSounds[i]);
                    }
                    else
                    {
                        //int audioSize = entry.size - 0x1EC;
                        int chunkCount = (sounds[i].size + sounds[i].chunkSize - 1) / sounds[i].chunkSize;
                        int sampleCount = (sounds[i].size - sizeof(ImaAdpcmChunkHeader) * chanelCount * chunkCount << 1) + chunkCount * chanelCount;
                        sampleCount /= chanelCount;

                        WavImaAdpcmHeader h = BuildWavImaAdpcmHeader(sounds[i].size, sampleCount, sounds[i].sampleRate, chanelCount);
                        WavImaAdpcmHeader* hptr = &h;

                        int dataSize = sounds[i].size + sizeof(WavImaAdpcmHeader);
                        byte[] data = new byte[dataSize];

                        fixed (byte* dst = &data[0])
                        {
                            Memory.Copy(hptr, dst, sizeof(WavImaAdpcmHeader));
                        }
                        Buffer.BlockCopy(buffer, 0, data, sizeof(WavImaAdpcmHeader), buffer.Length);

                        FMOD.CREATESOUNDEXINFO exinfo = new FMOD.CREATESOUNDEXINFO();
                        exinfo.cbsize = System.Runtime.InteropServices.Marshal.SizeOf(exinfo);
                        exinfo.length = (uint)dataSize;

                        soundSystem.createSound(data, FMOD.MODE.CREATECOMPRESSEDSAMPLE | FMOD.MODE.LOWMEM | FMOD.MODE._2D | FMOD.MODE.DEFAULT | FMOD.MODE.SOFTWARE | FMOD.MODE.OPENMEMORY, ref exinfo, ref this.fmodSounds[i]);
                    } // if
                }
            }
            br.Close();
        }

        protected override void unload()
        {
            for (int i = 0; i < fmodSounds.Length; i++)
            {
                if (fmodSounds[i] != null)
                {
                    fmodSounds[i].release();
                }
            }

            fmodSounds = null;
        }

        public override bool IsUnloadable
        {
            get { return isUnloadable; }
        }

    }
}