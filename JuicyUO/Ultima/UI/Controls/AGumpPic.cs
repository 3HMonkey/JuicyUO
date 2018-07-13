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
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;

namespace JuicyUO.Ultima.UI.Controls
{
    abstract class AGumpPic : AControl
    {
        protected Texture2D m_Texture;
        private int m_LastFrameGumpID = -1;

        internal int GumpID
        {
            get;
            set;
        }

        internal int Hue
        {
            get;
            set;
        }

        internal bool IsPaperdoll
        {
            get;
            set;
        }

        public AGumpPic(AControl parent)
            : base(parent)
        {
            MakeThisADragger();
        }

        protected void BuildGumpling(int x, int y, int gumpID, int hue)
        {
            Position = new Point(x, y);
            GumpID = gumpID;
            Hue = hue;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_Texture == null || GumpID != m_LastFrameGumpID)
            {
                m_LastFrameGumpID = GumpID;
                IResourceProvider provider = Service.Get<IResourceProvider>();
                m_Texture = provider.GetUITexture(GumpID);
                Size = new Point(m_Texture.Width, m_Texture.Height);
            }
            base.Update(totalMS, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            IResourceProvider provider = Service.Get<IResourceProvider>();
            return provider.IsPointInUITexture(GumpID, x, y);
        }
    }
}