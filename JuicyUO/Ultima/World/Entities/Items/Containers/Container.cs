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
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.Entities.Items.Containers
{
    public class ContainerItem : Item
    {
        List<Item> m_Contents;
        bool m_ContentsUpdated;

        public List<Item> Contents
        {
            get
            {
                if (m_Contents == null)
                    m_Contents = new List<Item>();
                return m_Contents;
            }
        }

        public ContainerItem(Serial serial, Map map)
            : base(serial, map)
        {
            m_ContentsUpdated = true;
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (m_ContentsUpdated)
            {
                m_OnUpdated?.Invoke(this);
                m_ContentsUpdated = false;
            }
        }

        public override void Dispose()
        {
            for (int i = 0; i < Contents.Count; i++)
            {
                Contents[i].Dispose();
            }
            base.Dispose();
        }

        public void AddItem(Item item)
        {
            if (!Contents.Contains(item))
            {
                Contents.Add(item);
                item.Parent = this;
            }
            m_ContentsUpdated = true;
        }

        public virtual void RemoveItem(Serial serial)
        {
            foreach (Item item in Contents)
            {
                if (item.Serial == serial)
                {
                    item.SaveLastParent();
                    Contents.Remove(item);
                    break;
                }
            }
            m_ContentsUpdated = true;
        }
    }
}
