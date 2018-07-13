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
    public class BookPageChangePacket : SendPacket
    {
        public BookPageChangePacket(Serial serial, int page, string[] lines)
            : base(0x66, "Book Page Change")
        {
            Stream.Write(serial);
            Stream.Write((short)1); // Page count always 1
            Stream.Write((short)(page + 1)); // Page number
            Stream.Write((short)lines.Length); // Number of lines
            // Send each line of the page
            for (int i = 0; i < lines.Length; i++)
            {
                Stream.WriteUTF8Null(lines[i]);
            }
        }
    }
}
