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

using JuicyUO.Core.Windows;

namespace JuicyUO.Core.Input
{
    public class InputEvent
    {
        readonly WinKeys m_Modifiers;
        bool m_Handled;

        public bool Alt => (m_Modifiers & WinKeys.Alt) == WinKeys.Alt;

        public bool Control => (m_Modifiers & WinKeys.Control) == WinKeys.Control;

        public bool Shift => (m_Modifiers & WinKeys.Shift) == WinKeys.Shift;

        public bool Handled
        {
            get { return m_Handled; }
            set { m_Handled = value; }
        }

        public InputEvent(WinKeys modifiers)
        {
            m_Modifiers = modifiers;
        }

        protected InputEvent(InputEvent parent)
        {
            m_Modifiers = parent.m_Modifiers;
        }
    }
}
