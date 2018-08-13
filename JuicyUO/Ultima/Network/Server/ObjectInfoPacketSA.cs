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
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class ObjectInfoPacketSA : RecvPacket
    {
        public readonly ushort OsiID;
        public readonly sbyte DataType;
        public readonly Serial Serial;
        public readonly ushort ItemID;
        public readonly sbyte Facing;
        public readonly ushort Amount;
        public readonly ushort Unknown;
        public readonly short X;
        public readonly short Y;
        public readonly sbyte Z;
        public readonly sbyte Layer;
        public readonly ushort Hue;
        public readonly byte Flags;
        public readonly ushort Unknown2;

        public ObjectInfoPacketSA(PacketReader reader)
            : base(0xF3, "ObjectInfoPacketSA")
        {
            OsiID = reader.ReadUInt16();
            DataType = reader.ReadSByte();
            Serial = reader.ReadInt32();
            ItemID = reader.ReadUInt16();
            Facing = reader.ReadSByte();
            Amount = reader.ReadUInt16();
            Unknown = reader.ReadUInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Z = reader.ReadSByte();
            Layer = reader.ReadSByte();
            Hue = reader.ReadUInt16();
            Flags = reader.ReadByte();
            Unknown2 = reader.ReadUInt16();

            // sanitize values
            Serial = (int)(Serial & 0x7FFFFFFF);
            ItemID = (ushort)(ItemID & 0x7FFF);
            X = (short)(X & 0x7FFF);
            Y = (short)(Y & 0x3FFF);

            //Tracer.Info("###################### " + Serial);
        }
    }
}
