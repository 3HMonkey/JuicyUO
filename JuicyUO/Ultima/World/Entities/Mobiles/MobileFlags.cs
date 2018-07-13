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

using System;

namespace JuicyUO.Ultima.World.Entities.Mobiles
{
    [Flags]
    public enum MobileFlag
    {
        None = 0x00,
        Female = 0x02,
        Poisoned = 0x04,
        Blessed = 0x08,
        Warmode = 0x40,
        Hidden = 0x80,
    }

    public class MobileFlags
    {
        /// <summary>
        /// These are the only flags sent by RunUO
        /// 0x02 = female
        /// 0x04 = poisoned
        /// 0x08 = blessed/yellow health bar
        /// 0x40 = warmode
        /// 0x80 = hidden
        /// </summary>
        private MobileFlag m_flags;

        public bool IsFemale { get { return ((m_flags & MobileFlag.Female) != 0); } }
        public bool IsPoisoned { get { return ((m_flags & MobileFlag.Poisoned) != 0); } }
        public bool IsBlessed { get { return ((m_flags & MobileFlag.Blessed) != 0); } }
        public bool IsWarMode
        {
            get { return ((m_flags & MobileFlag.Warmode) != 0); }
            set
            {
                if (value == true)
                {
                    m_flags |= MobileFlag.Warmode;
                }
                else
                {
                    m_flags &= ~MobileFlag.Warmode;
                }
            }
        }
        public bool IsHidden { get { return ((m_flags & MobileFlag.Hidden) != 0); } }

        public MobileFlags(MobileFlag flags)
        {
            m_flags = flags;
        }

        public MobileFlags()
        {
            m_flags = (MobileFlag)0;
        }
    }
}
