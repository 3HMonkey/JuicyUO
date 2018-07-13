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
using System.Collections.Generic;
using System.Windows.Forms;
using JuicyUO.Configuration.Properties;
#endregion

namespace JuicyUO.Core
{
    /// <summary>
    /// Contains a list of all valid resolutions, and the code to change the size of the rendered window.
    /// </summary>
    class Resolutions
    {
        public static readonly List<ResolutionProperty> FullScreenResolutionsList;
        public static readonly List<ResolutionProperty> PlayWindowResolutionsList;
        public const int MAX_BUFFER_SIZE = 2056;

        public static void SetWindowSize(GameWindow window)
        {
            Rectangle game;
            System.Drawing.Rectangle screen;

            if (window != null)
            {
                game = window.ClientBounds;
                screen = Screen.GetWorkingArea(new System.Drawing.Rectangle(game.X, game.Y, game.Width, game.Height));
            }
            else
            {
                screen = Screen.GetWorkingArea(new System.Drawing.Point(0, 0));
            }

            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Format != SurfaceFormat.Color)
                    continue;
                ResolutionProperty res = new ResolutionProperty(mode.Width, mode.Height);
                if (res.Width <= MAX_BUFFER_SIZE && res.Height <= MAX_BUFFER_SIZE)
                {
                    if (!FullScreenResolutionsList.Contains(res))
                    {
                        FullScreenResolutionsList.Add(res);
                    }
                }
            }

            foreach (ResolutionProperty res in FullScreenResolutionsList)
            {
                if (!PlayWindowResolutionsList.Contains(res) 
                    && res.Width <= screen.Width && res.Height <= screen.Height)
                {
                    PlayWindowResolutionsList.Add(res);
                }
            }
        }

        static Resolutions()
        {
            FullScreenResolutionsList = new List<ResolutionProperty>();
            PlayWindowResolutionsList = new List<ResolutionProperty>();

            SetWindowSize(null);
        }

        public static bool IsValidFullScreenResolution(ResolutionProperty resolution)
        {
            foreach (ResolutionProperty res in FullScreenResolutionsList)
                if (resolution.Width == res.Width && resolution.Height == res.Height)
                    return true;
            return false;
        }

        public static bool IsValidPlayWindowResolution(ResolutionProperty resolution)
        {
            foreach (ResolutionProperty res in PlayWindowResolutionsList)
                if (resolution.Width == res.Width && resolution.Height == res.Height)
                    return true;
            return false;
        }
    }
}
