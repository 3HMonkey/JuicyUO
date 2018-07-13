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

using System;

namespace JuicyUO.Core.Network
{

    class PacketChunk
    {
        readonly byte[] m_Buffer;
        int m_Length;

        public PacketChunk(byte[] buffer)
        {
            m_Buffer = buffer;
        }

        public int Length
        {
            get { return m_Length; }
        }

        public void Write(byte[] source, int offset, int length)
        {
            Buffer.BlockCopy(source, offset, m_Buffer, m_Length, length);

            m_Length += length;
        }

        public void Prepend(byte[] dest, int length)
        {
            // Offset the intial buffer by the amount we need to prepend
            if (length > 0)
            {
                Buffer.BlockCopy(dest, 0, dest, m_Length, length);
            }

            // Prepend the buffer to the destination buffer
            Buffer.BlockCopy(m_Buffer, 0, dest, 0, m_Length);
        }

        public void Clear()
        {
            m_Length = 0;
        }
    }
}
