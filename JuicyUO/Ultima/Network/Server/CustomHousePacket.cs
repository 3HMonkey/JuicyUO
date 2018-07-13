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
using JuicyUO.Ultima.World.Data;
#endregion

namespace JuicyUO.Ultima.Network.Server {
    public class CustomHousePacket : RecvPacket {
        readonly Serial m_HouseSerial;
        public Serial HouseSerial => m_HouseSerial;

        readonly int m_RevisionHash;
        public int RevisionHash => m_RevisionHash;

        readonly int m_NumPlanes;
        public int PlaneCount => m_NumPlanes;

        readonly CustomHousePlane[] m_Planes;
        public CustomHousePlane[] Planes => m_Planes;

        public CustomHousePacket(PacketReader reader)
            : base(0xD8, "Custom House Packet") {
            byte CompressionType = reader.ReadByte();
            if (CompressionType != 3) {
                m_HouseSerial = Serial.Null;
                return;
            }
            reader.ReadByte(); // unknown, always 0?
            m_HouseSerial = reader.ReadInt32();
            m_RevisionHash = reader.ReadInt32();
            // this is for compression type 3 only
            int bufferLength = reader.ReadInt16();
            int trueBufferLength = reader.ReadInt16();
            m_NumPlanes = reader.ReadByte();
            // end compression type 3
            m_Planes = new CustomHousePlane[m_NumPlanes];
            for (int i = 0; i < m_NumPlanes; i++) {
                m_Planes[i] = new CustomHousePlane(reader);
            }
        }
    }
}
