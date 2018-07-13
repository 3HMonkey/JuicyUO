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
using System.IO;
using JuicyUO.Core.UI.Fonts;
#endregion

namespace JuicyUO.Ultima.Resources.Fonts
{
    class CharacterUnicode : ACharacter
    {
        public CharacterUnicode()
        {

        }

        public CharacterUnicode(BinaryReader reader)
        {
            XOffset = reader.ReadSByte();
            YOffset = reader.ReadSByte();
            Width = reader.ReadByte();
            Height = reader.ReadByte();
            ExtraWidth = 1;

            // only read data if there is IO...
            if ((Width > 0) && (Height > 0))
            {
                m_PixelData = new uint[Width * Height];

                // At this point, we know we have data, so go ahead and start reading!
                for (int y = 0; y < Height; ++y)
                {
                    byte[] scanline = reader.ReadBytes(((Width - 1) / 8) + 1);
                    int bitX = 7;
                    int byteX = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        uint color = 0x00000000;
                        if ((scanline[byteX] & (byte)Math.Pow(2, bitX)) != 0)
                            color = 0xFFFFFFFF;

                        m_PixelData[y * Width + x] = color;

                        bitX--;
                        if (bitX < 0)
                        {
                            bitX = 7;
                            byteX++;
                        }
                    }
                }
            }
        }
    }
}
