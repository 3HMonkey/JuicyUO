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
using System.Net.Sockets;
#endregion

namespace JuicyUO.Core.Network
{
    public class SocketState
    {
        private readonly Socket m_Socket;
        private byte[] m_Buffer;
        private int m_DataLength;

        public Socket Socket
        {
            get { return m_Socket; }
        }

        public byte[] Buffer
        {
            get { return m_Buffer; }
            set { m_Buffer = value; }
        }

        public int DataLength
        {
            get { return m_DataLength; }
            set { m_DataLength = value; }
        }

        public SocketState(Socket socket, byte[] buffer)
        {
            m_Socket = socket;
            m_Buffer = buffer;
            m_DataLength = 0;
        }
    }
}
