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
using System;
using JuicyUO.Core.Resources;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.Login.Data;
using JuicyUO.Ultima.UI.Controls;
#endregion

namespace JuicyUO.Ultima.UI.LoginGumps {
    class CreateCharAppearanceGump : Gump {
        enum Buttons {
            BackButton,
            ForwardButton,
            QuitButton
        }

        event Action m_OnForward;
        event Action m_OnBackward;

        public string Name { get { return m_TxtName.Text; } set { m_TxtName.Text = value; } }
        public int Gender { get { return m_Gender.Index; } set { } }
        public int Race { get { return 1; } set { } } // hard coded to human
        public int HairID {
            get { return (Gender == 0) ? HairStyles.MaleIDs[m_HairMale.Index] : HairStyles.FemaleIDs[m_HairFemale.Index]; }
            set {
                for (int i = 0; i < 10; i++) {
                    if (value == ((Gender == 0) ? HairStyles.MaleIDs[i] : HairStyles.FemaleIDs[i])) {
                        m_HairMale.Index = i;
                        m_HairFemale.Index = i;
                        break;
                    }
                }
            }
        }
        public int FacialHairID {
            get { return (Gender == 0) ? HairStyles.FacialHairIDs[m_FacialHairMale.Index] : 0; }
            set {
                for (int i = 0; i < 8; i++) {
                    if (value == HairStyles.FacialHairIDs[i]) {
                        m_FacialHairMale.Index = i;
                        break;
                    }
                }
            }
        }
        public int SkinHue {
            get { return m_SkinHue.HueValue; }
            set { m_SkinHue.HueValue = value; }
        }
        public int HairHue {
            get { return m_HairHue.HueValue; }
            set { m_HairHue.HueValue = value; }
        }
        public int FacialHairHue {
            get { return (Gender == 0) ? m_FacialHairHue.HueValue : 0; }
            set { m_FacialHairHue.HueValue = value; }
        }

        internal void RestoreData(CreateCharacterData m_Data) {
            Name = m_Data.Name;
            Gender = m_Data.Gender;
            HairID = m_Data.HairStyleID;
            FacialHairID = m_Data.FacialHairStyleID;
            SkinHue = m_Data.SkinHue;
            HairHue = m_Data.HairHue;
            FacialHairHue = m_Data.FacialHairHue;
        }

        TextEntry m_TxtName;
        DropDownList m_Gender;
        DropDownList m_HairMale;
        DropDownList m_FacialHairMale;
        DropDownList m_HairFemale;
        ColorPicker m_SkinHue;
        ColorPicker m_HairHue;
        ColorPicker m_FacialHairHue;
        PaperdollLargeUninteractable m_paperdoll;

        public CreateCharAppearanceGump(Action onForward, Action onBackward)
            : base(0, 0) {
            m_OnForward = onForward;
            m_OnBackward = onBackward;

            // get the resource provider
            IResourceProvider provider = Service.Get<IResourceProvider>();

            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // character name 
            AddControl(new GumpPic(this, 280, 53, 1801, 0));
            m_TxtName = new TextEntry(this, 238, 70, 234, 20, 0, 0, 29, string.Empty);
            m_TxtName.LeadingHtmlTag = "<span color='#000' style='font-family:uni0;'>";
            AddControl(new ResizePic(this, m_TxtName));
            AddControl(m_TxtName);
            // character window
            AddControl(new GumpPic(this, 238, 98, 1800, 0));
            // paperdoll
            m_paperdoll = new PaperdollLargeUninteractable(this, 237, 97);
            m_paperdoll.IsCharacterCreation = true;
            AddControl(m_paperdoll);

            // left option window
            AddControl(new ResizePic(this, 82, 125, 3600, 151, 310));
            // this is the place where you would put the race selector.
            // if you do add it, move everything else in this left window down by 45 pixels
            // gender
            AddControl(new TextLabelAscii(this, 100, 141, 9, 2036, provider.GetString(3000120)), 1);
            AddControl(m_Gender = new DropDownList(this, 97, 154, 122, new string[] { provider.GetString(3000118), provider.GetString(3000119) }, 2, 0, false));
            // hair (male)
            AddControl(new TextLabelAscii(this, 100, 186, 9, 2036, provider.GetString(3000121)), 1);
            AddControl(m_HairMale = new DropDownList(this, 97, 199, 122, HairStyles.MaleHairNames, 6, 0, false), 1);
            // facial hair (male)
            AddControl(new TextLabelAscii(this, 100, 231, 9, 2036, provider.GetString(3000122)), 1);
            AddControl(m_FacialHairMale = new DropDownList(this, 97, 244, 122, HairStyles.FacialHair, 6, 0, false), 1);
            // hair (female)
            AddControl(new TextLabelAscii(this, 100, 186, 9, 2036, provider.GetString(3000121)), 2);
            AddControl(m_HairFemale = new DropDownList(this, 97, 199, 122, HairStyles.FemaleHairNames, 6, 0, false), 2);

            // right option window
            AddControl(new ResizePic(this, 475, 125, 3600, 151, 310));
            // skin tone
            AddControl(new TextLabelAscii(this, 489, 141, 9, 2036, provider.GetString(3000183)));
            AddControl(m_SkinHue = new ColorPicker(this, new Rectangle(490, 154, 120, 24), new Rectangle(490, 140, 120, 280), 7, 8, Data.Hues.SkinTones));
            // hair color
            AddControl(new TextLabelAscii(this, 489, 186, 9, 2036, provider.GetString(3000184)));
            AddControl(m_HairHue = new ColorPicker(this, new Rectangle(490, 199, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones));
            // facial hair color (male)
            AddControl(new TextLabelAscii(this, 489, 231, 9, 2036, provider.GetString(3000185)), 1);
            AddControl(m_FacialHairHue = new ColorPicker(this, new Rectangle(490, 244, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones), 1);

            // back button
            AddControl(new Button(this, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton), 0);
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton), 0);
            ((Button)LastControl).GumpOverID = 5541;
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            IsUncloseableWithRMB = true;
        }

        internal void SaveData(CreateCharacterData data) {
            data.HasAppearanceData = true;
            data.Name = Name;
            data.Gender = Gender;
            data.HairStyleID = HairID;
            data.FacialHairStyleID = FacialHairID;
            data.SkinHue = SkinHue;
            data.HairHue = HairHue;
            data.FacialHairHue = FacialHairHue;
        }

        public override void Update(double totalMS, double frameMS) {
            base.Update(totalMS, frameMS);

            // show different controls based on what gender we're looking at.
            // Also copy over the hair id to facilitate easy switching between male and female appearances.
            if (m_Gender.Index == 0) {
                ActivePage = 1;
                m_HairFemale.Index = m_HairMale.Index;
            }
            else {
                ActivePage = 2;
                m_HairMale.Index = m_HairFemale.Index;
            }
            // update the paperdoll
            m_paperdoll.Gender = m_Gender.Index;
            m_paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Body, SkinHue);
            m_paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.Hair, HairID);
            m_paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Hair, HairHue);
            m_paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairID);
            m_paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairHue);
        }

        public override void OnButtonClick(int buttonID) {
            switch ((Buttons)buttonID) {
                case Buttons.BackButton:
                    m_OnBackward();
                    break;
                case Buttons.ForwardButton:
                    m_OnForward();
                    break;
                case Buttons.QuitButton:
                    Service.Get<UltimaGame>().Quit();
                    break;
            }
        }
    }
}
