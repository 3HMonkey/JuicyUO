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
    class CroppedText : AControl
    {
        public int Hue;
        public string Text = string.Empty;
        RenderedText m_Texture;

        CroppedText(AControl parent)
            : base(parent)
        {

        }

        public CroppedText(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            int x, y, width, height, hue, textIndex;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            hue = Int32.Parse(arguements[5]);
            textIndex = Int32.Parse(arguements[6]);
            BuildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        public CroppedText(AControl parent, int x, int y, int width, int height, int hue, int textIndex, string[] lines)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        void BuildGumpling(int x, int y, int width, int height, int hue, int textIndex, string[] lines)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            Hue = hue;
            Text = lines[textIndex];
            m_Texture = new RenderedText(Text, width);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            m_Texture.Draw(spriteBatch, new Rectangle(position.X, position.Y, Width, Height), 0, 0);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
