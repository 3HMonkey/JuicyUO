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
using JuicyUO.Core.Input;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World;
using JuicyUO.Ultima.World.Entities.Items;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class SplitItemStackGump : Gump
    {
        public Item Item
        {
            get;
            private set;
        }

        private readonly Point m_PickupOffset;

        private readonly HSliderBar m_Slider;
        private TextEntry m_AmountEntry;
        private Button m_OKButton;
        private int m_LastValue;

        public SplitItemStackGump(Item item, Point pickupOffset)
            : base(0, 0)
        {
            Item = item;
            m_PickupOffset = pickupOffset;

            IsMoveable = true;

            // Background
            AddControl(new GumpPic(this, 0, 0, 0x085c, 0));
            // Slider
            m_Slider = (HSliderBar)AddControl(new HSliderBar(this, 30, 16, 104, 0, item.Amount, item.Amount, HSliderBarStyle.BlueWidgetNoBar));
            m_LastValue = m_Slider.Value;
            // Ok button
            AddControl(m_OKButton = new Button(this, 102, 38, 0x085d, 0x085e, ButtonTypes.Default, 0, 0));
            m_OKButton.GumpOverID = 0x085f;
            m_OKButton.MouseClickEvent += ClickOkayButton;
            // Text entry field
            m_AmountEntry = (TextEntry)AddControl(new TextEntry(this, 30, 39, 60, 16, 0, 0, 5, item.Amount.ToString()));
            m_AmountEntry.LeadingHtmlTag = "<big>";
            m_AmountEntry.LegacyCarat = true;
            m_AmountEntry.Hue = 1001;
            m_AmountEntry.ReplaceDefaultTextOnFirstKeypress = true;
            m_AmountEntry.NumericOnly = true;
        }

        public override void Dispose()
        {
            m_OKButton.MouseClickEvent -= ClickOkayButton;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            // update children controls first
            base.Update(totalMS, frameMS);

            // update strategy: if slider != last value, then set text equal to slider value. else try parsing text.
            //                  if text is empty, value = minvalue.
            //                  if can't parse text, then set text equal to slider value.
            //                  if can parse text, and text != slider, then set slider = text.
            if (m_Slider.Value != m_LastValue)
            {
                m_AmountEntry.Text = m_Slider.Value.ToString();
            }
            else
            {
                int textValue;
                if (m_AmountEntry.Text.Length == 0)
                {
                    m_Slider.Value = m_Slider.MinValue;
                }
                else if (!int.TryParse(m_AmountEntry.Text, out textValue))
                {
                    m_AmountEntry.Text = m_Slider.Value.ToString();
                }
                else
                {
                    if (textValue != m_Slider.Value)
                    {
                        if (textValue <= m_Slider.MaxValue)
                            m_Slider.Value = textValue;
                        else
                        {
                            m_Slider.Value = m_Slider.MaxValue;
                            m_AmountEntry.Text = m_Slider.Value.ToString();
                        }
                    }
                }
            }
            m_LastValue = m_Slider.Value;
        }

        private void ClickOkayButton(AControl sender, int x, int y, MouseButton button)
        {
            if (m_Slider.Value > 0)
            {
                WorldModel world = Service.Get<WorldModel>();
                world.Interaction.PickupItem(Item, m_PickupOffset, m_Slider.Value);
            }
            Dispose();
        }

        public override void OnKeyboardReturn(int textID, string text)
        {
            if (m_Slider.Value > 0)
            {
                WorldModel world = Service.Get<WorldModel>();
                world.Interaction.PickupItem(Item, m_PickupOffset, m_Slider.Value);
            }
            Dispose();
        }
    }
}
