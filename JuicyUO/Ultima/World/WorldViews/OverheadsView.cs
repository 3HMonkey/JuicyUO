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
using System.Collections.Generic;
using JuicyUO.Core.Graphics;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Input;
using JuicyUO.Ultima.World.Maps;

namespace JuicyUO.Ultima.World.WorldViews
{
    static class OverheadsView
    {
        private static List<ViewWithDrawInfo> m_Views = new List<ViewWithDrawInfo>();

        public static void AddView(AEntityView view, Vector3 drawPosition)
        {
            m_Views.Add(new ViewWithDrawInfo(view, drawPosition));
        }

        public static void Render(SpriteBatch3D spriteBatch, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (m_Views.Count > 0)
            {
                for (int i = 0; i < m_Views.Count; i++)
                {
                    m_Views[i].View.Draw(spriteBatch, m_Views[i].DrawPosition, mouseOver, map, roofHideFlag);
                }

                m_Views.Clear();
            }
        }

        private struct ViewWithDrawInfo
        {
            public readonly AEntityView View;
            public readonly Vector3 DrawPosition;

            public ViewWithDrawInfo(AEntityView view, Vector3 drawPosition)
            {
                View = view;
                DrawPosition = drawPosition;
            }
        }
    }
}
