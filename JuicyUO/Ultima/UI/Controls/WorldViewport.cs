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
using JuicyUO.Core.UI;
using JuicyUO.Ultima.World;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    /// <summary>
    /// A control that shows the current isometric view around the player.
    /// </summary>
    class WorldViewport : AControl
    {
        private WorldModel m_Model;

        public Point MousePosition;

        private Vector2 m_InputMultiplier = Vector2.One;

        public WorldViewport(AControl parent, int x, int y, int width, int height)
            : base(parent)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);

            HandlesMouseInput = true;
            Service.Add<WorldViewport>(this);
        }

        protected override void OnInitialize()
        {
            m_Model = Service.Get<WorldModel>();
        }

        public override void Dispose()
        {
            Service.Remove<WorldViewport>();
            base.Dispose();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            Texture2D worldTexture = (m_Model.GetView() as WorldView).Isometric.Texture;
            if (worldTexture == null)
                return;

            m_InputMultiplier = new Vector2((float)worldTexture.Width / Width, (float)worldTexture.Height / Height);

            spriteBatch.Draw2D(worldTexture, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnMouseOver(int x, int y)
        {
            MousePosition = new Point((int)(x * m_InputMultiplier.X), (int)(y * m_InputMultiplier.Y));
        }
    }
}
