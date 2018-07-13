﻿#region license
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

namespace JuicyUO.Core.IO
{
    public class BufferPool
    {
        private static List<BufferPool> m_Pools = new List<BufferPool>();

        public static List<BufferPool> Pools { get { return m_Pools; } set { m_Pools = value; } }

        private string m_Name;

        private int m_InitialCapacity;
        private int m_BufferSize;

        private int m_Misses;
        private Queue<byte[]> m_FreeBuffers;

        public void GetInfo(out string name, out int freeCount, out int initialCapacity, out int currentCapacity, out int bufferSize, out int misses)
        {
            lock (this)
            {
                name = m_Name;
                freeCount = m_FreeBuffers.Count;
                initialCapacity = m_InitialCapacity;
                currentCapacity = m_InitialCapacity * (1 + m_Misses);
                bufferSize = m_BufferSize;
                misses = m_Misses;
            }
        }

        public BufferPool(string name, int initialCapacity, int bufferSize)
        {
            m_Name = name;

            m_InitialCapacity = initialCapacity;
            m_BufferSize = bufferSize;

            m_FreeBuffers = new Queue<byte[]>(initialCapacity);

            for (int i = 0; i < initialCapacity; ++i)
                m_FreeBuffers.Enqueue(new byte[bufferSize]);

            lock (m_Pools)
                m_Pools.Add(this);
        }
        
        public byte[] AcquireBuffer()
        {
            lock (this)
            {
                if (m_FreeBuffers.Count > 0)
                    return m_FreeBuffers.Dequeue();

                ++m_Misses;

                for (int i = 0; i < m_InitialCapacity; ++i)
                    m_FreeBuffers.Enqueue(new byte[m_BufferSize]);

                return m_FreeBuffers.Dequeue();
            }
        }

        public void ReleaseBuffer(byte[] buffer)
        {
            if (buffer == null)
                return;

            lock (this)
                m_FreeBuffers.Enqueue(buffer);
        }

        public void Free()
        {
            lock (m_Pools)
                m_Pools.Remove(this);
        }
    }
}
