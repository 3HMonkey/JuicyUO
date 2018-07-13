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
using JuicyUO.Ultima.World.Input;
using JuicyUO.Ultima.World.Maps;
using JuicyUO.Ultima.World.Entities.Mobiles;
using JuicyUO.Ultima.World.Entities;

namespace JuicyUO.Ultima.World.EntityViews
{
    public class DeferredView : AEntityView
    {
        readonly Vector3 m_DrawPosition;
        readonly AEntityView m_BaseView;

        public DeferredView(DeferredEntity entity, Vector3 drawPosition, AEntityView baseView)
            : base(entity)
        {
            m_DrawPosition = drawPosition;
            m_BaseView = baseView;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (m_BaseView.Entity is Mobile)
            { 
                Mobile mobile = m_BaseView.Entity as Mobile;
                if (!mobile.IsAlive || mobile.IsDisposed || mobile.Body == 0)
                {
                    Entity.Dispose();
                    return false;
                }
            }
            /*m_BaseView.SetYClipLine(m_DrawPosition.Y - 22 -
                ((m_BaseView.Entity.Position.Z + m_BaseView.Entity.Position.Z_offset) * 4) +
                ((m_BaseView.Entity.Position.X_offset + m_BaseView.Entity.Position.Y_offset) * IsometricRenderer.TILE_SIZE_INTEGER_HALF));*/
            bool success = m_BaseView.DrawInternal(spriteBatch, m_DrawPosition, mouseOver, map, roofHideFlag);
            /*m_BaseView.ClearYClipLine();*/
            return success;
        }
    }
}
