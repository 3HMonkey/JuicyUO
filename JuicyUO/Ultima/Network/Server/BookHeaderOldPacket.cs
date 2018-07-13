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
    public class BookHeaderOldPacket : RecvPacket
    {
        /*
             Packet Name: Book Header ( Old )
            Last Modified: 2010-01-08 02:06:00
            Modified By: Tomi

            Packet: 0x93
            Sent By: Both
            Size: 99 Bytes

            Packet Build
            BYTE[1] 0x93
            BYTE[4] Book Serial
            BYTE[1] Write Flag (see notes)
            BYTE[1] 0x1 (unknown)
            BYTE[2] Page Count
            BYTE[60] Title
            BYTE[30] Author

            Subcommand Build
            N/A

            Notes
            Write Flag
            0: Not Writable
            1: Writable

            Server version of packet is followed by packet 0x66 for Book Contents.

            Client sends a 0x93 message on book close. Update packet for the server to handle changes. Write Flag through Page Count are all 0's on client response
         */

        public BookHeaderOldPacket(PacketReader reader)
            : base(0x93, "Book Header (Old)")
        {

        }
    }
}
