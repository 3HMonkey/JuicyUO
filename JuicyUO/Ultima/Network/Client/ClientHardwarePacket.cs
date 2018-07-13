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
    class ClientHardwarePacket : SendPacket
    {
        public ClientHardwarePacket()
            : base(0xD9, "Client Version") {
            Stream.Write((byte)0x02); // BYTE[1] unknown(02) Always 0x02 in my tests
                                      /*
                                      BYTE[1] cmd

                                      BYTE[4] Unique Instance ID of UO
                                      BYTE[4] OS Major
                                      BYTE[4] OS Minor
                                      BYTE[4] OS Revision
                                      BYTE[1] CPU Manufacturer
                                      BYTE[4] CPU Family
                                      BYTE[4] CPU Model
                                      BYTE[4] CPU Clock Speed
                                      BYTE[1] CPU Quantity
                                      BYTE[4] Memory
                                      BYTE[4] Screen Width
                                      BYTE[4] Screen Height
                                      BYTE[4] Screen Depth
                                      BYTE[2] Direct X Version
                                      BYTE[2] Direct X Minor
                                      BYTE[76 ?] Video Card Description
                                       BYTE[4] Video Card Vendor ID
                                      BYTE[4] Video Card Device ID
                                      BYTE[4] Video Card Memory
                                      BYTE[1] Distribution
                                      BYTE[1] Clients Running
                                      BYTE[1] Clients Installed
                                      BYTE[1] Partial Insstalled
                                      BYTE[1] Unknown
                                      BYTE[4] Language Code
                                      BYTE[67] Unknown Ending*/
        }
    }
}
