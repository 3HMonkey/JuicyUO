﻿#region license
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
using System;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.UI;

namespace JuicyUO.Ultima.UI.Controls
{
    class GumpPic : AGumpPic
    {
        public GumpPic(AControl parent, string[] arguements)
            : base(parent)
        {
            int x, y, gumpID, hue = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID = Int32.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE=XXX arguement (and potentially a CLASS=XXX argument).
                string hueArgument = arguements[4].Substring(arguements[4].IndexOf('=') + 1);
                hue = Int32.Parse(hueArgument);
            }
            BuildGumpling(x, y, gumpID, hue);
        }

        public GumpPic(AControl parent, int x, int y, int gumpID, int hue)
            : base(parent)
        {
            BuildGumpling(x, y, gumpID, hue);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            Vector3 hueVector = Utility.GetHueVector(Hue);
            spriteBatch.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0), hueVector);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}