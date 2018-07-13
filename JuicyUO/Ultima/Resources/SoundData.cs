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
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Core.IO;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.IO;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public static class SoundData
    {
        private static AFileIndex m_Index;
        //private static Stream m_Stream;
        private static Dictionary<int, int> m_Translations;
        
        private static bool m_filesPrepared;

        public static bool TryGetSoundData(int soundID, out byte[] data, out string name)
        {
            // Sounds.mul is exclusively locked by the legacy client, so we need to make sure this file is available
            // before attempting to play any sounds.
            if (!m_filesPrepared)
                setupFiles();

            data = null;
            name = null;

            if (!m_filesPrepared || soundID < 0)
                return false;
            else
            {
                int length, extra;
                bool is_patched;

                BinaryFileReader reader = m_Index.Seek(soundID, out length, out extra, out is_patched);
                int streamStart = (int)reader.Position;
                int offset = (int)reader.Position;
                

                if ((offset < 0) || (length <= 0))
                {
                    if (!m_Translations.TryGetValue(soundID, out soundID))
                        return false;


                    reader = m_Index.Seek(soundID, out length, out extra, out is_patched);
                    streamStart = (int)reader.Position;
                    offset = (int)reader.Position;
                }

                if ((offset < 0) || (length <= 0))
                    return false;

                byte[] stringBuffer = new byte[40];
                data = new byte[length - 40];

                reader.Seek((long)(offset), SeekOrigin.Begin);
                stringBuffer = reader.ReadBytes(40);
                data = reader.ReadBytes(length - 40);

                name = Encoding.ASCII.GetString(stringBuffer).Trim();
                int end = name.IndexOf("\0");
                name = name.Substring(0, end);
                Metrics.ReportDataRead((int)reader.Position - streamStart);

                return true;
            }
        }

        private static void setupFiles()
        {
            try
            {
                m_Index = ClientVersion.InstallationIsUopFormat ? FileManager.CreateFileIndex("soundLegacyMUL.uop", 0xFFF, false, ".dat") : FileManager.CreateFileIndex("soundidx.mul", "sound.mul", 0x1000, -1); // new BinaryReader(new FileStream(FileManager.GetFilePath("soundidx.mul"), FileMode.Open));
               // m_Stream = new FileStream(FileManager.GetFilePath("sound.mul"), FileMode.Open);
                m_filesPrepared = true;
            }
            catch
            {
                m_filesPrepared = false;
                return;
            }

            Regex reg = new Regex(@"(\d{1,3}) \x7B(\d{1,3})\x7D (\d{1,3})", RegexOptions.Compiled);

            m_Translations = new Dictionary<int, int>();

            string line;
            using (StreamReader reader = new StreamReader(FileManager.GetFilePath("Sound.def")))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (((line = line.Trim()).Length != 0) && !line.StartsWith("#"))
                    {
                        Match match = reg.Match(line);

                        if (match.Success)
                        {
                            m_Translations.Add(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
                        }
                    }
                }
            }
        }

        
    }
}