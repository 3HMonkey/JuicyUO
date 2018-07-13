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
using JuicyUO.Ultima.World.Entities;
#endregion

namespace JuicyUO.Ultima.Network.Client
{
    public class TargetObjectPacket : SendPacket
    {
        public TargetObjectPacket(AEntity entity, int cursorID, byte cursorType)
            : base(0x6C, "Target Object", 19)
        {
            Stream.Write((byte)0x00); // BYTE[1] type: 0x00 = Select Object; 0x01 = Select X, Y, Z
            Stream.Write(cursorID); // BYTE[4] cursorID 
            Stream.Write(cursorType); // BYTE[1] Cursor Type
            Stream.Write((int)entity.Serial); // BYTE[4] Clicked On ID. Not used in this packet.
            Stream.Write((short)entity.X); // BYTE[2] click xLoc
            Stream.Write((short)entity.Y); // BYTE[2] click yLoc
            Stream.Write((byte)0x00); // BYTE unknown (0x00)
            Stream.Write((byte)entity.Z); // BYTE click zLoc
            Stream.Write((short)0); // BYTE[2] model # (if a static tile, 0 if a map/landscape tile)
        }
    }
}
