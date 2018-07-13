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
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Input;
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.EntityViews
{
    class OverheadView : AEntityView
    {
        new Overhead Entity => (Overhead)base.Entity;
        RenderedText m_Text;

        public OverheadView(Overhead entity)
            : base(entity)
        {
            m_Text = new RenderedText(Entity.Text, collapseContent: true);
            DrawTexture = m_Text.Texture;
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            HueVector = Utility.GetHueVector(Entity.Hue, false, false, true);
            return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
        }
    }
}
