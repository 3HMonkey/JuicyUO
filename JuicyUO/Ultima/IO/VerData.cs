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

using System.IO;

namespace JuicyUO.Ultima.IO
{
    public class VerData
    {
        private static readonly FileIndexEntry5D[] m_Patches;
        private static readonly Stream m_Stream;

        public static Stream Stream { get { return m_Stream; } }
        public static FileIndexEntry5D[] Patches { get { return m_Patches; } }

        static VerData()
        {
            string path = FileManager.GetFilePath("verdata.mul");

            if (!File.Exists(path))
            {
                m_Patches = new FileIndexEntry5D[0];
                m_Stream = Stream.Null;
            }
            else
            {
                m_Stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryReader bin = new BinaryReader(m_Stream);

                m_Patches = new FileIndexEntry5D[bin.ReadInt32()];

                for (int i = 0; i < m_Patches.Length; ++i)
                {
                    m_Patches[i].file = bin.ReadInt32();
                    m_Patches[i].index = bin.ReadInt32();
                    m_Patches[i].lookup = bin.ReadInt32();
                    m_Patches[i].length = bin.ReadInt32();
                    m_Patches[i].extra = bin.ReadInt32();
                }
            }
        }
    }
}