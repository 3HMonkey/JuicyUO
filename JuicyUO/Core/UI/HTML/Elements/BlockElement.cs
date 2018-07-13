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

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JuicyUO.Core.UI.HTML.Styles;

namespace JuicyUO.Core.UI.HTML.Elements
{
    /// <summary>
    /// Blocks fit their content. They can be assigned width, height, and alignment.
    /// </summary>
    class BlockElement : AElement
    {
        public List<AElement> Children = new List<AElement>();
        public BlockElement Parent;

        public string Tag;

        private Rectangle m_Area;

        public Rectangle Area
        {
            get { return m_Area; }
            set { m_Area = value; }
        }

        public override int Width
        {
            get
            {
                return m_Area.Width;
            }
            set
            {
                m_Area.Width = value;
            }
        }

        public override int Height
        {
            get
            {
                return m_Area.Height;
            }
            set
            {
                m_Area.Height = value;
            }
        }

        public Alignments Alignment = Alignments.Default;

        public int Layout_MinWidth;
        public int Layout_MaxWidth;

        public BlockElement(string tag, StyleState style)
            : base(style)
        {
            Tag = tag;
        }

        public void AddAtom(AElement atom)
        {
            Children.Add(atom);
            if (atom is BlockElement)
                (atom as BlockElement).Parent = this;
        }

        public override string ToString() => Tag;

        public bool Err_Cant_Fit_Children;
    }
}
