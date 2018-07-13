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
using Microsoft.Xna.Framework;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.UI;

namespace JuicyUO.Ultima.UI.Controls
{
    public class TextLabel : AControl
    {
        private const int DefaultLabelWidth = 300;

        public int Hue
        {
            get;
            set;
        }

        private string m_Text;
        public string Text
        {
            get { return m_Text; }
            set
            {
                if (m_textRenderer == null)
                {
                    m_textRenderer = new RenderedText(m_Text, DefaultLabelWidth);
                }
                m_textRenderer.Text = m_Text = value;
            }
        }

        RenderedText m_textRenderer;

        TextLabel(AControl parent)
            : base(parent)
        {

        }

        public TextLabel(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            int x, y, hue, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            hue = ServerRecievedHueTransform(Int32.Parse(arguements[3]));
            textIndex = Int32.Parse(arguements[4]);
            BuildGumpling(x, y, hue, lines[textIndex]);
        }

        public TextLabel(AControl parent, int x, int y, int hue, string text)
            : this(parent)
        {
            BuildGumpling(x, y, hue, text);
        }

        void BuildGumpling(int x, int y, int hue, string text)
        {
            Position = new Point(x, y);
            Text = string.Format("{0}{1}", (hue == 0 ? string.Empty : "<outline>"), text);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            m_textRenderer.Draw(spriteBatch, position, Utility.GetHueVector(Hue, false, false, true));
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
