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
using System.IO;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public static class MobtypeData
    {
        private static readonly Dictionary<int, MobtypeEntry> m_entries = new Dictionary<int, MobtypeEntry>();

        static MobtypeData()
        {
            string path = FileManager.GetFilePath("mobtypes.txt");
            {
                StreamReader stream = new StreamReader(path);
                while (!stream.EndOfStream)
                {
                    string line = stream.ReadLine();
                    Metrics.ReportDataRead(line.Length);
                    if ((line != string.Empty) && (line.Substring(0, 1) != "#"))
                    {
                        string[] data = line.Split('\t');
                        int bodyID = Int32.Parse(data[0]);
                        if (m_entries.ContainsKey(bodyID))
                        {
                            m_entries.Remove(bodyID);
                        }
                        m_entries.Add(bodyID, new MobtypeEntry(data[1], data[2]));
                    }
                }
            }
        }

        public static MobType AnimationTypeXXX(int bodyID)
        {
            return m_entries[bodyID].AnimationType;
        }
    }

    public struct MobtypeEntry
    {
        public string Flags;
        public MobType AnimationType;

        public MobtypeEntry(string type, string flags)
        {
            Flags = flags;
            switch (type)
            {
                case "MONSTER":
                    AnimationType = MobType.Monster;
                    break;
                case "ANIMAL":
                    AnimationType = MobType.Animal;
                    break;
                case "SEA_MONSTER":
                    AnimationType = MobType.Monster;
                    break;
                case "HUMAN":
                    AnimationType = MobType.Humanoid;
                    break;
                case "EQUIPMENT":
                    AnimationType = MobType.Humanoid;
                    break;
                default:
                    AnimationType = MobType.Null;
                    break;
            }
        }
    }

    public enum MobType
    {
        Null = -1,
        Monster = 0,
        Animal = 1,
        Humanoid = 2
    }
}
