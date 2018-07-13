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
using JuicyUO.Core.IO;
#endregion

namespace JuicyUO.Ultima.IO
{
    public abstract class AFileIndex
    {
        FileIndexEntry3D[] m_Index;
        Stream m_Stream;

        public FileIndexEntry3D[] Index => m_Index;
        public Stream Stream => m_Stream;

        public string DataPath { get; private set; }
        public int Length { get; protected set; }

        protected abstract FileIndexEntry3D[] ReadEntries();

        protected AFileIndex(string dataPath)
        {
            DataPath = dataPath;
        }

        protected AFileIndex(string dataPath, int length)
        {
            Length = length;
            DataPath = dataPath;
        }

        public void Open()
        {
            m_Index = ReadEntries();
            Length = m_Index.Length;
        }

        public BinaryFileReader Seek(int index, out int length, out int extra, out bool patched)
        {
            if (Stream == null)
            {
                m_Stream = new FileStream(DataPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }

            if (index < 0 || index >= m_Index.Length)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            FileIndexEntry3D e = m_Index[index];

            if (e.Lookup < 0)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            length = e.Length & 0x7FFFFFFF;
            extra = e.Extra;

            if ((e.Length & 0xFF000000) != 0)
            {
                patched = true;

                VerData.Stream.Seek(e.Lookup, SeekOrigin.Begin);
                return new BinaryFileReader(new BinaryReader(VerData.Stream));
            }
            else if (m_Stream == null)
            {
                length = extra = 0;
                patched = false;
                return null;
            }

            patched = false;

            m_Stream.Position = e.Lookup;
            return new BinaryFileReader(new BinaryReader(m_Stream));
        }
    }
}
