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

#region usings
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class UOGInfoResponsePacket : RecvPacket
    {
        readonly string m_name;
        readonly int m_age;
        readonly int m_clientCount;
        readonly int m_itemCount;
        readonly int m_mobileCount;
        readonly string m_memory;

        public int Age
        {
            get { return m_age; }
        }

        public int ClientCount
        {
            get { return m_clientCount; }
        }

        public int ItemCount
        {
            get { return m_itemCount; }
        }

        public int MobileCount
        {
            get { return m_mobileCount; }
        }

        public string ServerName
        {
            get { return m_name; }
        }

        public string Memory
        {
            get { return m_memory; }
        }

        public UOGInfoResponsePacket(PacketReader reader)
            : base(0x52, "UOG Information Response")
        {
            string response = reader.ReadString();
            string[] parts = response.Split(',');

            for (int j = 0; j < parts.Length; j++)
            {
                string[] keyValue = parts[j].Split('=');

                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    switch (key)
                    {
                        case "Name":
                            m_name = value;
                            break;
                        case "Age":
                            m_age = int.Parse(value);
                            break;
                        case "Clients":
                            m_clientCount = int.Parse(value) - 1;
                            break;
                        case "Items":
                            m_itemCount = int.Parse(value);
                            break;
                        case "Chars":
                            m_mobileCount = int.Parse(value);
                            break;
                        case "Mem":
                            m_memory = value;
                            break;
                    }
                }
            }
        }
    }
}
