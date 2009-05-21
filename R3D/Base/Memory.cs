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

namespace R3D.Base
{
    public unsafe static class Memory
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct helper102400
        {
            public const int Size = 102400;
            fixed byte buffer[Size];
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct helper10240
        {
            public const int Size = 10240;
            fixed byte buffer[Size];
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct helper1024
        {
            public const int Size = 1024;
            fixed byte buffer[Size];
        }

        public static void Copy(void* sr, void* dst, int size)
        {
            byte* dest = (byte*)dst;
            byte* src = (byte*)sr;

            if (size >= helper102400.Size)
            {
                do
                {
                    *((helper102400*)dest) = *((helper102400*)src);
                    dest += helper102400.Size;
                    src += helper102400.Size;
                }
                while ((size -= helper102400.Size) >= helper102400.Size);
            }
            if (size >= helper10240.Size)
            {
                do
                {
                    *((helper10240*)dest) = *((helper10240*)src);
                    dest += helper10240.Size;
                    src += helper10240.Size;
                }
                while ((size -= helper10240.Size) >= helper10240.Size);
            }
            if (size >= helper1024.Size)
            {
                do
                {
                    *((helper1024*)dest) = *((helper1024*)src);
                    dest += helper1024.Size;
                    src += helper1024.Size;
                }
                while ((size -= helper1024.Size) >= helper1024.Size);
            }
            if (size >= 16)
            {
                do
                {
                    *((long*)dest) = *((long*)src);
                    *((long*)(dest + 8)) = *((long*)(src + 8));
                    dest += 16;
                    src += 16;
                }
                while ((size -= 16) >= 16);
            }
            if ((size & 8) != 0)
            {
                *((long*)dest) = *((long*)src);
                dest += 8;
                src += 8;
            }
            if ((size & 4) != 0)
            {
                *((int*)dest) = *((int*)src);
                dest += 4;
                src += 4;
            }
            if ((size & 2) != 0)
            {
                *((short*)dest) = *((short*)src);
                dest += 2;
                src += 2;
            }
            if ((size & 1) != 0)
            {
                dest++;
                src++;
                dest[0] = src[0];
            }
        }
        public static void Zero(void* p, int size)
        {
            byte* src = (byte*)p;
            while (true)
            {
                size--;
                if (size <= 0)
                {
                    return;
                }
                src[size] = 0;
            }

        }

    }
}
