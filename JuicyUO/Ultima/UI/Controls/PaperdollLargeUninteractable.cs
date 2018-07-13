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

using Microsoft.Xna.Framework;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Data;

namespace JuicyUO.Ultima.UI.Controls
{
    class PaperdollLargeUninteractable : AControl
    {
        public enum EquipSlots
        {
            Body = 0, First = Body,
            RightHand = 1,
            LeftHand = 2,
            Footwear = 3,
            Legging = 4,
            Shirt = 5,
            Head = 5,
            Gloves = 7,
            Ring = 8,
            Talisman = 9,
            Neck = 10,
            Hair = 11,
            Belt = 12,
            Chest = 13,
            Bracelet = 14,
            // skip 15, unused
            FacialHair = 16,
            Sash = 17,
            Earring = 18,
            Sleeves = 19,
            Back = 20,
            Backpack = 21,
            Robe = 22,
            Skirt = 23,
            // skip 24, inner legs (!!! do we really skip this?)
            Max = 23,
        }

        int[] m_equipmentSlots = new int[(int)EquipSlots.Max];
        readonly int[] m_hueSlots = new int[(int)EquipSlots.Max];

        bool m_isFemale;
        public int Gender { set { m_isFemale = (value == 1) ? true : false; } }
        bool m_isElf;
        public int Race { set { m_isElf = (value == 1) ? true : false; } }

        public bool IsCharacterCreation;

        public void SetSlotEquipment(EquipSlots slot, int gumpID)
        {
            m_equipmentSlots[(int)slot] = gumpID;
        }
        public void SetSlotHue(EquipSlots slot, int gumpID)
        {
            m_hueSlots[(int)slot] = gumpID;
        }

        public int this[EquipSlots slot]
        {
            set { m_equipmentSlots[(int)slot] = value; }
            get { return m_equipmentSlots[(int)slot]; }
        }

        PaperdollLargeUninteractable(AControl parent)
            : base(parent)
        {

        }

        public PaperdollLargeUninteractable(AControl parent, int x, int y)
            : this(parent)
        {
            Position = new Point(x, y);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            EquipSlots[] slotsToDraw = { EquipSlots.Body, EquipSlots.Footwear, EquipSlots.Legging, EquipSlots.Shirt, EquipSlots.Hair, EquipSlots.FacialHair };
            for (int i = 0; i < slotsToDraw.Length; i++)
            {
                int bodyID = 0;
                int hue = hueSlot(slotsToDraw[i]);
                bool hueGreyPixelsOnly = true;

                switch (slotsToDraw[i])
                {
                    case EquipSlots.Body:
                        if (m_isElf)
                            bodyID = (m_isFemale ? 1893 : 1894);
                        else
                            bodyID = (m_isFemale ? 1888 : 1889);
                        break;
                    case EquipSlots.Footwear:
                        bodyID = (m_isFemale ? 1891 : 1890);
                        hue = 900;
                        break;
                    case EquipSlots.Legging:
                        bodyID = (m_isFemale ? 1892 : 1848);
                        hue = 348;
                        break;
                    case EquipSlots.Shirt:
                        bodyID = (m_isFemale ? 1812 : 1849);
                        hue = 792;
                        break;
                    case EquipSlots.Hair:
                        if (equipmentSlot(EquipSlots.Hair) != 0)
                        {
                            bodyID = m_isFemale ?
                                HairStyles.FemaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair)) :
                                HairStyles.MaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair));
                            hueGreyPixelsOnly = false;
                        }
                        break;
                    case EquipSlots.FacialHair:
                        if (equipmentSlot(EquipSlots.FacialHair) != 0)
                        {
                            bodyID = m_isFemale ?
                                0 : HairStyles.FacialHairGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.FacialHair));
                            hueGreyPixelsOnly = false;
                        }
                        break;
                }

                if (bodyID != 0)
                {
                    // this is silly, we should be keeping a local copy of the body texture.
                    IResourceProvider provider = Service.Get<IResourceProvider>();
                    spriteBatch.Draw2D(provider.GetUITexture(bodyID), new Vector3(position.X, position.Y, 0), Utility.GetHueVector(hue, hueGreyPixelsOnly, false, false));
                }
            }
        }

        int equipmentSlot(EquipSlots slotID)
        {
            return m_equipmentSlots[(int)slotID];
        }

        int hueSlot(EquipSlots slotID)
        {
            return m_hueSlots[(int)slotID];
        }
    }
}
