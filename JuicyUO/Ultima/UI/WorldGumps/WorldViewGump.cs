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
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    /// <summary>
    /// A bordered container that displays the world control.
    /// </summary>
    class WorldViewGump : Gump
    {
        private WorldModel m_Model;

        private WorldViewport m_Viewport;
        private ResizePic m_Border;
        private ChatControl m_ChatWindow;

        private const int BorderWidth = 5, BorderHeight = 7;
        private int m_WorldWidth, m_WorldHeight;

        public WorldViewGump()
            : base(0, 0)
        {
            HandlesMouseInput = false;
            IsUncloseableWithRMB = true;
            IsUncloseableWithEsc = true;
            IsMoveable = true;
            MetaData.Layer = UILayer.Under;

            m_Model = Service.Get<WorldModel>();

            m_WorldWidth = Settings.UserInterface.PlayWindowGumpResolution.Width;
            m_WorldHeight = Settings.UserInterface.PlayWindowGumpResolution.Height;

            Position = new Point(32, 32);
            OnResize();
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("worldview");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_WorldWidth != Settings.UserInterface.PlayWindowGumpResolution.Width || m_WorldHeight != Settings.UserInterface.PlayWindowGumpResolution.Height)
            {
                m_WorldWidth = Settings.UserInterface.PlayWindowGumpResolution.Width;
                m_WorldHeight = Settings.UserInterface.PlayWindowGumpResolution.Height;
                OnResize();
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnMove()
        {
            // base.OnMove() would make sure that the gump remained at least half on screen, but we want more fine-grained control over movement.
            SpriteBatchUI sb = Service.Get<SpriteBatchUI>();
            Point position = Position;

            if (position.X < -BorderWidth)
                position.X = -BorderWidth;
            if (position.Y < -BorderHeight)
                position.Y = -BorderHeight;
            if (position.X + Width - BorderWidth > sb.GraphicsDevice.Viewport.Width)
                position.X = sb.GraphicsDevice.Viewport.Width - (Width - BorderWidth);
            if (position.Y + Height - BorderHeight > sb.GraphicsDevice.Viewport.Height)
                position.Y = sb.GraphicsDevice.Viewport.Height - (Height - BorderHeight);

            Position = position;
        }

        private void OnResize()
        {
            if (Service.Has<ChatControl>())
                Service.Remove<ChatControl>();

            ClearControls();

            Size = new Point(m_WorldWidth + BorderWidth * 2, m_WorldHeight + BorderHeight * 2);
            AddControl(m_Border = new ResizePic(this, 0, 0, 0xa3c, Width, Height));
            AddControl(m_Viewport = new WorldViewport(this, BorderWidth, BorderHeight, m_WorldWidth, m_WorldHeight));
            AddControl(m_ChatWindow = new ChatControl(this, BorderWidth, BorderHeight, m_WorldWidth, m_WorldHeight));
            Service.Add<ChatControl>(m_ChatWindow);
        }
    }
}
