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
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
using JuicyUO.Ultima.Data;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class SupportedFeaturesPacket : RecvPacket
    {
        /// <summary>
        /// From POLServer packet docs: http://docs.polserver.com/packets/index.php?Packet=0xB9
        /// 0x01: enable T2A features: chat, regions
        /// 0x02: enable renaissance features
        /// 0x04: enable third dawn features
        /// 0x08: enable LBR features: skills, map
        /// 0x10: enable AOS features: skills, map, spells, fightbook
        /// 0x20: 6th character slot
        /// 0x40: enable SE features
        /// 0x80: enable ML features: elven race, spells, skills
        /// 0x100: enable 8th age splash screen
        /// 0x200: enable 9th age splash screen, crystal/shadow housing tiles
        /// 0x400: enable 10th age
        /// 0x800: enable increased housing and bank storage
        /// 0x1000: 7th character slot
        /// 0x2000: 10th age KR faces
        /// 0x4000: enable trial account
        /// 0x8000: enable 11th age
        /// 0x10000: enable SA features: gargoyle race, spells, skills
        /// 0x20000: HS features
        /// 0x40000: Gothic housing tiles
        /// 0x80000: Rustic housing tiles
        /// </summary>
        public FeatureFlags Flags
        {
            get;
            private set;
        }

        public SupportedFeaturesPacket(PacketReader reader)
            : base(0xB9, "Enable Features")
        {
            if (reader.Buffer.Length == 3)
                Flags = (FeatureFlags)reader.ReadUInt16();
            else if (reader.Buffer.Length == 5)
                Flags = (FeatureFlags)reader.ReadUInt16();
            else
            {
                Flags = (FeatureFlags)reader.ReadUInt16();
                Tracer.Error("Bad feature flag size in SupportedFeaturesPacket; expected 16 or 32 bit features, received {0} bits.", (reader.Buffer.Length - 1) * 8);
            }
        }
    }
}
