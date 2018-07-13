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

using JuicyUO.Core.Network;

namespace JuicyUO.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x06 / 0x03 and 0x06 / 0x04: Party message.
    /// </summary>
    public class PartyMessageInfo : IGeneralInfo {
        public readonly bool IsPrivate;
        public readonly Serial Source;
        public readonly string Message;

        public PartyMessageInfo(PacketReader reader, bool isPrivate) {
            IsPrivate = isPrivate;
            Source = (Serial)reader.ReadInt32();
            Message = reader.ReadUnicodeString();
        }
    }
}
