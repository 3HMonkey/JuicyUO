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

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.UI;

namespace JuicyUO.Ultima.UI.Controls
{
    class CheckerTrans : AControl
    {
        private static Texture2D s_CheckeredTransTexture;
        public static Texture2D CheckeredTransTexture
        {
            get
            {
                if (s_CheckeredTransTexture == null)
                {
                    ushort[] data = new ushort[32 * 32];
                    for (int h = 0; h < 32; h++)
                    {
                        int i = h % 2;
                        for (int w = 0; w < 32; w++)
                        {
                            if (i++ >= 1)
                            {
                                data[h * 32 + w] = 0x8000;
                                i = 0;
                            }
                            else
                            {
                                data[h * 32 + w] = 0x0000;
                            }
                        }
                    }
                    SpriteBatchUI sb = Service.Get<SpriteBatchUI>();
                    s_CheckeredTransTexture = new Texture2D(sb.GraphicsDevice, 32, 32, false, SurfaceFormat.Bgra5551);
                    s_CheckeredTransTexture.SetData<ushort>(data);
                }
                return s_CheckeredTransTexture;
            }
        }

        CheckerTrans(AControl parent)
            : base(parent)
        {

        }

        public CheckerTrans(AControl parent, string[] arguements)
            : this(parent)
        {
            int x, y, width, height;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);

            BuildGumpling(x, y, width, height);
        }

        public CheckerTrans(AControl parent, int x, int y, int width, int height)
            : this(parent)
        {
            BuildGumpling(x, y, width, height);
        }

        void BuildGumpling(int x, int y, int width, int height)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            spriteBatch.Draw2DTiled(CheckeredTransTexture, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
