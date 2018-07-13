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
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Input;
using JuicyUO.Ultima.World.Maps;
using JuicyUO.Ultima.World.WorldViews;

namespace JuicyUO.Ultima.World.EntityViews
{
    class ItemView : AEntityView
    {
        new Item Entity
        {
            get { return (Item)base.Entity; }
        }

        int m_DisplayItemID = -1;

        public ItemView(Item item)
            : base(item)
        {
            if (Entity.ItemData.IsWet)
            {
                SortZ += 1;
            }
        }

        protected override void Pick(MouseOverList mouseOver, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            int x = mouseOver.MousePosition.X - (int)vertexBuffer[0].Position.X;
            int y = mouseOver.MousePosition.Y - (int)vertexBuffer[0].Position.Y;
            if (Provider.IsPointInItemTexture(m_DisplayItemID, x, y, 1))
            {
                mouseOver.AddItem(Entity, vertexBuffer[0].Position);
            }
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (Entity.NoDraw)
            {
                return false;
            }
            // Update Display texture, if necessary.
            if (Entity.DisplayItemID != m_DisplayItemID)
            {
                m_DisplayItemID = Entity.DisplayItemID;
                DrawTexture = Provider.GetItemTexture(m_DisplayItemID);
                if (DrawTexture == null) // ' no draw ' item.
                {
                    return false;
                }
                DrawArea = new Rectangle(DrawTexture.Width / 2 - IsometricRenderer.TILE_SIZE_INTEGER_HALF, DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4), DrawTexture.Width, DrawTexture.Height);
                PickType = PickType.PickObjects;
                DrawFlip = false;
            }
            if (DrawTexture == null) // ' no draw ' item.
            {
                return false;
            }
            DrawArea.Y = DrawTexture.Height - IsometricRenderer.TILE_SIZE_INTEGER + (Entity.Z * 4);
            HueVector = Utility.GetHueVector(Entity.Hue, Entity.ItemData.IsPartialHue, false, false);
            if (Entity.Amount > 1 && Entity.ItemData.IsGeneric && Entity.DisplayItemID == Entity.ItemID)
            {
                int offset = Entity.ItemData.Unknown4;
                Vector3 offsetDrawPosition = new Vector3(drawPosition.X - 5, drawPosition.Y - 5, 0);
                base.Draw(spriteBatch, offsetDrawPosition, mouseOver, map, roofHideFlag);
            }
            bool drawn = base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
            DrawOverheads(spriteBatch, drawPosition, mouseOver, map, DrawArea.Y - IsometricRenderer.TILE_SIZE_INTEGER_HALF);
            return drawn;
        }
    }
}
