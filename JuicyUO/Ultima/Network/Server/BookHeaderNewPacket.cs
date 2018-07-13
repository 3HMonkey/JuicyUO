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
    public class BookHeaderNewPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly byte Flag0;
        public readonly byte Flag1;
        public readonly short Pages;
        public readonly short AuthorLength;
        public readonly string Author;
        public readonly short TitleLength;
        public readonly string Title;

        public BookHeaderNewPacket(PacketReader reader)
            : base(0xD4, "Book Header (New)")
        {
            Serial = reader.ReadInt32();
            Flag0 = reader.ReadByte();
            Flag1 = reader.ReadByte();
            Pages = reader.ReadInt16();
            TitleLength = reader.ReadInt16();
            Title = reader.ReadString();
            AuthorLength = reader.ReadInt16();
            Author = reader.ReadString();
        }
    }
}
