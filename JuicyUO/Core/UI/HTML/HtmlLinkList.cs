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
using System.Collections.Generic;
using JuicyUO.Core.UI.HTML.Styles;

namespace JuicyUO.Core.UI.HTML
{
    public class HtmlLinkList
    {
        public static HtmlLinkList Empty => new HtmlLinkList();

        readonly List<HtmlLink> m_Links = new List<HtmlLink>();

        public HtmlLink this[int index]
        {
            get
            {
                if (m_Links.Count == 0)
                    return null;
                if (index >= m_Links.Count)
                    index = m_Links.Count - 1;
                if (index < 0)
                    index = 0;
                return m_Links[index];
            }
        }

        public int Count
        {
            get { return m_Links.Count; }
        }

        public HtmlLink AddLink(StyleState style, Rectangle area)
        {
            HtmlLink matched = null;
            foreach (HtmlLink link in m_Links)
            {
                if (link.Style.HREF == style.HREF && link.Area.Right == area.Left)
                {
                    bool overlap = link.Area.Top < area.Bottom && area.Top < link.Area.Bottom;
                    if (overlap)
                    {
                        matched = link;
                        matched.Area.Width = matched.Area.Width + area.Width;
                        if (matched.Area.Y > area.Y)
                            matched.Area.Y = area.Y;
                        if (matched.Area.Bottom < area.Bottom)
                            matched.Area.Height += (area.Bottom - matched.Area.Bottom);
                        break;
                    }
                }
            }

            if (matched == null)
            {
                m_Links.Add(new HtmlLink(m_Links.Count, style));
                matched = m_Links[m_Links.Count - 1];
                matched.Area = area;
            }
            return matched;
        }

        public void Clear()
        {
            m_Links.Clear();
        }

        public HtmlLink RegionfromPoint(Point p)
        {
            int index = -1;
            for (int i = 0; i < m_Links.Count; i++)
            {
                if (m_Links[i].Area.Contains(p))
                    index = i;
            }
            if (index == -1)
                return null;
            else
                return m_Links[index];
        }

        public HtmlLink Region(int index)
        {
            return m_Links[index];
        }
    }
}
