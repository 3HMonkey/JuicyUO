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
using JuicyUO.Core.Graphics;
using JuicyUO.Ultima.IO;
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.World.Entities.Effects;
using JuicyUO.Ultima.World.Input;
using JuicyUO.Ultima.World.Maps;
using JuicyUO.Ultima.World.WorldViews;

namespace JuicyUO.Ultima.World.EntityViews
{
    public class MovingEffectView : AEntityView
    {
        MovingEffect Effect => (MovingEffect)Entity;

        EffectData m_AnimData;
        readonly bool m_Animated;
        int m_DisplayItemID = -1;

        public MovingEffectView(MovingEffect effect)
            : base(effect)
        {
            m_Animated = TileData.ItemData[Effect.ItemID & FileManager.ItemIDMask].IsAnimation;
            if (m_Animated)
            {
                m_AnimData = Provider.GetResource<EffectData>(Effect.ItemID);
                m_Animated = m_AnimData.FrameCount > 0;
            }
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            CheckDefer(map, drawPosition);
            return DrawInternal(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            int displayItemdID = (m_Animated) ? Effect.ItemID + ((Effect.FramesActive / m_AnimData.FrameInterval) % m_AnimData.FrameCount) : Effect.ItemID;
            if (displayItemdID != m_DisplayItemID)
            {
                m_DisplayItemID = displayItemdID;
                DrawTexture = Provider.GetItemTexture(m_DisplayItemID);
                DrawArea = new Rectangle(DrawTexture.Width / 2 - IsometricRenderer.TILE_SIZE_INTEGER_HALF, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER, DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }
            DrawArea.X = 0 - (int)((Entity.Position.X_offset - Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            DrawArea.Y = 0 + (int)((Entity.Position.Z_offset + Entity.Z) * 4) - (int)((Entity.Position.X_offset + Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            Rotation = Effect.AngleToTarget;
            HueVector = Utility.GetHueVector(Entity.Hue);
            return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }
    }
}
