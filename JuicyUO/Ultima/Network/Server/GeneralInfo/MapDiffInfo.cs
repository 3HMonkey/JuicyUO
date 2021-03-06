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

using JuicyUO.Core.Network;

namespace JuicyUO.Ultima.Network.Server.GeneralInfo {
    /// <summary>
    /// Subcommand 0x18: The count of map diffs that were received.
    /// As of 6.0.0.0, this is only used to inform the client of the number of active maps.
    /// </summary>
    public class MapDiffInfo : IGeneralInfo {
        public readonly int MapCount;
        public readonly int[] MapPatches;
        public readonly int[] StaticPatches;

        public MapDiffInfo(PacketReader reader) {
            MapCount = reader.ReadInt32();
            MapPatches = new int[MapCount];
            StaticPatches = new int[MapCount];
            for (int i = 0; i < MapCount; i++) {
                StaticPatches[i] = reader.ReadInt32();
                MapPatches[i] = reader.ReadInt32();
            }
        }
    }
}
