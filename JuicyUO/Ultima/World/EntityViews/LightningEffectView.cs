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
using JuicyUO.Ultima.World.Entities.Effects;
using JuicyUO.Ultima.World.Input;
using JuicyUO.Ultima.World.Maps;

namespace JuicyUO.Ultima.World.EntityViews
{
    class LightningEffectView : AEntityView
    {
        LightningEffect Effect => (LightningEffect)Entity;

        int m_DisplayItemID = -1;

        public LightningEffectView(LightningEffect effect)
            : base(effect)
        {

        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            CheckDefer(map, drawPosition);

            return DrawInternal(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }

        public override bool DrawInternal(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            int displayItemdID = 0x4e20 + Effect.FramesActive;
            if (displayItemdID > 0x4e29)
            {
                return false;
            }
            if (displayItemdID != m_DisplayItemID)
            {
                m_DisplayItemID = displayItemdID;
                DrawTexture = Provider.GetUITexture(displayItemdID);
                Point offset = s_Offsets[m_DisplayItemID - 20000];
                DrawArea = new Rectangle(offset.X, DrawTexture.Height - 33 + (Entity.Z * 4) + offset.Y, DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickNothing;
                DrawFlip = false;
            }

            // Update hue vector.
            HueVector = Utility.GetHueVector(Entity.Hue);

            return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }

        static readonly Point[] s_Offsets = {
                new Point(48, 0),
                new Point(68, 0),
                new Point(92, 0),
                new Point(72, 0),
                new Point(48, 0),
                new Point(56, 0),
                new Point(76, 0),
                new Point(76, 0),
                new Point(92, 0),
                new Point(80, 0)
            };
    }
}
