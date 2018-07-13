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

using System.Linq;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.World.Maps;

namespace JuicyUO.Ultima.World.Entities.Items.Containers
{
    class SpellBook : ContainerItem
    {
        static ushort[] m_SpellBookItemIDs = {
            0xE3B, // bugged or static item spellbook? Not wearable.
            0xEFA, // standard, wearable spellbook
            0x2252, // paladin
            0x2253, // necro
            0x238C, // bushido
            0x23A0, // ninjitsu
            0x2D50 // spellweaving
        };

        /// <summary>
        ///  Returns true if the parameter ItemID matches a Spellbook Item.
        /// </summary>
        /// <param name="itemID">An itemID to be tested</param>
        /// <returns>True if the itemID is a Spellbook ite, false otherwise.</returns>
        public static bool IsSpellBookItem(ushort itemID)
        {
            return m_SpellBookItemIDs.Contains<ushort>(itemID);
        }

        public SpellBookTypes BookType
        {
            get;
            private set;
        }

        ulong m_SpellsBitfield;
        public bool HasSpell(int circle, int index)
        {
            index = ((3 - circle % 4) + (circle / 4) * 4) * 8 + (index - 1);
            ulong flag = ((ulong)1) << index;
            return (m_SpellsBitfield & flag) == flag;
        }

        public SpellBook(Serial serial, Map map)
            : base(serial, map)
        {
            BookType = SpellBookTypes.Unknown;
            m_SpellsBitfield = 0;
        }

        public void ReceiveSpellData(SpellBookTypes sbType, ulong sbBitfield)
        {
            bool entityUpdated = false;
            if (BookType != sbType)
            {
                BookType = sbType;
                entityUpdated = true;
            }

            if (m_SpellsBitfield != sbBitfield)
            {
                m_SpellsBitfield = sbBitfield;
                entityUpdated = true;
            }
            if (entityUpdated)
            {
                m_OnUpdated?.Invoke(this);
            }
        }
    }
}