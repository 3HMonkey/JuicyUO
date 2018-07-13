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

using System.Collections.Generic;

namespace JuicyUO.Ultima.Data {
    public class ContextMenuData {
        readonly List<ContextMenuItem> m_Entries = new List<ContextMenuItem>();
        readonly Serial m_Serial;

        public ContextMenuData(Serial serial) {
            m_Serial = serial;
        }

        public Serial Serial => m_Serial;

        public int Count => m_Entries.Count;

        public ContextMenuItem this[int index] {
            get {
                if (index < 0 || index >= m_Entries.Count)
                    return null;
                return m_Entries[index];
            }
        }

        // Add a new context menu entry.
        internal void AddItem(int responseCode, int stringID, int flags, int hue) {
            m_Entries.Add(new ContextMenuItem(responseCode, stringID, flags, hue));
        }
    }
}