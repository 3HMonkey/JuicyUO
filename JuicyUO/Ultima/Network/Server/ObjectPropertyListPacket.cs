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
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class ObjectPropertyListPacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly int m_hash;
        readonly List<int> m_clilocs;
        readonly List<string> m_arguments;

        public Serial Serial
        {
            get { return m_serial; }
        }
        
        public int Hash
        {
            get { return m_hash; }
        }
        
        public List<int> CliLocs
        {
            get { return m_clilocs; }
        }
        
        public List<string> Arguements
        {
            get { return m_arguments; }
        }
        
        public ObjectPropertyListPacket(PacketReader reader)
            : base(0xD6, "Object Property List")
        {
            reader.ReadInt16(); // Always 0x0001
            m_serial = reader.ReadInt32();

            reader.ReadInt16(); // Always 0x0000
            m_hash = reader.ReadInt32();

            m_clilocs = new List<int>();
            m_arguments = new List<string>();

            // Loop of all the item/creature's properties to display in the order to display them. The name is always the first entry.
            int clilocId = reader.ReadInt32();

            while (clilocId != 0)
            {
                m_clilocs.Add(clilocId);

                int textLength = reader.ReadUInt16();
                string args = string.Empty;

                if (textLength > 0)
                {
                    args = reader.ReadUnicodeStringReverse(textLength / 2);
                }

                m_arguments.Add(args);

                clilocId = reader.ReadInt32();
            }
        }
    }
}
