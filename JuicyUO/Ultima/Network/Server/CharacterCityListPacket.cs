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
using JuicyUO.Core.Network;
using JuicyUO.Core.Network.Packets;
using JuicyUO.Ultima.Login.Accounts;
#endregion

namespace JuicyUO.Ultima.Network.Server
{
    public class CharacterCityListPacket : RecvPacket
    {
        readonly StartingLocation[] m_locations;
        readonly CharacterListEntry[] m_characters;

        public StartingLocation[] Locations
        {
            get { return m_locations; }
        }

        public CharacterListEntry[] Characters
        {
            get { return m_characters; }
        }

        public CharacterCityListPacket(PacketReader reader)
            : base(0xA9, "Char/City List")
        {
            int characterCount = reader.ReadByte();
            m_characters = new CharacterListEntry[characterCount];

            for (int i = 0; i < characterCount; i++)
            {
                m_characters[i] = new CharacterListEntry(reader);
            }

            int locationCount = reader.ReadByte();
            m_locations = new StartingLocation[locationCount];

            for (int i = 0; i < locationCount; i++)
            {
                m_locations[i] = new StartingLocation(reader);
            }
        }

        public class StartingLocation
        {
            readonly byte index;
            readonly string cityName;
            readonly string areaOfCityOrTown;

            public byte Index
            {
                get { return index; }
            }

            public string CityName
            {
                get { return cityName; }
            }

            public string AreaOfCityOrTown
            {
                get { return areaOfCityOrTown; }
            }

            public StartingLocation(PacketReader reader)
            {
                index = reader.ReadByte();
                cityName = reader.ReadString(31);
                areaOfCityOrTown = reader.ReadString(31);
            }
        }
    }
}
