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

using System;
using System.Collections.Generic;
using System.IO;
using JuicyUO.Ultima.IO;

namespace JuicyUO.Ultima.Resources
{
    class MusicData
    {
        private const string m_ConfigFilePath = @"Music/Digital/Config.txt";
        private static char[] m_configFileDelimiters = { ' ', ',', '\t' };

        private static readonly Dictionary<int, Tuple<string, bool>> m_MusicData = new Dictionary<int, Tuple<string, bool>>();

        static MusicData()
        {
            // open UO's music Config.txt
            if (!FileManager.Exists(m_ConfigFilePath))
                return;
            // attempt to read out all the values from the file.
            using (StreamReader reader = new StreamReader(FileManager.GetFile(m_ConfigFilePath)))
            {
                String line;
                while ((line = reader.ReadLine()) != null)
                {
                    Tuple<int, string, bool> songData;
                    if (TryParseConfigLine(line, out songData))
                    {
                        m_MusicData.Add(songData.Item1, new Tuple<string, bool>(songData.Item2, songData.Item3));
                    }
                }
            }
        }

        /// <summary>
        /// Attempts to parse a line from UO's music Config.txt.
        /// </summary>
        /// <param name="line">A line from the file.</param>
        /// <param name="?">If successful, contains a tuple with these fields: int songIndex, string songName, bool doesLoop</param>
        /// <returns>true if line could be parsed, false otherwise.</returns>
        private static bool TryParseConfigLine(string line, out Tuple<int, string, bool> songData)
        {
            songData = null;

            string[] splits = line.Split(m_configFileDelimiters);
            if (splits.Length < 2 || splits.Length > 3)
            {
                return false;
            }

            int index = int.Parse(splits[0]);
            string name = splits[1].Trim();
            bool doesLoop = splits.Length == 3 && splits[2] == "loop";

            songData = new Tuple<int, string, bool>(index, name, doesLoop);
            return true;
        }

        public static bool TryGetMusicData(int index, out string name, out bool doesLoop)
        {
            name = null;
            doesLoop = false;

            if (m_MusicData.ContainsKey(index))
            {
                name = m_MusicData[index].Item1;
                doesLoop = m_MusicData[index].Item2;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
