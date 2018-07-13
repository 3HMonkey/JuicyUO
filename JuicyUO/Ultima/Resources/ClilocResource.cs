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
using System.Collections;
using System.IO;
using System.Text;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public class ClilocResource
    {
        Hashtable m_Table;

        public readonly string Language;

        public ClilocResource(string language)
        {
            Language = language;
            LoadAllClilocs(language);
        }

        public string GetString(int index)
        {
            if (m_Table[index] == null)
            {
                Tracer.Warn("Missing cliloc with index {0}. Client version is lower than expected by Server.", index);
                return $"Err: Cliloc Entry {index} not found.";
            }
            return m_Table[index].ToString();
        }

        void LoadAllClilocs(string language)
        {
            m_Table = new Hashtable();
            string mainClilocFile = $"Cliloc.{language}";
            LoadCliloc(mainClilocFile);
            // All the other Cliloc*.language files:
            /*string[] paths = FileManager.GetFilePaths($"cliloc*.{language}");
            foreach (string path in paths)
            {
                if (path != mainClilocFile)
                {
                    LoadCliloc(path);
                }
            }*/
        }

        void LoadCliloc(string path)
        {
            path = FileManager.GetFilePath(path);
            if (path == null)
            {
                return;
            }
            byte[] buffer;
            using (BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                buffer = bin.ReadBytes((int)bin.BaseStream.Length);
                Metrics.ReportDataRead((int)bin.BaseStream.Position);
            }
            int pos = 6;
            int count = buffer.Length;
            while (pos < count)
            {
                int number = BitConverter.ToInt32(buffer, pos);
                int length = BitConverter.ToInt16(buffer, pos + 5);
                string text = Encoding.UTF8.GetString(buffer, pos + 7, length);
                pos += length + 7;
                m_Table[number] = text; // auto replace with updates.
            }
        }

        class StringEntry
        {
            readonly int m_Number;
            readonly string m_Text;

            public int Number { get { return m_Number; } }
            public string Text { get { return m_Text; } }

            public StringEntry(int number, string text)
            {
                m_Number = number;
                m_Text = text;
            }
        }
    }
}