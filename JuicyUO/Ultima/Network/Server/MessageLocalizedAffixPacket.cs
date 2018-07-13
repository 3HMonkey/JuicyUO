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
using JuicyUO.Ultima.Data;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class MessageLocalizedAffixPacket : RecvPacket
    {
        public bool IsSystemMessage { get { return (Serial == 0xFFFF); } }
        public readonly Serial Serial;
        public readonly int Body;
        public readonly MessageTypes MessageType;
        public readonly ushort Hue;
        public readonly int Font;
        public readonly int CliLocNumber;
        public readonly string SpeakerName;
        public readonly string Arguements;
        public readonly string Affix;

        private readonly byte flags;
        public bool Flag_IsPrefix { get { return (flags & 0x01) != 0x00; } }
        public bool Flag_IsSystem { get { return (flags & 0x02) != 0x00; } }
        public bool Flag_MessageDoesNotMove { get { return (flags & 0x04) != 0x00; } }

        public MessageLocalizedAffixPacket(PacketReader reader)
            : base(0xCC, "Message Localized Affix")
        {
            Serial = reader.ReadInt32(); // 0xffff for system message
            Body = reader.ReadInt16(); // (0xff for system message
            MessageType = (MessageTypes)reader.ReadByte(); // 6 - lower left, 7 on player
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            CliLocNumber = reader.ReadInt32();
            flags = reader.ReadByte();
            SpeakerName = reader.ReadString(30);
            Affix = reader.ReadStringSafe();
            Arguements = reader.ReadUnicodeStringSafeReverse();
        }
    }
}
