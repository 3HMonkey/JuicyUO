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
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Entities.Items.Containers;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class ContainerGump : Gump
    {
        ContainerData m_data;
        ContainerItem m_item;

        public ContainerGump(AEntity containerItem, int gumpID)
            : base(containerItem.Serial, 0)
        {
            m_data = ContainerData.Get(gumpID);
            m_item = (ContainerItem)containerItem;
            m_item.SetCallbacks(OnItemUpdated, OnItemDisposed);
            IsMoveable = true;
            AddControl(new GumpPicContainer(this, 0, 0, m_data.GumpID, 0, m_item));
        }

        public override void Dispose()
        {
            m_item.ClearCallBacks(OnItemUpdated, OnItemDisposed);
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        void OnItemUpdated(AEntity entity)
        {
            // delete any items in our pack that are no longer in the container.
            List<AControl> ControlsToRemove = new List<AControl>();
            foreach (AControl c in Children)
            {
                if (c is ItemGumpling && !m_item.Contents.Contains(((ItemGumpling)c).Item))
                {
                    ControlsToRemove.Add(c);
                }
            }
            foreach (AControl c in ControlsToRemove)
                Children.Remove(c);

            // add any items in the container that are not in our pack.
            foreach (Item item in m_item.Contents)
            {
                bool controlForThisItem = false;
                foreach (AControl c in Children)
                {
                    if (c is ItemGumpling && ((ItemGumpling)c).Item == item)
                    {
                        controlForThisItem = true;
                        break;
                    }
                }
                if (!controlForThisItem)
                {
                    AddControl(new ItemGumpling(this, item));
                }
            }
        }

        void OnItemDisposed(AEntity entity)
        {
            Dispose();
        }
    }
}
