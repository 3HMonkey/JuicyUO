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
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Core.UI;
using JuicyUO.Core.UI.HTML;
using JuicyUO.Core.Windows;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    class TextEntry : AControl
    {
        const float MSBetweenCaratBlinks = 500f;
        
        bool m_IsFocused;
        bool m_CaratBlinkOn;
        float m_MSSinceLastCaratBlink;
        RenderedText m_RenderedText;
        RenderedText m_RenderedCarat;

        public int Hue;
        public int EntryID;
        public int MaxCharCount;
        public bool IsPasswordField;
        public bool ReplaceDefaultTextOnFirstKeypress;
        public bool NumericOnly;
        public string LeadingHtmlTag;
        public string LeadingText;
        public string Text;
        public bool LegacyCarat;

        public override bool HandlesMouseInput => base.HandlesMouseInput & IsEditable;
        public override bool HandlesKeyboardFocus => base.HandlesKeyboardFocus & IsEditable;

        // ============================================================================================================
        // Ctors and BuildGumpling Methods
        // ============================================================================================================

        public TextEntry(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            int x, y, width, height, hue, entryID, textIndex, maxCharCount = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            width = Int32.Parse(arguements[3]);
            height = Int32.Parse(arguements[4]);
            hue = ServerRecievedHueTransform(Int32.Parse(arguements[5]));
            entryID = Int32.Parse(arguements[6]);
            textIndex = Int32.Parse(arguements[7]);
            if (arguements[0] == "textentrylimited")
            {
                maxCharCount = Int32.Parse(arguements[8]);
            }
            BuildGumpling(x, y, width, height, hue, entryID, maxCharCount, lines[textIndex]);
        }

        public TextEntry(AControl parent, int x, int y, int width, int height, int hue, int entryID, int maxCharCount, string text)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, hue, entryID, maxCharCount, text);
        }

        TextEntry(AControl parent)
            : base(parent)
        {
            base.HandlesMouseInput = true;
            base.HandlesKeyboardFocus = true;
            IsEditable = true;
        }

        void BuildGumpling(int x, int y, int width, int height, int hue, int entryID, int maxCharCount, string text)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);
            Hue = hue;
            EntryID = entryID;
            Text = text == null ? string.Empty : text;
            MaxCharCount = maxCharCount;
            m_CaratBlinkOn = false;
            m_RenderedText = new RenderedText(string.Empty, 2048, true);
            m_RenderedCarat = new RenderedText(string.Empty, 16, true);
        }

        // ============================================================================================================
        // Update and Draw
        // ============================================================================================================

        public override void Update(double totalMS, double frameMS)
        {
            if (UserInterface.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                if (!m_IsFocused)
                {
                    m_IsFocused = true;
                    m_CaratBlinkOn = true;
                    m_MSSinceLastCaratBlink = 0f;
                }
                // if we're using the legacy carat, keep it visible. Else blink it every x seconds.
                if (LegacyCarat)
                {
                    m_CaratBlinkOn = true;
                }
                else
                {
                    m_MSSinceLastCaratBlink += (float)frameMS;
                    if (m_MSSinceLastCaratBlink >= MSBetweenCaratBlinks)
                    {
                        m_MSSinceLastCaratBlink -= MSBetweenCaratBlinks;
                        if (m_CaratBlinkOn == true)
                        {
                            m_CaratBlinkOn = false;
                        }
                        else
                        {
                            m_CaratBlinkOn = true;
                        }
                    }
                }
            }
            else
            {
                m_IsFocused = false;
                m_CaratBlinkOn = false;
            }
            string text = Text == null ? null : (IsPasswordField ? new string('*', Text.Length) : Text);
            m_RenderedText.Text = $"{LeadingHtmlTag}{LeadingText}{text}";
            m_RenderedCarat.Text = $"{LeadingHtmlTag}{(LegacyCarat ? "_" : "|")}";
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            Point caratPosition = new Point(position.X, position.Y);
            if (IsEditable)
            {
                if (m_RenderedText.Width + m_RenderedCarat.Width <= Width)
                {
                    m_RenderedText.Draw(spriteBatch, position, Utility.GetHueVector(Hue));
                    caratPosition.X += m_RenderedText.Width;
                }
                else
                {
                    int textOffset = m_RenderedText.Width - (Width - m_RenderedCarat.Width);
                    m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, m_RenderedText.Width - textOffset, m_RenderedText.Height), textOffset, 0, Utility.GetHueVector(Hue));
                    caratPosition.X += (Width - m_RenderedCarat.Width);
                }
            }
            else
            {
                caratPosition.X = 0;
                m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, Width, Height), 0, 0, Utility.GetHueVector(Hue));
            }

            if (m_CaratBlinkOn)
            {
                m_RenderedCarat.Draw(spriteBatch, caratPosition, Utility.GetHueVector(Hue));
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        // ============================================================================================================
        // Input
        // ============================================================================================================

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Tab:
                    Parent.KeyboardTabToNextFocus(this);
                    break;
                case WinKeys.Enter:
                    Parent.OnKeyboardReturn(EntryID, Text);
                    break;
                case WinKeys.Back:
                    if (ReplaceDefaultTextOnFirstKeypress)
                    {
                        Text = string.Empty;
                        ReplaceDefaultTextOnFirstKeypress = false;
                    }
                    else if (!string.IsNullOrEmpty(Text))
                    {
                        int escapedLength;
                        if (EscapeCharacters.TryFindEscapeCharacterBackwards(Text, Text.Length - 1, out escapedLength))
                        {
                            Text = Text.Substring(0, Text.Length - escapedLength);
                        }
                        else
                        {
                            Text = Text.Substring(0, Text.Length - 1);
                        }
                    }
                    break;
                default:
                    // place a char, so long as it's within the char count limit.
                    if (MaxCharCount != 0 && Text.Length >= MaxCharCount)
                    {
                        return;
                    }
                    if (NumericOnly && !char.IsNumber(e.KeyChar))
                    {
                        return;
                    }
                    if (ReplaceDefaultTextOnFirstKeypress)
                    {
                        Text = string.Empty;
                        ReplaceDefaultTextOnFirstKeypress = false;
                    }
                    if (e.IsChar && e.KeyChar >= 32)
                    {
                        string escapedCharacter;
                        if (EscapeCharacters.TryMatchChar(e.KeyChar, out escapedCharacter))
                        {
                            Text += escapedCharacter;
                        }
                        else
                        {
                            Text += e.KeyChar;
                        }
                    }
                    break;
            }
        }
    }
}