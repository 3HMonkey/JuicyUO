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
using System.IO;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    class RadarColorData
    {
        public static uint[] Colors = new uint[0x20000];

        const int multiplier = 0xFF / 0x1F;

        static RadarColorData()
        {
            using (FileStream index = new FileStream(FileManager.GetFilePath("Radarcol.mul"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader bin = new BinaryReader(index);

                // Prior to 7.0.7.1, all clients have 0x10000 colors. Newer clients have fewer colors.
                int colorCount = (int)index.Length / 2;

                for (int i = 0; i < colorCount; i++)
                {
                    uint c = bin.ReadUInt16();
                    Colors[i] = 0xFF000000 | (
                            ((((c >> 10) & 0x1F) * multiplier)) |
                            ((((c >> 5) & 0x1F) * multiplier) << 8) |
                            (((c & 0x1F) * multiplier) << 16)
                            );
                }
                // fill the remainder of the color table with non-transparent magenta.
                for (int i = colorCount; i < Colors.Length; i++)
                {
                    Colors[i] = 0xFFFF00FF;
                }

                Metrics.ReportDataRead((int)bin.BaseStream.Position);
            }
        }
    }
}