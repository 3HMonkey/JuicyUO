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

using JuicyUO.Core.UI;
using Microsoft.Xna.Framework;
using JuicyUO.Core.Graphics;

namespace JuicyUO.Ultima.UI.Controls
{
    class GumpPicWithWidth : AGumpPic
    {
        float m_PercentWidthDrawn = 1.0f;

        /// <summary>
        /// The percent of this gump pic's width which is drawn. Clipped to 0.0f to 1.0f.
        /// </summary>
        public float PercentWidthDrawn
        {
            get
            {
                return m_PercentWidthDrawn;
            }
            set
            {
                if (value < 0f)
                    value = 0f;
                else if (value > 1f)
                    value = 1f;
                m_PercentWidthDrawn = value;
            }
        }

        public GumpPicWithWidth(AControl parent, int x, int y, int gumpID, int hue, float percentWidth)
            : base(parent)
        {
            BuildGumpling(x, y, gumpID, hue);
            PercentWidthDrawn = percentWidth;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            Vector3 hueVector = Utility.GetHueVector(Hue);
            int width = (int)(m_PercentWidthDrawn * Width);
            spriteBatch.Draw2D(m_Texture, new Rectangle(position.X, position.Y, width, Height), new Rectangle(0, 0, width, Height), hueVector);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
