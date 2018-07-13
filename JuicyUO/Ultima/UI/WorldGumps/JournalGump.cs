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
using JuicyUO.Core.Graphics;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Player;
using JuicyUO.Ultima.UI.Controls;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class JournalGump : Gump
    {
        private ExpandableScroll m_Background;
        private RenderedTextList m_JournalEntries;
        private readonly IScrollBar m_ScrollBar;

        public JournalGump()
            : base(0, 0)
        {
            IsMoveable = true;

            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 300));
            m_Background.TitleGumpID = 0x82A;

            m_ScrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(m_JournalEntries = new RenderedTextList(this, 30, 36, 242, 200, m_ScrollBar));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("journal");

            InitializeJournalEntries();
            PlayerState.Journaling.OnJournalEntryAdded += AddJournalEntry;
        }

        public override void Dispose()
        {
            PlayerState.Journaling.OnJournalEntryAdded -= AddJournalEntry;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            m_JournalEntries.Height = Height - 98;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        private void AddJournalEntry(JournalEntry entry)
        {
            string text = string.Format("{0}{1}", entry.SpeakerName != string.Empty ? entry.SpeakerName + ": " : string.Empty, entry.Text);
            int font = entry.Font;
            bool asUnicode = entry.AsUnicode;
            TransformFont(ref font, ref asUnicode);

            m_JournalEntries.AddEntry(string.Format(
                "<span color='#{3}' style='font-family:{1}{2};'>{0}</span>", text, asUnicode ? "uni" : "ascii", font,
                // "<span color='#{1}' style='font-family:ascii9;'>{0}</span>", text, 
                Utility.GetColorFromUshort(Resources.HueData.GetHue(entry.Hue, 0))));
        }

        private void TransformFont(ref int font, ref bool asUnicode)
        {
            if (asUnicode)
                return;
            else
            {
                switch (font)
                {
                    case 3:
                        {
                            font = 1;
                            asUnicode = true;
                            break;
                        }
                }
            }
        }

        private void InitializeJournalEntries()
        {
            for (int i = 0; i < PlayerState.Journaling.JournalEntries.Count; i++)
            {
                AddJournalEntry(PlayerState.Journaling.JournalEntries[i]);
            }

            m_ScrollBar.MinValue = 0;
        }
    }
}
