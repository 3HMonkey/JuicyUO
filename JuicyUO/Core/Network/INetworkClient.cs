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
using System.Net;

namespace JuicyUO.Core.Network
{
    public interface INetworkClient
    {
        int ClientAddress
        {
            get;
        }

        IPAddress ServerAddress
        {
            get;
        }

        bool IsConnected
        {
            get;
        }

        bool IsDecompressionEnabled
        {
            get;
            set;
        }

        bool Connect(string ipAddressOrHostName, int port);
        void Disconnect();
        bool Send(ISendPacket packet);
        bool Send(byte[] buffer, int offset, int length, string name);
        void Slice();

        void Register<T>(object client, int id, int length, Action<T> onReceive) where T : IRecvPacket;
        void RegisterExtended<T>(object client, int extendedId, int subId, int length, Action<T> onReceive) where T : IRecvPacket;
        void Unregister(object client);
        void Unregister(object client, int id);
    }
}