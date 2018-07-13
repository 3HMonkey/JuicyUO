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

using JuicyUO.Core.UI.HTML.Styles;

namespace JuicyUO.Core.UI.HTML.Elements
{
    public class CharacterElement : AElement
    {
        public override int Width
        {
            get
            {
                if (Character < 32)
                {
                    return 0;
                }
                ICharacter ch = Style.Font.GetCharacter(Character);
                return ch.Width + ch.ExtraWidth + (Style.IsBold ? 1 : 0);
            }
            set
            {
                // does nothing
            }
        }

        public override int Height
        {
            get
            {
                return Style.Font.Height;
            }
            set
            {
                // does nothing
            }
        }

        public override bool CanBreakAtThisAtom
        {
            get
            {
                if (Character == ' ' || Character == '\n')
                {
                    return true;
                }
                return false;
            }
        }

        public override bool IsThisAtomABreakingSpace
        {
            get
            {
                if (Character == ' ')
                {
                    return true;
                }
                return false;
            }
        }

        public override bool IsThisAtomALineBreak
        {
            get
            {
                if (Character == '\n')
                {
                    return true;
                }
                return false;
            }
        }

        public char Character;

        public CharacterElement(StyleState style, char c)
            : base(style)
        {
            Character = c;
        }

        public override string ToString()
        {
            if (IsThisAtomALineBreak)
            {
                return @"\n";
            }
            return Character.ToString();
        }
    }
}
