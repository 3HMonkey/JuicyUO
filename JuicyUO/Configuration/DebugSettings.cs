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
    public sealed class DebugSettings : ASettingsSection
    {
        bool m_IsConsoleEnabled;
        bool m_ShowFps;
        bool m_LogPackets;

        public DebugSettings()
        {
            LogPackets = false;
            IsConsoleEnabled = true;
            ShowFps = true;
        }

        /// <summary>
        /// If true, all received packets will be logged to Tracer.Debug, and any active Tracer listeners (console, debug.txt file logger)
        /// </summary>
        public bool LogPackets
        {
            get { return m_LogPackets; }
            set { SetProperty(ref m_LogPackets, value); }
        }

        /// <summary>
        /// If true, FPS should display either in the window caption or in the game window. (not currently enabled).
        /// </summary>
        public bool ShowFps
        {
            get { return m_ShowFps; }
            set { SetProperty(ref m_ShowFps, value); }
        }

        /// <summary>
        /// If true, a console window which will display debug and error messages should appear at runtime. This may not work in Release configurations.
        /// </summary>
        public bool IsConsoleEnabled
        {
            get { return m_IsConsoleEnabled; }
            set { SetProperty(ref m_IsConsoleEnabled, value); }
        }
    }
}