#region license
//  Copyright (C) 2018 JuicyUO Development Community on Github
//
//	This project is an alternative client for the game Ultima Online.
//	The goal of this is to develop a lightweight client considering 
//	new technologies such as DirectX (MonoGame included). The foundation
//	is originally licensed (GNU) on JuicyUO and the JuicyUO Development
//	Team. (Copyright (c) 2015 JuicyUO Development Team)
//    
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Collections.Generic;

namespace JuicyUO.Ultima.Resources
{
    class PixelPicking
    {
        const int InitialDataCount = 0x40000; // 256kb

        Dictionary<int, int> m_IDs = new Dictionary<int, int>();
        readonly List<byte> m_Data = new List<byte>(InitialDataCount); // list<t> access is 10% slower than t[].

        public bool Get(int textureID, int x, int y, int extraRange = 0)
        {
            int index;
            if (!m_IDs.TryGetValue(textureID, out index))
            {
                return false;
            }
            int width = ReadIntegerFromData(ref index);
            if (x < 0 || x >= width)
            {
                return false;
            }
            int height = ReadIntegerFromData(ref index);
            if (y < 0 || y >= height)
            {
                return false;
            }
            int current = 0;
            int target = x + y * width;
            bool inTransparentSpan = true;
            while (current < target)
            {
                int spanLength = ReadIntegerFromData(ref index);
                current += spanLength;
                if (extraRange == 0)
                {
                    if (target < current)
                    {
                        return !inTransparentSpan;
                    }
                }
                else
                {
                    if (!inTransparentSpan)
                    {
                        int y0 = current / width;
                        int x1 = current % width;
                        int x0 = x1 - spanLength;
                        for (int range = -extraRange; range <= extraRange; range++)
                        {
                            if (y + range == y0 && (x + extraRange >= x0) && (x - extraRange <= x1))
                            {
                                return true;
                            }
                        }
                    }
                }
                inTransparentSpan = !inTransparentSpan;
            }
            return false;
        }

        public void GetDimensions(int textureID, out int width, out int height)
        {
            int index;
            if (!m_IDs.TryGetValue(textureID, out index))
            {
                width = height = 0;
                return;
            }
            width = ReadIntegerFromData(ref index);
            height = ReadIntegerFromData(ref index);
        }

        public void Set(int textureID, int width, int height, ushort[] pixels)
        {
            int begin = m_Data.Count;
            WriteIntegerToData(width);
            WriteIntegerToData(height);
            bool countingTransparent = true;
            int count = 0;
            for (int i = 0; i < pixels.Length; i++)
            {
                bool isTransparent = pixels[i] == 0;
                if (countingTransparent != isTransparent)
                {
                    WriteIntegerToData(count);
                    countingTransparent = !countingTransparent;
                    count = 0;
                }
                count += 1;
            }
            WriteIntegerToData(count);
            m_IDs[textureID] = begin;
        }

        bool Has(int textureID)
        {
            return m_IDs.ContainsKey(textureID);
        }

        void WriteIntegerToData(int value)
        {
            while (value > 0x7f)
            {
                m_Data.Add((byte)((value & 0x7f) | 0x80));
                value >>= 7;
            }
            m_Data.Add((byte)value);
        }

        int ReadIntegerFromData(ref int index)
        {
            int value = 0;
            int shift = 0;
            while (true)
            {
                byte data = m_Data[index++];
                value += (data & 0x7f) << shift;
                if ((data & 0x80) == 0x00)
                {
                    return value;
                }
                shift += 7;
            }
        }
    }
}
