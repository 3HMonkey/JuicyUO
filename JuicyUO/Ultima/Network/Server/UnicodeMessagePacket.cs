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
    public class UnicodeMessagePacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly short Model;
        public readonly MessageTypes MsgType;
        public readonly ushort Hue;
        public readonly short Font;
        public readonly string Language;
        public readonly string SpeakerName;
        public readonly string Text;
        
        public UnicodeMessagePacket(PacketReader reader)
            : base(0xAE, "Unicode Message")
        {
            Serial = reader.ReadInt32();
            Model = reader.ReadInt16();
            MsgType = (MessageTypes)reader.ReadByte();
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            Language = reader.ReadString(4).Trim();
            SpeakerName = reader.ReadString(30).Trim();
            Text = reader.ReadUnicodeString((reader.Buffer.Length - 48) / 2);
        }
    }
}
