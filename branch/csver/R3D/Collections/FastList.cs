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

namespace R3D.Collections
{
    public class FastList<T>
    {
        public T[] Elements;

        int internalPointer;

        int length;

        public FastList()
        {
            Elements = new T[4];
            length = 4;
        }

        public FastList(int elementsCount)
        {
            Elements = new T[elementsCount];
            length = elementsCount;
        }

        public T this[int i]
        {
            get { return Elements[i]; }
            set { Elements[i] = value; }
        }

        public void Add(T Data)
        {
            if (length <= internalPointer)
            {
                this.Resize(length == 0 ? 4 : (length * 2));
            }
            Elements[internalPointer++] = Data;
        }
        public void Add(ref T Data)
        {
            if (length <= internalPointer)
            {
                this.Resize(length == 0 ? 4 : (length * 2));
            }
            Elements[internalPointer++] = Data;
        }

        public void Add(T[] data)
        {
            int addL = internalPointer + data.Length;

            if (length <= addL)
            {
                int twoL = length * 2;

                this.Resize(twoL > addL ? twoL : addL);
            }
            int len = data.Length;
            Array.Copy(data, 0, Elements, internalPointer, len);
            internalPointer += len;
        }
        public void Add(FastList<T> data)
        {
            int addL = internalPointer + data.Count;
            if (length <= addL)
            {
                int twoL = length * 2;
                this.Resize(twoL > addL ? twoL : addL);
            }
            int len = data.Count;
            Array.Copy(data.Elements, 0, Elements, internalPointer, len);
            internalPointer += len;
        }

        public void FastClear()
        {
            internalPointer = 0;
        }
        public void Clear()
        {
            Array.Clear(Elements, 0, internalPointer);
            internalPointer = 0;
        }
        public void Resize(int newSize)
        {
            T[] destinationArray = new T[newSize];
            Array.Copy(Elements, destinationArray, internalPointer);
            Elements = destinationArray;
            length = newSize;
        }

        public int IndexOf(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (Elements[i] == null)
                    {
                        return i;
                    }
                }
                return -1;
            }
            else
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;

                for (int i = 0; i < Count; i++)
                {
                    if (comparer.Equals(Elements[i], item))
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
        public void Remove(T item)
        {
            int index = IndexOf(item);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        public void RemoveAt(int idx)
        {
            if (idx == internalPointer - 1)
            {
                internalPointer--;
                Elements[idx] = default(T);
            }
            else
            {
                T[] destinationArray = new T[length - 1];
                Array.Copy(Elements, 0, destinationArray, 0, idx);

                if (Count - ++idx > 0)
                {
                    Array.Copy(Elements, idx, destinationArray, idx - 1, Count - idx);
                }

                Elements = destinationArray;

                length--;
                internalPointer--;
            }
        }

        public int Count
        {
            get
            {
                return internalPointer;
            }
        }

        public override string ToString()
        {
            return "Count: " + Count.ToString();
        }

        public bool Contains(T item)
        {
            if (item == null)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (Elements[i] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            else
            {
                EqualityComparer<T> comparer = EqualityComparer<T>.Default;

                for (int i = 0; i < Count; i++)
                {
                    if (comparer.Equals(Elements[i], item))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
