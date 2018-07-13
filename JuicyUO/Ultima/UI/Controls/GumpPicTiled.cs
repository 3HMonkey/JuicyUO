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
    class GumpPicTiled : AControl
    {
        Texture2D m_bgGump;
        int m_gumpID;

        GumpPicTiled(AControl parent)
            : base(parent)
        {

        }

        public GumpPicTiled(AControl parent, string[] arguements)
            : this(parent)
        {
            int x, y, gumpID, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            gumpID = Int32.Parse(arguements[5]);
            BuildGumpling(x, y, width, height, gumpID);
        }

        public GumpPicTiled(AControl parent, int x, int y, int width, int height, int gumpID)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, gumpID);
        }

        void BuildGumpling(int x, int y, int width, int height, int gumpID)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            m_gumpID = gumpID;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_bgGump == null)
            {
                IResourceProvider provider = Service.Get<IResourceProvider>();
                m_bgGump = provider.GetUITexture(m_gumpID);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            spriteBatch.Draw2DTiled(m_bgGump, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
