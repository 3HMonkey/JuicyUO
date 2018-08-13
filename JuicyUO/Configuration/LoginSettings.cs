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
using JuicyUO.Core.Configuration;
#endregion

namespace JuicyUO.Configuration
{
    public sealed class LoginSettings : ASettingsSection
    {
        string m_ServerAddress;
        int m_ServerPort;
        string m_UserName;
        bool m_AutoSelectLastCharacter;
        string m_LastCharacterName;

        public LoginSettings()
        {
            //ServerAddress = "login.uoforever.com";
            //ServerPort = 2599;
            ServerAddress = "127.0.0.1";
            ServerPort = 2593;
            LastCharacterName = string.Empty;
            AutoSelectLastCharacter = false;
        }

        public string UserName
        {
            get { return m_UserName; }
            set { SetProperty(ref m_UserName, value); }
        }

        public int ServerPort
        {
            get { return m_ServerPort; }
            set { SetProperty(ref m_ServerPort, value); }
        }

        public string ServerAddress
        {
            get { return m_ServerAddress; }
            set { SetProperty(ref m_ServerAddress, value); }
        }

        public string LastCharacterName
        {
            get { return m_LastCharacterName; }
            set { SetProperty(ref m_LastCharacterName, value); }
        }

        public bool AutoSelectLastCharacter
        {
            get { return m_AutoSelectLastCharacter; }
            set { SetProperty(ref m_AutoSelectLastCharacter, value); }
        }
    }
}