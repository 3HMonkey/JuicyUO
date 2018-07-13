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
using System;
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    // RunUO (Packets.cs:3402)
    public enum LoginRejectionReasons : byte
    {
        InvalidAccountPassword = 0x00,
        AccountInUse = 0x01,
        AccountBlocked = 0x02,
        BadPassword = 0x03,
        IdleExceeded = 0xFE,
        BadCommuncation = 0xFF
    }

    public class LoginRejectionPacket : RecvPacket
    {
        private static Tuple<int, string>[] m_Reasons = {
            new Tuple<int, string>(0x00, "Incorrect username and/or password."), 
            new Tuple<int, string>(0x01, "Someone is already using this account."),
            new Tuple<int, string>(0x02, "Your account has been blocked."),
            new Tuple<int, string>(0x03, "Your account credentials are invalid."),
            new Tuple<int, string>(0xFE, "Login idle period exceeded."),
            new Tuple<int, string>(0xFF, "Communication problem.")
        };
        readonly byte m_id;

        public string ReasonText
        {
            get
            {
                for (int i = 0; i < m_Reasons.Length; i++)
                {
                    if (m_Reasons[i].Item1 == m_id)
                    {
                        return m_Reasons[i].Item2;
                    }
                }
                return (m_Reasons[m_Reasons.Length - 1].Item2);
            }
        }

        public LoginRejectionReasons Reason
        {
            get { return (LoginRejectionReasons)m_id; }
        }

        public LoginRejectionPacket(PacketReader reader)
            : base(0x82, "Login Rejection")
        {
            m_id = reader.ReadByte();
        }
    }
}
