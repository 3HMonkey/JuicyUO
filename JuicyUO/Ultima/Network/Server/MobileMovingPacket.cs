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
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class MobileMovingPacket : RecvPacket
    {
        readonly Serial serial;
        readonly ushort bodyid;
        readonly short x;
        readonly short y;
        readonly sbyte z;
        readonly byte direction;
        readonly ushort hue;
        public readonly MobileFlags Flags;
        readonly byte notoriety;

        public Serial Serial 
        {
            get { return serial; }
        }

        public ushort BodyID 
        {
            get { return bodyid; }
        }

        public short X
        {
            get { return x; }
        }

        public short Y 
        {
            get { return y; } 
        }

        public sbyte Z 
        {
            get { return z; } 
        }

        public byte Direction
        {
            get { return direction; }
        }

        public ushort Hue
        {
            get { return hue; }
        }

        public byte Notoriety
        {
            get { return notoriety; }
        }

        public MobileMovingPacket(PacketReader reader)
            : base(0x77, "Mobile Moving")
        {
            serial = reader.ReadInt32();
            bodyid = reader.ReadUInt16();
            x = reader.ReadInt16();
            y = reader.ReadInt16();
            z = reader.ReadSByte();
            direction = reader.ReadByte();
            hue = reader.ReadUInt16();
            Flags = new MobileFlags((MobileFlag)reader.ReadByte());
            notoriety = reader.ReadByte();
        }
    }
}
