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

using JuicyUO.Core.UI.HTML.Styles;

namespace JuicyUO.Core.UI.HTML.Elements
{
    public class ImageElement : AElement
    {
        public HtmlImage AssociatedImage
        {
            get;
            set;
        }

        public int ImgSrc = -1;
        public int ImgSrcOver = -1;
        public int ImgSrcDown = -1;

        private int m_Width, m_Height;

        public override int Width
        {
            set
            {
                m_Width = value;
            }
            get
            {
                if (m_Width != 0)
                    return m_Width;
                return AssociatedImage.Texture.Width;
            }
        }

        public override int Height
        {
            set
            {
                m_Height = value;
            }
            get
            {
                if (m_Height != 0)
                    return m_Height;
                return AssociatedImage.Texture.Height;
            }
        }

        public ImageTypes ImageType
        {
            get;
            private set;
        }

        public ImageElement(StyleState style, ImageTypes imageType = ImageTypes.UI)
            : base(style)
        {
            ImageType = imageType;
        }

        public override string ToString()
        {
            return string.Format("<img {0} {1}>", ImgSrc, ImageType.ToString());
        }

        public enum ImageTypes
        {
            UI,
            Item
        }
    }
}
