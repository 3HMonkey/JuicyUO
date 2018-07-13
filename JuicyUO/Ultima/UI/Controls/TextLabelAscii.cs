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
using Microsoft.Xna.Framework;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.UI;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    class TextLabelAscii : AControl
    {
        public int Hue;
        public int FontID;

        private readonly RenderedText m_Rendered;
        private string m_Text;
        private int m_Width;

        public string Text
        {
            get
            {
                return m_Text;
            }
            set
            {
                if (m_Text != value)
                {
                    m_Text = value;
                    m_Rendered.Text = string.Format("<span style=\"font-family:ascii{0}\">{1}", FontID, m_Text);
                }
            }
        }

        public TextLabelAscii(AControl parent, int width = 400)
            : base(parent)
        {
            m_Width = width;
            m_Rendered = new RenderedText(string.Empty, m_Width, true);
        }

        public TextLabelAscii(AControl parent, int x, int y, int font, int hue, string text, int width = 400)
            : this(parent, width)
        {
            BuildGumpling(x, y, font, hue, text);
        }

        void BuildGumpling(int x, int y, int font, int hue, string text)
        {
            Position = new Point(x, y);
            Hue = hue;
            FontID = font;
            Text = text;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            m_Rendered.Draw(spriteBatch, position, Utility.GetHueVector(Hue, true, false, true));
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
