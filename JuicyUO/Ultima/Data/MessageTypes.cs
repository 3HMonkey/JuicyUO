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

using System;

namespace JuicyUO.Ultima.Data
{
    [Flags]
    public enum MessageTypes
    {
        Normal = 0x00,
        System = 0x01,
        Emote = 0x02,
        SpeechUnknown = 0x03,
        Information = 0x04,     // Overhead information messages
        Label = 0x06,
        Focus = 0x07,
        Whisper = 0x08,
        Yell = 0x09,
        Spell = 0x0A,
        Guild = 0x0D,
        Alliance = 0x0E,
        Command = 0x0F,
        /// <summary>
        /// This is used for display only. This is not in the UO protocol. Do not send msgs of this type to the server.
        /// </summary>
        PartyDisplayOnly = 0x10,
        EncodedTriggers = 0xC0 // 0x40 + 0x80
    }
}
