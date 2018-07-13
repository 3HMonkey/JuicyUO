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
using JuicyUO.Core.Network.Packets;
#endregion

namespace JuicyUO.Ultima.Network.Client
{
    public class RequestCustomHousePacket : SendPacket
    {
        public RequestCustomHousePacket(Serial serial)
            : base(0xBF, "Request Custom House Packet")
        {
            Stream.Write((short)0x1E); // subcommand 0x1E, request custom house
            Stream.Write((int)serial);
        }
    }

    public class RequestContextMenuPacket : SendPacket
    {
        public RequestContextMenuPacket(Serial serial)
            : base(0xBF, "Context Menu Request")
        {
            Stream.Write((short)0x13); // subcommand 0x13, request context menu
            Stream.Write((int)serial);
        }
    }

    public class ContextMenuResponsePacket : SendPacket
    {
        public ContextMenuResponsePacket(Serial serial, short responseIndex)
            : base(0xBF, "Context Menu Response")
        {
            Stream.Write((short)0x15); // subcommand 0x15,  response to context menu
            Stream.Write((int)serial);
            Stream.Write((short)responseIndex);
        }
    }

    public class ReportClientScreenSizePacket : SendPacket
    {
        public ReportClientScreenSizePacket(int width, int height)
            : base(0xBF, "Report Screen Size Packet")
        {
            Stream.Write((short)0x05); // subcommand 0x05
            Stream.Write((ushort)width);
            Stream.Write((ushort)height);
            Stream.Write((ushort)0xFFA7); // unknown value. Might not always be this value.
        }
    }

    public class ReportClientLocalizationPacket : SendPacket
    {
        public ReportClientLocalizationPacket(string locale)
            : base(0xBF, "Report Client Localization Packet")
        {
            Stream.Write((short)0x0B); // subcommand 0x0B
            Stream.WriteAsciiNull(locale);
        }
    }

    public class RequestSpecialMovesPacket : SendPacket
    {
        public RequestSpecialMovesPacket()
            : base(0xBF, "Request Special Moves book")
        {
            Stream.Write((short)0x24);
            Stream.Write((byte)0x67);
        }
    }
}