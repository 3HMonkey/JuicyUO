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

namespace JuicyUO.Core.Network {
    public abstract class PacketHandler {
        public readonly int ID;
        public readonly int Length;
        public readonly Type PacketType;
        public readonly object Client;

        public PacketHandler(int id, int length, Type packetType, object client) {
            ID = id;
            Length = length;
            PacketType = packetType;
            Client = client;
        }

        public abstract void Invoke(PacketReader reader);
    }

    public class PacketHandler<T> : PacketHandler where T : IRecvPacket {
        readonly Action<T> m_Handler;

        public PacketHandler(int id, int length, Type packetType, object client, Action<T> handler)
            : base(id, length, packetType, client) {
            m_Handler = handler;
        }
        
        public override void Invoke(PacketReader reader) {
            T packet = (T)Activator.CreateInstance(PacketType, new object[] { reader });
            m_Handler(packet);
        }
    }
}
