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
using JuicyUO.Core.Resources;

namespace JuicyUO.Core.UI.HTML.Styles
{
    public class StyleState
    {
        private StyleValue m_IsUnderlined = StyleValue.Default;
        public bool IsUnderlined
        {
            set
            {
                if (value)
                {
                    m_IsUnderlined = StyleValue.True;
                }
                else
                {
                    m_IsUnderlined = StyleValue.False;
                }
            }
            get
            {
                if (m_IsUnderlined == StyleValue.False)
                    return false;
                else if (m_IsUnderlined == StyleValue.True)
                    return true;
                else // m_IsUnderlined == TagValue.Default
                {
                    if (HREF != null)
                        return true;
                    else
                        return false;
                }
            }
        }

        public bool IsBold;
        public bool IsItalic;
        public bool IsOutlined;

        public bool DrawOutline
        {
            get { return IsOutlined && !Font.HasBuiltInOutline; }
        }

        public IFont Font; // default value set in manager ctor.
        public Color Color = Color.White;
        public int ColorHue = 2;
        public int ActiveColorHue = 12;
        public int HoverColorHue = 24;

        public string HREF;
        public bool IsHREF { get { return HREF != null; } }

        public int ExtraWidth
        {
            get
            {
                int extraWidth = 0;
                if (IsItalic)
                {
                    extraWidth = Font.Height / 2;
                }
                if (DrawOutline)
                {
                    extraWidth += 2;
                }
                return extraWidth;
            }
        }

        public StyleState(IResourceProvider provider)
        {
            Font = provider.GetUnicodeFont((int)Fonts.Default);
        }

        public StyleState(StyleState parent)
        {
            m_IsUnderlined = parent.m_IsUnderlined;
            IsBold = parent.IsBold;
            IsItalic = parent.IsItalic;
            IsOutlined = parent.IsOutlined;
            Font = parent.Font;
            Color = parent.Color;
            ColorHue = parent.ColorHue;
            ActiveColorHue = parent.ActiveColorHue;
            HoverColorHue = parent.HoverColorHue;
            HREF = parent.HREF;
        }
    }
}
