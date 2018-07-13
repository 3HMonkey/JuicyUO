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
using JuicyUO.Ultima.Network.Server;
#endregion

namespace JuicyUO.Ultima.Login.Accounts
{
    public static class Characters
    {
        static CharacterListEntry[] m_characters;
        public static CharacterListEntry[] List { get { return m_characters; } }
        public static int Length { get { return m_characters.Length; } }

        static CharacterCityListPacket.StartingLocation[] m_locations;
        public static CharacterCityListPacket.StartingLocation[] StartingLocations { get { return m_locations; } }

        static int m_updateValue;
        public static int UpdateValue { get { return m_updateValue; } }

        public static int FirstEmptySlot
        {
            get
            {
                for (int i = 0; i < m_characters.Length; i++)
                {
                    if (m_characters[i].Name == string.Empty)
                        return i;
                }
                return -1;
            }
        }

        public static void SetCharacterList(CharacterListEntry[] list)
        {
            m_characters = list;
            m_updateValue++;
        }

        public static void SetStartingLocations(CharacterCityListPacket.StartingLocation[] list)
        {
            m_locations = list;
            m_updateValue++;
        }
    }
}
