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
    public abstract class SendRecvPacket: ISendPacket, IRecvPacket
    {
        readonly int m_Id;
        readonly int m_Length;
        readonly string m_Name;

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public int Length
        {
            get { return m_Length; }
        }

        private const int BufferSize = 4096;

        protected PacketWriter Stream;

        public SendRecvPacket(int id, string name)
        {
            m_Id = id;
            m_Name = name;
            Stream = PacketWriter.CreateInstance(m_Length);
            Stream.Write(id);
            Stream.Write((short)0);
        }

        public SendRecvPacket(int id, string name, int length)
        {
            m_Id = id;
            m_Name = name;
            m_Length = length;

            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)id);
        }

        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)m_Id);
            Stream.Write((short)length);
        }

        public byte[] Compile()
        {
            Stream.Flush();

            if (Length == 0)
            {
                long length = Stream.Length;
                Stream.Seek((long)1, SeekOrigin.Begin);
                Stream.Write((ushort)length);
                Stream.Flush();
            }

            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", m_Id, m_Name, m_Length);
        }
    }
}
