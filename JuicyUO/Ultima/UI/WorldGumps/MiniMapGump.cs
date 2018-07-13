﻿#region license
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
using System;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Core.Resources;
using JuicyUO.Ultima.World;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Core.UI;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class MiniMapGump : Gump
    {
        const float ReticleBlinkMS = 250f;

        float m_TimeMS;
        bool m_UseLargeMap;
        WorldModel m_World;
        Texture2D m_GumpTexture;
        Texture2D m_PlayerIndicator;

        public static bool MiniMap_LargeFormat
        {
            get;
            set;
        }

        public static void Toggle()
        {
            UserInterfaceService ui = Service.Get<UserInterfaceService>();
            if (ui.GetControl<MiniMapGump>() == null)
            {
                ui.AddControl(new MiniMapGump(), 566, 25);
            }
            else
            {
                if (MiniMapGump.MiniMap_LargeFormat == false)
                {
                    MiniMapGump.MiniMap_LargeFormat = true;
                }
                else
                {
                    ui.RemoveControl<MiniMapGump>();
                    MiniMapGump.MiniMap_LargeFormat = false;
                }
            }
        }

        public MiniMapGump()
            : base(0, 0)
        {
            m_World = Service.Get<WorldModel>();

            m_UseLargeMap = MiniMap_LargeFormat;

            IsMoveable = true;
            MakeThisADragger();
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("minimap");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_GumpTexture == null || m_UseLargeMap != MiniMap_LargeFormat)
            {
                m_UseLargeMap = MiniMap_LargeFormat;
                if (m_GumpTexture != null)
                {
                    m_GumpTexture = null;
                }
                IResourceProvider provider = Service.Get<IResourceProvider>();
                m_GumpTexture = provider.GetUITexture((m_UseLargeMap ? 5011 : 5010), true);
                Size = new Point(m_GumpTexture.Width, m_GumpTexture.Height);
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            AEntity player = WorldModel.Entities.GetPlayerEntity();
            float x = (float)Math.Round((player.Position.X % 256) + player.Position.X_offset) / 256f;
            float y = (float)Math.Round((player.Position.Y % 256) + player.Position.Y_offset) / 256f;
            Vector3 playerPosition = new Vector3(x - y, x + y, 0f);
            float minimapU = (m_GumpTexture.Width / 256f) / 2f;
            float minimapV = (m_GumpTexture.Height / 256f) / 2f;

            VertexPositionNormalTextureHue[] v = 
            {
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y, 0), playerPosition + new Vector3(-minimapU, -minimapV, 0), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + Width, position.Y, 0), playerPosition + new Vector3(minimapU, -minimapV, 0), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y + Height, 0), playerPosition + new Vector3(-minimapU, minimapV, 0), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + Width, position.Y + Height, 0), playerPosition + new Vector3(minimapU, minimapV, 0), new Vector3(1, 1, 0))
            };

            spriteBatch.DrawSprite(m_GumpTexture, v, Techniques.MiniMap);

            m_TimeMS += (float)frameMS;
            if (m_TimeMS >= ReticleBlinkMS)
            {
                if (m_PlayerIndicator == null)
                {
                    m_PlayerIndicator = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    m_PlayerIndicator.SetData(new uint[1] { 0xFFFFFFFF });
                }
                spriteBatch.Draw2D(m_PlayerIndicator, new Vector3(position.X + Width / 2, position.Y + Height / 2 - 8, 0), Vector3.Zero);
            }
            if (m_TimeMS >= ReticleBlinkMS * 2)
            {
                m_TimeMS -= ReticleBlinkMS * 2;
            }
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                MiniMap_LargeFormat = !MiniMap_LargeFormat;
            }
        }
    }
}
