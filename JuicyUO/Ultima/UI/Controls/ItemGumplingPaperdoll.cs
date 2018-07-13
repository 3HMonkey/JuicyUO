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
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.World.Entities.Items;

namespace JuicyUO.Ultima.UI.Controls
{
    class ItemGumplingPaperdoll : ItemGumpling
    {
        public int SlotIndex;
        public bool IsFemale;

        int m_HueOverride;
        int m_GumpIndex;

        public ItemGumplingPaperdoll(AControl parent, int x, int y, Item item)
            : base(parent, item)
        {
            Position = new Point(x, y);
            HighlightOnMouseOver = false;
        }

        protected override Point InternalGetPickupOffset(Point offset)
        {
            return offset;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            if (m_Texture == null)
            {
                IResourceProvider provider = Service.Get<IResourceProvider>();
                m_GumpIndex = Item.ItemData.AnimID + (IsFemale ? 60000 : 50000);
                int indexTranslated, hueTranslated;
                if (GumpDefTranslator.ItemHasGumpTranslation(m_GumpIndex, out indexTranslated, out hueTranslated))
                {
                    m_GumpIndex = indexTranslated;
                    m_HueOverride = hueTranslated;
                }
                m_Texture = provider.GetUITexture(m_GumpIndex);
                Size = new Point(m_Texture.Width, m_Texture.Height);
            }
            int hue = (Item.Hue == 0 & m_HueOverride != 0) ? m_HueOverride : Item.Hue;
            spriteBatch.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0), Utility.GetHueVector(hue));
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            IResourceProvider provider = Service.Get<IResourceProvider>();
            return provider.IsPointInUITexture(m_GumpIndex, x, y);
        }
    }
}
