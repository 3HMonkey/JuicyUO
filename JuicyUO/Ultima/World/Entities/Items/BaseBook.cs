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

using JuicyUO.Ultima.World.Maps;

namespace JuicyUO.Ultima.World.Entities.Items
{
    public class BaseBook : Item
    {
        string m_Title;
        string m_Author;
        BookPageInfo[] m_Pages;
        bool m_IsEditable;

        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        public string Author
        {
            get { return m_Author; }
            set { m_Author = value; }
        }

        public bool IsEditable
        {
            get { return m_IsEditable; }
            set
            {
                m_IsEditable = value;
            }
        }

        public int PageCount => m_Pages.Length;

        public BookPageInfo[] Pages
        {
            get { return m_Pages; }
            set
            {
                m_Pages = value;
                m_OnUpdated?.Invoke(this);
            }
        }

        public BaseBook(Serial serial, Map map) 
            : this(serial, map, true)
        {
        }

        public BaseBook(Serial serial, Map map, bool writable)
            : this(serial, map, writable, null, null)
        {
        }

        public BaseBook(Serial serial, Map map, bool writable, string title, string author)
            : base(serial, map)
        {
            m_Title = title;
            m_Author = author;
            m_IsEditable = writable;
            m_Pages = new BookPageInfo[0];
        }

        public class BookPageInfo
        {
            string[] m_Lines;
            public string[] Lines
            {
                get
                {
                    return m_Lines;
                }
                set
                {
                    m_Lines = value;
                }
            }

            public BookPageInfo()
            {
                m_Lines = new string[0];
            }

            public BookPageInfo(string[] lines)
            {
                m_Lines = lines;
            }

            public string GetAllLines() => string.Join("\n", m_Lines);
        }
    }
}