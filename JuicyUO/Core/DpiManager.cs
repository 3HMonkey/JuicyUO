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
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace JuicyUO.Core
{
    static class DpiManager
    {
        private const int LogPixelsX = 88; // Used for GetDeviceCaps().
        private const int LogPixelsY = 90; // Used for GetDeviceCaps().
        private const float StandardDpi = 96f; // Used for GetDeviceCaps().

        public static Vector2 GetSystemDpiScalar()
        {
            Vector2 result = new Vector2();
            IntPtr hdc = GetDC(IntPtr.Zero);

            result.X = GetDeviceCaps(hdc, LogPixelsX) / StandardDpi;
            result.Y = GetDeviceCaps(hdc, LogPixelsY) / StandardDpi;

            ReleaseDC(IntPtr.Zero, hdc);

            return result;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}