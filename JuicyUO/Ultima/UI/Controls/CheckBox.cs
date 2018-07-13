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
using Microsoft.Xna.Framework.Graphics;
using System;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    /// <summary>
    /// A checkbox control.
    /// </summary>
    class CheckBox : AControl
    {
        Texture2D m_Inactive, m_Active;
        bool m_ischecked;

        public bool IsChecked
        {
            get { return m_ischecked; }
            set
            {
                m_ischecked = value;
            }
        }

        CheckBox(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public CheckBox(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            int x, y, inactiveID, activeID, switchID;
            bool initialState;

            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            inactiveID = Int32.Parse(arguements[3]);
            activeID = Int32.Parse(arguements[4]);
            initialState = Int32.Parse(arguements[5]) == 1;
            switchID = Int32.Parse(arguements[6]);

            BuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public CheckBox(AControl parent, int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
            : this(parent)
        {
            BuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
            if (IsChecked && m_Active != null)
            {
                spriteBatch.Draw2D(m_Active, new Vector3(position.X, position.Y, 0), Vector3.Zero);
            }
            else if (!IsChecked && m_Inactive != null)
            {
                spriteBatch.Draw2D(m_Inactive, new Vector3(position.X, position.Y, 0), Vector3.Zero);
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            IsChecked = !IsChecked;
        }

        void BuildGumpling(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
        {
            IResourceProvider provider = Service.Get<IResourceProvider>();
            m_Inactive = provider.GetUITexture(inactiveID);
            m_Active = provider.GetUITexture(activeID);

            Position = new Point(x, y);
            Size = new Point(m_Inactive.Width, m_Inactive.Height);
            IsChecked = initialState;
            GumpLocalID = switchID;
        }
    }
}