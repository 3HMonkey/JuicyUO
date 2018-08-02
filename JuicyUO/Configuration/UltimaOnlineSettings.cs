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
using JuicyUO.Ultima.Data;
#endregion

namespace JuicyUO.Configuration
{
    public sealed class UltimaOnlineSettings : ASettingsSection
    {
        bool m_AllowCornerMovement;
        string m_DataDirectory;
        byte[] m_ClientVersion;

        public UltimaOnlineSettings()
        {
            PatchVersion = ClientVersion.DefaultVersion;
        }

        /// <summary>
        /// The patch version which is sent to the server. RunUO (and possibly other server software) rely on the
        /// client's reported patch version to enable/disable certain packets and features.
        /// </summary>
        public byte[] PatchVersion
        {
            get {
                if (m_ClientVersion == null || m_ClientVersion.Length != 4)
                    return ClientVersion.DefaultVersion;
                return m_ClientVersion;
            }
            set
            {
                if (value == null || value.Length != 4)
                    return;
                // Note from ZaneDubya: I will not support your client if you change or remove this line:
                if (!ClientVersion.EqualTo(value, ClientVersion.DefaultVersion)) return;
                SetProperty(ref m_ClientVersion, value);
            }
        }
        
        /// <summary>
        /// The directory where the Ultima Online resource files and executable are located.
        /// </summary>
        public string DataDirectory
        {
            get { return @"D:\Program Files (x86)\Electronic Arts\Ultima Online Classic"; }
            set { SetProperty(ref m_DataDirectory, value); }
        }

        /// <summary>
        /// When true, allows corner-cutting movement (ala the God client and RunUO administrator-mode movement).
        /// </summary>
        public bool AllowCornerMovement
        {
            get { return m_AllowCornerMovement; }
            set { SetProperty(ref m_AllowCornerMovement, value); }
        }
    }
}