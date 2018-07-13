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
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class MobileAnimationPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_action;
        readonly short m_framecount;
        readonly short m_repeatcount;
        readonly byte m_reverse;
        readonly byte m_repeat;
        readonly byte m_delay;

        public Serial Serial
        {
            get { return m_serial; } 
        }

        public short Action
        {
            get { return m_action; }
        }

        public short FrameCount
        {
            get { return m_framecount; }
        }

        public short RepeatCount
        {
            get { return m_repeatcount; }
        }

        public bool Reverse 
        {
            get { return (m_reverse == 1); }
        }

        public bool Repeat
        {
            get { return (m_repeat == 1); } 
        }

        public byte Delay
        {
            get { return m_delay; }
        }

        public MobileAnimationPacket(PacketReader reader)
            : base(0x6E, "Mobile Animation")
        {
            m_serial = reader.ReadInt32();
            m_action = reader.ReadInt16();
            m_framecount = reader.ReadInt16();
            m_repeatcount = reader.ReadInt16();
            m_reverse = reader.ReadByte(); // 0x00=forward, 0x01=backwards
            m_repeat = reader.ReadByte(); // 0 - Don't repeat / 1 repeat
            m_delay = reader.ReadByte();
        }
    }
}
