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
using JuicyUO.Core.ComponentModel;
#endregion

namespace JuicyUO.Configuration.Properties
{
    /// <summary>
    /// A class that describes a resolution width height pair.
    /// </summary>
    public class ResolutionProperty : NotifyPropertyChangedBase
    {
        int m_Height;
        int m_Width;

        public ResolutionProperty()
        {
            Width = 800;
            Height = 600;
        }

        public ResolutionProperty(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height
        {
            get { return m_Height; }
            set { SetProperty(ref m_Height, value); }
        }

        public int Width
        {
            get { return m_Width; }
            set { SetProperty(ref m_Width, value); }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", m_Width, m_Height);
        }
    }
}