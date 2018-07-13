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
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Graphics;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public class HuedTexture
    {
        private readonly Texture2D m_Texture;
        private readonly Rectangle m_SourceRect = Rectangle.Empty;

        private Point m_Offset;
        public Point Offset
        {
            set { m_Offset = value; }
            get { return m_Offset; }
        }
        
        private int m_Hue;
        public int Hue
        {
            set { m_Hue = value; }
            get { return m_Hue; }
        }

        public HuedTexture(Texture2D texture, Point offset, Rectangle source, int hue)
        {
            m_Texture = texture;
            m_Offset = offset;
            m_SourceRect = source;
            m_Hue = hue;
        }

        public void Draw(SpriteBatchUI sb, Point position)
        {
            Vector3 v = new Vector3(position.X - m_Offset.X, position.Y - m_Offset.Y, 0);
            sb.Draw2D(m_Texture, v, m_SourceRect, Utility.GetHueVector(m_Hue));
        }
    }
}
