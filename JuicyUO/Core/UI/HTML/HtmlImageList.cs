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

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace JuicyUO.Core.UI.HTML
{
    class HtmlImageList
    {
        public static HtmlImageList Empty = new HtmlImageList();
        readonly List<HtmlImage> m_Images = new List<HtmlImage>();

        public HtmlImage this[int index]
        {
            get
            {
                if (m_Images.Count == 0)
                    return null;
                if (index >= m_Images.Count)
                    index = m_Images.Count - 1;
                if (index < 0)
                    index = 0;
                return m_Images[index];
            }
        }

        public int Count
        {
            get { return m_Images.Count; }
        }

        public void AddImage(Rectangle area, Texture2D image)
        {
            m_Images.Add(new HtmlImage(area, image));
        }

        public void AddImage(Rectangle area, Texture2D image, Texture2D overimage, Texture2D downimage)
        {
            AddImage(area, image);
            m_Images[m_Images.Count - 1].TextureOver = overimage;
            m_Images[m_Images.Count - 1].TextureDown = downimage;
        }

        public void Clear()
        {
            foreach (HtmlImage image in m_Images)
            {
                image.Dispose();
            }
            m_Images.Clear();
        }
    }

    
}
