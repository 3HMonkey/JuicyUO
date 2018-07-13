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
using System.Collections.Generic;
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
using JuicyUO.Ultima.Data;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class ContainerContentPacket : RecvPacket
    {
        private ItemInContainer[] m_items;

        public ItemInContainer[] Items
        {
            get { return m_items; }
            set { m_items = value; }
        }

        public ContainerContentPacket(PacketReader reader)
            : base(0x3C, "Container ContentPacket")
        {
            int itemCount = reader.ReadUInt16();
            List<ItemInContainer> items = new List<ItemInContainer>(itemCount);

            bool PacketIsPre6017 = (reader.Buffer.Length == 5 + (19 * itemCount));

            for (int i = 0; i < itemCount; i++)
            {
                Serial serial = reader.ReadInt32();
                int iItemID = reader.ReadUInt16();
                int iUnknown = reader.ReadByte(); // signed, itemID offset. always 0 in RunUO.
                int iAmount = reader.ReadUInt16();
                int iX = reader.ReadInt16();
                int iY = reader.ReadInt16();
                int iGridLocation = 0;
                if (!PacketIsPre6017)
                    iGridLocation = reader.ReadByte(); // always 0 in RunUO.
                int iContainerSerial = reader.ReadInt32();
                int iHue = reader.ReadUInt16();

                items.Add(new ItemInContainer(serial, iItemID, iAmount, iX, iY, iGridLocation, iContainerSerial, iHue));
            }

            m_items = items.ToArray();
        }

        public bool AllItemsInSameContainer
        {
            get
            {
                if (Items.Length == 0)
                    return true;
                Serial containerSerial = Items[0].ContainerSerial;
                for (int i = 1; i < Items.Length; i++)
                    if (Items[i].ContainerSerial != containerSerial)
                        return false;
                return true;
            }
        }
    }
}
