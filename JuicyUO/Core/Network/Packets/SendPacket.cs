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

#region usings
using System.IO;
#endregion

namespace JuicyUO.Core.Network.Packets
{
    /// <summary>
    /// A formatted unit of data used in point to point communications.  
    /// </summary>
    public abstract class SendPacket : ISendPacket
    {
        private const int BufferSize = 4096;

        /// <summary>
        /// Used to create the a buffered datablock to be sent
        /// </summary>
        protected PacketWriter Stream;
        readonly int m_Id;
        int m_Length;
        readonly string m_Name;

        /// <summary>
        /// Gets the name of the packet
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets the size in bytes of the packet
        /// </summary>
        public int Length
        {
            get { return m_Length; }
        }

        /// <summary>
        /// Gets the Id, or Command that identifies the packet.
        /// </summary>
        public int Id
        {
            get { return m_Id; }
        }

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        public SendPacket(int id, string name)
        {
            m_Id = id;
            m_Name = name;
            Stream = PacketWriter.CreateInstance(m_Length);
            Stream.Write((byte)id);
            Stream.Write((short)0);
        }

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        /// <param name="length">The size in bytes of the packet</param>
        public SendPacket(int id, string name, int length)
        {
            m_Id = id;
            m_Name = name;
            m_Length = length;

            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)id);
        }

        /// <summary>
        /// Resets the Packet Writer and ensures the packet's 2nd and 3rd bytes are used to store the length
        /// </summary>
        /// <param name="length"></param>
        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)m_Id);
            Stream.Write((short)length);
        }

        /// <summary>
        /// Compiles the packet into a System.Byte[] and Disposes the underlying Stream
        /// </summary>
        /// <returns></returns>
        public byte[] Compile()
        {
            Stream.Flush();

            if (Length == 0)
            {
                m_Length = (int)Stream.Length;
                Stream.Seek((long)1, SeekOrigin.Begin);
                Stream.Write((ushort)m_Length);
            }

            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", m_Id, m_Name, m_Length);
        }
    }
}
