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
using JuicyUO.Ultima.World.Entities;
#endregion

namespace JuicyUO.Ultima.UI
{
    class Tooltip
    {
        public string Caption
        {
            get;
            protected set;
        }

        RenderedText m_RenderedText;

        int m_PropertyListHash;
        AEntity m_Entity;

        public Tooltip(string caption)
        {
            m_Entity = null;
            Caption = caption;
        }

        public Tooltip(AEntity entity)
        {
            m_Entity = entity;
            m_PropertyListHash = m_Entity.PropertyList.Hash;
            Caption = m_Entity.PropertyList.Properties;
        }

        public void Dispose()
        {
            Caption = null;
        }

        public void Draw(SpriteBatchUI spriteBatch, int x, int y)
        {
            // determine if properties need to be updated.
            if (m_Entity != null && m_PropertyListHash != m_Entity.PropertyList.Hash)
            {
                m_PropertyListHash = m_Entity.PropertyList.Hash;
                Caption = m_Entity.PropertyList.Properties;
            }

            // update text if necessary.
            if (m_RenderedText == null)
            {
                m_RenderedText = new RenderedText("<center>" + Caption, 300, true);
            }
            else if (m_RenderedText.Text != "<center>" + Caption)
            {
                m_RenderedText = null;
                m_RenderedText = new RenderedText("<center>" + Caption, 300, true);
            }

            // draw checkered trans underneath.
            spriteBatch.Draw2DTiled(CheckerTrans.CheckeredTransTexture, new Rectangle(x - 4, y - 4, m_RenderedText.Width + 8, m_RenderedText.Height + 8), Vector3.Zero);
            // draw tooltip contents
            m_RenderedText.Draw(spriteBatch, new Point(x, y));
        }

        internal void UpdateEntity(AEntity entity)
        {
            if (m_Entity == null || m_Entity != entity || m_PropertyListHash != m_Entity.PropertyList.Hash)
            {
                m_Entity = entity;
                m_PropertyListHash = m_Entity.PropertyList.Hash;
                Caption = m_Entity.PropertyList.Properties;
            }
        }

        internal void UpdateCaption(string caption)
        {
            m_Entity = null;
            Caption = caption;
        }
    }
}
