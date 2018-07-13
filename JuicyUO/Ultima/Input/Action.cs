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
using JuicyUO.Core.Windows;

namespace JuicyUO.Ultima.Input
{
    /// <summary>
    /// A list of one or more macros that is executed on a given keystroke.
    /// </summary>
    public class Action
    {
        public WinKeys Keystroke = WinKeys.None;
        public bool Shift;
        public bool Alt;
        public bool Ctrl;

        public List<Macro> Macros = new List<Macro>();

        public Action()
        {

        }

        public override string ToString()
        {
            return string.Format("{0}{1}{2}{3}", 
                Shift ? "Shift+" : string.Empty,
                Alt ? "Alt+" : string.Empty,
                Ctrl ? "Ctrl+" : string.Empty,
                Keystroke.ToString());
        }
    }
}
