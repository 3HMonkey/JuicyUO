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

using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Network;

namespace JuicyUO.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x06: Party info.
    /// </summary>
    class PartyInfo : IGeneralInfo {
        public const byte CommandPartyList = 0x01;
        public const byte CommandRemoveMember = 0x02;
        public const byte CommandPrivateMessage = 0x03;
        public const byte CommandPublicMessage = 0x04;
        public const byte CommandInvitation = 0x07;

        public readonly byte SubsubCommand;
        public readonly IGeneralInfo Info;

        public PartyInfo(PacketReader reader) {
            SubsubCommand = reader.ReadByte();
            switch (SubsubCommand) {
                case CommandPartyList:
                    Info = new PartyMemberListInfo(reader);
                    break;
                case CommandRemoveMember:
                    Info = new PartyRemoveMemberInfo(reader);
                    break;
                case CommandPrivateMessage:
                    Info = new PartyMessageInfo(reader, true);
                    break;
                case CommandPublicMessage:
                    Info = new PartyMessageInfo(reader, false);
                    break;
                case CommandInvitation://PARTY INVITE PROGRESS
                    Info = new PartyInvitationInfo(reader);
                    break;
                default:
                    Tracer.Warn($"Unhandled Subsubcommand {SubsubCommand:X2} in PartyInfo.");
                    break;
            }
        }
    }
}
