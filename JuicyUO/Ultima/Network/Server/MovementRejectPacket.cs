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
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class MovementRejectPacket : RecvPacket
    {
        readonly byte m_sequence;
        readonly short m_x;
        readonly short m_y;
        readonly byte m_direction;
        readonly sbyte m_z;

        public byte Sequence 
        {
            get { return m_sequence; } 
        }
        
        public short X
        {
            get { return m_x; }
        }

        public short Y 
        {
            get { return m_y; }
        }
        
        public byte Direction
        {
            get { return m_direction; }
        }
        
        public sbyte Z 
        {
            get { return m_z; } 
        }

        public MovementRejectPacket(PacketReader reader)
            : base(0x21, "Move Request Rejected")
        {
            m_sequence = reader.ReadByte(); // (matches sent sequence)
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            m_direction = reader.ReadByte();
            m_z = reader.ReadSByte();
        }
    }
}
