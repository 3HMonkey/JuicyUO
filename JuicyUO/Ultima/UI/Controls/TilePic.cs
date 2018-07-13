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
using Microsoft.Xna.Framework.Graphics;
using System;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;

namespace JuicyUO.Ultima.UI.Controls
{
    /// <summary>
    /// A gump that shows a static item.
    /// </summary>
    class StaticPic : AControl
    {
        Texture2D m_texture;
        int Hue;
        int m_tileID;

        StaticPic(AControl parent)
            : base(parent)
        {

        }

        public StaticPic(AControl parent, string[] arguements)
            : this(parent)
        {
            int x, y, tileID, hue = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            tileID = Int32.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE="XXX" arguement!
                hue = Int32.Parse(arguements[4]);
            }
            BuildGumpling(x, y, tileID, hue);
        }

        public StaticPic(AControl parent, int x, int y, int itemID, int hue)
            : this(parent)
        {
            BuildGumpling(x, y, itemID, hue);
        }

        void BuildGumpling(int x, int y, int tileID, int hue)
        {
            Position = new Point(x, y);
            Hue = hue;
            m_tileID = tileID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_texture == null)
            {
                IResourceProvider provider = Service.Get<IResourceProvider>();
                m_texture = provider.GetItemTexture(m_tileID);
                Size = new Point(m_texture.Width, m_texture.Height);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            spriteBatch.Draw2D(m_texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(0, false, false, true));
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
