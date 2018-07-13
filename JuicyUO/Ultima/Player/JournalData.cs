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

using System;
using System.Collections.Generic;

namespace JuicyUO.Ultima.Player
{
    public class JournalData
    {
        private readonly List<JournalEntry> m_JournalEntries = new List<JournalEntry>();
        public List<JournalEntry> JournalEntries
        {
            get { return m_JournalEntries; }
        }

        public event Action<JournalEntry> OnJournalEntryAdded;

        public void AddEntry(string text, int font, ushort hue, string speakerName, bool asUnicode)
        {
            while (m_JournalEntries.Count > 99)
                m_JournalEntries.RemoveAt(0);
            m_JournalEntries.Add(new JournalEntry(text, font, hue, speakerName, asUnicode));
            OnJournalEntryAdded?.Invoke(m_JournalEntries[m_JournalEntries.Count - 1]);
        }
    }

    public class JournalEntry
    {
        public readonly string Text;
        public readonly int Font;
        public readonly ushort Hue;
        public readonly string SpeakerName;
        public readonly bool AsUnicode;

        public JournalEntry(string text, int font, ushort hue, string speakerName, bool asUnicode)
        {
            Text = text;
            Font = font;
            Hue = hue;
            SpeakerName = speakerName;
            AsUnicode = asUnicode;
        }
    }
}
