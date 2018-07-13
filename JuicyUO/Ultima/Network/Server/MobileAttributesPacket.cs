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
    public class MobileAttributesPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_maxHits;
        readonly short m_currentHits;
        readonly short m_maxMana;
        readonly short m_currentMana;
        readonly short m_maxStamina;
        readonly short m_currentStamina;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public short MaxHits
        {
            get { return m_maxHits; }
        }

        public short CurrentHits
        {
            get { return m_currentHits; }
        }

        public short MaxMana
        {
            get { return m_maxMana; }
        }

        public short CurrentMana
        {
            get { return m_currentMana; }
        }

        public short MaxStamina
        {
            get { return m_maxStamina; }
        }

        public short CurrentStamina
        {
            get { return m_currentStamina; }
        }


        public MobileAttributesPacket(PacketReader reader)
            : base(0x2D, "Mobile Attributes")
        {
            m_serial = reader.ReadInt32();
            m_maxHits = reader.ReadInt16();
            m_currentHits = reader.ReadInt16();
            m_maxMana = reader.ReadInt16();
            m_currentMana = reader.ReadInt16();
            m_maxStamina = reader.ReadInt16();
            m_currentStamina = reader.ReadInt16();
        }
    }
}
