﻿#region license
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
    public class DisplayMenuPacket : RecvPacket
    {
        public readonly int BlockSize;          // BYTE[2]
        public readonly int DialogID;           // BYTE[4]
        public readonly int MenuID;             // BYTE[2]
        public readonly int LengthOfQuestion;   // BYTE[1]
        public readonly string QuestionText;    // BYTE[length of q]
        public readonly int NumberOfResponses;  // BYTE[1]

        public DisplayMenuPacket(PacketReader reader)
            : base(0x7C, "Display Menu")
        {
            // TODO: Write this packet.
            BlockSize = reader.ReadInt16();
            DialogID = reader.ReadInt32();
            MenuID = reader.ReadInt16();
            LengthOfQuestion = reader.ReadByte();
            QuestionText = reader.ReadString();
            NumberOfResponses = reader.ReadByte();

            System.Console.WriteLine(BlockSize);
        }
    }
}
