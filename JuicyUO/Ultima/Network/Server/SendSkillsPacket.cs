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
    public class SendSkillsPacket : RecvPacket
    {
        public readonly int PacketType;
        public readonly SendSkillsPacket_SkillEntry[] Skills;

        public SendSkillsPacket(PacketReader reader)
            : base(0x3A, "Send Skills List")
        {
            PacketType = reader.ReadByte();
            bool hasSkillCap = (PacketType == 0x02 || PacketType == 0xDF);
            int numSkills = (reader.Size - reader.Index - (PacketType == 0x00 ? 2 : 0)) / (hasSkillCap ? 9 : 7);
            Skills = new SendSkillsPacket_SkillEntry[numSkills];
            for (int i = 0; i < numSkills; i++)
            {
                Skills[i] = new SendSkillsPacket_SkillEntry(reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadByte());
                if (hasSkillCap)
                    Skills[i].SetSkillCap(reader.ReadUInt16());
            }
            // 0x00: Full List of skills
            // 0xFF: Single skill update
            // 0x02: Full List of skills with skill cap for each skill
            // 0xDF: Single skill update with skill cap for skill

        }
    }

    public class SendSkillsPacket_SkillEntry
    {
        public int SkillID;
        public float SkillValue;
        public float SkillValueUnmodified;
        public byte SkillLock;
        public float SkillCap;

        public SendSkillsPacket_SkillEntry(ushort id, ushort value, ushort unmodified, byte lockvalue)
        {
            SkillID = id;
            SkillValue = (float)value / 10f;
            SkillValueUnmodified = (float)unmodified / 10f;
            SkillLock = lockvalue;
            SkillCap = 100.0f;
        }

        public void SetSkillCap(ushort value)
        {
            SkillCap = (float)value / 10f;
        }
    }
}
