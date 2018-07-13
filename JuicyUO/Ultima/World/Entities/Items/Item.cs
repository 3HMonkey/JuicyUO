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
using Microsoft.Xna.Framework;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.Resources;
using JuicyUO.Ultima.World.Entities.Items.Containers;
using JuicyUO.Ultima.World.Entities.Mobiles;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Maps;
#endregion

namespace JuicyUO.Ultima.World.Entities.Items
{
    public class Item : AEntity
    {
        public AEntity Parent;

        public override string Name
        {
            get
            {
                return ItemData.Name;
            }
        }

        public override Position3D Position
        {
            get
            {
                if (Parent != null)
                    return Parent.Position;
                else
                    return base.Position;
            }
        }

        public Point InContainerPosition = Point.Zero;

        public Item(Serial serial, Map map)
            : base(serial, map)
        {

        }

        public override void Dispose()
        {
            base.Dispose();
            // if is worn, let the wearer know we are disposing.
            if (Parent is Mobile)
                ((Mobile)Parent).RemoveItem(Serial);
            else if (Parent is ContainerItem)
                ((ContainerItem)Parent).RemoveItem(Serial);
        }

        protected override AEntityView CreateView()
        {
            return new ItemView(this);
        }

        private int m_amount;
        public int Amount
        {
            get
            {
                return m_amount;
            }
            set
            {
                m_amount = value;
            }
        }

        public ItemData ItemData;

        private int m_ItemID;
        private int? m_DisplayItemID;

        public int ItemID
        {
            get { return m_ItemID; }
            set
            {
                m_ItemID = value;
                ItemData = TileData.ItemData[m_ItemID & 0xFFFF]; // TODO: Does this work on both legacy and UOP clients?
            }
        }

        public int DisplayItemID
        {
            get
            {
                if (m_DisplayItemID.HasValue)
                    return m_DisplayItemID.Value;
                if (IsCoin)
                {
                    if (Amount > 5)
                    {
                        return m_ItemID + 2;
                    }
                    else if (Amount > 1)
                    {
                        return m_ItemID + 1;
                    }
                }
                return m_ItemID;
            }
            set
            {
                m_DisplayItemID = value;
            }
        }

        public bool NoDraw
        {
            get { return m_ItemID <= 1 || m_DisplayItemID <= 1; } // no draw
        }

        public bool IsCoin
        {
            get { return m_ItemID >= 0xEEA && m_ItemID <= 0xEF2; }
        }

        public int ContainerSlotIndex;

        public override void Update(double frameMS)
        {
            if (WorldView.AllLabels && !(this is StaticItem) && (Parent == null) && (ItemData.Weight != 255))
            {
                AddOverhead(MessageTypes.Label, Name, 3, 0, false);
            }

            base.Update(frameMS);
        }

        public override string ToString()
        {
            return base.ToString() + " | " + ItemData.Name;
        }

        public bool AtWorldPoint(int x, int y)
        {
            if (Position.X == x && Position.Y == y)
                return true;
            else
                return false;
        }

        public virtual bool TryPickUp()
        {
            if (ItemData.Weight == 255)
                return false;
            else
                return true;
        }

        // ============================================================================================================
        // Last Parent routines 
        // ============================================================================================================

        private AEntity m_lastParent;
        public bool HasLastParent
        {
            get { return (m_lastParent != null); }
        }

        public void SaveLastParent()
        {
            m_lastParent = Parent;
        }

        public void RestoreLastParent()
        {
            if (m_lastParent != null)
            {
                ((ContainerItem)m_lastParent).AddItem(this);
            }
        }
    }
}
