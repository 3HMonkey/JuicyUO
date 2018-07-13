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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using JuicyUO.Core.UI.Fonts;
using JuicyUO.Ultima.IO;
using JuicyUO.Ultima.Resources.Fonts;
#endregion

namespace JuicyUO.Ultima.Resources
{
    public class FontsResource
    {
        bool m_Initialized;
        GraphicsDevice m_GraphicsDevice;

        public const int UniFontCount = 3;
        readonly AFont[] m_UnicodeFonts = new AFont[UniFontCount];

        public const int AsciiFontCount = 10;
        readonly AFont[] m_AsciiFonts = new AFont[AsciiFontCount];

        internal AFont GetUniFont(int index)
        {
            if (index < 0 || index >= UniFontCount)
                return m_UnicodeFonts[0];
            return m_UnicodeFonts[index];
        }

        internal AFont GetAsciiFont(int index)
        {
            if (index < 0 || index >= AsciiFontCount)
                return m_AsciiFonts[9];
            return m_AsciiFonts[index];
        }
       
        public FontsResource(GraphicsDevice graphics)
        {
            m_GraphicsDevice = graphics;
            Initialize();
        }

        void Initialize()
        {
            if (!m_Initialized)
            {
                m_Initialized = true;
                m_GraphicsDevice.DeviceReset -= GraphicsDeviceReset;
                m_GraphicsDevice.DeviceReset += GraphicsDeviceReset;
                LoadFonts();
            }
        }

        void GraphicsDeviceReset(object sender, EventArgs e)
        {
            LoadFonts();
        }

        void LoadFonts()
        {
            // load Ascii fonts
            using (BinaryReader reader = new BinaryReader(new FileStream(FileManager.GetFilePath("fonts.mul"), FileMode.Open, FileAccess.Read)))
            {
                for (int i = 0; i < AsciiFontCount; i++)
                {
                    m_AsciiFonts[i] = new FontAscii();
                    m_AsciiFonts[i].Initialize(reader);
                    m_AsciiFonts[i].HasBuiltInOutline = true;
                }
            }
            // load Unicode fonts
            int maxHeight = 0; // because all unifonts are designed to be used together, they must all share a single maxheight value.
            for (int i = 0; i < UniFontCount; i++)
            {
                string path = FileManager.GetFilePath("unifont" + (i == 0 ? "" : i.ToString()) + ".mul");
                if (path != null)
                {
                    m_UnicodeFonts[i] = new FontUnicode();
                    m_UnicodeFonts[i].Initialize(new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    if (m_UnicodeFonts[i].Height > maxHeight)
                    {
                        maxHeight = m_UnicodeFonts[i].Height;
                    }
                }
            }
            for (int i = 0; i < UniFontCount; i++)
            {
                if (m_UnicodeFonts[i] == null)
                {
                    continue;
                }
                m_UnicodeFonts[i].Height = maxHeight;
            }
        }
    }
}
