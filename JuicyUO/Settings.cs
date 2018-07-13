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
using JuicyUO.Configuration;
using JuicyUO.Core.Configuration;
#endregion

namespace JuicyUO
{
    public class Settings
    {
        // === Instance ===============================================================================================
        readonly DebugSettings m_Debug;
        readonly EngineSettings m_Engine;
        readonly GumpSettings m_Gumps;
        readonly UserInterfaceSettings m_UI;
        readonly LoginSettings m_Login;
        readonly UltimaOnlineSettings m_UltimaOnline;
        readonly AudioSettings m_Audio;

        Settings()
        {
            m_Debug = CreateOrOpenSection<DebugSettings>();
            m_Login = CreateOrOpenSection<LoginSettings>();
            m_UltimaOnline = CreateOrOpenSection<UltimaOnlineSettings>();
            m_Engine = CreateOrOpenSection<EngineSettings>();
            m_UI = CreateOrOpenSection<UserInterfaceSettings>();
            m_Gumps = CreateOrOpenSection<GumpSettings>();
            m_Audio = CreateOrOpenSection<AudioSettings>();
        }

        // === Static Settings properties =============================================================================
        public static DebugSettings Debug => s_Instance.m_Debug;
        public static LoginSettings Login => s_Instance.m_Login;
        public static UltimaOnlineSettings UltimaOnline => s_Instance.m_UltimaOnline;
        public static EngineSettings Engine => s_Instance.m_Engine;
        public static GumpSettings Gumps => s_Instance.m_Gumps;
        public static UserInterfaceSettings UserInterface => s_Instance.m_UI;
        public static AudioSettings Audio => s_Instance.m_Audio;

        static readonly Settings s_Instance;
        static readonly SettingsFile s_File;

        static Settings()
        {
            s_File = new SettingsFile("settings.cfg");
            s_Instance = new Settings();
            s_File.Load();
        }

        public static void Save()
        {
            s_File.Save();
        }

        public static T CreateOrOpenSection<T>()
            where T : ASettingsSection, new()
        {
            string sectionName = typeof(T).Name;
            T section = s_File.CreateOrOpenSection<T>(sectionName);
            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;
            return section;
        }

        static void OnSectionPropertyChanged(object sender, EventArgs e)
        {
            s_File.InvalidateDirty();
        }

        static void OnSectionInvalidated(object sender, EventArgs e)
        {
            s_File.InvalidateDirty();
        }
    }
}