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
using System.Collections.Generic;
using JuicyUO.Core.Network.Packets;
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.Data;
#endregion

namespace JuicyUO.Ultima.Network.Client
{
    public class AsciiSpeechPacket : SendPacket
    {
        public AsciiSpeechPacket(MessageTypes type, int font, int hue, string lang, string text)
            : base(0xAD, "Ascii Speech")
        {
            // get triggers
            int triggerCount; int[] triggers;
            SpeechData.GetSpeechTriggers(text, lang, out triggerCount, out triggers);
            if (triggerCount > 0)
                type = type | MessageTypes.EncodedTriggers;

            Stream.Write((byte)type);
            Stream.Write((short)hue);
            Stream.Write((short)font);
            Stream.WriteAsciiNull(lang);
            if (triggerCount > 0)
            {
                byte[] t = new byte[(int)Math.Ceiling((triggerCount + 1) * 1.5f)];
                // write 12 bits at a time. first write count: byte then half byte.
                t[0] = (byte)((triggerCount & 0x0FF0) >> 4);
                t[1] = (byte)((triggerCount & 0x000F) << 4);
                for (int i = 0; i < triggerCount; i++)
                {
                    int index = (int)((i + 1) * 1.5f);
                    if (i % 2 == 0) // write half byte and then byte
                    {
                        t[index + 0] |= (byte)((triggers[i] & 0x0F00) >> 8);
                        t[index + 1] = (byte)(triggers[i] & 0x00FF);
                    }
                    else // write byte and then half byte
                    {
                        t[index] = (byte)((triggers[i] & 0x0FF0) >> 4);
                        t[index + 1] = (byte)((triggers[i] & 0x000F) << 4);
                    }
                }
                Stream.BaseStream.Write(t, 0, t.Length);
                Stream.WriteAsciiNull(text);
            }
            else
            {
                Stream.WriteBigUniNull(text);
            }
        }

        List<int> getSpeechTriggers(string text)
        {
            List<int> triggers = new List<int>();

            return triggers;
        }
    }
}
