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
using System;
using Microsoft.Xna.Framework;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Core.UI;
using JuicyUO.Core.UI.HTML;
using JuicyUO.Core.Windows;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    class TextEntryPage : AControl
    {
        const float MSBetweenCaratBlinks = 500f;
        
        string m_Text;
        bool m_IsFocused;
        bool m_CaratBlinkOn;
        float m_MSSinceLastCaratBlink;
        readonly RenderedText m_RenderedText;
        RenderedText m_RenderedCarat;
        int m_CaratAt;
        int? m_CaratKeyUpDownX;
        Action<int, string> m_OnPageOverflow;
        Action<int> m_OnPageUnderflow;
        Action<int> m_OnPreviousPage;
        Action<int> m_OnNextPage;

        public int PageIndex;
        public string LeadingHtmlTag;
        public string Text
        {
            get
            {
                return (m_Text == null) ? string.Empty : m_Text;
            }
            set
            {
                if (m_Text != value)
                {
                    m_Text = value;
                    m_RenderedText.Text = $"{LeadingHtmlTag}{Text}";
                }
            }
        }

        public string TextWithLineBreaks
        {
            get
            {
                if (string.IsNullOrEmpty(m_Text))
                {
                    return string.Empty;
                }
                return m_RenderedText.Document.TextWithLineBreaks;
            }
        }

        public int CaratAt
        {
            get
            {
                if (m_CaratAt < 0)
                    m_CaratAt = 0;
                if (Text != null && m_CaratAt > Text.Length)
                    m_CaratAt = Text.Length;
                return m_CaratAt;
            }
            set
            {
                m_CaratAt = value;
                if (m_CaratAt < 0)
                    m_CaratAt = 0;
                if (m_CaratAt > Text.Length)
                    m_CaratAt = Text.Length;
            }
        }

        public override bool HandlesMouseInput => base.HandlesMouseInput & IsEditable;
        public override bool HandlesKeyboardFocus => base.HandlesKeyboardFocus & IsEditable;

        // ============================================================================================================
        // Ctors and BuildGumpling Methods
        // ============================================================================================================

        public TextEntryPage(AControl parent, int x, int y, int width, int height, int pageIndex)
            : base(parent)
        {
            base.HandlesMouseInput = true;
            base.HandlesKeyboardFocus = true;
            IsEditable = true;
            Position = new Point(x, y);
            Size = new Point(width, height);
            PageIndex = pageIndex;
            m_CaratBlinkOn = false;
            m_RenderedText = new RenderedText(string.Empty, Width, true);
            m_RenderedCarat = new RenderedText(string.Empty, 16, true);
        }

        public void SetMaxLines(int maxLines, Action<int, string> onPageOverflow, Action<int> onPageUnderflow)
        {
            m_RenderedText.Document.SetMaxLines(maxLines, OnDocumentSplitPage);
            m_OnPageOverflow = onPageOverflow;
            m_OnPageUnderflow = onPageUnderflow;
        }

        public void SetKeyboardPageControls(Action<int> onPrevious, Action<int> onNext)
        {
            m_OnPreviousPage = onPrevious;
            m_OnNextPage = onNext;
        }

        void OnDocumentSplitPage(int index)
        {
            string overflowText = Text.Substring(index);
            m_Text = Text.Substring(0, index);
            m_OnPageOverflow?.Invoke(PageIndex, overflowText);
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
            else
            {
                m_IsFocused = false;
                m_CaratBlinkOn = false;
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            m_RenderedText.Draw(spriteBatch, new Rectangle(position.X, position.Y, Width, Height), 0, 0);
            if (IsEditable)
            {
                m_RenderedCarat.Text = $"{LeadingHtmlTag}|";
                m_RenderedText.Draw(spriteBatch, position);
                if (m_CaratBlinkOn)
                {
                    Point caratPosition = m_RenderedText.Document.GetCaratPositionByIndex(m_CaratAt);
                    caratPosition.X += position.X;
                    caratPosition.Y += position.Y;
                    m_RenderedCarat.Draw(spriteBatch, caratPosition);
                }
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        void SetBlinkOn()
        {
            m_CaratBlinkOn = true;
            m_MSSinceLastCaratBlink = 0;
        }

        // ============================================================================================================
        // Text Editing Functions
        // ============================================================================================================

        public void InsertCharacter(int index, char ch)
        {
            string escapedCharacter;
            int caratAt = index;
            string text = Text;
            if (EscapeCharacters.TryMatchChar(ch, out escapedCharacter))
            {
                text = text.Insert(CaratAt, escapedCharacter);
                caratAt += escapedCharacter.Length;
            }
            else
            {
                text = text.Insert(CaratAt, ch.ToString());
                caratAt += 1;
            }
            Text = text;
            CaratAt = caratAt;
        }

        public void RemoveCharacter(int index)
        {
            int escapedLength;
            if (EscapeCharacters.TryFindEscapeCharacterBackwards(Text, index, out escapedLength))
            {
                Text = Text.Substring(0, Text.Length - escapedLength);
                int carat = index - 1;
                string before = (index == 0) ? null : Text.Substring(0, index - 1);
                string after = Text.Substring(index);
                Text = before + after;
                CaratAt = carat;
            }
            else
            {
                int carat = index - 1;
                string before = (index == 0) ? null : Text.Substring(0, index - 1);
                string after = Text.Substring(index);
                Text = before + after;
                CaratAt = carat;
            }
        }

        // ============================================================================================================
        // Input
        // ============================================================================================================

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            if (e.KeyCode == WinKeys.Up || e.KeyCode == WinKeys.Down)
            {
                Point current = m_RenderedText.Document.GetCaratPositionByIndex(CaratAt);
                if (m_CaratKeyUpDownX == null)
                {
                    m_CaratKeyUpDownX = current.X;
                }
                Point next = new Point(m_CaratKeyUpDownX.Value, current.Y + (e.KeyCode == WinKeys.Up ? -18 : 18));
                int carat = m_RenderedText.Document.GetCaratIndexByPosition(next);
                if (carat != -1)
                {
                    CaratAt = carat;
                }
            }
            else
            {
                m_CaratKeyUpDownX = null;
                switch (e.KeyCode)
                {
                    case WinKeys.Tab:
                        Parent.KeyboardTabToNextFocus(this);
                        break;
                    case WinKeys.Enter:
                        InsertCharacter(CaratAt, '\n');
                        break;
                    case WinKeys.Back:
                        if (CaratAt == 0)
                        {
                            m_OnPageUnderflow.Invoke(PageIndex);
                        }
                        else
                        {
                            RemoveCharacter(CaratAt);
                        }
                        break;
                    case WinKeys.Left:
                        if (CaratAt == 0)
                        {
                            m_OnPreviousPage?.Invoke(PageIndex);
                        }
                        else
                        {
                            CaratAt = CaratAt - 1;
                        }
                        break;
                    case WinKeys.Right:
                        if (CaratAt == Text.Length)
                        {
                            m_OnNextPage?.Invoke(PageIndex);
                        }
                        else
                        {
                            CaratAt = CaratAt + 1;
                        }
                        break;
                    default:
                        if (e.IsChar && e.KeyChar >= 32)
                        {
                            InsertCharacter(CaratAt, e.KeyChar);
                        }
                        break;
                }
            }
            SetBlinkOn();
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                int carat = m_RenderedText.Document.GetCaratIndexByPosition(new Point(x, y));
                if (carat != -1)
                {
                    CaratAt = carat;
                    SetBlinkOn();
                    return;
                }
            }
            base.OnMouseClick(x, y, button);
        }
    }
}