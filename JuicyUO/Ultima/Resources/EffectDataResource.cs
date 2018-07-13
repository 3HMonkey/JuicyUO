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
using JuicyUO.Core.Resources;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    /// <summary>
    /// This file contains information about animated effects.
    /// </summary>
    public class EffectDataResource : IResource<EffectData>
    {
        const int Count = 0x0800;
        private readonly EffectData[][] m_AnimData;

        public EffectDataResource()
        {
            // From http://wpdev.sourceforge.net/docs/guide/node167.html:
            // There are 2048 blocks, 8 entries per block, 68 bytes per entry.
            // Thanks to Krrios for figuring out the blocksizes.
            // Each block has an 4 byte header which is currently unknown. The
            // entries correspond with the Static ID. You can lookup an entry
            // for a given static with this formula:
            // Offset = (id>>3)*548+(id&15)*68+4;
            // Here is the record format for each entry:
            // byte[64] Frames
            // byte     Unknown
            // byte     Number of Frames Used
            // byte     Frame Interval
            // byte     Start Interval

            m_AnimData = new EffectData[Count][];

            FileStream stream = FileManager.GetFile("animdata.mul");
            BinaryReader reader = new BinaryReader(stream);

            for (int i = 0; i < Count; i++)
            {
                EffectData[] data = new EffectData[8];
                int header = reader.ReadInt32(); // unknown value.
                for (int j = 0; j < 8; j++)
                {
                    data[j] = new EffectData(reader);
                }
                m_AnimData[i] = data;
            }
        }

        public EffectData GetResource(int itemID)
        {
            itemID &= FileManager.ItemIDMask;
            return m_AnimData[(itemID >> 3)][itemID & 0x07];
        }
    }
}
