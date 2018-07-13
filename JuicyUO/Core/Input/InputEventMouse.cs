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

using Microsoft.Xna.Framework;
using JuicyUO.Core.Windows;

namespace JuicyUO.Core.Input
{
    public class InputEventMouse : InputEvent
    {
        const int WHEEL_DELTA = 120;

        public readonly MouseEvent EventType;
        public readonly int X;
        public readonly int Y;
        public int WheelValue => m_Clicks / WHEEL_DELTA;
        public Point Position => new Point(X, Y);

        readonly WinMouseButtons m_Buttons;
        readonly int m_Clicks;
        readonly int m_MouseData;

        public MouseButton Button
        {
            get
            {
                if ((m_Buttons & WinMouseButtons.Left) == WinMouseButtons.Left)
                    return MouseButton.Left;
                if ((m_Buttons & WinMouseButtons.Right) == WinMouseButtons.Right)
                    return MouseButton.Right;
                if ((m_Buttons & WinMouseButtons.Middle) == WinMouseButtons.Middle)
                    return MouseButton.Middle;
                if ((m_Buttons & WinMouseButtons.XButton1) == WinMouseButtons.XButton1)
                    return MouseButton.XButton1;
                if ((m_Buttons & WinMouseButtons.XButton2) == WinMouseButtons.XButton2)
                    return MouseButton.XButton2;
                return MouseButton.None;
            }
        }

        public InputEventMouse(MouseEvent type, WinMouseButtons btn, int clicks, int x, int y, int data, WinKeys modifiers)
            : base(modifiers)
        {
            Vector2 dpi = DpiManager.GetSystemDpiScalar();
            EventType = type;
            m_Buttons = btn;
            m_Clicks = clicks;
            X = (int)(x / dpi.X);
            Y = (int)(y / dpi.Y);
            m_MouseData = data;
        }

        public InputEventMouse(MouseEvent eventType, InputEventMouse parent)
            : base(parent)
        {
            EventType = eventType;
            m_Buttons = parent.m_Buttons;
            m_Clicks = parent.m_Clicks;
            X = parent.X;
            Y = parent.Y;
            m_MouseData = parent.m_MouseData;
        }
    }
}
