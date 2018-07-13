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
    public static class BodyDef
    {
        static Dictionary<int, BodyTableEntry> m_Entries;

        static BodyDef()
        {
            m_Entries = new Dictionary<int, BodyTableEntry>();

            string filePath = FileManager.GetFilePath("body.def");

            if (filePath == null)
                return;

            StreamReader def = new StreamReader(filePath);

            string line;
            int totalDataRead = 0;

            while ((line = def.ReadLine()) != null)
            {
                totalDataRead += line.Length;

                if ((line = line.Trim()).Length == 0 || line.StartsWith("#"))
                    continue;

                try
                {
                    int index1 = line.IndexOf("{");
                    int index2 = line.IndexOf("}");

                    string origBody = line.Substring(0, index1);
                    string newBody = line.Substring(index1 + 1, index2 - index1 - 1);
                    string newHue = line.Substring(index2 + 1);

                    int indexOf = newBody.IndexOf(',');
                    if (indexOf > -1)
                        newBody = newBody.Substring(0, indexOf).Trim();

                    int iParam1 = Convert.ToInt32(origBody);
                    int iParam2 = Convert.ToInt32(newBody);
                    int iParam3 = Convert.ToInt32(newHue);

                    m_Entries[iParam1] = new BodyTableEntry(iParam1, iParam2, iParam3);
                }
                catch
                {
                }
            }
            Metrics.ReportDataRead(totalDataRead);
        }

        public static void TranslateBodyAndHue(ref int body, ref int hue)
        {
            BodyTableEntry bte = null;
            if (m_Entries.TryGetValue(body, out bte))
            {
                body = bte.NewBody;
                if (hue == 0)
                    hue = bte.NewHue;
            }
        }

        private class BodyTableEntry
        {
            public readonly int OriginalBody;
            public readonly int NewBody;
            public readonly int NewHue;

            public BodyTableEntry(int oldID, int newID, int newHue)
            {
                OriginalBody = oldID;
                NewBody = newID;
                NewHue = newHue;
            }

            public override string ToString()
            {
                return string.Format("{0} {1} {2}", OriginalBody, NewBody, NewHue);
            }
        }
    }
}
