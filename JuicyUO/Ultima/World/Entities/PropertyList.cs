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
using System.Collections.Generic;
using System.Text;
#endregion

namespace JuicyUO.Ultima.World.Entities
{
    public class PropertyList
    {
        public int Hash;
        private readonly List<string> m_PropertyList = new List<string>();

        public bool HasProperties
        {
            get
            {
                if (m_PropertyList.Count == 0)
                    return false;
                else
                    return true;
            }
        }

        public string Properties
        {
            get
            {
                StringBuilder concat = new StringBuilder();
                for (int i = 0; i < m_PropertyList.Count; i++)
                {
                    concat.Append(m_PropertyList[i]);
                    if (i < m_PropertyList.Count - 1)
                    {
                        concat.Append('\n');
                    }
                }
                return concat.ToString();
            }
        }

        public void Clear()
        {
            m_PropertyList.Clear();
        }

        public void AddProperty(string nProperty)
        {
            m_PropertyList.Add(nProperty);
        }
    }
}
