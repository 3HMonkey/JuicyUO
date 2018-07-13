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
using System.Collections.Generic;
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class MobileIncomingPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_body;        
        readonly short m_x;
        readonly short m_y;
        readonly short m_z;
        readonly byte m_direction;
        readonly ushort m_hue;
        public readonly MobileFlags Flags;
        readonly byte m_notoriety;
        readonly EquipmentEntry[] m_equipment;

        public Serial Serial
        {
            get { return m_serial; } 
        }

        public short BodyID 
        {
            get { return m_body; }
        }

        public short X
        {
            get { return m_x; } 
        }

        public short Y 
        {
            get { return m_y; } 
        }

        public short Z 
        {
            get { return m_z; } 
        }

        public byte Direction
        {
            get { return m_direction; } 
        }

        public ushort Hue 
        {
            get { return m_hue; } 
        }

        public EquipmentEntry[] Equipment
        {
            get { return m_equipment; }
        }

        /// <summary>
        /// 0x1: Innocent (Blue)
        /// 0x2: Friend (Green)
        /// 0x3: Grey (Grey - Non Criminal)
        /// 0x4: Criminal (Grey)
        /// 0x5: Enemy (Orange)
        /// 0x6: Murderer (Red)
        /// 0x7: Invulnerable (Yellow)
        /// </summary>
        public byte Notoriety
        {
            get { return m_notoriety; }
        }  

        public MobileIncomingPacket(PacketReader reader)
            : base(0x78, "Mobile Incoming")
        {
            // Mobile
            m_serial = reader.ReadInt32();
            m_body = reader.ReadInt16();
            m_x = reader.ReadInt16();
            m_y = reader.ReadInt16();
            m_z = reader.ReadSByte();
            m_direction = reader.ReadByte();
            m_hue = reader.ReadUInt16();
            Flags = new MobileFlags((MobileFlag)reader.ReadByte());
            m_notoriety = reader.ReadByte();

            // Read equipment - nine bytes ea.
            List<EquipmentEntry> items = new List<EquipmentEntry>();

            Serial serial = reader.ReadInt32();
            if (!serial.IsValid)
            {
                reader.ReadByte(); //zero terminated
                m_equipment = new EquipmentEntry[0];
            }
            else
            {
                while (serial.IsValid)
                {
                    ushort gumpId = reader.ReadUInt16();
                    byte layer = reader.ReadByte();
                    ushort hue = 0;

                    if ((gumpId & 0x8000) == 0x8000)
                    {
                        gumpId = (ushort)((int)gumpId - 0x8000);
                        hue = reader.ReadUInt16();
                    }

                    items.Add(new EquipmentEntry(serial, gumpId, layer, hue));
                    // read the next serial and begin the loop again. break at 0x00000000
                    serial = reader.ReadInt32();
                }
                m_equipment = items.ToArray();
            }
        }
    }
}
