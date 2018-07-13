﻿#region license
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
using JuicyUO.Core.Input;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.World;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Entities.Items.Containers;
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    class PaperDollInteractable : Gump
    {
        bool m_isFemale;
        bool m_isElf;
        GumpPicBackpack m_Backpack;

        WorldModel m_World;

        public PaperDollInteractable(AControl parent, int x, int y, Mobile sourceEntity)
            : base(0, 0)
        {
            Parent = parent;
            Position = new Point(x, y);
            m_isFemale = sourceEntity.Flags.IsFemale;
            SourceEntity = sourceEntity;
            m_World = Service.Get<WorldModel>();
        }

        public override void Dispose()
        {
            m_sourceEntity.ClearCallBacks(OnEntityUpdated, OnEntityDisposed);
            if (m_Backpack != null)
            {
                m_Backpack.MouseDoubleClickEvent -= On_Dblclick_Backpack;
            }
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_sourceEntity != null)
            {
                m_isFemale = ((Mobile)m_sourceEntity).Flags.IsFemale;
                m_isElf = false;
            }
            base.Update(totalMS, frameMS);
        }

        void OnEntityUpdated(AEntity entity)
        {
            ClearControls();
            // Add the base gump - the semi-naked paper doll.
            if (true)
            {
                int bodyID = 12 + (m_isElf ? 2 : 0) + (m_isFemale ? 1 : 0); // ((Mobile)m_sourceEntity).BodyID;
                GumpPic paperdoll = (GumpPic)AddControl(new GumpPic(this, 0, 0, bodyID, ((Mobile)m_sourceEntity).Hue));
                paperdoll.HandlesMouseInput = true;
                paperdoll.IsPaperdoll = true;
            }
            // Loop through the items on the mobile and create the gump pics.
            for (int i = 0; i < s_DrawOrder.Length; i++)
            {
                Item item = ((Mobile)m_sourceEntity).GetItem((int)s_DrawOrder[i]);
                if (item == null)
                    continue;

                bool canPickUp = true;
                switch (s_DrawOrder[i])
                {
                    case PaperDollEquipSlots.FacialHair:
                    case PaperDollEquipSlots.Hair:
                        canPickUp = false;
                        break;
                    default:
                        break;
                }

                AddControl(new ItemGumplingPaperdoll(this, 0, 0, item));
                ((ItemGumplingPaperdoll)LastControl).SlotIndex = (int)i;
                ((ItemGumplingPaperdoll)LastControl).IsFemale = m_isFemale;
                ((ItemGumplingPaperdoll)LastControl).CanPickUp = canPickUp;
            }
            // If this object has a backpack, add it last.
            if (((Mobile)m_sourceEntity).GetItem((int)PaperDollEquipSlots.Backpack) != null)
            {
                Item backpack = ((Mobile)m_sourceEntity).GetItem((int)PaperDollEquipSlots.Backpack);
                AddControl(m_Backpack = new GumpPicBackpack(this, -7, 0, backpack));
                m_Backpack.HandlesMouseInput = true;
                m_Backpack.MouseDoubleClickEvent += On_Dblclick_Backpack;
            }
        }

        void OnEntityDisposed(AEntity entity)
        {
            Dispose();
        }

        void On_Dblclick_Backpack(AControl control, int x, int y, MouseButton button)
        {
            ContainerItem backpack = ((Mobile)m_sourceEntity).Backpack;
            m_World.Interaction.DoubleClick(backpack);
        }

        AEntity m_sourceEntity;
        public AEntity SourceEntity
        {
            set
            {
                if (value != m_sourceEntity)
                {
                    if (m_sourceEntity != null)
                    {
                        m_sourceEntity.ClearCallBacks(OnEntityUpdated, OnEntityDisposed);
                        m_sourceEntity = null;
                    }
                    if (value is Mobile)
                    {
                        m_sourceEntity = value;
                        // update the gump
                        OnEntityUpdated(m_sourceEntity);
                        // if the entity changes in the future, update the gump again
                        m_sourceEntity.SetCallbacks(OnEntityUpdated, OnEntityDisposed);
                    }
                }
            }
            get
            {
                return m_sourceEntity;
            }
        }

        enum PaperDollEquipSlots
        {
            Body = 0,
            RightHand = 1,
            LeftHand = 2,
            Footwear = 3,
            Legging = 4,
            Shirt = 5,
            Head = 6,
            Gloves = 7,
            Ring = 8,
            Talisman = 9,
            Neck = 10,
            Hair = 11,
            Belt = 12,
            Chest = 13,
            Bracelet = 14,
            Unused = 15,
            FacialHair = 16,
            Sash = 17,
            Earring = 18,
            Sleeves = 19,
            Back = 20,
            Backpack = 21,
            Robe = 22,
            Skirt = 23,
            // skip 24, inner legs (!!! do we really skip this?)
        }

        static PaperDollEquipSlots[] s_DrawOrder = {
            PaperDollEquipSlots.Footwear,
            PaperDollEquipSlots.Legging,
            PaperDollEquipSlots.Shirt,
            PaperDollEquipSlots.Sleeves,
            PaperDollEquipSlots.Gloves,
            PaperDollEquipSlots.Ring,
            PaperDollEquipSlots.Talisman,
            PaperDollEquipSlots.Neck,
            PaperDollEquipSlots.Belt,
            PaperDollEquipSlots.Chest,
            PaperDollEquipSlots.Bracelet,
            PaperDollEquipSlots.Hair,
            PaperDollEquipSlots.FacialHair,
            PaperDollEquipSlots.Head,
            PaperDollEquipSlots.Sash,
            PaperDollEquipSlots.Earring,
            PaperDollEquipSlots.Back,
            PaperDollEquipSlots.Skirt,
            PaperDollEquipSlots.Robe,
            PaperDollEquipSlots.LeftHand,
            PaperDollEquipSlots.RightHand
        };
    }
}
