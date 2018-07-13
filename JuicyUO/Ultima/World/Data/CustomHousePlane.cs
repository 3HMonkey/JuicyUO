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
using JuicyUO.Core.Network.Compression;

namespace JuicyUO.Ultima.World.Data {
    public class CustomHousePlane {
        public readonly int Index;
        public readonly bool IsFloor;
        public readonly byte[] ItemData;

        public CustomHousePlane(PacketReader reader) {
            byte[] data = reader.ReadBytes(4);
            Index = data[0];
            int uncompressedsize = data[1] + ((data[3] & 0xF0) << 4);
            int compressedLength = data[2] + ((data[3] & 0xF) << 8);
            ItemData = new byte[uncompressedsize];
            ZlibCompression.Unpack(ItemData, ref uncompressedsize, reader.ReadBytes(compressedLength), compressedLength);
            IsFloor = ((Index & 0x20) == 0x20);
            Index &= 0x1F;
        }
    }
}
