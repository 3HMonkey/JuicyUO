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
using System;
using System.Collections.Generic;
using System.Text;
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Compression;
using JuicyUO.Core.Network.Packets;
#endregion


namespace JuicyUO.Ultima.Network.Server
{
    public class CompressedGumpPacket : RecvPacket
    {
        public readonly int GumpSerial;
        public readonly int GumpTypeID;
        public readonly int X;
        public readonly int Y;
        public readonly string GumpData;
        public readonly string[] TextLines;

        public bool HasData
        {
            get { return GumpData != null; }
        }

        public CompressedGumpPacket(PacketReader reader)
            : base(0xDD, "Compressed Gump")
        {
            GumpSerial = reader.ReadInt32();
            GumpTypeID = reader.ReadInt32();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            
            int compressedLength = reader.ReadInt32() - 4;
            int decompressedLength = reader.ReadInt32();
            byte[] compressedData = reader.ReadBytes(compressedLength);
            byte[] decompressedData = new byte[decompressedLength];

            if (ZlibCompression.Unpack(decompressedData, ref decompressedLength, compressedData, compressedLength) != ZLibError.Okay)
            {
                // Problem decompressing, go ahead and quit.
                return;
            }
            else
            {
                GumpData = Encoding.ASCII.GetString(decompressedData);

                int numTextLines = reader.ReadInt32();
                int compressedTextLength = reader.ReadInt32() - 4;
                int decompressedTextLength = reader.ReadInt32();
                byte[] decompressedText = new byte[decompressedTextLength];
                if (numTextLines > 0 && decompressedTextLength > 0)
                {
                    byte[] compressedTextData = reader.ReadBytes(compressedTextLength);
                    ZlibCompression.Unpack(decompressedText, ref decompressedTextLength, compressedTextData, compressedTextLength);
                    int index = 0;
                    List<string> lines = new List<string>();
                    for (int i = 0; i < numTextLines; i++)
                    {
                        int length = decompressedText[index] * 256 + decompressedText[index + 1];
                        index += 2;
                        byte[] b = new byte[length * 2];
                        Array.Copy(decompressedText, index, b, 0, length * 2);
                        index += length * 2;
                        lines.Add(Encoding.BigEndianUnicode.GetString(b));
                    }
                    TextLines = lines.ToArray();
                }
                else
                {
                    TextLines = new string[0];
                }
            }
        }
    }
}
