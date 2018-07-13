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
    public class VendorSellListPacket : RecvPacket
    {
        public readonly Serial VendorSerial;
        public readonly VendorSellItem[] Items;
        public VendorSellListPacket(PacketReader reader)
            : base(0x9E, "Vendor Sell List")
        {
            VendorSerial = (Serial)reader.ReadInt32();

            ushort numItems = reader.ReadUInt16();
            Items = new VendorSellItem[numItems];
            for (int i = 0; i < numItems; i++)
            {
                Items[i] = new VendorSellItem(reader);
            }
        }

        public struct VendorSellItem
        {
            public readonly Serial ItemSerial;
            public readonly ushort ItemID;
            public readonly ushort Hue;
            public readonly ushort Amount;
            public readonly ushort Price;
            public readonly string Name;

            public VendorSellItem(PacketReader reader)
            {
                ItemSerial = reader.ReadInt32();
                ItemID = reader.ReadUInt16();
                Hue = reader.ReadUInt16();
                Amount = reader.ReadUInt16();
                Price = reader.ReadUInt16();

                ushort nameLength = reader.ReadUInt16();
                Name = reader.ReadString(nameLength);
            }
        }
    }
}
