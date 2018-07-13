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
    public class DragEffectPacket : RecvPacket
    {
        public readonly int ItemId;
        public readonly int Amount;
        public readonly Serial Source;
        public readonly int SourceX;
        public readonly int SourceY;
        public readonly int SourceZ;
        public readonly Serial Destination;
        public readonly int DestX;
        public readonly int DestY;
        public readonly int DestZ;

        public DragEffectPacket(PacketReader reader)
            : base(0x23, "Dragging Item")
        {
            ItemId = reader.ReadUInt16();
            reader.ReadByte(); // 0x03 bytes unknown.
            reader.ReadByte(); //
            reader.ReadByte(); //
            Amount = reader.ReadUInt16();
            Source = reader.ReadInt32(); // 0x00000000 or 0xFFFFFFFF for ground
            SourceX = reader.ReadUInt16();
            SourceY = reader.ReadUInt16();
            SourceZ = reader.ReadByte();
            Destination = reader.ReadInt32(); // 0x00000000 or 0xFFFFFFFF for ground
            DestX = reader.ReadUInt16();
            DestY = reader.ReadUInt16();
            DestZ = reader.ReadByte();
        }
    }
}
